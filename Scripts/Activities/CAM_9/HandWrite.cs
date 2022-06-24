using UnityEngine.UI;
using UnityEngine;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
namespace CAM_9
{
    [System.Serializable]
    public class HandWrite
    {
        private Transform _hand;
        public MoveByPoint[] movebyPoint;
        public void Write(Transform hand)
        {
            _hand = hand;
            float time = 0;
            hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "hand_write", true);
            movebyPoint.ToObservable().Subscribe(m => {
                m.Item.SetActive(false);
                LeanTween.delayedCall(time +=m.Total(), () => {
                   
                    if (m.Item.GetComponent<Write>() != null)
                    {
                        m.Item.SetActive(true);
                        m.Item.GetComponent<Write>().delay = m.miniDelay;
                        LeanTween.delayedCall(m.miniDelay, () => m.Item.GetComponent<Write>().Effect());
                    }
                    else
                    {
                        LeanTween.delayedCall(m.miniDelay, () => {
                            m.Item.SetActive(true);
                        });
                    }
                    int j = 0;
                    m.Points.ToObservable().Subscribe(p =>
                    {
                        LeanTween.moveLocal(_hand.gameObject, p.localPosition + new Vector3(200, -100, 0), m.miniDelay)
                        .setDelay(m.miniDelay * j++).setOnStart(() => { LeanTween.dispatchEvent(Cam9Event.SOUND, "writing"); });
                    });
                });
            });
            LeanTween.delayedCall(time + 1, () => {
                hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "hand_off", false);
                //LeanTween.dispatchEvent(Cam9Event.WRITEEND,_hand);
                LeanTween.moveLocal(_hand.gameObject, new Vector3(610, 262, 0), 0.5f);
            });
        }
        public void reset()
        {

            movebyPoint.ToObservable().Subscribe(x => {
                x.Item.SetActive(false);
                if(x.Item.GetComponent<Write>()!=null)
                    x.Item.GetComponent<Write>().Reset();
            });
        }
    }
}