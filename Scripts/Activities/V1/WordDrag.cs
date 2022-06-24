using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using System.Linq;
using common;
namespace V1
{
    public class WordDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler,IPointerClickHandler
    {
        public Vector3 oldPos;
        public float MAX = 40;
        public bool isDrag = false;
        public bool isLock = false;
        public bool isTutorial = false;
        public bool isTutorialEnd = false;
        public int id = 0;
        private float[] _Distances;
        public WordManager[] wordmanagers;
        private WordDrop[] _wordDrops;
        private float DistancesMin;
        private WordManager _wordmanager;
        private WordDrop _wordDrop;
        private int _SiblingIndex;
        void Awake()
        {
            _wordDrops = FindObjectsOfType<WordDrop>();
            _Distances = new float[_wordDrops.Length];
            _SiblingIndex = transform.GetSiblingIndex();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if ((!isDrag && !isLock))
            {
                GetComponent<AudioSource>().Play();
                LeanTween.textAlpha(transform.GetChild(1).GetComponent<RectTransform>(), 1f, 0.5f).setFrom(0f).setEase(LeanTweenType.easeOutQuad);
            }
        }
        public void Glow()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            //LeanTween.alpha(transform.GetChild(0).GetComponent<RectTransform>(), 1f,0.3f).setFrom(0f).setRepeat(12);
        }
        public void GlowEnd(bool IsEnd)
        {
            transform.GetChild(0).gameObject.SetActive(IsEnd);
        }
        public void CloseGlow()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        public void Close()
        {
            LeanTween.cancel(gameObject);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isLock && !isDrag)
            {
                isDrag = true;
                transform.GetChild(0).gameObject.SetActive(isTutorialEnd|| isTutorial ? true:false);
                if (isTutorial)
                {
                    //transform.GetChild(0).gameObject.SetActive(true);
                    LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.TUTORIAL);
                }
                LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.CHANGEPOS);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isLock && isDrag)
            {
                transform.position = Input.mousePosition;
                transform.SetAsLastSibling();
                findNear();
            }
        }
        //tìm đối tượng ở gần 
        private void findNear()
        {
            int i = 0;
            _wordDrops.ToObservable().Subscribe(xx => {
                xx.Distance = Vector3.Distance(transform.position, xx.transform.position);
                _Distances[i++] = xx.Distance;
            });
            wordmanagers.Where(w =>!w.isLock).ToObservable().Subscribe(x => x.Clear());
            DistancesMin = Mathf.Min(_Distances);
            _wordmanager = null;
            _wordDrop = null;
            if (DistancesMin < MAX)
            {
                if (id == 0) LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.TUTORIALDTAPNEAR);
                _wordDrop = _wordDrops.ToArray().Where(xxx => Mathf.Abs(xxx.Distance - DistancesMin) < 0.01).ToArray()[0];
                if (_wordDrop != null) {
                    _wordmanager = wordmanagers[_wordDrop.id];
                    if (!_wordmanager.isLock && !isTutorialEnd) _wordmanager.Glow(1);
                }
            }
            if (isTutorialEnd) wordmanagers[id].Glow(1);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isLock && isDrag)
            {
                if (_wordDrop != null && _wordmanager!=null && _wordDrop.id == _wordmanager.id && id == _wordDrop.id)
                {
                    LeanTween.dispatchEvent(WordEvent.SOUND,"yes");
                    LeanTween.move(GetComponent<RectTransform>(),_wordDrop.transform.localPosition, 0.1f).setOnComplete(() =>
                    {
                        transform.localScale = Vector3.zero;
                        _wordmanager.transform.SetAsLastSibling();
                        _wordmanager.RunSub();
                        if (isTutorial) LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.TUTORIALDROP);
                        if (isTutorialEnd) LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.TUTORIALWHENEND);
                        if(!isTutorial && !isTutorialEnd) LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.BONUS);
                    });
                }
                else
                {
                    if (_wordmanager != null && !_wordmanager.isLock)
                    {
                        _wordmanager.Glow(0);
                        LeanTween.delayedCall(1,()=>_wordmanager.Clear());
                    }
                    isDrag = false;
                    isLock = false;
                    LeanTween.dispatchEvent(WordEvent.SOUND,"no");
                    LeanTween.move(GetComponent<RectTransform>(),oldPos,0.3f).setOnComplete(() => {
                        transform.SetSiblingIndex(_SiblingIndex);
                        if (isTutorialEnd) wordmanagers[id].Glow(1);
                        LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.CHANGEPOS);
                        if (isTutorial) LeanTween.dispatchEvent(WordEvent.ACTION, WordEvent.TUTORIALTAP);
                    });
                }
            }
        }
    }
}
