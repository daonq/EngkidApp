using UnityEngine;
using UniRx;
using common;
using System.Linq;
namespace G5
{
    [System.Serializable]
    public class Contents
    {
        public int id = 0;
        public bool isTutorial = false;
        public string sentences;
        public ContentByTime[] contentByTime;
        public string end;
        public string page;
        public int typePos = 0;
        public Vector3 pos;
    }
}