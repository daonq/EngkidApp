using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam14TutorialBehavior : MonoBehaviour
{
    public GameObject m_Hand;
    public List<Transform> m_PivotsList = new List<Transform>();
    public List<GameObject> m_HighLightsList = new List<GameObject>();

    //internals
    Cam14GameEventBehavior eventBehavior;

    private void OnEnable()
    {
        LeanTween.alpha(m_Hand.GetComponent<RectTransform>(), 0.0f, 0.0f);

        foreach (GameObject go in m_HighLightsList)
        {
            go.SetActive(false);
        }
    }

    public void StartTutorial(Cam14GameEventBehavior event_caller, float delay)
    {
        eventBehavior = event_caller;
        StartCoroutine(TutorialSequence(delay));
    }

    IEnumerator TutorialSequence(float delay)
    {
        eventBehavior.m_ActivityBehavior.m_TeacherReady = false;
        yield return new WaitForSeconds(delay + 1.0f);
        LeanTween.alpha(m_Hand.GetComponent<RectTransform>(), 1.0f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        LeanTween.move(m_Hand, m_PivotsList[0], 0.5f).setOnComplete(() => {
            m_HighLightsList[0].SetActive(true);
        });
        yield return new WaitForSeconds(1.5f);
        m_HighLightsList[0].SetActive(false);
        LeanTween.move(m_Hand, m_PivotsList[1], 0.5f).setOnComplete(() => {
            m_HighLightsList[1].SetActive(true);
        });
        yield return new WaitForSeconds(1.5f);
        m_HighLightsList[1].SetActive(false);
        LeanTween.move(m_Hand, m_PivotsList[2], 0.5f).setOnComplete(() => {
            m_HighLightsList[2].SetActive(true);
        });
        yield return new WaitForSeconds(1.5f);
        m_HighLightsList[2].SetActive(false);
        LeanTween.alpha(m_Hand.GetComponent<RectTransform>(), 0.0f, 0.25f).setOnComplete(() => {
            eventBehavior.m_ActivityBehavior.m_TeacherReady = true;
            this.gameObject.SetActive(false);
        });
    }
}
