using UnityEngine;
using UniRx;
using common;
using System.Linq;
namespace CAM_9
{
    [System.Serializable]
    public class MoveByPoint
    {
        public GameObject Item;
        public float Delay = 1;
        public Transform[] Points;
        public float miniDelay = 0.2f;
        public float Total()
        {
            return Delay + (Points.Length+1) * miniDelay;
        }
    }
}