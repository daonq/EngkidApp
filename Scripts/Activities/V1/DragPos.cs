using UnityEngine;
using UniRx;
using common;
namespace V1
{
    public class DragPos : MonoBehaviour
    {
        public Transform[] DragList;
        public WordPos wordPos;
        void Awake()
        {
            int i = 0;
            DragList.ToObservable().Subscribe(xx => {
                wordPos.Pos[0].girdPos[i++] = xx.localPosition;
            });
        }
        public void ChangePos(Transform[] transforms)
        {
            int stt = DragList.Length - transforms.Length;
            int i = 0;
            transforms.ToObservable().Subscribe(xx => {
                LeanTween.moveLocal(xx.gameObject, wordPos.Pos[stt].girdPos[i],0.2f);
                i++;
            });

        }
    }

}
