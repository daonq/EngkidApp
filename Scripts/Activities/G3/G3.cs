using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
namespace G3
{
    public class Event
    {
        public const int ACTION = 3;
        public const int SOUND = 5;
        public const string BEGIN = "Begin";
        public const string CHANGE = "change";
        public const string FAIL = "fail";
        public const int YES = 0;
        public const int WAKE_UP = 0;
        public const int TOOTHBRUSH = 1;
        public const int TOWEL = 2;
        public const int CLOTHES = 3;
        public const int SCHOOL = 4;
        public const int EATING = 5;
        public const int BUS = 6;
    }
    public class G3 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brige;
        public GameObject UI;
        public SceneBySpine end;
        public Tutorial tutorial;
        public Contents[] contents;
        private Dictionary<string, int> _dicClips = new Dictionary<string, int>();
        private int _count = 0;
        private Head _header;
        private GameObject _footer;
        private Frame[] boxs = new Frame[2];
        private Drag[] drags = new Drag[2];
        private AudioSource _audio;
        private int _score = 0;
        public int _fail = 0;
        private SceneBySpine _sceneBySpine;
        private void Awake()
        {
            _dicClips.Add("back", 0);
            _dicClips.Add("tap", 1);
            _dicClips.Add("yes", 2);
            _dicClips.Add("no", 3);
            _dicClips.Add("appear", 4);
            _dicClips.Add("paper", 5);
            _dicClips.Add("click", 6);
            _dicClips.Add("drag", 1);
            _dicClips.Add("yes1", 7);
            _dicClips.Add("yes2", 8);
            _dicClips.Add("yes3", 9);
            _dicClips.Add("no1", 10);
            _dicClips.Add("no2", 11);
            _dicClips.Add("no3", 12);
            _dicClips.Add("btn", 13);
            _header = UI.transform.GetChild(0).GetComponent<Head>();
            _footer = UI.transform.GetChild(1).gameObject;
            LeanTween.addListener(gameObject, Event.ACTION, OnEvent);
            LeanTween.addListener(gameObject, Event.SOUND, OnSoundEvent);
            LeanTween.addListener(gameObject, Event.YES, OnYesEvent);
            boxs[0] = _footer.transform.GetChild(0).GetComponent<Frame>();
            boxs[1] = _footer.transform.GetChild(1).GetComponent<Frame>();
            drags[0] = boxs[0].transform.GetChild(4).GetComponent<Drag>();
            drags[1] = boxs[1].transform.GetChild(4).GetComponent<Drag>();
            _audio = GetComponent<AudioSource>();
        }
        public void removeListenner()
        {
            LeanTween.removeListener(gameObject, Event.ACTION, OnEvent);
            LeanTween.removeListener(gameObject, Event.SOUND, OnSoundEvent);
        }
        public void Again()
        {
            _count = 0;
            boxs[0].gameObject.SetActive(true);
            boxs[1].gameObject.SetActive(true);
            tutorial.Istutorial = false;
            tutorial.gameObject.SetActive(false);
            end.gameObject.SetActive(false);
            hideUI(true);
            Begin();
        }
        void Start()
        {
            Begin();
            LeanTween.delayedCall(3, () => {
                if (tutorial.Istutorial) OpenTutorial();
            });
        }
        public void OpenTutorial()
        {
            if(_sceneBySpine!=null) _sceneBySpine.Close();
            tutorial.gameObject.SetActive(true);
            StartCoroutine(tutorial.Play());
            soundManager.SoundContentOn(true);
            LeanTween.dispatchEvent(Event.SOUND, "tap");
        }
        public void CloseTutorial()
        {
            StopCoroutine(tutorial.Play());
            LeanTween.dispatchEvent(Event.SOUND, "back");
            tutorial.Istutorial = false;
            tutorial.gameObject.SetActive(false);
            soundManager.SoundContentOn(false);
        }
        void OnSoundEvent(LTEvent e)
        {
            soundManager.Play(_dicClips[e.data as string]);
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case Event.BEGIN:
                    CloseTutorial();
                    break;
                case Event.CHANGE:
                    Change();
                    break;
                case Event.FAIL:
                    _fail = 1;
                    StopAllCoroutines();
                    //StopCoroutine(glow());
                    StartCoroutine(glow());
                    break;
                default:
                    break;
            }
        }
        void OnYesEvent(LTEvent e)
        {
            Frame frame = e.data as Frame;
            Frame other = boxs.Where(x => x != frame).ToArray()[0];
            other.transform.GetChild(4).GetComponent<Drag>().OnShowDisable();
            StartCoroutine(OnYes());
        }
        void Begin()
        {
            tutorial.gameObject.SetActive(false);
            UI.SetActive(false);
            SetContent(_count);
            end.gameObject.SetActive(false);
        }
        public IEnumerator glow()
        {
            Debug.Log("glow");
            yield return new WaitForSeconds(20);
            boxs.ToObservable().Subscribe(x => {
                if (!x.transform.GetChild(3).gameObject.activeSelf)
                {
                    x.Glow();
                }
            });
            yield return StartCoroutine(glow());
        }
        void SetContent(int i)
        {
            StopAllCoroutines();
            contents.ToObservable().Subscribe(x => { 
                x.scene.gameObject.SetActive(false);
                x.pic.gameObject.SetActive(false);
            });
            _header.gameObject.SetActive(true);
            _header.transform.GetChild(0).GetComponent<Text>().text = contents[_count].sentences;
            boxs[0].txts[0].text = contents[_count].words[0];
            boxs[0].txts[1].text = contents[_count].words[0];
            boxs[1].txts[0].text = contents[_count].words[1];
            boxs[1].txts[1].text = contents[_count].words[1];
            contents[_count].pic.gameObject.SetActive(true);
            _header.Content = contents[_count].Yes;
            contents.ToObservable().Subscribe(x => {
                x.scene.gameObject.SetActive(false);
                x.scene.enabled = false;
            });

            contents[_count].scene.source.loop = false;
            _sceneBySpine = contents[_count].scene;
            _sceneBySpine.gameObject.SetActive(true);
            _sceneBySpine.enabled = true;
            UI.SetActive(false);
            SkeletonGraphic bone = _sceneBySpine.GetComponent<SkeletonGraphic>();
                LeanTween.value(bone.gameObject, new Color(bone.color.r, bone.color.g, bone.color.b, 0), new Color(bone.color.r, bone.color.g, bone.color.b, 1f), 0.3f).setOnUpdate((Color val) =>
                {
                    bone.color = val;
                }).setOnComplete(() => {
                    StartCoroutine(_sceneBySpine.Begin(_count));
                    LeanTween.delayedCall(!tutorial.Istutorial?1:9, () =>
                    {
                        UI.SetActive(true);
                        LeanTween.dispatchEvent(Event.SOUND, "btn");
                        LeanTween.delayedCall(0.5f, () => {
                          float time = soundManager.PlayByTime(0);
                            LeanTween.delayedCall(time, () => StartCoroutine(glow()));
                        });
                    });
                });
        }
        IEnumerator OnYes()
        {
            StopCoroutine(glow());
            _sceneBySpine.Close();
            string yescontent = "<color=#45f248>" + contents[_count].Yes + "</color>";
            _header.transform.GetChild(0).GetComponent<Text>().text = contents[_count].sentences.Replace("___", yescontent);
            yield return new WaitForSeconds(1.5f);
            _audio.clip = contents[_count].clipByTime.clip;
            _audio.Play();
            _header.transform.GetChild(1).gameObject.SetActive(true);
            if (_fail == 0) _score += 1;
            yield return new WaitForSeconds(contents[_count].clipByTime.time + 1);
            hideUI(false);
            _header.transform.GetChild(1).gameObject.SetActive(false);
            _header.gameObject.SetActive(false);
            StartCoroutine(_sceneBySpine.End());
        }
        void hideUI(bool isShow)
        {
            _footer.GetComponent<Image>().enabled = isShow;
            boxs[0].gameObject.SetActive(isShow);
            boxs[1].gameObject.SetActive(isShow);
            _footer.transform.GetChild(2).gameObject.SetActive(isShow);
        }
        void Change()
        {
            boxs[0].close();
            boxs[1].close();
            drags[0].close();
            drags[1].close();
            _count++;
            if (_count < contents.Length) Next(); else StartCoroutine(End());
        }
        void Next()
        {
            hideUI(true);
            _fail = 0;
            if(_count<contents.Length) SetContent(_count);
        }
        private IEnumerator End()
        {
            contents.ToObservable().Subscribe(x => x.scene.gameObject.SetActive(false));
            end.gameObject.SetActive(true);
            yield return end.Begin(Event.BUS);
            yield return new WaitForSeconds(1);
            yield return end.End();
            yield return new WaitForSeconds(0.5f);
            Win();
        }
        void Win()
        {
            end.Close();
            StopAllCoroutines();
            if (_score == 0) _score = 1;
            brige._score = _score;
            brige.ShowPopup();
        }
    } 
 }

