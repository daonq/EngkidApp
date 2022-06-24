using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using Spine.Unity;
namespace G2
{
    public class Fly : MonoBehaviour
    {
        public SkeletonGraphic bone;
        public Transform[] Points;
        public int type = 0;
        private Vector3 _oldPos;
        private Vector3[] pos;
        void Start()
        {
            _oldPos = bone.transform.localPosition;
            pos = Points.Select(x => x.transform.localPosition).ToArray();
            if (type == 0) butterfly();
            if (type == 1) fly();
        }
        void butterfly()
        {
            LeanTween.moveLocal(bone.gameObject, pos[0], 2.0f).setOnComplete(() => {
                LeanTween.moveLocal(bone.gameObject, pos[1], 2.0f).setOnComplete(() => {
                    LeanTween.moveLocal(bone.gameObject, pos[2], 2.0f).setOnComplete(() => {
                        LeanTween.moveLocal(bone.gameObject, pos[3], 2.0f).setOnComplete(() => {
                            bone.transform.localPosition = _oldPos;
                            Start();
                        });
                    });
                });
            });
        }
        void fly()
        {
            bone.AnimationState.SetAnimation(0, "idle", false);
            bone.startingLoop = false;
            LeanTween.delayedCall(1, () => {
                bone.startingLoop = true;
                bone.AnimationState.SetAnimation(0, "flying", true);
            });
            LeanTween.moveLocal(bone.gameObject, pos[0], 2.0f).setOnComplete(() =>
            {
                LeanTween.moveLocal(bone.gameObject, pos[1], 2.0f).setOnComplete(() =>
                {
                    LeanTween.moveLocal(bone.gameObject, pos[2], 2.0f).setOnComplete(() =>
                    {
                        LeanTween.moveLocal(bone.gameObject, pos[3], 2.0f).setOnComplete(() =>
                        {
                            bone.transform.localPosition = _oldPos;
                            Start();
                        });
                    });
                });
            }).setDelay(1);
        }
    }
}
