using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G1
{
    public class Frame: MonoBehaviour
    {
        public int id = 0;
        public int Fail = 0;
        public float Distance = 0;
        public string OldContent;
        public string content;
        public Drag drag;
    }
}
