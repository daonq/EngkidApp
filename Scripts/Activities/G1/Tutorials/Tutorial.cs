using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G1
{
    public class Tutorial : MonoBehaviour
    {
        public SkeletonGraphic bone;
        public string[] status;
        public float[] time;
        public bool isTutorial = true;
        public IEnumerator Play()
        {
            for(int i = 0; i < status.Length; i++)
            {
                yield return bone.AnimationState.SetAnimation(0, status[i], false);
                yield return new WaitForSeconds(time[i]);
            }
            yield return new WaitForSeconds(1);
            if (isTutorial) LeanTween.dispatchEvent(Event.ACTION, Event.BEGIN);
            gameObject.SetActive(false);
        }
    }
}
