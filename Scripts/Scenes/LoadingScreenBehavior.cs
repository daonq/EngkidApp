using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenBehavior : MonoBehaviour
{
    [Header("Loading Image")]
    public Image m_LoadingImage;
    public float m_CurrentImgAlpha;

    // Start is called before the first frame update
    void Start()
    {
        m_LoadingImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Color color = m_LoadingImage.color;
        m_CurrentImgAlpha = color.a;
    }

    private void Update()
    {
        if (m_LoadingImage.color.a <= 0.0f)
            m_LoadingImage.raycastTarget = false;
        else
            m_LoadingImage.raycastTarget = true;

        Color color = m_LoadingImage.color;
        m_CurrentImgAlpha = color.a;
    }

    //fade the alpha chanel of the loading image
    public void FadeLoadingImageAlpha(float start_alpha, float end_alpha, float duration)
    {
        LeanTween.cancel(m_LoadingImage.gameObject);
        LeanTween.value(m_LoadingImage.gameObject, start_alpha, end_alpha, duration).setOnUpdate((float val) =>
        {
            Color color = m_LoadingImage.color;
            color.a = val;
            m_LoadingImage.color = color;
        }).setOnComplete(() => {
            Color color = m_LoadingImage.color;
            color.a = end_alpha;
            m_LoadingImage.color = color;
        });
    }
}
