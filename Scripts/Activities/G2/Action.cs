using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G2
{
    public class Action: MonoBehaviour
    {
        public Drag[] drags;
        public Frame[] frames;
        public Button btnGo;
        private int[] rd;
        private GameObject _glowDrop;
        private void Awake()
        {
            LeanTween.addListener(gameObject,Event.DRAG,OnEvent);
            Vector3[] AllPos = drags.Select(x => x.transform.localPosition).ToArray();
            int[] rd = Shuffle.createList(AllPos.Length);
            for(int i = 0; i < drags.Length; i++)
            {
                drags[i].transform.localPosition = AllPos[rd[i]];
            }
            GlowBrige();
        }
        private void GlowBrige()
        {
            var glowPrefab = Resources.Load("glow") as GameObject;
            _glowDrop = GameObject.Instantiate(glowPrefab,transform.position,transform.rotation);
            _glowDrop.transform.SetParent(transform);
            _glowDrop.transform.localPosition = Vector3.zero;
            _glowDrop.transform.localScale = Vector3.one;
            _glowDrop.SetActive(false);
            RectTransform reGlow = drags[0].transform.GetChild(1).GetComponent<RectTransform>();
            _glowDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(reGlow.rect.width, reGlow.rect.height);
        }
        public void setForIpad()
        {
            for (int i = 0; i < drags.Length; i++)
            {
                drags[i].setForIpad();
            }
        }
        public void glow()
        {
            Drag[] others = drags.Where(x => !x.isDrag).ToArray();
            if (others.Length > 0)
            {
                rd = Shuffle.createList(others.Length);
                drags[rd[0]].Glow(true);
                _glowDrop.gameObject.SetActive(true);
                _glowDrop.transform.localPosition = frames[rd[0]].transform.localPosition;
            }
        }
        public void closeGlow()
        {
            for (int i = 0; i < drags.Length; i++)
            {
                drags[i].Glow(false);
            }
            _glowDrop.gameObject.SetActive(false);
        }
        public void Begin()
        {
            for (int i = 0; i < drags.Length; i++)
            {
                drags[i].Begin();
            }
        }
        void OnEvent(LTEvent e)
        {
            var check = frames.Where(x => x.transform.childCount > 0).ToArray();
            if (check.Length==frames.Length)
            {
                btnGo.interactable = true;
                LeanTween.dispatchEvent(Event.SCENE,Event.GLOW);
            }
        }
        public void Check()
        {
            btnGo.interactable = false;
            var check = frames.Where(x => x.isCheck()).ToArray();
            LeanTween.dispatchEvent(Event.SOUND, "drag");
            if (check.Length == frames.Length)
            {
                for (int i = 0; i < drags.Length; i++)
                {
                    drags[i].enabled = false;
                }
                LeanTween.dispatchEvent(Event.CHANGE,transform);
            }
            else
            {
               for(int i = 0; i < drags.Length; i++)
               {
                    drags[i].No();
               }
                LeanTween.dispatchEvent(Event.SCENE, Event.FAIL);
            }
        }
        public void clear()
        {
            for (int i = 0; i < drags.Length; i++)
            {
                drags[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
