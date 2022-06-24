using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfBlinking : MonoBehaviour
{
    public Image m_Img;
    public float m_FadeSpeed = 0.25f;

    float multiplier = 1.0f;

    private void Start()
    {
        if (m_Img == null)
            m_Img = this.GetComponent<Image>();
    }

    private void Update()
    {
        m_Img.color = new Color(m_Img.color.r, m_Img.color.g, m_Img.color.b, m_Img.color.a - (Time.deltaTime * multiplier * m_FadeSpeed));

        if (m_Img.color.a <= 0.0f)
        {
            multiplier = -1.0f;
        }    
        else if (m_Img.color.a >= 1.0f)
        {
            multiplier = 1.0f;
        }    
    }
}
