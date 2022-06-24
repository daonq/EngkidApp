using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace G5
{
    public class Drag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public GameObject targets;
        public GameObject DragDisable;
        private float _Distances;
        private Vector3 _oldPos;
        public bool isPrint = false;
        void Awake()
        {
            _oldPos = transform.localPosition;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!G5Event.IsLock)
            {
                DragDisable.SetActive(true);
                transform.SetAsLastSibling();
                LeanTween.dispatchEvent(G5Event.SOUND, "tap");
                LeanTween.dispatchEvent(G5Event.ACTION, G5Event.DRAG);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
                return;
            if (!G5Event.IsLock) transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _Distances = Vector3.Distance(transform.position, targets.transform.position);
            if (!G5Event.IsLock && _Distances < 80) Yes(); else No();
        }
        private void No()
        {
            DragDisable.SetActive(false);
            transform.localPosition = _oldPos;
        }
        private void Yes()
        {
            LeanTween.dispatchEvent(G5Event.SOUND, "drag");
            targets.transform.GetChild(1).GetComponent<Text>().text = transform.GetChild(1).GetComponent<Text>().text;
            transform.localScale = Vector3.zero;
            LeanTween.dispatchEvent(G5Event.DROP, transform);
        }
        public void close()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
        }
    }
}
