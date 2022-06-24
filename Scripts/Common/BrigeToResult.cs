using UnityEngine;

namespace common
{
    public class BrigeToResult : MonoBehaviour
    {
        public GameObject m_Lesson;
        public GameObject resultPopup;
        public SoundManager soundManager;
        public int MAXSCORE = 6;
        public float[] Star= new float[2] { 0.6f,0.8f};
        public int _score;
        public GameObject gamecurrent;
        public bool IsCeil = false;
        int Bonus(int score)
        {
            int countStar = 0;
            float percent = (float)score / (float)MAXSCORE;
            if (percent > 0 && percent < Star[0]) countStar = 1;
            if (percent >= Star[0] && percent < Star[1]) countStar = 2;
            if (percent >= Star[1]) countStar = 3;
            return countStar;
        }
        int BonusByCeil(int score)
        {
            int countStar = 0;
            float percent = (float)score / (float)MAXSCORE;
            countStar = Mathf.CeilToInt(percent);
            Debug.Log("star:" + countStar);
            return countStar;
        }
        public void ShowPopup()
        {
            resultPopup.SetActive(true);
            resultPopup.transform.GetChild(1).transform.localScale = Vector3.zero;
            LeanTween.dispatchEvent(TextEvent.SOUND, "appear");
            int starBonus = IsCeil? BonusByCeil(_score):Bonus(_score);
            resultPopup.GetComponent<ResultPopupBehavior>().m_XPText.text= "";
            resultPopup.GetComponent<ResultPopupBehavior>().m_DiamondText.text = "";
            LeanTween.scale(resultPopup.transform.GetChild(1).gameObject, Vector3.one, 0.5f).setOnComplete(() => {
                Debug.Log("starBonus:" + starBonus);
                resultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(starBonus);
            });
        }
        public void OnReturnToActivityScreen()
        {
            LeanTween.dispatchEvent(TextEvent.SOUND, "back");
            LeanTween.delayedCall(0.3f, () => {
                //LeanTween.cancelAll();
                SceneManagerBehavior.GetInstance().BackToLastScene(null, true);
            });
        }
        public void PlayAgain()
        {
            _score = 0;
            LeanTween.dispatchEvent(TextEvent.SOUND, "back");
            resultPopup.SetActive(false);
        }
        private void OnEnable()
        {
            //Cursor.visible = false;
            if (BGMManagerBehavior.GetInstance())
                BGMManagerBehavior.GetInstance().PauseBGM();
        }

        private void OnDisable()
        {
            //Cursor.visible = true;
            if (BGMManagerBehavior.GetInstance())
                BGMManagerBehavior.GetInstance().ResumeBGM();
        }
    }
}
