using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL
{
    public class ResourceWebRequester : ResourceAsyncOperation
    {
        /// <summary>
        /// 网络请求缓存队列
        /// </summary>
        static Queue<ResourceWebRequester> pool = new Queue<ResourceWebRequester>();
        static int sequence = 0;

        /// <summary>
        /// 获取一个Requester
        /// </summary>
        public static ResourceWebRequester Get()
        {
            if (pool.Count > 0)
                return pool.Dequeue();
            else
                return new ResourceWebRequester(++sequence);
        }

        /// <summary>
        /// 回收Requester
        /// </summary>
        public static void Recycle(ResourceWebRequester creater)
        {
            pool.Enqueue(creater);
        }

        protected UnityWebRequest request = null;
        protected bool isOver = false;

        public int Sequence
        {
            get;
            protected set;
        }

        public bool noCache
        {
            get;
            protected set;
        }

        public string assetbundleName
        {
            get;
            protected set;
        }

        public string url
        {
            get;
            protected set;
        }

        public int timeout
        {
            get;
            protected set;
        }

        public byte[] bytes
        {
            get
            {
                return request.downloadHandler.data;
            }
        }

        public string text
        {
            get
            {
                return request.downloadHandler.text;
            }
        }

        public string error
        {
            get
            {
                return string.IsNullOrEmpty(request.error) ? null : request.error;
            }
        }

        public ResourceWebRequester(int sequence)
        {
            Sequence = sequence;
        }

        public void Init(string name, string url, int timeout = 0, bool noCache = false)
        {
            assetbundleName = name;
            this.url = url;
            this.timeout = timeout;
            this.noCache = noCache;
            request = null;
            isOver = false;
        }

        public void Start()
        {
            request = new UnityWebRequest(url);
            if (request == null)
            {
                GLog.Error("New UnityWebRequest failed!!!");
                isOver = true;
            }
            else
            {
                request.timeout = timeout;
                request.SendWebRequest();
            }
        }

        public override bool IsDone()
        {
            return isOver;
        }

        public override float Progress()
        {
            if (isDone)
            {
                return 1.0f;
            }

            return request != null ? request.downloadProgress : 0f;
        }

        public override void Update()
        {
            if (isDone)
                return;

            isOver = request != null && (request.isDone || !string.IsNullOrEmpty(request.error));
            if (!isOver)
                return;

            if (request != null && !string.IsNullOrEmpty(request.error))
            {
                GLog.Error(request.error);
                GLog.Error(request.url);
            }
        }

        public override void Dispose()
        {
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
            Recycle(this);
        }
    }
}
