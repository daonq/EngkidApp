using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSlideBehavior : MonoBehaviour
{
    [Header("Intro Slide Pages")]
    public GameObject m_PagesHolder;
    public Vector3 m_MovementSnapVector = new Vector3(1920.0f, 0.0f, 0.0f);
    public List<GameObject> m_PagesList = new List<GameObject>();
    public GameObject m_RightButton;
    public GameObject m_LeftButton;

    [Header("Page Paginators")]
    public Sprite m_PaginatorDefault;
    public Sprite m_PaginatorHighlighted;
    public List<Image> m_PaginatorsList = new List<Image>();

    [Header("SoundHolder")]
    public SoundHolder m_SoundHolder;
    public AudioClip swipe_sound;

    [Header("On finialize kid info settings")]
    public LogInSceneScreensManager m_ScreenManager;

    //internal
    int currentPageIndex = 0;

    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerSwipe += HandleScreenSwipe;
        StartCoroutine(AutoRunSlide());
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerSwipe -= HandleScreenSwipe;
    }

    private void Start()
    {
        foreach (Image pagin in m_PaginatorsList)
        {
            pagin.sprite = m_PaginatorDefault;
        }
        m_PaginatorsList[0].sprite = m_PaginatorHighlighted;

        m_LeftButton.SetActive(false);
    }

    public void OnSkipClicked()
    {
        while (currentPageIndex != 0)
        {
            OnPreviousPage();
            float new_pos = -m_MovementSnapVector.x * currentPageIndex;
            LeanTween.moveLocalX(m_PagesHolder, new_pos, 0.25f);
        }
    }

    public void OnNextPage()
    {
        if (currentPageIndex < m_PagesList.Count - 1)
        {
            currentPageIndex++;
            m_LeftButton.SetActive(true);

            if (currentPageIndex >= m_PagesList.Count - 1)
            {
                m_RightButton.SetActive(false);
            }
            else
            {
                m_RightButton.SetActive(true);
            }

            if (LeanTween.isTweening(m_PagesHolder) == false)
            {
                float new_pos = -m_MovementSnapVector.x * currentPageIndex;
                LeanTween.moveLocalX(m_PagesHolder, new_pos, 0.25f);
            }
        }
        else
        {
            m_RightButton.SetActive(false);
        }

        foreach (Image pagin in m_PaginatorsList)
        {
            pagin.sprite = m_PaginatorDefault;
        }
        m_PaginatorsList[currentPageIndex].sprite = m_PaginatorHighlighted;
    }

    public void OnPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            m_RightButton.SetActive(true);

            if (currentPageIndex <= 0)
            {
                m_LeftButton.SetActive(false);
            }
            else
            {
                m_LeftButton.SetActive(true);
            }

            if (LeanTween.isTweening(m_PagesHolder) == false)
            {
                float new_pos = -m_MovementSnapVector.x * currentPageIndex;
                LeanTween.moveLocalX(m_PagesHolder, new_pos, 0.25f);
            }
        }
        else
        {
            m_LeftButton.SetActive(false);
        }

        foreach (Image pagin in m_PaginatorsList)
        {
            pagin.sprite = m_PaginatorDefault;
        }
        m_PaginatorsList[currentPageIndex].sprite = m_PaginatorHighlighted;
    }

    public void HandleScreenSwipe(Lean.Touch.LeanFinger finger)
    {
        if (finger.StartScreenPosition.x < finger.LastScreenPosition.x)
        {
            if (currentPageIndex > 0)
            {
                m_SoundHolder.PlaySound(swipe_sound);
            }
            OnPreviousPage();
        }
        else if (finger.StartScreenPosition.x > finger.LastScreenPosition.x)
        {
            if (currentPageIndex < m_PagesList.Count - 1)
            {
                m_SoundHolder.PlaySound(swipe_sound);
            }
            OnNextPage();
        }
    }

    public void OnBackClick()
    {
        while (currentPageIndex != 0)
        {
            OnPreviousPage();
            float new_pos = -m_MovementSnapVector.x * currentPageIndex;
            LeanTween.moveLocalX(m_PagesHolder, new_pos, 0.25f);
        }
        m_ScreenManager.OnOptionWindow();
    }

    IEnumerator AutoRunSlide()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            OnNextPage();
        }
    }
}
