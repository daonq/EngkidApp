using UnityEngine;
using UniRx;
using common;
using System.Linq;
namespace common
{
    [System.Serializable]
    public class V2Word
    {
        public string word;
        public string wordBefore;
        public string[] chars;
        public string charForWord;
        public GameObject pic;
        public bool isTutorial = false;
        public AudioClip clip;
        public float time=0.5f;
    }
}