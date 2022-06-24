using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewSelectionTrigger : MonoBehaviour
{
    public ScrollViewContentCentering m_ContentCentering;

    //internals
    RectTransform currentRectTransform;
    AudioSource audioSource = null;

    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;

        if (audioSource == null)
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.clip = BGMManagerBehavior.GetInstance().m_ScrollviewSwipeClip;
        }
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        RectTransform temp = currentRectTransform;

        var fingers = Lean.Touch.LeanTouch.Fingers;
        if (temp != null && fingers.Count == 0)
            m_ContentCentering.OnCenteringOnElement(temp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            RectTransform temp = other.GetComponent<RectTransform>();

            var fingers = Lean.Touch.LeanTouch.Fingers;
            if (temp != null && fingers.Count == 0)
                m_ContentCentering.OnCenteringOnElement(temp);

            currentRectTransform = temp;

            audioSource.Stop();
            audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            RectTransform temp = other.GetComponent<RectTransform>();

            var fingers = Lean.Touch.LeanTouch.Fingers;
            if (temp != null && fingers.Count == 0)
                m_ContentCentering.OnCenteringOnElement(temp);

            currentRectTransform = temp;
        }
    }
}
