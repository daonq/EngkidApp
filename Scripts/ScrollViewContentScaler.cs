using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewContentScaler : MonoBehaviour
{
    public Transform m_ContentHolder;
    public List<GameObject> m_ContentList = new List<GameObject>();
    public int m_CurrentHighLightIndex = -1;

    //internals
    int currentIndex = 0;

    private void OnEnable()
    {
        StartCoroutine(DelayedGetContentList());
    }

    public IEnumerator DelayedGetContentList()
    {
        yield return null;
        yield return null;
        yield return null;
        m_ContentList.Clear();

        foreach (Transform child in m_ContentHolder)
        {
            m_ContentList.Add(child.gameObject);
            child.localScale = Vector3.one * 0.75f;
        }
    }

    public void OnScrollViewElementMoveOutOfFocus(GameObject element)
    {
        if (element == null)
        {
            return;
        }

        LeanTween.cancel(element);
        LeanTween.scale(element, Vector3.one * 0.75f, 0.25f);

        currentIndex = m_ContentList.IndexOf(element);

        if (currentIndex - 1 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 1]);
            LeanTween.scale(m_ContentList[currentIndex - 1], Vector3.one * 0.75f, 0.25f);
        }

        if (currentIndex + 1 < m_ContentList.Count)
        {
            if (m_ContentList[currentIndex + 1] == null)
            {
                return;
            }
            LeanTween.cancel(m_ContentList[currentIndex + 1]);
            LeanTween.scale(m_ContentList[currentIndex + 1], Vector3.one * 0.75f, 0.25f);
        }

        if (currentIndex - 2 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 2]);
            LeanTween.scale(m_ContentList[currentIndex - 2], Vector3.zero, 0.25f);
        }


        if (currentIndex + 2 < m_ContentList.Count)
        {
            LeanTween.cancel(m_ContentList[currentIndex + 2]);
            LeanTween.scale(m_ContentList[currentIndex + 2], Vector3.zero, 0.25f);
        }

        if (currentIndex - 3 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 2]);
            LeanTween.scale(m_ContentList[currentIndex - 2], Vector3.zero, 0.25f);
        }


        if (currentIndex + 3 < m_ContentList.Count)
        {
            LeanTween.cancel(m_ContentList[currentIndex + 2]);
            LeanTween.scale(m_ContentList[currentIndex + 2], Vector3.zero, 0.25f);
        }

        if (currentIndex - 4 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 2]);
            LeanTween.scale(m_ContentList[currentIndex - 2], Vector3.zero, 0.25f);
        }


        if (currentIndex + 4 < m_ContentList.Count)
        {
            LeanTween.cancel(m_ContentList[currentIndex + 2]);
            LeanTween.scale(m_ContentList[currentIndex + 2], Vector3.zero, 0.25f);
        }
    }

    public void OnScrollViewElementMoveToFocus(GameObject element)
    {
        if (element == null)
        {
            return;
        }

        LeanTween.cancel(element);
        LeanTween.scale(element, Vector3.one, 0.25f);

        currentIndex = m_ContentList.IndexOf(element);
        m_CurrentHighLightIndex = currentIndex;

        if (currentIndex - 1 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 1]);
            LeanTween.scale(m_ContentList[currentIndex - 1], Vector3.one * 0.75f, 0.25f);
        }

        if (currentIndex + 1 < m_ContentList.Count)
        {
            if (m_ContentList[currentIndex + 1] == null)
            {
                return;
            }
            LeanTween.cancel(m_ContentList[currentIndex + 1]);
            LeanTween.scale(m_ContentList[currentIndex + 1], Vector3.one * 0.75f, 0.25f);
        }

        if (currentIndex - 2 >= 0)
        {
            LeanTween.cancel(m_ContentList[currentIndex - 2]);
            LeanTween.scale(m_ContentList[currentIndex - 2], Vector3.zero, 0.25f);
        }


        if (currentIndex + 2 < m_ContentList.Count)
        {
            LeanTween.cancel(m_ContentList[currentIndex + 2]);
            LeanTween.scale(m_ContentList[currentIndex + 2], Vector3.zero, 0.25f);
        }
    }
}
