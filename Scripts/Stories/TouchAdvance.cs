using UnityEngine;
using UnityEngine.EventSystems;
using BookCurlPro;
namespace Stories
{
	public class TouchAdvance: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private Vector3 beginPosition;
		private Vector3 mouseDeltal;
		public void OnBeginDrag(PointerEventData eventData)
		{
			beginPosition = Input.mousePosition;
		}
		public void OnDrag(PointerEventData eventData)
		{
			if (Input.touchCount > 1)
				return;
		}
        //Kiểm tra sự kiện vuốt trái hay vuốt phải rồi trả về
        public void OnEndDrag(PointerEventData eventData)
		{
			mouseDeltal = Input.mousePosition - beginPosition;
			if (mouseDeltal.x > Mathf.Abs (mouseDeltal.y)) {
                LeanTween.dispatchEvent(StoryEvent.TouchPage, StoryEvent.PAGELEFT);
            } 
			if (mouseDeltal.x < -Mathf.Abs (mouseDeltal.y)){
                LeanTween.dispatchEvent(StoryEvent.TouchPage, StoryEvent.PAGERIGHT);
            }
		}
	}
}

