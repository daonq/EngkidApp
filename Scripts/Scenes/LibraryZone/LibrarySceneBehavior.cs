using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LibrarySceneBehavior : MonoBehaviour
{
    [Header("Sound effects")]
    public AudioClip m_BGM;

    [Header("UI")]
    public Canvas mainCanvas;
    public Canvas uiCanvas;
    public Canvas bookCanvas;
    public Canvas resultCanvas;
    public Transform storyInfosHolder;
    public GameObject storyInfoPrefab;
    public GameObject downloadBtn;
    public GameObject downloadIndicator;
    public GameObject readBtn;
    public Text downloadProgressTxt;

    [Header("Popup")]
    public GameObject storyDetailPopUp;
    public Image detailedThumbnail;
    public Text storyNameTxt;
    public Text authorTxt;
    public Text artistTxt;
    public Text publisherTxt;
    public GameObject resultPopUp;
    public Transform recommendedBooksHolder;

    [Header("Book setting")]
    public GameObject bookTemplate;
    public FreeBookDialogBehavior dialogBehavior;
    public List<FreeBookPageData> currentBookPagesList = new List<FreeBookPageData>();

    [Header("Avatar and Info")]
    public EngKidUIAvatarController avatarController;
    public Image avatarImg;
    public Text nameTxt;
    public Text expTxt;
    public AudioSource avatarAudioSource;
    public AudioClip openingClip;
    public float idleDuration = 15.0f;

    [Header("Curtain")]
    public Spine.Unity.SkeletonGraphic curtain;

    //internal
    bool loadingCheck = false;
    Texture2D tex = null;
    int loadingProgress = 0;
    [HideInInspector] public List<LibraryZoneStoryData> storyDataList = new List<LibraryZoneStoryData>();
    [HideInInspector] public int clicked_book_index = -1;
    Coroutine downloadCor = null;
    [HideInInspector] public string currentBookID = "";

    private void OnEnable()
    {
        BGMManagerBehavior.GetInstance().PlayBGM(m_BGM);

        //TODO: adjust canvas based on screen ratio
        if (Camera.main.aspect >= 1.7)
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            uiCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            bookCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        }
        else
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            uiCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            bookCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        }

        loadingCheck = false;
        storyDetailPopUp.SetActive(false);

        bookTemplate.SetActive(false);
        downloadBtn.SetActive(true);
        downloadIndicator.SetActive(false);
        readBtn.SetActive(false);
        resultPopUp.SetActive(false);

        last_book_index = -1;
    }

    private void OnDestroy()
    {
        if (tex != null)
            Destroy(tex);

        if (tex2 != null)
            Destroy(tex2);
    }

    // Start is called before the first frame update
    void Start()
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);
        loadingCheck = false;

        StartCoroutine(GetAvatarImage(UserDataManagerBehavior.GetInstance().currentSelectedKidAvatarURL));
        nameTxt.text = UserDataManagerBehavior.GetInstance().currentSelectedKidName;
        GetKidInfoFromServer();

        //pull all books by default
        OnAllBookClicked();

        StartCoroutine(DelayedOpeningTalk());
    }

    IEnumerator DelayedOpeningTalk()
    {
        yield return new WaitForSeconds(3.0f);
        idleCor = StartCoroutine(IdleSequence());
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: adjust canvas based on screen ratio
        if (Camera.main.aspect >= 1.7)
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            uiCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            bookCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            resultCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        }
        else
        {
            mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            uiCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            bookCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            resultCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        }

        //loading checker
        if (loadingCheck == true && loadingProgress >= 2 + storyDataList.Count)
        {
            loadingProgress++;
            loadingCheck = false;

            curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
        }

        //idle checker
        if (idleDuration > 0.0f &&
            //storyDetailPopUp.activeSelf == false && 
            bookTemplate.activeSelf == false &&
            resultPopUp.activeSelf == false)
        {
            idleDuration -= Time.deltaTime;
            if (idleDuration <= 0.0f)
            {
                if (idleCor == null)
                {
                    idleCor = StartCoroutine(IdleSequence());
                }
            }
        }
        else
        {
            if (idleCor == null)
            {
                idleCor = StartCoroutine(IdleSequence());
            }
        }

        if (bookTemplate.activeSelf == true ||
            resultPopUp.activeSelf == true)
        {
            avatarAudioSource.Stop();
        }
    }

    Coroutine idleCor = null;
    IEnumerator IdleSequence()
    {
        yield return new WaitForSeconds(1.0f);
        avatarAudioSource.Stop();
        avatarAudioSource.clip = openingClip;
        avatarAudioSource.Play();

        avatarController.PlayAnimationLooped("Pointing");
        avatarController.PlayAnimationLooped("talk", 1);
        yield return new WaitForSeconds(avatarAudioSource.clip.length);
        avatarController.PlayAnimationLooped("idle_with_book");
        avatarController.PlayAnimationLooped("idle_with_book", 1);
        idleDuration = 15.0f;
        idleCor = null;
    }

    public void OnBackToMainScene()
    {
        Time.timeScale = 1.0f;

        if (resultPopUp.activeSelf == true)
        {
            resultPopUp.SetActive(false);
            bookTemplate.SetActive(false);

            GetKidInfoFromServer();
        }
        else if (bookTemplate.activeSelf == true)
        {
            UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("library_zone", "on_back");
            SendEventFirebase.SendEventStoryRead(storyNameTxt.text, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "exit");
            /*
            SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
            bookTemplate.SetActive(false);
        }
        else if (storyDetailPopUp.activeSelf == true)
        {
            storyDetailPopUp.SetActive(false);
        }
        else
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_MainWorldMapScene, this.gameObject));
    }

    public void OnBackToMainSceceShort()
    {
        Time.timeScale = 1.0f;

        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_MainWorldMapScene, this.gameObject));

        if(resultPopUp.activeSelf == false && bookTemplate.activeSelf == true)
        {
            UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("library_zone", "on_back");
            SendEventFirebase.SendEventStoryRead(storyNameTxt.text, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "exit");
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

    public void OnAllBookClicked()
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);

        //loading stories data
        if (loadingProgress >= 2)
            loadingProgress -= storyDataList.Count;
        storyDataList.Clear();
        StartCoroutine(GetStoriesListInfo());
    }

    public void OnNewBookClicked()
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);

        if (loadingProgress >= 2)
            loadingProgress -= storyDataList.Count;
        storyDataList.Clear();
        StartCoroutine(GetStoriesListInfo("is_newarrival=1"));
    }

    public void OnMostReadClicked()
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);

        if (loadingProgress >= 2)
            loadingProgress -= storyDataList.Count;
        storyDataList.Clear();
        StartCoroutine(GetStoriesListInfo("is_suggested=1"));
    }

    public void OnReadBookListClicked()
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);

        if (loadingProgress >= 2)
            loadingProgress -= storyDataList.Count;
        storyDataList.Clear();
        StartCoroutine(GetStoriesListInfo("is_read=1"));
    }

    public void OnReadClicked()
    {
        UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);
        SendEventFirebase.SendEventStoryAccess(storyNameTxt.text);
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
        //populate data here
        resultPopUp.SetActive(false);
        bookTemplate.SetActive(true);
        //dialogBehavior.gameObject.SetActive(false);
        //dialogBehavior.gameObject.SetActive(true);

        currentBookID = storyDataList[clicked_book_index]._id;
        //dialogBehavior.InitPage(0);
        bookTemplate.GetComponent<FreeBookDisplayBehavior>().StartBook();
    }

    public void OnDownloadClicked()
    {
        downloadCor = StartCoroutine(OnDownloadClickedSequence());
        SendEventFirebase.SendEventStoryDownload(storyNameTxt.text);
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
    }

    public void CancelDownLoad()
    {
        if (downloadCor != null)
            StopCoroutine(downloadCor);

        downloadBtn.SetActive(true);
        downloadIndicator.SetActive(false);
        readBtn.SetActive(false);
    }


    IEnumerator OnDownloadClickedSequence()
    {
        //download data here
        downloadBtn.SetActive(false);
        downloadIndicator.SetActive(true);
        readBtn.SetActive(false);
        downloadProgressTxt.text = "";

        //start download data
        currentBookPagesList.Clear();
        foreach (LibraryZoneStoryData story in storyDataList)
        {
            if (clicked_book_index == storyDataList.IndexOf(story))
            {
                for (int i = 0; i < story.pages.Length; i++)
                {
                    FreeBookPageData new_page = new FreeBookPageData(story.pages[i].illustration_image.value,
                                                         story.background_music.value,
                                                         story.pages[i].content,
                                                         this
                                                        );

                    yield return StartCoroutine(new_page.DownloadDataFromURI(i));
                    currentBookPagesList.Add(new_page);

                    downloadProgressTxt.text = "Progress: " + (int)(((float)i / (float)(story.pages.Length + 1)) * 100f) + "%";
                }
            }
            else
                continue;
        }

        yield return null;
        //put downloaded data into display
        //bookTemplate.GetComponent<EngKidLibrary.Book>().bookPages = new Sprite[currentBookPagesList.Count * 2];
        bookTemplate.GetComponent<FreeBookDisplayBehavior>().InitBook();
        foreach (FreeBookPageData page in currentBookPagesList)
        {
            //1: main img
            if (page.pageImgSprite != null)
            {
                //bookTemplate.GetComponent<EngKidLibrary.Book>().bookPages[currentBookPagesList.IndexOf(page) * 2] = page.pageImgSprite;
                //if ((currentBookPagesList.IndexOf(page) * 2) + 1 < bookTemplate.GetComponent<EngKidLibrary.Book>().bookPages.Length)
                //    bookTemplate.GetComponent<EngKidLibrary.Book>().bookPages[(currentBookPagesList.IndexOf(page) * 2) + 1] = page.pageImgSprite;
                bookTemplate.GetComponent<FreeBookDisplayBehavior>().AddPage(this, page.pageImgSprite, page.bgmClip, page.sentencesList, page.sentencesAudioClipsList, page.storyTellerSpritesList);
            }
        }
        bookTemplate.GetComponent<FreeBookDisplayBehavior>().CommitBook();
        downloadProgressTxt.text = "Progress: 100%";

        //Debug.Log("Chosen book has: " + bookTemplate.GetComponent<EngKidLibrary.Book>().bookPages.Length + " pages.");

        yield return null;
        yield return null;
        yield return null;
        downloadBtn.SetActive(false);
        downloadIndicator.SetActive(false);
        readBtn.SetActive(true);
    }

    IEnumerator GetAvatarImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);

            yield return null;
            loadingProgress++;
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            avatarImg.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            yield return null;
            loadingProgress++;
        }
    }

    public void GetKidInfoFromServer()
    {
        StartCoroutine(GetKidInfoSequence());
    }

    IEnumerator GetKidInfoSequence()
    {
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_KID_DETAIL_INFO,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get kid information fail.");
            },
            server_reply =>
            {
                expTxt.text = server_reply.data.account.xp.ToString() + " XP";
                Debug.Log("__________Thien_Xp: " + server_reply.data.account.xp);
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );

        yield return null;
        loadingProgress++;
    }

    List<string> read_books_ids = new List<string>(); 
    IEnumerator GetStoriesListInfo(string extra_pulling_tag = "")
    {
        //storyDataList.Clear();
        //force reselect accout here
        //Debug.Log("Current user ID: " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken);
        Debug.Log("Start loading stories list.");

        if (string.IsNullOrEmpty(extra_pulling_tag) == false)
            extra_pulling_tag = "?" + extra_pulling_tag;

        Debug.Log("Request: " + DataBaseInterface.LIBRARY_GET_ALL_STORIES_URI + extra_pulling_tag);

        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.LIBRARY_GET_ALL_STORIES_URI + extra_pulling_tag,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: Data pulling failed.");
            },
            server_reply =>
            {
                if (server_reply.data.list.Length > 0)
                {
                    foreach (UsableDataInfo story in server_reply.data.list)
                    {
                        Debug.Log("Story: " + story.title + " is read: " + story.read_times + " time.");
                        storyDataList.Add(new LibraryZoneStoryData(
                                story.title,
                                story.author,
                                story.illustrator,
                                story.publisher,
                                story.index_thumbnail,
                                story.detail_thumbnail,
                                story.background_music,
                                story.read_times,
                                story.createdAt,
                                story.updatedAt,
                                story.__v.ToString(),
                                story._id,
                                story.pages
                            ));
                    }
                }

                read_books_ids.Clear();
                if (server_reply.data.read_story_list.Length > 0)
                {
                    foreach (ReadbookData read_book in server_reply.data.read_story_list)
                    {
                        read_books_ids.Add(read_book.story_id);
                    }
                }

                Debug.Log("There are: " + storyDataList.Count + " stories.");
                StartCoroutine(LoadAllStoriesthumbnail());
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
        );

        yield return null;
    }

    IEnumerator LoadAllStoriesthumbnail()
    {
        foreach (Transform child in storyInfosHolder)
        {
            Destroy(child.gameObject);
        }

        //change thumbnail image
        yield return null;
        loadingCheck = true;

        //yield return null;
        //loadingProgress++;

        Debug.Log("updating displays of stories list.");
        yield return null;
        foreach (LibraryZoneStoryData story in storyDataList)
        {
            Debug.Log("updating displays of story number: " + storyDataList.IndexOf(story) + " , story name: " + story.title);
            GameObject book = Instantiate(storyInfoPrefab, storyInfosHolder);
            book.GetComponent<BookCoverBehavior>().index = storyDataList.IndexOf(story);
            book.GetComponent<BookCoverBehavior>().librarySceneBehavior = this;

            //Debug.Log("This story is read: " + story.read_times + " times.");
            //if (story.read_times.Equals("0") == false)
            //    book.transform.GetChild(1).gameObject.SetActive(true);
            //else
            //    book.transform.GetChild(1).gameObject.SetActive(false);

            book.transform.GetChild(1).gameObject.SetActive(false);
            foreach (string book_id in read_books_ids)
            {
                if (book_id.Equals(story._id) == true)
                {
                    book.transform.GetChild(1).gameObject.SetActive(true);
                    break;
                }
            }

            book.GetComponent<BookCoverBehavior>().bookLabel.text = story.title;
            StartCoroutine(book.GetComponent<BookCoverBehavior>().GetCoverImage(story.index_thumbnail.value));
        }
        yield return null;
    }

    public void LoadingPing()
    {
        loadingProgress++;
    }

    bool cover_clickable = true;
    int last_book_index = -1;
    public void ShowStoryPopUp(int story_index)
    {
        if (cover_clickable == false)
            return;

        if (resultPopUp.activeSelf)
            resultPopUp.SetActive(false);

        if (storyDetailPopUp.activeSelf)
            storyDetailPopUp.SetActive(false);

        if (bookTemplate.activeSelf)
            bookTemplate.SetActive(false);

        if (story_index != last_book_index)
        {
            last_book_index = story_index;
            downloadBtn.SetActive(true);
            downloadIndicator.SetActive(false);
            readBtn.SetActive(false);
        }

        cover_clickable = false;
        clicked_book_index = story_index;
        StartCoroutine(ShowStorypopUpSequence(story_index));
    }

    Texture2D tex2;
    IEnumerator ShowStorypopUpSequence(int story_index)
    {
        curtain.gameObject.SetActive(true);
        curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);
        yield return new WaitForSeconds(0.5f);

        storyDetailPopUp.SetActive(true);
        cover_clickable = true;
        storyNameTxt.text = storyDataList[story_index].title;
        authorTxt.text = storyDataList[story_index].author.name;
        artistTxt.text = storyDataList[story_index].illustrator.name;
        publisherTxt.text = storyDataList[story_index].publisher.name;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(storyDataList[story_index].detail_thumbnail.value);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);

            yield return null;
            curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
        }
        else
        {
            tex2 = ((DownloadHandlerTexture)www.downloadHandler).texture;
            detailedThumbnail.sprite = Sprite.Create(tex2, new Rect(0.0f, 0.0f, tex2.width, tex2.height), new Vector2(0.5f, 0.5f), 100.0f);

            yield return null;
            curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
        }

        yield return null;
        curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
    }
}

[System.Serializable]
public class FreeBookPageData
{
    //uri data
    string pageIllustrationURL = "";
    string bgmURI = "";
    List<PageDataContent> pageDataContentList = new List<PageDataContent>();
    MonoBehaviour callerObj;

    //usable data
    public int pageIndex = -1;
    public Sprite pageImgSprite;
    public List<string> sentencesList = new List<string>();
    public List<Sprite> storyTellerSpritesList = new List<Sprite>();
    public List<AudioClip> sentencesAudioClipsList = new List<AudioClip>();
    public AudioClip bgmClip = null;

    public FreeBookPageData(string i, string b, PageDataContent[] data_list, MonoBehaviour caller)
    {
        //init
        sentencesList.Clear();
        storyTellerSpritesList.Clear();
        sentencesAudioClipsList.Clear();

        callerObj = caller;

        pageIllustrationURL = i;
        bgmURI = b;

        pageDataContentList.Clear();
        for (int index = 0; index < data_list.Length; index++)
        {
            PageDataContent temp_data = new PageDataContent();

            if (data_list[index] != null &&
                data_list[index].story_teller != null &&
                data_list[index].story_teller.avatar != null &&
                data_list[index].story_teller.avatar.value != null)
                temp_data.story_teller.avatar.value = data_list[index].story_teller.avatar.value;

            if (data_list[index] != null &&
                data_list[index].text != null &&
                data_list[index].text.value != null)
                temp_data.text.value = data_list[index].text.value;

            if (data_list[index] != null &&
                data_list[index].audio != null &&
                data_list[index].audio.value != null)
                temp_data.audio.value = data_list[index].audio.value;

            pageDataContentList.Add(temp_data);
        }
    }

    public IEnumerator DownloadDataFromURI(int page_index)
    {
        pageIndex = page_index;

        //download main img
        yield return callerObj.StartCoroutine(GlobalHelper.GetInstance().GetSpriteFromURLSequence(pageIllustrationURL, return_sprite =>
        {
            if (return_sprite != null)
            {
                pageImgSprite = return_sprite;
                //Debug.Log("Downloaded page: " + pageIllustrationURL);
            }
            else
                Debug.Log("Page illustration is null.");
        }));

        //download bgm
        GlobalHelper.GetInstance().GetAudioClipFromURLSequence(
            bgmURI,
            callback =>
            {
                if (bgmClip != null)
                {
                    bgmClip = callback;
                }
            }
        );

        foreach (PageDataContent page in pageDataContentList)
        {
            //set sentences
            sentencesList.Add(page.text.value);

            //download story teller img
            yield return callerObj.StartCoroutine(GlobalHelper.GetInstance().GetSpriteFromURLSequence(page.story_teller.avatar.value, return_sprite =>
            {
                if (return_sprite != null)
                {
                    storyTellerSpritesList.Add(return_sprite);
                    //Debug.Log("Downloaded story teller avatar: " + page.story_teller.avatar.value);
                }
                else
                {
                    Debug.Log("Story teller avatar is null.");
                    storyTellerSpritesList.Add(null);
                }
            }));

            //download audio
            yield return callerObj.StartCoroutine(GlobalHelper.GetInstance().GetAudioClipFromURLSequence(page.audio.value, return_clip =>
            {
                if (return_clip != null)
                {
                    sentencesAudioClipsList.Add(return_clip);
                    //Debug.Log("Downloaded sentence audio: " + page.audio.value);
                }
                else
                {
                    sentencesAudioClipsList.Add(null);
                    Debug.Log("Story sentence audio is null.");
                }
            }));

            yield return null;
        }
    }
}

public class LibraryZoneStoryData
{
    public string title = "";
    public PersonelInfo author;
    public PersonelInfo illustrator;
    public PersonelInfo publisher;
    public IndexThumbnail index_thumbnail;
    public IndexThumbnail detail_thumbnail;
    public AudioInfo background_music;
    public string read_times = "";
    public string createdAt = "";
    public string updatedAt = "";
    public string __v = "";
    public string _id = "";
    public PageData[] pages;

    public LibraryZoneStoryData(string t, PersonelInfo a, PersonelInfo i, PersonelInfo p, IndexThumbnail it, IndexThumbnail dt, AudioInfo bgm, string rt, string ca, string ua, string v, string id, PageData[] ps)
    {
        title = t;
        author = a;
        illustrator = i;
        publisher = p;
        index_thumbnail = it;
        detail_thumbnail = dt;
        background_music = bgm;
        read_times = rt;
        createdAt = ca;
        updatedAt = ua;
        __v = v;
        _id = id;

        if (ps == null)
            Debug.LogError("Story pages array is null!");

        pages = new PageData[ps.Length];
        for (int index = 0; index < pages.Length; index++)
        {
            pages[index] = ps[index];
        }
    }
}
