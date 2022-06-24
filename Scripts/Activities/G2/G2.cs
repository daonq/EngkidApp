using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
namespace G2
{
    public class Event
    {
        public const int SCENE = 3;
        public const int SOUND = 5;
        public const int DRAG = 0;
        public const int CHANGE = 1;
        public const string CLOSE = "close";
        public const string BEGIN_DRAG = "Begin_Drag";
        public const string FAIL = "Fail";
        public const string GLOW = "GlowGo";
        public static bool LOCK = false;
    }
    public class G2 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brige;
        public Tutorial tutorial;
        
        public Contents[] contents;
        private Dictionary<string, int> _dicClips = new Dictionary<string, int>();
        public Player player;
        private GameObject _PlayScene;

        private int _count = 0;
        private int _score = 0;
        private int _Fail = 0;
        private float TimeGLow = 15;
        private void Awake()
        {
            _dicClips.Add("back", 0);
            _dicClips.Add("tap", 1);
            _dicClips.Add("yes", 2);
            _dicClips.Add("no", 3);
            _dicClips.Add("jump", 4);
            _dicClips.Add("win", 5);
            _dicClips.Add("click", 6);
            _dicClips.Add("drag", 1);
            _dicClips.Add("yes0", 7);
            _dicClips.Add("yes1", 8);
            _dicClips.Add("yes2", 9);
            _dicClips.Add("no0", 10);
            _dicClips.Add("no1", 11);
            _dicClips.Add("no2", 12);
            _dicClips.Add("Drop", 13);
            _dicClips.Add("sentencesWin", 14);
            _dicClips.Add("jumpWin", 15);
            _dicClips.Add("walk", 16);
            _dicClips.Add("appear", 1);
            _PlayScene = transform.GetChild(0).transform.GetChild(0).gameObject;
            LeanTween.addListener(gameObject, Event.SCENE, OnEvent);
            LeanTween.addListener(gameObject, Event.CHANGE, OnChangeEvent);
            LeanTween.addListener(gameObject, Event.SOUND, OnSoundEvent);
            contents.ToObservable().Subscribe(x =>
            {
                x.setContent();
                if (x.action != null)
                {
                    string endChar = x.sentences.Substring(x.sentences.Length - 1, 1);
                    if (endChar != x.end)
                    {
                        x.sentences += x.end;
                    }
                    x.action.gameObject.SetActive(false);
                }
            });
            

        }
        public void removeListenner()
        {
            LeanTween.removeListener(gameObject,Event.SCENE, OnEvent);
            LeanTween.removeListener(gameObject,Event.SOUND, OnSoundEvent);
            LeanTween.removeListener(gameObject, Event.CHANGE, OnChangeEvent);
        }
        public void Again()
        {
            _score = 0;
            _count = 0;
            _PlayScene.transform.localPosition = contents[_count].Pos;
            player.Begin();
            StopAllCoroutines();
            contents.ToObservable().Subscribe(x =>
            {
                if (x.action != null)
                {
                    x.action.gameObject.SetActive(false);
                    x.action.drags.ToObservable().Subscribe(xx => {
                        xx.enabled = true;
                        xx.transform.SetParent(x.action.transform);
                        xx.close();
                    });
                    x.action.btnGo.interactable = false;
                }
            });
            Start();
        }
        void Start()
        {
            tutorial.gameObject.SetActive(false);
            FixRatio FixRatio = transform.GetChild(0).GetComponent<FixRatio>();
            if (FixRatio.ratio < 1.61)
            {
                contents.ToObservable().Subscribe(x =>
                {
                    if (x.action != null)
                    {
                        x.action.setForIpad();
                    }
                });
            }
            Begin();
        }
        void OnSoundEvent(LTEvent e)
        {

            soundManager.Play(_dicClips[e.data as string]);
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case Event.CLOSE:
                    tutorial.close();
                    break;
                case Event.BEGIN_DRAG:
                    StopAllCoroutines();
                    StartCoroutine(glowWord());
                    contents[_count].action.btnGo.interactable = false;
                    break;
                case Event.FAIL:
                    StopAllCoroutines();
                    contents[_count].action.btnGo.transform.GetChild(0).gameObject.SetActive(false);
                    LeanTween.dispatchEvent(Event.SOUND, "no");
                    LeanTween.delayedCall(0.3f, () => {
                        var list = Shuffle.createList(3);
                        string no = "no" + list[Random.Range(0,2)];
                        LeanTween.dispatchEvent(Event.SOUND, no);
                    });
                    _Fail++;
                    StartCoroutine(glowWord());
                    if (_Fail == 3)
                    {
                        Event.LOCK = true;
                        LeanTween.delayedCall(1.5f, () =>
                        {
                            contents[_count].action.drags.ToObservable().Subscribe(x => {
                                x.AutoDrop();
                            });
                            LeanTween.dispatchEvent(Event.CHANGE, transform);
                        });
                    } 
                    break;
                case Event.GLOW:
                    StartCoroutine(NoTap(contents[_count].action.btnGo.gameObject));
                    break;
                default:
                    break;
            }
        }
        
        void Begin()
        {
            player.gameObject.SetActive(true);
            StartCoroutine(player.goWalk(3,true));
            LeanTween.moveLocalX(player.gameObject, contents[_count].point.localPosition.x,3).setOnComplete(changeScene);
        }
        void changeScene()
        {
            contents[_count].action.drags.ToObservable().Subscribe(X => X.enabled = false);
            StopAllCoroutines();
            StartCoroutine(player.Oops());
            Event.LOCK = false;
            LeanTween.delayedCall(2.5f, () =>
            {
                float time = 0.5f;
                if (tutorial.Istutorial)
                {
                    time = soundManager.PlayByTime(0);
                    StartCoroutine(player.talkBegin());
                }
                LeanTween.delayedCall(time, () =>
                {
                    if (contents[_count].action != null)
                    {

                        contents[_count].action.Begin();
                        contents[_count].action.drags.ToObservable().Subscribe(X => X.enabled = true);
                        contents.ToObservable().Subscribe(x =>
                        {
                            if (x.action != null)
                            {
                                x.action.gameObject.SetActive(true);
                            }
                        });
                        _Fail = 0;
                        StartCoroutine(glowWord());
                    }
                    LeanTween.delayedCall(1, () => {
                        if (tutorial.Istutorial)
                        {
                            tutorial.Play();
                        }
                    });
                });
            });
        }
        public IEnumerator glowWord()
        {
            if (_count<contents.Length && contents[_count].action != null)
            {
                contents[_count].action.closeGlow();
                yield return new WaitForSeconds(TimeGLow);
                contents[_count].action.glow();
                yield return new WaitForSeconds(3);
                yield return StartCoroutine(glowWord());
            }
        }
        private IEnumerator NoTap(GameObject btn)
        {
            if (btn.GetComponent<Button>().interactable)
            {
                btn.transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitForSeconds(TimeGLow);
                btn.transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(2);
                btn.transform.GetChild(0).gameObject.SetActive(false);
                StartCoroutine(NoTap(btn));
            }
            yield return null;
        }
        void OnChangeEvent(LTEvent e)
        {
            if (_count <= contents.Length - 2)
            {
                if (_Fail == 0) _score += 3;
                else
                {
                    if (_Fail == 1) _score += 2;
                    if (_Fail >= 2) _score += 1;
                }
            }
            if (_Fail < 3)
            {
                LeanTween.dispatchEvent(Event.SOUND, "yes");
                LeanTween.delayedCall(0.3f, () => {
                    LeanTween.dispatchEvent(Event.SOUND, "yes" + Shuffle.createList(3)[Random.Range(0, 2)]);
                });
            }
            LeanTween.delayedCall(1.5f, () => {
                GetComponent<AudioSource>().clip = contents[_count].clipByTime.clip;
                GetComponent<AudioSource>().Play();
                StartCoroutine(player.talk(contents[_count].clipByTime.time+1));
                LeanTween.delayedCall(contents[_count].clipByTime.time+2, () => {
                    contents[_count].action.clear();
                    _count++;
                    StartCoroutine(jumpToPoint());
                    float time = contents[_count - 1].posInJump.Length;
                    LeanTween.moveLocalX(_PlayScene, contents[_count].Pos.x, 3).setDelay(time).setOnComplete(() => {
                        if (_count <= contents.Length - 2) changeScene();
                        else end();
                    });
                    LeanTween.delayedCall(time, () => {
                        StartCoroutine(player.goWalk(5,true));
                        LeanTween.moveLocal(player.gameObject,contents[_count].point.localPosition,3);
                    });
                });
            });
        }
        IEnumerator jumpToPoint()
        {
            for (int i = 0; i < contents[_count-1].posInJump.Length; i++)
            {
                LeanTween.moveLocal(player.gameObject,contents[_count-1].posInJump[i].localPosition,0.5f);
                yield return player.Jump();
                yield return new WaitForSeconds(0.5f);
            }
        }
        void end()
        {
            StopAllCoroutines();
            StartCoroutine(player.Win());
            LeanTween.delayedCall(5, () => {
                brige._score = _score;
                brige.ShowPopup();
            });
        }
    } 
 }

