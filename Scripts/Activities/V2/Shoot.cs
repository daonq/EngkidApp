using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Spine;
using Spine.Unity;
using common;
using UniRx;
using System.Linq;
namespace V2
{
    public class Shoot : MonoBehaviour
    {
        public UILineRenderer _line;
        public GameObject _tip;
        public Transform target;
        private Vector3 oldPos;
        public Trash[] trashs;
        private SkeletonGraphic _bone;
        private SkeletonGraphic _hit;
        private Vector3 _boneScale;
        public bool isTutorial = false;
        public Trash current;
        private Vector3 _oldLinePos;
        public Vector3 add = new Vector3(-50, 50, 0);
        public Vector3[] trashOldPos;
        private void Awake()
        {
            _line.UseNativeSize = true;
            oldPos = _tip.transform.localPosition;
            _bone = _tip.transform.GetChild(0).GetComponent<SkeletonGraphic>();
            _hit = _tip.transform.GetChild(1).GetComponent<SkeletonGraphic>();
            LeanTween.addListener(gameObject,V2Event.TAP,OnTapEvent);
            _boneScale = _bone.transform.localScale;
            _oldLinePos = _line.Points[1];
            trashOldPos = new Vector3[trashs.Length];
            int i = 0;
            trashs.ToObservable().Subscribe(x => {
                trashOldPos[i] = x.transform.localPosition;
                i++;
            });
        }
        private void Start()
        {
            _tip.SetActive(false);
        }
        public void Drop()
        {
            trashs.ToObservable().Subscribe(x => x.Drop());
        }
        public void clear()
        {
            trashs.ToObservable().Subscribe(x => x.clear());
        }
        void OnTapEvent(LTEvent e)
        {
            if (!isTutorial)
            {
                target = e.data as Transform;
                current = target.GetComponent<Trash>();
                if(current.isShoot) shoot();
                else LeanTween.dispatchEvent(V2Event.ACTION, V2Event.SHOOT);
                LeanTween.dispatchEvent(V2Event.SOUND,"tap");
            }
        }
        public void ChangePos()
        {
            int i = 0;
            int[] randoms = Shuffle.createList(trashs.Length);
            trashs.ToObservable().Subscribe(x => {
                x.transform.localPosition = trashOldPos[randoms[i]];
                i++;
            });
        }
        public void shoot()
        {
            _tip.SetActive(true);
             //OnPause();
            _line.Points[1] = _oldLinePos;
            _tip.transform.localScale = Vector3.one;
            _line.UseNativeSize = true;
            _hit.gameObject.SetActive(false);
            _bone.transform.localScale = _boneScale;
            _bone.gameObject.SetActive(true);
            _hit.transform.localScale = Vector3.zero;
            _bone.AnimationState.SetAnimation(0, "shooting", false);
            _bone.timeScale = 2;
            LeanTween.dispatchEvent(V2Event.SOUND, "shoot");
            LeanTween.moveLocal(_tip,target.localPosition+add,0.5f)
            .setOnUpdate(Changed)
            .setOnComplete(ToTrash);
            LeanTween.dispatchEvent(V2Event.ACTION,V2Event.SHOOT);
        }
        public void OnPause()
        {
            trashs.Where(y => y != target && y.transform.localScale!=Vector3.zero).ToArray().ToObservable().Subscribe(y => {
                y.Back();
                LeanTween.cancel(y.gameObject);
            });
            _tip.transform.SetAsLastSibling();
        }
        void ToTrash()
        {
            _bone.AnimationState.SetAnimation(0,"hit", false);
            _bone.timeScale = 2;
            target.transform.SetParent(_tip.transform);
            target.SetAsFirstSibling();
            LeanTween.dispatchEvent(V2Event.SOUND, "catch");
            LeanTween.delayedCall(0.5f, () => {
                _bone.AnimationState.SetAnimation(0, "back", false);
                _bone.timeScale = 0.5f;
                LeanTween.moveLocal(_tip,oldPos,1).setOnUpdate(Changed).setOnComplete(ToBin).setDelay(1);
            });
        }
        private void Changed(float val)
        {
            _line.UseNativeSize = true;
            _line.Points[1] = _tip.transform.localPosition;
        }
        private void ToBin()
        {
            _line.Points[1] = _oldLinePos;
            _hit.transform.localScale = Vector3.one;
            _hit.gameObject.SetActive(true);
            _hit.AnimationState.SetAnimation(0, "get hit", false);
            _bone.transform.localScale = Vector3.zero;
            target.localScale = Vector3.zero;
            target.transform.SetParent(_tip.transform.parent);
            LeanTween.delayedCall(0.3f, () => {
                _tip.transform.localScale = Vector3.zero;
                LeanTween.dispatchEvent(V2Event.ACTION, V2Event.PROCESS);
            }).setDelay(0.3f);
        }
    }
}
