using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace C4
{
    public class Frame: MonoBehaviour
    {
        public GameObject[] status;
        public GameObject content;
        public int id = 0;
        public float Distance = 0;
        public Sprite[] pics;
        public DragInFrame dragInFrame;
        public bool isClose = false;
        public Sprite _oldSprte;
        public Text txt;
        public bool isYes = false;
        private void Awake()
        {
            _oldSprte = content.GetComponent<Image>().sprite;
        }
        public void Start()
        {
            Clear();
            isYes = false;
            txt.gameObject.SetActive(false);
        }
        public void Clear()
        {
            status.ToObservable().Subscribe(x => x.gameObject.SetActive(false));
            content.SetActive(false);
            content.GetComponent<Image>().sprite = _oldSprte;
           // txt.gameObject.SetActive(false);
        }
        void SetStatus(int i)
        {
            Clear();
            status[i].SetActive(true);
        }
        void SetStatusInGame(int i)
        {
            status[i].SetActive(true);
        }
        public void glow()
        {
            SetStatus(0);
        }
        public void Yes()
        {
            SetStatusInGame(1);
            isYes = true;
            Show();
            txt.gameObject.SetActive(false);
        }
        public void No()
        {
            SetStatusInGame(2);
            isYes = false;
            Show();
        }
        public void Content()
        {
            setContentBySpirte(id);
        }
        public void Show()
        {
            content.SetActive(true);
        }
        public void setContentBySpirte(int i)
        {
            content.SetActive(true);
            content.GetComponent<Image>().sprite = pics[i];
            dragInFrame.GetComponent<Image>().sprite = pics[i];
        }
        public void setText(string textcontent)
        {
            txt.text = textcontent;
        }
        public void ShowText()
        {
            txt.gameObject.SetActive(true);
        }
        public void Alpha(bool isAlpha)
        {
            LeanTween.alpha(content.GetComponent<RectTransform>(),isAlpha? 0.6f:1, 0.3f).setFrom(0f);
        }
    }
}
