using common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Spine;
using Spine.Unity;
namespace V2
{
    public class Trash : MonoBehaviour, IPointerClickHandler
    {
        public Vector3 oldPos;
        private Vector3 _oldScale;
        public Transform point;
        public float time = 2;
        private AudioSource _audio;
        private Text _text;
        public string charYes = "";
        private SkeletonGraphic spine;
        public bool isLock=false;
        public bool isShoot = false;
        public Color ColorSpine;
        public Color ColorText;
        private Color _alpha = new Color(255, 255, 255, 0);

        private void Awake()
        {
            oldPos = transform.localPosition;
            _oldScale = transform.localScale;
            _audio = GetComponent<AudioSource>();
            _text = transform.GetChild(1).GetComponent<Text>();
            spine = transform.GetChild(0).GetComponent<SkeletonGraphic>();
            ColorSpine = spine.color;
            ColorText = _text.color;
        }
        private void Start()
        {
            clear();
            GetComponent<Image>().color = _alpha;
        }
        public void clear()
        {
            transform.localPosition = point.localPosition;
            transform.localScale = Vector3.zero;
            spine.AnimationState.SetAnimation(0,gameObject.name,true);
        }
        public void Back()
        {
            transform.localPosition = oldPos;
            spine.AnimationState.SetAnimation(0,gameObject.name,true);
        }
        public void setText(string content)
        {
            _text.text = content;
            charYes = content;
        }
        public void Drop()
        {
            transform.localPosition = point.localPosition;
            transform.localScale = Vector3.zero;
            LeanTween.moveLocal(gameObject,oldPos,time).setEaseInQuad();
            LeanTween.scale(gameObject,_oldScale,time).setOnComplete(()=> {
                _audio.Play();
            });
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!isLock) LeanTween.dispatchEvent(V2Event.TAP, transform);
        }
        public void Glow()
        {
            spine.AnimationState.SetAnimation(0, gameObject.name+"_glow", true);
        }
        public void noneGlow()
        {
            spine.AnimationState.SetAnimation(0, gameObject.name, true);
        }
        public void fadeOutTutorial()
        {
            GetComponent<Image>().enabled = false;
            LeanTween.alpha(GetComponent<RectTransform>(), 0, 0.2f).setFrom(1).setOnComplete(() =>
            {
                GetComponent<Image>().enabled = true;
                clear();
            });

        }
        public void fadeOut()
        {
            //LeanTween.alpha(GetComponent<RectTransform>(), 0, 0.2f).setFrom(1);
            gameObject.SetActive(false);
        }
        public void fadeIn()
        {
            transform.localScale = Vector3.one;
            LeanTween.alpha(GetComponent<RectTransform>(), 0,0);
            spine.AnimationState.SetAnimation(0, gameObject.name, true);
            gameObject.SetActive(true);
            GetComponent<Image>().enabled = false;
            LeanTween.alpha(GetComponent<RectTransform>(), 1, 0.5f).setOnComplete(() =>
            {
                GetComponent<Image>().enabled = true;
                GetComponent<Image>().color = _alpha;
            });
        }
    }
}
