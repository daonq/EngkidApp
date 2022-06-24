using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DuelMode : MonoBehaviour
{
    [Header("Manager")]
    public InteractiveZoneManager m_InteractiveZoneManager;

    [Header("Page")]
    public GameObject m_RoundPage;
    public GameObject m_CountdownPopup;
    public GameObject m_ReadyPopup;
    public GameObject m_MainPage;
    public GameObject m_MainMenu;
    public GameObject m_WinPopup;
    public GameObject m_LoosePopup;
    public GameObject m_ReviewPage;
    public GameObject m_ConfirmPopup;

    [Header("InGame")]
    public Text m_TextCountdown;
    public Text m_TextRound;
    public GameObject m_ImageTV;
    public Text m_CurrentText;
    public Text m_NextText;
    public Image m_LeftPersonAvatar;
    public Image m_RightPersonAvatar;
    public Image m_LeftScoreProgress;
    public Image m_RightScoreProgress;
    public GameObject m_LeftLight;
    public GameObject m_RightLight;
    public GameObject m_LeftWinLight;
    public GameObject m_RightWinLight;
    public GameObject m_LeftWin;
    public GameObject m_RightWin;
    public GameObject leftPlayer;
    public GameObject rightPlayer;

    public Text m_RoundName;
    public Text m_LeftPersonRoundName;
    public Text m_RightPersonRoundName;

    public Text m_LeftPersonMainName;
    public Text m_RightPersonMainName;

    public Image m_RoundProgress;

    public GameObject m_MicActive;
    public GameObject m_MicInactive;
    public GameObject m_MicLock;

    [Header("Audio")]
    public SoundHolder soundHolder;
    public AudioClip soundCountdown;
    public AudioClip soundReady;
    public AudioClip soundBegin;
    public AudioClip soundYourTurn;
    public AudioClip soundWin;
    public AudioClip soundLoose;
    public AudioClip soundWaitingResult;
    public AudioClip soundScore;

    AudioSource audioSource;
    AudioClip myClip;

    ReturnedData dataOrigin;

    float playerScoreTurn1 = 0;
    float rivalScoreTurn1 = 0;
    float playerScoreTurn2 = 0;
    float rivalScoreTurn2 = 0;
    float playerSumTurn1 = 0;
    float playerSumTurn2 = 0;
    float rivalSumTurn1 = 0;
    float rivalSumTurn2 = 0;

    List<SentenceAI> dataReview = new List<SentenceAI>();

    string opponent_costume_id = "";

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        m_ImageTV.SetActive(false);

        StartCoroutine(DelayedEnable());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRound == 0)
        {
            if (!String.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().opponent_skin_name))
                leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().opponent_skin_name);
            else
                leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin("boy1");

            if (!String.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().currentSkinName))
                rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
            else
                rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin("boy1");
        }
        else
        {
            if (!String.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().currentSkinName))
                leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
            else
                leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin("boy1");

            if (!String.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().opponent_skin_name))
                rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().opponent_skin_name);
            else
                rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin("boy1");
        }
    }

    private void OnEnable()
    {
        m_LeftPersonAvatar.transform.parent.parent.gameObject.SetActive(false);
        m_RightPersonAvatar.transform.parent.parent.gameObject.SetActive(false);

        StartCoroutine(DelayedEnable());
    }

    IEnumerator DelayedEnable()
    {
        yield return null;
        yield return null;
        yield return null;

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
    }

    public void GetDataFromServer()
    {
        m_MainMenu.SetActive(false);
        m_MainPage.SetActive(false);

        RefreshAllData();
        Refresh();

        //Debug.Log("Starts getting duel data.");
        //Debug.Log("id lesson == " + m_InteractiveZoneManager.currentLesson._id);
        //Debug.Log("id user == " + m_InteractiveZoneManager.currentDuel._id);
        //Debug.Log("current user == " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_GET_SPEAKING_DUEL
            + "lesson_id=" + m_InteractiveZoneManager.currentLesson._id
            + "&competitor_id=" + m_InteractiveZoneManager.currentDuel._id,
            callback_flag =>
            {
                //if (callback_flag == false)
                //    Debug.Log("Error: get speaking detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get speaking detail.");

                //Debug.Log(server_reply);

                if (server_reply.statusCode.Equals("400"))
                {
                    Debug.Log("Get duel data failed.");
                    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();

                    m_InteractiveZoneManager.BackFromDuelMode();
                }
                else
                {
                    BindData(server_reply.data);
                }

            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    public void BindData(ReturnedData data)
    {
        dataOrigin = data;

        SetupScore();
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().PauseBGM();

        m_LeftLight.SetActive(false);
        m_RightLight.SetActive(false);

        //rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);

        //Debug.Log("Duel mode starts loading opponent skin");
        opponent_costume_id = m_InteractiveZoneManager.currentDuel._id;
        UserDataManagerBehavior.GetInstance().opponent_skin_name = "";
        UserDataManagerBehavior.GetInstance().GetOpponentCostumeFromServer(dataOrigin.competitor_account.outfit.body, opponent_costume_id);
        StartCoroutine(LoadOpponentCostumeSequence());
    }

    IEnumerator LoadOpponentCostumeSequence()
    {
        soundHolder.PlaySound(soundBegin);
        ShowRound1();

        float time_out_dur = 30.0f;
        while (UserDataManagerBehavior.GetInstance().opponent_skin_name == "" && time_out_dur > 0.0f)
        {
            yield return null;
            time_out_dur -= Time.deltaTime;
        }
        yield return null;
        yield return null;

        //Debug.Log("Duel mode opponent_skin_name: " + UserDataManagerBehavior.GetInstance().opponent_skin_name);
        //leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().opponent_skin_name);
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        StartCoroutine(GameOnTurn1());
    }

    IEnumerator GameOnTurn1()
    {
        Debug.Log("GameOnTurn1");

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        yield return new WaitForSeconds(3.0f);
        m_RoundPage.SetActive(false);
        ShowCountdown();
        soundHolder.PlaySound(soundCountdown);
        DisplayCountdown("3");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("2");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("1");
        yield return new WaitForSeconds(1.0f);
        ShowReady();
        soundHolder.PlaySound(soundReady);
        yield return new WaitForSeconds(3.0f);
        PrepareRound1();
        yield return new WaitForSeconds(1.0f);
        m_ImageTV.SetActive(false);
        //StartRound1();
        StartCoroutine(ListenProcess());
    }

    private void ShowRound1()
    {
        Debug.Log("ShowRound1");
        m_MainPage.SetActive(false);
        m_RoundPage.SetActive(true);
        m_CountdownPopup.SetActive(false);
        m_ReadyPopup.SetActive(false);

        m_CurrentText.text = "";
        m_NextText.text = "";
        m_RoundName.text = "ROUND 1/2";
        m_LeftPersonRoundName.text = dataOrigin.competitor_account.name;
        m_RightPersonRoundName.text = dataOrigin.user_account.name;
        m_TextRound.text = "Round 1/2";
        m_LeftPersonMainName.text = dataOrigin.competitor_account.name;
        m_RightPersonMainName.text = dataOrigin.user_account.name;

        //rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
        //leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().opponent_skin_name);

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        m_RoundProgress.fillAmount = 0;
        m_LeftScoreProgress.fillAmount = 0;
        m_RightScoreProgress.fillAmount = 0;

        StateWaiting();

        m_LeftPersonAvatar.transform.parent.parent.gameObject.SetActive(false);
        m_RightPersonAvatar.transform.parent.parent.gameObject.SetActive(false);
        StartCoroutine(GetImage(dataOrigin.competitor_account.avatar, m_LeftPersonAvatar));
        StartCoroutine(GetImage(dataOrigin.user_account.avatar, m_RightPersonAvatar));

    }

    private void ShowCountdown()
    {
        Debug.Log("ShowCountdown");
        m_MainPage.SetActive(true);
        m_RoundPage.SetActive(false);
        m_CountdownPopup.SetActive(true);
        m_ReadyPopup.SetActive(false);
    }

    private void ShowReady()
    {
        Debug.Log("ShowReady");
        m_MainPage.SetActive(true);
        m_RoundPage.SetActive(false);
        m_CountdownPopup.SetActive(false);
        m_ReadyPopup.SetActive(true);
    }

    private void PrepareRound1()
    {
        Debug.Log("PrepareRound1");
        m_MainPage.SetActive(true);
        m_RoundPage.SetActive(false);
        m_CountdownPopup.SetActive(false);
        m_ReadyPopup.SetActive(false);

        m_TextRound.text = "Round 1/2";
        m_ImageTV.SetActive(true);
    }

    private void DisplayCountdown(string value)
    {
        m_TextCountdown.text = value;
    }

    public void NextRound()
    {
        Debug.Log("NextRound");
        currentRound = 1;
        StartCoroutine(GameOnTurn2());
    }

    IEnumerator GameOnTurn2()
    {
        Debug.Log("GameOnTurn2");

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        //swap point progress
        float temp_fill_amount = m_LeftScoreProgress.fillAmount;
        m_LeftScoreProgress.fillAmount = m_RightScoreProgress.fillAmount;
        m_RightScoreProgress.fillAmount = temp_fill_amount;

        soundHolder.PlaySound(soundBegin);
        ShowRound2();
        yield return new WaitForSeconds(3.0f);
        ShowCountdown();
        soundHolder.PlaySound(soundCountdown);
        DisplayCountdown("3");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("2");
        yield return new WaitForSeconds(1.0f);
        DisplayCountdown("1");
        yield return new WaitForSeconds(1.0f);
        ShowReady();
        soundHolder.PlaySound(soundReady);
        yield return new WaitForSeconds(3.0f);
        PrepareRound2();
        yield return new WaitForSeconds(1.0f);
        m_ImageTV.SetActive(false);
        //StartRound2();
        StartCoroutine(RecordProcess());
    }

    private void ShowRound2()
    {
        Debug.Log("ShowRound2");

        m_MainPage.SetActive(false);
        m_RoundPage.SetActive(true);
        m_CountdownPopup.SetActive(false);
        m_ReadyPopup.SetActive(false);

        m_CurrentText.text = "";
        m_NextText.text = "";
        m_RoundName.text = "ROUND 2/2";
        m_LeftPersonRoundName.text = dataOrigin.user_account.name;
        m_RightPersonRoundName.text = dataOrigin.competitor_account.name;
        m_TextRound.text = "Round 2/2";
        m_LeftPersonMainName.text = dataOrigin.user_account.name;
        m_RightPersonMainName.text = dataOrigin.competitor_account.name;

        //Debug.Log("Skins: " + UserDataManagerBehavior.GetInstance().currentSkinName + " " + UserDataManagerBehavior.GetInstance().opponent_skin_name);
        //leftPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
        //rightPlayer.GetComponent<EngKidUIAvatarController>().SetSkin(UserDataManagerBehavior.GetInstance().opponent_skin_name);

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        m_RoundProgress.fillAmount = 0;
        //m_LeftScoreProgress.fillAmount = 0;
        //m_RightScoreProgress.fillAmount = 0;

        StateWaiting();
        m_LeftPersonAvatar.transform.parent.parent.gameObject.SetActive(false);
        m_RightPersonAvatar.transform.parent.parent.gameObject.SetActive(false);
        StartCoroutine(GetImage(dataOrigin.user_account.avatar, m_LeftPersonAvatar));
        StartCoroutine(GetImage(dataOrigin.competitor_account.avatar, m_RightPersonAvatar));

    }

    private void PrepareRound2()
    {
        Debug.Log("PrepareRound2");

        m_MainPage.SetActive(true);
        m_RoundPage.SetActive(false);
        m_CountdownPopup.SetActive(false);
        m_ReadyPopup.SetActive(false);

        m_TextRound.text = "Round 2/2";
        m_ImageTV.SetActive(true);
    }

    private void ShowWinLoose()
    {
        Debug.Log("ShowWinLoose");

        m_WinPopup.SetActive(true);
        //m_LoosePopup.SetActive(false);
    }

    IEnumerator GetImage(string url, Image image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f); ;
        }

        yield return null;
        image.transform.parent.parent.gameObject.SetActive(true);
    }

    IEnumerator GetAudioClip(string link)
    {
        Debug.Log("GetAudioClip: " + link + ", round: " + currentRound);

        //Debug.Log("link audio == " + link);
        if (string.IsNullOrEmpty(link) == false)
        {
            if (link.EndsWith(".wav"))
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        Debug.Log("Current round: " + currentRound);

                        if (currentRound == 0)
                        {
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }
                        else if (currentRound == 1)
                        {
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }

                        myClip = DownloadHandlerAudioClip.GetContent(www);
                        audioSource.clip = myClip;
                        audioSource.Play();
                        //Debug.Log("Audio is playing.");
                        yield return new WaitForSeconds(myClip.length);
                        Debug.Log("Current round: " + currentRound);

                        if (currentRound == 0)
                        {
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }
                        else if (currentRound == 1)
                        {
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }
                    }
                }
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        if (currentRound == 0)
                        {
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }
                        else if (currentRound == 1)
                        {
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
                        }

                        myClip = DownloadHandlerAudioClip.GetContent(www);
                        audioSource.clip = myClip;
                        audioSource.Play();
                        //Debug.Log("Audio is playing.");
                        yield return new WaitForSeconds(audioSource.clip.length);

                        if (currentRound == 0)
                        {
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
                        }
                        else if (currentRound == 1)
                        {
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                            leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
                            rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
                        }
                    }
                }
            }
        }    
    }

    private void SetupScore()
    {
        Debug.Log("SetupScore");

        int rivalNo = 0;
        int playerNo = 0;
        for (int i = 0; i < dataOrigin.rounds[0].turn_play.Length; i++)
        {
            if (dataOrigin.rounds[0].turn_play[i].is_user_turn)
            {
                playerNo = playerNo + 1;
            }
            else
            {
                rivalNo = rivalNo + 1;
            }
        }
        playerSumTurn1 = playerNo * 100f;
        rivalSumTurn1 = rivalNo * 100f;

        rivalNo = 0;
        playerNo = 0;
        for (int i = 0; i < dataOrigin.rounds[1].turn_play.Length; i++)
        {
            if (dataOrigin.rounds[1].turn_play[i].is_user_turn)
            {
                playerNo = playerNo + 1;
            }
            else
            {
                rivalNo = rivalNo + 1;
            }
        }
        playerSumTurn2 = playerNo * 100f;
        rivalSumTurn2 = rivalNo * 100f;

        //Debug.Log("chi so == " + playerSumTurn1 + " vs " + playerSumTurn2 + " vs " + rivalSumTurn1 + " vs " + rivalSumTurn2);
    }

    int currentRound = 0;
    int step = 0;
    IEnumerator ListenProcess()
    {
        Debug.Log("ListenProcess" + ", round: " + currentRound);

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);

        if (step < dataOrigin.rounds[currentRound].turn_play.Length)
        {
            m_ImageTV.SetActive(false);
            StatePlaying();

            if (currentRound == 0)
            {
                m_LeftLight.SetActive(true);
                m_RightLight.SetActive(false);

                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
            }
            else if (currentRound == 1)
            {
                m_LeftLight.SetActive(false);
                m_RightLight.SetActive(true);

                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
            }

            m_CurrentText.text = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
            if (step + 1 >= dataOrigin.rounds[currentRound].turn_play.Length)
            {
                m_NextText.text = "";
            }
            else
            {
                m_NextText.text = dataOrigin.rounds[currentRound].turn_play[step + 1].sentence.content;
            }

            //todo nobita
            SentenceAI item = new SentenceAI();
            item.result.textReference = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
            dataReview.Add(item);
            //end

            //Debug.Log("Using user record: " + dataOrigin.rounds[currentRound].turn_play[step].competitor_result.audio_url);
            if (String.IsNullOrEmpty(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.audio_url) == true)
                yield return StartCoroutine(GetAudioClip(dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.audio_high_speed.value));
            else
                yield return StartCoroutine(GetAudioClip(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.audio_url));

            if (currentRound == 0)
            {
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
            }
            else if (currentRound == 1)
            {
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
            }

            if (currentRound == 0)
            {
                if (String.IsNullOrEmpty(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score) == false)
                {
                    rivalScoreTurn1 = rivalScoreTurn1 + float.Parse(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score);
                }
                else
                {
                    rivalScoreTurn1 = rivalScoreTurn1 + (dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.score);
                }
                m_LeftScoreProgress.fillAmount = (rivalScoreTurn1 + rivalScoreTurn2) / (rivalSumTurn1 + rivalSumTurn2);
                soundHolder.PlaySound(soundScore);
            }
            else
            {
                if (String.IsNullOrEmpty(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score) == false)
                    rivalScoreTurn2 = rivalScoreTurn2 + float.Parse(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score);
                else
                    rivalScoreTurn2 = rivalScoreTurn2 + (dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.score);
                m_RightScoreProgress.fillAmount = (rivalScoreTurn1 + rivalScoreTurn2) / (rivalSumTurn2 + rivalSumTurn1);
                soundHolder.PlaySound(soundScore);
            }

            if (String.IsNullOrEmpty(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score) == false)
                opponent_score += float.Parse(dataOrigin.rounds[currentRound].turn_play[step].competitor_result.result.score);
            else
                opponent_score += (dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.score);

            step = step + 1;
            m_RoundProgress.fillAmount = (float)step / dataOrigin.rounds[currentRound].turn_play.Length;
            StartCoroutine(RecordProcess());
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
                FinishChallenge();
            }
        }
    }

    IEnumerator RecordProcess()
    {
        Debug.Log("RecordProcess, round: " + currentRound);

        if (step < dataOrigin.rounds[currentRound].turn_play.Length)
        {
            StateRecording();

            if (currentRound == 0)
            {
                m_LeftLight.SetActive(false);
                m_RightLight.SetActive(true);

                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
            }
            else if (currentRound == 1)
            {
                m_LeftLight.SetActive(true);
                m_RightLight.SetActive(false);

                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
            }

            m_CurrentText.text = dataOrigin.rounds[currentRound].turn_play[step].sentence.content;
            if (step + 1 >= dataOrigin.rounds[currentRound].turn_play.Length)
            {
                m_NextText.text = "";
            }
            else
            {
                m_NextText.text = dataOrigin.rounds[currentRound].turn_play[step + 1].sentence.content;
            }
            m_MicActive.GetComponent<FillProgress>().FillEmpty();
            soundHolder.PlaySound(soundYourTurn);
            yield return new WaitForSeconds(3f);
            if (currentRound == 0)
            {
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
            }
            else if (currentRound == 1)
            {
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
            }

            m_MicActive.GetComponent<FillProgress>().LockTime(dataOrigin.rounds[currentRound].turn_play[step].sentence.duration);

            RecordVoice(dataOrigin.rounds[currentRound].turn_play[step].sentence);

            yield return new WaitForSeconds(dataOrigin.rounds[currentRound].turn_play[step].sentence.duration);
            if (currentRound == 0)
            {
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
            }
            else if (currentRound == 1)
            {
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
                rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
                leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
            }

            //if (currentRound == 0)
            //{
            //    playerScoreTurn1 = playerScoreTurn1 + dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.score;
            //    m_RightScoreProgress.fillAmount = playerScoreTurn1 / playerSumTurn1;
            //}
            //else
            //{
            //    playerScoreTurn2 = playerScoreTurn2 + dataOrigin.rounds[currentRound].turn_play[step].sentence.sample.score;
            //    m_LeftScoreProgress.fillAmount = playerScoreTurn2 / playerSumTurn2;
            //}

            //step = step + 1;
            //m_RoundProgress.fillAmount = (float)step / dataOrigin.rounds[currentRound].turn_play.Length;
            //StartCoroutine(ListenProcess());
        }
        else
        {
            //Debug.Log("khong ghi am duoc nua");
            yield return new WaitForSeconds(1f);
            if (currentRound + 1 < dataOrigin.rounds.Length)
            {
                currentRound = currentRound + 1;
                step = 0;
                StartCoroutine(GameOnTurn2());
            }
            else
            {
                //Debug.Log("Ket thuc duoc roi tu ghi am");
                FinishChallenge();
            }
        }
    }

    public void RecordVoice(Sentence sentenceOrigin)
    {
        Debug.Log("RecordVoice");

        audioSource.Stop();
        StartCoroutine(EvaluationSequence(sentenceOrigin));
    }

    float player_score = 0.0f;
    float opponent_score = 0.0f;
    IEnumerator EvaluationSequence(Sentence sentenceOrigin)
    {
        Debug.Log("EvaluationSequence");

        AuthorizationManager.GetInstance().TriggerMicroPopUp();
        while(AuthorizationManager.GetInstance().isMicPermitted == false)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.125f);

        AudioClip record;
        record = Microphone.Start(null, false, (int)sentenceOrigin.duration, 16000);
        yield return new WaitForSeconds(sentenceOrigin.duration);

        // todo nobita save file
        SaveMav.Save("record.wav", record);

        // todo nobita get score from AI
        WWWForm audio_validation_form = new WWWForm();

        audio_validation_form.AddField("token", "gcxpHQmLeVwLWobE6apU1lgAg49YTMa0");
        audio_validation_form.AddBinaryData("audio-file", WavUtility.FromAudioClip(record), "record.wav");
        audio_validation_form.AddField("text-refs", sentenceOrigin.content);

        StateWaiting();
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
                //Debug.Log(validation.result.score);

                float voiceScore = (float)Math.Round(validation.result.score, 2);
                player_score += voiceScore;

                if (currentRound == 0)
                {
                    playerScoreTurn1 = playerScoreTurn1 + voiceScore;
                    m_RightScoreProgress.fillAmount = (playerScoreTurn1 + playerScoreTurn2) / (playerSumTurn1 + playerSumTurn2);
                    soundHolder.PlaySound(soundScore);
                }
                else
                {
                    playerScoreTurn2 = playerScoreTurn2 + voiceScore;
                    m_LeftScoreProgress.fillAmount = (playerScoreTurn2 + playerScoreTurn1) / (playerSumTurn2 + playerSumTurn1);
                    soundHolder.PlaySound(soundScore);
                }

                // todo NOBITA send to BACKEND
                //Debug.Log("nobita AI result == " + audio_validation_request.downloadHandler.text);
                //Debug.Log("nobita record == " + dataOrigin.record_id);
                //Debug.Log("nobita sentence == " + dataOrigin.rounds[currentRound].turn_play[step].sentence._id);
                //Debug.Log("nobita token == " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);

                SpeakingAI speakingAi = new SpeakingAI();
                speakingAi.ai_result = audio_validation_request.downloadHandler.text;
                speakingAi.record_id = dataOrigin.record_id;
                speakingAi.sentence_id = dataOrigin.rounds[currentRound].turn_play[step].sentence._id;

                dataReview.Add(validation);

                SendAIResultToBackend(speakingAi);

                step = step + 1;
                m_RoundProgress.fillAmount = (float)step / dataOrigin.rounds[currentRound].turn_play.Length;
                StartCoroutine(ListenProcess());

            }

        }
        yield return new WaitForSeconds(0.5f);

    }

    public void SendAIResultToBackend(SpeakingAI data)
    {
        Debug.Log("SendAIResultToBackend");

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

    private void StatePlaying()
    {
        m_MicActive.SetActive(false);
        m_MicInactive.SetActive(true);
        m_MicLock.SetActive(false);
    }

    private void StateRecording()
    {
        m_MicActive.SetActive(true);
        m_MicInactive.SetActive(false);
        m_MicLock.SetActive(false);
    }

    private void StateWaiting()
    {
        m_MicActive.SetActive(false);
        m_MicInactive.SetActive(false);
        m_MicLock.SetActive(true);
    }

    private void StateInit()
    {
        m_MicActive.SetActive(false);
        m_MicInactive.SetActive(false);
        m_MicLock.SetActive(true);
    }

    private void FinishChallenge()
    {
        Debug.Log("FinishChallenge");
        Debug.Log("Player final score: " + player_score + " vs Opponent score: " + opponent_score);
        Debug.Log("Player fill: " + m_LeftScoreProgress.fillAmount + " vs Opponent fill: " + m_RightScoreProgress.fillAmount);

        soundHolder.PlaySound(soundWaitingResult);
        FinalPractice record = new FinalPractice(dataOrigin.record_id);
        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_DUEL_FINAL,
            JsonUtility.ToJson(record),
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get duel final detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get duel final detail.");
                //Debug.Log("diem xp == " + server_reply.data.gained_xp);
                //Debug.Log("thang hay thua == " + server_reply.data.is_winner);

                RefreshCoin();

                if (server_reply.data.is_winner)
                {
                    ProcessWin();
                }
                else
                {
                    ProcessLoose();
                }

                //if (player_score >= opponent_score)
                //{
                //    ProcessWin();
                //}
                //else
                //{
                //    ProcessLoose();
                //}
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    private void ProcessWin()
    {
        StartCoroutine(WinAction());
    }

    private void ProcessLoose()
    {
        StartCoroutine(LooseAction());
    }

    IEnumerator WinAction()
    {
        m_LeftLight.SetActive(true);
        m_LeftWinLight.SetActive(true);
        m_LeftWin.SetActive(true);
        m_RightLight.SetActive(false);
        m_RightWinLight.SetActive(false);
        m_RightWin.SetActive(false);

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");

        soundHolder.PlaySound(soundWin);
        yield return new WaitForSeconds(4.3f);
        m_WinPopup.SetActive(true);
        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("interactive_zone", "on_win");
        SendEventFirebase.SendEventInteractiveChallenge(m_InteractiveZoneManager.currentLesson.title, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
    }

    IEnumerator LooseAction()
    {
        m_LeftLight.SetActive(false);
        m_LeftWinLight.SetActive(false);
        m_LeftWin.SetActive(false);
        m_RightLight.SetActive(true);
        m_RightWinLight.SetActive(true);
        m_RightWin.SetActive(true);

        rightPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark");
        leftPlayer.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_dark", 1);

        soundHolder.PlaySound(soundLoose);
        yield return new WaitForSeconds(3.4f);
        m_LoosePopup.SetActive(true);
        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("interactive_zone", "on_loose");
        SendEventFirebase.SendEventInteractiveChallenge(m_InteractiveZoneManager.currentLesson.title, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "loss");
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
    }

    public void Refresh()
    {
        m_LeftLight.SetActive(false);
        m_LeftWinLight.SetActive(false);
        m_LeftWin.SetActive(false);
        m_RightLight.SetActive(false);
        m_RightWinLight.SetActive(false);
        m_RightWin.SetActive(false);
    }

    public void OnPopupReviewClick()
    {
        m_ReviewPage.SetActive(true);
        m_WinPopup.SetActive(false);
        m_LoosePopup.SetActive(false);

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

        m_ReviewPage.GetComponent<ReplayPageScript>().Setup(reviewList);
    }

    public void OnPopupExitClick()
    {
        m_WinPopup.SetActive(false);
        m_LoosePopup.SetActive(false);
        m_InteractiveZoneManager.BackFromDuelPopup();
    }

    public void RefreshAllData()
    {
        // refresh data
        currentRound = 0;
        step = 0;
        dataReview = new List<SentenceAI>();

        playerScoreTurn1 = 0;
        rivalScoreTurn1 = 0;
        playerScoreTurn2 = 0;
        rivalScoreTurn2 = 0;
        playerSumTurn1 = 0;
        playerSumTurn2 = 0;
        rivalSumTurn1 = 0;
        rivalSumTurn2 = 0;

        player_score = 0.0f;
        opponent_score = 0.0f;

        // refresh UI
        m_TextRound.text = "";
        m_CurrentText.text = "";
        m_NextText.text = "";

        StopAllCoroutines();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void OnBackClick()
    {
        m_ConfirmPopup.SetActive(true);
    }

    public void OnConfirmBack()
    {
        m_ConfirmPopup.SetActive(false);
        RefreshAllData();
        m_InteractiveZoneManager.BackFromDuelMode();
        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("interactive_zone", "on_confirm_back");
        SendEventFirebase.SendEventInteractiveChallenge(m_InteractiveZoneManager.currentLesson.title, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "exit");
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
    }

    public void OnConfirmCancel()
    {
        m_ConfirmPopup.SetActive(false);
    }

    public void RefreshCoin()
    {
        m_InteractiveZoneManager.GetKidInfoFromServer();
    }
}
