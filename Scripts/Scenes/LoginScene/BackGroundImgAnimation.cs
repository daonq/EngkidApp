using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundImgAnimation : MonoBehaviour
{
    [Header("Rainbow Mask")]
    public GameObject m_RainbowMask;

    [Header("Avatars")]
    public Image m_BoiImg;
    public Sprite m_BoiNormal;
    public Sprite m_BoiWinks;
    public Image m_GirlImg;
    public Sprite m_GirlNormal;
    public Sprite m_GirlWinks;

    private void OnEnable()
    {
        m_RainbowMask.SetActive(true);
        m_RainbowMask.transform.localPosition = new Vector3();
        Vector3 new_mask_pos = m_RainbowMask.transform.localPosition + new Vector3(1000.0f, 0.0f, 0.0f);
        LeanTween.moveLocal(m_RainbowMask, new_mask_pos, 1.5f).setOnComplete(() => {
            m_RainbowMask.SetActive(false);
        });

        StartCoroutine(AvatarWinking());
    }

    IEnumerator AvatarWinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            m_BoiImg.sprite = m_BoiWinks;
            m_GirlImg.sprite = m_GirlWinks;
            m_BoiImg.SetNativeSize();
            m_GirlImg.SetNativeSize();
            yield return new WaitForSeconds(1.0f);
            m_BoiImg.sprite = m_BoiNormal;
            m_GirlImg.sprite = m_GirlNormal;
            m_BoiImg.SetNativeSize();
            m_GirlImg.SetNativeSize();
        }
    }

    private void OnDisable()
    {
        LeanTween.cancel(m_RainbowMask);
        StopAllCoroutines();
    }
}
