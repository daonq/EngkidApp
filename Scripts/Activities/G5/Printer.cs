using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;

namespace G5
{
    public class Printer : MonoBehaviour
    {
        public SkeletonGraphic spic;
        public SkeletonGraphic sPrinter;
        private float time = 1;
        private string status;
        public bool isFly = false;
        private void Start()
        {
            status = "";
        }
        public void OutPut(string _status)
        {
            status = _status;
            sPrinter.AnimationState.SetAnimation(0,"printing", false);
            LeanTween.delayedCall(0.3f, () => LeanTween.dispatchEvent(G5Event.SOUND, "print"));
            LeanTween.delayedCall(time,() => {
                sPrinter.AnimationState.SetAnimation(0,"idle",false);
                spic.AnimationState.SetAnimation(0,status,false);
            });
        }
        public void FLy()
        {
            spic.AnimationState.SetAnimation(0, status + "2", false);
            LeanTween.dispatchEvent(G5Event.SOUND, "drop");
            isFly = true;
            LeanTween.delayedCall(2, () => {
                spic.AnimationState.SetAnimation(0, "printing_null", true);
                status = "";
            });
        }
        public void Normal()
        {
            spic.AnimationState.SetAnimation(0, "printing_null", false);
            LeanTween.delayedCall(3, clear);
        }
        public void clear()
        {
            spic.AnimationState.ClearTracks();
            spic.Skeleton.SetToSetupPose();
            sPrinter.AnimationState.ClearTracks();
            sPrinter.Skeleton.SetToSetupPose();
            status = "";
        }
    }
}
