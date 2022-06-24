using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace CAM_6
{
    public class ActivityCAM6 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public Question[] Questions;
        public BrigeToResult brigeToResult;
        public Text content;
        public InputContent inputContent;
        private int _count = 0;
        public Hand hand;
        public RectTransform window;
        public Dictionary<string, int> dicClips = new Dictionary<string, int>();
        private float time=0;
        public Tutorial mctutorial;
        private void Awake()
        {
            dicClips.Add("keyfail", 5);
            dicClips.Add("keyyes", 10);
            dicClips.Add("highlinePic", 6);
            dicClips.Add("highlineText", 7);
            dicClips.Add("appear",8);
            dicClips.Add("back", 0);
            dicClips.Add("tapTextFiled", 1);
            dicClips.Add("correct", 2);
            dicClips.Add("eraser", 3);
            dicClips.Add("wrong", 4);
            dicClips.Add("writing", 9);
            LeanTween.addListener(gameObject,TextEvent.ACTION,OnEvent);
            LeanTween.addListener(gameObject,TextEvent.SOUND,OnSoundEvent);
        }
        void Start()
        {
            BGMManagerBehavior.GetInstance().PauseBGM();
            OpenTutoral();
        }
        public void OpenTutoral()
        {
            StartCoroutine(mctutorial.Begin());
        }
        private void clearHighLine()
        {
            Questions.ToObservable().Subscribe(x=> {
                x.clear();
            });
        }
        private void setContent(int N)
        {
            content.text = Questions[N].Q1;
            inputContent.gameObject.SetActive(true);
            inputContent.content = Questions[N].Q2;
            inputContent._input.enabled = false;
            inputContent.isTutorial = Questions[N].isTutorial;
            inputContent.clipByTime = Questions[N].clipByTime;
        }
        private void tutorial()
        {
            setContent(_count);
            content.GetComponent<Outline>().enabled = false;
            inputContent.clear();
            inputContent._input.enabled = true;
            soundManager.PlayByTime(0);
            /*
            LeanTween.delayedCall(2, () =>
            {
                //Questions[_count].HighLine();
                soundManager.PlayByTime(0);
                LeanTween.delayedCall(3,ContentTutorial);
            });*/
        }
        void ContentTutorial()
        {
            hand.go(1);
            soundManager.PlayByTime(4);
            content.GetComponent<Outline>().enabled = true;
            LeanTween.dispatchEvent(TextEvent.SOUND, "appear");
            //inputContent._input.placeholder.GetComponent<Text>().text = inputContent.content;
            inputContent._input.enabled = true;
            LeanTween.delayedCall(2,() =>
            {
                hand.goOut();
                LeanTween.delayedCall(5, () => {
                    content.GetComponent<Outline>().enabled = false;
                    Invoke("SoundHelp", 10);
                });
            });
        }
        void OnSoundEvent(LTEvent e)
        {
            string str = e.data as string;
            soundManager.Play(dicClips[str]);
            if (str == "tapTextFiled") CancelInvoke("SoundHelp");
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
                case TextEvent.HIGHLINE:
                    Questions[_count].HighLine();
                    LeanTween.delayedCall(0.5f,() => {
                        //time = soundManager.PlayByTime(0);
                        resetHelpSound();
                    });
                    break;
                case TextEvent.HIGHLINEEND:
                    resetHelpSound();
                    break;
                /*
                Questions[_count].HighLineEnd();
                CancelInvoke("SoundHelp");
                LeanTween.delayedCall(2, () => {
                    Invoke("SoundHelp", 15+this.time);
                });
                */
                case TextEvent.START:
                    LeanTween.move(window, new Vector3(1900, 0, 0), 1.5f).setOnComplete(tutorial);
                    break;
                case TextEvent.BEGIN:
                    
                    inputContent._count = 0;
                    inputContent.result = "";
                    /*
                    if (Questions[_count].isTutorial)
                    {
                        time = soundManager.PlayByTime(Random.Range(1,3));
                    }*/
                    LeanTween.delayedCall(1,Play);
                    break;
                default:
                    break;
            }
        }
        private void Play()
        {
            _count++;
            if (_count < Questions.Length)
            {
                setContent(_count);
                inputContent.clear();
                clearHighLine();
                inputContent._input.text = "";
                inputContent._input.enabled = true;
                inputContent._input.placeholder.GetComponent<Text>().text = "";
                inputContent._input.textComponent.color = Color.white;
                resetHelpSound();
            } 
            else {
                brigeToResult._score = inputContent.Score;
                brigeToResult.ShowPopup();
                StopAllCoroutines();
            }
        }
        public void Again()
        {
            _count = 0;
            inputContent.Score = 0;
            inputContent._input.text = "";
            clearHighLine();
            tutorial();
        }
        public void removeListenner()
        {
            LeanTween.addListener(gameObject, TextEvent.ACTION, OnEvent);
            LeanTween.addListener(gameObject, TextEvent.SOUND,OnSoundEvent);
        }
        public void SoundHelp() {
            time = soundManager.PlayByTime(4);
            Invoke("SoundHelp", 15 + time);
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
        public void BackToLastScene()
        {
           // SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject);
            SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
        }
    }
}
