using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cam14ActivityBehavior : MonoBehaviour
{
    [Header("Debugging")]
    public bool m_IsDeveloping = true;
    public GameObject m_ConnectionErrorPopup;

    [Header("Audio")]
    public AudioSource m_AudioSource;
    public AudioClip m_OpeningClip;
    public AudioSource m_BGMAudioSource;
    public Sprite m_BGMPlaying;
    public Sprite m_BGMStoped;
    public Image m_BGMButton;

    public List<AudioClip> m_ChooseAnAnswers = new List<AudioClip>();
    public List<AudioClip> m_LetsListens = new List<AudioClip>();
    public AudioClip m_LetsSayItAudio;
    public AudioClip m_WrongSFX;
    public List<AudioClip> m_WrongAnswers = new List<AudioClip>();
    public AudioClip m_CorrectSFX;
    public List<AudioClip> m_CorrectAnswers = new List<AudioClip>();

    public AudioClip m_IdleSingleWord;
    public AudioClip m_IdleMultiWords;

    public AudioClip m_TeacherTapped;
    public bool m_TeacherReady = false;

    [Header("Legacy Animation")]
    public GameObject m_TalkingOpening;
    public GameObject m_Cheering;
    public GameObject m_Consoling;
    public GameObject m_TalkingOnly;
    public GameObject m_PointingAtBoard;

    [Header("New teacher character animation")]
    public TeacherCharacterAnimationController teacherCharacter;

    [Header("Score")]
    public int m_Score = 0;
    public int m_MaxScore = 5;

    [Header("First event")]
    public BaseAcivityEventBehavior m_FirstEvent;

    [Header("Navigation")]
    public GameObject m_LastScene;

    public void IncreaseScore()
    {
        m_Score++;

        if (m_Score > m_MaxScore)
            m_Score = m_MaxScore;
        else if (m_Score < 0)
            m_Score = 0;
    }

    public void OnReplay()
    {
        m_Score = 0;
    }

    private void OnEnable()
    {
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().PauseBGM();

        if (AuthorizationManager.GetInstance() != null)
            AuthorizationManager.GetInstance().TriggerMicroPopUp();

        m_ConnectionErrorPopup.SetActive(false);

        StartActivity();
    }

    public void StartActivity()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_OpeningClip;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.opening_delay);
        teacherCharacter.PlayOpeningAnimation(m_OpeningClip.length);

        StartCoroutine(StarFirstEventSequence(m_OpeningClip.length));
    }

    IEnumerator StarFirstEventSequence(float dur)
    {
        yield return new WaitForSeconds(dur);
        m_FirstEvent.gameObject.SetActive(true);
        m_FirstEvent.StartEvent();
    }

    public void ToggleBGM()
    {
        if (m_BGMAudioSource.isPlaying)
        {
            m_BGMAudioSource.Stop();
            m_BGMButton.sprite = m_BGMStoped;
        }
        else
        {
            m_BGMAudioSource.Play();
            m_BGMButton.sprite = m_BGMPlaying;
        }

    }

    private void OnDisable()
    {
        m_AudioSource.Stop();
        BGMManagerBehavior.GetInstance().ResumeBGM();
    }

    public void BackToUnit()
    {
        SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
    }

    public void BackToLastScene()
    {
        SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
    }

    public void PlayAudio(AudioClip clip, float delay = 0.0f)
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = clip;
        m_AudioSource.PlayDelayed(delay);
    }

    public void PlayAudio(AudioClip clip)
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }

    public void StopAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
    }

    public void PlayAudioLoop(AudioClip clip, float delay = 0.0f)
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = true;
        m_AudioSource.clip = clip;
        m_AudioSource.PlayDelayed(delay);
    }

    public float PlayChooseAnAnswerAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_ChooseAnAnswers[Random.Range(0, m_ChooseAnAnswers.Count)];
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
        teacherCharacter.PlayPointingAnimation(m_AudioSource.clip.length);

        //m_TalkingOpening.SetActive(true);
        //m_Cheering.SetActive(false);
        //m_Consoling.SetActive(false);
        //m_TalkingOnly.SetActive(false);
        //m_PointingAtBoard.SetActive(false);

        //m_TalkingOpening.GetComponent<SpineAnimationController>().TriggerDefaultANimation();

        return m_AudioSource.clip.length;
    }

    public void PlayIdleAudio(bool is_single_word = true)
    {
        m_TeacherReady = false;
        if (is_single_word)
        {
            m_AudioSource.Stop();
            m_AudioSource.loop = false;
            m_AudioSource.clip = m_IdleSingleWord;
            m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
            teacherCharacter.PlayPointingAnimation(m_AudioSource.clip.length);
        }
        else
        {
            m_AudioSource.Stop();
            m_AudioSource.loop = false;
            m_AudioSource.clip = m_IdleMultiWords;
            m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
            teacherCharacter.PlayPointingAnimation(m_AudioSource.clip.length);
        }

        StartCoroutine(ResetTeacherReady(m_AudioSource.clip.length));
    }

    IEnumerator ResetTeacherReady(float dur)
    {
        yield return new WaitForSeconds(dur);
        m_TeacherReady = true;
    }

    public void PlayLetsListenAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_LetsListens[Random.Range(0, m_LetsListens.Count)];
        m_AudioSource.Play();
        teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);

        //m_TalkingOpening.SetActive(false);
        //m_Cheering.SetActive(false);
        //m_Consoling.SetActive(false);
        //m_TalkingOnly.SetActive(true);
        //m_PointingAtBoard.SetActive(false);

        //m_TalkingOnly.GetComponent<SpineAnimationController>().TriggerDefaultANimation();
    }

    public void PlayLetsSayItAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_LetsSayItAudio;
        m_AudioSource.Play();
        teacherCharacter.PlayTalkingAnimation(m_AudioSource.clip.length);

        //m_TalkingOpening.SetActive(false);
        //m_Cheering.SetActive(false);
        //m_Consoling.SetActive(false);
        //m_TalkingOnly.SetActive(true);
        //m_PointingAtBoard.SetActive(false);

        //m_TalkingOnly.GetComponent<SpineAnimationController>().TriggerDefaultANimation();
    }

    public void PlayWrongAnswerSFX()
    {
        StartCoroutine(WrongAnswerSFX());
    }

    IEnumerator WrongAnswerSFX()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_WrongSFX;
        m_AudioSource.Play();
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_WrongAnswers[Random.Range(0, m_WrongAnswers.Count)];
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.consoling_delay);
        teacherCharacter.PlayConsolingAnimation(m_AudioSource.clip.length);

        //m_TalkingOpening.SetActive(false);
        //m_Cheering.SetActive(false);
        //m_Consoling.SetActive(true);
        //m_TalkingOnly.SetActive(false);
        //m_PointingAtBoard.SetActive(false);

        //m_Consoling.GetComponent<SpineAnimationController>().TriggerDefaultANimation();
    }

    public void PlayCorrectAnswerSFX()
    {
        StartCoroutine(CorrectAnswerSFX());
    }

    IEnumerator CorrectAnswerSFX()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_CorrectSFX;
        m_AudioSource.Play();
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_CorrectAnswers[Random.Range(0, m_CorrectAnswers.Count)];
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.cheering_delay);
        teacherCharacter.PlayCheeringAnimation(m_AudioSource.clip.length);

        //m_TalkingOpening.SetActive(false);
        //m_Cheering.SetActive(true);
        //m_Consoling.SetActive(false);
        //m_TalkingOnly.SetActive(false);
        //m_PointingAtBoard.SetActive(false);

        //m_Cheering.GetComponent<SpineAnimationController>().TriggerDefaultANimation();
    }

    public void OnTeacherTapped()
    {
        if (m_TeacherReady == false)
            return;

        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_TeacherTapped;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);
        teacherCharacter.PlayPointingAnimation(m_AudioSource.clip.length);
    }

    public void ShowConnectionError(string error_text)
    {
        m_ConnectionErrorPopup.SetActive(true);
        m_ConnectionErrorPopup.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<b>Lỗi kết nối</b>: " + error_text + ". Hãy mở lại hoạt động.";
    }

    public void OnConnectionErrorClose()
    {
        if (m_IsDeveloping == false)
            BackToLastScene();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
