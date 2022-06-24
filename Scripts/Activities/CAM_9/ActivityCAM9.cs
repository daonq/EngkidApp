using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace CAM_9
{
    public class ActivityCAM9 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brigeToResult;
        public Contents[] contents;
        public Tivi tivi;
        public AutoBook autoBook;
        public Check check;
        public LineByHand linebyHand;
        private Answer answer;
        private int _count = 0;
        private int _score = 0;
        public Dictionary<string, int> dicClips = new Dictionary<string, int>();
        private float time=0;
        private int _countFail = 0;
        private bool IsTutorial = true;
        public Tutorial mctutorial;
        private void Awake()
        {
            addDic();
            LeanTween.addListener(gameObject,Cam9Event.ACTION,OnEvent);
            LeanTween.addListener(gameObject,Cam9Event.SOUND,OnSoundEvent);
            LeanTween.addListener(gameObject,Cam9Event.WRITEEND,OnLineHand);
            LeanTween.addListener(gameObject,Cam9Event.INPUT,OnInputEvent);
        }
        void addDic()
        {
            dicClips.Add("back", 0);
            dicClips.Add("tapTextFiled", 1);
            dicClips.Add("correct", 2);
            dicClips.Add("eraser", 3);
            dicClips.Add("wrong", 4);
            dicClips.Add("keyfail", 5);
            dicClips.Add("highlinePic", 6);
            dicClips.Add("highlineText", 7);
            dicClips.Add("appear", 8);
            dicClips.Add("writing", 9);
            dicClips.Add("keyyes", 10);
            dicClips.Add("tiviTurnOn", 11);
            dicClips.Add("TiviEffect", 12);
            dicClips.Add("paper", 12);
            dicClips.Add("cross", 14);
        }
        private void Start()
        {
            linebyHand.gameObject.SetActive(false);
            answer = contents[_count].answer;
            transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
            //LeanTween.delayedCall(0.3f, tutorial);
            OpenTutorial();
        }
        public void OpenTutorial()
        {
            LeanTween.dispatchEvent(TextEvent.SOUND, "tapTextFiled");
            StartCoroutine(mctutorial.Begin());
        }
        void tutorial()
        {
            LeanTween.dispatchEvent(Cam9Event.SOUND, "tiviTurnOn");
            LeanTween.delayedCall(0.3f, () => {
                tivi.Open(contents[_count].id);
                autoBook.FirstPage();
                LeanTween.delayedCall(2, () => {
                    autoBook.goWrite(contents[_count].answer.sttInPage, contents[_count].id);
                });
            });
        }
        public void ContentTutorial()
        {
            //answer._input.placeholder.GetComponent<Text>().text = contents[_count].Yes[0];
            answer.isTutorial = contents[_count].isTutorial;
            answer.content = contents[_count].Yes[0];
            answer.clipByTime = contents[_count].clipByTime;
            check.begin();
            Invoke("SoundHelp", 20);
        }
        void OnSoundEvent(LTEvent e)
        {
            string str = e.data as string;
            soundManager.Play(dicClips[str]);
            if (str == "tapTextFiled") CancelInvoke("SoundHelp");
        }
        void OnLineHand(LTEvent e)
        {
            if (_count<contents.Length && contents[_count].isTutorial && IsTutorial)
            {
                //linebyHand.gameObject.SetActive(true);
                linebyHand.OnGo();
                soundManager.PlayByTime(0);
                
            }
        }
        public void resetHelpSound()
        {
            CancelInvoke("SoundHelp");
            Invoke("SoundHelp", 20);
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case "Pic":
                    tivi.Hand();
                    LeanTween.delayedCall(1, () => soundManager.PlayByTime(1));
                    LeanTween.delayedCall(3, () => { 
                        //linebyHand.Tap();
                        answer.Glow();
                        LeanTween.delayedCall(1, () => {
                            soundManager.PlayByTime(2);
                            if (contents[_count].isTutorial) ContentTutorial();
                        });
                    });
                    break;
                case "Check":
                    OnCheck();
                    break;
                case "Hint":
                    OnHint();
                    break;
                case "hideHint":
                    answer.closeHint();
                    break;
                case Cam9Event.START:
                    //if (mctutorial.isTutorial) soundManager.Play(12);
                    mctutorial.isTutorial = false;
                    mctutorial.gameObject.SetActive(false);
                    LeanTween.dispatchEvent(Cam9Event.SOUND, "tiviTurnOn");
                    LeanTween.delayedCall(0.3f, () => {
                        tivi.Open(contents[_count].id);
                        autoBook.FirstPage();
                        LeanTween.delayedCall(2, () => {
                            autoBook.goWrite(contents[_count].answer.sttInPage, contents[_count].id);
                        });
                    });
                    break;
                default:
                    break;
            }
        }
        void OnHint()
        {
            answer.Hint();
        }
        void OnCheck()
        {
            var isCheck = contents[_count].Yes.Where(x => x.ToLower() == answer._input.text.ToLower()).ToArray().Length > 0;
           if (isCheck)
            {
                CancelInvoke("SoundHelp");
                answer.Yes();
                LeanTween.delayedCall(1 + contents[_count].clipByTime[0].time, Next);
            }
            else
            {
                CancelInvoke("SoundHelp");
                answer.No();
                _countFail++;
                if (_countFail == 1)
                {
                    soundManager.PlayByTime(Random.Range(5, 7));
                }
                if (_countFail == 2)
                {
                    check.showHint();
                    soundManager.PlayByTime(Random.Range(8, 10));
                }
                if (_countFail == 3)
                {
                    answer.MaxFail();
                    LeanTween.delayedCall(1 + contents[_count].clipByTime[1].time, Next).setDelay(0.5f);
                }
            }
        }
        void Next()
        {
            check.closeHint();
            answer.closeHint();
            if (_countFail == 0) _score += 3;
            if (_countFail == 1) _score += 2;
            if (_countFail == 2) _score += 1;
            if (_countFail == 3) _score += 1;
            _count++;
            _countFail = 0;
            if (_count < contents.Length)
            {
                answer = contents[_count].answer;
                answer.isTutorial = contents[_count].isTutorial;
                answer.content = contents[_count].Yes[0];
                answer.clipByTime = contents[_count].clipByTime;
                autoBook.goWrite(contents[_count].answer.sttInPage, contents[_count].id);
                if (contents[_count].answer.sttInPage == 0)
                {
                    if(_count==0) tivi.Open(contents[_count].id);
                    else
                    {
                        tivi.OpenByAlpha(contents[_count].id);
                    }
                }
                resetHelpSound();
            }
            else
            {
                brigeToResult._score = _score;
                CancelInvoke("SoundHelp");
                brigeToResult.ShowPopup();
                
            }
            
        }
        void OnInputEvent(LTEvent e)
        {
            answer = (e.data as Transform).GetComponent<Answer>();
            check.Active();
        }
        public void Again()
        {
            _count = 0;
            _score = 0;;
            autoBook.Reset();
            tivi.Reset();
            IsTutorial = false;
            LeanTween.delayedCall(2, ()=> {
                linebyHand.gameObject.SetActive(false);
                answer = contents[_count].answer;
                tutorial();
            });
        }
        public void removeListenner()
        {
            LeanTween.addListener(gameObject, Cam9Event.ACTION, OnEvent);
            LeanTween.addListener(gameObject, Cam9Event.SOUND,OnSoundEvent);
        }
        public void SoundHelp() {
            time = soundManager.PlayByTime(2);
            Invoke("SoundHelp", 15 + time);
        }

        public void BackToLastScene()
        {
            SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
        }
        void OnApplicationFocus(bool hasFocus)
        {
            Application.targetFrameRate = hasFocus ?300:0;
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
