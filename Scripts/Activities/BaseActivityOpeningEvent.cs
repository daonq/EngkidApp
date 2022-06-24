using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActivityOpeningEvent : BaseAcivityEventBehavior
{
    public float start_delay = 10.0f;
    public GameObject tutorial;

    public override void StartEvent()
    {
        //base.StartEvent();
        StartCoroutine(OpeningEventSequence());
    }

    IEnumerator OpeningEventSequence()
    {
        yield return new WaitForSeconds(start_delay);
        if (tutorial != null)
        {
            tutorial.SetActive(true);
            while (tutorial.activeSelf == true)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.25f);
        }
        
        base.EndEvent();
        yield return new WaitForSeconds(base.m_BoardWritingErasingEffects.m_ErasingDuration + 1.0f);
        base.NextEvent();
    }
}
