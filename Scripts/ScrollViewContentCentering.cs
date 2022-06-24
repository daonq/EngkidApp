using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewContentCentering : MonoBehaviour
{
    public RectTransform m_DefaultTarget;
    ScrollRect scrollRect;

    private void Start()
    {
        if (scrollRect == null)
            scrollRect = this.GetComponent<ScrollRect>();

        if (m_DefaultTarget != null)
        {
            LeanTween.cancel(this.gameObject);

            if (scrollRect == null)
                scrollRect = this.GetComponent<ScrollRect>();

            UIExtensions.ScrollToCenter(scrollRect, this.gameObject, m_DefaultTarget, RectTransform.Axis.Horizontal);
        }    
    }

    public void OnCenteringOnElement(RectTransform target)
    {
        LeanTween.cancel(this.gameObject);

        if (scrollRect == null)
            scrollRect = this.GetComponent<ScrollRect>();

        UIExtensions.ScrollToCenter(scrollRect, this.gameObject, target, RectTransform.Axis.Horizontal);
    }
}
