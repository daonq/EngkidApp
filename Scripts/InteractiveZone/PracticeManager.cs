using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PracticeManager : MonoBehaviour
{
    [Header("Manager")]
    public InteractiveZoneManager m_InteractiveZoneManager;

    [Header("Screen page")]
    public GameObject m_PageRound1;
    public GameObject m_PageRound2;
    public GameObject m_PageReady;
    public GameObject m_Countdown;
    public GameObject m_FinalXp;
    public GameObject m_Review;
    public GameObject m_Intro;
    public GameObject m_ConfirmPopup;
    public GameObject m_Loading;

    [Header("Game Play")]
    public Text m_CountdownText;
    public Text m_CurrentText;
    public Text m_TextRound;
    public Image m_RoundProgress;
    public Button m_HighAudio;
    public Button m_LowAudio;
    public GameObject m_MicInactive;
    public GameObject m_MicRecording;
    public GameObject m_MicWaiting;
    public GameObject m_MicDone;
    public GameObject m_MicFail;
    public GameObject m_TryAgainButton;
    public Text m_gainXP;
    public GameObject m_HighAudioPlaying;
    public GameObject m_LowAudioPlaying;
    public GameObject m_MedAudioPlaying;

    public GameObject m_ScrollChatBox;
    public GameObject m_SampleBox;

    [Header("ResultAi")]
    public GameObject m_ResultAI;
    public Image m_ResultFace;
    public Text m_ResultText;
    public Text m_ResultSentence;
    public Sprite[] m_ResultIcon;
    AudioSource audioSource;
    AudioClip myClip;

    [Header("List Sentence")]
    public GameObject m_ListSentenceBox;
    public GameObject m_CellSentence;
    public ScrollRect m_Scrollview;

    [Header("Audio")]
    public SoundHolder soundHolder;
    public AudioClip soundRound1;
    public AudioClip soundRound2;
    public AudioClip soundCountdown;
    public AudioClip soundReady;
    public AudioClip soundYourTurn;
    public AudioClip soundWellDone;
    public AudioClip soundWrongAnswer;
    public AudioClip soundMicIntro;
    public AudioClip endingMusicClip;

    [Header("UI Control")]
    public SkeletonGraphic roboPracticeAnim;

    ReturnedData dataOrigin;

    bool flagHigh = false;
    bool flagLow = false;
    bool flagMed = false;

    int countFail = 0;
    int retryCount = 0;
    float currentScore = 0.0f;
    List<SentenceAI> dataReview = new List<SentenceAI>();

    string currentLink;

    IEnumerator highProcess;
    IEnumerator lowProcess;
    IEnumerator medProcess;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //AuthorizationManager.GetInstance().TriggerMicroPopUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
        }    
    }

    public void GetDataFromServer()
    {
        //init 
        m_ResultAI.SetActive(false);
        m_MicFail.SetActive(false);
        m_TryAgainButton.GetComponent<Button>().interactable = false;
        countFail = 0;
        retryCount = 0;
        m_MicDone.GetComponent<Button>().interactable = false;

        roboPracticeAnim.AnimationState.SetAnimation(0, "idle", true);

        RefreshAllData();
        FreshData();
        m_Loading.SetActive(true);
        Debug.Log("id == " + m_InteractiveZoneManager.currentLesson._id);
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_GET_SPEAKING_PRACTICE + m_InteractiveZoneManager.currentLesson._id,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get speaking detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get speaking detail.");
                StartCoroutine(PrepareData(server_reply.data));
                //BindData(server_reply.data);
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    IEnumerator PrepareData(ReturnedData data)
    {
        m_Loading.GetComponent<FillProgress>().LockTime(2.0f);
        yield return new WaitForSeconds(2.0f);
        BindData(data);
    }

    public void BindData(ReturnedData data)
    {
        m_Loading.SetActive(false);
        dataOrigin = data;
        //Debug.Log("danh sach vong ==" + data.rounds.Length);
        //for (int i = 0; i < data.rounds.Length; i++)
        //{
        //    for (int j = 0; j < data.rounds[i].turn_play.Length; j++)
        //    {
        //        Debug.Log("cau mau == " + data.rounds[i].turn_play[j].sentence.content);
        //    }
        //}

        //for (int k = 0; k < data.rounds[0].turn_play.Length; k++)
        //{
        //    Debug.Log("cau mau k == " + data.rounds[0].turn_play[k].sentence.content);
        //    GameObject cell = Instantiate(m_CellSentence, m_ListSentenceBox.transform);
        //    cell.GetComponent<ItemRobot>().Setup(data.rounds[0].turn_play[k].sentence.content);
        //}

        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;
        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;

        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().PauseBGM();

        StateRobot();

        StartCoroutine(GameOn());

    }

    IEnumerator GameOn()
    {
        StartRound1();
        yield return new WaitForSeconds(2.0f);
        ShowCountdown();
        soundHolder.PlaySound(soundCountdown);
        DisplayCountdown("3");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("2");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("1");
        yield return new WaitForSeconds(1.0f);
        StartReady();
        yield return new WaitForSeconds(3.0f);
        PlayRound1();
    }

    private void StartRound1()
    {
        m_PageRound1.SetActive(true);
        m_Countdown.SetActive(false);
        m_PageReady.SetActive(false);
        m_PageRound2.SetActive(false);

        m_TextRound.text = "ROUND 1/2";
        m_RoundProgress.fillAmount = 0;
        soundHolder.PlaySound(soundRound1);

        ClearChat();
    }

    private void ShowCountdown()
    {
        m_PageRound1.SetActive(false);
        m_Countdown.SetActive(true);
        m_PageReady.SetActive(false);
        m_PageRound2.SetActive(false);
    }

    private void DisplayCountdown(string value)
    {
        m_CountdownText.text = value;
    }

    private void StartReady()
    {
        m_PageRound1.SetActive(false);
        m_Countdown.SetActive(false);
        m_PageReady.SetActive(true);
        m_PageRound2.SetActive(false);
        soundHolder.PlaySound(soundReady);
    }

    private void StartRound2()
    {

        FreshData();

        m_PageRound1.SetActive(false);
        m_PageReady.SetActive(false);
        m_PageRound2.SetActive(true);

        m_TextRound.text = "ROUND 2/2";
        m_RoundProgress.fillAmount = 0;
        soundHolder.PlaySound(soundRound2);

        ClearChat();
    }

    private void PlayRound1()
    {
        m_PageRound1.SetActive(false);
        m_PageReady.SetActive(false);
        m_PageRound2.SetActive(false);

        StartCoroutine(ListenProcess());
    }

    int step = 0;
    int currentRound = 0;
    IEnumerator ListenProcess()
    {

        roboPracticeAnim.AnimationState.SetAnimation(0, "talking_sentence", true);

        m_SampleBox.SetActive(false);
        if (step < dataOrigin.rounds[currentRound].turn_play.Length)
        {
            StatePlaying();
            StateRobot();
            m_CurrentText.text = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
            //TODO: keep the following line for future updates
            //StartCoroutine(GetAudioClip(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_high_speed.value));
            //StartCoroutine(GetAudioClip(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_low_speed.value));
            StartCoroutine(GetAudioClip(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio.value));
            Debug.Log("dang phat am: " + dataOrigin.rounds[currentRound].turn_play[step].sentence.duration);

            //todo nobita
            GameObject cell = Instantiate(m_CellSentence, m_ListSentenceBox.transform);
            yield return null;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            //Canvas.ForceUpdateCanvases();
            //m_Scrollview.verticalScrollbar.value = 0f;
            //Canvas.ForceUpdateCanvases();
            //m_Scrollview.verticalScrollbar.value = 0f;
            cell.GetComponent<ItemRobot>().Setup(dataOrigin.rounds[currentRound].turn_play[step].sentence.content, 0);
            cell.GetComponent<ItemRobot>().DisplayMode(1);
            yield return null;
            yield return null;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            SentenceAI item = new SentenceAI();
            item.result.textReference = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
            dataReview.Add(item);
            yield return null;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            //end

            yield return new WaitForSeconds(dataOrigin.rounds[currentRound].turn_play[step].sentence.duration);
            cell.GetComponent<ItemRobot>().DisplayMode(0);

            yield return null;
            yield return null;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            m_Scrollview.verticalScrollbar.value = 0f;

            step = step + 1;
            m_RoundProgress.fillAmount = (float)step / dataOrigin.rounds[currentRound].turn_play.Length;

            yield return intro_1_cor = StartCoroutine(ShowIntro1());

            if (step < dataOrigin.rounds[currentRound].turn_play.Length)
            {

                TurnRecord();
            }
            else
            {
                //Debug.Log("khong ghi duoc nua");
                yield return new WaitForSeconds(1f);
                if (currentRound + 1 < dataOrigin.rounds.Length)
                {
                    currentRound = currentRound + 1;
                    step = 0;
                    StartCoroutine(GameOnTurn2());
                }
                else
                {
                    //Debug.Log("Ket thuc duoc roi tu phat am");
                    //roboPracticeAnim.AnimationState.SetAnimation(0, "Idle_to_Jump", false);
                    //yield return new WaitForSeconds(0.467f);
                    soundHolder.PlaySound(endingMusicClip);
                    roboPracticeAnim.AnimationState.SetAnimation(0, "jumping", true);
                    yield return new WaitForSeconds(3.37f);
                    roboPracticeAnim.AnimationState.SetAnimation(0, "idle", true);
                    yield return new WaitForSeconds(1.0f);

                    FinishChallenge();
                }
            }
        }
        else
        {
            //Debug.Log("khong phat am duoc nua");
            yield return new WaitForSeconds(1f);
            if (currentRound + 1 < dataOrigin.rounds.Length)
            {
                currentRound = currentRound + 1;
                step = 0;
                StartCoroutine(GameOnTurn2());
            }
            else
            {
                //Debug.Log("Ket thuc duoc roi tu phat am");
                //roboPracticeAnim.AnimationState.SetAnimation(0, "Idle_to_Jump", false);
                //yield return new WaitForSeconds(0.467f);
                soundHolder.PlaySound(endingMusicClip);
                roboPracticeAnim.AnimationState.SetAnimation(0, "jumping", true);
                yield return new WaitForSeconds(3.37f);
                roboPracticeAnim.AnimationState.SetAnimation(0, "idle", true);
                yield return new WaitForSeconds(1.0f);

                FinishChallenge();
            }
        }
    }

    IEnumerator GetAudioClip(string link)
    {
        Debug.Log("link audio == " + link);
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    //Debug.Log("Audio is playing.");
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    //Debug.Log("Audio is playing.");
                }
            }
        }

    }

    //public void RecordVoice()
    //{
    //    StateRecording();
    //    audioSource.Stop();
    //    StartCoroutine(EvaluationSequence());
    //}

    //IEnumerator EvaluationSequence()
    //{

    //    AudioClip record;
    //    record = Microphone.Start(null, false, 10, 44100);
    //    yield return new WaitForSeconds(10.0f);

    //    // todo nobita save file
    //    SaveMav.Save("record.wav", record);

    //    // todo nobita get score from AI
    //    WWWForm audio_validation_form = new WWWForm();

    //    audio_validation_form.AddField("token", "gcxpHQmLeVwLWobE6apU1lgAg49YTMa0");
    //    audio_validation_form.AddBinaryData("audio-file", WavUtility.FromAudioClip(record), "record.wav");
    //    audio_validation_form.AddField("text-refs", "How are you?");

    //    using (UnityWebRequest audio_validation_request = UnityWebRequest.Post("https://ai.x3english.org/phone/api/x3/pronunciation", audio_validation_form))
    //    {
    //        yield return audio_validation_request.SendWebRequest();
    //        if (audio_validation_request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log(audio_validation_request.error);
    //        }
    //        else
    //        {
    //            AudioValidation validation = JsonUtility.FromJson<AudioValidation>(audio_validation_request.downloadHandler.text);
    //            Debug.Log(validation.total_score);
    //            // todo NOBITA send to BACKEND
    //            Debug.Log("nobita AI result == " + audio_validation_request.downloadHandler.text);
    //            Debug.Log("nobita record == " + dataOrigin.record_id);
    //            Debug.Log("nobita sentence == " + dataOrigin.rounds[0].turn_play[0].sentence._id);
    //            Debug.Log("nobita token == " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);

    //            SpeakingAI speakingAi = new SpeakingAI();
    //            speakingAi.ai_result = audio_validation_request.downloadHandler.text;
    //            speakingAi.record_id = dataOrigin.record_id;
    //            speakingAi.sentence_id = dataOrigin.rounds[0].turn_play[0].sentence._id;

    //            SendAIResultToBackend(speakingAi);
    //        }

    //    }
    //    yield return new WaitForSeconds(0.5f);

    //}

    public void SendAIResultToBackend(SpeakingAI data)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_POST_AI_RECORD_TO_BACKEND,
            JsonUtility.ToJson(data),
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get speaking detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get speaking detail.");

            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    public void GetFinalResutFromBackend()
    {
        FinalPractice record = new FinalPractice(dataOrigin.record_id);
        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_SPEAKING_FINAL,
            JsonUtility.ToJson(record),
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get speaking final detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get speaking final detail.");
                m_FinalXp.SetActive(true);
                m_gainXP.text = "+" + server_reply.data.gained_xp;
                RefreshCoin();
                UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("interactive_zone", "on_get_final_result_practice");
                SendEventFirebase.SendEventInteractivePractice(m_InteractiveZoneManager.currentLesson.title, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
                /*
                SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    private void StatePlaying()
    {
        //m_MicActive.SetActive(false);
        //m_MicInactive.SetActive(true);
        //m_MicLock.SetActive(false);

        m_HighAudio.interactable = false;
        m_LowAudio.interactable = false;
        m_MicInactive.GetComponent<Button>().interactable = false;
    }

    private void TurnRecord()
    {
        //m_HighAudio.interactable = true;
        //m_LowAudio.interactable = true;
        m_MicInactive.GetComponent<Button>().interactable = false;
        soundHolder.PlaySound(soundYourTurn);
        StartCoroutine(StartYourTurn());
        m_CurrentText.text = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
        m_Intro.GetComponent<IntroPracticeMode>().txtQuest.text = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;

    }

    IEnumerator StartYourTurn()
    {
        roboPracticeAnim.AnimationState.SetAnimation(0, "talk_to_yourturn", false);
        yield return new WaitForSeconds(0.47f);
        roboPracticeAnim.AnimationState.SetAnimation(0, "yourturn_talking", true);
        yield return new WaitForSeconds(2.5f);
        roboPracticeAnim.AnimationState.SetAnimation(0, "yourturn_idle", true);
        m_SampleBox.SetActive(true);
        m_HighAudio.interactable = true;
        m_LowAudio.interactable = true;
        m_MicInactive.GetComponent<Button>().interactable = true;

    }

    public void OnHighAudioClick()
    {
        m_Intro.GetComponent<IntroPracticeMode>().soundHolder.CancelSound();
        audioSource.Stop();
        if (flagHigh)
        {
            m_HighAudioPlaying.SetActive(false);
            if (highProcess != null)
            {
                StopCoroutine(highProcess);
            }
            flagHigh = false;
        }
        else
        {
            // stop snail
            m_LowAudioPlaying.SetActive(false);
            if (lowProcess != null)
            {
                StopCoroutine(lowProcess);
            }
            flagLow = false;

            // stop record
            m_MedAudioPlaying.SetActive(false);
            if (medProcess != null)
            {
                StopCoroutine(medProcess);
            }
            flagMed = false;

            //TODO: keep following lines for future updates
            //string link = dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_high_speed.value;
            //string link = dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_low_speed.value;
            string link = dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio.value;
            highProcess = GetAudioClipHigh(link);
            StartCoroutine(highProcess);
        }
    }

    public void OnLowAudioClick()
    {
        audioSource.Stop();
        if (flagLow)
        {
            m_LowAudioPlaying.SetActive(false);
            if (lowProcess != null)
            {
                StopCoroutine(lowProcess);
            }
            flagLow = false;
        }
        else
        {
            m_HighAudioPlaying.SetActive(false);
            if (highProcess != null)
            {
                StopCoroutine(highProcess);
            }
            flagHigh = false;

            m_MedAudioPlaying.SetActive(false);
            if (medProcess != null)
            {
                StopCoroutine(medProcess);
            }
            flagMed = false;

            //string link = dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_low_speed.value;
            string link = dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio.value;
            lowProcess = GetAudioClipLow(link);
            StartCoroutine(lowProcess);
        }
    }

    public void OnRecordClick()
    {
        StartCoroutine(OnRecordClickSequence());
    }

    IEnumerator OnRecordClickSequence()
    {
        //if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        //{
        //    StateRecording();
        //    audioSource.Stop();
        //    StartCoroutine(EvaluationSequence2());
        //}
        //else
        //{
        //    m_MicPermissionPopup.SetActive(true);
        //    soundHolder.PlaySound(soundMicIntro);
        //    yield return new WaitForSeconds(soundMicIntro.length);

        //    yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        //    if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        //    {
        //        StateRecording();
        //        audioSource.Stop();
        //        StartCoroutine(EvaluationSequence2());
        //    }
        //    else
        //    {
        //        StartCoroutine(OnRecordClickSequence());
        //    }
        //}

        //AuthorizationManager.GetInstance().TriggerMicroPopUp();
        //while (AuthorizationManager.GetInstance().isMicPermitted == false)
        //{
        //    yield return null;
        //}
        //if (AuthorizationManager.GetInstance().isMicPermitted == true)
        //{
        //    StateRecording();
        //    audioSource.Stop();
        //    StartCoroutine(EvaluationSequence2());
        //}

        yield return null;
        StateRecording();
        audioSource.Stop();
        StartCoroutine(EvaluationSequence2());
    }

    GameObject cell;
    AudioClip lastRecorded = null;
    IEnumerator EvaluationSequence2()
    {
        //mic permission
        AuthorizationManager.GetInstance().TriggerMicroPopUp();
        while (AuthorizationManager.GetInstance().isMicPermitted == false)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        m_HighAudio.transform.GetChild(0).gameObject.SetActive(false);
        m_TryAgainButton.GetComponent<Button>().interactable = false;

        Sentence sentence = dataOrigin.rounds[currentRound].turn_play[step].sentence;
        m_MicRecording.GetComponent<FillProgress>().LockTime(dataOrigin.rounds[currentRound].turn_play[step].sentence.duration);
        AudioClip record;
        //Debug.Log("thoi gian ghi am == " + (int)sentence.duration);
        record = Microphone.Start(null, false, (int)sentence.duration, 16000);
        //if (countFail == 0)
        //{
            //todo nobita
        cell = Instantiate(m_CellSentence, m_ListSentenceBox.transform);
        cell.GetComponent<ItemRobot>().Setup(dataOrigin.rounds[currentRound].turn_play[step].sentence.content, 1);
        cell.GetComponent<ItemRobot>().DisplayMode(1);
        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;
        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;

        yield return null;
        yield return new WaitForSeconds((int)sentence.duration);
        lastRecorded = record;
        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;
        Canvas.ForceUpdateCanvases();
        m_Scrollview.verticalScrollbar.value = 0f;

        StateWaiting();

        // todo nobita save file
        SaveMav.Save("record.wav", record);

        // todo nobita get score from AI
        WWWForm audio_validation_form = new WWWForm();

        audio_validation_form.AddField("token", "gcxpHQmLeVwLWobE6apU1lgAg49YTMa0");
        audio_validation_form.AddBinaryData("audio-file", WavUtility.FromAudioClip(record), "record.wav");
        audio_validation_form.AddField("text-refs", sentence.content);
        using (UnityWebRequest audio_validation_request = UnityWebRequest.Post(DataBaseInterface.INTERACTIVE_ZONE_AI_SERVER, audio_validation_form))
        {
            yield return audio_validation_request.SendWebRequest();
            if (audio_validation_request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(audio_validation_request.error);
            }
            else
            {
                SentenceAI validation = JsonUtility.FromJson<SentenceAI>(audio_validation_request.downloadHandler.text);
                //Debug.Log("diem cua cau noi == " + validation.result.score);
                //Debug.Log("link audio == " + validation.audio_url);
                currentLink = validation.audio_url;

                currentScore = validation.result.score;

                // todo NOBITA send to BACKEND
                //Debug.Log("nobita AI result == " + audio_validation_request.downloadHandler.text);
                //Debug.Log("nobita record == " + dataOrigin.record_id);
                //Debug.Log("nobita sentence == " + sentence._id);
                //Debug.Log("nobita token == " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);

                string result = "";
                for (int i = 0; i < validation.result.words.Length; i++)
                {
                    string word = validation.result.words[i].wordRef;
                    Debug.Log("Word " + i + ": " + word + " == " + validation.result.words[i].wordScore);
                    
                    if (validation.result.words[i].wordScore >= 70.0f)
                    {
                        word = "<color=#226D45>" + word + "</color>";
                    }
                    else if (validation.result.words[i].wordScore >= 50.0f &&
                             validation.result.words[i].wordScore < 70.0f)
                    {
                        word = "<color=#FFD700>" + word + "</color>";
                    }
                    else
                    {
                        word = "<color=#CC2D00>" + word + "</color>";
                    }
                    result = result + word + " ";
                }

                //m_CurrentText.text = result; //not display color
                yield return null;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;

                m_ResultAI.SetActive(true);
                m_ResultSentence.text = result;

                if (validation.result.score <= 0.25f)
                {
                    soundHolder.PlaySound(soundWrongAnswer);
                }
                else
                {
                    soundHolder.PlaySound(soundWellDone);
                }

                if (validation.result.score <= 0.25f)
                {
                    m_ResultText.text = "Sorry, try again!!!";
                    m_ResultFace.sprite = m_ResultIcon[0];
                }
                else if (validation.result.score > 0.25f && validation.result.score <= 0.50f)
                {
                    m_ResultText.text = "Good!";
                    m_ResultFace.sprite = m_ResultIcon[1];
                }
                else if (validation.result.score > 0.5f && validation.result.score <= 0.75f)
                {
                    m_ResultText.text = "Very Good!";
                    m_ResultFace.sprite = m_ResultIcon[2];
                }
                else
                {
                    m_ResultText.text = "Awesome!";
                    m_ResultFace.sprite = m_ResultIcon[3];
                }

                SpeakingAI speakingAi = new SpeakingAI();
                speakingAi.ai_result = audio_validation_request.downloadHandler.text;
                speakingAi.record_id = dataOrigin.record_id;
                speakingAi.sentence_id = sentence._id;

                yield return null;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;

                //if (countFail == 0)
                if (validation.result.score > 0.25f)
                {
                    //todo nobita
                    //GameObject cell = Instantiate(m_CellSentence, m_ListSentenceBox.transform);
                    //cell.GetComponent<ItemRobot>().Setup(dataOrigin.rounds[currentRound].turn_play[step].sentence.content, 1);

                    //ShowCurrentItem();

                    //end
                    if (cell != null)
                    {
                        cell.GetComponent<ItemRobot>().DisplayMode(0);
                        yield return null;
                        yield return null;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                    }
                }

                if (validation.result.score > 0.25f)
                {
                    StateDone();
                    //countFail = 0;

                    SentenceAI item = new SentenceAI();
                    dataReview.Add(validation);

                    if (cell != null)
                    {
                        cell.GetComponent<ItemRobot>().DisplayMode(0);
                        yield return null;
                        yield return null;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                    }

                    //step = step + 1; todo increase step
                    m_RoundProgress.fillAmount = (float)(step + 1) / dataOrigin.rounds[currentRound].turn_play.Length;
                }
                else
                {
                    countFail = countFail + 1;

                    SentenceAI item = new SentenceAI();
                    item.result.textReference = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
                    dataReview.Remove(dataReview[dataReview.Count - 1]);
                    dataReview.Add(item);

                    if (cell != null)
                    {
                        //cell.GetComponent<ItemRobot>().DisplayMode(1);
                        Destroy(cell);
                        yield return null;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                        Canvas.ForceUpdateCanvases();
                        m_Scrollview.verticalScrollbar.value = 0f;
                    }

                    StateFail();
                    m_RoundProgress.fillAmount = (float)(step + 1) / dataOrigin.rounds[currentRound].turn_play.Length;
                }

                yield return null;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;
                Canvas.ForceUpdateCanvases();
                m_Scrollview.verticalScrollbar.value = 0f;

                if (validation.result.score <= 0.25f)
                {
                    m_Intro.GetComponent<IntroPracticeMode>().txtResult.text = "Sorry, try again!!!";
                    m_Intro.GetComponent<IntroPracticeMode>().imvResult.sprite = m_ResultIcon[0];
                }
                else if (validation.result.score > 0.25f && validation.result.score <= 0.50f)
                {
                    m_Intro.GetComponent<IntroPracticeMode>().txtResult.text = "Good!";
                    m_Intro.GetComponent<IntroPracticeMode>().imvResult.sprite = m_ResultIcon[1];
                }
                else if (validation.result.score > 0.5f && validation.result.score <= 0.75f)
                {
                    m_Intro.GetComponent<IntroPracticeMode>().txtResult.text = "Very Good!";
                    m_Intro.GetComponent<IntroPracticeMode>().imvResult.sprite = m_ResultIcon[2];
                }
                else
                {
                    m_Intro.GetComponent<IntroPracticeMode>().txtResult.text = "Awesome!";
                    m_Intro.GetComponent<IntroPracticeMode>().imvResult.sprite = m_ResultIcon[3];
                }
                m_Intro.GetComponent<IntroPracticeMode>().txtResultAI.text = result;
                
                if (PlayerPrefs.HasKey("E_INTERACTIVE_ZONE_STEP_4") == true)
                {
                    ShowIntro5();
                }
                yield return intro_4_cor = StartCoroutine(ShowIntro4());

                SendAIResultToBackend(speakingAi);
            }

        }
        //yield return new WaitForSeconds(0.5f);
        m_HighAudio.interactable = true;
        yield return new WaitForSeconds(0.5f);
    }

    private void StateRobot()
    {
        m_MicInactive.SetActive(true);
        m_MicRecording.SetActive(false);
        m_MicWaiting.SetActive(false);
        m_MicDone.SetActive(false);
        m_MicFail.SetActive(false);

        m_HighAudio.interactable = false;
        m_LowAudio.interactable = false;
    }

    private void StateRecording()
    {
        m_MicInactive.SetActive(false);
        m_MicRecording.SetActive(true);
        m_MicWaiting.SetActive(false);
        m_MicDone.SetActive(false);
        m_MicFail.SetActive(false);

        m_HighAudio.interactable = false;
        m_LowAudio.interactable = false;
    }

    private void StateWaiting()
    {
        m_MicInactive.SetActive(false);
        m_MicRecording.SetActive(false);
        m_MicWaiting.SetActive(true);
        m_MicDone.SetActive(false);
        m_MicFail.SetActive(false);

        m_HighAudio.interactable = false;
        m_LowAudio.interactable = false;
    }

    private void StateDone()
    {
        m_MicInactive.SetActive(false);
        m_MicRecording.SetActive(false);
        m_MicWaiting.SetActive(false);

        m_MicDone.SetActive(true);
        if (countFail > 2)
            m_MicDone.GetComponent<Button>().interactable = true;
        else
            m_MicDone.GetComponent<Button>().interactable = true;

        m_MicFail.SetActive(false);
        m_TryAgainButton.GetComponent<Button>().interactable = true;

        m_HighAudio.interactable = false;
        m_LowAudio.interactable = false;
    }

    private void StateFail()
    {
        m_MicInactive.SetActive(false);
        m_MicRecording.SetActive(false);
        m_MicWaiting.SetActive(false);

        m_MicDone.SetActive(true);
        if (countFail > 2)
            m_MicDone.GetComponent<Button>().interactable = true;
        else
            m_MicDone.GetComponent<Button>().interactable = false;

        //m_MicFail.SetActive(true);
        m_TryAgainButton.GetComponent<Button>().interactable = true;

        m_HighAudio.interactable = true;
        m_LowAudio.interactable = true;

        //TODO: new onboarding


    }

    public void OnDoneClick()
    {
        //retryCount = 0;
        countFail = 0;
        m_Intro.GetComponent<IntroPracticeMode>().soundHolder.CancelSound();
        step = step + 1; // todo increase step
        
        m_ResultAI.SetActive(false);
        m_TryAgainButton.GetComponent<Button>().interactable = false;

        StartCoroutine(ListenProcess());
    }

    public void OnFailClick()
    {
        //Debug.Log("play luot " + countFail);
        //retryCount++;
        m_Intro.GetComponent<IntroPracticeMode>().soundHolder.CancelSound();

        m_ResultAI.SetActive(false);

        m_MicInactive.SetActive(true);
        m_MicRecording.SetActive(false);
        m_MicWaiting.SetActive(false);
        m_MicDone.SetActive(false);
        m_MicFail.SetActive(false);
        m_TryAgainButton.GetComponent<Button>().interactable = false;

        m_HighAudio.interactable = true;
        m_LowAudio.interactable = true;

        //StateRecording();
        //audioSource.Stop();
        //StartCoroutine(EvaluationSequence2());
    }

    IEnumerator GameOnTurn2()
    {
        StartRound2();
        yield return new WaitForSeconds(2.0f);
        ShowCountdown();
        soundHolder.PlaySound(soundCountdown);
        DisplayCountdown("3");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("2");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("1");
        yield return new WaitForSeconds(1.0f);
        StartReady();
        yield return new WaitForSeconds(3.0f);
        PlayRound2();
    }

    private void PlayRound2()
    {
        m_PageRound1.SetActive(false);
        m_PageReady.SetActive(false);
        m_PageRound2.SetActive(false);

        StateRobot();
        TurnRecord();
    }

    private void FinishChallenge()
    {
        GetFinalResutFromBackend();
    }

    public void OnReviewClick()
    {
        m_Review.SetActive(true);
        List<SentenceAI> reviewList = new List<SentenceAI>();
        for (int i = 0; i < dataReview.Count / 2; i++)
        {
            if (dataReview[i].audio_url == null || dataReview[i].audio_url.Equals(""))
            {
                reviewList.Add(dataReview[i + dataReview.Count / 2]);
            }
            else
            {
                reviewList.Add(dataReview[i]);
            }
        }
        //for (int k = 0; k < reviewList.Count; k++)
        //{
        //    Debug.Log("cau [" + k + "] == " + reviewList[k].result.textReference);
        //    Debug.Log("cau [" + k + "] == " + reviewList[k].audio_url);
        //}
        m_Review.GetComponent<ReplayPageScript>().Setup(reviewList);
    }

    public void OnChallengePopupClick()
    {
        m_InteractiveZoneManager.ShowDuelListPage();
    }

    public void OnChallengeReviewClick()
    {

    }

    private void ClearChat()
    {
        foreach (Transform child in m_ListSentenceBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    //private void ShowCurrentItem()
    //{
    //    StartCoroutine(ScrollToBottom());
    //}

    //IEnumerator ScrollToBottom()
    //{
    //    yield return new WaitForEndOfFrame();
    //    m_ScrollChatBox.SetActive(true);
    //    m_ScrollChatBox.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    //}

    public void OnNormalSpeedIntro()
    {
        //TODO: keep the following line for future updates
        //StartCoroutine(GetAudioClipStep1(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_high_speed.value));

        //StartCoroutine(GetAudioClipStep1(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_low_speed.value));
        //Time.timeScale = 1.0f;
        if (intro_1_cor != null)
            StopCoroutine(intro_1_cor);
        m_Intro.GetComponent<IntroPracticeMode>().soundHolder.CancelSound();
        StartCoroutine(GetAudioClipStep1(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio.value));
    }

    //public void OnLowSpeedIntro()
    //{
    //    StartCoroutine(GetAudioClipStep2(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_low_speed.value));
    //}

    public void OnRecordIntro()
    {
        //#if PLATFORM_ANDROID
        //        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        //        {
        //            m_MicPermissionPopup.SetActive(true);
        //            soundHolder.PlaySound(soundMicIntro);
        //            return;
        //        }
        //#endif

        //#if PLATFORM_
        //#endif
        m_Intro.GetComponent<IntroPracticeMode>().soundHolder.CancelSound();

        StartCoroutine(OnRecordIntroSequence());
    }

    IEnumerator OnRecordIntroSequence()
    {
        //if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        //{
        //    m_Intro.SetActive(false);
        //    StateRecording();
        //    audioSource.Stop();
        //    StartCoroutine(EvaluationSequence2());
        //}
        //else
        //{
        //    m_MicPermissionPopup.SetActive(true);
        //    soundHolder.PlaySound(soundMicIntro);
        //    yield return new WaitForSeconds(soundMicIntro.length);

        //    yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        //    if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        //    {
        //        m_Intro.SetActive(false);
        //        StateRecording();
        //        audioSource.Stop();
        //        StartCoroutine(EvaluationSequence2());
        //    }
        //    else
        //    {
        //        StartCoroutine(OnRecordIntroSequence());
        //    }
        //}

        //AuthorizationManager.GetInstance().TriggerMicroPopUp();
        //while (AuthorizationManager.GetInstance().isMicPermitted == false)
        //{
        //    yield return null;
        //}
        //if (AuthorizationManager.GetInstance().isMicPermitted == true)
        //{
        //    m_Intro.SetActive(false);
        //    StateRecording();
        //    audioSource.Stop();
        //    StartCoroutine(EvaluationSequence2());
        //}

        m_Intro.SetActive(false);
        yield return null;

        if (step < dataOrigin.rounds[currentRound].turn_play.Length)
        {

            TurnRecord();
        }
        else
        {
            //Debug.Log("khong ghi duoc nua");
            yield return new WaitForSeconds(1f);
            if (currentRound + 1 < dataOrigin.rounds.Length)
            {
                currentRound = currentRound + 1;
                step = 0;
                StartCoroutine(GameOnTurn2());
            }
            else
            {
                //Debug.Log("Ket thuc duoc roi tu phat am");
                //roboPracticeAnim.AnimationState.SetAnimation(0, "Idle_to_Jump", false);
                //yield return new WaitForSeconds(0.467f);
                soundHolder.PlaySound(endingMusicClip);
                roboPracticeAnim.AnimationState.SetAnimation(0, "jumping", true);
                yield return new WaitForSeconds(3.37f);
                roboPracticeAnim.AnimationState.SetAnimation(0, "idle", true);
                yield return new WaitForSeconds(1.0f);

                FinishChallenge();
            }
        }

        //StateRecording();
        //audioSource.Stop();
        //StartCoroutine(EvaluationSequence2());
    }

    IEnumerator GetAudioClipStep1(string link)
    {
        m_Intro.GetComponent<IntroPracticeMode>().PlayNormalSpeed();
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                    ShowIntro3();
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    m_Intro.GetComponent<IntroPracticeMode>().StopNormalSpeed();
                    //ShowIntro2();
                    ShowIntro3();
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                Debug.Log("link audio == " + link);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                    ShowIntro3();
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    //ShowIntro2();
                    ShowIntro3();
                }
            }
        }

    }

    //IEnumerator GetAudioClipStep2(string link)
    //{
    //    Debug.Log("link audio == " + link);
    //    m_Intro.GetComponent<IntroPracticeMode>().PlaySlowSpeed();
    //    if (link.EndsWith(".wav"))
    //    {
    //        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
    //        {
    //            yield return www.SendWebRequest();

    //            if (www.result == UnityWebRequest.Result.ConnectionError)
    //            {
    //                Debug.Log(www.error);
    //            }
    //            else
    //            {
    //                myClip = DownloadHandlerAudioClip.GetContent(www);
    //                audioSource.clip = myClip;
    //                audioSource.Play();
    //                yield return new WaitForSeconds(myClip.length);
    //                m_Intro.GetComponent<IntroPracticeMode>().StopSlowSpeed();
    //                ShowIntro3();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
    //        {
    //            yield return www.SendWebRequest();

    //            if (www.result == UnityWebRequest.Result.ConnectionError)
    //            {
    //                Debug.Log(www.error);
    //            }
    //            else
    //            {
    //                myClip = DownloadHandlerAudioClip.GetContent(www);
    //                audioSource.clip = myClip;
    //                audioSource.Play();
    //                yield return new WaitForSeconds(myClip.length);
    //                ShowIntro3();
    //            }
    //        }
    //    }

    //}

    IEnumerator GetAudioClipResult(string link, AudioClip clip = null)
    {
        //Debug.Log("link audio == " + link);
        flagMed = true;
        m_MedAudioPlaying.SetActive(true);

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            flagMed = false;
            m_MedAudioPlaying.SetActive(false);

            ShowIntro5();
        }
        else
        {
            if (link.EndsWith(".wav"))
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        myClip = DownloadHandlerAudioClip.GetContent(www);
                        audioSource.clip = myClip;
                        audioSource.Play();
                        yield return new WaitForSeconds(myClip.length);
                        flagMed = false;
                        m_MedAudioPlaying.SetActive(false);

                        ShowIntro5();
                    }
                }
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        myClip = DownloadHandlerAudioClip.GetContent(www);
                        audioSource.clip = myClip;
                        audioSource.Play();
                        yield return new WaitForSeconds(myClip.length);
                        flagMed = false;
                        m_MedAudioPlaying.SetActive(false);

                        ShowIntro5();
                    }
                }
            }
        }
    }

    public void OnReplayRecord()
    {
        //StartCoroutine(GetAudioClipResult(currentLink));

        audioSource.Stop();
        if (flagMed)
        {
            m_MedAudioPlaying.SetActive(false);
            if (medProcess != null)
            {
                StopCoroutine(medProcess);
            }
            flagMed = false;
        }
        else
        {
            // stop audio
            m_HighAudioPlaying.SetActive(false);
            if (highProcess != null)
            {
                StopCoroutine(highProcess);
            }
            flagHigh = false;

            // stop snail
            m_LowAudioPlaying.SetActive(false);
            if (lowProcess != null)
            {
                StopCoroutine(lowProcess);
            }
            flagLow = false;

            medProcess = GetAudioClipResult(currentLink, lastRecorded);
            StartCoroutine(medProcess);
        }

    }

    public void OnReplayRecordIntro()
    {
        if (intro_4_cor != null)
            StopCoroutine(intro_4_cor);
        soundHolder.CancelSound();
        OnReplayRecord();
    }

    IEnumerator GetAudioClipResultIntro(string link)
    {
        Debug.Log("link audio == " + link);
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                }
            }
        }

    }

    Coroutine intro_1_cor = null;
    public IEnumerator ShowIntro1()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_1", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep1();
            yield return new WaitForSeconds(6.0f);
        }
        yield return null;
    }

    public void ShowIntro2()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_2", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep2();
        }

        //Time.timeScale = 0.0f;
    }

    public void ShowIntro3()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_3", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep3();
        }

        //Time.timeScale = 0.0f;
    }

    Coroutine intro_4_cor = null;
    public IEnumerator ShowIntro4()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_4", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep4();

            yield return new WaitForSeconds(4.0f);
        }

        //Time.timeScale = 0.0f;
    }

    public void ShowIntro5()
    {
        Debug.Log("Retry count: " + countFail);
        if (currentScore <= 0.25f && countFail < 3)
        {
            ShowIntro5L();
        }
        else
        {
            ShowIntro5W();
        }
    }

    public void ShowIntro5W()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_5_W", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep5W();
        }
    }

    public void ShowIntro5L()
    {
        int isFirstTime = PlayerPrefs.GetInt("E_INTERACTIVE_ZONE_STEP_5_L", 0);
        if (isFirstTime == 0)
        {
            m_Intro.GetComponent<IntroPracticeMode>().ShowStep5L();
        }
    }

    public void FreshData()
    {
        m_TextRound.text = "";
        m_ResultText.text = "";
        m_ResultSentence.text = "";
        m_CurrentText.text = "";

        m_SampleBox.SetActive(false);
        m_Loading.GetComponent<FillProgress>().FillEmpty();
    }

    public void RefreshAllData()
    {
        currentRound = 0;
        step = 0;
        dataReview = new List<SentenceAI>();
        FreshData();
        StopAllCoroutines();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void RequestMicPermission()
    {
        Permission.RequestUserPermission(Permission.Microphone);
    }

    public void OnBackClick()
    {
        m_ConfirmPopup.SetActive(true);
    }

    public void OnConfirmBack()
    {
        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("interactive_zone", "on_comfirm_back_practice");
        SendEventFirebase.SendEventInteractivePractice(m_InteractiveZoneManager.currentLesson.title, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "exit");
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
        m_ConfirmPopup.SetActive(false);
        m_InteractiveZoneManager.BackFromPractice();
    }

    public void OnConfirmCancel()
    {
        m_ConfirmPopup.SetActive(false);
    }

    IEnumerator GetAudioClipHigh(string link)
    {
        flagHigh = true;
        m_HighAudioPlaying.SetActive(true);
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    m_HighAudioPlaying.SetActive(false);
                    flagHigh = false;
                    Debug.Log("Audio is playing.");
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    m_HighAudioPlaying.SetActive(false);
                    flagHigh = false;
                    Debug.Log("Audio is playing.");
                }
            }
        }
    }

    IEnumerator GetAudioClipLow(string link)
    {
        flagLow = true;
        m_LowAudioPlaying.SetActive(true);
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    m_LowAudioPlaying.SetActive(false);
                    flagLow = false;
                    Debug.Log("Audio is playing.");
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    m_LowAudioPlaying.SetActive(false);
                    flagLow = false;
                    Debug.Log("Audio is playing.");
                }
            }
        }

    }

    public void RefreshCoin()
    {
        m_InteractiveZoneManager.GetKidInfoFromServer();
    }
}
