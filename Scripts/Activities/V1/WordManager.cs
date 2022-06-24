using UnityEngine;
using UniRx;
using common;
namespace V1
{
    public class WordManager : MonoBehaviour
    {
        private Word[] _word;
        public AudioClip[] clips;
        private AudioSource _Source;
        public TextAsset sub;
        private SRTParser parserSub;
        public int id;
        public bool isTutorial = false;
        public bool isLock = false;
        public float time = 1;
        void Awake()
        {
            _word = GetComponentsInChildren<Word>();
            _Source = GetComponent<AudioSource>();
            parserSub = new SRTParser(sub);
        }
        public void Glow(int N)
        {
            _word.ToObservable().Subscribe(_color => _color.Glow(N));
        }
        public void Clear()
        {
            _word.ToObservable().Subscribe(_color => _color.Clear());
        }
        private void PlaySound(int K)
        {
            if (!_Source.isPlaying)
            {
                _Source.clip = clips[K];
                _Source.Play();
            }
        }
        public void PlaySoundContent()
        {
            PlaySound(0);
        }
        public void PlaySoundHighLine()
        {
            PlaySound(1);
        }
        public void RunSub()
        {
            int m = 0;
            _word.ToObservable().Subscribe(_glow => {
                _glow.Show();
                LeanTween.delayedCall((float) parserSub._subtitles[m++].From, () => {
                    _glow.Scale();
                });
            });
            Glow(2);
            PlaySoundHighLine();
            LeanTween.delayedCall(0.5f + (float)parserSub._subtitles[parserSub._subtitles.Count - 1].To,()=>{
                PlaySoundContent();
                LeanTween.delayedCall((float) time, () => {
                    Clear();
                    LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.READSUBDONE);
                });
            });
            isLock = true;
        }
        public void clearForBegin()
        {
            _word.ToObservable().Subscribe(_glow => _glow.Start()); ;
        }
    }
}
