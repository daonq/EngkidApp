using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using common;
namespace CAM_9
{
    public class Check : MonoBehaviour
    {
        private Button _Done;
        private Image _hint;
        private void Awake()
        {
            _Done = transform.GetChild(1).GetComponent<Button>();
            _hint = transform.GetChild(0).GetComponent<Image>();
        }
        private void Start()
        {
            _Done.interactable = false;
            _Done.gameObject.SetActive(false);
            _hint.gameObject.SetActive(false);
        }
        public void begin()
        {
            _Done.interactable = false;
            _Done.gameObject.SetActive(true);
        }
        public void Active()
        {
            _Done.interactable = true;
        }
        public void goCheck()
        {
            LeanTween.dispatchEvent(Cam9Event.ACTION, "Check");
            _Done.interactable = false;
        }
        public void showHint()
        {
            _hint.gameObject.SetActive(true);
        }
        public void closeHint()
        {
            _hint.gameObject.SetActive(false);
        }
        public void Hint()
        {
            LeanTween.dispatchEvent(Cam9Event.ACTION, "Hint");
        }
        public void hideHint()
        {
            LeanTween.dispatchEvent(Cam9Event.ACTION, "hideHint");
        }
    }
}
