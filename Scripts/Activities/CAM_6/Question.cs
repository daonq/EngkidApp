using UnityEngine;
using UniRx;
using common;
using System.Linq;
namespace common
{
    [System.Serializable]
    public class Question
    {
        public int ID = 0;
        public string Q1;
        public string Q2;
        public string YES;
        public string NO;
        public bool isTutorial = false;
        public ClipByTime[] clipByTime;
        public GameObject[] highlines;
        public string page;
        public float timeHighline = 1.5f;
        public void HighLine()
        {
            int i = 0;
            highlines.ToObservable().Subscribe(x=>LeanTween.delayedCall(timeHighline*i++,()=> {
                x.SetActive(true);
                LeanTween.alpha(x.GetComponent<RectTransform>(),1f, 0.2f).setFrom(0f).setRepeat(5);
                //LeanTween.dispatchEvent(TextEvent.SOUND,"highlinePic");
            }));
        }
        public void HighLineEnd()
        {
            int i = 0;
            highlines.ToObservable().Subscribe(x => LeanTween.delayedCall(1.5f*i++, () => {
                x.SetActive(true);
                LeanTween.alpha(x.GetComponent<RectTransform>(), 1f, 0.2f).setFrom(0f).setRepeat(4);
                if (x.transform.childCount > 0)
                {
                    x.transform.GetChild(0).gameObject.SetActive(true);
                    LeanTween.delayedCall(0.8f, () => { x.transform.GetChild(0).gameObject.SetActive(false); });
                }
                LeanTween.dispatchEvent(TextEvent.SOUND, "highlinePic");
            }));
        }
        public void clear()
        {
            highlines.ToObservable().Subscribe(x => {
                x.SetActive(false); 
            });
        }
    }
}