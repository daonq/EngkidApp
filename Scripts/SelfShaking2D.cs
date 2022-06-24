using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfShaking2D : MonoBehaviour
{
    [Header("Shake settings")]
    public float m_ShakeValue = 15.0f;
    public int m_ShakeCount = 2;
    public float m_ShakeCooldown = 3.0f;
    public float m_ShakeDuration = 0.1f;

    //internal
    Transform ogTransform;

    private void OnEnable()
    {
        ogTransform = this.transform;
        StartCoroutine(ShakingSequence());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        LeanTween.cancel(this.gameObject);
        this.transform.rotation = ogTransform.rotation;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        LeanTween.cancel(this.gameObject);
        try
        {
            this.transform.rotation = ogTransform.rotation;
        }
        catch(Exception e)
        {

        }
        
    }

    IEnumerator ShakingSequence()
    {
        float left_val = this.transform.rotation.z - m_ShakeValue;
        while (true)
        {
            LeanTween.rotateZ(this.gameObject, left_val, m_ShakeDuration).setLoopPingPong(2);
            yield return new WaitForSeconds(m_ShakeCooldown);
        }
    }
}
