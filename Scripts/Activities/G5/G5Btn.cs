using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace G5
{
    public class G5Btn : MonoBehaviour, IPointerClickHandler
    {
        public Sprite Idle;
        public Sprite Active;
        public Sprite Disable;
        public bool IsActive;
        private void Start()
        {
            SetStatus(0);
        }
        public void SetStatus(int stt)
        {
            if (stt == 0)
            {
                GetComponent<Image>().sprite = Idle;
                IsActive = false;
            }
            if (stt == 1)
            {
                IsActive = true;
                GetComponent<Image>().sprite = Active;
            }
            if (stt == 2)
            {
                GetComponent<Image>().sprite = Disable;
                IsActive = false;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsActive)
            {
                SetStatus(2);
                LeanTween.dispatchEvent(G5Event.ACTION, gameObject.name);
            }
        }
    }
}
