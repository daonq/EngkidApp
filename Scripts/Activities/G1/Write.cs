using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using common;
using UniRx;
namespace G1
{
    public class Write : MonoBehaviour
    {
        private Text txt;
        private string _contentOld;
        private string[] _words;
        public float delay = 0.2f;
        private string _ContentNew = "";
        public void Begin()
        {
            txt = GetComponent<Text>();
            _contentOld = txt.text;
            _words = _contentOld.Split(' ');
            Effect();
        }
        public void Effect()
        {
            int i = 0;
            _ContentNew = "";
            txt.text = "";
            _words.ToObservable().Subscribe(z => {
                LeanTween.delayedCall(delay * i++, () =>
                {
                    _ContentNew += z + " ";
                    txt.text = _ContentNew;
                });
            });
        }
    }
}
