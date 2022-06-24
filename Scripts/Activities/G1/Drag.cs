using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;
namespace G1
{
    public class Drag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
    {
        public Transform[] targets;
        private GameObject DragDisable;
        private GameObject Glow;
        private float[] _Distances;
        private Vector3 _oldPos;
        private float DistancesMin;
        private float MAX = 80;
        public int id = 0;
        void Awake()
        {
            _oldPos = transform.localPosition;
            DragDisable = transform.parent.GetChild(0).gameObject;
            Glow = transform.parent.GetChild(1).gameObject;
            _Distances = new float[targets.Length];
        }
        void Start()
        {
            DragDisable.SetActive(false);
           
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Image a = DragDisable.GetComponent<Image>();
            if (transform.localScale == Vector3.one)
            {
                OnGlow();
                LeanTween.dispatchEvent(Event.SOUND, "click");
            }
        }
        public void OnGlow()
        {
            Glow.SetActive(true);
            Image a = DragDisable.GetComponent<Image>();
            a.color = new Color(a.color.r, a.color.g, a.color.b, 0);
            LeanTween.delayedCall(1,()=> Glow.SetActive(false));
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Event.LOCK)
            {
                StopScale();
                DragDisable.SetActive(true);
                Text txt01 = DragDisable.transform.GetChild(0).GetComponent<Text>();
                txt01.color = new Color(txt01.color.r, txt01.color.g, txt01.color.b, 0.4f);
                Image a = DragDisable.GetComponent<Image>();
                a.color = new Color(a.color.r, a.color.g, a.color.b, 0);
                Glow.SetActive(false);
                transform.SetAsLastSibling();
                LeanTween.dispatchEvent(Event.SOUND, "click");
                LeanTween.dispatchEvent(Event.ACTION, Event.DRAG);
            }
        }
        public void StopScale()
        {
            transform.GetChild(0).transform.localScale = Vector3.one;
            LeanTween.cancelAll();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
                return;
            if (!Event.LOCK)  transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Event.LOCK)
            {
                int i = 0;
                targets.ToObservable().Subscribe(xx => {
                    xx.GetComponent<Frame>().Distance = Vector3.Distance(transform.position, xx.transform.position);
                    _Distances[i++] = xx.GetComponent<Frame>().Distance;
                });
                DistancesMin = Mathf.Min(_Distances);
                Transform tran = targets.Where(xx => xx.GetComponent<Frame>().Distance == DistancesMin).ToArray()[0];
                GameObject btnDone = tran.parent.GetChild(4).gameObject;
                if (DistancesMin < MAX && btnDone.activeSelf) Yes(); else No();
            }
        }
        private void No()
        {
            DragDisable.SetActive(false);
            transform.localPosition = _oldPos;
        }
        private void Yes()
        {
            Transform target = targets.Where(xxx => xxx.GetComponent<Frame>().Distance == DistancesMin).ToArray()[0];
            LeanTween.dispatchEvent(Event.SOUND, "drag");
            target.GetComponent<Frame>().OldContent = target.GetChild(0).GetComponent<Text>().text;
            target.GetChild(0).GetComponent<Text>().text = transform.GetChild(0).GetComponent<Text>().text;
            transform.localScale = Vector3.zero;
            target.GetComponent<Frame>().drag = GetComponent<Drag>();
            LeanTween.dispatchEvent(Event.DROP,target.GetComponent<Frame>());
        }
        public void close()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
            Start();
        }
    }
}
