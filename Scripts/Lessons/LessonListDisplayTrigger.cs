using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonListDisplayTrigger : MonoBehaviour
{
    public LessonChoosingWindowBehavior m_LessonChoosing;
    public GameObject m_DefaultLesson;

    private void Start()
    {
        //m_LessonChoosing.UpdateLessonDisplays(m_DefaultLesson);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            m_LessonChoosing.UpdateLessonDisplays(other.gameObject);
        }
    }
}
