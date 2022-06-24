using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
namespace common
{
    public class AutoScrollByPos : MonoBehaviour
    {
        public Vector3[] allPos;
        public Dictionary<string, Vector3> dicPos = new Dictionary<string, Vector3>();
        private Transform transContent;
        public InputWord[] listWord;
        public CanvasScaler canvasScaler;
        public Vector3 _oldPos;
        void Awake()
        {
            transContent = transform.GetChild(0).GetChild(0).transform;
            LeanTween.addListener(gameObject,TextEvent.BEGININPUT,OnEvent);
            LeanTween.addListener(gameObject,TextEvent.DONEINPUT,OnEditEvent);
            _oldPos = transContent.localPosition;
            int i = 0;
            listWord[0].GetComponent<InputField>().enabled = false;
            listWord[0].GetComponent<InputField>().placeholder.gameObject.SetActive(false);
            allPos.ToObservable().Subscribe(xx => dicPos.Add((i++).ToString(),xx));
        }
        public void Highline(bool isHighLine)
        {
            listWord[0].highLine(isHighLine);
        }
        public void clear()
        {
            listWord[0].GetComponent<InputField>().placeholder.gameObject.SetActive(true);
        }
        void OnEvent(LTEvent e)
        {
            string Name = (e.data as Transform).name;
            if (Name != null && canvasScaler.matchWidthOrHeight==1)
            {
                transContent.localPosition = dicPos[Name];
            }
            listWord.ToObservable().Subscribe(x => x.highLine(false));
        }
        void OnEditEvent(LTEvent e)
        {
            Transform tran = e.data as Transform;
            transContent.localPosition = _oldPos;
            LeanTween.dispatchEvent(TextEvent.CHECK,tran.GetComponent<InputField>());
        }
    }
}
