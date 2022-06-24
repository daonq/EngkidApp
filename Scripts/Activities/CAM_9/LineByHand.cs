using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UniRx;
using UnityEngine.UI.Extensions;
namespace CAM_9
{
    public class LineByHand : MonoBehaviour
    {
        public SkeletonGraphic spine;
        public Transform[] P;
        public Transform[] line;
        private Vector3[] _oldPos=new Vector3[2];
        public float[] length = {-400,-200 };
        private void Awake()
        {
            _oldPos[0] = line[0].transform.localPosition;
            _oldPos[1] = line[1].transform.localPosition;
            line.ToObservable().Subscribe(x => x.GetComponent<Image>().color = Color.yellow);
        }
        public void OnGo()
        {
            Debug.Log("move");
            line[0].transform.localPosition = new Vector3(length[0],_oldPos[0].y,0);
            line[1].transform.localPosition = new Vector3(length[1], _oldPos[1].y, 0);
            spine.gameObject.SetActive(true);
            LeanTween.moveLocal(line[0].gameObject, _oldPos[0], 2);
            LeanTween.moveLocal(spine.gameObject, P[0].localPosition, 2).setOnComplete(() => {
                spine.transform.localPosition = P[2].localPosition;
                LeanTween.moveLocal(line[1].gameObject, _oldPos[1], 2);
                LeanTween.moveLocal(spine.gameObject, P[3].localPosition, 2).setOnComplete(() => {
                    LeanTween.dispatchEvent(Cam9Event.ACTION, "Pic");
                    line.ToObservable().Subscribe(x => x.gameObject.SetActive(false));
                    spine.gameObject.SetActive(false);
                });
            });
        }
        public void Tap()
        {
            spine.gameObject.SetActive(true);
            spine.AnimationState.SetAnimation(0, "tap", false);
            spine.transform.localPosition = P[4].localPosition;
            LeanTween.delayedCall(3, () => { 
                spine.gameObject.SetActive(false);
            });
        }
        
    }
}
