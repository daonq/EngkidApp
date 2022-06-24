using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAM5ActivityBehavior : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource m_AudioSource;
    public AudioClip m_OpeningClip;
    public AudioClip m_TeacherTap;
    public AudioClip m_WrongSFX;
    public List<AudioClip> m_WrongAnswers = new List<AudioClip>();
    public AudioClip m_CorrectSFX;
    public List<AudioClip> m_CorrectAnswers = new List<AudioClip>();
    public AudioClip m_ActivityGuide;

    [Header("Legacy Animation")]
    public GameObject m_TalkingOpening;
    public GameObject m_Cheering;
    public GameObject m_Consoling;
    public GameObject m_TalkingOnly;
    public GameObject m_PointingAtBoard;

    [Header("New teacher animation")]
    public TeacherCharacterAnimationController teacherCharacter;

    [Header("Score")]
    public int m_Score = 0;
    public int m_PartScore = 0;
    public int m_MaxScore = 5;
    public GameObject m_ResultPopup;

    [Header("Navigation")]
    public GameObject m_LastScene;

    [Header("Game events settings")]
    public BaseAcivityEventBehavior m_FirstEvent;

    [Header("BGM")]
    public AudioSource m_BGMSource;
    public Sprite m_BGMOn;
    public Sprite m_BGMOff;
    public Image m_BGMImg;

    //intenals
    List<int> partScores = new List<int>();

    private void OnEnable()
    {
        StartCoroutine(DelayedEnableSequence());

        if (m_BGMSource.isPlaying)
            m_BGMImg.sprite = m_BGMOn;
        else
            m_BGMImg.sprite = m_BGMOff;
    }

    IEnumerator DelayedEnableSequence()
    {
        yield return null;
        yield return null;
        yield return null;
        if (BGMManagerBehavior.GetInstance() != null)
            BGMManagerBehavior.GetInstance().PauseBGM();
        m_AudioSource.loop = false;

        m_ResultPopup.SetActive(false);
        m_Score = 0;
        m_PartScore = 0;
        partScores.Clear();

        m_TalkingOpening.SetActive(false);
        m_Cheering.SetActive(false);
        m_Consoling.SetActive(false);
        m_TalkingOnly.SetActive(false);
        m_PointingAtBoard.SetActive(false);

        StartMiniGame();
    }

    public void OnReplay()
    {
        m_Score = 0;
        m_PartScore = 0;
        partScores.Clear();
    }

    public void ToggleGameObject(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }    

    public void StartMiniGame()
    {
        m_FirstEvent.StartEvent();

        teacherCharacter.PlayOpeningAnimation(m_OpeningClip.length);
        m_AudioSource.Stop();
        m_AudioSource.clip = m_OpeningClip;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.opening_delay);
    }

    private void OnDisable()
    {
        m_AudioSource.Stop();
        BGMManagerBehavior.GetInstance().ResumeBGM();

        m_ResultPopup.SetActive(false);
    }

    public void IncreaseScore(int new_score)
    {
        partScores.Add(new_score);
    }

    public void GoToNextScene()
    {
        m_ResultPopup.GetComponent<ResultPopupBehavior>().OnQuitActivity();
    }    

    public void BackToUnit()
    {
        SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
        //m_ResultPopup.GetComponent<ResultPopupBehavior>().OnQuitActivity();
    }

    public void BackToLastScene()
    {
        //SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
        m_ResultPopup.GetComponent<ResultPopupBehavior>().OnQuitActivity();
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

    public void PlayWrongAnswerAudio()
    {
        StartCoroutine(WrongAnswerAudioSequence());
    }

    IEnumerator WrongAnswerAudioSequence()
    {
        //m_AudioSource.Stop();
        //m_AudioSource.loop = false;
        //m_AudioSource.clip = m_WrongSFX;
        //m_AudioSource.Play();
        //yield return new WaitForSeconds(2.0f);

        yield return null;

        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_WrongAnswers[Random.Range(0, m_WrongAnswers.Count)];
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.consoling_delay);
        teacherCharacter.PlayConsolingAnimation(m_AudioSource.clip.length);
    }

    public void PlayCorrectAnswerAudio()
    {
        StartCoroutine(CorrectAnswerAudioSequence());
    }

    IEnumerator CorrectAnswerAudioSequence()
    {
        //m_AudioSource.Stop();
        //m_AudioSource.loop = false;
        //m_AudioSource.clip = m_CorrectSFX;
        //m_AudioSource.Play();
        //yield return new WaitForSeconds(2.0f);

        yield return null;

        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_CorrectAnswers[Random.Range(0, m_CorrectAnswers.Count)];
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.cheering_delay);
        teacherCharacter.PlayCheeringAnimation(m_AudioSource.clip.length);
    }

    public void PlayTeacherTapAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_TeacherTap;
        m_AudioSource.Play();

        teacherCharacter.PlayTalkingAnimation(m_TeacherTap.length);
    }

    public void ShowResultPopUp()
    {
        int sum = 0;
        foreach (int score in partScores)
        {
            sum += score;
        }
        int average = sum / partScores.Count;
        if (sum % partScores.Count >= 0.5f)
            average += 1;

        if (average > 3)
            average = 3;
        if (average < 0)
            average = 0;

        m_ResultPopup.SetActive(true);
        m_ResultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(average);
    }

    public void PlayIdleAudio()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = m_ActivityGuide;
        m_AudioSource.PlayDelayed(TeacherCharacterAnimationController.pointing_delay);

        teacherCharacter.PlayPointingAnimation(m_ActivityGuide.length);
    }    

    public void ToggleBMG()
    {
        if (m_BGMSource.isPlaying == false)
        {
            m_BGMImg.sprite = m_BGMOn;
            m_BGMSource.Play();
        }
        else
        {
            m_BGMImg.sprite = m_BGMOff;
            m_BGMSource.Stop();
        }
    }
}
