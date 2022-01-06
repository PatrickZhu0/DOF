using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDLL.Resource.Statistics
{
    public class AnimationClipUsageStatistics : GenericUsageStatistics<AnimationClip>
    {
        public AnimationClipUsageStatistics(StatisticsFacade facade) : base(facade)
        {

        }

        public override void OnGameObjectAwake(GameObject gameObject)
        {
            /*
            Animator[] animators = gameObject.GetComponentsInChildren<Animator>(true);
            Facade.LogVerboseFormat("AnimationClipUsageStatistics.OnGameObjectAwake, found {0} Animators for {1}, id={2}", (animators == null) ? 0 : animators.Length, gameObject.name, gameObject.GetInstanceID());
            if(animators!=null)
            {
                foreach (var animator in animators)
                {
                    for(int layer = 0; layer < animator.layerCount; ++layer)
                    {
                        var clipInfoArray = animator.GetCurrentAnimatorClipInfo(layer);
                        Facade.LogVerboseFormat("AnimationClipUsageStatistics.OnGameObjectAwake, found {0} Animators for {1}, id={2}, layer={3}, count={4}", (animators == null) ? 0 : animators.Length, animator.name, animator.GetInstanceID(), layer, clipInfoArray.Length);
                        foreach (var clip in clipInfoArray)
                        {
                            AddRef(clip.clip);
                        }
                    }
                }
            }
            */
        }

        public override void OnGameObjectDestroy(GameObject gameObject)
        {
            /*
            Animator[] animators = gameObject.GetComponentsInChildren<Animator>(true);
            Facade.LogVerboseFormat("AnimationClipUsageStatistics.OnGameObjectDestroy, found {0} Animators for {1}, id={2}", (animators == null) ? 0 : animators.Length, gameObject.name, gameObject.GetInstanceID());
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    Facade.LogVerboseFormat("AnimationClipUsageStatistics.OnGameObjectDestroy for animators {0}, id={1}", animator.gameObject.name, animator.gameObject.GetInstanceID());
                    for (int layer = 0; layer < animator.layerCount; ++layer)
                    {
                        var clipInfoArray = animator.GetCurrentAnimatorClipInfo(layer);
                        Facade.LogVerboseFormat("AnimationClipUsageStatistics.OnGameObjectDestroy, found {0} Animators for {1}, id={2}, layer={3}, count={4}", (animators == null) ? 0 : animators.Length, animator.name, animator.GetInstanceID(), layer, clipInfoArray.Length);
                        foreach (var clip in clipInfoArray)
                        {
                            ReleaseRef(clip.clip);
                        }
                    }
                }
            }
            */
        }

        /// <summary>
        /// 接管所有使用AnimationClip的地方
        /// </summary>
        /// <param name="animatorOverrideController"></param>
        /// <param name="clip"></param>
        public void ReplaceAnimationClip(AnimatorOverrideController animatorOverrideController, string stateName, AnimationClip clip)
        {
            string oldName = string.Empty, newName = string.Empty;
            AnimationClip oldClip = animatorOverrideController[stateName];
            if (oldClip != null)
            {
                oldName = oldClip.name;
                ReleaseRef(oldClip);
            }
            animatorOverrideController[stateName] = clip;
            if(clip!=null)
            {
                newName = clip.name;
                AddRef(clip);
            }
            Facade.LogVerboseFormat("ReplaceAnimationClip {0} -> {1}", oldName, newName);
        }

        public void InitGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Facade.LogVerboseFormat("InitGlobalAnimatorController animator={0}, id={1}", animator.name, animator.GetInstanceID());
            foreach (AnimationClip clip in animator.animationClips)
            {
                AddRef(clip);
            }
        }

        public void UseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Facade.LogVerboseFormat("UseGlobalAnimatorController animator={0}, id={1}", animator.name, animator.GetInstanceID());
            foreach (AnimationClip clip in animator.animationClips)
            {
                AddRef(clip);
            }
        }

        public void UnuseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Facade.LogVerboseFormat("UnuseGlobalAnimatorController animator={0}, id={1}", animator.name, animator.GetInstanceID());
            foreach (AnimationClip clip in animator.animationClips)
            {
                ReleaseRef(clip);
            }
        }
    }
}
