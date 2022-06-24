using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAM5EventBehavior : BaseAcivityEventBehavior
{
    public CAM5ActivityBehavior m_ActivityBehavior;
    public GameObject m_YesBtn;
    public GameObject m_NoBtn;
    public Sprite m_DefaultSprite;
    public Sprite m_CorrectSprite;
    public Sprite m_IncorrectSprite;
    public bool m_IsCorrectAnswerIsYes = true;
    public bool m_IsFirstEvent = false;
    public bool m_IsLastEvent = false;

    [Header("Audio")]
    public AudioSource m_AudioSource;
    public AudioClip m_AnswerAudio;
    public AudioClip m_TryAgainClip;

    //internals
    float idleDuration = 15.0f;
    bool idleCheck = false;
    Coroutine processAnswerCor = null;
    bool isReady = false;
    int wrongCounter = 0;

    private void OnEnable()
    {
        idleCheck = false;
        m_YesBtn.SetActive(false);
        m_NoBtn.SetActive(false);
        m_YesBtn.GetComponent<Button>().interactable = false;
        m_NoBtn.GetComponent<Button>().interactable = false;
        m_YesBtn.transform.GetChild(1).gameObject.SetActive(false);
        m_NoBtn.transform.GetChild(1).gameObject.SetActive(false);
        m_YesBtn.GetComponent<Image>().sprite = m_DefaultSprite;
        m_NoBtn.GetComponent<Image>().sprite = m_DefaultSprite;
        m_YesBtn.GetComponent<Button>().interactable = true;
        m_NoBtn.GetComponent<Button>().interactable = true;
        processAnswerCor = null;
        wrongCounter = 0;
    }

    private void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
        StopAllCoroutines();
    }

    public override void StartEvent()
    {
        base.StartEvent();
        idleCheck = true;
        wrongCounter = 0;
        idleCheck = false;
        isReady = true;

        if (m_IsFirstEvent)
        {
            firstEventCor = StartCoroutine(FirstEventSequence());
        }

        StartCoroutine(DelayedEnableButtons());
    }

    IEnumerator DelayedEnableButtons()
    {
        yield return new WaitForSeconds(2.5f);
        m_YesBtn.GetComponent<Button>().interactable = true;
        m_NoBtn.GetComponent<Button>().interactable = true;
        yield return new WaitForSeconds(0.01f);
        m_YesBtn.SetActive(true);
        m_NoBtn.SetActive(true);
    }

    Coroutine firstEventCor = null;
    IEnumerator FirstEventSequence()
    {
        m_ActivityBehavior.PlayIdleAudio();
        idleDuration = 15.0f + m_ActivityBehavior.m_ActivityGuide.length;
        yield return new WaitForSeconds(m_ActivityBehavior.m_ActivityGuide.length);
    }

    public void ProcessAnswer(bool answer)
    {
        if (isReady == false)
            return;

        if (processAnswerCor != null)
            return;

        processAnswerCor = StartCoroutine(ProcessAnswerSequence(answer));
    }

    IEnumerator ProcessAnswerSequence(bool answer)
    {
        if (firstEventCor != null)
        {
            StopCoroutine(firstEventCor);
            m_AudioSource.Stop();
            m_ActivityBehavior.m_AudioSource.Stop();
        }    

        isReady = false;
        idleCheck = false;

        m_ActivityBehavior.m_AudioSource.Stop();

        if (answer == m_IsCorrectAnswerIsYes)
        {
            if (answer)
            {
                m_YesBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                m_YesBtn.transform.GetChild(1).gameObject.SetActive(true);
                m_NoBtn.GetComponent<Button>().interactable = false;
            }
            else
            {
                //m_YesBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                m_NoBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                m_NoBtn.transform.GetChild(1).gameObject.SetActive(true);
                m_YesBtn.GetComponent<Button>().interactable = false;
            }

            m_AudioSource.Stop();
            m_AudioSource.loop = false;
            m_AudioSource.clip = m_ActivityBehavior.m_CorrectSFX;
            m_AudioSource.Play();
            yield return new WaitForSeconds(m_AudioSource.clip.length);

            m_AudioSource.Stop();
            m_AudioSource.clip = m_AnswerAudio;
            m_AudioSource.Play();
            yield return new WaitForSeconds(m_AnswerAudio.length);

            //m_ActivityBehavior.PlayCheeringAnimation();
            m_ActivityBehavior.PlayCorrectAnswerAudio();
            int score = 0;
            if (wrongCounter == 0)
                score = 3;
            else if (wrongCounter == 1)
                score = 1;
            else
                score = 0;
            m_ActivityBehavior.IncreaseScore(score);
            yield return new WaitForSeconds(1.75f);

            //trigger next event
            if (m_NextEvent != null)
                base.EndEvent();
            else
            {
                //trigger result popup
                if (m_IsLastEvent)
                {
                    m_ActivityBehavior.ShowResultPopUp();
                }
            }
        }
        else
        {
            wrongCounter++;

            if (wrongCounter == 1)
            {
                if (answer)
                {
                    m_YesBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    //m_NoBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                    //m_NoBtn.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    m_NoBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    //m_YesBtn.transform.GetChild(1).gameObject.SetActive(true);
                }

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length - 1.5f);

                m_AudioSource.Stop();
                m_AudioSource.clip = m_TryAgainClip;
                m_AudioSource.Play();
                m_ActivityBehavior.teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);
                yield return new WaitForSeconds(m_AudioSource.clip.length);

                isReady = true;
            }
            else if (wrongCounter == 2)
            {
                if (answer)
                {
                    m_YesBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    //m_NoBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                    //m_NoBtn.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    m_NoBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    //m_YesBtn.transform.GetChild(1).gameObject.SetActive(true);
                }

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length - 1.5f);

                m_AudioSource.Stop();
                m_AudioSource.clip = m_TryAgainClip;
                m_AudioSource.Play();
                m_ActivityBehavior.teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);
                yield return new WaitForSeconds(m_AudioSource.clip.length);

                isReady = true;
            }
            else if (wrongCounter == 3)
            {
                if (answer)
                {
                    m_YesBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    m_YesBtn.GetComponent<Button>().interactable = false;
                    m_YesBtn.transform.GetChild(1).gameObject.SetActive(false);
                    m_NoBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                    m_NoBtn.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    m_NoBtn.GetComponent<Image>().sprite = m_IncorrectSprite;
                    m_NoBtn.GetComponent<Button>().interactable = false;
                    m_NoBtn.transform.GetChild(1).gameObject.SetActive(false);
                    m_YesBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                    m_YesBtn.transform.GetChild(1).gameObject.SetActive(true);
                }

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length - 1.5f);

                m_AudioSource.Stop();
                m_ActivityBehavior.PlayWrongAnswerAudio();
                yield return new WaitForSeconds(1.75f);

                //trigger next event
                if (m_NextEvent != null)
                    base.EndEvent();
                else
                {
                    //trigger result popup
                    if (m_IsLastEvent)
                    {
                        m_ActivityBehavior.ShowResultPopUp();
                    }
                }    
            }
        }

        processAnswerCor = null;
    }

    private void Update()
    {
        if (idleCheck == true)
        {
            idleDuration -= Time.deltaTime;
            if (idleDuration <= 0.0f)
            {
                idleDuration = 15.0f + m_ActivityBehavior.m_ActivityGuide.length;
                m_ActivityBehavior.PlayIdleAudio();
            }
        }
    }
}
