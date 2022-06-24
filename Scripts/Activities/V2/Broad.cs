using common;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
namespace V2
{
    public class Broad : MonoBehaviour
    {
        private Vector3 _oldPos;
        private SkeletonGraphic _spine;
        [Header("GameObject")]
        public GameObject window;
        public GameObject pic;
        public GameObject word;
        public GameObject content;
        public Vector3 PosWindow = new Vector3(500, 0, 0);
        private Vector3 _oldVectorWindow;
        public GameObject glow;
        public bool _isText;
        private void Awake()
        {
            _oldPos = transform.localPosition;
            _spine = transform.GetChild(0).GetComponent<SkeletonGraphic>();
            _oldVectorWindow = window.transform.localPosition;
        }
        private void Start()
        {
            clear(true,false);
        }
        public void clear(bool isAppear,bool isHide)
        {
            Scale(true);
            LeanTween.delayedCall(1, () => {
                if(isAppear) _spine.AnimationState.SetAnimation(0,"first_appear", false);
            });
            pic.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            _spine.gameObject.SetActive(isHide);
            word.SetActive(false);
            content.SetActive(false);
        }
        public void setText(string content)
        {
            word.GetComponent<Text>().text = content;
        }
        public void flipChange(bool isText)
        {
            _isText = isText;
            Scale(false);
            transform.GetChild(1).gameObject.SetActive(false);
            _spine.gameObject.SetActive(true);
            window.SetActive(false);
            pic.SetActive(false);
            content.SetActive(false);
            LeanTween.dispatchEvent(V2Event.SOUND, "flip");
            LeanTween.delayedCall(0.5f, () => {
                _spine.AnimationState.SetAnimation(0, "flip", false);
                LeanTween.delayedCall(0.5f, () => {
                    content.SetActive(true);
                    transform.GetChild(1).gameObject.SetActive(true);
                    window.SetActive(true);
                    window.transform.localPosition = _oldVectorWindow;
                    pic.SetActive(isText ? false : true);
                    word.SetActive(isText ? true : false);
                    _spine.AnimationState.SetAnimation(0, "idle", true);
                    LeanTween.moveLocal(window, PosWindow, 0.2f);
                });
            });
        }
        public void change()
        {
            flipChange(!_isText);
        }
        public void Glow(bool isGlow)
        {
            glow.SetActive(isGlow);
        }
        public void Vibrate()
        {
            Glow(true);
            LeanTween.delayedCall(2, () => Glow(false));
        }
        public void green()
        {
            _spine.AnimationState.SetAnimation(0, "green", false);
        }
        public void red()
        {
            _spine.AnimationState.SetAnimation(0, "red", false);
            LeanTween.delayedCall(0.5f, () => LeanTween.dispatchEvent(V2Event.SOUND, "fail"));
            LeanTween.delayedCall(1.3f, () => LeanTween.dispatchEvent(V2Event.SOUND, "red"));
            LeanTween.delayedCall(2, () => _spine.AnimationState.SetAnimation(0, "idle", true));
        }
        public void Scale(bool isScale)
        {
           LeanTween.scale(gameObject,Vector3.one, 1);
        }
        public void ScaleToZero()
        {
            LeanTween.scale(gameObject, Vector3.zero, 1);
            LeanTween.moveLocal(gameObject, new Vector3(0, 1400, 0), 1f).setOnComplete(() => {
                transform.localPosition = _oldPos;
            });
        }
    }
}
