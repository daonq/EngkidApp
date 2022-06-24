using UnityEngine;
using UniRx;
using common;
using System.Linq;
using UnityEngine.UI;
namespace G2
{
    
    [System.Serializable]
    public class Contents
    {
        public string sentences;
        public ClipByTime clipByTime;
        public Action action;
        public Vector3 Pos;
        public Transform point;
        public Transform[] posInJump;
        public char strSplit =' ';
        public string end = ".";
        public string[] words()
        {
            return sentences.Split(strSplit);
        }
        public void setContent()
        {
            if (action != null)
            {
                int i = 0;
                action.drags.ToObservable().Subscribe(x => {
                    x.transform.GetChild(0).GetComponent<Text>().text = words()[i];
                    action.frames[i].content = words()[i];
                    Debug.Log(action.gameObject.name);
                    i++;
                });
            }
        }
    }
}