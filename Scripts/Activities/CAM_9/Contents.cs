using UnityEngine;
using UniRx;
using common;
using System.Linq;
namespace CAM_9
{
    [System.Serializable]
    public class Contents
    {
        public int id = 0;
        public Answer answer;
        public string[] Yes;
        public bool isTutorial = false;
        public ClipByTime[] clipByTime;
        
    }
}