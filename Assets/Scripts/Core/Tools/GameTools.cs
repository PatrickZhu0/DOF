using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameDLL
{
    public static class GameTools
    {
        #region Object 扩展
        /// <summary>
        /// 检测Object是否为null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNull(Object o)
        {
            return o == null;
        }

        /// <summary>
        /// 检测Object是否不为null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNotNull(Object o)
        {
            return o != null;
        }

        public static void DestroyObject(Object o, float time = 0)
        {
            if (o)
            {
                //如果传递过来的是Transform则直接删除物体
                Transform trans = o as Transform;
                if (trans)
                    o = trans.gameObject;

                //by Rick,添加清理入口
                if (applicationRunning)
                {
                    ResourceManager.Instance.PreDestroyObject(o);
                }
                //~by Rick
                //#if UNITY_EDITOR
                if (!Application.isPlaying)
                    GameObject.DestroyImmediate(o);
                else
                //#endif
                if (applicationRunning)     //by Rick, 防止杀进程时删除报错
                {
                    if (time > 0)
                        GameObject.Destroy(o, time);
                    else if (time == 0)
                        GameObject.DestroyImmediate(o); //by Rick, 立即删除，防止子物体链接没有断开，删除父物体时检测报错
                    else
                        GameObject.Destroy(o);      //运行时有些情况无法直接DestroyImmediate物体，可以使用这个销毁
                }
            }
        }

        public static void DestroyImmediate(Object obj)
        {
            if (obj != null)
            {
                if (Application.isEditor) Object.DestroyImmediate(obj);
                else Object.Destroy(obj);
            }
        }
        #endregion

        #region Scene 扩展
        /// <summary>
        /// 获取所有的场景
        /// </summary>
        public static Scene[] LoadedScene()
        {
            int count = SceneManager.sceneCount;
            Scene[] scenes = new Scene[count];
            for (int i = 0; i < count; i++)
                scenes[i] = SceneManager.GetSceneAt(i);
            return scenes;
        }

        public static List<GameObject> GetAllRootGameObjects()
        {
            List<GameObject> objects = new List<GameObject>();
            Scene[] scenes = LoadedScene();
            for (int i = 0; i < scenes.Length; i++)
                scenes[i].GetRootGameObjects(objects);
            return objects;
        }

        public static List<Transform> GetAllRootTransforms()
        {
            List<GameObject> objects = GetAllRootGameObjects();
            List<Transform> transforms = new List<Transform>();
            for (int i = 0; i < objects.Count; i++)
                transforms.Add(objects[i].transform);
            return transforms;
        }
        #endregion

        #region GameObject 扩展

        public static bool ActiveInHierarchy(GameObject gameobject)
        {
            if (gameobject)
                return gameobject.activeInHierarchy;
            return false;
        }

        /// <summary>
        /// 查找GameObject下指定path的GameObject
        /// </summary>
        /// <param name="gameObject">父物体</param>
        /// <param name="path">相对路径</param>
        public static GameObject FindGameObject(this GameObject gameObject, string path)
        {
            if (gameObject)
            {
                Transform trans = gameObject.transform.Find(path);
                if (trans)
                    return trans.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 重置物体的父节点并重置本地参数
        /// </summary>
        public static void ResetParent(this GameObject gameObject, Transform parent, bool setScale = true)
        {
            if (gameObject)
                gameObject.transform.ResetParent(parent, setScale);
        }

        /// <summary>
        /// 设置绑定父级并设定layer
        /// </summary>
        public static void SetParentAndLayer(this GameObject gameObject, Transform parent, int layer)
        {
            Transform transform = gameObject.transform;
            transform.ResetParent(parent);
            gameObject.SetLayer(parent, layer);
        }

        /// <summary>
        /// 设置绑定父级
        /// </summary>
        public static void SetParent(this GameObject child, Transform parent, bool worldPositionStays)
        {
            if (child)
                child.transform.SetParent(parent, worldPositionStays);
        }

        /// <summary>
        /// 设置绑定父级
        /// </summary>
        public static void SetParent(this GameObject child, Transform parent, float ox = 0f, float oy = 0f, float oz = 0f, float size = 1f)
        {
            if (child)
                child.transform.SetParent(parent, ox, oy, oz, size);
        }

        /// <summary>
        /// 设置绑定父级，并给对象名称赋值
        /// </summary>
        public static void SetParent(this GameObject child, string name, Transform parent, float ox = 0f, float oy = 0f, float oz = 0f, float size = 1f)
        {
            if (child)
            {
                child.name = name;
                child.transform.SetParent(parent, ox, oy, oz, size);
            }
        }

        /// <summary>
        /// 绑定父节点，并保持原有的本地参数
        /// </summary>
        public static void SetParentStayLocalData(this GameObject child, Transform parent)
        {
            if (child)
                child.transform.SetParentStayLocalData(parent);
        }

        /// <summary>
        /// 重置物体的本地坐标旋转及缩放
        /// </summary>
        public static void ResetLocalTransform(this GameObject gameObject, float scale = 1f)
        {
            if (gameObject)
                ResetLocalTransform(gameObject.transform, scale);
        }

        /// <summary>
        /// 批量激活物体
        /// </summary>
        public static void SetGameObjectsActive(this GameObject[] objects, bool value)
        {
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                    objects[i].SetActive(value);
            }
        }

        public static void SetLayer(this GameObject gameObject, Transform parent, int layer)
        {
            if (layer > -1 && layer < 32) gameObject.SetLayer(layer);
            else if (layer == -1 && parent) gameObject.SetLayer(parent.gameObject.layer);
        }

        public static void SetRenderersLayer(this GameObject gameObject, int layer, bool includeInactive = true)
        {
            gameObject.SetComponentsLayer(typeof(Renderer), layer, includeInactive);
        }

        public static void SetRenderersAndSelfLayer(this GameObject gameObject, int layer, bool includeInactive = true)
        {
            gameObject.layer = layer;
            gameObject.SetComponentsLayer(typeof(Renderer), layer, includeInactive);
        }

        public static void SetComponentsLayer<T>(this GameObject gameObject, int layer, bool includeInactive = true) where T : Component
        {
            gameObject.SetComponentsLayer(typeof(T), layer, includeInactive);
        }

        public static void SetComponentsLayer(this GameObject gameObject, Type type, int layer, bool includeInactive = true)
        {
            Component[] components = gameObject.GetComponentsInChildren(type, includeInactive);
            for (int i = 0; i < components.Length; i++)
                components[i].gameObject.layer = layer;
        }

        /// <summary>
        /// 设置物体的layer
        /// </summary>
        public static void SetLayer(this GameObject gameObject, string layerName, bool includeChildren = true, bool includeInactive = true)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer >= 0)
            {
                if (includeChildren)
                    gameObject.SetLayer(layer, includeChildren);
                else
                    gameObject.layer = layer;
            }
        }

        /// <summary>
        /// 设置物体的layer
        /// </summary>
        public static void SetLayer(this GameObject gameObject, int layer, bool includeChildren = true, bool includeInactive = true)
        {
            if (includeChildren)
            {
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
                for (int i = 0; i < transforms.Length; i++)
                    transforms[i].gameObject.layer = layer;
            }
            else
                gameObject.layer = layer;
        }

        /// <summary>
        /// 批量设置物体的layer
        /// </summary>
        public static void SetGameObjectsLayer(this GameObject[] objects, int layer, bool includeChildren)
        {
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                    objects[i].SetLayer(layer, includeChildren);
            }
        }

        /// <summary>
        /// 根节点创建一个特定层级的物体
        /// </summary>
        public static GameObject CreateObject(bool normalize = true, int layer = -1, bool undo = false)
        {
            GameObject go = new GameObject();
            //#if UNITY_EDITOR
            if (undo)
                RegisterCreatedObjectUndo(go, "Create Object");
            //#endif
            if (layer != -1)
            {
                if (normalize)
                    go.SetParentAndLayer(null, layer);
                else
                    go.SetLayer(null, layer);
            }
            return go;
        }

        /// <summary>
        /// 根节点创建一个物体
        /// </summary>
        public static GameObject CreateObject(string name, bool normalize = true, bool undo = false)
        {
            GameObject go = CreateObject(normalize, -1, undo);
            go.name = name;
            return go;
        }

        public static GameObject CreatePrefab(GameObject prefab, float ox = 0, float oy = 0, float oz = 0, float size = 1f)
        {
            if (prefab)
            {
                GameObject go = GameObject.Instantiate(prefab, new Vector3(ox, oy, oz), Quaternion.identity);
                go.name = prefab.name;
                go.transform.localScale = size * Vector3.one;
                return go;
            }
            return null;
        }

        public static GameObject CreatePrefab(GameObject prefab, int layer, float ox, float oy, float oz, float size)
        {
            if (prefab)
            {
                GameObject go = GameObject.Instantiate(prefab, new Vector3(ox, oy, oz), Quaternion.identity);
                go.name = prefab.name;
                go.transform.localScale = size * Vector3.one;
                return go;
            }
            return null;
        }

        /// <summary>
        /// 添加一个特定层级的物体
        /// </summary>
        public static GameObject AddChild(this GameObject parent, int layer = -1, bool undo = false)
        {
            if (parent == null)
                return null;
            GameObject go = new GameObject();
            //#if UNITY_EDITOR
            if (undo)
                RegisterCreatedObjectUndo(go, "Create Object");
            //#endif
            go.SetParentAndLayer(parent.transform, layer);
            return go;
        }

        /// <summary>
        /// 添加一个物体，并指定名称
        /// </summary>
        public static GameObject AddChild(this GameObject parent, string name, bool undo = false)
        {
            GameObject go = AddChild(parent, -1, undo);
            if (name != null)
                go.name = name;
            return go;
        }

        /// <summary>
        /// 通过预设创建一个特定层级的子物体
        /// </summary>
        public static GameObject AddChild(this GameObject parent, GameObject prefab, int layer, bool undo = false)
        {
            if (prefab == null)
                return null;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            //#if UNITY_EDITOR
            if (undo)
                RegisterCreatedObjectUndo(go, "Create Object");
            //#endif
            go.SetParentAndLayer(parent.transform, layer);
            return go;
        }

        /// <summary>
        /// 通过预设创建一个子物体
        /// </summary>
        public static GameObject AddChild(this GameObject parent, GameObject prefab, bool undo = false)
        {
            return AddChild(parent, prefab, -1, undo);
        }

        /// <summary>
        /// 通过预设创建一个子物体，并设置坐标
        /// </summary>
        public static GameObject AddChild(this GameObject parent, GameObject prefab, float x, float y, float z, bool undo = false)
        {
            return AddChild(parent, prefab, -1, x, y, z, undo);
        }

        /// <summary>
        /// 通过预设创建一个子物体层级，并设置坐标
        /// </summary>
        public static GameObject AddChild(this GameObject parent, GameObject prefab, int layer, float x, float y, float z, bool undo = false)
        {
            GameObject obj = AddChild(parent, prefab, layer, undo);
            obj.SetLocalPosition(x, y, z);
            return obj;
        }

        /// <summary>
        /// 查询一个子节点是否存在，不存在创建一个
        /// </summary>
        public static GameObject AddMissingChild(this GameObject parent, string name, int layer, bool undo = false)
        {
            if (parent)
                return AddMissingChild(parent.transform, name, layer, undo);
            else
                return AddMissingChild((Transform)null, name, layer, undo);

        }

        /// <summary>
        /// 查询一个子节点是否存在，不存在创建一个
        /// </summary>
        public static GameObject AddMissingChild(this GameObject parent, string name, bool undo = false)
        {
            return AddMissingChild(parent, name, -1, undo);
        }

        /// <summary>
        /// 添加一个带特定脚本物体
        /// </summary>
        public static Component AddChild(this GameObject parent, Type type, string name = null, bool undo = false)
        {
            return AddChild(parent.transform, type, name, undo);
        }

        /// <summary>
        /// 添加一个带特定脚本物体
        /// </summary>
        public static T AddChild<T>(this GameObject parent, string name = null, bool undo = false) where T : Component
        {
            Type type = typeof(T);
            return AddChild(parent, type, name, undo) as T;
        }

        /// <summary>
        /// 物体身上不存在脚本时添加脚本
        /// </summary>
        public static Component AddMissingComponent(this GameObject go, Type type, bool undo = false)
        {
            Component t = go.GetComponent(type);
            if (t == null)
            {
                //#if UNITY_EDITOR
                if (undo)
                    RegisterCreatedObjectUndo(go, "Create Component");
                //#endif
                t = go.AddComponent(type);
            }
            return t;
        }

        /// <summary>
        /// 物体身上不存在脚本时添加脚本
        /// </summary>
        public static T AddMissingComponent<T>(this GameObject go, bool undo = false) where T : Component
        {
            return AddMissingComponent(go, typeof(T), undo) as T;
        }

        /// <summary>
        /// 物体子节点不存在脚本时添加脚本
        /// </summary>
        public static Component AddMissingChildComponent(this GameObject parent, Type type, string path, bool undo = false)
        {
            GameObject go = AddMissingChild(parent, path, undo);
            return AddMissingComponent(go, type, undo);
        }

        /// <summary>
        /// 物体子节点不存在脚本时添加脚本
        /// </summary>
        public static T AddMissingChildComponent<T>(this GameObject parent, string name = null, bool undo = false) where T : Component
        {
            return AddMissingChildComponent(parent, typeof(T), name, undo) as T;
        }

        /// <summary>
        /// 获取物体上的Transform
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Transform FindTransform(GameObject parent, string path, bool includeInactive = false)
        {
            return FindTransform(parent == null ? null : parent.transform, path, includeInactive);
        }

        /// <summary>
        /// 获取物体上的GameObject
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static GameObject FindGameObject(GameObject parent, string path, bool includeInactive = false)
        {
            return FindGameObject(parent == null ? null : parent.transform, path, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component FindComponent(GameObject parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            return FindComponent(parent == null ? null : parent.transform, type, path, includeChildren, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T FindComponent<T>(GameObject parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            return FindComponent(parent, typeof(T), path, includeChildren, includeInactive) as T;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component[] FindComponents(GameObject parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            return FindComponents(parent == null ? null : parent.transform, type, path, includeChildren, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T[] FindComponents<T>(GameObject parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            return FindComponents<T>(parent, path, includeChildren, includeInactive);
        }

        /// <summary>
        /// 设置物体的本地坐标
        /// </summary>
        public static void SetLocalPosition(this GameObject gameObject, Vector3 position)
        {
            SetLocalPosition(gameObject.transform, position);
        }

        /// <summary>
        /// 设置物体的本地坐标
        /// </summary>
        public static void SetLocalPosition(this GameObject gameObject, float x = 0f, float y = 0f, float z = 0f)
        {
            SetLocalPosition(gameObject.transform, x, y, z);
        }

        public static void SetParentLocalPosition(this GameObject gameObject, Transform parent, float ox = 0, float oy = 0, float oz = 0)
        {
            SetParentLocalPosition(gameObject.transform, parent, ox, oy, oz);
        }

        /// <summary>
        /// 设置物体的单个轴向的本地坐标
        /// </summary>
        public static void SetLocalPositionAxis(this GameObject gameObject, int axis, float value = 0f)
        {
            SetLocalPositionAxis(gameObject.transform, axis, value);
        }

        /// <summary>
        /// 设置物体的全局坐标
        /// </summary>
        public static void SetPosition(this GameObject gameObject, Vector3 position)
        {
            SetPosition(gameObject.transform, position);
        }

        /// <summary>
        /// 设置物体的全局坐标
        /// </summary>
        public static void SetPosition(this GameObject gameObject, float x = 0f, float y = 0f, float z = 0f)
        {
            SetPosition(gameObject.transform, x, y, z);
        }

        public static void SetParentPosition(this GameObject gameObject, Transform parent, float x = 0f, float y = 0f, float z = 0f)
        {
            SetParentPosition(gameObject.transform, parent, x, y, z);
        }

        /// <summary>
        /// 设置物体的单个轴向的本地坐标
        /// </summary>
        public static void SetPositionAxis(this GameObject gameObject, int axis, float value = 0f)
        {
            SetPositionAxis(gameObject.transform, axis, value);
        }

        /// <summary>
        /// 设置物体的本地旋转角度
        /// </summary>
        public static void SetLocalEuler(this GameObject gameObject, Vector3 eulerAngles)
        {
            SetLocalEuler(gameObject.transform, eulerAngles);
        }

        /// <summary>
        /// 设置物体的本地旋转角度
        /// </summary>
        public static void SetLocalEuler(this GameObject gameObject, float x = 0f, float y = 0f, float z = 0f)
        {
            SetLocalEuler(gameObject.transform, x, y, z);
        }

        /// <summary>
        /// 设置物体的本地旋转角度单个轴向
        /// </summary>
        public static void SetLocalEulerAxis(this GameObject gameObject, int axis, float value = 0f)
        {
            SetLocalEulerAxis(gameObject.transform, axis, value);
        }

        /// <summary>
        /// 设置物体的全局旋转角度
        /// </summary>
        public static void SetEuler(this GameObject gameObject, Vector3 eulerAngles)
        {
            SetEuler(gameObject.transform, eulerAngles);
        }

        /// <summary>
        /// 设置物体的全局旋转角度
        /// </summary>
        public static void SetEuler(this GameObject gameObject, float x = 0f, float y = 0f, float z = 0f)
        {
            SetEuler(gameObject.transform, x, y, z);
        }

        /// <summary>
        /// 设置物体的本地缩放
        /// </summary>
        public static void SetLocalScale(this GameObject gameObject, float x = 1f, float y = 1f, float z = 1f)
        {
            SetLocalScale(gameObject.transform, x, y, z);
        }

        /// <summary>
        /// 设置物体的统一本地缩放
        /// </summary>
        public static void SetLocalUniformScale(this GameObject gameObject, float scale = 1f)
        {
            SetLocalUniformScale(gameObject.transform, scale);
        }

        public static void SetScale(this GameObject gameObject, Vector3 scale)
        {
            SetScale(gameObject.transform, scale);
        }

        public static void SetScale(this GameObject gameObject, float ox, float oy, float oz)
        {
            SetScale(gameObject.transform, ox, oy, oz);
        }

        public static void SetScale(this GameObject gameObject, float scale)
        {
            SetScale(gameObject.transform, scale, scale, scale);
        }

        /// <summary>
        /// 物体保持全局的缩放比例挂载至指定父节点
        /// </summary>
        public static void SetParentAndScale(this GameObject gameObject, Transform parent, float scale)
        {
            if (gameObject)
                gameObject.transform.SetParentAndScale(parent, scale);
        }

        /// <summary>
        /// 物体保持全局的缩放比例以一定本地偏移挂载至指定父节点
        /// </summary>
        public static void SetParentLocalPosAndScale(this GameObject gameObject, Transform parent, float scale, float ox = 0, float oy = 0, float oz = 0)
        {
            if (gameObject)
                gameObject.transform.SetParentLocalPosAndScale(parent, scale, ox, oy, oz);
        }


        public static void ResetAllChildren(this GameObject gameObject)
        {
            if (gameObject)
                ResetAllChildren(gameObject.transform);
        }
        #endregion

        #region Transform 扩展

        public static void SetActive(this Transform transform, bool value)
        {
            transform.gameObject.SetActive(value);
        }

        /// <summary>
        /// 返回KeyValuePair<string, string>中Key为根节点路径，Value为对象名，如果为对象名为空，则表示是根节点对象
        /// </summary>
        public static KeyValuePair<string, string> SplitPath(string path)
        {
            string file = null;
            int index = path.IndexOf('/');
            if (index > 0)
            {
                file = path.Substring(index + 1);
                path = path.Substring(0, index);
            }
            return new KeyValuePair<string, string>(path, file);
        }

        public static Transform CreateTransform(this Transform parent, string path, int layer)
        {
            string[] paths = path.Split('/');
            Transform transfom = null;
            for (int i = 0; i < paths.Length; i++)
            {
                transfom = parent.Find(paths[i]);
                if (transfom)
                    parent = transfom;
                else
                    transfom = parent.AddChild(paths[i], layer).transform;
            }
            return transfom;
        }

        /// <summary>
        /// 通过路径查找物体
        /// </summary>
        /// <param name="parent">父节点物体，可以为空</param>
        /// <param name="path">相对或绝对路径</param>
        /// <param name="force">当父节点为空时，是否通过场景根节点强制查找物体</param>
        /// <param name="create">如果找不到物体，是否创建一个</param>
        /// <param name="layer">创建物体时设置layer</param>
        public static Transform FindAndCreate(this Transform parent, string path, bool force = false, bool create = false, int layer = -1, bool undo = false)
        {
            Transform transform = null;
            if (parent)
            {
                transform = parent.Find(path);
                if (transform == null && create)
                    transform = CreateTransform(parent, path, layer);
            }
            else
            {
                GameObject gameObject = GameObject.Find(path);
                if (gameObject)
                    transform = gameObject.transform;
                else if (force && !string.IsNullOrEmpty(path))
                {
                    List<GameObject> rootGameObjects = GetAllRootGameObjects();
                    KeyValuePair<string, string> pathInfo = SplitPath(path);
                    for (int i = 0; i < rootGameObjects.Count; i++)
                    {
                        if (pathInfo.Key == rootGameObjects[i].name)
                        {
                            if (string.IsNullOrEmpty(pathInfo.Value))
                                transform = rootGameObjects[i].transform;
                            else
                                transform = rootGameObjects[i].transform.Find(pathInfo.Value);
                            break;
                        }
                    }
                    if (transform == null && create)
                        transform = CreateTransform(parent, path, layer);
                }
            }
            return transform;
        }

        public static void ResetParent(this Transform transform, Transform parent, bool setScale = true)
        {
            if (transform)
            {
                transform.SetParent(parent);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                if (setScale)
                    transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 设置绑定父级
        /// </summary>
        public static void SetParent(this Transform child, Transform parent, float ox = 0f, float oy = 0f, float oz = 0f, float size = 1f)
        {
            child.SetParent(parent, false);
            child.localPosition = new Vector3(ox, oy, oz);
            child.localRotation = Quaternion.identity;
            if (size != 0)
                child.localScale = Vector3.one * size;
            else
                child.localScale = Vector3.zero;
        }

        /// <summary>
        /// 设置绑定父级，并给对象名称赋值
        /// </summary>
        public static void SetParent(this Transform child, string name, Transform parent, float ox = 0f, float oy = 0f, float oz = 0f, float size = 1f)
        {
            if (child)
            {
                child.name = name;
                child.SetParent(parent, ox, oy, oz, size);
            }
        }

        /// <summary>
        /// 绑定父节点，并保持原有的本地参数
        /// </summary>
        public static void SetParentStayLocalData(this Transform child, Transform parent)
        {
            if (child)
            {
                Vector3 localPosition = child.localPosition;
                Quaternion localRotation = child.localRotation;
                Vector3 localScale = child.localScale;
                child.SetParent(parent);
                child.localPosition = localPosition;
                child.localRotation = localRotation;
                child.localScale = localScale;
            }
        }

        /// <summary>
        /// 重置物体的本地坐标旋转及缩放
        /// </summary>
        public static void ResetLocalTransform(this Transform transform, float scale = 1f)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * scale;
        }

        public static void SetLayer(this Transform transform, string layerName, bool includeChildren = true, bool includeInactive = true)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer >= 0)
                transform.SetLayer(layer, includeChildren, includeInactive);
        }

        public static void SetLayer(this Transform transform, int layer, bool includeChildren = true, bool includeInactive = true)
        {
            if (includeChildren)
            {
                Transform[] transforms = transform.GetComponentsInChildren<Transform>(includeInactive);
                for (int i = 0; i < transforms.Length; i++)
                    transforms[i].gameObject.layer = layer;
            }
            else
                transform.gameObject.layer = layer;
        }

        public static void SetRenderersLayer(this Transform transform, int layer, bool includeInactive = true)
        {
            transform.SetComponentsLayer(typeof(Renderer), layer, includeInactive);
        }

        public static void SetRenderersAndSelfLayer(this Transform transform, int layer, bool includeInactive = true)
        {
            transform.gameObject.layer = layer;
            transform.SetComponentsLayer(typeof(Renderer), layer, includeInactive);
        }

        public static void SetComponentsLayer<T>(this Transform transform, int layer, bool includeInactive = true) where T : Component
        {
            transform.SetComponentsLayer(typeof(T), layer, includeInactive);
        }

        public static void SetComponentsLayer(this Transform transform, Type type, int layer, bool includeInactive = true)
        {
            Component[] components = transform.GetComponentsInChildren(type, includeInactive);
            for (int i = 0; i < components.Length; i++)
                components[i].gameObject.layer = layer;
        }

        /// <summary>
        /// 批量设置物体的layer
        /// </summary>
        public static void SetTransformsLayer(this Transform[] transforms, int layer, bool includeChildren, bool includeInactive = true)
        {
            if (transforms != null)
            {
                for (int i = 0; i < transforms.Length; i++)
                    transforms[i].SetLayer(layer, includeChildren, includeInactive);
            }
        }

        /// <summary>
        /// 添加一个特定层级的物体
        /// </summary>
        public static GameObject AddChild(this Transform parent, int layer, bool undo = false)
        {
            GameObject go = new GameObject();
            //#if UNITY_EDITOR
            if (undo)
                RegisterCreatedObjectUndo(go, "Create Object");
            //#endif
            go.SetParentAndLayer(parent, layer);
            return go;
        }

        /// <summary>
        /// 添加一个物体，并指定名称
        /// </summary>
        public static GameObject AddChild(this Transform parent, string name, bool undo = false)
        {
            GameObject go = AddChild(parent, -1, undo);
            if (name != null)
                go.name = name;
            return go;
        }

        /// <summary>
        /// 添加一个物体，并指定名称
        /// </summary>
        public static GameObject AddChild(this Transform parent, string name, int layer, bool undo = false)
        {
            GameObject go = AddChild(parent, layer, undo);
            if (name != null)
                go.name = name;
            return go;
        }

        /// <summary>
        /// 添加一个物体
        /// </summary>
        public static GameObject AddChild(this Transform parent, bool undo = false)
        {
            return AddChild(parent, -1, undo);
        }

        /// <summary>
        /// 通过预设创建一个特定层级的子物体
        /// </summary>
        public static GameObject AddChild(this Transform parent, GameObject prefab, int layer, bool undo = false)
        {
            if (prefab == null)
                return null;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            //#if UNITY_EDITOR
            if (undo)
                RegisterCreatedObjectUndo(go, "Create Object");
            //#endif
            go.SetParentAndLayer(parent, layer);
            return go;
        }

        /// <summary>
        /// 通过预设创建一个子物体
        /// </summary>
        public static GameObject AddChild(this Transform parent, GameObject prefab, bool undo = false)
        {
            return AddChild(parent, prefab, -1, undo);
        }

        /// <summary>
        /// 通过预设创建一个子物体，并设置坐标
        /// </summary>
        public static GameObject AddChild(this Transform parent, GameObject prefab, float x, float y, float z, bool undo = false)
        {
            return AddChild(parent, prefab, -1, x, y, z, undo);
        }

        /// <summary>
        /// 通过预设创建一个子物体，并设置坐标
        /// </summary>
        public static GameObject AddChild(this Transform parent, GameObject prefab, int layer, float x, float y, float z, bool undo = false)
        {
            GameObject obj = AddChild(parent, prefab, layer, undo);
            obj.SetLocalPosition(x, y, z);
            return obj;
        }

        /// <summary>
        /// 查询一个子节点是否存在，不存在创建一个
        /// </summary>
        public static GameObject AddMissingChild(this Transform parent, string path, int layer, bool undo = false)
        {
            return FindAndCreate(parent, path, true, true, layer, undo).gameObject;
        }

        /// <summary>
        /// 查询一个子节点是否存在，不存在创建一个
        /// </summary>
        public static GameObject AddMissingChild(this Transform parent, string path, bool undo = false)
        {
            return AddMissingChild(parent, path, -1, undo);
        }

        static Dictionary<Type, string> typeNames = new Dictionary<Type, string>();

        public static string GetTypeName(Type type)
        {
            string s = type.ToString();
            if (s.StartsWith("UI")) s = s.Substring(2);
            else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
            return s;
        }

        static public string GetTypeName<T>()
        {
            return GetTypeName(typeof(T));
        }

        /// <summary>
        /// 添加一个带特定脚本物体
        /// </summary>
        public static Component AddChild(this Transform parent, Type type, string name = null, bool undo = false)
        {
            GameObject go = AddChild(parent, undo);
            if (name == null)
            {
                if (!typeNames.TryGetValue(type, out name) || name == null)
                {
                    name = GetTypeName(type);
                    typeNames[type] = name;
                }
            }
            go.name = name;
            return go.AddComponent(type);
        }

        /// <summary>
        /// 添加一个带特定脚本物体
        /// </summary>
        public static T AddChild<T>(this Transform parent, string name = null, bool undo = false) where T : Component
        {
            Type type = typeof(T);
            return AddChild(parent, type, name, undo) as T;
        }

        /// <summary>
        /// 物体身上不存在脚本时添加脚本
        /// </summary>
        public static Component AddMissingComponent(this Transform transform, Type type, bool undo = false)
        {
            return transform.gameObject.AddMissingComponent(type, undo);
        }

        /// <summary>
        /// 物体身上不存在脚本时添加脚本
        /// </summary>
        public static T AddMissingComponent<T>(this Transform transform, bool undo = false) where T : Component
        {
            return transform.gameObject.AddMissingComponent(typeof(T), undo) as T;
        }

        /// <summary>
        /// 物体子节点不存在脚本时添加脚本
        /// </summary>
        public static Component AddMissingChildComponent(this Transform parent, Type type, string path, bool undo = false)
        {
            GameObject go = AddMissingChild(parent, path, undo);
            return AddMissingComponent(go, type, undo);
        }

        /// <summary>
        /// 物体子节点不存在脚本时添加脚本
        /// </summary>
        public static T AddMissingChildComponent<T>(this Transform parent, string name = null, bool undo = false) where T : Component
        {
            return AddMissingChildComponent(parent, typeof(T), name, undo) as T;
        }

        /// <summary>
        /// 获取物体上的Transform
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Transform FindTransform(Transform parent, string path, bool includeInactive = false)
        {
            Transform transform;
            if (!string.IsNullOrEmpty(path))
                transform = FindAndCreate(parent, path, includeInactive);
            else
                transform = parent;
            return transform;
        }

        /// <summary>
        /// 获取物体上的GameObject
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static GameObject FindGameObject(Transform parent, string path, bool includeInactive = false)
        {
            Transform transform = FindTransform(parent, path, includeInactive);
            return transform ? transform.gameObject : null;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component FindComponent(Transform parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            Transform transform = FindTransform(parent, path, includeInactive);
            Component component = null;
            if (transform)
            {
                if (includeChildren)
                    component = transform.GetComponentInChildren(type, includeInactive);
                else
                    component = transform.GetComponent(type);
            }
            return component;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T FindComponent<T>(Transform parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            return FindComponent(parent, typeof(T), path, includeChildren, includeInactive) as T;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component[] FindComponents(Transform parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            Transform transform = FindTransform(parent, path, includeInactive);
            Component[] components = null;
            if (transform)
            {
                if (includeChildren)
                    components = transform.GetComponentsInChildren(type, includeInactive);
                else
                    components = transform.GetComponents(type);
            }
            return components;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T[] FindComponents<T>(Transform parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            Transform transform = FindTransform(parent, path, includeInactive);
            T[] components = null;
            if (transform)
            {
                if (includeChildren)
                    components = transform.GetComponentsInChildren<T>(includeInactive);
                else
                    components = transform.GetComponents<T>();
            }
            return components;
        }

        /// <summary>
        /// 获取物体上的Transform
        /// </summary>
        /// <param name="path">父级物体下的子路径</param>
        public static Transform FindTransform(string path)
        {
            GameObject obj = GameObject.Find(path);
            if (obj)
                return obj.transform;
            return null;
        }

        /// <summary>
        /// 获取物体上的GameObject
        /// </summary>
        /// <param name="path">父级物体下的子路径</param>
        public static GameObject FindGameObject(string path)
        {
            return GameObject.Find(path);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        public static Component FindComponent(Type type, string path)
        {
            GameObject obj = GameObject.Find(path);
            if (obj)
                return obj.GetComponent(type);
            return null;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        public static T FindComponent<T>(string path) where T : Component
        {
            GameObject obj = GameObject.Find(path);
            if (obj)
                return obj.GetComponent<T>();
            return null;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        public static Component[] FindComponents(Type type, string path)
        {
            GameObject obj = GameObject.Find(path);
            if (obj)
                return obj.GetComponents(type);
            return null;
        }

        /// <summary>
        /// 本地坐标偏移
        /// </summary>
        public static void AddLocalPosition(this Transform transform, Vector3 offset)
        {
            if (transform)
                transform.localPosition += offset;
        }

        /// <summary>
        /// 本地坐标偏移单个轴向
        /// </summary>
        public static void AddLocalPositionAxis(this Transform transform, int axis, float value = 0f)
        {
            Vector3 newV3 = transform.localPosition;
            newV3[axis] += value;
            if (transform)
                transform.localPosition += newV3;
        }

        /// <summary>
        /// 本地坐标偏移
        /// </summary>
        public static void AddLocalPosition(this Transform transform, float ox, float oy, float oz)
        {
            if (transform)
                transform.localPosition += new Vector3(ox, oy, oz);
        }

        /// <summary>
        /// 设置物体的本地坐标
        /// </summary>
        public static void SetLocalPosition(this Transform transform, Vector3 position)
        {
            transform.localPosition = position;
        }

        /// <summary>
        /// 设置物体的本地坐标
        /// </summary>
        public static void SetLocalPosition(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
        {
            transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetParentLocalPosition(this Transform transform, Transform parent, float ox = 0, float oy = 0, float oz = 0)
        {
            if (transform)
            {
                transform.SetParent(parent);
                transform.localPosition = new Vector3(ox, oy, oz);
                transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 设置物体的单个轴向的本地坐标
        /// </summary>
        public static void SetLocalPositionAxis(this Transform transform, int axis, float value = 0f)
        {
            Vector3 newPos = transform.localPosition;
            newPos[axis] = value;
            transform.localPosition = newPos;
        }

        /// <summary>
        /// 全局坐标偏移
        /// </summary>
        public static void AddPosition(this Transform transform, Vector3 offset)
        {
            if (transform)
                transform.position += offset;
        }

        /// <summary>
        /// 全局坐标偏移
        /// </summary>
        public static void AddPosition(this Transform transform, float ox, float oy, float oz)
        {
            if (transform)
                transform.position += new Vector3(ox, oy, oz);
        }

        /// <summary>
        /// 设置物体的本地坐标
        /// </summary>
        public static void SetPosition(this Transform transform, Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// 设置物体的全局坐标
        /// </summary>
        public static void SetPosition(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
        {
            transform.position = new Vector3(x, y, z);
        }

        public static void SetParentPosition(this Transform transform, Transform parent, float x = 0, float y = 0, float z = 0)
        {
            if (transform)
            {
                transform.SetParent(parent);
                transform.position = new Vector3(x, y, z);
                transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 设置物体的单个轴向的本地坐标
        /// </summary>
        public static void SetPositionAxis(this Transform transform, int axis, float value = 0f)
        {
            Vector3 newPos = transform.position;
            newPos[axis] = value;
            transform.position = newPos;
        }

        /// <summary>
        /// 本地旋转角度偏移
        /// </summary>
        public static void AddLocalEuler(this Transform transform, Vector3 offset)
        {
            if (transform)
                transform.localEulerAngles += offset;
        }

        /// <summary>
        /// 本地旋转角度偏移
        /// </summary>
        public static void AddLocalEuler(this Transform transform, float ox, float oy, float oz)
        {
            if (transform)
                transform.localEulerAngles += new Vector3(ox, oy, oz);
        }

        /// <summary>
        /// 本地旋转角度偏移单个轴向
        /// </summary>
        public static void AddLocalEulerAxis(this Transform transform, int axis, float value = 0f)
        {
            Vector3 newV3 = transform.localEulerAngles;
            newV3[axis] += value;
            if (transform)
                transform.localEulerAngles = newV3;
        }

        /// <summary>
        /// 设置物体的本地旋转角度
        /// </summary>
        public static void SetLocalEuler(this Transform transform, Vector3 eulerAngles)
        {
            transform.localEulerAngles = eulerAngles;
        }

        /// <summary>
        /// 设置物体的本地旋转角度
        /// </summary>
        public static void SetLocalEuler(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
        {
            transform.localEulerAngles = new Vector3(x, y, z);
        }

        /// <summary>
        /// 设置物体的本地旋转角度单个轴向
        /// </summary>
        public static void SetLocalEulerAxis(this Transform transform, int axis, float value = 0f)
        {
            Vector3 newV3 = transform.localEulerAngles;
            newV3[axis] = value;
            transform.localEulerAngles = newV3;
        }

        /// <summary>
        /// 全局旋转角度偏移
        /// </summary>
        public static void AddEuler(this Transform transform, Vector3 offset)
        {
            if (transform)
                transform.eulerAngles += offset;
        }

        /// <summary>
        /// 全局旋转角度偏移
        /// </summary>
        public static void AddEuler(this Transform transform, float ox, float oy, float oz)
        {
            if (transform)
                transform.eulerAngles += new Vector3(ox, oy, oz);
        }

        /// <summary>
        /// 设置物体的全局旋转角度
        /// </summary>
        public static void SetEuler(this Transform transform, Vector3 eulerAngles)
        {
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// 设置物体的全局旋转角度
        /// </summary>
        public static void SetEuler(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
        {
            transform.eulerAngles = new Vector3(x, y, z);
        }

        /// <summary>
        /// 本地缩放偏移
        /// </summary>
        public static void AddLocalScale(this Transform transform, Vector3 offset)
        {
            if (transform)
                transform.localScale += offset;
        }

        /// <summary>
        /// 本地缩放偏移
        /// </summary>
        public static void AddLocalScale(this Transform transform, float ox, float oy, float oz)
        {
            if (transform)
                transform.localScale += new Vector3(ox, oy, oz);
        }

        /// <summary>
        /// 设置本地缩放
        /// </summary>
        public static void SetLocalScale(this Transform transform, float x = 1f, float y = 1f, float z = 1f)
        {
            transform.localScale = new Vector3(x, y, z);
        }

        /// <summary>
        /// 设置物体的统一本地缩放
        /// </summary>
        public static void SetLocalUniformScale(this Transform transform, float scale = 1f)
        {
            transform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// 全局缩放偏移
        /// </summary>
        public static void AddScale(this Transform transform, Vector3 offset)
        {
            if (transform)
            {
                if (transform.parent)
                {
                    Vector3 scale = transform.lossyScale + offset;
                    Vector3 parentScale = transform.parent.lossyScale;
                    transform.localScale = new Vector3(
                        parentScale.x == 0 ? scale.x : scale.x / parentScale.x,
                        parentScale.y == 0 ? scale.y : scale.y / parentScale.y,
                        parentScale.z == 0 ? scale.z : scale.z / parentScale.z);
                }
                else
                    transform.localScale += offset;
            }
        }

        /// <summary>
        /// 全局缩放偏移
        /// </summary>
        public static void AddScale(this Transform transform, float ox, float oy, float oz)
        {
            AddScale(transform, new Vector3(ox, oy, oz));
        }

        public static void SetScale(this Transform transform, Vector3 scale)
        {
            if (transform)
            {
                if (transform.parent)
                {
                    Vector3 parentScale = transform.parent.lossyScale;
                    transform.localScale = new Vector3(
                        parentScale.x == 0 ? scale.x : scale.x / parentScale.x,
                        parentScale.y == 0 ? scale.y : scale.y / parentScale.y,
                        parentScale.z == 0 ? scale.z : scale.z / parentScale.z);
                }
                else
                    transform.localScale = scale;
            }
        }

        public static void SetScale(this Transform transform, float ox, float oy, float oz)
        {
            SetScale(transform, new Vector3(ox, oy, oz));
        }

        public static void SetScale(this Transform transform, float scale)
        {
            SetScale(transform, scale, scale, scale);
        }

        /// <summary>
        /// 物体保持全局的缩放比例挂载至指定父节点
        /// </summary>
        public static void SetParentAndScale(this Transform transform, Transform parent, float scale)
        {
            if (transform)
            {
                transform.SetParent(parent);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.SetScale(scale);
            }
        }

        /// <summary>
        /// 物体保持全局的缩放比例以一定本地偏移挂载至指定父节点
        /// </summary>
        public static void SetParentLocalPosAndScale(this Transform transform, Transform parent, float scale, float ox = 0, float oy = 0, float oz = 0)
        {
            if (transform)
            {
                transform.SetParent(parent);
                transform.localPosition = new Vector3(ox, oy, oz);
                transform.localRotation = Quaternion.identity;
                transform.SetScale(scale);
            }
        }

        public static void ResetAllChildren(this Transform transform)
        {
            if (transform)
            {
                Transform trans;
                for (int i = 0; i < transform.childCount; i++)
                {
                    trans = transform.GetChild(i);
                    trans.ResetLocalTransform();
                    ResetAllChildren(trans);
                }
            }
        }
        #endregion

        #region Component 扩展

        /// <summary>
        /// 获取物体上的Transform
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Transform FindTransform(Component parent, string path, bool includeInactive = false)
        {
            return FindTransform(parent == null ? null : parent.transform, path, includeInactive);
        }

        /// <summary>
        /// 获取物体上的GameObject
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static GameObject FindGameObject(Component parent, string path, bool includeInactive = false)
        {
            return FindGameObject(parent ? parent.transform : null, path, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component FindComponent(Component parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            return FindComponent(parent == null ? null : parent.transform, type, path, includeChildren, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T FindComponent<T>(Component parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            return FindComponent(parent, typeof(T), path, includeChildren, includeInactive) as T;
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static Component[] FindComponents(Component parent, Type type, string path, bool includeChildren = false, bool includeInactive = false)
        {
            return FindComponents(parent == null ? null : parent.transform, type, path, includeChildren, includeInactive);
        }

        /// <summary>
        /// 获取物体上的脚本
        /// </summary>
        /// <param name="parent">父级物体</param>
        /// <param name="type">脚本类型</param>
        /// <param name="path">父级物体下的子路径</param>
        /// <param name="includeChildren">是否查询子物体</param>
        /// <param name="includeInactive">是否查找隐藏物体</param>
        public static T[] FindComponents<T>(Component parent, string path, bool includeChildren = false, bool includeInactive = false) where T : Component
        {
            return FindComponents<T>(parent, path, includeChildren, includeInactive);
        }
        #endregion

        #region 其他
        [Conditional("UNITY_EDITOR")]
        static void RegisterCreatedObjectUndo(Object objectToUndo, string name)
        {
            if (!Application.isPlaying && ResourceManager.editorTools != null)
                ResourceManager.editorTools.RegisterCreatedObjectUndo(objectToUndo, name);
        }

        public static float ParticleSystemLength(GameObject go, out bool isLoop, bool ignoreLoop = false)
        {
            ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
            float maxDuration = 0;
            isLoop = false;
            foreach (ParticleSystem ps in particleSystems)
            {
                if (!ps.gameObject.activeSelf) continue;
                ParticleSystem.EmissionModule emission = ps.emission;
                if (emission.enabled)
                {
                    ParticleSystem.MainModule main = ps.main;
                    isLoop |= main.loop;
                    if (main.loop && !ignoreLoop)
                        return -1f;
                    float delay = 0f;

                    if (main.loop && main.prewarm)
                    {
                        //循环且预热功能功能开启delay无效
                    }
                    else
                    {
                        ParticleSystem.MinMaxCurve delayCurve = main.startDelay;
                        if (delayCurve.mode == ParticleSystemCurveMode.Constant)
                            delay = delayCurve.constant;
                        else if (delayCurve.mode == ParticleSystemCurveMode.TwoConstants)
                            delay = Mathf.Max(delayCurve.constantMin, delayCurve.constantMax);
                    }

                    float duration = 0;
                    ParticleSystem.MinMaxCurve lifeCurve = main.startLifetime;
                    if (lifeCurve.mode == ParticleSystemCurveMode.Constant)
                        duration = lifeCurve.constant;
                    else if (lifeCurve.mode == ParticleSystemCurveMode.TwoConstants)
                        duration = Mathf.Max(lifeCurve.constantMin, lifeCurve.constantMax);
                    else if (lifeCurve.mode == ParticleSystemCurveMode.Curve)
                        duration = GetAnimationCurveMaxKeyValue(lifeCurve.curve);
                    else if (lifeCurve.mode == ParticleSystemCurveMode.TwoCurves)
                        duration = Mathf.Max(GetAnimationCurveMaxKeyValue(lifeCurve.curveMin), GetAnimationCurveMaxKeyValue(lifeCurve.curveMax));

                    duration = delay + Mathf.Max(duration, main.duration);

                    if (duration > maxDuration)
                        maxDuration = duration;
                }
            }
            return maxDuration;
        }

        static float GetAnimationCurveMaxKeyValue(AnimationCurve curve)
        {
            Keyframe[] keys = curve.keys;
            float max = float.MinValue;
            float min = float.MaxValue;
            for (int i = 0; i < keys.Length; i++)
            {
                max = Mathf.Max(max, keys[i].value);
                min = Mathf.Min(min, keys[i].value);
            }
            return max;
        }

        public static AnimationCurve CloneAnimationCurve(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve(curve.keys);
            newCurve.preWrapMode = curve.preWrapMode;
            newCurve.postWrapMode = curve.postWrapMode;
            return newCurve;
        }

        public static void CopyAnimationCurve(AnimationCurve from, AnimationCurve to)
        {
            to.keys = from.keys;
            to.preWrapMode = from.preWrapMode;
            to.postWrapMode = from.postWrapMode;
        }

        public static Gradient CloneGradient(Gradient gradient)
        {
            Gradient newGradient = new Gradient();
            newGradient.SetKeys(gradient.colorKeys, gradient.alphaKeys);
            newGradient.mode = gradient.mode;
            return newGradient;
        }

        public static void CopyGradient(Gradient from, Gradient to)
        {
            to.SetKeys(from.colorKeys, from.alphaKeys);
            to.mode = from.mode;
        }
        #endregion

        #region 资源相关
        static private bool applicationRunning = false;
        /// <summary>
        /// by Rick
        /// 设置程序是否在运行，因为游戏关闭时的OnDestroy顺序不稳定，Singletion可能已经失效了
        /// </summary>
        /// <param name="val"></param>
        static public void ApplicationRunning(bool val)
        {
            applicationRunning = val;
        }

        /// <summary>
        /// by Rick
        /// 设置图片，需要为图片资源计数
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sprite"></param>
        public static void AssetProxySetSprite(UnityEngine.UI.Image image, Sprite sprite)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxySetSprite(image, sprite);
        }

        public static void AssetProxySetMaterialTexture(Material material, string name, Texture texture)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxySetMaterialTexture(material, name, texture);
        }

        public static void AssetProxySetRawImage(UnityEngine.UI.RawImage rawImage, Texture texture)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxySetRawImage(rawImage, texture);
        }

        public static void AssetProxyOnImageCreate(UnityEngine.UI.Image image)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxyOnImageCreate(image);
        }

        public static void AssetProxyOnImageDestroy(UnityEngine.UI.Image image)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.AssetProxyOnImageDestroy(image);
            }
        }

        public static void AssetProxyOnRawImageCreate(UnityEngine.UI.RawImage image)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxyOnRawImageCreate(image);
        }

        public static void AssetProxyOnRawImageDestroy(UnityEngine.UI.RawImage image)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.AssetProxyOnRawImageDestroy(image);
            }
        }

        public static void AssetProxyAddCustomTextureRef(Texture texture)
        {
            if (applicationRunning && texture)
            {
                ResourceManager.Instance.StatisticsFacade.AssetProxyAddCustomTextureRef(texture);
            }
        }

        public static void AssetProxyReleaseCustomTextureRef(Texture texture)
        {
            if(applicationRunning && texture)
            {
                ResourceManager.Instance.StatisticsFacade.AssetProxyReleaseCustomTextureRef(texture);
            }
        }


        public static void ReplaceAnimationClip(AnimatorOverrideController animatorOverrideController, string stateName, AnimationClip clip)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.ReplaceAnimationClip(animatorOverrideController, stateName, clip);
            }
        }

        public static void InitGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.InitGlobalAnimatorController(animator);
            }
        }

        public static void UseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.UseGlobalAnimatorController(animator);
            }
        }

        public static void UnuseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            if (applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.UnuseGlobalAnimatorController(animator);
            }
        }

        public static string AssetProxyCreateCachedGroup(Type assetType, string groupName)
        {
            return ResourceManager.Instance.StatisticsFacade.AssetProxyCreateCachedGroup(assetType, groupName);
        }


        public static void AssetProxyAddUsageToCachedGroup(int assetId, string groupUniqueName)
        {
            ResourceManager.Instance.StatisticsFacade.AssetProxyAddUsageToCachedGroup(assetId, groupUniqueName);
        }

        public static void AssetProxyReleaseCachedGroup(string groupUniqueName)
        {
            if(applicationRunning)
            {
                ResourceManager.Instance.StatisticsFacade.AssetProxyReleaseCachedGroup(groupUniqueName);
            }
        }

        public static bool AssetProxyIsAssetLoaded(int assetId)
        {
            if(applicationRunning)
            {
                return ResourceManager.Instance.StatisticsFacade.AssetProxyIsAssetLoaded(assetId);
            }
            else
            {
                return false;
            }
        }

        public static string GetGameObjectFullPath(GameObject obj, char seperator)
        {
            string path = string.Format("{0}({1})", obj.name, obj.GetInstanceID());
            Transform t = obj.transform.parent;
            while(t)
            {
                path = string.Format("{0}({1})", t.gameObject.name, t.gameObject.GetInstanceID()) + seperator + path;
                t = t.parent;
            }

            return path;
        }


        #endregion

        #region 性能分析
        public static void BeginSample(string name, UnityEngine.Object target )
        {
            UnityEngine.Profiling.Profiler.BeginSample(name, target);
        }

        public static void EndSample()
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }

        #endregion

    }
}
