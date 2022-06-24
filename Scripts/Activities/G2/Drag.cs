using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using Spine.Unity;
namespace G2
{
    public class Drag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
    {
        public Transform[] targets;
        private float[] _Distances;
        public Vector3 _oldPos;
        private float DistancesMin;
        private float MAX = 80;
        private Transform _oldParent;
        private SkeletonGraphic water;
        public bool isDrag = false;
        private LTDescr _Tween;
        void Awake()
        {
            _Distances = new float[targets.Length];
            _oldParent = transform.parent;
            water = transform.GetChild(2).GetComponent<SkeletonGraphic>();
        }
        public void setForIpad()
        {
            transform.localPosition = transform.localPosition + new Vector3(0, -200, 0);
        }
        public void Begin()
        {
            
            _oldPos = transform.localPosition;
            wave();
        }
        public void wave()
        {
            if (!isDrag)
            {
                int id = LeanTween.moveLocalY(gameObject, _oldPos.y + 20, 2f).setLoopPingPong().setEaseSpring().id;
                _Tween = LeanTween.descr(id);
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Event.LOCK)
            {
                transform.SetParent(_oldParent);
                transform.SetAsLastSibling();
                _Tween.pause();
                LeanTween.dispatchEvent(Event.SCENE, Event.BEGIN_DRAG);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
                return;
            if (!Event.LOCK)  transform.position = Input.mousePosition;
        }
        public void Glow(bool isGlow)
        {
            transform.GetChild(1).gameObject.SetActive(isGlow);
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
                _Tween.pause();
                Glow(false);
                if (DistancesMin < MAX) Yes(); else No();
            }
        }
        public void No()
        {
            transform.SetParent(_oldParent);
            LeanTween.moveLocal(gameObject,_oldPos,0.3f).setOnComplete(() => {
                water.transform.localPosition = new Vector3(0,-100,0);
                water.gameObject.SetActive(true);
                LeanTween.dispatchEvent(Event.SOUND, "Drop");
                water.AnimationState.SetAnimation(0,"action",false);
                LeanTween.delayedCall(0.5f, () => {
                    isDrag = false;
                    _Tween.reset();
                });
            });
        }
        private void Yes()
        {
            isDrag = true;
            Transform tran = targets.Where(xx => xx.GetComponent<Frame>().Distance == DistancesMin).ToArray()[0];
            if (tran.childCount==1)
            {
                tran.GetChild(0).GetComponent<Drag>().No();
            }
            transform.SetParent(tran);
            transform.localPosition = Vector3.zero;
            LeanTween.dispatchEvent(Event.DRAG,tran.GetComponent<Frame>());
            LeanTween.dispatchEvent(Event.SOUND,"drag");
        }
        public void AutoDrop()
        {
            isDrag = true;
            _Tween.pause();
            string content = transform.GetChild(0).GetComponent<Text>().text;
            Transform tran = targets.Where(xx => xx.GetComponent<Frame>().content == content).ToArray()[0];
            transform.SetParent(tran);
            transform.localPosition = Vector3.zero;
        }
        public void close()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = _oldPos;
            transform.GetChild(0).gameObject.SetActive(true);
            _Tween.reset();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            Glow(true);
            LeanTween.delayedCall(0.5f, () => Glow(false));
        }
    }
}
