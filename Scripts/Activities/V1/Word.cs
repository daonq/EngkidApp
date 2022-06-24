using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace V1
{
    public class Word : MonoBehaviour
    {
        private string _content;
        private Text _text;
        public Image[] highlines;
        void Awake()
        {
            _text = transform.GetChild(1).GetComponent<Text>();
            _content = _text.text;
        }
        public void Start()
        {
            _text.text = "";
            Glow(-1);
        }
        public void Clear() {
            highlines.ToObservable().Subscribe(color => color.gameObject.SetActive(false));
        }
        public void Glow(int i)
        {
            int j = 0;
            highlines.ToObservable().Subscribe(color =>color.gameObject.SetActive(i == j++?true:false));
        }
        public void Show()
        {
            _text.text = _content;
        }
        public void Scale()
        {
            LeanTween.scale(_text.gameObject, 1.5f * Vector3.one, 0.3f);
            LeanTween.scale(_text.gameObject, Vector3.one, 0.3f).setDelay(0.3f);
        }
    }
}
