using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UniRx;
namespace C4
{
    public class DragInFrame : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public GameObject[] targets;
        private Vector3 _oldPos;
        public bool isPrint = false;
        public float MAX = 80;
        public Frame[] frame;
        private float[] _Distances;
        private float DistancesMin;
        public Frame myframe;
        public int id = 0;
        void Awake()
        {
            _oldPos = transform.localPosition;
        }
        void Start()
        {
            Alpha(true);
        }
        public void Begin()
        {
            frame = targets.Select(x => x.GetComponent<Frame>()).ToArray().Where(y=>y!=myframe).ToArray();
            _Distances = new float[frame.Length];
        }
        public void setContent(Sprite pic)
        {
            GetComponent<Image>().sprite = pic;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (id != -1)
            {
                transform.SetAsLastSibling();
                frame.ToObservable().Subscribe(x => {
                    x.GetComponent<Image>().maskable = false;
                });
                Alpha(false);
                myframe.Clear();
                LeanTween.dispatchEvent(Event.ACTION, Event.BEGIN_DRAG);
            }
            
        }
        public void Alpha(bool isAlpha)
        {
            LeanTween.alpha(GetComponent<RectTransform>(),isAlpha?0:1,0);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
                return;
            if (id!=-1) transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (id != -1)
            {
                int i = 0;
                frame.ToObservable().Subscribe(xx => {
                    xx.Distance = Vector3.Distance(transform.position, xx.transform.position);
                    _Distances[i++] = xx.Distance;
                });
                DistancesMin = Mathf.Min(_Distances);
                if (DistancesMin < MAX) Yes();
                else
                {
                    myframe.setContentBySpirte(id);
                }
                transform.localPosition = _oldPos;
                Alpha(true);
                frame.ToObservable().Subscribe(x => x.GetComponent<Image>().maskable = true);
                LeanTween.dispatchEvent(Event.ACTION, Event.END_DRAG);
            }
        }
        private void Yes()
        {
            Frame _box = frame.Where(xxx => xxx.Distance==DistancesMin).ToArray()[0];
            if (_box != null && !_box.isClose && _box.dragInFrame.enabled)
            {
                int oldId = _box.id;
                if (_box.content.activeSelf)
                {
                    setbyId(_box, id);
                    LeanTween.dispatchEvent(Event.DRAG, oldId);
                    setbyId(myframe, oldId);
                }
                else
                {
                    setbyId(_box, id);
                    myframe.content.SetActive(false);
                    id = -1;
                }
            }
            else
            {
                myframe.setContentBySpirte(id);
            }
        }
        public void setbyId(Frame _box,int _id) {
            _box.setContentBySpirte(_id);
            _box.dragInFrame.setContent(_box.pics[_id]);
            _box.dragInFrame.id = _id;
            _box.id = _id;
            _box.dragInFrame.enabled = true;
            _box.dragInFrame.Begin();
        }
        public void setNone(Frame _box)
        {
            _box.content.GetComponent<Image>().sprite = null;
            _box.dragInFrame.enabled = false;
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
    }
}
