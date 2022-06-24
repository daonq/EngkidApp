using UnityEngine;
using Spine.Unity;
using UniRx;
namespace CAM_9
{
    public class Tivi : MonoBehaviour
    {
        public SkeletonGraphic spine;
        public GameObject[] pics;
        public GameObject[] cricles;
        public SkeletonGraphic hand;
        public Transform p;
        private int _channel = -1;
        private void Start()
        {
            clear(true);
        }
        public void Open(int channel)
        {
            _channel = channel;
            clear(false);
            LeanTween.dispatchEvent(Cam9Event.SOUND, "TiviEffect");
            LeanTween.delayedCall(1, () =>
            {
                cricles[_channel].transform.GetChild(0).transform.gameObject.SetActive(true);
                pics[_channel].SetActive(true);
            });
        }
        public void OpenByAlpha(int channel)
        {
            _channel = channel;
            clear(true);
            LeanTween.dispatchEvent(Cam9Event.SOUND, "cross");
            LeanTween.delayedCall(1, () =>
            {
                cricles[_channel].transform.GetChild(0).transform.gameObject.SetActive(true);
                pics[_channel].SetActive(true);
                LeanTween.alpha(pics[_channel].GetComponent<RectTransform>(), 1, 0.5f).setFrom(0);
            });
        }
        public void Reset()
        {
            clear(true);
        }
        public void clear(bool isClear)
        {
            pics.ToObservable().Subscribe(x => x.SetActive(false));
            cricles.ToObservable().Subscribe(x => {
                x.transform.GetChild(0).transform.gameObject.SetActive(false);
            });
            spine.AnimationState.SetAnimation(0,isClear?"tivi_effect_1": "tivi_effect_2", false);
        }
        public void Hand()
        {
            /*
            LeanTween.moveLocal(hand.gameObject, p.transform.localPosition, 1).setOnComplete(() => {
                hand.AnimationState.SetAnimation(0, "tap", false);
                LeanTween.delayedCall(1, () => hand.gameObject.SetActive(false));
            });*/
        }
    }
}
