using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameDLL
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly Action<T> _actionOnCreate;
        private readonly Func<T, bool> _actionOnGet;
        private readonly Func<T, bool> _actionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return _stack.Count; } }

        public ObjectPool(Action<T> actionOnCreate = null, Func<T, bool> actionOnGet = null, Func<T, bool> actionOnRelease = null)
        {
            _actionOnCreate = actionOnCreate;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
        }

        public ObjectPool(int count, Action<T> actionOnCreate = null, Func<T, bool> actionOnGet = null, Func<T, bool> actionOnRelease = null) : this(actionOnCreate, actionOnGet, actionOnRelease)
        {
            for (int i = 0; i < count; i++)
                create();

        }

        public bool Contains(T element)
        {
            return _stack.Contains(element);
        }

        T create()
        {
            T element = new T();
            if (_actionOnCreate != null)
                _actionOnCreate(element);
            countAll++;
            return element;
        }

        public T Get()
        {
            T element;
            while(_stack.Count > 0)
            {
                element = _stack.Pop();
                if (element != null)
                {
                    if (_actionOnGet != null)
                    {
                        if (!_actionOnGet(element))
                        {
                            countAll--;
                            continue;
                        }
                    }
                    return element;
                }
            }
            element = create();
            if (_actionOnGet != null)
                _actionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
                GLog.Error("Internal error. Trying to destroy object that is already released to pool.");
            if (_actionOnRelease != null)
            {
                if (!_actionOnRelease(element))
                    return;
            }
            _stack.Push(element);
        }
    }
}