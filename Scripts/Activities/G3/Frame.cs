using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;
namespace G3
{
    public class Frame : MonoBehaviour
    {
        public Text[] txts;
        public void Glow()
        {
            transform.GetChild(1).gameObject.SetActive(true);
            LeanTween.delayedCall(2, () => {
                transform.GetChild(1).gameObject.SetActive(false);
            });
        }
        public void Green()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            LeanTween.alpha(transform.GetChild(0).GetComponent<RectTransform>(),1, 0.5f).setFrom(0).setRepeat(4);
        }
        public void Red()
        {
            transform.GetChild(2).gameObject.SetActive(true);
            LeanTween.alpha(transform.GetChild(2).GetComponent<RectTransform>(), 1, 0.5f).setFrom(0).setRepeat(4);
        }
        public void close()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
