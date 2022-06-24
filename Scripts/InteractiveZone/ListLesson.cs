using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ListLesson : MonoBehaviour
{
    [Header("Manager")]
    public InteractiveZoneManager m_InteractiveZoneManager;

    public GameObject LessonCell;
    public GameObject LessonListContainer;
    public GameObject PopupDetail;
    public GameObject Intro;

    [Header("Audio")]
    public SoundHolder soundHolder;
    public AudioClip backAudio;
    public AudioClip introAudio;
    public AudioClip itemClick;
    public AudioClip introPracticeAudio;

    [Header("Popup Detail")]
    public Text m_PopupTitle;
    public Text m_PopupDes;
    public Image m_PopupThumb;

    [Header("UI Control")]
    public EngKidUIAvatarController playerLessonListAnim;
    //public Animator LeftCurtain;
    //public Animator RightCurtain;
    public GameObject curtain;

    private ReturnedData dataServer;
    private UsableDataInfo currentLesson;
    List<Texture2D> texList = new List<Texture2D>();
    List<GameObject> cellList = new List<GameObject>();
    private UsableDataInfo[] _UsableDataInfos;

    private void OnEnable()
    {
        m_InteractiveZoneManager.RefeshKidInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        curtain.SetActive(true);
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_close", loop: true);
        GetListLessonFromServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PrepareStage()
    {
        GetListLessonFromServerA();

        //LeftCurtain.Play("curtain_left");
        //RightCurtain.Play("curtain_right");
        //yield return new WaitForSeconds(3f);
        //LeftCurtain.enabled = false;
        //RightCurtain.enabled = false;
        //PlayAnimIntro();
    }

    int ping = 0;
    public void LoadingPing()
    {
        ping++;
        //if (ping == LessonListContainer.transform.childCount)
        //    StartCoroutine(StartStage());
    }

    IEnumerator StartStage()
    {
        //LeftCurtain.speed = 1.0f;
        //RightCurtain.speed = 1.0f;
        //LeftCurtain.Play("curtain_left");
        //RightCurtain.Play("curtain_right");
        //curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "out", loop: false);
        yield return new WaitForSeconds(1.0f);
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_open", loop: true);
        //LeftCurtain.enabled = false;
        //RightCurtain.enabled = false;
        PlayAnimIntro();
        //hiển thị đối tượng khi 
        
    }

    public void GetListLessonFromServer()
    {
        ping = 0;
        //LeftCurtain.speed = 0.0f;
        //RightCurtain.speed = 0.0f;
        PrepareStage();
        //Debug.Log("lay danh sach bai hoc");
        //DataBaseInterface.GetInstance().GetJSONRequest(
        //    DataBaseInterface.INTERACTIVE_ZONE_GET_LIST_LESSON,
        //    callback_flag =>
        //    {
        //        if (callback_flag == false)
        //            Debug.Log("Error: get lesson detail.");
        //    },
        //    server_reply =>
        //    {
        //        Debug.Log("Success: get lesson detail." + server_reply.data.list[0].title);
        //        DisplayLesson(server_reply.data);
        //    },
        //    UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
        //    );
        //soundHolder.PlaySound(introAudio);
    }

    public void GetListLessonFromServerA()
    {
        //Debug.Log("lay danh sach bai hoc");
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_GET_LIST_LESSON,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get lesson detail.");
            },
            server_reply =>
            {
                Debug.Log("Success: get lesson detail." + server_reply.data.list[0].title);
                StartCoroutine(DisplayLesson(server_reply.data));
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );

    }

    public IEnumerator DisplayLesson(ReturnedData data)
    {
        dataServer = data;
        cellList.Clear();
        _UsableDataInfos = data.list;
        for (int i = 0; i < data.list.Length; i++)
        {
            //Debug.Log("Success: get lesson " + data.list[i].title);
            GameObject cell = Instantiate(LessonCell, LessonListContainer.transform);

            for (int k = 0; k < data.account_records.Length; k++)
            {
                if (data.list[i]._id.Equals(data.account_records[k].lesson_id))
                {
                    data.list[i].isLearn = true;
                    break;
                }
            }

            cell.GetComponent<SpeakingLesson>().listLesson = this;
            cell.GetComponent<SpeakingLesson>().SetLesson(data.list[i]);
            /*yield return */StartCoroutine(cell.GetComponent<SpeakingLesson>().SetupLesson());
            cell.GetComponent<Button>().onClick.AddListener(() =>
            {
                soundHolder.PlaySound(itemClick);

                if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.INTERACTIVE_ZONE_PERMISSION) == true)
                {
                    currentLesson = cell.GetComponent<SpeakingLesson>().GetLesson();
                    StartCoroutine(ShowPopupDetail());
                }
                else if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.TRIAL_PERMISSION) == true)
                {
                    if (cell.GetComponent<SpeakingLesson>().isFreemium == true)
                    {
                        currentLesson = cell.GetComponent<SpeakingLesson>().GetLesson();
                        StartCoroutine(ShowPopupDetail());
                    }
                    else
                    {
                        PopupManagerBehavior.GetInstance().TriggerPremiumPopUp();
                        SendEventFirebase.SendEventProductCheckout("interactive_zone");
                        /*
                        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
                    }
                }
                else
                {
                    PopupManagerBehavior.GetInstance().TriggerPremiumPopUp();
                    SendEventFirebase.SendEventProductCheckout("interactive_zone");
                    /*
                    SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
                }
            });
            cellList.Add(cell);

            yield return null;
        }

        foreach (GameObject go in cellList)
        {
            go.GetComponent<SpeakingLesson>().isFreemium = false;
        }    
        cellList[0].GetComponent<SpeakingLesson>().isFreemium = true;

        yield return StartCoroutine(StartStage());
        if (SceneManagerBehavior.GetInstance().StoryId != "")
        {
            yield return StartCoroutine(ShowPopupByStory(SceneManagerBehavior.GetInstance().StoryId));
            SceneManagerBehavior.GetInstance().StoryId = "";
        }
    }
    public UsableDataInfo FindByLesson(string id)
    {
        UsableDataInfo _usableDataInfo=null;
        for (int i = 0; i < _UsableDataInfos.Length; i++)
        {
            if (_UsableDataInfos[i]._id == id)
            {
                _usableDataInfo = _UsableDataInfos[i];
            }
        }
        return _usableDataInfo;
    }
    public IEnumerator ShowPopupByStory(string id)
    {
        UsableDataInfo _usableDataInfo = FindByLesson(id);
        if (_usableDataInfo != null)
        {
            currentLesson = _usableDataInfo;
            soundHolder.PlaySound(itemClick);
            StartCoroutine(ShowPopupDetail());
        }
        yield return null;
    }
    public IEnumerator ShowPopupDetail()
    {
        foreach (GameObject cell in cellList)
        {
            cell.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        //if (curtain.GetComponent<SkeletonGraphic>().AnimationState.GetCurrent(0).ToString().Contains("idle_open") == true)
        //{
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_close", loop: true);
        yield return new WaitForSeconds(1.0f);

        PopupDetail.SetActive(true);
        m_PopupTitle.text = currentLesson.title;
        m_PopupDes.text = currentLesson.description.value;
        Debug.Log("ShowPopupDetail");
        yield return StartCoroutine(GetImage(currentLesson.detail_thumbnail.value, m_PopupThumb));

        //curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "out", loop: false);
        //int isFirstTime = PlayerPrefs.GetInt("E_FIRST_TIME_SPEAKING_CLICK_LESSON", 0);
        //if (isFirstTime == 1)
        //{
        //    soundHolder.CancelSound();
        //    Intro.SetActive(false);
        //}
        //else
        //{
        //    soundHolder.CancelSound();
        //    Intro.SetActive(true);
        //    soundHolder.PlaySound(introPracticeAudio);
        //}

        yield return new WaitForSeconds(1.0f);
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_open", loop: true);
        yield return new WaitForSeconds(0.25f);

        int isFirstTime = PlayerPrefs.GetInt("E_FIRST_TIME_SPEAKING_CLICK_LESSON", 0);
        if (isFirstTime == 1)
        {
            soundHolder.CancelSound();
            Intro.SetActive(false);
        }
        else
        {
            soundHolder.CancelSound();
            Intro.SetActive(true);
            soundHolder.PlaySound(introPracticeAudio);
        }
        //}
        //
        foreach (GameObject cell in cellList)
        {
            cell.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentLesson = cell.GetComponent<SpeakingLesson>().GetLesson();
                soundHolder.PlaySound(itemClick);
                //StartCoroutine(ShowPopupDetail());

                if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.INTERACTIVE_ZONE_PERMISSION) == true)
                {
                    currentLesson = cell.GetComponent<SpeakingLesson>().GetLesson();
                    StartCoroutine(ShowPopupDetail());
                }
                else if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.TRIAL_PERMISSION) == true)
                {
                    if (cell.GetComponent<SpeakingLesson>().isFreemium == true)
                    {
                        currentLesson = cell.GetComponent<SpeakingLesson>().GetLesson();
                        StartCoroutine(ShowPopupDetail());
                    }
                    else
                    {
                        PopupManagerBehavior.GetInstance().TriggerPremiumPopUp();
                    }
                }
                else
                {
                    PopupManagerBehavior.GetInstance().TriggerPremiumPopUp();
                }
            });
        }
    }

    IEnumerator GetImage(string url, Image image)
    {
        //Debug.Log("hien anh == " + currentLesson.index_thumbnail.value);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            texList.Add(tex);
        }
    }

    private void OnDestroy()
    {
        foreach (Texture2D tex in texList)
        {
            if (tex != null)
                Destroy(tex);
        }
    }

    public void MarkFirstTime()
    {
        PlayerPrefs.SetInt("E_FIRST_TIME_SPEAKING_CLICK_LESSON", 1);
        PlayerPrefs.Save();
        soundHolder.CancelSound();
        Intro.SetActive(false);
    }

    public void GoToPracticePage()
    {
        Debug.Log("Go to practive page!");
        m_InteractiveZoneManager.currentLesson = currentLesson;
        m_InteractiveZoneManager.ShowPracticePage();
        UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);
    }

    public void GoToDuelListPage()
    {
        m_InteractiveZoneManager.currentLesson = currentLesson;
        m_InteractiveZoneManager.ShowDuelListPage();
    }

    IEnumerator LoopPlayerSpeaking()
    {
        //while (true)
        //{
            playerLessonListAnim.PlayAnimationLooped("idle_with_mic", 1);
            playerLessonListAnim.PlayAnimationLooped("idle_with_mic");
            yield return new WaitForSeconds(1.4f);
            playerLessonListAnim.PlayAnimationLooped("talk", 1);
            soundHolder.PlaySound(introAudio);
            playerLessonListAnim.PlayAnimationLooped("idle_talk");
            yield return new WaitForSeconds(3.2f);
            playerLessonListAnim.PlayAnimationLooped("idle_with_mic");
            playerLessonListAnim.PlayAnimationLooped("idle_with_mic", 1);
            yield return new WaitForSeconds(10f);
        //}
    }

    public void PlayAnimIntro()
    {
        //Debug.Log("vao intro");
        StartCoroutine(LoopPlayerSpeaking());
    }
}
