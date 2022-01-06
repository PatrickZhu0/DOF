using UnityEngine;

namespace GameDLL
{
    /// <summary>
    /// 用来检测模型是否在屏幕内
    /// </summary>
    public class VisibleOrInvisible : MonoBehaviour
    {
        public bool isInScreen = true;

        private void OnBecameVisible()
        {
            isInScreen = true;
            enabled = true;
        }

        private void OnBecameInvisible()
        {
            isInScreen = false;
            enabled = false;
        }

    }
}