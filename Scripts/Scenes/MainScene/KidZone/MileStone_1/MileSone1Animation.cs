using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileSone1Animation : MonoBehaviour
{
    [Header("Cloud")]
    public GameObject m_Cloud;
    public Vector3 m_CloudSpawnPoint;
    public Vector3 m_CloudPoolPoint;
    public float m_Speed = 0.1f;

    //internal
    Vector3 moveDir = new Vector3();
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        moveDir = (m_CloudPoolPoint - m_CloudSpawnPoint).normalized;
        spriteRenderer = m_Cloud.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        m_Cloud.transform.localPosition += moveDir * m_Speed;
        if (Vector3.Distance(m_Cloud.transform.localPosition, m_CloudPoolPoint) <= 0.01f)
        {
            m_Cloud.transform.localPosition = m_CloudSpawnPoint;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.0f);
        }
        float cloudAlpha = spriteRenderer.color.a;
        if (cloudAlpha < 1.0f)
        {
            cloudAlpha += Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, cloudAlpha);
        }
    }
}
