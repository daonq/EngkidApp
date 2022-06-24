using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using Stories;
using System.Collections;
namespace G5
{
    public class Book : MonoBehaviour
    {
        public SkeletonGraphic above;
        public SkeletonGraphic below;
        public GameObject page;
        public GameObject btnclose;
        public GameObject pageBox;
        public List<ContentByTime> Albums;
        public TouchAdvance Touch;
        private AudioSource _audio;
        private int _count=0;
        private bool isLock = false;
        private void Awake()
        {
            LeanTween.addListener(gameObject, StoryEvent.TouchPage, OnEventTouchPage);
            _audio = GetComponent<AudioSource>();
        }
        private void Play(AudioClip clip)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        public void close()
        {
            below.timeScale = 2;
            below.AnimationState.SetAnimation(0, "close_album", false);
            btnclose.gameObject.SetActive(false);
            above.gameObject.SetActive(false);
            pageBox.gameObject.SetActive(false);
            page.gameObject.SetActive(false);
            LeanTween.delayedCall(0.5f, () => {
                gameObject.SetActive(false);
                below.timeScale = 1;
            });
        }
        public void Open()
        {
            Touch.gameObject.SetActive(false);
            gameObject.SetActive(true);
            below.AnimationState.SetAnimation(0, "open_album", false);
            above.gameObject.SetActive(false);
            pageBox.gameObject.SetActive(false);
            page.gameObject.SetActive(false);
            btnclose.SetActive(false);
            LeanTween.delayedCall(0.5f,()=> StartCoroutine(OpenContent(true,_count)));
            LeanTween.delayedCall(1,()=>
            {
                btnclose.SetActive(true);
                Touch.gameObject.SetActive(true);
            });
        }
        public IEnumerator OpenContent(bool isNext,int stt)
        {
            
            above.gameObject.SetActive(true);
            clear();
            below.AnimationState.SetAnimation(0, Albums[stt].StatusInBook, false);
            above.AnimationState.SetAnimation(0, "flip_" + Albums[stt].StatusInBook + (isNext ? "" : "_reverse"), false);
            pageBox.gameObject.SetActive(false);
            page.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
            Play(Albums[stt].clip);
            pageBox.gameObject.SetActive(true);
            page.gameObject.SetActive(true);
            pageBox.transform.GetChild(0).GetComponent<Text>().text = Albums[stt].Sentence;
            page.transform.GetChild(0).GetComponent<Text>().text = (stt + 1) + "/" + Albums.Count;
        }
        //Xử lý sự kiện vuốt trái vuốt phải 
        private void OnEventTouchPage(LTEvent e)
        {
            if (!isLock)
            {
                
                switch (e.data as string)
                {
                    case StoryEvent.PAGELEFT:
                        Back();
                        break;
                    case StoryEvent.PAGERIGHT:
                        Next();
                        break;
                    default:
                        break;
                }
            }
        }
        public void Begin()
        {
            gameObject.SetActive(true);
            below.AnimationState.SetAnimation(0, "open_album", false);
            above.gameObject.SetActive(false);
            pageBox.gameObject.SetActive(false);
            page.gameObject.SetActive(false);
            btnclose.SetActive(false);
            LeanTween.delayedCall(0.5f, () => {
                above.AnimationState.SetAnimation(0, "idle", false);
            });
            LeanTween.delayedCall(1,()=> btnclose.SetActive(true));
        }
        
        private void Back()
        {
            if (_count >= 1)
            {
                _count--;
                StartCoroutine(OpenContent(false, _count));
                LeanTween.dispatchEvent(G5Event.SOUND, "paper");
            }
        }
        private void Next()
        {
            if (_count < Albums.Count-1)
            {
                _count++;
                StartCoroutine(OpenContent(true,_count));
                LeanTween.dispatchEvent(G5Event.SOUND, "paper");

            }
        }
        public void clear()
        {
            above.AnimationState.ClearTracks();
            above.Skeleton.SetToSetupPose();
            below.AnimationState.ClearTracks();
            below.Skeleton.SetToSetupPose();
           // above.AnimationState.SetEmptyAnimations(0);
        }
        public IEnumerator AutoOpen()
        {
            Touch.gameObject.SetActive(false);
            btnclose.SetActive(false);
            yield return new WaitForSeconds(2);
            gameObject.SetActive(true);
            _count = 0;
            isLock = true;
            above.gameObject.SetActive(false);
            pageBox.gameObject.SetActive(false);
            page.gameObject.SetActive(false);
            isLock = true;
            below.AnimationState.SetAnimation(0, "open_album", false);
            yield return new WaitForSeconds(1);
            above.gameObject.SetActive(true);
            btnclose.SetActive(false);
            for (int i=0; i < Albums.Count; i++)
            {
                //clear();
                below.AnimationState.SetAnimation(0, Albums[i].StatusInBook, false);
                above.AnimationState.SetAnimation(0, "flip_" + Albums[i].StatusInBook, false);
                LeanTween.dispatchEvent(G5Event.SOUND, "paper");
                pageBox.gameObject.SetActive(false);
                page.gameObject.SetActive(false);
                yield return new WaitForSeconds(1.5f);
                Play(Albums[i].clip);
                pageBox.gameObject.SetActive(true);
                page.gameObject.SetActive(true);
                pageBox.transform.GetChild(0).GetComponent<Text>().text = Albums[i].Sentence;
                page.transform.GetChild(0).GetComponent<Text>().text = (i + 1) + "/" + Albums.Count;
                yield return new WaitForSeconds(Albums[i].time+2);
            }
            yield return new WaitForSeconds(2);
            close();
            LeanTween.dispatchEvent(G5Event.ACTION, G5Event.END);
            yield return new WaitForSeconds(0.5f);
            isLock = false;
        }
    }
}
