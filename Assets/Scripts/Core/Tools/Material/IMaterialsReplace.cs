using System.Collections.Generic;
using UnityEngine;

namespace GameDLL.Tools
{

    public enum RendererFilter
    {
        /// <summary>
        /// 自身对象
        /// </summary>
        SELF,
        /// <summary>
        /// 子节点下对象
        /// </summary>
        CHILD,
        /// <summary>
        /// 自身加子节点下对象
        /// </summary>
        SELF_CHILD,
        /// <summary>
        /// 所有子节点对象
        /// </summary>
        CHILDREN,
        /// <summary>
        /// 自身及所有子节点对象
        /// </summary>
        SELF_CHILDREN,
    }

    public enum ChangeQueueMode
    {
        DO_NOTHING,
        PLUS_VALUE,
        SET_VALUE,
    }

    public enum ChangeDataMode
    {
        DATA_ONLY,
        DATA_SHADER,
        DATA_MATERIAL,
        DATA_UNIFIED_SHADER,
        DATA_UNIFIED_MATERIAL,
        DATA_UNIFIED_SHADER_REVERSE,
        DATA_UNIFIED_MATERIAL_REVERSE,
        DATA_UNIFIED_MATERIAL_ADD,
        DATA_UNIFIED_MATERIAL_REMOVE,
    }


    public abstract class IMaterialsReplace : MonoBehaviour
    {
        [SerializeField]
        protected RendererFilter _filter = RendererFilter.CHILD;
        public RendererFilter filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        [SerializeField]
        protected bool _includeInactive = true;
        public bool includeInactive
        {
            get
            {
                return _includeInactive;
            }
        }

        /// <summary>
        /// 缓存Renderer对象
        /// </summary>
        protected Renderer[] _renderers;

        protected MaterialPropertyBlock mpb;

        protected bool _init;

        void Awake()
        {
            Init();
        }

        protected void Init()
        {
            if (!_init)
            {
                if (mpb == null)
                    mpb = new MaterialPropertyBlock();
                GetRenderers();
                _init = true;
                SetInit();
            }
        }

        protected void GetRenderers()
        {
            if (_renderers != null && _renderers.Length > 0)
                return;
            List<Renderer> list = new List<Renderer>();
            Renderer renderer;
            switch (_filter)
            {
                case RendererFilter.SELF:
                    {
                        renderer = GetComponent<Renderer>();
                        if (renderer)
                            list.Add(renderer);
                    }
                    break;
                case RendererFilter.CHILD:
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            renderer = transform.GetChild(i).GetComponent<Renderer>();
                            if (renderer)
                                list.Add(renderer);
                        }
                    }
                    break;
                case RendererFilter.SELF_CHILD:
                    {
                        renderer = GetComponent<Renderer>();
                        if (renderer)
                            list.Add(renderer);
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            renderer = transform.GetChild(i).GetComponent<Renderer>();
                            if (renderer)
                                list.Add(renderer);
                        }
                    }
                    break;
                case RendererFilter.CHILDREN:
                    {
                        transform.GetComponentsInChildren(_includeInactive, list);
                        renderer = GetComponent<Renderer>();
                        if (renderer)
                            list.Remove(renderer);
                    }
                    break;
                case RendererFilter.SELF_CHILDREN:
                    {
                        transform.GetComponentsInChildren(_includeInactive, list);
                    }
                    break;
            }
            _renderers = list.ToArray();
        }

        protected abstract void SetInit();


    }
}
