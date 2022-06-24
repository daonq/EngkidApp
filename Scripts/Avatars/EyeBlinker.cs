using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlinker : MonoBehaviour
{
    public SpriteRenderer m_RightEye;
    public SpriteRenderer m_LeftEye;

    public Sprite m_RightEyeOpen;
    public Sprite m_LeftEyeOpen;

    public Sprite m_RightEyeClose;
    public Sprite m_LeftEyeClose;

    private void Start()
    {
        StartCoroutine(Delayedblinking());
    }

    IEnumerator Delayedblinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
            m_RightEye.sprite = m_RightEyeClose;
            m_LeftEye.sprite = m_LeftEyeClose;
            yield return new WaitForSeconds(0.5f);
            m_RightEye.sprite = m_RightEyeOpen;
            m_LeftEye.sprite = m_LeftEyeOpen;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
