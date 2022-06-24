using UnityEngine;
using UniRx;
using common;
using System.Linq;
using UnityEngine.UI;
namespace G1
{
    [System.Serializable]
    public class Contents
    {
        public pageContent[] pageContent;
        public string page;
    }
    [System.Serializable]
    public class pageContent
    {
        public string sentences;
        public Sprite pic;
        public ClipByTime clipByTime;
        public GameObject Box;
        public Vector3 pos;
        public void setContent()
        {
            Box.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = sentences;
            Box.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = sentences;
        }
        public void setBox()
        {
            Box.GetComponent<Frame>().content = sentences;
        }
    }
}