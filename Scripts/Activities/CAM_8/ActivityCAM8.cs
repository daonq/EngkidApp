using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
namespace cam8
{
    public class ActivityCAM8 : MonoBehaviour
    {
        public SoundManager soundManager;
        public GameObject m_Lesson;
        public GameObject resultPopup;
        private int _score;
        public AutoScrollByPos autoScrollByPos;
        public GridPic gridPic;
        public Button btncheck;
        private InputField[] _inputFieldList;
        private InputField[] _inputFieldListEnd;
        public Dictionary<string, int> dicClips = new Dictionary<string, int>();
        public SkeletonGraphic highLineText;
        public float timeHandText = 2;
        public Tutorial mctutorial;
        void Awake()
        {
            LeanTween.addListener(gameObject,TextEvent.ACTION,OnEvent);
            LeanTween.addListener(gameObject,TextEvent.CHECK,OnEventEdit);
            LeanTween.addListener(gameObject,TextEvent.SOUND,(e) => { soundManager.Play(dicClips[e.data as string]); });
            _inputFieldList = autoScrollByPos.listWord.Select(x => x.GetComponent<InputField>()).ToArray().Where(x=>!x.GetComponent<InputWord>().isTutorial).ToArray();
            dicClips.Add("keyfail", 8);
            dicClips.Add("keyyes", 9);
            dicClips.Add("tap", 10);
            dicClips.Add("fail", 11);
            dicClips.Add("hand", 12);
            dicClips.Add("yes0", 13);
            dicClips.Add("yes1", 14);
            dicClips.Add("yes2", 15);
            dicClips.Add("no0", 16);
            dicClips.Add("no1", 17);
            dicClips.Add("no2", 18);
            _inputFieldList.ToObservable().Subscribe(x => x.enabled = false);
        }
        public void OpenTutorial()
        {
            LeanTween.dispatchEvent(TextEvent.SOUND, "tap");
            StartCoroutine(mctutorial.Begin());
        }
        void Start()
        {
            //soundManager.SoundTutorial();
            highLineText.gameObject.SetActive(false);
            //LeanTween.delayedCall(5, () => gridPic.Begin());
            OpenTutorial();
            _countCheck = 0;
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case TextEvent.HIGHLINE:
                    highLineText.gameObject.SetActive(true);
                    highLineText.AnimationState.SetAnimation(0, "ban tay di chuyen chi sang cau mo ta theo duong thang.spine", false);
                    LeanTween.delayedCall(timeHandText, () => {
                        highLineText.gameObject.SetActive(false);
                        autoScrollByPos.Highline(true);
                        autoScrollByPos.listWord[0].GetComponent<InputField>().enabled = true;
                        autoScrollByPos.listWord[0].GetComponent<InputField>().placeholder.gameObject.SetActive(true);
                        gridPic.m_lock.SetActive(false);
                        LeanTween.delayedCall(20, () => { 
                            autoScrollByPos.Highline(false);
                            if(!autoScrollByPos.listWord[0].GetComponent<InputField>().enabled) soundManager.Play(2);
                        });
                    });
                    break;
                case TextEvent.START:
                    _inputFieldList.ToObservable().Subscribe(x => x.enabled = true);
                    if(mctutorial.isTutorial) soundManager.Play(12);
                    mctutorial.isTutorial = false;
                    mctutorial.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        void OnEventEdit(LTEvent e)
        {
            InputField currentField = e.data as InputField;
            var checkEnable = _inputFieldList.Where(x => x.text!="").ToArray();
            checkEnable.ToObservable().Subscribe(xx => {
                xx.GetComponent<InputWord>().highLine(false);
                xx.GetComponent<InputWord>().Mypic.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            });
            if (checkEnable.Length == autoScrollByPos.listWord.Length)
            {
                
                if (btncheck.gameObject.activeSelf)
                {
                    btncheck.interactable = true;
                } else {
                    OnCheck(false);
                    var list = Shuffle.createList(3);
                    string yes = "yes" + list[Random.Range(0, 2)];
                    LeanTween.dispatchEvent(TextEvent.SOUND, yes);
                }
            }
            if (checkEnable.Length != 0 && currentField.GetComponent<InputWord>().isHelpFinish) checkOut(currentField);
        }
        private void checkOut(InputField currentField)
        {
            if (currentField.GetComponent<InputWord>().id==currentField.text.ToLower())
            {
                currentField.enabled = false;
                currentField.textComponent.color = Color.green;
                currentField.GetComponent<InputWord>().Mypic.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                LeanTween.alpha(currentField.GetComponent<InputWord>().Mypic.GetComponent<RectTransform>(),0.5f,0.5f).setFrom(0f);
                /*
                if (currentField.name == "0")
                {
                    LeanTween.delayedCall(0.5f, () => soundManager.Play(3));
                    LeanTween.delayedCall(1, Begin);
                }
                else {
                    */
                /*
                    var checkEnable = _inputFieldList.Where(x => x.enabled).ToArray();
                    if (checkEnable.Length == 0)
                    {
                    Debug.Log("checkEnable.Length == 0");
                        LeanTween.delayedCall(3,ShowPopup);
                    } else {
                        LeanTween.delayedCall(0.5f, () => soundManager.Play(3));
                    }*/
                //}
            }
            else
            {
                currentField.textComponent.color = Color.red;
                var list = Shuffle.createList(3);
                string no = "no" + list[Random.Range(0, 2)];
                LeanTween.dispatchEvent(TextEvent.SOUND, no);
                LeanTween.delayedCall(5, () => {
                    currentField.GetComponent<InputWord>().highLine(true);
                    currentField.GetComponent<InputWord>().Mypic.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    if (currentField.GetComponent<InputWord>().isNext) gridPic.Next();
                    else gridPic.Back();
                    LeanTween.delayedCall(0.5f, () => soundManager.Play(2));
                });
            }
        }
        void Begin()
        {
            _inputFieldList.ToObservable().Subscribe(x => x.enabled = true);
            autoScrollByPos.Highline(false);
            btncheck.interactable = true;
            gridPic.m_lock.SetActive(false);
        }
        public void check()
        {
            OnCheck(true);
        }
        public void OnCheck(bool ischeck)
        {
            if (ischeck) _score=0;
            _inputFieldList.ToObservable().Subscribe(x => {
                if (x.GetComponent<InputWord>().id==x.text.ToLower())
                {
                    x.enabled = false;
                    x.textComponent.color = Color.green;
                    LeanTween.alpha(x.GetComponent<InputWord>().Mypic.GetComponent<RectTransform>(), 0.5f, 0.5f).setFrom(0f);
                    if(ischeck) _score++;
                } else {
                    x.textComponent.color = Color.red;
                    x.GetComponent<InputWord>().isHelpFinish = true;
                }
            });
            btncheck.gameObject.SetActive(false);
            var checkBonus = _inputFieldList.Where(x =>!x.enabled).ToArray();
            _inputFieldListEnd = _inputFieldList.Where(x =>x.enabled).ToArray();
            if (checkBonus.Length == _inputFieldList.Length)
            {
                _inputFieldList.ToObservable().Subscribe(x => x.enabled = false);
                gridPic.m_lock.SetActive(false);
                LeanTween.delayedCall(5,ShowPopup);
                soundManager.Play(6);
            } 
            else
            {
                soundManager.Play(7);
                _countCheck++;
                if(_countCheck>1) showHelpfinish();
            }
        }
        private int _countCheck = 0;
        private void showHelpfinish()
        {
            //_inputFieldListEnd[0].GetComponent<InputWord>().highLine(true);
            _inputFieldListEnd[0].GetComponent<InputWord>().Mypic.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            if (_inputFieldListEnd[0].GetComponent<InputWord>().isNext)
            {
                gridPic.Next();
            }
            else gridPic.Back();
        }
        int Bonus(int score)
        {
            int countStar = 0;
            Debug.Log("score:" + score);
            if (score==5 || score==6) countStar = 3;
            if (score == 3 || score == 4) countStar = 2;
            if (score == 1 || score == 2) countStar = 1;
            return countStar;
        }
        void ShowPopup()
        {
            Debug.Log("ShowPopup");
            resultPopup.SetActive(true);
            resultPopup.transform.GetChild(1).transform.localScale = Vector3.zero;
            int starBonus = Bonus(_score);
            resultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(starBonus);
        }
        public void OnReturnToActivityScreen()
        {
            soundManager.Play(4);
            LeanTween.delayedCall(0.3f,() => {
                SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
            });
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
        public void PlayAgain()
        {
            _score = 0;
            soundManager.Play(4);
            resultPopup.SetActive(false);
            autoScrollByPos.clear();
            autoScrollByPos.listWord.ToObservable().Subscribe(x =>
            {
                x._inputField.text = "";
                x._inputField.enabled = true;
                x._inputField.placeholder.GetComponent<Text>().text="";
                LeanTween.alpha(x.Mypic.GetComponent<RectTransform>(),1,0.3f).setFrom(0f);
            });
            LeanTween.delayedCall(0.3f,() =>Start());
            _countCheck = 0;
        }
    }
}
