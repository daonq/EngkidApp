using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;

namespace G5
{
    public class PrinterInReuse : MonoBehaviour
    {
        public Image pic;
        public SkeletonGraphic sPrinter;
        public Transform[] P;
        private float time = 1;
        public bool isFly = false;
        private Vector3 _oldPos;
        private Vector2 _oldPosMask;
        private Quaternion _oldRotation;
        private bool _isDrop = false;
        void Awake()
        {
            _oldPos = pic.transform.localPosition;
            _oldPosMask = pic.transform.parent.localPosition;
            _oldRotation = pic.transform.parent.rotation;
        }
        private void Start()
        {
            pic.transform.parent.localPosition = _oldPosMask;
            pic.transform.parent.rotation = _oldRotation;
            pic.transform.localPosition = P[0].transform.localPosition;
            pic.transform.parent.localScale = Vector3.one;
        }
        public void OutPut(Sprite spr)
        {
            transform.SetAsLastSibling();
            pic.sprite = spr;
            Start();
            _isDrop = true;
            sPrinter.AnimationState.SetAnimation(0,"printing", false);
            LeanTween.delayedCall(0.3f, () => LeanTween.dispatchEvent(G5Event.SOUND, "print"));
            LeanTween.moveLocal(pic.gameObject,_oldPos, time/2).setDelay(0.3f).setOnComplete(() => {
                sPrinter.AnimationState.SetAnimation(0, "idle", false);
            });
        }
        public void FLy()
        {
            if(_isDrop) LeanTween.dispatchEvent(G5Event.SOUND, "drop");
            isFly = true;
            LeanTween.moveLocal(pic.transform.parent.gameObject, P[1].transform.localPosition, time/2).setOnComplete(() => {
                LeanTween.scale(pic.transform.parent.gameObject,Vector3.zero, 0.3f);
                LeanTween.moveLocal(pic.transform.parent.gameObject,P[2].transform.localPosition,0.3f);
                LeanTween.rotateAround(pic.transform.parent.gameObject,Vector3.forward,360,0.3f);
            });
        }
        public void Normal()
        {
            LeanTween.delayedCall(3, clear);
        }
        public void clear()
        {
            sPrinter.AnimationState.ClearTracks();
            sPrinter.Skeleton.SetToSetupPose();
            _isDrop = false;
        }
    }
}
