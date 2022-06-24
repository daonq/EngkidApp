using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAcivityEventBehavior : MonoBehaviour
{
    public BoardWritingErasingEffects m_BoardWritingErasingEffects;
    public GameObject m_NextEvent;
    public float m_LoadingScreenStartPos = 225.0f;
    public float m_LoadingScreenEndPos = 1225.0f;

    private void OnEnable()
    {
        if (m_NextEvent != null)
            m_NextEvent.SetActive(false);
    }

    public virtual void StartEvent()
    {
        Debug.Log("Game event starts!");
        m_BoardWritingErasingEffects.OnWriting(m_LoadingScreenStartPos, m_LoadingScreenEndPos);
    }

    public virtual void EndEvent()
    {
        m_BoardWritingErasingEffects.OnErasing();
        StartCoroutine(DelayedCallNextEvent());
    }

    IEnumerator DelayedCallNextEvent()
    {
        yield return new WaitForSeconds(m_BoardWritingErasingEffects.m_ErasingDuration);
        NextEvent();
    }

    public void NextEvent()
    {
        if (m_NextEvent != null)
        {
            m_NextEvent.SetActive(true);
            m_NextEvent.GetComponent<BaseAcivityEventBehavior>().StartEvent();
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Next event is null. Make sure this is intentional.");
        }
    }

    public virtual void ProcessAnswer()
    {

    }
}
