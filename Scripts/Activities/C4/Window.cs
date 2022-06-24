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
    public class Window : MonoBehaviour
    {
        public Sprite[] pics;
        public int stt = 0;
        private Image _current;
        public Image main;
        private Transform _Back;
        private Transform _Next;
        public GameObject glow;
        public DragInBox drag;
        public string[] contents;
        public Text txt;
        private void Awake()
        {
            _current = main.GetComponent<Image>();
            _Back = transform.GetChild(2);
            _Next = transform.GetChild(1);
        }
        private void Start()
        {
            _current.sprite = pics[stt];
            if(drag!=null) drag.id = stt;
        }
        public void Begin()
        {
            if (drag != null)
            {
                drag.setContent(pics[stt]);
                drag.id = stt;
                txt.text = contents[stt];
            }
        }
        public void FlyToDrop(int oldId)
        {
            if (drag != null)
            {
                stt = oldId;
                _current.sprite = pics[stt];
                closeAlpha();
                drag.id = stt;
                txt.text = contents[stt];
                drag.close();
                drag.GetComponent<Image>().sprite = _current.sprite;
            }
        }
        public void Next()
        {
            if (stt < pics.Length-1)
            {
                stt++;
                _current.sprite = pics[stt];
                if (drag != null)
                {
                    drag.id = stt;
                    drag.Back();
                    txt.text = contents[stt];
                    drag.GetComponent<Image>().sprite = _current.sprite;
                    LeanTween.dispatchEvent(Event.CHANGE, _current.sprite);
                }
                LeanTween.dispatchEvent(Event.SOUND, "tap");
            }
        }
        public void Back()
        {
            if (stt > 0)
            {
                stt--;
                _current.sprite = pics[stt];
                if (drag != null)
                {
                    drag.id = stt;
                    txt.text = contents[stt];
                    drag.Back();
                    drag.GetComponent<Image>().sprite = _current.sprite;
                    LeanTween.dispatchEvent(Event.CHANGE, _current.sprite);
                }
                LeanTween.dispatchEvent(Event.SOUND, "tap");
            }
        }
        public void setGlowBtn(bool isNext)
        {
            _Next.GetChild(0).gameObject.SetActive(isNext?true:false);
            _Back.GetChild(0).gameObject.SetActive(isNext?false:true);
        }
        public void Alpha()
        {
            LeanTween.alpha(_current.GetComponent<RectTransform>(), 0.4f, 0.3f).setFrom(0f);
        }
        public void closeGlowBtn()
        {
            _Next.GetChild(0).gameObject.SetActive(false);
            _Back.GetChild(0).gameObject.SetActive(false);
        }
        public void Setglow()
        {
            glow.SetActive(true);
        }
        public void closeGlow()
        {
            glow.SetActive(false);
        }
        public void closeAlpha()
        {
            LeanTween.alpha(_current.GetComponent<RectTransform>(), 1, 0.3f).setFrom(0f);
        }
        public void SetDefault()
        {
            _current.sprite = pics[stt];
            closeAlpha();
            drag.id = stt;
            drag.GetComponent<Image>().sprite = _current.sprite;
            txt.text = contents[stt];
        }
        public void close()
        {
            stt = 0;
        }
    }
}
