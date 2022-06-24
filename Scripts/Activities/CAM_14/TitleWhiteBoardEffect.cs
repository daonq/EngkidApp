using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleWhiteBoardEffect : MonoBehaviour
{
    [Header("Title vfx")]
    public GameObject m_TitleImg;
    public GameObject m_TitleMask;
    public GameObject m_WritingHand;
    public GameObject m_ErasingHand;
    public GameObject m_Eraser;

    [Header("Audio fx")]
    public AudioSource m_AudioSource;
    public AudioClip m_Writing;
    public AudioClip m_Erasing;

    [Header("First event")]
    public GameObject m_FirstEvent;

    private void OnEnable()
    {
        StartTitleVFX();

        if (m_FirstEvent != null)
            m_FirstEvent.SetActive(false);
    }

    public void StartTitleVFX()
    {
        m_AudioSource.clip = m_Writing;
        m_AudioSource.PlayDelayed(1.0f);

        if (m_FirstEvent != null)
            m_FirstEvent.SetActive(false);

        float og_pos = m_TitleMask.transform.position.x;
        float new_pos = og_pos + m_TitleMask.GetComponent<RectTransform>().sizeDelta.x;
        LeanTween.moveX(m_TitleMask, new_pos, 4.0f).setDelay(1.0f).setOnComplete(() => {
            m_AudioSource.Stop();
            LeanTween.moveX(m_TitleMask, og_pos, 4.0f).setDelay(5.0f).setOnStart(() => {
                m_AudioSource.clip = m_Erasing;
                m_AudioSource.Play();
            }).setOnComplete(() => {
                m_AudioSource.Stop();
            });
        });

        Transform erasing_hand_og_parent = m_ErasingHand.transform.parent;
        LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.0f);
        LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.0f);
        LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 1.0f, 1.0f).setOnComplete(() => {
            LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 1.0f).setDelay(4.0f).setOnComplete(() => {
                LeanTween.alpha(m_Eraser.GetComponent<RectTransform>(), 0.0f, 0.25f).setDelay(2.0f);
                LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 1.0f, 0.25f).setDelay(2.0f).setOnComplete(() => {
                    LeanTween.move(m_ErasingHand, m_WritingHand.transform.position - new Vector3(100.0f, 0.0f, 0.0f), 1.0f).setOnComplete(() => {
                        m_ErasingHand.transform.parent = m_TitleMask.transform;
                    });
                });

                LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.25f).setDelay(8.0f).setOnComplete(() => {
                    m_ErasingHand.transform.parent = erasing_hand_og_parent;

                    //call first event
                    if (m_FirstEvent != null)
                    {
                        m_FirstEvent.SetActive(true);

                        if (m_FirstEvent.GetComponent<Cam14GameEventBehavior>() != null)
                            m_FirstEvent.GetComponent<Cam14GameEventBehavior>().StartGameEvent();
                    }
                });
                LeanTween.alpha(m_Eraser.GetComponent<RectTransform>(), 1.0f, 0.25f).setDelay(8.0f);
            });
        });
    }

    public void LoadEventVfx(GameObject new_event)
    {
        m_AudioSource.clip = m_Writing;
        m_AudioSource.PlayDelayed(1.0f);

        if (new_event != null)
            new_event.SetActive(false);

        float og_pos = m_TitleMask.transform.position.x;
        float new_pos = og_pos + m_TitleMask.GetComponent<RectTransform>().sizeDelta.x;
        LeanTween.moveX(m_TitleMask, new_pos, 4.0f).setDelay(1.0f).setOnComplete(() => {
            m_AudioSource.Stop();
            LeanTween.moveX(m_TitleMask, og_pos, 4.0f).setDelay(5.0f).setOnStart(() => {
                m_AudioSource.clip = m_Erasing;
                m_AudioSource.Play();
            }).setOnComplete(() => {
                m_AudioSource.Stop();
            });
        });

        Transform erasing_hand_og_parent = m_ErasingHand.transform.parent;
        LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.0f);
        LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.0f);
        LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 1.0f, 1.0f).setOnComplete(() => {
            LeanTween.alpha(m_WritingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 1.0f).setDelay(4.0f).setOnComplete(() => {
                LeanTween.alpha(m_Eraser.GetComponent<RectTransform>(), 0.0f, 0.25f).setDelay(2.0f);
                LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 1.0f, 0.25f).setDelay(2.0f).setOnComplete(() => {
                    LeanTween.move(m_ErasingHand, m_WritingHand.transform.position - new Vector3(100.0f, 0.0f, 0.0f), 1.0f).setOnComplete(() => {
                        m_ErasingHand.transform.parent = m_TitleMask.transform;
                    });
                });

                LeanTween.alpha(m_ErasingHand.transform.GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.25f).setDelay(8.0f).setOnComplete(() => {
                    m_ErasingHand.transform.parent = erasing_hand_og_parent;

                    //call first event
                    if (new_event != null)
                    {
                        new_event.SetActive(true);

                        if (new_event.GetComponent<Cam14GameEventBehavior>() != null)
                            new_event.GetComponent<Cam14GameEventBehavior>().StartGameEvent();
                    }
                });
                LeanTween.alpha(m_Eraser.GetComponent<RectTransform>(), 1.0f, 0.25f).setDelay(8.0f);
            });
        });
    }    

    private void OnDisable()
    {

    }
}
