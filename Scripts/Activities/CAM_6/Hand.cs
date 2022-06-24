using UnityEngine;
using Spine;
using Spine.Unity;
using common;
namespace CAM_6
{
    public class Hand : MonoBehaviour
    {
        public Transform[] trans;
        public Vector3[] pos;
        public Vector3 _oldPos;
        private void Start()
        {
            _oldPos = transform.localPosition;
        }
        public void go(int i)
        {
            GetComponent<SkeletonGraphic>().timeScale = 0;
            LeanTween.move(GetComponent<RectTransform>(),pos[i], 1f).setOnComplete(() => {
                GetComponent<SkeletonGraphic>().timeScale = 2;
                LeanTween.delayedCall(0.5f, () => {
                    LeanTween.dispatchEvent(TextEvent.SOUND, "appear");
                });
                LeanTween.delayedCall(1, () => {
                    GetComponent<SkeletonGraphic>().timeScale = 0;
                });
            });
        }
        public void goOut()
        {
            LeanTween.move(GetComponent<RectTransform>(),_oldPos,1f);
        }
    }
}
