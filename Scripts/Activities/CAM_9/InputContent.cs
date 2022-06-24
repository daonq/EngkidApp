using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using common;
namespace CAM_9
{
    public class InputContent : MonoBehaviour, IPointerClickHandler
    {
        public Color color = Color.black;
        public void OnPointerClick(PointerEventData eventData)
        {
            LeanTween.dispatchEvent(Cam9Event.EDIT,transform.parent);
            GetComponent<InputField>().textComponent.color = color;
        }
    }
}
