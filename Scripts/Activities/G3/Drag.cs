using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using common;
namespace G3
{
    public class Drag : MonoBehaviour, IPointerClickHandler
    {
        public Transform target;
        public GameObject DragDisable;
        private float _Distances;
        private Vector3 _oldPos;
        private float MAX = 180;
        void Awake()
        {
            _oldPos = transform.localPosition;
        }
        void Start()
        {
            DragDisable.SetActive(false);
            LeanTween.alpha(DragDisable.GetComponent<RectTransform>(), 0.4f, 0.3f).setFrom(0f);
        }
        public void OnShowDisable()
        {
            DragDisable.SetActive(true);
            transform.localScale = Vector3.zero;
        }
        /*
        public void OnBeginDrag(PointerEventData eventData)
        {
            DragDisable.SetActive(true);
            transform.SetAsLastSibling();
        }
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _Distances = Vector3.Distance(transform.position,target.transform.position);
            Head head = transform.parent.parent.parent.GetChild(0).GetComponent<Head>();
            bool isOK = head.Content == transform.GetChild(0).GetComponent<Text>().text;
            if (_Distances < MAX && isOK) Yes(); else No();
        }*/
        public void OnPointerClick(PointerEventData eventData)
        {
            Head head = transform.parent.parent.parent.GetChild(0).GetComponent<Head>();
            bool isOK = head.Content == transform.GetChild(0).GetComponent<Text>().text;
            StopAllCoroutines();
            DragDisable.SetActive(true);
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            LeanTween.delayedCall(0.3f, () => {
                if (isOK) Yes(); else No();
            });
        }
        private void No()
        {
            transform.localPosition = _oldPos;
            transform.localScale = Vector3.zero;
            LeanTween.dispatchEvent(Event.SOUND, "no");
            LeanTween.delayedCall(0.5f, () => {
                LeanTween.dispatchEvent(Event.SOUND, "no" + Shuffle.createList(3)[0]);
            });
            transform.parent.GetComponent<Frame>().Red();
            LeanTween.dispatchEvent(Event.ACTION, Event.FAIL);
        }
        private void Yes()
        {
            
            transform.localScale = Vector3.zero;
            LeanTween.dispatchEvent(Event.YES, transform.parent.GetComponent<Frame>());
            LeanTween.dispatchEvent(Event.SOUND, "yes");
            LeanTween.delayedCall(0.5f, () => {
                LeanTween.dispatchEvent(Event.SOUND, "yes" + Shuffle.createList(3)[0]);
            });
            transform.parent.GetComponent<Frame>().Green();
        }
        public void close()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
            Start();
        }
    }
}
