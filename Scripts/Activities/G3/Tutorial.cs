using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G3
{
    public class Tutorial : MonoBehaviour
    {
        public SkeletonGraphic bone;
        public string status;
        public float time;
        public bool Istutorial = true;
        public IEnumerator Play()
        {
            yield return new WaitForSeconds(0.5f);
            bone.AnimationState.SetAnimation(0, status, false);
            yield return new WaitForSeconds(time);
            if(Istutorial) LeanTween.dispatchEvent(Event.ACTION, Event.BEGIN);
            gameObject.SetActive(false);
        }
    }
}
