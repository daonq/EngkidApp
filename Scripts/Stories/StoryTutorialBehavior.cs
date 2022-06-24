using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTutorialBehavior : MonoBehaviour
{
    public RectTransform m_BookCorner;
    public RectTransform m_Hand;
    public Transform m_HandPivotA;
    public Transform m_HandPivotB;

    private void OnEnable()
    {
        LeanTween.alpha(m_Hand, 0.0f, 0.01f);
        LeanTween.alpha(m_BookCorner, 0.0f, 0.01f).setOnComplete(() => {
            LeanTween.alpha(m_Hand, 1.0f, 0.5f);
            LeanTween.alpha(m_BookCorner, 1.0f, 0.5f);
        });

        m_Hand.transform.position = m_HandPivotA.position;
        LeanTween.move(m_Hand.gameObject, m_HandPivotB, 3.0f).setDelay(1.0f).setLoopCount(3).setOnComplete(() => {
            m_Hand.transform.position = m_HandPivotA.position;
        });

        LeanTween.alpha(m_Hand, 0.0f, 0.5f).setDelay(10.0f);
        LeanTween.alpha(m_BookCorner, 0.0f, 0.5f).setDelay(10.0f).setOnComplete(() => {
            this.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
    }
}
