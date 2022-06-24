using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActivityChoosingWindowBehavior : LessonChoosingWindowBehavior
{
    [Header("Activity IDs")]
    public EngKidAPI.ActivityIDs m_ActivityID = EngKidAPI.ActivityIDs.NULL;

    [Header("Lesson prefab of this Activity")]
    public AssetReference m_Lesson;
    //public GameObject m_ActivityDisplayPrefab;
    public int m_ActivityAmount = 5;

    [Header("Special Navigation")]
    public int m_LessonIndex = -1;
    public AssetReference next_lesson;

    //internal
    [HideInInspector] public LessonDetailInfo lessonDetail = null;
    bool isReadyToWork = false;
    //AudioSource audioSource;
    Transform activitiesHolder;
    bool isReadyToClickOnActivity = true;

    private void OnEnable()
    {
        is_back_clicked = true;
        StartCoroutine(DelayedResetBackClicked());
        RectTransform rt = m_LockedPopup.transform.parent.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        m_LockedPopup.SetActive(false);

        isReadyToClickOnActivity = true;
        GlobalCurtain.GetInstance().TriggerCurtain();
        activitiesHolder = this.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0);
        //Debug.Log(activitiesHolder.name);
        foreach (Transform child in activitiesHolder)
        {
            Destroy(child.gameObject);
        }
        m_LessonList.Clear();

        for (int i = 0; i < m_ActivityAmount; i++)
        {
            GameObject go = Instantiate(CommonPrefabAssets.GetInstance().m_ActivityDisplay, activitiesHolder);
            m_LessonList.Add(go);
            go.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnLessonClicked(m_LessonList.IndexOf(go));
            });
        }

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
            cb.highlightedColor = Color.white;
            cb.pressedColor = Color.white;
            cb.selectedColor = Color.white;
            cb.disabledColor = Color.gray;
            go.GetComponent<Button>().colors = cb;
        }

        RectTransform content = m_LessonList[0].transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(4000f, m_LessonList[0].transform.parent.GetComponent<RectTransform>().sizeDelta.y);

        GameObject sc = content.parent.parent.gameObject;
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
        if (PlayerPrefs.HasKey(this.gameObject.name + "_LAST_ACTIVITY_INDEX"))
            PlayerPrefs.GetInt(this.gameObject.name + "_LAST_ACTIVITY_INDEX");
        StartCoroutine(DelayedReCentering(sc, start_pos));
    }

    IEnumerator DelayedResetBackClicked()
    {
        yield return new WaitForSeconds(1.0f);
        is_back_clicked = false;
    }

    //Coroutine recenterCor = null;
    //float idleDur = 2.0f;
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
    }

    private void Update()
    {
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

    new void Start()
    {
        Image label_bg = m_LessonLabelText.transform.parent.GetComponent<Image>();
        if (label_bg == null)
        {
            label_bg = m_LessonLabelText.transform.parent.gameObject.AddComponent<Image>();
            label_bg.sprite = CommonPrefabAssets.GetInstance().m_LabelSprite;
            label_bg.SetNativeSize();
        }

        //add to mapping
        StartCoroutine(DelayedUpdateMapping());
    }

    List<SceneMappingData> local_mapping_data = new List<SceneMappingData>();
    IEnumerator DelayedUpdateMapping()
    {
        local_mapping_data.Clear();
        yield return null;
        yield return null;
        yield return null;

        foreach (AssetReference act in m_ActivityChoosingWindowList)
        {
            SceneMappingData data = new SceneMappingData(
                "",
                act,
                SceneManagerBehavior.GetInstance().current_active_scene,
                (m_ActivityChoosingWindowList.IndexOf(act) + 1 < m_ActivityChoosingWindowList.Count) ? m_ActivityChoosingWindowList[m_ActivityChoosingWindowList.IndexOf(act) + 1] : next_lesson
                );

            local_mapping_data.Add(data);
        }

        yield return null;
        yield return null;

        //init
        isDataPulled = false;
        m_DownloadPopUp.SetActive(false);
        m_LockedPopup.SetActive(false);
        BGMManagerBehavior.GetInstance().PlayBGM(m_BGMClip);

        // TODO: change this later to the last unlocked one
        //and coloring correctly
        foreach (GameObject lesson_btn in m_LessonList)
        {
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

    new void DelayedUpdateDisplay()
    {
        isDataPulled = false;
        //load activity info
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_LESSON_DETAIL_URI + UserDataManagerBehavior.GetInstance().unitDetailsInfo.lessons[m_LessonIndex]._id,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get lesson detail failed.");
                else
                    Debug.Log("Get lesson details success!");

                isDataPulled = false;
            },
            server_reply =>
            {
                //Debug.Log(server_reply.data.unit.lessons[0].title);
                lessonDetail = null;
                lessonDetail = server_reply.data.lesson;

                for (int i = 0; i < lessonDetail.activities.Length && i < local_mapping_data.Count; i++)
                {
                    //SceneManagerBehavior.GetInstance().sceneMappingDatas[i].id = lessonDetail.activities[i]._id;
                    local_mapping_data[i].id = lessonDetail.activities[i]._id;
                }

                for (int i = 0; i < local_mapping_data.Count; i++)
                {
                    if (SceneManagerBehavior.GetInstance().CheckIfDuplucateMapping(local_mapping_data[i]) == false)
                    {
                        SceneManagerBehavior.GetInstance().sceneMappingDatas.Add(local_mapping_data[i]);
                    }
                }

                unitDetailsInfo = null;
                unitDetailsInfo = UserDataManagerBehavior.GetInstance().unitDetailsInfo;

                isDataPulled = true;

                //Debug.Log(lessonDetail.title);
                if (m_LessonLabelText != null)
                    m_LessonLabelText.text = lessonDetail.title;

                for (int i = 0; i < m_LessonList.Count && i < lessonDetail.activities.Length; i++)
                {
                    m_LessonList[i].GetComponent<ActivityDisplayManager>().id = lessonDetail.activities[i]._id;
                }

                for (int i = 0; i < m_LessonList.Count && i < lessonDetail.activities.Length; i++)
                {
                    //Debug.LogWarning("Activity: " + lessonDetail.activities[i].title + " has: " + lessonDetail.activities[i].archive_star + " stars.");
                    //Debug.LogWarning("Activity: " + lessonDetail.activities[i].title + " has: " + server_reply.data.account_activity_results[i].activity.archived_star + " stars.");
                    if (int.Parse(server_reply.data.account_progress.mile_stone_mark) > UserDataManagerBehavior.GetInstance().GetMileStoneProgressMarkFromID(unitDetailsInfo.ancestor.mile_stone_id))
                    {
                        m_LessonList[i].GetComponent<ActivityDisplayManager>().m_Score = int.Parse(lessonDetail.activities[i].archive_star);
                        m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                        m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                        foreach (GameObject star in m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsList)
                        {
                            star.SetActive(false);
                        }
                    }
                    else if (int.Parse(server_reply.data.account_progress.mile_stone_mark) == UserDataManagerBehavior.GetInstance().GetMileStoneProgressMarkFromID(unitDetailsInfo.ancestor.mile_stone_id))
                    {
                        //TODO: filter through unit progression 
                        if (int.Parse(server_reply.data.account_progress.unit_mark) > int.Parse(unitDetailsInfo.progress_mark))
                        {
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_Score = int.Parse(lessonDetail.activities[i].archive_star);
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                            foreach (GameObject star in m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsList)
                            {
                                star.SetActive(false);
                            }
                        }
                        else if (int.Parse(server_reply.data.account_progress.unit_mark) == int.Parse(unitDetailsInfo.progress_mark))
                        {
                            //TODO: filter through lesson progression
                            if (int.Parse(server_reply.data.account_progress.lesson_mark) > int.Parse(lessonDetail.progress_mark))
                            {
                                m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                                m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                                foreach (GameObject star in m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsList)
                                {
                                    star.SetActive(false);
                                }
                            }
                            else if (int.Parse(server_reply.data.account_progress.lesson_mark) == int.Parse(lessonDetail.progress_mark))
                            {
                                if (int.Parse(server_reply.data.account_progress.activity_mark)/* + 1*/ >= int.Parse(lessonDetail.activities[i].progress_mark))
                                {
                                    m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                                    m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                                    foreach (GameObject star in m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsList)
                                    {
                                        star.SetActive(false);
                                    }
                                }
                                else
                                {
                                    m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(false);
                                    m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(true);
                                }
                            }
                            else
                            {
                                m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(false);
                                m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(true);
                            }
                        }
                        else
                        {
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(false);
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(true);
                        }
                    }
                    else
                    {
                        m_LessonList[i].GetComponent<ActivityDisplayManager>().m_Score = int.Parse(lessonDetail.activities[i].archive_star);
                        if (int.Parse(server_reply.data.account_progress.activity_mark)/* + 1 */>= int.Parse(lessonDetail.activities[i].progress_mark))
                        {
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                            foreach (GameObject star in m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsList)
                            {
                                star.SetActive(false);
                            }
                        }
                        else
                        {
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(false);
                            m_LessonList[i].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(true);
                        }
                    }
                }

                for (int j = 0; j < m_LessonList.Count; j++)
                {
                    m_LessonList[j].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(false);
                }

                for (int i = 0; i < server_reply.data.account_activity_results.Length; i++)
                {
                    for (int j = 0; j < m_LessonList.Count; j++)
                    {
                        if (server_reply.data.account_activity_results[i].activity._id == m_LessonList[j].GetComponent<ActivityDisplayManager>().id)
                        {
                            //Debug.Log("Archived: " + server_reply.data.account_activity_results[i].archived_star);
                            if (string.IsNullOrEmpty(server_reply.data.account_activity_results[i].archived_star) == false)
                            {
                                int stars_amount = int.Parse(server_reply.data.account_activity_results[i].archived_star);
                                Debug.Log(stars_amount);
                                if (stars_amount > 0)
                                {
                                    for (int k = 0; k < stars_amount && k < 3; k++)
                                    {
                                        m_LessonList[j].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);
                                        m_LessonList[j].GetComponent<ActivityDisplayManager>().m_StarsList[k].SetActive(true);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }

                //force unlock first activity
                m_LessonList[0].GetComponent<ActivityDisplayManager>().m_LockIndicator.SetActive(false);
                //m_LessonList[0].GetComponent<ActivityDisplayManager>().m_StarsHolder.SetActive(true);

                StartCoroutine(UpdateDisplaySequence(server_reply));

                isReadyToWork = true;
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    IEnumerator UpdateDisplaySequence(ReturnedMessage server_reply)
    {
        for (int i = 0; i < m_LessonList.Count && i < lessonDetail.activities.Length; i++)
        {
            yield return new WaitForEndOfFrame();
            //Debug.Log("Progress mark activity: " + lessonDetail.activities[i].progress_mark);
            yield return StartCoroutine(OnUpdateActivityDisplay(m_LessonList[i], lessonDetail.activities[i].title, lessonDetail.activities[i].thumbnail.value));
        }
        yield return new WaitForEndOfFrame();
        GlobalCurtain.GetInstance().OpenCurtain();
    }

    public IEnumerator OnUpdateActivityDisplay(GameObject activity_button_obj, string label, string img_url)
    {
        activity_button_obj.GetComponent<ActivityDisplayManager>().m_Label.text = "Activity " + label;

        Texture2D activity_temp_texture = null;
        yield return StartCoroutine(GetAndSetActivityImage(
            img_url,
            ret_sprite =>
            {
                activity_temp_texture = ret_sprite;

                if (activity_temp_texture != null)
                    activity_button_obj.GetComponent<ActivityDisplayManager>().m_Image.texture = activity_temp_texture;
            }
            ));
    }

    public void UpdateLessonDisplays(GameObject selected_lesson)
    {
        if (isDataPulled == false)
            return;

        current_lesson_index = m_LessonList.IndexOf(selected_lesson);
    }

    IEnumerator GetAndSetActivityImage(string url, System.Action<Texture2D> callback_texture)
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

    public override void OnLessonClicked(int lesson_index)
    {
        if (isDataPulled == false)
            return;

        if (isReadyToClickOnActivity == false)
            return;
        else
            isReadyToClickOnActivity = false;

        if (isReadyToWork == false)
        {
            Debug.LogWarning("Warning: data pulling not yet complete!");
            return;
        }

        if (m_LessonList[lesson_index].GetComponent<ActivityDisplayManager>().m_LockIndicator.activeSelf == true)
        {
            //TODO: pop up to tell user to complete previous activity
            m_LockedPopup.SetActive(true);
            m_LockedPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Bé cần hoàn thành <b>Activity: " + lessonDetail.activities[lesson_index - 1].title + "</b> trước.";
            isReadyToClickOnActivity = true;
            return;
        }

        PlayerPrefs.SetInt(this.gameObject.name + "_LAST_ACTIVITY_INDEX", lesson_index);
        PlayerPrefs.Save();

        audioSource.Stop();
        audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonTapClip;
        audioSource.Play();

        if (lesson_index < lessonDetail.activities.Length)
        {
            UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);

            UserDataManagerBehavior.GetInstance().currentSelectedActivityID = lessonDetail.activities[lesson_index]._id;
            StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_ActivityChoosingWindowList[lesson_index], null));

            SceneManagerBehavior.GetInstance().current_active_scene = m_ActivityChoosingWindowList[lesson_index];

            if (lesson_index != 2)
            {
                //StartCoroutine(GetNameActivity(lesson_index));
            }
        }
        else
            Debug.LogError("Error: lesson detail data have activities number smaller than in app activities count. Lesson index: " + lesson_index + " data activity count: " + lessonDetail.activities.Length);

        GlobalCurtain.GetInstance().HideCurtain();
    }

    IEnumerator GetNameActivity(int lesson_index)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(m_ActivityChoosingWindowList[lesson_index], transform);
        yield return handle;
        Debug.Log("Name: " + handle.Result.name);
        SendEventFirebase.SendEventActivityAccess(handle.Result.name);
        /*
        SendEventFirebase.SendUserProperties(
                UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                UserDataManagerBehavior.GetInstance().user_premium_permission[0]);*/
        Addressables.ReleaseInstance(handle);
        Addressables.Release(handle);
    }

    public override void OnCloseLockedPopup()
    {
        AudioSource audio_source = m_LockedPopup.GetComponent<AudioSource>();
        if (audio_source == null)
        {
            audio_source = m_LockedPopup.AddComponent<AudioSource>();
        }
        audio_source.Stop();
        audio_source.playOnAwake = false;
        audio_source.clip = BGMManagerBehavior.GetInstance().m_ButtonTapClip;
        audio_source.loop = false;
        audio_source.Play();

        //base.OnCloseLockedPopup();
        m_LockedPopup.SetActive(false);
    }

    bool is_back_clicked = true;
    public void OnBackToLessonButtonClicked()
    {
        if (is_back_clicked == true)
            return;

        is_back_clicked = true;
        GlobalCurtain.GetInstance().CloseCurtain();
        audioSource.Stop();
        audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonBackClip;
        audioSource.Play();
        //SceneManagerBehavior.GetInstance().ChangeToNewScene(m_Lesson, this.gameObject);
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_Lesson, this.gameObject));
    }

    bool is_world_map_clicked = false;
    public override void OnBackToWorldMapClicked()
    {
        if (is_world_map_clicked == true)
            return;

        is_world_map_clicked = true;
        audioSource.Stop();
        audioSource.clip = BGMManagerBehavior.GetInstance().m_ButtonBackClip;
        audioSource.Play();
        base.OnBackToWorldMapClicked();
    }
}
