using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDisplays : MonoBehaviour
{
    [Header("Display Settings")]
    public GameObject m_LockedDisplay;
    public GameObject m_UnlockedDisplay;
    public Canvas m_UnitCanvas;
    public Text m_UnitNumberText;
    public Text m_UnitStarsText;
    public GameObject m_NewlyUnlockedIcon;

    [Header("Testing Params")]
    public bool m_UnlockUnit = false;

    //internals
    int starAmount = 0;
    int maxStarsAmount = 63;
    EngKidAPI.UnitStates m_UnitState = EngKidAPI.UnitStates.LOCKED;
    Vector3 og_unlocked_scale;
    Vector3 og_locked_scale;

    private void OnEnable()
    {
        if (m_UnitCanvas != null)
            m_UnitCanvas.worldCamera = Camera.main;

        og_unlocked_scale = m_UnlockedDisplay.transform.localScale;
        og_locked_scale = m_LockedDisplay.transform.localScale;

        m_NewlyUnlockedIcon.SetActive(false);
    }

    private void Update()
    {
        if (m_UnlockUnit == true)
        {
            m_UnlockUnit = false;
            OnUnitStateChanged(EngKidAPI.UnitStates.UNLOCKED);
        }
    }

    public void InitDisplay(int unit_number, int stars_amount, int max_stars_amount, EngKidAPI.UnitStates unit_state)
    {
        if (m_UnitCanvas == null)
            return;

        maxStarsAmount = max_stars_amount;
        m_UnitState = unit_state;

        m_UnitNumberText.text = unit_number.ToString();
        m_UnitStarsText.text = starAmount.ToString() + "/" + maxStarsAmount.ToString();

        if (m_UnitState == EngKidAPI.UnitStates.LOCKED)
        {
            m_LockedDisplay.SetActive(true);
            m_UnlockedDisplay.SetActive(false);
        }
        else
        {
            m_LockedDisplay.SetActive(false);
            m_UnlockedDisplay.SetActive(true);
        }
    }

    public void OnUnitStateChanged(EngKidAPI.UnitStates new_state)
    {
        if (new_state == m_UnitState)
            return;

        m_UnitState = new_state;
        if (new_state == EngKidAPI.UnitStates.LOCKED)
        {
            m_LockedDisplay.SetActive(true);
            m_UnlockedDisplay.SetActive(false);
        }
        else
        {
            //TODO: add unlocking effects
            if (LeanTween.isTweening(m_LockedDisplay) == true)
                LeanTween.cancel(m_LockedDisplay);
            if (LeanTween.isTweening(m_UnlockedDisplay) == true)
                LeanTween.cancel(m_UnlockedDisplay);

            m_UnlockedDisplay.transform.localScale = new Vector3();
            LeanTween.scale(m_LockedDisplay, new Vector3(), 0.25f).setOnComplete(() => {
                m_LockedDisplay.SetActive(false);
                m_UnlockedDisplay.SetActive(true);
                LeanTween.scale(m_UnlockedDisplay, og_unlocked_scale, 0.25f);
            });

            m_NewlyUnlockedIcon.SetActive(true);
            m_NewlyUnlockedIcon.transform.localScale = new Vector3();
            LeanTween.scale(m_NewlyUnlockedIcon, new Vector3(1.0f, 1.0f, 1.0f), 0.5f).setDelay(0.5f);
        }
    }

    public void OnStarsGained(int amount = 0)
    {
        starAmount += amount;
        m_UnitStarsText.text = starAmount.ToString() + "/" + maxStarsAmount.ToString();
    }

    public void OnUnitClicked()
    {
        if (LeanTween.isTweening(m_LockedDisplay) == true)
            LeanTween.cancel(m_LockedDisplay);
        if (LeanTween.isTweening(m_UnlockedDisplay) == true)
            LeanTween.cancel(m_UnlockedDisplay);

        m_LockedDisplay.transform.localScale = og_locked_scale;
        m_UnlockedDisplay.transform.localScale = og_unlocked_scale;

        Vector3 new_scale = m_LockedDisplay.transform.localScale * 1.1f;
        LeanTween.scale(m_LockedDisplay, new_scale, 0.25f).setLoopPingPong(2);
        LeanTween.scale(m_UnlockedDisplay, new_scale, 0.25f).setLoopPingPong(2);

        if (m_NewlyUnlockedIcon.activeSelf == true)
        {
            LeanTween.scale(m_NewlyUnlockedIcon, new Vector3(), 0.5f).setOnComplete(() => {
                m_NewlyUnlockedIcon.SetActive(false);
            });
        }
    }
}
