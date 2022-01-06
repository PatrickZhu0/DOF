using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDLL
{
    /// <summary>
    /// AssetBundle信息文件
    /// </summary>
    public class AssetBundleInfo
    {
        private string _bundleName;      //bundle名称

        private AssetBundle _bundle;      //bundle资源

        private Dictionary<string, Object> _assetDic;    //资源名及资源对象

        private int _length = 0;        //bundle文件中资源总数

        public AssetBundle bundle
        {
            get
            {
                return _bundle;
            }
        }

        public string bundleName
        {
            get
            {
                return _bundleName;
            }
        }

        public AssetBundleInfo(string name, AssetBundle ab)
        {
            _bundleName = name;
            _bundle = ab;
            if (null != _bundle) _length = _bundle.GetAllAssetNames().Length;
        }

        /// <summary>
        /// bundle资源是否已被释放
        /// </summary>
        public bool IsUnloadBundle()
        {
            return _bundle == null;
        }

        /// <summary>
        /// bundle资源是否已全部读取
        /// </summary>
        public bool IsAllAssetLoaded()
        {
            if (null != _assetDic) return _assetDic.Count == _length;
            return false;
        }

        /// <summary>
        /// 通过Type加载资源，根据isCacheAsset缓存读取的资源
        /// </summary>
        public Object LoadAsset(System.Type type, string assetName, bool isCacheAsset = false)
        {
            Object asset = null;
            string key = assetName.ToLower();
            if (isCacheAsset)
            {
                if (null == _assetDic) _assetDic = new Dictionary<string, Object>();
                if (!_assetDic.TryGetValue(key, out asset))
                {
                    if (null == _bundle) return null;
                    asset = _bundle.LoadAsset(assetName, type);
                    if (asset)
                        _assetDic[key] = asset;

                    if (IsAllAssetLoaded() && Application.isPlaying) //所有资源都缓存则直接释放文件文本
                        ILoop.CreateCoroutine(UnloadBundle());
                }
            }
            else
            {
                if(null != _bundle)
                    asset = _bundle.LoadAsset(assetName, type);
                else if (null != _assetDic)
                    _assetDic.TryGetValue(key, out asset);
            }
            return asset;
        }

        /// <summary>
        /// 异步加载资源，assetIn返回资源，根据isCacheAsset缓存读取的资源
        /// by Rick, 增加abName，方便统计
        /// </summary>
        public IEnumerator LoadAssetAsync(string assetName, string abName, AssetBundleData assetIn, bool isCacheAsset = false, ResourceManager.LoadAsyncComplete complete = null, ResourceManager.LoadAsyncPreComplete preComplete = null)
        {
            Object asset = null;
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.asset = asset;
            string key = assetName.ToLower();
            AssetBundleRequest request;
            if (isCacheAsset)
            {
                if (null == _assetDic) _assetDic = new Dictionary<string, Object>();
                if (!_assetDic.TryGetValue(key, out asset))
                {
                    if (null != _bundle)
                    {
                        request = _bundle.LoadAssetAsync(assetName, assetIn.type);

                        while (!request.isDone)
                            yield return null;

                        asset = request.asset;
                        if (asset)
                            _assetDic[key] = asset;
                        assetIn.asset = asset;

                        if (IsAllAssetLoaded() && Application.isPlaying) //所有资源都缓存则直接释放文件文本
                            yield return UnloadBundle();
                    }
                }
                else
                    assetIn.asset = asset;
            }
            else
            {
                if (null != _bundle)
                {
                    request = _bundle.LoadAssetAsync(assetName, assetIn.type);

                    while (!request.isDone)
                        yield return null;
                    asset = request.asset;
                    assetIn.asset = asset;
                }
                else
                {
                    if (null != _assetDic)
                        _assetDic.TryGetValue(key, out asset);
                }
            }

            if (preComplete != null)
                preComplete(assetName, abName, asset);

            if (complete != null)
                complete(asset);
        }

        /// <summary>
        /// 释放资源文件本身及未加载的内容
        /// </summary>
        public IEnumerator UnloadBundle()
        {
            yield return new WaitForEndOfFrame();
            if (null != _bundle)
            {
                _bundle.Unload(false);
                _bundle = null;
            }
        }

        /// <summary>
        /// 释放资源，unloadAllLoadedObjects用于是否要释放全部引用Assets内存
        /// </summary>
        public void UnloadAll(bool unloadAllLoadedObjects)
        {
            if (null != _assetDic)
            {
                foreach (KeyValuePair<string, Object> pair in _assetDic)
                {
                    //释放掉Dictionary中的资源
                    if (pair.Value is GameObject)       //跳过GameObject等预设
                        continue;
                    Resources.UnloadAsset(pair.Value);
                }
                _assetDic.Clear();
                _assetDic = null;
            }
            if (null != _bundle)
            {
                //视实际情况是否释放全部的内存
                _bundle.Unload(unloadAllLoadedObjects);
                _bundle = null;
            }
        }
    }
}