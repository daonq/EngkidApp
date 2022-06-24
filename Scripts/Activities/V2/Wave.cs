using common;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using UniRx;
using System.Linq;
namespace V2
{
    public class Wave : MonoBehaviour
    {
        private Vector3 _oldPos;
        private AudioSource _audio;
        private SkeletonGraphic _bone;
        public Vector3[] Points;
        public Vector3[] PointsBack;
        public Trash[] trash;
        public Vector3 posOut;
        public float time = 2;
        public Vector3[] trashPos;
        private void Awake()
        {
            _bone = transform.GetChild(0).GetComponent<SkeletonGraphic>();
            _oldPos = _bone.transform.localPosition;
        }
        /*
        public void Move()
        {
            int i = 0;
            _bone.transform.localEulerAngles = Vector3.zero;
            Points.ToObservable().Subscribe(x =>
            {
                var time = i*Delay;
                var n = i;
                LeanTween.moveLocal(_bone.gameObject,x, Delay).setOnComplete(() =>
                {
                    trash[n].gameObject.SetActive(false);
                }).setDelay(time);
                i++;
            });
            LeanTween.moveLocal(_bone.gameObject, posOut, Delay).setDelay(Points.Length * Delay).setOnComplete(Back);
        }
        public void Back()
        {
            _bone.transform.localEulerAngles = new Vector3(0, 0, 180);
            int i = Points.Length;
            PointsBack.ToObservable().Subscribe(x =>
            {
                var time = (Points.Length -i) * Delay;
                var n = i;
                LeanTween.moveLocal(_bone.gameObject, x, Delay).setOnComplete(() =>
                {
                    trash[n-1].gameObject.SetActive(true);
                }).setDelay(time);
                i--;
            });
            LeanTween.moveLocal(_bone.gameObject,_oldPos, Delay).setDelay(Points.Length * Delay);
        }*/
        public void Move()
        {
            _bone.gameObject.SetActive(true);
            _bone.transform.localEulerAngles = Vector3.zero;
            LeanTween.moveLocal(_bone.gameObject, posOut, time).setOnUpdate(ChangedNew).setOnComplete(() => {
                _bone.gameObject.SetActive(false);
            });
        }
        private void Changed(float val)
        {
            if (_bone.transform.localPosition.x >= Points[0].x && _bone.transform.localPosition.x < Points[1].x)
            {
                trash[0].fadeOut();
            }
            if (_bone.transform.localPosition.x >= Points[1].x && _bone.transform.localPosition.x < Points[2].x)
            {
                trash[1].fadeOut();
            }
            if (_bone.transform.localPosition.x >= Points[2].x && _bone.transform.localPosition.x < Points[3].x)
            {
                trash[2].fadeOut();
            }
            if (_bone.transform.localPosition.x >= Points[3].x)
            {
                trash[3].fadeOut();
            }
        }

        private void ChangedNew(float val)
        {
            Debug.Log(trashPos.Length);
            if (_bone.transform.localPosition.x >= Points[0].x && _bone.transform.localPosition.x < Points[1].x)
            {
                var trash01 = trash.Where(x => x.transform.localPosition == trashPos[0]).ToArray();
                if (trash01.Length > 0)
                {
                    trash01[0].fadeOut();
                }
            }
            if (_bone.transform.localPosition.x >= Points[1].x && _bone.transform.localPosition.x < Points[2].x)
            {
                var trash02 = trash.Where(x => x.transform.localPosition == trashPos[1]).ToArray();
                if (trash02.Length > 0)
                {
                    trash02[0].fadeOut();
                }
            }
            if (_bone.transform.localPosition.x >= Points[2].x && _bone.transform.localPosition.x < Points[3].x)
            {
                var trash03 = trash.Where(x => x.transform.localPosition == trashPos[2]).ToArray();
                if (trash03.Length > 0)
                {
                    trash03[0].fadeOut();
                }
            }
            if (_bone.transform.localPosition.x >= Points[3].x)
            {
                var trash04 = trash.Where(x => x.transform.localPosition == trashPos[3]).ToArray();
                if (trash04.Length > 0)
                {
                    trash04[0].fadeOut();
                }
            }
        }
        private void changeByPos(float val)
        {
            int i = 0;
            trash.ToObservable().Subscribe(xx => { 
                if(_bone.transform.localPosition.x == PointsBack[i].x)
                {
                    xx.fadeOut();
                }
                i++;
            });
        }
        public void Back()
        {
            _bone.gameObject.SetActive(true);
            _bone.transform.localEulerAngles = new Vector3(0, 0, 180);
            _bone.transform.localPosition = posOut;
            LeanTween.dispatchEvent(V2Event.ACTION, V2Event.TRASH);
            LeanTween.moveLocal(_bone.gameObject, _oldPos, time).setOnComplete(() => {
                _bone.gameObject.SetActive(false);
                LeanTween.dispatchEvent(V2Event.ACTION, V2Event.FADEIN);
                trash.ToObservable().Subscribe(x => x.fadeIn());
            });
            
        }
        private void ChangedBack(float val)
        {
            if (_bone.transform.localPosition.x <= PointsBack[0].x && (_bone.transform.localPosition.x > PointsBack[1].x))
            {
                trash[3].fadeIn();
            }
            if (_bone.transform.localPosition.x <= PointsBack[1].x && (_bone.transform.localPosition.x > PointsBack[2].x))
            {
                trash[2].fadeIn();
            }
            if (_bone.transform.localPosition.x <= PointsBack[2].x && (_bone.transform.localPosition.x > PointsBack[3].x))
            {
                trash[1].fadeIn();
            }
            if ((_bone.transform.localPosition.x <= PointsBack[3].x))
            {
                trash[0].fadeIn();
            }
        }
    }
}
