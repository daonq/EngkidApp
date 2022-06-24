using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using CAM_9;

namespace G1
{
    [System.Serializable]
    public class page
    {
        public Image pic;
        public Button btnDone;
        public Transform box;
        public Image glow;
        public ClipByTime clipByTime;
        public Vector3 pos;
        public Text txt;
        public bool isEnd = false;
        public void show()
        {
            pic.transform.parent.GetComponent<Image>().enabled = true;
            pic.gameObject.SetActive(true);
            if (isEnd)
            {
                txt.gameObject.SetActive(true);
                btnDone.gameObject.SetActive(false);
                box.gameObject.SetActive(false);
            }
            else
            {
                btnDone.gameObject.SetActive(true);
                btnDone.interactable = false;
                box.gameObject.SetActive(true);
                box.GetChild(0).gameObject.SetActive(false);
            }
        }
        public void Hide()
        {
            if (isEnd)
            {
                txt.gameObject.SetActive(true);
            }
            pic.transform.parent.GetComponent<Image>().enabled = false;
            pic.gameObject.SetActive(false);
            btnDone.gameObject.SetActive(false);
            box.gameObject.SetActive(false);
        }
        public void showContent()
        {
            box.GetChild(0).gameObject.SetActive(true);
            txt.gameObject.SetActive(true);
        }
        public void end()
        {
            showContent();
            btnDone.gameObject.SetActive(false);
            box.gameObject.SetActive(false);
            txt.gameObject.SetActive(true);
        }
    }
    [System.Serializable]
    public class Bone
    {
        public SkeletonGraphic bone;
        public string[] status;
        public void Play(int stt,bool isLoop)
        {
            
            bone.AnimationState.SetAnimation(0, status[stt], isLoop);
        }
        public void clear()
        {
            //bone.AnimationState.SetEmptyAnimations(0);
            bone.AnimationState.ClearTracks();
            bone.Skeleton.SetToSetupPose();
        }
    }
    public class Book : MonoBehaviour
    {
        public Bone below;
        public Bone above;
        public page[] pages;
        public GameObject stt;
        private AudioSource _audio;
        public int fail = 0;
        private GameObject glow;
        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }
        void Start()
        {
            pages[0].txt.text = "";
            pages[1].txt.text = "";
            hide();
            Close();
            above.bone.gameObject.SetActive(true);
            above.Play(2, false);
        }
        public void Open()
        {
            below.Play(4, false);
            hide();
            LeanTween.dispatchEvent(Event.SOUND,"paper");
            LeanTween.delayedCall(0.3f, () => {
                pages[1].show();
                LeanTween.delayedCall(0.7f, () => {
                    show();
                    LeanTween.delayedCall(1, () => {
                        above.clear();
                        above.bone.gameObject.SetActive(false);
                    });
                });
            });
            StartCoroutine(ScaleHelp());
        }
        public IEnumerator ScaleHelp()
        {
            yield return new WaitForSeconds(Event.TIME_HELP);
            int _count = 0;
            if (!pages[0].btnDone.gameObject.activeSelf && pages[1].btnDone.gameObject.activeSelf) _count = 1;
            if (pages[0].btnDone.gameObject.activeSelf || pages[1].btnDone.gameObject.activeSelf) 
                LeanTween.dispatchEvent(Event.HELP, pages[_count].box.GetComponent<Frame>());
        }
        public void hide()
        {
            pages[0].Hide();
            pages[1].Hide();
            stt.SetActive(false);
        }
        public void show()
        {
            pages[0].show();
            pages[1].show();
            stt.gameObject.SetActive(true);
        }
        public void Next()
        {
            below.Play(3, true);
            above.bone.gameObject.SetActive(true);
            above.Play(0, false);
            LeanTween.dispatchEvent(Event.SOUND,"paper");
            hide();
            pages[1].show();
            LeanTween.delayedCall(1, () => {
                pages[0].show();
                stt.gameObject.SetActive(true);
                LeanTween.delayedCall(1, () => {
                    above.clear();
                    above.bone.gameObject.SetActive(false);
                    StartCoroutine(ScaleHelp());
                });
            });
        }
        public void Back()
        {
            below.Play(3, true);
            above.bone.gameObject.SetActive(true);
            above.Play(1, false);
            LeanTween.dispatchEvent(Event.SOUND, "paper");
            hide();
            pages[0].show();
            LeanTween.delayedCall(1, () => {
                pages[1].show();
                LeanTween.delayedCall(1, () => {
                    above.clear();
                    above.bone.gameObject.SetActive(false);
                    stt.gameObject.SetActive(true);
                });
            });
        }
        public void Play(AudioClip clip)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        private IEnumerator PlayContent(int count)
        {
            Play(pages[count].clipByTime.clip);
            pages[count].pic.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(pages[count].clipByTime.time);
            yield return new WaitUntil(() => !_audio.isPlaying);
            pages[count].pic.transform.GetChild(0).gameObject.SetActive(false);
            LeanTween.dispatchEvent(Event.CHECK, pages[count].btnDone.gameObject);
            Event.LOCK = false;
            StartCoroutine(ScaleHelp());
        }
        public void closeGlow()
        {
            if(glow!=null) glow.gameObject.SetActive(false);
        }
        public IEnumerator SoundYes()
        {
            LeanTween.dispatchEvent(Event.SOUND, "yes");
            yield return new WaitForSeconds(0.3f);
            int[] rd = Shuffle.createList(3);
            LeanTween.dispatchEvent(Event.SOUND, "yes" + rd[0]);
        }
        public void CheckDone(int count)
        {
            StartCoroutine(Check(count));
        }
        public IEnumerator Check(int count)
        {
            yield return new WaitForSeconds(0.01f);
            glow = pages[count].glow.gameObject;
            if (pages[count].box.GetComponent<Frame>().content==pages[count].box.GetChild(0).GetComponent<Text>().text)
            {
                fail = 0;
                pages[count].btnDone.interactable = false;
                pages[count].btnDone.gameObject.SetActive(false);
                pages[count].glow.gameObject.SetActive(true);
                pages[count].glow.color = Color.green;
                if (!_audio.isPlaying)
                {
                    Event.LOCK = true;
                    yield return StartCoroutine(SoundYes());
                    //yield return new WaitForSeconds(1);
                    pages[count].box.gameObject.SetActive(false);
                    pages[count].glow.gameObject.SetActive(false);
                    pages[count].showContent();
                    yield return StartCoroutine(PlayContent(count));
                } else {
                    yield return new WaitUntil(()=> !_audio.isPlaying);
                    Event.LOCK = true;
                    pages[count].box.gameObject.SetActive(false);
                    pages[count].glow.gameObject.SetActive(false);
                    pages[count].showContent();
                    yield return StartCoroutine(PlayContent(count));
                }
            } else {
                pages[count].glow.color = Color.red;
                pages[count].glow.gameObject.SetActive(true);
                pages[count].glow.color = Color.red;
                pages[count].box.GetChild(0).GetComponent<Text>().text = "";
                LeanTween.dispatchEvent(Event.SOUND, "no");
                pages[count].box.GetComponent<Frame>().drag.close();
                yield return new WaitForSeconds(0.5f);
                int[] rd = Shuffle.createList(3);
                LeanTween.dispatchEvent(Event.SOUND, "no" + rd[0]);
                yield return new WaitForSeconds(1);
                pages[count].glow.gameObject.SetActive(false);
                pages[count].btnDone.interactable = false;
                fail++;
                pages[count].box.GetComponent<Frame>().Fail = fail;
                LeanTween.dispatchEvent(Event.FAIL, pages[count].box.GetComponent<Frame>());
                if (fail >= 3) {
                    fail = 0;
                    LeanTween.dispatchEvent(Event.FAIL_MAX, pages[count].box.GetComponent<Frame>());
                }
            }
        }
        public void clear()
        {
            fail = 0;
        }
        public void Close()
        {
            below.Play(0, false);
            fail = 0;
            above.bone.gameObject.SetActive(false);
        }
    }
}
