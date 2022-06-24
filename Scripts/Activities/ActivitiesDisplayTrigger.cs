using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitiesDisplayTrigger : MonoBehaviour
{
    public ActivityChoosingWindowBehavior m_ActivityChoosingWindow;
    public GameObject m_DefaultActivity;

    private void Start()
    {
        //m_LessonChoosing.UpdateLessonDisplays(m_DefaultLesson);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            m_ActivityChoosingWindow.UpdateLessonDisplays(other.gameObject);
        }
    }
}
