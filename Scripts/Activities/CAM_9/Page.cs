using UnityEngine;
using UnityEngine.UI;
using common;
using System.Collections.Generic;
using UniRx;
using Spine.Unity;
using UnityEngine.EventSystems;
namespace CAM_9
{
    public class Page : MonoBehaviour, IPointerClickHandler
    {
        private Vector3 _oldPos;
        private Transform _Parent;
        public Answer[] answers;
        public Vector3[] AllPos;
        public Transform content;
        public Dictionary<string, Vector3> dicPos = new Dictionary<string, Vector3>();
        public CanvasScaler canvasScaler;
        private Vector3 _oldPosOfContent;
        public SkeletonGraphic hand;
        public HandWrite[] handWrites;
        private void Awake()
        {
            _oldPos = transform.localPosition;
            _Parent = transform.parent;
            if(content!=null) _oldPosOfContent = content.localPosition;
            
            if (AllPos.Length > 0)
            {
                int i = 1;
                AllPos.ToObservable().Subscribe(xx => dicPos.Add("Answer0" + (i++).ToString(), xx));
                LeanTween.addListener(gameObject, Cam9Event.EDIT, OnEditEvent);
                LeanTween.addListener(gameObject, Cam9Event.INPUT, OnInputEvent);
            }
        }
        private void Start()
        {
            if (handWrites.Length > 0)
            {
                handWrites.ToObservable().Subscribe(x => {
                    x.movebyPoint.ToObservable().Subscribe(y => y.Item.SetActive(false));
                });
            }
            
        }
        public void BackToBegin()
        {
            transform.SetParent(_Parent);
            transform.localPosition = _oldPos;
            gameObject.SetActive(false);
        }
        void OnEditEvent(LTEvent e)
        {
            if (gameObject.activeSelf)
            {
                string Name = (e.data as Transform).name;
                if (Name != null && AllPos.Length > 0)
                {
                    if (content != null && dicPos.Count > 0)
                    {
                        Vector3 pos = dicPos[Name];
                        if (pos != null) content.localPosition = pos;
                    }
                }
                LeanTween.dispatchEvent(Cam9Event.SOUND, "tapTextFiled");
            }
        }
        void OnInputEvent(LTEvent e)
        {
            if(AllPos.Length>0 && content != null) content.localPosition = _oldPosOfContent;
        }
        public void Write(int k)
        { 
            if(handWrites.Length>0)
                 handWrites[k].Write(hand.transform);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            LeanTween.dispatchEvent(Cam9Event.ACTION, "hideHint");
        }
        public void Reset()
        {
            Start();
            answers.ToObservable().Subscribe(x => x.Reset());
            handWrites.ToObservable().Subscribe(xx => xx.reset());
        }
    }
}
