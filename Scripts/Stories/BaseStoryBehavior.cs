using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stories;
using UniRx;
using UnityEngine.AddressableAssets;

public class BaseStoryBehavior : MonoBehaviour
{
    [Header("Story Windows")]
    public GameObject m_InfoWindow;
    public GameObject m_StoryWindow;
    public GameObject m_ContinueReadingWindow;
    private Text m_ContinueReadingMsgTxt;

    [Header("Book Settings")]
    private GameObject m_Book;
    public Sprite m_EmptyPage;

    [Header("Result Popup")]
    public GameObject m_ResultPopup;

    [Header("Audio")]
    public AudioSource m_AudioSource;
    private float m_ReadingAudioVolume = 0.0f; 
    public GameObject m_AudioOnImg;
    public GameObject m_AudioOffImg;

    [Header("Tutorial")]
    public GameObject m_TutorialWindow;

    [Header("Lesson prefab of this Activity")]
    public AssetReference m_Lesson;

    [Header("Nav override")]
    public AssetReference m_NextScene;

    [HideInInspector] public bool isSoundEnabled = false;
    float inactivityTimer = 7.0f;
    bool isInactiveTimerOn = true;
    private StorySoundManager soundManager;
    private BookCurlPro.AutoFlip autoFlip;
    private BookCurlPro.BookPro bookPro;
    [HideInInspector] public LessonDetailInfo lessonDetail = null;

    bool isReadToMe;
    void Awake()
    {
        if(Debug.isDebugBuild) Debug.Log("BaseStoryBehavior Loaded");
        m_ContinueReadingMsgTxt = m_ContinueReadingWindow.transform.GetChild(2).transform.GetChild(2).GetComponent<Text>();
    }
    private void Start()
    {
        fixRatio();
        GlobalCurtain.GetInstance().OpenCurtain();
        /*
        m_InfoWindow.SetActive(true);
        m_StoryWindow.SetActive(false);
        m_ResultPopup.SetActive(false);
        m_ContinueReadingWindow.SetActive(false);
        m_TutorialWindow.SetActive(false);
        */
        //m_AudioOnImg.SetActive(true);
        //m_AudioOffImg.SetActive(false);
        m_ReadingAudioVolume = m_AudioSource.volume;
        m_AudioSource.playOnAwake = false;
        m_Book = m_StoryWindow.transform.GetChild(0).gameObject;
        isInactiveTimerOn = true;
        soundManager = transform.GetChild(1).GetComponent<StorySoundManager>();
        bookPro = m_Book.GetComponent<BookCurlPro.BookPro>();
        LeanTween.addListener(gameObject,StoryEvent.TouchPage,OnEventTouchPage);
        LeanTween.addListener(gameObject, StoryEvent.Tutorial, OnEventTutorial);
        LeanTween.addListener(gameObject, StoryEvent.Hightline, OnEventSoundHighline);
        if (m_Book.GetComponent<BookCurlPro.AutoFlip>() == null)
        {
            m_Book.AddComponent<BookCurlPro.AutoFlip>(); 
            autoFlip = m_Book.GetComponent<BookCurlPro.AutoFlip>();
            autoFlip.AutoStartFlip = false;
            autoFlip.PageFlipTime = 0.25f;
        }
        BGMManagerBehavior.GetInstance().PauseBGM();
        //m_ResultPopup.gameObject.SetActive(false);
    }
    IEnumerator callAPI()
    {
        yield return 0.5f;
        DataBaseInterface.GetInstance().GetJSONRequest(
           DataBaseInterface.GET_LESSON_DETAIL_URI + UserDataManagerBehavior.GetInstance().currentSelectedLessonID,
           callback_flag =>
           {
               if (callback_flag == false)
                   Debug.Log("Error: get lesson detail failed.");
           },
           server_reply =>
           {
               lessonDetail = server_reply.data.lesson;
               if(lessonDetail.activities.Length>=1) UserDataManagerBehavior.GetInstance().currentSelectedActivityID = lessonDetail.activities[0]._id;
           },
           UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
           );
    }
    private void OnEnable()
    {
        m_InfoWindow.SetActive(true);
        m_StoryWindow.SetActive(false);
        m_ResultPopup.SetActive(false);
        m_ContinueReadingWindow.SetActive(false);
        m_TutorialWindow.SetActive(false);
        isInactiveTimerOn = true;
    }

    private void OnDisable()
    {
        LeanTween.cancel(m_ResultPopup);
        LeanTween.cancel(m_StoryWindow);
    }
    /*
    private void Update()
    {
        if (isInactiveTimerOn == true)
        {
            if (m_StoryWindow.activeSelf)
            {
                inactivityTimer -= Time.deltaTime;
            }  
            else if (m_TutorialWindow.activeSelf)
            {
                inactivityTimer = 7.0f;
                m_TutorialWindow.SetActive(false);
            }
            if (inactivityTimer <= 0.0f)
            {
                inactivityTimer = 7.0f;
                m_TutorialWindow.SetActive(true);
            }
        }
    }
    */
    public void OnReadToMeClicked(bool is_sound_enable)
    {
        m_StoryWindow.transform.GetChild(1).gameObject.SetActive(true);
        m_StoryWindow.transform.GetChild(2).transform.localScale = Vector3.one;
        isInactiveTimerOn = true; 
        OnStartCountingInactivity();
        LeanTween.delayedCall(0.5f, () => { soundManager.playEffect(6); });
        StartCoroutine(OpenStorySequence(is_sound_enable));

        UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);
        if (is_sound_enable)
        {
            isReadToMe = true;
            SendEventFirebase.SendEventActivityAccess(gameObject.name, "story", "read_to_me");
            /*
            SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
        }
        else
        {
            isReadToMe = false;
            SendEventFirebase.SendEventActivityAccess(gameObject.name, "story", "read_by_me");
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

    public void OnContinueReading()
    {
        StartCoroutine(ContinueReadingSequence());
    }

    public void OnStartCountingInactivity()
    {
        inactivityTimer = 7.0f;

        m_TutorialWindow.SetActive(false);

    }

    public void TurnOffInactiveTimer()
    {
        isInactiveTimerOn = false;
    }

    IEnumerator ContinueReadingSequence()
    {
        yield return new WaitForSeconds(0.5f);
        int page = PlayerPrefs.GetInt(this.gameObject.name + "_CurrentPage");
        BookCurlPro.AutoFlip flipper;
        if(m_Book.GetComponent<BookCurlPro.AutoFlip>()==null)
        {
            m_Book.AddComponent<BookCurlPro.AutoFlip>();
        }
        flipper = m_Book.GetComponent<BookCurlPro.AutoFlip>();
        bookPro.interactable = false;
        flipper.ControledBook = bookPro;
        flipper.AutoStartFlip = false;
        flipper.PageFlipTime = 0.25f;
        flipper.TimeBetweenPages = 0.1f;
        flipper.StartFlipping(page);
        yield return new WaitForSeconds(page * 0.25f + page * 0.1f + 0.5f);
        Destroy(flipper);
        bookPro.interactable = true;
    }

    public void OnBackToInfoWindow()
    {
        m_InfoWindow.SetActive(true);
        m_StoryWindow.SetActive(false);
        m_ResultPopup.SetActive(false);
        m_ContinueReadingWindow.SetActive(false);
        //save current page
        if (bookPro.CurrentPaper > 1)
        {
            PlayerPrefs.SetInt(this.gameObject.name + "_CurrentPage", bookPro.currentPaper);
            PlayerPrefs.SetInt(this.gameObject.name + "_isReadToMe", isSoundEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
        bookPro.CurrentPaper = 1;

        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("game_activity", "on_back");

        if(isReadToMe)
        {
            SendEventFirebase.SendEventActivityPlay(gameObject.name, "story_read_to_me", UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
            /*
            SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
        }
        else
        {
            SendEventFirebase.SendEventActivityPlay(gameObject.name, "story_read_by_me",
                UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
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
    public void Pause()
    {
        bookPro.papers.ToObservable().Subscribe(x => {
            if (x.Front.GetComponent<StoryPageBehavior>()!= null) x.Front.GetComponent<StoryPageBehavior>().StopAll();
            if (x.Back.GetComponent<StoryPageBehavior>() != null) x.Back.GetComponent<StoryPageBehavior>().StopAll();
        });
    }
    public void ToggleStoryBGM(bool IsOn)
    {
        if (IsOn)
        {
            m_AudioOnImg.SetActive(false);
            m_AudioOffImg.SetActive(true);
            m_AudioSource.volume = 0;
        }    
        else
        {
            m_AudioSource.volume = m_ReadingAudioVolume;
            m_AudioOnImg.SetActive(true);
            m_AudioOffImg.SetActive(false);
        }

    }
    IEnumerator OpenStorySequence(bool is_sound_enable)
    {
        isSoundEnabled = is_sound_enable;
        bookPro.papers.ToObservable().Subscribe(x => {
        x.Front.SetActive(true);
        x.Back.SetActive(true);
        });
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        Sprite full_screen = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //fade audio out
        /*
        float vol = m_AudioSource.volume;
        LeanTween.value(this.gameObject, vol, m_ReadingAudioVolume, 0.5f).setOnUpdate((float val) => {
            m_AudioSource.volume = val;
        });*/
        // do something with texture
        yield return null;
        m_StoryWindow.SetActive(true);
        m_StoryWindow.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        LeanTween.scale(m_StoryWindow, Vector3.one, 0.5f).setOnComplete(() => {
            m_InfoWindow.SetActive(false);
            //check if reading is in progress, then ask to continue
            if (PlayerPrefs.HasKey(this.gameObject.name + "_CurrentPage") && PlayerPrefs.HasKey(this.gameObject.name + "_isReadToMe"))
            {
                bool isSoundEnabledCurent = PlayerPrefs.GetInt(this.gameObject.name + "_isReadToMe") == 1 ? true : false;
                if (isSoundEnabled == isSoundEnabledCurent)
                {
                    int currentPage = PlayerPrefs.GetInt(this.gameObject.name + "_CurrentPage")-1;
                    int totalPage = bookPro.papers.Length-3;
                    m_ContinueReadingMsgTxt.text = "Bạn đang đọc dở <b>trang " + currentPage + "/"+ totalPage + "</b>. Bạn có muốn đọc tiếp không?";
                    m_ContinueReadingWindow.SetActive(true);
                    bookPro.CurrentPaper = 1;
                }
            }

        });

        // cleanup
        yield return null;
        Object.Destroy(texture);
    }   
    void fixRatio()
    {
        float ratio = Screen.width / Screen.height;
        CanvasScaler canvasScaler = transform.GetChild(0).GetComponent<CanvasScaler>();
        canvasScaler.matchWidthOrHeight = ratio >1.5f ? 0 : 1;
    }
    public void OnEndOfStory()
    {
        Debug.Log("End of story reached.");
        StartCoroutine(DelayedShowResultPopup());
    }

    IEnumerator DelayedShowResultPopup()
    {
        //fade audio in
        /*
        LeanTween.value(this.gameObject, m_ReadingAudioVolume, 0.0f, 1.0f).setOnUpdate((float val) => {
            m_AudioSource.volume = val;
        });*/
        callAPI();
        m_ResultPopup.SetActive(true);
        m_StoryWindow.transform.GetChild(1).gameObject.SetActive(false);
        m_StoryWindow.transform.GetChild(2).transform.localScale = Vector3.zero;
        m_ResultPopup.transform.GetChild(1).transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        LeanTween.scale(m_ResultPopup.transform.GetChild(1).gameObject,Vector3.one, 0.5f).setOnComplete(() => {
            if (isReadToMe)
            {
                m_ResultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(3, true, "read_to_me");
            }
            else
            {
                m_ResultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(3, true, "read_by_me");
            }
           
        });
        //lưu điểm theo lesson id
        Debug.Log("id:" + UserDataManagerBehavior.GetInstance().currentSelectedLessonID);
        StoryComplete storyComplete = new StoryComplete(UserDataManagerBehavior.GetInstance().currentSelectedLessonID);
        var json = JsonUtility.ToJson(storyComplete);
        //DataBaseInterface.GetInstance().PostJSONRequest(
        //        DataBaseInterface.LESSON_COMPLETE,
        //        JsonUtility.ToJson(json),
        //        null,
        //        null,
        //        UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);
        PlayerPrefs.DeleteKey(this.gameObject.name + "_CurrentPage");
        PlayerPrefs.DeleteKey(this.gameObject.name + "_isReadToMe");
    }

    public void OnReturnToActivityScreen()
    {
        soundManager.play(0);
        LeanTween.delayedCall(0.3f,() => {
            LeanTween.removeListener(gameObject, StoryEvent.TouchPage, OnEventTouchPage);
            LeanTween.removeListener(gameObject, StoryEvent.Tutorial, OnEventTutorial);
            LeanTween.removeListener(gameObject, StoryEvent.Hightline, OnEventSoundHighline);
            LeanTween.cancelAll();
            // Destroy(this.gameObject);
            BGMManagerBehavior.GetInstance().ResumeBGM();
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_Lesson, this.gameObject));
        });
    }

    public void OnCompleteStory()
    {
        soundManager.play(0);
        LeanTween.delayedCall(0.3f, () => {
            LeanTween.removeListener(gameObject, StoryEvent.TouchPage, OnEventTouchPage);
            LeanTween.removeListener(gameObject, StoryEvent.Tutorial, OnEventTutorial);
            LeanTween.removeListener(gameObject, StoryEvent.Hightline, OnEventSoundHighline);
            LeanTween.cancelAll();
            // Destroy(this.gameObject);
            BGMManagerBehavior.GetInstance().ResumeBGM();
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_NextScene, this.gameObject));
        });
    }

    //Xử lý sự kiện vuốt trái vuốt phải 
    private void OnEventTouchPage(LTEvent e)
    {
        bookPro.papers.ToObservable().Subscribe(xy => {
            xy.Front.GetComponentsInChildren<TapStoryBehavior>().ToObservable().Subscribe(x => x.callOff());
            xy.Back.GetComponentsInChildren<TapStoryBehavior>().ToObservable().Subscribe(x => x.callOff());
        });
        switch (e.data as string)
        {
            case StoryEvent.PAGELEFT:
                if(bookPro.CurrentPaper>1) 
                    autoFlip.FlipLeftPage();
                OnStartCountingInactivity();
                break;
            case StoryEvent.PAGERIGHT:
                if (bookPro.CurrentPaper <= bookPro.papers.Length-1) 
                    autoFlip.FlipRightPage();
                OnStartCountingInactivity();
                break;
            default:
                break;
        }
    }
    private void OnEventTutorial(LTEvent e)
    {
        //OnStartCountingInactivity();
        //inactivityTimer = 7.0f;
        m_TutorialWindow.SetActive(true);
    }
    private void OnEventSoundHighline(LTEvent e)
    {
        if (bookPro.currentPaper >= 1)
        {
            bookPro.papers[bookPro.currentPaper].Front.GetComponentsInChildren<TapStoryBehavior>().ToObservable().Subscribe(x => x.callOn());
            bookPro.papers[bookPro.currentPaper - 1].Back.GetComponentsInChildren<TapStoryBehavior>().ToObservable().Subscribe(x => x.callOn());
        }
    }
}
[System.Serializable]
public class StoryComplete
{
    [SerializeField] public string lesson_id = "";

    public StoryComplete(string id)
    {
        lesson_id = id;
    }
}
