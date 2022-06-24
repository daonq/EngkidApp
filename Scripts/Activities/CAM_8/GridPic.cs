using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
namespace common
{
    public class GridPic : MonoBehaviour
    {
        private Vector3 _PosOld;
        public Vector3 PosNew;
        public Vector3 PosTutorial;
        public RectTransform Grid;
        public GameObject Hand;
        public Transform[] checkPoints;
        public GameObject[] highlines;
        public GameObject btnNext;
        public GameObject btnBack;
        public GameObject picTutorial;
        public GameObject[] pics;
        public GameObject m_lock;
        void Awake()
        {
            _PosOld = Grid.transform.localPosition;
        }
        public void Begin()
        {
            m_lock.SetActive(true);
           // autoHighLine();
        }
        public void Next()
        {
            btnNext.GetComponent<Button>().interactable = false;
            btnBack.GetComponent<Button>().interactable = true;
            LeanTween.move(Grid,m_lock.activeSelf?PosTutorial:PosNew,0.5f);
        }
        public void Back()
        {
            btnNext.GetComponent<Button>().interactable = true;
            btnBack.GetComponent<Button>().interactable = false;
            LeanTween.move(Grid,_PosOld,0.5f);
        }
        public void autoHighLine()
        {
            Hand.transform.localPosition = checkPoints[0].localPosition;
            Hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
            Hand.GetComponent<SkeletonGraphic>().timeScale = 1f;
            LeanTween.delayedCall(1, () => {
                LeanTween.dispatchEvent(TextEvent.SOUND, "tap");
            });
            LeanTween.delayedCall(2,() => {
               Hand.GetComponent<SkeletonGraphic>().timeScale = 0;
                highlines[1].SetActive(true);
                LeanTween.delayedCall(1,Next);
                LeanTween.delayedCall(1.5f,()=> { 
                    highlines[1].SetActive(false);
                    autoHighLineBack();
                });
            });
        }
        public void autoHighLineBack()
        {
            Hand.transform.localPosition = checkPoints[1].localPosition;
            Hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,"animation", false);
            Hand.GetComponent<SkeletonGraphic>().timeScale = 1f;
            LeanTween.delayedCall(1, () => {
                LeanTween.dispatchEvent(TextEvent.SOUND, "tap");
            });
            LeanTween.delayedCall(2f, () => {
                highlines[0].SetActive(true);
                Hand.GetComponent<SkeletonGraphic>().timeScale = 0;
                LeanTween.delayedCall(1,Back);
                LeanTween.delayedCall(2f, () => {
                    picTutorial.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    Hand.GetComponent<SkeletonGraphic>().timeScale = 1f;
                    Hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
                    Hand.transform.localPosition = checkPoints[2].localPosition;
                    LeanTween.delayedCall(1, () => {
                        LeanTween.dispatchEvent(TextEvent.SOUND, "tap");
                    });
                    highlines[0].SetActive(false);
                    LeanTween.delayedCall(5f, () => {
                        Hand.GetComponent<SkeletonGraphic>().timeScale = 0;
                        Hand.transform.localPosition = new Vector3(2000, 0, 0);
                        LeanTween.dispatchEvent(TextEvent.ACTION, TextEvent.HIGHLINE);
                    });
                });
            });
        }
    }
}
