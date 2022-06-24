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
    public class Hand : MonoBehaviour
    {
        public Sprite[] status;
        private Vector3 oldPos;
        private void Awake()
        {
            oldPos = transform.localPosition;
        }
        public void setBegin()
        {
            transform.localPosition = oldPos;
        }
        public void Drag()
        {
            setStatus(0);
        }
        public void Tap()
        {
            setStatus(1);
        }
        public void setStatus(int i)
        {
            GetComponent<Image>().sprite = status[i];
        }
    }
}
