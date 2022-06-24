using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LessonChoosingWindowBehavior : MonoBehaviour
{
    [Header("BGM Settings")]
    public AudioClip m_BGMClip;

    [Header("Lesson ID")]
    public EngKidAPI.MilestoneIDs m_MileStoneID = EngKidAPI.MilestoneIDs.NULL;
    public EngKidAPI.UnitIDs m_UnitID = EngKidAPI.UnitIDs.NULL;

    [Header("Lesson Buttons List")]
    public List<GameObject> m_LessonList = new List<GameObject>();

    [Header("Lesson Label")]
    public Text m_LessonLabelText;

    [Header("Activities Choosing Windows")]
    public List<AssetReference> m_ActivityChoosingWindowList = new List<AssetReference>();

    [Header("Popups Settings")]
    public GameObject m_DownloadPopUp;
    public GameObject m_LockedPopup;

    //internal
    [HideInInspector] public GameObject currentHighlightedLesson;
    [HideInInspector] public int lockedPopupScaleTweenId;
    [HideInInspector] public UnitDetailsInfo unitDetailsInfo = null;
    [HideInInspector] public bool isDataPulled = false;
    bool isReady = true;

    [HideInInspector] public List<Texture2D> texList = new List<Texture2D>();

    //internal
    [HideInInspector] public AudioSource audioSource;

    private void OnEnable()
    {
        foreach (Texture2D tex in texList)
        {
            DestroyImmediate(tex);
        }
        texList.Clear();

        back_clicked = true;
        StartCoroutine(DelayedResetBackClicked());

        isReady = true;
        GlobalCurtain.GetInstance().TriggerCurtain();

        if (audioSource == null)
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonTapClip;
        }

        foreach (GameObject go in m_LessonList)
        {
            ColorBlock cb = go.GetComponent<Button>().colors;
            cb.normalColor = Color.white;
            go.GetComponent<Button>().colors = cb;
        }

        RectTransform content = m_LessonList[0].transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(4000f, content.sizeDelta.y);

        GameObject sc = content.parent.parent.gameObject;
        //Debug.Log(sc.name);
        if (sc.GetComponent<ScrollViewContentScaler>() == null)
        {
            ScrollViewContentScaler contentScaler = sc.AddComponent<ScrollViewContentScaler>();
            contentScaler.m_ContentHolder = content;
            //Debug.Log(contentScaler.gameObject.name);

            Transform canvas = sc.transform.parent;
            if (canvas.GetComponentInChildren<ScrollViewSelectionTrigger>() != null &&
                canvas.GetComponentInChildren<ScrollViewSelectionTrigger>().gameObject.GetComponent<ScrollViewContentScalerTrigger>() == null)
            {
                ScrollViewContentScalerTrigger trigger = canvas.GetComponentInChildren<ScrollViewSelectionTrigger>().gameObject.AddComponent<ScrollViewContentScalerTrigger>();
                trigger.scrollViewContentScaler = contentScaler;
                trigger.grayOutOfFocus = false;

                //Debug.Log(trigger.gameObject.name);

                trigger.gameObject.GetComponent<BoxCollider>().size = new Vector3(100f, 1000f, 100f);
            }
            else if (canvas.GetComponentInChildren<ScrollViewSelectionTrigger>().gameObject.GetComponent<ScrollViewContentScalerTrigger>() != null)
            {
                canvas.GetComponentInChildren<ScrollViewSelectionTrigger>().gameObject.GetComponent<ScrollViewContentScalerTrigger>().grayOutOfFocus = false;
            }
        }

        int start_pos = -1;
        if (PlayerPrefs.HasKey(this.gameObject.name + "_LAST_LESSON_INDEX"))
            start_pos = PlayerPrefs.GetInt(this.gameObject.name + "_LAST_LESSON_INDEX");
        StartCoroutine(DelayedReCentering(sc, start_pos));
    }

    IEnumerator DelayedResetBackClicked()
    {
        yield return new WaitForSeconds(1.0f);
        back_clicked = false;
    }

    public Coroutine recenterCor = null;
    public float idleDur = 2.0f;
    IEnumerator DelayedReCentering(GameObject sc, int i = -1)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (sc.GetComponent<ScrollViewContentCentering>() != null)
        {
            sc.GetComponent<ScrollViewContentCentering>().m_DefaultTarget = m_LessonList[(i == -1) ? 0 : i].GetComponent<RectTransform>();
            sc.GetComponent<ScrollViewContentCentering>().OnCenteringOnElement(m_LessonList[(i == -1) ? 0 : i].GetComponent<RectTransform>());
        }
        yield return new WaitForSeconds(2.25f);
        //ready_to_focus = true;
    }

    //bool ready_to_focus = false;
    private void Update()
    {
        //if (ready_to_focus == false)
        //    return;

        if (idleDur > 0.0f && Input.GetMouseButton(0) == false)
        {
            idleDur -= Time.deltaTime;
            if (idleDur <= 0.0f)
            {
                //idleDur = 2.0f;
                RectTransform content = m_LessonList[0].transform.parent.GetComponent<RectTransform>();
                GameObject sc = content.parent.parent.gameObject;
                recenterCor = StartCoroutine(DelayedReCentering(sc, current_lesson_index));
            }
        }

        if (Input.GetMouseButton(0) == true)
        {
            idleDur = 2.0f;
            if (recenterCor != null)
                StopCoroutine(recenterCor);
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        //init
        isDataPulled = false;

        m_DownloadPopUp.SetActive(false);
        m_LockedPopup.SetActive(false);

        if (BGMManagerBehavior.GetInstance().m_BGMAudioSource.isPlaying == false ||
            BGMManagerBehavior.GetInstance().m_BGMAudioSource.clip != m_BGMClip)
            BGMManagerBehavior.GetInstance().PlayBGM(m_BGMClip);

        // TODO: change this later to the last unlocked one
        //and coloring correctly
        foreach (GameObject lesson_btn in m_LessonList)
        {
            //TODO: remove this after build
            //lesson_btn.transform.GetChild(0).gameObject.SetActive(false);

            var colors = lesson_btn.GetComponent<Button>().colors;
            //colors.normalColor = Color.gray;
            lesson_btn.GetComponent<Button>().colors = colors;
        }
        currentHighlightedLesson = m_LessonList[0];
        var colors_normal = currentHighlightedLesson.GetComponent<Button>().colors;
        colors_normal.normalColor = Color.white;
        currentHighlightedLesson.GetComponent<Button>().colors = colors_normal;

        DelayedUpdateDisplay();

        SceneManagerBehavior.GetInstance().FadeLoadingScreenOut();
    }

    public void DelayedUpdateDisplay()
    {
        isDataPulled = false;
        //Debug.Log(DataBaseInterface.GET_UNIT_DETAIL_URI + UserDataManagerBehavior.GetInstance().currentSelectedUnitID);
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_UNIT_DETAIL_URI + UserDataManagerBehavior.GetInstance().currentSelectedUnitID,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get unit detail failed.");

                isDataPulled = false;
            },
            server_reply =>
            {
                //Debug.Log(server_reply.data.unit.lessons[0].title);
                unitDetailsInfo = null;
                unitDetailsInfo = server_reply.data.unit;
                UserDataManagerBehavior.GetInstance().unitDetailsInfo = unitDetailsInfo;

                isDataPulled = true;
                UpdateLessonDisplays(m_LessonList[0]);

                //load lesson images for all lessons
                StartCoroutine(UpdateDisplaySequence());

                for (int i = 0; i < unitDetailsInfo.lessons.Length; i++)
                {
                    if (int.Parse(server_reply.data.account_progress.mile_stone_mark) > UserDataManagerBehavior.GetInstance().GetMileStoneProgressMarkFromID(unitDetailsInfo.ancestor.mile_stone_id))
                    {
                        m_LessonList[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    else if (int.Parse(server_reply.data.account_progress.mile_stone_mark) == UserDataManagerBehavior.GetInstance().GetMileStoneProgressMarkFromID(unitDetailsInfo.ancestor.mile_stone_id))
                    {
                        //filter through unit progression
                        if (int.Parse(server_reply.data.account_progress.unit_mark) > int.Parse(unitDetailsInfo.progress_mark))
                            m_LessonList[i].transform.GetChild(0).gameObject.SetActive(false);
                        else
                        {
                            //filter through lesson progression
                            if (int.Parse(server_reply.data.account_progress.lesson_mark)/* + 1*/ >= int.Parse(unitDetailsInfo.lessons[i].progress_mark))
                                m_LessonList[i].transform.GetChild(0).gameObject.SetActive(false);
                            else
                                m_LessonList[i].transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (int.Parse(server_reply.data.account_progress.lesson_mark)/* + 1*/ >= int.Parse(unitDetailsInfo.lessons[i].progress_mark))
                            m_LessonList[i].transform.GetChild(0).gameObject.SetActive(false);
                        else
                            m_LessonList[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    IEnumerator UpdateDisplaySequence()
    {
        for (int i = 0; i < m_LessonList.Count && i < unitDetailsInfo.lessons.Length; i++)
        {
            int temp_index = i;

            //Debug.Log("Progress mark lesson: " + unitDetailsInfo.lessons[temp_index].progress_mark);

            if (m_LessonList[temp_index].GetComponent<Image>() != null)
                Destroy(m_LessonList[temp_index].GetComponent<Image>());

            yield return new WaitForEndOfFrame();

            yield return StartCoroutine(GetAndSetLessonImages(
                unitDetailsInfo.lessons[temp_index].thumbnail.value,
                ret_texture =>
                {
                    //if (ret_texture == null)
                    //    Debug.Log("ret_texture is null!");
                    //Debug.Log("Update image of lesson #: " + temp_index);
                    //Debug.Log(unitDetailsInfo.lessons[temp_index].thumbnail.value);
                    if (ret_texture != null)
                    {
                        if (m_LessonList[temp_index].GetComponent<RawImage>() == null)
                        {
                            if (m_LessonList[temp_index].GetComponent<Image>() != null)
                                Destroy(m_LessonList[temp_index].GetComponent<Image>());

                            RawImage ri = m_LessonList[temp_index].AddComponent<RawImage>();
                            m_LessonList[temp_index].GetComponent<Button>().targetGraphic = ri;

                            ri.texture = ret_texture;
                        }
                        else
                        {
                            m_LessonList[temp_index].GetComponent<RawImage>().texture = ret_texture;
                        }
                    }
                }
                ));
        }

        yield return new WaitForEndOfFrame();
        GlobalCurtain.GetInstance().OpenCurtain();
    }

    IEnumerator GetAndSetLessonImages(string url, System.Action<Texture2D> callback_texture)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request URL: " + url + "\nError: " + www.error);
        }
        else
        {
            Texture2D tex = (DownloadHandlerTexture.GetContent(www)) as Texture2D;
            texList.Add(tex);
            callback_texture.Invoke(tex);
            //DestroyImmediate(((DownloadHandlerTexture)www.downloadHandler).texture);
        }


        www.Dispose();
        www = null;
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        foreach (Texture2D tex in texList)
        {
            DestroyImmediate(tex);
        }
        texList.Clear();

        StopAllCoroutines();
    }

    public int current_lesson_index = 0;
    public void UpdateLessonDisplays(GameObject selected_lesson, bool is_activity = false)
    {
        if (isDataPulled == false)
            return;

        //if (ready_to_focus == false)
        //    return;

        if (is_activity == false)
        {
            current_lesson_index = m_LessonList.IndexOf(selected_lesson);
            m_LessonLabelText.text = unitDetailsInfo.lessons[current_lesson_index].title;
        }
        else
        {

        }
        //OnLessonClicked(current_lesson_index);
    }

    public virtual void OnLessonClicked(int lesson_index)
    {
        if (isDataPulled == false) return;

        current_lesson_index = lesson_index;

        if (m_LessonList[lesson_index].transform.GetChild(0).gameObject.activeSelf)
        {
            //TODO: show pop up
            m_LockedPopup.SetActive(true);
            m_LockedPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Bé cần hoàn thành <b>" + unitDetailsInfo.lessons[lesson_index - 1].title + "</b> trước.";

            return;
        }
        else
        {
            if (isReady == true)
                isReady = false;
            else
                return;
        }

        //if (m_LessonList[lesson_index].transform.GetChild(0).gameObject.activeSelf)
        //{
        //    //TODO: show pop up
        //    m_LockedPopup.SetActive(true);


        //    return;
        //}   
        //else
        //{
        //    //do nothing
        //}

        GlobalCurtain.GetInstance().CloseCurtain();

        PlayerPrefs.SetInt(this.gameObject.name + "_LAST_LESSON_INDEX", lesson_index);
        PlayerPrefs.Save();

        UserDataManagerBehavior.GetInstance().currentLessonIndex = lesson_index;

        if (lesson_index < m_ActivityChoosingWindowList.Count && m_ActivityChoosingWindowList[lesson_index] != null)
        {
            UserDataManagerBehavior.GetInstance().currentSelectedLessonID = unitDetailsInfo.lessons[lesson_index]._id;
            UserDataManagerBehavior.GetInstance().currentSelectedActivityID = unitDetailsInfo.lessons[lesson_index]._id;
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_ActivityChoosingWindowList[lesson_index], null));
        }
        //else
        //{
        //    m_LockedPopup.SetActive(true);
        //    //m_LockedPopup.transform.localScale = Vector3.zero;
        //    //LeanTween.scale(m_LockedPopup, Vector3.one, 0.25f);
        //}

        audioSource.Stop();
        audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonTapClip;
        audioSource.Play();
    }

    public virtual void OnCloseLockedPopup()
    {
        //m_LockedPopup.transform.localScale = Vector3.one;
        //if (LeanTween.isTweening(lockedPopupScaleTweenId) == false)
        //{
        //    lockedPopupScaleTweenId = LeanTween.scale(m_LockedPopup, Vector3.zero, 0.25f).setOnComplete(() => {
        m_LockedPopup.SetActive(false);
        //    }).id;
        //}
    }

    public void OnLockedPopUpConfirmed()
    {
        //TODO: navigate


        //m_LockedPopup.transform.localScale = Vector3.one;
        //if (LeanTween.isTweening(lockedPopupScaleTweenId) == false)
        //{
        //    lockedPopupScaleTweenId = LeanTween.scale(m_LockedPopup, Vector3.zero, 0.25f).setOnComplete(() => {
        m_LockedPopup.SetActive(false);
        //    }).id;
        //}
    }

    bool back_clicked = false;
    public virtual void OnBackToWorldMapClicked()
    {
        if (back_clicked == true)
            return;

        back_clicked = true;
        GlobalCurtain.GetInstance().CloseCurtain();
        //GlobalCurtain.GetInstance().HideCurtain();
        //audioSource.Stop();
        //audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonBackClip;
        //audioSource.Play();
        SceneManagerBehavior.GetInstance().OpenMainWorldMapScene(this.gameObject);
    }
}
