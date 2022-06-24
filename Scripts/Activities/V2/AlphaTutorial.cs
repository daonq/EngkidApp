using common;
using UnityEngine;
using UniRx;
using Spine;
using Spine.Unity;
using UnityEngine.UI;
namespace V2
{
    public class AlphaTutorial : MonoBehaviour
    {
        public GameObject bg;
        private Trash[] _trashs;
        private Color _NewColor = new Color(255, 255, 255, 100);
        public void Start()
        {
            bg.SetActive(false);
        }
        public void Tutorial(Trash[] trashs)
        {
            _trashs = trashs;
            bg.SetActive(true);
            _trashs.ToObservable().Subscribe(z => {
                z.transform.GetChild(0).GetComponent<SkeletonGraphic>().color = _NewColor;
                z.transform.GetChild(1).GetComponent<Text>().color = _NewColor;
                z.transform.GetChild(1).GetComponent<Text>().GetComponent<Outline>().enabled = false;
                
            });
        }
        public void Close()
        {
            bg.SetActive(false);
            _trashs.ToObservable().Subscribe(z => {
                z.transform.GetChild(0).GetComponent<SkeletonGraphic>().color = z.ColorSpine;
                z.transform.GetChild(1).GetComponent<Text>().color = z.ColorText;
                z.transform.GetChild(1).GetComponent<Text>().GetComponent<Outline>().enabled = true;
            });
        }
    }
}
