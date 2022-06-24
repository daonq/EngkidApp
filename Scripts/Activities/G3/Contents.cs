using UnityEngine;
using UniRx;
using common;
using System.Linq;
using UnityEngine.UI;
using System;
using Spine.Unity;
namespace G3
{
    [Serializable]
    public class Contents
    {
        public string sentences;
        public string[] words;
        public string Yes;
        public ClipByTime clipByTime;
        public SceneBySpine scene;
        public Image pic;
        public string fullsentences()
        {
            return sentences.Replace("___", Yes);
        }
    }
}