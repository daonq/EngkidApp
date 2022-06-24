using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadingIndicatorBehavior : MonoBehaviour
{
    public float m_YPos = 15.0f;
    public List<GameObject> m_IndicatorsList = new List<GameObject>();
    public GameObject m_TextObject;

    private void OnEnable()
    {
        for (int i = 0; i < m_IndicatorsList.Count; i++)
        {
            m_IndicatorsList[i].SetActive(false);
        }
        if (m_TextObject)
            m_TextObject.SetActive(true);
    }

    public void StartAnimation()
    {
        if (m_TextObject)
            m_TextObject.SetActive(false);

        for (int i = 0; i < m_IndicatorsList.Count; i++)
        {
            m_IndicatorsList[i].SetActive(true);
        }

        StartCoroutine(AnimationSequence());
    }

    public void StopAnimation()
    {
        StopAllCoroutines();
        for (int i = 0; i < m_IndicatorsList.Count; i++)
        {
            LeanTween.cancel(m_IndicatorsList[i]);
        }

        for (int i = 0; i < m_IndicatorsList.Count; i++)
        {
            m_IndicatorsList[i].SetActive(false);
        }
        if (m_TextObject)
            m_TextObject.SetActive(true);
    }

    IEnumerator AnimationSequence()
    {
        int iterator = 0;
        while (true)
        {
            yield return new WaitForSeconds(m_IndicatorsList.Count * iterator + 1.0f);
            for (int i = 0; i < m_IndicatorsList.Count; i++)
            {
                LeanTween.moveLocalY(m_IndicatorsList[i], m_YPos, 0.5f).setLoopPingPong(1).setDelay(i);
            }
            iterator++;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        for (int i = 0; i < m_IndicatorsList.Count; i++)
        {
            LeanTween.cancel(m_IndicatorsList[i]);
        }
    }
}
