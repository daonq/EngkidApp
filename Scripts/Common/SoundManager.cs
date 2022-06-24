using UnityEngine;
using System.Collections.Generic;
namespace common
{
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _channelSoundBG;
        private AudioSource _channelSoundEF;
        public AudioClip[] clips;
        public ClipByTime[] clipTutorial;
        public Dictionary<string, int> dicClips = new Dictionary<string, int>();
        public bool isAddDataDic = true;
        public AudioSource _Tutorial;
        private AudioSource _SoundEFByScene;
        public int SoundClick = 5;
        void Awake()
        {
            _channelSoundBG = transform.GetChild(2).GetChild(0).GetComponent<AudioSource>();
            _channelSoundEF = transform.GetChild(2).GetChild(1).GetComponent<AudioSource>();
            if (transform.GetChild(2).childCount == 3)
            {
                _Tutorial = transform.GetChild(2).GetChild(2).GetComponent<AudioSource>();
            }
            if (transform.GetChild(2).childCount == 4)
            {
                _SoundEFByScene = transform.GetChild(2).GetChild(3).GetComponent<AudioSource>();
            }
            if (isAddDataDic) addDic();
        }
        private void addDic()
        {
            dicClips.Add("listshow", 0);
            dicClips.Add("tap", 1);
            dicClips.Add("yes", 2);
            dicClips.Add("no", 3);
        }
        public void Play(int i)
        {
            _channelSoundEF.clip = clips[i];
            _channelSoundEF.Play();
        }
        public void SoundTutorial()
        {
            if (!_Tutorial.isPlaying) _Tutorial.Play();
        }
        public void SoundContentOn(bool isOn)
        {
            if(_Tutorial!=null && _SoundEFByScene != null)
            {
                _Tutorial.mute = isOn;
                _SoundEFByScene.mute = isOn;
            }
        }
        public void TurnOn(bool isOn)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(isOn?1:0).gameObject.SetActive(true);
            _channelSoundBG.mute = isOn;
            if (_channelSoundBG.transform.childCount == 1)
            {
                _channelSoundBG.transform.GetChild(0).GetComponent<AudioSource>().mute = isOn;
            }
            Play(SoundClick);
        }
        public float PlayByTime(int i)
        {
            float time=0;
            if (_Tutorial != null)
            {
                if (clipTutorial.Length > 0 && !_Tutorial.isPlaying)
                {
                    _Tutorial.clip = clipTutorial[i].clip;
                    _Tutorial.Play();
                    time = clipTutorial[i].time;
                }
            }
            return time;
        }
    }
}
