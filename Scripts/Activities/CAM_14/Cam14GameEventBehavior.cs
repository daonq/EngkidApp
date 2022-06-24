using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Cam14GameEventBehavior : BaseAcivityEventBehavior
{
    //[Header("Type setting")]
    //public bool isFillInTheBlank = true;

    [Header("Misc.")]
    public GameObject m_Tutorial;
    public AudioSource m_BGMManager;
    public Cam14ActivityBehavior m_ActivityBehavior;
    public GameObject m_ConnectionTimeOutPopUp;
    public GameObject m_ConnectionErrorPopUp;

    [Header("Audio")]
    public List<AudioClip> m_LinesAudio = new List<AudioClip>();
    int chosenAudioIndex = 0;
    public bool m_IsSingleWord = true;

    [Header("Evaluation")]
    public GameObject m_EvaluationWindow;
    public RectTransform m_CorrectHLImg;
    public RectTransform m_IncorrectHLImg;
    public Text m_ReadingText;
    public Image m_ReadingProgressBar;
    public GameObject m_ReadingProgressObj;

    [Header("Next event")]
    public GameObject m_ResultBoard;

    //internal
    float idleCheckCoolDown = 20.0f;
    bool isCheckingIdle = false;
    Coroutine evaluatingSequenceCor = null;

    private void OnEnable()
    {
        if (m_Tutorial != null)
        {
            m_Tutorial.SetActive(false);
        }

        m_EvaluationWindow.transform.localScale = Vector3.zero;
        LeanTween.alpha(m_CorrectHLImg, 0.0f, 0.0f);
        LeanTween.alpha(m_IncorrectHLImg, 0.0f, 0.0f);

        if (m_ResultBoard != null)
        {
            m_ResultBoard.SetActive(false);
        }

        m_ActivityBehavior.m_TeacherReady = false;
    }

    public override void StartEvent()
    {
        base.StartEvent();

        m_ConnectionTimeOutPopUp.SetActive(false);
        m_ConnectionErrorPopUp.SetActive(false);
        isCheckingIdle = false;

        foreach (Transform child in this.transform.GetChild(2).GetChild(1))
        {
            child.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        StartGameEvent();
    }

    public void StartGameEvent()
    {
        m_ActivityBehavior.m_TeacherReady = false;
        float dur = m_ActivityBehavior.PlayChooseAnAnswerAudio();

        if (m_Tutorial != null)
        {
            m_Tutorial.SetActive(true);
            m_Tutorial.GetComponent<Cam14TutorialBehavior>().StartTutorial(this, dur);
        }
        else
        {
            StartCoroutine(DelayedEnableTeacherTap(dur));
        }

        m_EvaluationWindow.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (isCheckingIdle == true)
        {
            idleCheckCoolDown -= Time.deltaTime;
            if (idleCheckCoolDown <= 0.0f)
            {
                idleCheckCoolDown = 20.0f;
                m_ActivityBehavior.PlayIdleAudio(m_IsSingleWord);
            }
            else
            {
                //empty
            }
        }
    }

    IEnumerator DelayedEnableTeacherTap(float dur)
    {
        yield return new WaitForSeconds(dur);
        isCheckingIdle = true;
        m_ActivityBehavior.m_TeacherReady = true;
    }    

    public void OnSetChosenLineIndex(int i)
    {
        isCheckingIdle = false;
        chosenAudioIndex = i;
    }

    public void OnReadingEvaluation(string sentence)
    {
        m_ActivityBehavior.m_TeacherReady = false;

        m_EvaluationWindow.transform.localScale = Vector3.zero;
        LeanTween.scale(m_EvaluationWindow, Vector3.one, 0.01f).setOnComplete(() => {
            m_EvaluationWindow.GetComponent<EvaluationBehavior>().ShowEvaluationWindow();
        });
        m_ReadingText.text = sentence;

        evaluatingSequenceCor = StartCoroutine(EvaluationSequence());
    }

    IEnumerator ResetEventOnConnectionTimeOut()
    {
        StopCoroutine(evaluatingSequenceCor);
        m_ConnectionTimeOutPopUp.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        StartEvent();
    }

    IEnumerator ResetEventOnConnectionError(string error_text)
    {
        StopCoroutine(evaluatingSequenceCor);
        m_ConnectionErrorPopUp.SetActive(true);
        m_ConnectionErrorPopUp.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<b>Đã có lỗi xảy ra</b>: " + error_text + ". Hãy mở lại hoạt động.";
        yield return new WaitForSeconds(3.0f);
        StartEvent();
    }

    IEnumerator EvaluationSequence()
    {
        yield return new WaitForSeconds(5.0f);
        //play chosen audio line
        m_ActivityBehavior.PlayAudio(m_LinesAudio[chosenAudioIndex]);

        yield return new WaitForSeconds(5.0f);
        m_ActivityBehavior.PlayLetsSayItAudio();

        yield return new WaitForSeconds(2.0f);
        //permission check
        AuthorizationManager.GetInstance().TriggerMicroPopUp();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (AuthorizationManager.GetInstance().isMicPermitted == true)
            StartCoroutine(RecordingAndEvalSequence());
        else
            StartCoroutine(ResetEventOnConnectionError("Micro not authorized!"));
    }

    IEnumerator RecordingAndEvalSequence()
    {
        //start audio recording
        yield return null;
        float record_duration = (m_LinesAudio[chosenAudioIndex].length * 2f);

        m_BGMManager.volume = 0.0f;
        AudioClip record;
        record = Microphone.Start(null, false, (int)record_duration, 44100);

        //audio recordings progress
        m_ReadingProgressObj.SetActive(true);
        float deltaTime = Time.deltaTime;
        for (float timer = 0.0f; timer < record_duration; timer += deltaTime)
        {
            yield return new WaitForSeconds(deltaTime);
            //Debug.Log(timer / record_duration);
            m_ReadingProgressBar.fillAmount = (timer / record_duration);
        }
        yield return null;
        m_ReadingProgressBar.fillAmount = 1.0f;
        yield return new WaitForSeconds(0.25f);

        //end audio recordings
        m_ReadingProgressObj.AddComponent<SelfShaking2D>();
        yield return null;
        WWWForm audio_validation_form = new WWWForm();

        audio_validation_form.AddField("token", "gcxpHQmLeVwLWobE6apU1lgAg49YTMa0");
        audio_validation_form.AddBinaryData("audio-file", WavUtility.FromAudioClip(record), "record.wav");
        audio_validation_form.AddField("text-refs", m_ReadingText.text);

        //pronounciation endpoints------------------------------------------------
        //new:https://ai-pronunciation.x3english.com/phone/api/x3/pronunciation
        //old:https://ai.x3english.org/phone/api/x3/pronunciation
        using (UnityWebRequest audio_validation_request = UnityWebRequest.Post("https://ai-pronunciation.x3english.com/phone/api/x3/pronunciation", audio_validation_form))
        {
            audio_validation_request.timeout = 30;
            yield return audio_validation_request.SendWebRequest();
            if (audio_validation_request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(audio_validation_request.error);
                StartCoroutine(ResetEventOnConnectionError(audio_validation_request.error));
            }
            else
            {
                AudioValidation validation = JsonUtility.FromJson<AudioValidation>(audio_validation_request.downloadHandler.text);
                Debug.Log(validation.total_score);
                if (validation.total_score >= 0.5f)
                {
                    LeanTween.alpha(m_CorrectHLImg, 1.0f, 0.25f).setOnComplete(() => {
                        LeanTween.alpha(m_CorrectHLImg, 0.0f, 0.25f).setDelay(3.0f);

                        m_ActivityBehavior.PlayCorrectAnswerSFX();

                        m_ActivityBehavior.IncreaseScore();
                    });
                }
                else
                {
                    LeanTween.alpha(m_IncorrectHLImg, 1.0f, 0.25f).setOnComplete(() => {
                        LeanTween.alpha(m_IncorrectHLImg, 0.0f, 0.25f).setDelay(3.0f);

                        m_ActivityBehavior.PlayWrongAnswerSFX();
                    });
                }
            }

            Destroy(m_ReadingProgressObj.GetComponent<SelfShaking2D>());
            m_ReadingProgressObj.SetActive(false);
            

            //m_BGMManager.Play();
            m_BGMManager.volume = 0.33f;

            
        }

        yield return new WaitForSeconds(6.0f);
        m_EvaluationWindow.GetComponent<EvaluationBehavior>().HideEvaluationWindow();
        LeanTween.scale(m_EvaluationWindow, Vector3.zero, 0.25f).setDelay(0.5f);

        //call next event
        this.EndEvent();

        yield return new WaitForSeconds(2.0f);
        //call result if result is available
        if (m_ResultBoard != null)
        {
            m_ResultBoard.SetActive(true);
            ResultPopupBehavior popupBehavior = m_ResultBoard.GetComponent<ResultPopupBehavior>();
            if (popupBehavior != null)
            {
                int score = m_ActivityBehavior.m_Score;

                if (score == 5)
                    score = 3;
                else if (score == 4 || score == 3)
                    score = 2;
                else
                    score = 1;

                popupBehavior.OnTriggerResultPopUp(score);
                m_ActivityBehavior.m_Score = 0;
            }
        }
    }
}

[System.Serializable]
public class AudioValidation
{
    [SerializeField] public string audio_url = "";
    [SerializeField] public string text = "";
    [SerializeField] public float total_score = 0.0f;
}
