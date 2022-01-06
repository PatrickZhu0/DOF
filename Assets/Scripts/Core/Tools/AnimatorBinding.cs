using System;
using UnityEngine;

namespace GameDLL
{
    /// <summary>
    /// 过渡动画及绑点脚本
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class AnimatorBinding : MonoBehaviour
    {
        public static readonly int SpeedHashCode = Animator.StringToHash("Speed");
#pragma warning disable 0169
        [SerializeField]
        int bindingType;

#pragma warning disable 0649
        [SerializeField]
        Transform[] bindingTransforms;

        Animator animator;

        //Action<int, int> endAction;

        int gameObjectInstanceID;

        public float collisionRadius = 0.5f;

        AnimationInfo[] nextAnimationInfos;

        /// <summary>
        /// 用于标记需要播放过渡动画的数量
        /// </summary>
        int activeNextCount;

        //保存不存在Layer时的数据
        AnimationInfo animationInfo;

        void Awake()
        {
            gameObjectInstanceID = gameObject.GetInstanceID();
        }

        void OnEnable()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
            if (animator == null)
                enabled = false;

            if (nextAnimationInfos == null)
            {
                nextAnimationInfos = new AnimationInfo[animator.layerCount];
                for (int i = 0; i < nextAnimationInfos.Length; i++)
                    nextAnimationInfos[i] = new AnimationInfo(this, i);
            }
            activeNextCount = 0;
        }

        void OnDisable()
        {

            activeNextCount = 0;
        }

        void Update()
        {
            if (activeNextCount > 0)
            {
                for (int i = 0; i < nextAnimationInfos.Length; i++)
                {
                    if ((activeNextCount & (1 << i)) != 0)
                        nextAnimationInfos[i].Update();
                }
            }
            if (animationInfo != null)
                animationInfo.Update();
        }

        void OnDestroy()
        {
            //主动释放掉回调方法
            if (nextAnimationInfos == null)
            {
                for (int i = 0; i < nextAnimationInfos.Length; i++)
                    nextAnimationInfos[i].endAction = null;
            }
            if (animationInfo != null)
                animationInfo.endAction = null;
        }

        void OnDrawGizmosSelected()
        {
            float step = 0.10472f; // 值越低圆环越平滑

            Matrix4x4 defaultMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.red;

            // 绘制圆环
            Vector3 beginPoint1 = Vector3.zero;
            Vector3 beginPoint2 = new Vector3(0, 2, 0);
            Vector3 firstPoint1 = Vector3.zero;
            Vector3 firstPoint2 = new Vector3(0, 2, 0);
            int drawLine = 0;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += step)
            {
                float x = collisionRadius * Mathf.Cos(theta);
                float z = collisionRadius * Mathf.Sin(theta);
                Vector3 endPoint1 = new Vector3(x, 0, z);
                Vector3 endPoint2 = new Vector3(x, 2, z);
                if (theta == 0)
                {
                    firstPoint1 = endPoint1;
                    firstPoint2 = endPoint2;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint1, endPoint1);
                    Gizmos.DrawLine(beginPoint2, endPoint2);
                    if (drawLine == 3)
                        Gizmos.DrawLine(beginPoint1, beginPoint2);
                    drawLine++;
                    drawLine %= 4;
                }
                beginPoint1 = endPoint1;
                beginPoint2 = endPoint2;
            }
            Gizmos.DrawLine(firstPoint1, beginPoint1);
            Gizmos.DrawLine(firstPoint2, beginPoint2);
            if (drawLine == 3)
                Gizmos.DrawLine(beginPoint1, beginPoint2);

            Gizmos.color = defaultColor;

            Gizmos.matrix = defaultMatrix;
        }

        public Transform GetBinding(int index)
        {
            if (index >= 0 && index < bindingTransforms.Length)
                return bindingTransforms[index];
            return null;
        }

        public Transform[] GetBindings(int[] indexes)
        {
            Transform[] trans = null;
            if (indexes != null)
            {
                trans = new Transform[indexes.Length];
                for (int i = 0; i < indexes.Length; i++)
                    trans[i] = GetBinding(indexes[i]);
            }
            return trans;
        }

        public int GetValidBindingCount()
        {
            for (int i = bindingTransforms.Length; i > 0; i--)
            {
                if (bindingTransforms[i - 1] != null)
                    return i;
            }
            return 0;
        }

        public void SetTransitionAnimation(int stateHash, int stateLayer, float layerWeight, float stateSpeed, float speed, float transition, Action<int, int> action)
        {
            if (stateLayer < 0 || stateLayer >= nextAnimationInfos.Length)
            {
                animationInfo = new AnimationInfo(this, -1);
                animationInfo.SetTransitionAnimation(stateHash, layerWeight, stateSpeed, speed, transition, action);
                return;
            }
            nextAnimationInfos[stateLayer].SetTransitionAnimation(stateHash, layerWeight, stateSpeed, speed, transition, action);
            activeNextCount |= (1 << stateLayer);
        }

        public void ClearTransitionAnimation(int stateLayer)
        {
            if (stateLayer < 0 || stateLayer >= nextAnimationInfos.Length)
            {
                if (animationInfo != null)
                    animationInfo = null;
                return;
            }
            nextAnimationInfos[stateLayer].ClearTransitionAnimation();
        }

        #region 动画状态切换
        class AnimationInfo
        {
            int stateLayer;

            int nextStateHash;
            float nextStateSpeed;
            float nextLayerWeight;
            float checkTransition;
            float currentSpeed;

            public Action<int, int> endAction;

            AnimatorBinding binding;

            public AnimationInfo(AnimatorBinding binding, int layer)
            {
                this.binding = binding;
                stateLayer = layer;
            }

            public void ClearTransitionAnimation()
            {
                nextStateHash = 0;
                endAction = null;
                if (stateLayer >= 0)
                    binding.activeNextCount &= ~(1 << stateLayer);
                else if (binding.animationInfo == this)
                {
                    endAction = null;
                    binding.animationInfo = null;
                }
            }

            void EndAnimation(int currentNameHash)
            {
                binding.animator.SetFloat(SpeedHashCode, nextStateSpeed);
                binding.animator.SetLayerWeight(stateLayer, nextLayerWeight);
                if (endAction != null)
                    endAction(binding.gameObjectInstanceID, currentNameHash);
                ClearTransitionAnimation();
            }

            public void SetTransitionAnimation(int stateHash, float layerWeight, float stateSpeed, float speed, float transition, Action<int, int> action)
            {
                nextStateHash = stateHash;
                nextLayerWeight = layerWeight;
                nextStateSpeed = stateSpeed;
                currentSpeed = speed;
                checkTransition = transition;
                endAction = action;
            }

            public void Update()
            {
                if (nextStateHash != 0 || endAction != null)
                {
                    AnimatorStateInfo stateInfo = binding.animator.GetCurrentAnimatorStateInfo(stateLayer);
                    float normalizedTransitionDuration = stateInfo.normalizedTime;
                    if (currentSpeed < 0)
                        normalizedTransitionDuration = 1f - normalizedTransitionDuration;
                    if (checkTransition > 1)
                    {
                        if (normalizedTransitionDuration >= checkTransition)
                        {
                            normalizedTransitionDuration %= 1f;
                            if (nextStateHash != 0)
                            {
                                //直接进行切换
                                binding.animator.CrossFade(nextStateHash, 1 - normalizedTransitionDuration, stateLayer);
                            }
                            EndAnimation(stateInfo.shortNameHash);
                        }
                    }
                    else if (checkTransition == 1)
                    {
                        if (normalizedTransitionDuration >= checkTransition)
                        {
                            if (nextStateHash != 0)
                            {
                                //直接进行切换
                                binding.animator.Play(nextStateHash, stateLayer);
                            }
                            EndAnimation(stateInfo.shortNameHash);
                        }
                    }
                    else
                    {
                        normalizedTransitionDuration %= 1f;
                        if (normalizedTransitionDuration >= checkTransition)
                        {
                            if (nextStateHash != 0)
                                binding.animator.CrossFade(nextStateHash, 1 - normalizedTransitionDuration, stateLayer);
                            EndAnimation(stateInfo.shortNameHash);
                        }
                    }
                }
            }
        }
        #endregion
    }
}