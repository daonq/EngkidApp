using UnityEngine;
using UnityEngine.UI;

public class KidZoneWelcomePopUp : MonoBehaviour
{
    [Header("UI Animation")]
    public Image m_BlackBG;
    public GameObject m_WelcomePanel;
    public GameObject m_CloudL;
    public GameObject m_CloudR;
    public GameObject m_Boi;
    public GameObject m_Girl;

    [Header("Animation Settings")]
    public float m_FadeInDuration = 0.5f;
    public float m_FadeOutDuration = 0.5f;
    public float m_StayDuration = 0.5f;

    //internal

    // Start is called before the first frame update
    void Start()
    {
        Vector3 cloud_left_pos = m_CloudL.transform.localPosition;
        Vector3 cloud_right_pos = m_CloudR.transform.localPosition;
        Vector3 boi_pos = m_Boi.transform.localPosition;
        Vector3 girl_pos = m_Girl.transform.localPosition;

        Vector3 cloudLOutPos = new Vector3(-Screen.width - m_CloudL.GetComponent<RectTransform>().sizeDelta.x, 0.0f, 0.0f);
        Vector3 cloudROutPos = new Vector3(Screen.width + m_CloudR.GetComponent<RectTransform>().sizeDelta.x, 0.0f, 0.0f);
        Vector3 boiOutPos = new Vector3(0.0f, -Screen.height - m_Boi.GetComponent<RectTransform>().sizeDelta.y, 0.0f);
        Vector3 girlOutPos = new Vector3(0.0f, -Screen.height - m_Girl.GetComponent<RectTransform>().sizeDelta.y, 0.0f);

        //init anim
        m_WelcomePanel.transform.localPosition += new Vector3(0.0f, Screen.height + m_WelcomePanel.GetComponent<RectTransform>().sizeDelta.y, 0.0f);
        m_CloudL.transform.localPosition += cloudLOutPos;
        m_CloudR.transform.localPosition += cloudROutPos;
        m_Boi.transform.localPosition += boiOutPos;
        m_Girl.transform.localPosition += girlOutPos;

        //execute anim
        LeanTween.moveLocal(m_WelcomePanel, Vector3.zero, m_FadeInDuration).setDelay(0.5f);
        LeanTween.moveLocal(m_CloudL, cloud_left_pos, m_FadeInDuration).setDelay(0.5f);
        LeanTween.moveLocal(m_CloudR, cloud_right_pos, m_FadeInDuration).setDelay(0.5f);
        LeanTween.moveLocal(m_Boi, boi_pos, m_FadeInDuration).setDelay(0.5f);
        LeanTween.moveLocal(m_Girl, girl_pos, m_FadeInDuration).setDelay(0.5f);

        float fade_out_delay = 0.5f + m_FadeInDuration + m_StayDuration;
        LeanTween.moveLocal(m_WelcomePanel, new Vector3(0.0f, Screen.height + m_WelcomePanel.GetComponent<RectTransform>().sizeDelta.y, 0.0f), m_FadeOutDuration).setDelay(fade_out_delay);
        LeanTween.moveLocal(m_CloudL, new Vector3(-Screen.width - m_CloudL.GetComponent<RectTransform>().sizeDelta.x, 0.0f, 0.0f), m_FadeOutDuration).setDelay(fade_out_delay);
        LeanTween.moveLocal(m_CloudR, new Vector3(Screen.width + m_CloudR.GetComponent<RectTransform>().sizeDelta.x, 0.0f, 0.0f), m_FadeOutDuration).setDelay(fade_out_delay);
        LeanTween.moveLocal(m_Boi, new Vector3(0.0f, -Screen.height - m_Boi.GetComponent<RectTransform>().sizeDelta.y, 0.0f), m_FadeOutDuration).setDelay(fade_out_delay);
        LeanTween.moveLocal(m_Girl, new Vector3(0.0f, -Screen.height - m_Girl.GetComponent<RectTransform>().sizeDelta.y, 0.0f), m_FadeOutDuration).setDelay(fade_out_delay).setOnComplete(() => {
            float start_alpha = m_BlackBG.color.a;
            LeanTween.value(m_BlackBG.gameObject, start_alpha, 0.0f, 0.5f).setOnUpdate((float val) =>
            {
                Color color = m_BlackBG.color;
                color.a = val;
                m_BlackBG.color = color;
            }).setOnComplete(() => {
                this.gameObject.SetActive(false);
            });
        });
    }
}
