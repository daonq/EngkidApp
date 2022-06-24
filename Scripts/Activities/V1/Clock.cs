using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using common;
using Spine;
using Spine.Unity;
namespace V1
{
    public class Clock : MonoBehaviour
    {
        private Text txtClock;
        public int totalTime = 300;
        private int _Time;
        public bool isFormat = true;
        public AudioClip[] clips;
        private SkeletonGraphic _spine;
        private GameObject _bgred;
        public const int TIME_END = 10;
        void Awake()
        {
            txtClock = transform.GetChild(0).GetChild(2).GetComponent<Text>();
            _spine = transform.GetChild(0).GetChild(1).GetComponent<SkeletonGraphic>();
            _bgred = transform.GetChild(0).GetChild(0).gameObject;
        }
        public void run()
        {
            jump();
        }
        private void jump()
        {
            SoundEffect(1);
            _spine.AnimationState.SetAnimation(0, "clock", true);
            LeanTween.delayedCall(3, () => {
               _spine.AnimationState.SetAnimation(0, "clock", false);
                BeginCount();
            });
        }
        private void alphaBgRed()
        {
            _bgred.SetActive(true);
            loop();
        }
        private void loop()
        {
            _bgred.GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alpha(_bgred, 0f, 0.5f).setOnComplete(() => {
                _bgred.GetComponent<CanvasGroup>().alpha = 1;
                LeanTween.alpha(_bgred, 1f, 0.5f).setEase(LeanTweenType.linear).setOnComplete(loop);
            });
        }
        private void BeginCount()
        {
            _Time = totalTime;
            Play();
        }
        public void Play()
        {
            StartCoroutine("Countdown", _Time);
        }
        public void stop()
        {
            _spine.AnimationState.SetAnimation(0, "clock", false);
            StopCoroutine("Countdown");
        }
        private IEnumerator Countdown(int Time)
        {
            while (Time > 0)
            {
                _Time = Time--;
                txtClock.text = (isFormat ? formatClock(_Time) : _Time + "").ToString();
                if (_Time==TIME_END)
                {
                    alphaBgRed();
                    SoundEffect(0);
                }
                yield return new WaitForSeconds(1);
            }
            CountDownComplete();
        }
        private string formatClock(int number)
        {
            int k = (int)Mathf.Floor(number / 60);
            return '0' + k.ToString() + ':' + (formatString(number - k * 60)).ToString();
        }
        private string formatString(int N)
        {
            return N >= 10 ? "" + N.ToString() : '0' + N.ToString();
        }
        private void CountDownComplete()
        {
            txtClock.text = isFormat ? "00:00" : "0";
            SoundEffect(1);
            _bgred.SetActive(false);
            _spine.AnimationState.SetAnimation(0, "clock", true);
            LeanTween.delayedCall(3,() => {
                _spine.AnimationState.SetAnimation(0, "clock", false);
            });
            LeanTween.dispatchEvent(WordEvent.ACTION,WordEvent.TIME_END);
        }
        private void SoundEffect(int i)
        {
            GetComponent<AudioSource>().clip = clips[i];
            GetComponent<AudioSource>().Play();
        }
        public void Reset()
        {
            _Time = totalTime;
            txtClock.text = (isFormat ? formatClock(_Time) : _Time + "").ToString();
        }
    }

}
