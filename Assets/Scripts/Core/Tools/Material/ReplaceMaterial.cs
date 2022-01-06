using System.Collections.Generic;
using UnityEngine;


namespace GameDLL.Tools
{
    internal static class ReplaceMaterial
    {
        private static readonly List<MatEntry> s_Entries = new List<MatEntry>();

        public static int Count
        {
            get
            {
                return s_Entries.Count;
            }
        }

        static Material FindMaterial(ref Material baseMat, int id, int renderQueue, Shader shader)
        {
            MatEntry e;
            for (int i = 0; i < s_Entries.Count; i++)
            {
                e = s_Entries[i];
                if (e.id == id)
                {
                    if (e.baseMat == baseMat && e.shader == shader)
                    {
                        if (renderQueue < 0 && e.renderQueue == baseMat.renderQueue)
                        {
                            ++e.count;
                            return e.customMat;
                        }
                        else if (renderQueue >= 0 && e.renderQueue == renderQueue)
                        {
                            ++e.count;
                            return e.customMat;
                        }
                    }
                    else if (e.customMat == baseMat)
                    {
                        baseMat = e.baseMat;
                        //if (shader == e.shader && (renderQueue < 0 || baseMat.renderQueue == renderQueue))
                        //{
                        //    ++e.count;
                        //    return baseMat;
                        //}
                        return FindMaterial(ref baseMat, id, renderQueue, shader);
                    }
                }
            }
            return null;
        }

        public static Material Add(Material baseMat, int id, int renderQueue = -1, Shader shader = null)
        {
            if (baseMat == null)
                return null;
            if (shader == null)
                shader = baseMat.shader;

            MatEntry e;
            //for (var i = 0; i < s_Entries.Count; ++i)
            //{
            //    e = s_Entries[i];
            //    if (e.baseMat != baseMat || e.id != id || (renderQueue < 0 && e.renderQueue != baseMat.renderQueue) || (renderQueue >= 0 && e.renderQueue != renderQueue)) continue;
            //    ++e.count;
            //    return e.customMat;
            //}
            Material material = FindMaterial(ref baseMat, id, renderQueue, shader);
            if (material)
                return material;

            e = new MatEntry();
            e.count = 1;
            e.baseMat = baseMat;
            e.id = id;
            e.customMat = new Material(baseMat);
            e.customMat.hideFlags = HideFlags.HideAndDontSave;
            e.shader = shader;
            if (baseMat.shader != shader)
                e.customMat.shader = shader;
            if (renderQueue < 0 || renderQueue == baseMat.renderQueue)
                e.renderQueue = baseMat.renderQueue;
            else
            {
                e.renderQueue = renderQueue;
                e.customMat.renderQueue = renderQueue;
            }
            s_Entries.Add(e);
            Log(">>>> ReplaceMaterial.Add -> count = {0} {1} {2} {3}", s_Entries.Count, baseMat, shader, id);
            return e.customMat;
        }

        public static void Remove(Material customMat)
        {
            if (!customMat) return;

            for (int i = 0; i < s_Entries.Count; ++i)
            {
                var e = s_Entries[i];
                if (e.customMat != customMat) continue;
                Remove(e, i);
                break;
            }
        }

        static void Remove(MatEntry e, int index)
        {
            if (--e.count == 0)
            {
                Log(">>>> ReplaceMaterial.Remove -> count = {0} {1} {2} {3}", s_Entries.Count - 1, e.customMat, e.shader, e.id);
                GameTools.DestroyImmediate(e.customMat);
                e.baseMat = null;
                s_Entries.RemoveAt(index);
            }
        }

        public static Material ResetShader(Material customMat)
        {
            if (!customMat) return null;

            MatEntry e;
            for (int i = 0; i < s_Entries.Count; ++i)
            {
                e = s_Entries[i];
                if (e.customMat != customMat) continue;

                if (customMat.shader != e.baseMat.shader)
                {
                    customMat = e.baseMat;
                    Remove(e, i);
                }
                break;
            }
            return customMat;
        }

        public static Material ResetRenderQueue(Material customMat)
        {
            if (!customMat) return null;

            MatEntry e;
            for (int i = 0; i < s_Entries.Count; ++i)
            {
                e = s_Entries[i];
                if (e.customMat != customMat) continue;

                if (customMat.renderQueue != e.baseMat.renderQueue)
                {
                    customMat = e.baseMat;
                    Remove(e, i);
                }
                break;
            }
            return customMat;
        }

        private class MatEntry
        {
            public Material baseMat;
            public Material customMat;
            public int count;
            //public Texture texture;
            public int id;
            public Shader shader;
            public int renderQueue;
        }

        public static bool LogInfo = false;

        static void Log(string format, params object[] args)
        {
            if (LogInfo)
                GLog.ErrorFormat(format, args);
        }
    }
}
