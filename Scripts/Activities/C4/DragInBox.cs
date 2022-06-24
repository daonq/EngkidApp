using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UniRx;
namespace C4
{
    public class DragInBox : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public GameObject[] targets;
        private Vector3 _oldPos;
        public bool isPrint = false;
        public float MAX = 80;
        public Frame[] frame;
        public DragInFrame[] dragInFrames;
        private Window _window;
        private float[] _Distances;
        private float DistancesMin;
        public int id = 0;
        public int Fail = 0;
        private Frame _box;
        void Awake()
        {
            _oldPos = transform.localPosition;
            _window = transform.parent.GetComponent<Window>();
        }
        public void Begin()
        {
            frame = targets.Select(x => x.GetComponent<Frame>()).ToArray();
            _Distances = new float[frame.Length];
        }
        public void setContent(Sprite pic)
        {
            GetComponent<Image>().sprite = pic;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
            _window.Alpha();
            dragInFrames.ToObservable().Subscribe(s => s.GetComponent<Image>().maskable = false);
            LeanTween.dispatchEvent(Event.ACTION, Event.BEGIN_DRAG);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
                return;
            transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
           
            int i = 0;
            frame.ToObservable().Subscribe(xx => {
                xx.Distance = Vector3.Distance(transform.position, xx.transform.position);
                _Distances[i++] = xx.Distance;
            });
            DistancesMin = Mathf.Min(_Distances);
            _box = frame.Where(xxx => xxx.Distance == DistancesMin).ToArray()[0];
            if ((DistancesMin < MAX) && !_box.isYes) Yes();
            else No();
            dragInFrames.ToObservable().Subscribe(s => s.GetComponent<Image>().maskable = true);
            LeanTween.dispatchEvent(Event.ACTION, Event.END_DRAG);
        }
        private void No()
        {
            transform.localPosition = _oldPos;
        }
        /*
        private void YesEnd()
        {
            transform.localScale = Vector3.zero;
            Frame _box = frame.Where(xxx => xxx.id == id).ToArray()[0];
            if (_box != null)
            {
                _box.setContentBySpirte(id);
                _box.dragInFrame.enabled = true;
                _box.dragInFrame.setContent(_box.pics[id]);
                _box.dragInFrame.Begin();
                _box.id = id;
                _box.dragInFrame.id = id;
                LeanTween.dispatchEvent(Event.DRAG,-1);
            }
        }*/
        private void Yes()
        {
            transform.localScale = Vector3.zero;
            if (_box != null)
            {
                _box.setContentBySpirte(id);
                _box.dragInFrame.enabled = true;
                _box.dragInFrame.setContent(_box.pics[id]);
                _box.dragInFrame.Begin();
                _box.id = id;
                _box.dragInFrame.id = id;
                LeanTween.dispatchEvent(Event.DRAG,-1);
            }
        }
        public void Back()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
        }
        public void close()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
            Begin();
        }
        public void end()
        {
            transform.localPosition = _oldPos;
            Begin();
        }
    }
}
