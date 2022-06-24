using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAM5TutorialEvent : BaseAcivityEventBehavior
{
    public CAM5ActivityBehavior m_ActivityBehavior;
    public GameObject m_HandUnderText;
    public GameObject m_HandPointingAtAnswer;
    public GameObject m_CorrectButtonHighlight;
    public GameObject m_CorrectBtn;
    public GameObject m_IncorrectBtn;
    public Sprite m_CorrectSprite;
    public Sprite m_IncorrectSprite;

    [Header("Audio")]
    public AudioSource m_AudioSource;
    public AudioClip m_TutorialGuide;
    public AudioClip m_TutorialGuideFirstLine;
    public AudioClip m_TutorialGuideSecondLine;
    public AudioClip m_TutorialAnswer;
    public AudioClip m_TutorialCheeringCorrectAnswer;
    public AudioClip m_TryAgainClip;

    //internals
    float idleDuration = 15.0f;
    bool idleCheck = false;
    bool isReady = false;
    Coroutine processAnswerCor = null;
    Coroutine tutorialSequenceCor = null;
    int wrongCounter = 0;

    private void OnEnable()
    {
        m_HandUnderText.SetActive(false);
        m_HandPointingAtAnswer.SetActive(false);
        m_CorrectButtonHighlight.SetActive(false);

        idleCheck = false;
        isReady = false;
        processAnswerCor = null;
    }

    private void OnDisable()
    {
        m_HandUnderText.SetActive(false);
        m_HandPointingAtAnswer.SetActive(false);
        m_CorrectButtonHighlight.SetActive(false);

        StopAllCoroutines();
        LeanTween.cancel(this.gameObject);
    }

    public override void StartEvent()
    {
        base.StartEvent();
        idleDuration = 15.0f;
        wrongCounter = 0;
        idleCheck = false;
        isReady = false;
        tutorialSequenceCor = StartCoroutine(TutorialSequence());
    }

    IEnumerator TutorialSequence()
    {
        m_CorrectBtn.GetComponent<Button>().interactable = false;
        m_IncorrectBtn.GetComponent<Button>().interactable = false;
        isReady = false;
        m_HandUnderText.SetActive(false);
        m_HandPointingAtAnswer.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Stop();
        m_AudioSource.clip = m_TutorialGuideFirstLine;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
        m_ActivityBehavior.teacherCharacter.PlayPointingAnimation(m_TutorialGuideFirstLine.length);

        //yield return new WaitForSeconds(m_TutorialGuide.length + 2.0f);
        Spine.Unity.SkeletonAnimation guidingAnim = m_HandUnderText.GetComponent<Spine.Unity.SkeletonAnimation>();
        Spine.Unity.SkeletonAnimation pushingAnim = m_HandPointingAtAnswer.GetComponent<Spine.Unity.SkeletonAnimation>();
        guidingAnim.ClearState();
        guidingAnim.state.ClearTracks();
        guidingAnim.skeleton.SetToSetupPose();
        pushingAnim.ClearState();
        pushingAnim.ClearState();
        pushingAnim.state.ClearTracks();
        pushingAnim.skeleton.SetToSetupPose();
        guidingAnim.AnimationName = "tab vao buc tranh";
        guidingAnim.state.SetAnimation(0, "tab vao buc tranh", false);
        m_HandUnderText.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        guidingAnim.AnimationName = "ban tay di chuyen chi sang cau mo ta theo duong thang.spine";
        guidingAnim.state.SetAnimation(0, "ban tay di chuyen chi sang cau mo ta theo duong thang.spine", false);
        yield return new WaitForSeconds(3.3f);

        m_AudioSource.Stop();
        m_AudioSource.clip = m_TutorialGuideSecondLine;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
        m_ActivityBehavior.teacherCharacter.PlayPointingAnimation(m_TutorialGuideSecondLine.length);

        m_CorrectBtn.GetComponent<Button>().interactable = true;
        m_IncorrectBtn.GetComponent<Button>().interactable = true;
        m_HandUnderText.SetActive(false);
        m_HandPointingAtAnswer.SetActive(true);
        m_CorrectButtonHighlight.SetActive(true);
        pushingAnim.AnimationName = "animation";
        pushingAnim.state.SetAnimation(0, "animation", false);
        yield return new WaitForSeconds(1.5f);
        m_HandPointingAtAnswer.SetActive(false);

        idleCheck = true;
        isReady = true;
    }

    public void ProcessAnswer(bool answer)
    {
        if (isReady == false)
            return;

        if (tutorialSequenceCor != null)
        {
            StopCoroutine(tutorialSequenceCor);
            idleCheck = true;
            isReady = true;
        }

        if (processAnswerCor != null)
            StopCoroutine(processAnswerCor);

        processAnswerCor = StartCoroutine(ProcessAnswerSequence(answer));
    }

    IEnumerator ProcessAnswerSequence(bool answer)
    {
        isReady = false;
        idleCheck = false;

        m_HandUnderText.SetActive(false);
        m_HandPointingAtAnswer.SetActive(false);
        m_CorrectButtonHighlight.SetActive(false);

        m_ActivityBehavior.m_AudioSource.Stop();

        if (answer == true)
        {
            m_CorrectBtn.GetComponent<Image>().sprite = m_CorrectSprite;

            m_AudioSource.Stop();
            m_AudioSource.clip = m_ActivityBehavior.m_CorrectSFX;
            m_AudioSource.Play();
            yield return new WaitForSeconds(m_ActivityBehavior.m_CorrectSFX.length);

            m_AudioSource.Stop();
            m_AudioSource.clip = m_TutorialAnswer;
            m_AudioSource.Play();
            //correct effect
            yield return new WaitForSeconds(m_TutorialAnswer.length);
            m_AudioSource.Stop();
            m_AudioSource.clip = m_TutorialCheeringCorrectAnswer;
            m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.cheering_delay);
            m_ActivityBehavior.teacherCharacter.PlayCheeringAnimation(m_TutorialCheeringCorrectAnswer.length);
            yield return new WaitForSeconds(m_TutorialCheeringCorrectAnswer.length + 0.5f);

            m_CorrectButtonHighlight.SetActive(false);

            //trigger next event
            base.EndEvent();
        }
        else
        {
            wrongCounter++;

            if (wrongCounter == 1)
            {
                m_CorrectButtonHighlight.SetActive(true);
                m_IncorrectBtn.GetComponent<Image>().sprite = m_IncorrectSprite;

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length);

                m_AudioSource.Stop();
                m_AudioSource.clip = m_TryAgainClip;
                m_AudioSource.Play();
                m_ActivityBehavior.teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);
                yield return new WaitForSeconds(m_AudioSource.clip.length);

                isReady = true;
            }
            else if (wrongCounter == 2)
            {
                m_CorrectButtonHighlight.SetActive(true);
                m_IncorrectBtn.GetComponent<Image>().sprite = m_IncorrectSprite;

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length);

                m_AudioSource.Stop();
                m_AudioSource.clip = m_TryAgainClip;
                m_AudioSource.Play();
                m_ActivityBehavior.teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);
                yield return new WaitForSeconds(m_AudioSource.clip.length);

                isReady = true;
            }
            else if (wrongCounter == 3)
            {
                m_CorrectBtn.GetComponent<Image>().sprite = m_CorrectSprite;
                m_CorrectButtonHighlight.SetActive(true);
                m_IncorrectBtn.GetComponent<Image>().sprite = m_IncorrectSprite;

                m_AudioSource.Stop();
                m_AudioSource.clip = m_ActivityBehavior.m_WrongSFX;
                m_AudioSource.Play();
                yield return new WaitForSeconds(m_ActivityBehavior.m_WrongSFX.length);

                m_AudioSource.Stop();
                m_ActivityBehavior.PlayWrongAnswerAudio();
                yield return new WaitForSeconds(m_TutorialCheeringCorrectAnswer.length + 0.5f);

                m_CorrectButtonHighlight.SetActive(false);

                //trigger next event
                base.EndEvent();
            }
        }
    }

    private void Update()
    {
        if (idleCheck == true)
        {
            idleDuration -= Time.deltaTime;
            if (idleDuration <= 0.0f)
            {
                idleDuration = 15.0f + m_TutorialGuide.length;
                m_ActivityBehavior.PlayIdleAudio();
            }
        }
    }
}
