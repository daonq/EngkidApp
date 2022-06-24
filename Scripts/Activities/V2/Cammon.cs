using UnityEngine;
using UnityEngine.UI;
using common;
using Spine;
using Spine.Unity;
namespace V2
{
    public class Cammon : MonoBehaviour
    {
        private SkeletonGraphic animBone;
        private Vector3 _scale;
        private Vector3 _oldPos;
        public SkeletonGraphic beamBone;
        public SkeletonGraphic bubleBone;
        public SkeletonGraphic hitBone;
        public SkeletonGraphic exproBone;
        public Broad broad;
        private AudioSource _audio;
        public SkeletonGraphic hand;
        public Transform[] points;
        private Vector3 _handOldPos;
        private void Awake()
        {
            animBone = GetComponent<SkeletonGraphic>();
            _scale = transform.localScale;
            _oldPos = transform.localPosition;
            _audio = GetComponent<AudioSource>();
            _handOldPos = hand.transform.localPosition;
        }
        private void Start()
        {
            hand.gameObject.SetActive(false);
            hitBone.gameObject.SetActive(false);
            exproBone.gameObject.SetActive(false);
        }
        public void setContent(string content)
        {
            broad.GetComponent<Broad>().setText(content);
        }
        public void showBroad(bool isTutorial)
        {
            if (isTutorial)
            {
                hand.gameObject.SetActive(true);
                broad.GetComponent<Broad>().flipChange(false);
            } 
            else
            {
                broad.GetComponent<Broad>().flipChange(true);
            }
           
        }
        public void CheckBroad()
        {
            if (!broad._isText) broad.GetComponent<Broad>().flipChange(true);
        }
        public void begin()
        {
            transform.localScale = Vector3.zero;
            LeanTween.scale(gameObject, _scale, 1.5f).setOnComplete(Bubble);
            transform.localPosition = new Vector3(_oldPos.x, _oldPos.y - 500, _oldPos.z);
            LeanTween.moveLocal(gameObject,_oldPos,1.5f);
        }
        public void clear()
        {
            broad.clear(false,true);
        }
        public void end()
        {
            broad.clear(false, false);
        }
        public void OnTap()
        {
            animBone.AnimationState.SetAnimation(0, "fire", false);
            LeanTween.delayedCall(1, () => { animBone.AnimationState.SetAnimation(0, "idle", true); });
            broad.Vibrate();
            LeanTween.dispatchEvent(V2Event.ACTION, V2Event.TAPME);
        }
        private void Bubble()
        {
            bubleBone.gameObject.SetActive(true);
            bubleBone.AnimationState.SetAnimation(0, "animation", true);
            _audio.Play();
            LeanTween.delayedCall(5, () => {
                bubleBone.gameObject.SetActive(false);
            });
            LeanTween.delayedCall(10,Bubble);
        }
        public void handGo()
        {
            LeanTween.moveLocal(hand.gameObject, points[0].localPosition, 2).setOnComplete(() => {
                hand.AnimationState.SetAnimation(0,"tab",false);
                broad.Glow(true);
                LeanTween.delayedCall(4, () => { 
                    broad.GetComponent<Broad>().flipChange(true);
                    broad.Glow(false);
                    LeanTween.moveLocal(hand.gameObject, points[1].localPosition, 2).setOnComplete(() =>
                    {
                        hand.AnimationState.SetAnimation(0,"tab", false);
                        LeanTween.dispatchEvent(V2Event.ACTION, V2Event.GLOWTRASH);
                        LeanTween.moveLocal(hand.gameObject, points[1].localPosition, 1).setDelay(2).setOnComplete(() => {
                            LeanTween.dispatchEvent(V2Event.SOUND,"tap");
                            LeanTween.moveLocal(hand.gameObject,_handOldPos, 2);
                        });
                    });
                });
            });
        }
        public void fire()
        {
            beamBone.gameObject.SetActive(true);
            beamBone.AnimationState.SetAnimation(0, "shoot", false);
        }
        public void Shoot()
        {
            animBone.AnimationState.SetAnimation(0, "fire", false);
            LeanTween.delayedCall(0.5f, () => {
                hitBone.gameObject.SetActive(true);
                LeanTween.delayedCall(0.5f, () => {
                    animBone.AnimationState.SetAnimation(0, "idle", true);
                    LeanTween.delayedCall(0.5f, () => { 
                        hitBone.gameObject.SetActive(false);
                        LeanTween.delayedCall(0.5f, () =>
                        {
                            LeanTween.dispatchEvent(V2Event.ACTION,V2Event.BEGIN);
                        });
                    });
                });
            });
        }
        public void ExproSive()
        {
            exproBone.gameObject.SetActive(true);
            exproBone.AnimationState.SetAnimation(0, "explosive", false);
            LeanTween.dispatchEvent(V2Event.SOUND, "Explosive");
            LeanTween.delayedCall(1, () => {
                exproBone.gameObject.SetActive(false);
            });
        }
        public void led(int n)
        {
            if (n == 0) broad.green();
            if (n == 1) broad.red();
        }
        public void tapMe()
        {
            LeanTween.dispatchEvent(V2Event.ACTION, V2Event.TAPME);
        }
        public void BroadScale()
        {
            broad.Scale(true);
            LeanTween.moveLocal(gameObject,_oldPos,1.5f).setDelay(1);
        }
        public void ScaleToEnd()
        {
            broad.ScaleToZero();
           
        }
        public void HandOut()
        {
            LeanTween.cancel(hand.gameObject);
            hand.transform.localPosition = _handOldPos;
        }
    }
}
