using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G2
{
    public class Tutorial : MonoBehaviour
    {
        public SkeletonGraphic bone;
        public string status;
        public float time;
        public bool Istutorial = true;
        public void Play()
        {
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            gameObject.SetActive(true);
            LeanTween.delayedCall(0.1f, () => {
                clear();
                bone.AnimationState.SetAnimation(0, status, false);
                LeanTween.delayedCall(time, () =>
                {
                    
                    if (Istutorial) LeanTween.dispatchEvent(Event.SCENE, Event.CLOSE);
                    else gameObject.SetActive(false);
                });
            });
        }
        public void close()
        {
            gameObject.SetActive(false);
            LeanTween.cancel(gameObject);
            LeanTween.dispatchEvent(Event.SOUND, "back");
            if (Istutorial) Istutorial = false;
        }
        public void clear()
        {
            bone.AnimationState.ClearTracks();
            bone.Skeleton.SetToSetupPose();
        }
    }
}
