using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace V2
{
    public class ActivityV2 : MonoBehaviour
    {
        [Header("class")]
        public SoundManager soundManager;
        public BrigeToResult brigeToResult;
        public SpaceShip spaceShip;
        public Shoot shoot;
        public Cammon cammon;
        public Review review;
        public Wave wave;
        public V2Word[] contents;
        [Header("Time help")]
        public float timeHelp=17;
        private int _count = 0;
        private float _score = 0;
        private float _fail = 0;
        private Dictionary<string, int> dicClips = new Dictionary<string, int>();
        private AudioSource _audio;
        private Trash trash;
        private void Awake()
        {
            dicClips.Add("back", 0);
            dicClips.Add("bubble", 1);
            dicClips.Add("spaceship", 2);
            dicClips.Add("bell", 3);
            dicClips.Add("drop", 4);
            dicClips.Add("flip", 5);
            dicClips.Add("shoot", 6);
            dicClips.Add("catch", 7);
            dicClips.Add("fire", 8);
            dicClips.Add("laser", 9);
            dicClips.Add("tap", 10);
            dicClips.Add("fail", 11);
            dicClips.Add("popupScale", 12);
            dicClips.Add("Explosive", 13);
            dicClips.Add("red", 14);
            LeanTween.addListener(gameObject,V2Event.ACTION,OnGameEvent);
            LeanTween.addListener(gameObject,V2Event.SOUND,OnSoundEvent);
            _audio = GetComponent<AudioSource>();
        }
        void playContent(AudioClip clip)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        public void removeListenner()
        {
            LeanTween.addListener(gameObject, V2Event.ACTION, OnGameEvent);
            LeanTween.addListener(gameObject, V2Event.SOUND, OnSoundEvent);
        }
        private void Start()
        {
            _count = 0;
            _score = 0;
            contents.ToObservable().Subscribe(x => x.pic.SetActive(false));
            cammon.begin();
            LeanTween.delayedCall(1.5f, () => spaceShip.goIn());
            review.gameObject.SetActive(false);
            wave.trash = shoot.trashs;
            wave.trashPos = shoot.trashOldPos;
        }
        void OnSoundEvent(LTEvent e)
        {
            string str = e.data as string;
            soundManager.Play(dicClips[str]);
        }
        void OnGameEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case V2Event.TRASH:
                    LeanTween.dispatchEvent(V2Event.SOUND, "bell");
                    int i = 0;
                    shoot.trashs.ToObservable().Subscribe(x => {
                        x.setText(contents[_count].chars[i++]);
                        if (contents[_count].isTutorial)
                        {
                            x.isLock = true;
                            x.gameObject.SetActive(true);
                            x.isShoot = true;
                        }
                        else
                        {
                            x.isLock = false;
                            x.isShoot = x.charYes == contents[_count].charForWord?true:false;
                        }
                    });
                    if (contents[_count].isTutorial) LeanTween.delayedCall(2, () => shoot.Drop());
                    LeanTween.delayedCall(contents[_count].isTutorial?5:0.5f, () => {
                        cammon.setContent(contents[_count].wordBefore);
                        contents.ToObservable().Subscribe(x => x.pic.SetActive(false));
                        contents[_count].pic.SetActive(true);
                        cammon.showBroad(contents[_count].isTutorial);
                        if (contents[_count].isTutorial) {
                            LeanTween.delayedCall(3,tutorial); 
                        }
                        else
                        {
                            soundManager.PlayByTime(1);
                            Invoke("loopSound", 17);
                        }
                    });
                    trash = shoot.trashs.Where(x => x.charYes == contents[_count].charForWord).ToArray()[0];
                    shoot.current = trash;
                    break;
                case V2Event.GLOWTRASH:
                    if (contents[_count].isTutorial)
                    {
                        trash.isLock = false;
                        trash.Glow();
                    } else trash.noneGlow();
                    break;
                case V2Event.SHOOT:
                    shoot.trashs.ToObservable().Subscribe(x => x.isLock = true);
                    CancelInvoke("loopSound");
                    cammon.HandOut();
                     if (shoot.current.charYes != contents[_count].charForWord) No();
                    break;
                case V2Event.PROCESS:
                    onFire();
                    trash.noneGlow();
                    break;
                case V2Event.BEGIN:
                    Debug.Log("end begin");
                    cammon.CheckBroad();
                    if (shoot.current.charYes == contents[_count].charForWord)
                    {
                        Yes();
                    }
                    LeanTween.delayedCall(3, () => {
                        cammon.showBroad(true);
                        LeanTween.delayedCall(3, () => {
                            if(_count<(contents.Length-2)) wave.Move();
                            if (contents[_count].isTutorial)
                            {
                                GetComponent<AlphaTutorial>().Close();
                            }
                            LeanTween.delayedCall(1,Next);
                        });
                    });
                    break;
                case V2Event.TAPME:
                    soundManager.PlayByTime(1);
                    CancelInvoke("loopSound");
                    Invoke("loopSound", timeHelp);
                    break;
                case V2Event.FADEIN:
                    shoot.ChangePos();
                    break;
                default:
                    break;
            }
        }
        void No()
        {
            cammon.led(1);
            _fail++;
            if (_fail >= 2) trash.Glow();
            LeanTween.delayedCall(2, () => {
                shoot.trashs.ToObservable().Subscribe(x => x.isLock = false);
                Invoke("loopSound", 15);
            });
        }
        void Yes()
        {
            cammon.led(0);
            cammon.setContent(contents[_count].word);
            playContent(contents[_count].clip);
            if (!contents[_count].isTutorial) bonus();
        }
        void bonus()
        {
            if (_fail == 0) _score += 1;
            if (_fail == 1) _score += 2 / 3;
            if (_fail >= 2)
            {
                _score += 1 / 3;
            }
        }
        private void onFire()
        {
            cammon.Shoot();
            LeanTween.dispatchEvent(V2Event.SOUND, "fire");
            LeanTween.delayedCall(0.5f, () => {
                cammon.fire();
                LeanTween.dispatchEvent(V2Event.SOUND, "laser");
                
            });
        }
        private void tutorial()
        {
            if (contents[_count].isTutorial)
                GetComponent<AlphaTutorial>().Tutorial(shoot.trashs.Where(w => w != trash).ToArray());
            LeanTween.delayedCall(soundManager.PlayByTime(0),() =>
            {
                cammon.handGo();
                
            });
        }
        private void Next()
        {
            _count++;
            _fail = 0;
            if (_count < contents.Length) change();
            else OnReview();
        }
        private void change()
        {
            cammon.clear();
            LeanTween.delayedCall(0.5f, () => {
                wave.Back();
            });
        }
        private void OnReview()
        {
            LeanTween.dispatchEvent(V2Event.SOUND, "popupScale");
            cammon.ScaleToEnd();
            LeanTween.delayedCall(2, () => {
                LeanTween.dispatchEvent(V2Event.SOUND, "Explosive");
                review.gameObject.SetActive(true);
                review.OnExplosive();
                LeanTween.delayedCall(1, () =>
                {
                    shoot.trashs.ToObservable().Subscribe(x => x.transform.localScale = Vector3.zero);
                    cammon.transform.localScale = Vector3.zero;
                    soundManager.TurnOn(true);
                    LeanTween.delayedCall(soundManager.PlayByTime(3)+1,()=>finish());
                });
            });
        }
        private void finish()
        {
            brigeToResult._score = Mathf.CeilToInt(_score);
            review.close();
            cammon.end();
            brigeToResult.ShowPopup();
        }
        public void Again()
        {
            soundManager.TurnOn(false);
            Start();
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
