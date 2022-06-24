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
    public class Box : MonoBehaviour
    {
        public GameObject[] status;
        public GameObject content;
        public int id = 0;
        public float Distance = 0;
        private void Start()
        {
            Clear();
        }
        public void Clear()
        {
            status.ToObservable().Subscribe(x => x.gameObject.SetActive(false));
            content.SetActive(false);
        }
        void SetStatus(int i)
        {
            Clear();
            status[i].SetActive(true);
        }
        public void glow()
        {
            SetStatus(0);
        }
        public void Content()
        {
            content.SetActive(true);
        }
    }
}
