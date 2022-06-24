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
    public class Frame: MonoBehaviour
    {
        public int id = 0;
        public float Distance = 0;
        public string content;
        public bool isCheck()
        {
            bool _isCheck = false;
            if (transform.childCount > 0)
            {
                Drag drag = transform.GetChild(0).GetComponent<Drag>();
                if (drag.transform.GetChild(0).GetComponent<Text>().text == content)
                {
                    _isCheck = true;
                }
            }
            return _isCheck;
        }
    }
}
