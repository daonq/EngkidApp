using UnityEngine;

namespace common
{
    [System.Serializable]
    public class ContentByTime
    {
        public string word;
        public AudioClip clip;
        public float time=0.5f;
        public string Status;
        public string StatusInBook;
        public string Sentence;
        public Sprite pic;
        public Sprite picInPrint;
    }
}
