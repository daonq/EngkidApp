using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G5
{
    public class Tutorial : MonoBehaviour
    {
        public SkeletonGraphic bone;
        public string[] status;
        public float[] time;
        public bool isTutorial = true;
        public IEnumerator PlayByYield()
        {
            LeanTween.dispatchEvent(G5Event.SOUND, "tap");
            yield return PlayHint();
            if (isTutorial) close();
        }
        public void Begin()
        {
            StartCoroutine(PlayByYield());
        }
        public void close()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            isTutorial = false;
            LeanTween.dispatchEvent(G5Event.SOUND, "tap");
            LeanTween.dispatchEvent(G5Event.ACTION,G5Event.BEGIN);
        }
        public IEnumerator PlayHint()
        {
            gameObject.SetActive(true);
            bone.AnimationState.SetAnimation(0, status[0], false);
            yield return new WaitForSeconds(time[0]);
            bone.AnimationState.SetAnimation(0, status[1], false);
            yield return new WaitForSeconds(time[1]);
            bone.AnimationState.SetAnimation(0, status[2], false);
            yield return new WaitForSeconds(time[2]);
            bone.AnimationState.SetAnimation(0, status[3], false);
        }
    }
}
