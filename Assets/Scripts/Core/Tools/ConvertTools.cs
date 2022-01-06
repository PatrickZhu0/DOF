using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDLL
{
    public static class ConvertTools
    {
        public delegate object ConvertToObject(string str);
        /// <summary>
        /// 支持的参数类型
        /// </summary>
        public static readonly List<Type> supportedTypeList = new List<Type>();
        static readonly Dictionary<string, Type> supportedTypeDic = new Dictionary<string, Type>();
        static readonly Dictionary<Type, ConvertToObject> supportedTypeConvertDic = new Dictionary<Type, ConvertToObject>();

        static readonly char[] splitVector = new char[] { ',', '(', ')' };
        static readonly char[] splitRect = new char[] { ',', '(', ')', '/' };

        static ConvertTools()
        {
            AddSupportedType(typeof(Enum), null);        //占位用
            AddSupportedType(typeof(bool), convertToBool);
            AddSupportedType(typeof(sbyte), convertToSByte);
            AddSupportedType(typeof(byte), convertToByte);
            AddSupportedType(typeof(short), convertToShort);
            AddSupportedType(typeof(ushort), convertToUShort);
            AddSupportedType(typeof(int), convertToInt);
            AddSupportedType(typeof(uint), convertToUInt);
            AddSupportedType(typeof(long), convertToLong);
            AddSupportedType(typeof(ulong), convertToULong);
            AddSupportedType(typeof(float), convertToFloat);
            AddSupportedType(typeof(double), convertToDouble);
            AddSupportedType(typeof(char), convertToChar);
            AddSupportedType(typeof(string), convertToString);
            AddSupportedType(typeof(Vector2), convertToVector2);
            AddSupportedType(typeof(Vector2Int), convertToVector2Int);
            AddSupportedType(typeof(Vector3), convertToVector3);
            AddSupportedType(typeof(Vector3Int), convertToVector3Int);
            AddSupportedType(typeof(Vector4), convertToVector4);
            AddSupportedType(typeof(Quaternion), convertToQuaternion);
            AddSupportedType(typeof(Color), convertToColor);
            AddSupportedType(typeof(Color32), convertToColor32);
            AddSupportedType(typeof(Rect), convertToRect);
            AddSupportedType(typeof(RectInt), convertToRectInt);
            AddSupportedType(typeof(Bounds), convertToBounds);
            AddSupportedType(typeof(BoundsInt), convertToBoundsInt);
        }

        public static void AddSupportedType(Type type, ConvertToObject convert)
        {
            if (!supportedTypeList.Contains(type))
            {
                supportedTypeList.Add(type);
                supportedTypeDic.Add(type.FullName, type);
                supportedTypeConvertDic.Add(type, convert);
            }
        }

        public static bool IsSupportedType(string typeName)
        {
            return supportedTypeDic.ContainsKey(typeName);
        }

        public static bool IsSupportedType(Type type)
        {
            return supportedTypeConvertDic.ContainsKey(type);
        }

        public static Type GetSupportedType(string typeName)
        {
            Type type = null;
            supportedTypeDic.TryGetValue(typeName, out type);
            return type;
        }

        public static object Convert(Type type, string value)
        {
            if (type != null)
            {
                if (type.IsEnum)
                    return convertToEnum(type, value);
                ConvertToObject func;
                supportedTypeConvertDic.TryGetValue(type, out func);
                if (func != null)
                    return func(value);
            }
            return null;
        }

        public static T Convert<T>(string value)
        {
            return (T)Convert(typeof(T), value);
        }

        private static object convertToEnum<T>(string value)
        {
            return convertToEnum(typeof(T), value);
        }

        private static object convertToEnum(Type type, string value)
        {
            try
            {
                return Enum.Parse(type, value, true);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToBool(string value)
        {
            bool b;
            if (bool.TryParse(value, out b))
                return b;
            return null;
        }

        private static object convertToSByte(string value)
        {
            sbyte s;
            if (sbyte.TryParse(value, out s))
                return s;
            return null;
        }

        private static object convertToByte(string value)
        {
            byte b;
            if (byte.TryParse(value, out b))
                return b;
            return null;
        }

        private static object convertToShort(string value)
        {
            short s;
            if (short.TryParse(value, out s))
                return s;
            return null;
        }

        private static object convertToUShort(string value)
        {
            ushort u;
            if (ushort.TryParse(value, out u))
                return u;
            return null;
        }

        private static object convertToInt(string value)
        {
            int i;
            if (int.TryParse(value, out i))
                return i;
            return null;
        }

        private static object convertToUInt(string value)
        {
            uint u;
            if (uint.TryParse(value, out u))
                return u;
            return null;
        }

        private static object convertToLong(string value)
        {
            long l;
            if (long.TryParse(value, out l))
                return l;
            return null;
        }

        private static object convertToULong(string value)
        {
            ulong u;
            if (ulong.TryParse(value, out u))
                return u;
            return null;
        }

        private static object convertToFloat(string value)
        {
            float f;
            if (float.TryParse(value, out f))
                return f;
            return null;
        }

        private static object convertToDouble(string value)
        {
            double d;
            if (double.TryParse(value, out d))
                return d;
            return null;
        }

        private static object convertToChar(string value)
        {
            char c;
            if (char.TryParse(value, out c))
                return c;
            return null;
        }

        private static object convertToString(string value)
        {
            if (value == null)
                return "";
            return value;
        }

        private static object convertToVector2(string value)
        {
            try
            {
                return ConvertToVector2(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToVector2Int(string value)
        {
            try
            {
                return ConvertToVector2Int(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToVector3(string value)
        {
            try
            {
                return ConvertToVector3(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToVector3Int(string value)
        {
            try
            {
                return ConvertToVector3Int(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToVector4(string value)
        {
            try
            {
                return ConvertToVector4(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToQuaternion(string value)
        {
            try
            {
                return ConvertToQuaternion(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToColor(string value)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(value, out color))
                return color;
            return null;
        }

        private static object convertToColor32(string value)
        {
            try
            {
                return HexToColor32(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToRect(string value)
        {
            try
            {
                return ConvertToRect(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToRectInt(string value)
        {
            try
            {
                return ConvertToRectInt(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToBounds(string value)
        {
            try
            {
                return ConvertToBounds(value, false);
            }
            catch
            {
                return null;
            }
        }

        private static object convertToBoundsInt(string value)
        {
            try
            {
                return ConvertToBoundsInt(value, false);
            }
            catch
            {
                return null;
            }
        }

        public static Vector2 ConvertToVector2(string str, bool forceConvert = true)
        {
            Vector2 v = Vector2.zero;
            try
            {
                string[] s = str.Split(splitVector);
                float f;
                if (float.TryParse(s[1], out f))
                    v.x = f;
                else if(!forceConvert)
                    throw new FormatException("ConvertToVector2.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    v.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector2.y: " + s[2]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToVector2: " + str);
            }
            return v;
        }

        public static string ToString(Vector2 v)
        {
            return string.Format("({0},{1})", v.x, v.y);
        }

        public static Vector2Int ConvertToVector2Int(string str, bool forceConvert = true)
        {
            Vector2Int v = Vector2Int.zero;
            try
            {
                string[] s = str.Split(splitVector);
                int i;
                if (int.TryParse(s[1], out i))
                    v.x = i;
                else if(!forceConvert)
                    throw new FormatException("ConvertToVector2Int.x: " + s[1]);
                if (int.TryParse(s[2], out i))
                    v.y = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector2Int.y: " + s[2]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToVector2Int: " + str);
            }
            return v;
        }

        public static string ToString(Vector2Int v)
        {
            return string.Format("({0},{1})", v.x, v.y);
        }

        public static Vector3 ConvertToVector3(string str, bool forceConvert = true)
        {
            Vector3 v = Vector3.zero;
            try
            {
                string[] s = str.Split(splitVector);
                float f;
                if (float.TryParse(s[1], out f))
                    v.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    v.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3.y: " + s[2]);
                if (float.TryParse(s[3], out f))
                    v.z = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3.z: " + s[3]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToVector3: " + str);
            }
            return v;
        }

        public static string ToString(Vector3 v)
        {
            return string.Format("({0},{1},{2})", v.x, v.y, v.z);
        }

        public static Vector3Int ConvertToVector3Int(string str, bool forceConvert = true)
        {
            Vector3Int v = Vector3Int.zero;
            try
            {
                string[] s = str.Split(splitVector);
                int i;
                if (int.TryParse(s[1], out i))
                    v.x = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3Int.x: " + s[1]);
                if (int.TryParse(s[2], out i))
                    v.y = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3Int.y: " + s[2]);
                if (int.TryParse(s[3], out i))
                    v.z = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector3Int.z: " + s[3]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToVector3Int: " + str);
            }
            return v;
        }

        public static string ToString(Vector3Int v)
        {
            return string.Format("({0},{1},{2})", v.x, v.y, v.z);
        }

        public static Vector4 ConvertToVector4(string str, bool forceConvert = true)
        {
            Vector4 v = Vector4.zero;
            try
            {
                string[] s = str.Split(splitVector);
                float f;
                if (float.TryParse(s[1], out f))
                    v.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector4.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    v.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector4.y: " + s[2]);
                if (float.TryParse(s[3], out f))
                    v.z = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector4.z: " + s[3]);
                if (float.TryParse(s[4], out f))
                    v.w = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToVector4.w: " + s[4]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToVector4: " + str);
            }
            return v;
        }

        public static string ToString(Vector4 v)
        {
            return string.Format("({0},{1},{2},{3})", v.x, v.y, v.z, v.w);
        }

        public static Quaternion ConvertToQuaternion(string str, bool forceConvert = true)
        {
            Quaternion q = Quaternion.identity;
            try
            {
                string[] s = str.Split(splitVector);
                float f;
                if (float.TryParse(s[1], out f))
                    q.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToQuaternion.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    q.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToQuaternion.y: " + s[2]);
                if (float.TryParse(s[3], out f))
                    q.z = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToQuaternion.z: " + s[3]);
                if (float.TryParse(s[4], out f))
                    q.w = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToQuaternion.w: " + s[4]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToQuaternion: " + str);
            }
            return q;
        }

        public static string ToString(Quaternion q)
        {
            return string.Format("({0},{1},{2},{3})", q.x, q.y, q.z, q.w);
        }

        public static Quaternion ConvertToQuaternion(Vector4 v4)
        {
            return new Quaternion(v4.x, v4.y, v4.z, v4.w);
        }

        public static Vector4 ConvertToVector4(Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        public static Color32 HexToColor32(string str, bool forceConvert = true)
        {
            Color32 color = Color.white;
            try
            {
                byte b;
                if (byte.TryParse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
                    color.r = b;
                else if (!forceConvert)
                    throw new FormatException("HexToColor32.r: " + str.Substring(1, 2));
                if (byte.TryParse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
                    color.g = b;
                else if (!forceConvert)
                    throw new FormatException("HexToColor32.g: " + str.Substring(3, 2));
                if (byte.TryParse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
                    color.b = b;
                else if (!forceConvert)
                    throw new FormatException("HexToColor32.b: " + str.Substring(5, 2));
                if (byte.TryParse(str.Substring(7, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
                    color.a = b;
                else if (!forceConvert)
                    throw new FormatException("HexToColor32.a: " + str.Substring(7, 2));
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("HexToColor32: " + str);
            }
            return color;
        }

        public static string Color32ToHex(Color32 color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
        }

        public static string ToString(Color color)
        {
            return string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(color));
        }

        public static Rect ConvertToRect(string str, bool forceConvert = true)
        {
            Rect rect = Rect.zero;
            try
            {
                string[] s = str.Split(splitRect);
                float f;
                if (float.TryParse(s[1], out f))
                    rect.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRect.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    rect.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRect.y: " + s[2]);
                if (float.TryParse(s[3], out f))
                    rect.width = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRect.width: " + s[3]);
                if (float.TryParse(s[4], out f))
                    rect.height = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRect.height: " + s[4]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToRect: " + str);
            }
            return rect;
        }

        public static string ToString(Rect rect)
        {
            return string.Format("({0},{1},{2},{3})", rect.x, rect.y, rect.width, rect.height);
        }

        public static RectInt ConvertToRectInt(string str, bool forceConvert = true)
        {
            RectInt rect = new RectInt();
            try
            {
                string[] s = str.Split(splitRect);
                int i;
                if (int.TryParse(s[1], out i))
                    rect.x = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRectInt.x: " + s[1]);
                if (int.TryParse(s[2], out i))
                    rect.y = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRectInt.y: " + s[2]);
                if (int.TryParse(s[3], out i))
                    rect.width = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRectInt.width: " + s[3]);
                if (int.TryParse(s[4], out i))
                    rect.height = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToRectInt.height: " + s[4]);
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToRectInt: " + str);
            }
            return rect;
        }

        public static string ToString(RectInt rect)
        {
            return string.Format("({0},{1}/{2},{3})", rect.x, rect.y, rect.width, rect.height);
        }

        public static Bounds ConvertToBounds(string str, bool forceConvert = true)
        {
            Bounds bounds = new Bounds();
            try
            {
                string[] s = str.Split(splitRect);
                Vector3 v = Vector3.zero;
                float f;
                if (float.TryParse(s[1], out f))
                    v.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.center.x: " + s[1]);
                if (float.TryParse(s[2], out f))
                    v.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.center.y: " + s[2]);
                if (float.TryParse(s[3], out f))
                    v.z = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.center.z: " + s[3]);
                bounds.center = v;

                v = Vector3.zero;
                if (float.TryParse(s[4], out f))
                    v.x = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.size.x: " + s[4]);
                if (float.TryParse(s[5], out f))
                    v.y = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.size.y: " + s[5]);
                if (float.TryParse(s[6], out f))
                    v.z = f;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBounds.size.z: " + s[6]);
                bounds.size = v;
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToBounds: " + str);
            }
            return bounds;
        }

        public static string ToString(Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            return string.Format("({0},{1},{2}/{3},{4},{5})", center.x, center.y, center.z, size.x, size.y, size.z);
        }

        public static BoundsInt ConvertToBoundsInt(string str, bool forceConvert = true)
        {
            BoundsInt bounds = new BoundsInt();
            try
            {
                string[] s = str.Split(splitRect);
                Vector3Int v = Vector3Int.zero;
                int i;
                if (int.TryParse(s[1], out i))
                    v.x = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.center.x: " + s[1]);
                if (int.TryParse(s[2], out i))
                    v.y = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.center.y: " + s[2]);
                if (int.TryParse(s[3], out i))
                    v.z = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.center.z: " + s[3]);
                bounds.position = v;

                v = Vector3Int.zero;
                if (int.TryParse(s[4], out i))
                    v.x = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.size.x: " + s[4]);
                if (int.TryParse(s[5], out i))
                    v.y = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.size.y: " + s[5]);
                if (int.TryParse(s[6], out i))
                    v.z = i;
                else if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt.size.z: " + s[6]);
                bounds.size = v;
            }
            catch
            {
                if (!forceConvert)
                    throw new FormatException("ConvertToBoundsInt: " + str);
            }
            return bounds;
        }

        public static string ToString(BoundsInt bounds)
        {
            Vector3Int position = bounds.position;
            Vector3Int size = bounds.size;
            return string.Format("({0},{1},{2}/{3},{4},{5})", position.x, position.y, position.z, size.x, size.y, size.z);
        }

        
    }
}
