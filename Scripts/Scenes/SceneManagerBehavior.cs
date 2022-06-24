using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

public class SceneManagerBehavior : MonoBehaviour
{
    #region Singleton
    private static SceneManagerBehavior _Instance;
    private void Awake()
    {
        _Instance = this;
        if (Debug.isDebugBuild) Debug.Log("SceneManagerBehavior Loaded");
    }
    public static SceneManagerBehavior GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: SceneManagerBehavior instance is null.");

        return _Instance;
    }
    #endregion

    [Header("Common Refences")]
    public Transform m_ScenesHolder;

    [Header("Loading Scene")]
    public LoadingScreenBehavior m_LoadingScreen;

    [Header("Splash Scenes Settings")]
    public AssetReference m_SplashScenePrefab;
    public float m_SplashDuration = 7.0f;
    public float m_LoadingScreenFadingDuration = 1.0f;

    [Header("Login / Sign up Scene")]
    public AssetReference m_LoginScenePrefab;

    [Header("Main World Map Scene")]
    public AssetReference m_MainWorldMapScene;

    [Header("Free Library scene")]
    public AssetReference m_LibraryScene;

    [Header("Interactive Zone scene")]
    public AssetReference m_InteractiveZoneScene;

    [Header("Parent Zone scene")]
    public AssetReference m_ParentZoneScene;

    [Header("Parent Check")]
    public AssetReference m_ParentCheckToSettingsScene;
    public AssetReference m_ParentCheckToIAPScene;

    [Header("IAP Screen")]
    public AssetReference m_IAPScene;

    [Header("Activation When Click Active Screen")]
    public AssetReference m_ActivationClickActiveScene;

    [Header("Activation When Login Screen")]
    public AssetReference m_ActivationWhenLoginScene;

    [Header("Customize avatar scene")]
    public AssetReference m_CustomizeScene;

    [Header("Pop-Ups")]
    public GameObject m_BackToLastScenePopUp;

    [Header("Generic scenes")]
    public List<SceneMappingData> sceneMappingDatas = new List<SceneMappingData>();

    private List<AsyncOperationHandle> LoadedPrefabs = new List<AsyncOperationHandle>();

    //internal
    GameObject currentObject = null;
    [HideInInspector] public AssetReference currentScene = null;
    [SerializeField] AssetReference previousScene = null;
    bool isSplashLoaded = false;

    // Start is called before the first frame update
    public void Start()
    {
        sceneMappingDatas.Clear();
        m_BackToLastScenePopUp.SetActive(false);
        StartCoroutine(DelayedStartLoadOut());
    }

    //delay load all assets since they are all Addressables => not loaded right away => may cause null ref
    IEnumerator DelayedStartLoadOut()
    {
        yield return null;
        yield return null;
        yield return null;

        isSplashLoaded = false;
        StartCoroutine(StartNewScene(m_SplashScenePrefab));
        StartCoroutine(DelayedDestroySplashScreen());
    }

    private IEnumerator StartNewScene(AssetReference prefab)
    {
        DestroyCurrentScene();

        while (prefab.IsDone == false)
        {
            //Debug.Log("Loading prefab from addressable.");
            yield return null;
        }

        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(prefab, transform);
        yield return handle; // wait for boat asset to load
        //Debug.Log("Start Screen: " + prefab);
        previousScene = currentScene;
        currentScene = prefab;
        current_active_scene = currentScene;
        backToLastSceneSequence = null;
        yield return null;
        yield return null;
        foreach (SceneMappingData data in sceneMappingDatas)
        {
            if (data.root.AssetGUID == current_active_scene.AssetGUID)
            {
                previousScene = data.back;
                UserDataManagerBehavior.GetInstance().currentSelectedActivityID = data.id;
                break;
            }
        }
        yield return null;
        yield return null;

        //Dai Nguyen: fix crash after 20mins
        for (var i = 0; i < LoadedPrefabs.Count; i++)
        {
            var af = LoadedPrefabs[i];
            if (af.IsValid())
            {
                Debug.Log("Release Asset: " + af);
                Addressables.Release(af);
                i--;
            }
        }
        //end Dai Nguyen-----------------------------------


        //Dao Ngo
        if (handle.Result.name.Contains("Lesson")) // access lesson
        {
            StartCoroutine(GetStarLesson(handle.Result.name));
        }
        else if (handle.Result.name.Contains("Unit")) // access unit
        {
            StartCoroutine(GetStarUnit(handle.Result.name));
        }
        //

        Debug.Log("Screen loaded: " + handle.Result.name);
        currentObject = handle.Result;
        LoadedPrefabs.Add(handle);
        isSplashLoaded = true;

        //backToLastSceneSequence = null;
        yield return new WaitForEndOfFrame();
        FadeLoadingScreenOut();

        //fade loading screen out
        //if (prefab == m_SplashScenePrefab)
        //{
        //    yield return new WaitForSeconds(1.0f);
        //    FadeLoadingScreenOut();
        //}
    }

    IEnumerator DelayedDestroySplashScreen()
    {
        while (isSplashLoaded == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(m_SplashDuration + 1.0f);
        FadeLoadingScreenIn();

        if (currentObject != null)
        {
            SplashSceneBehavior splash = currentObject.GetComponent<SplashSceneBehavior>();
            if (splash != null && splash.gameObject.activeSelf == true)
                splash.FadeAudio(0.0f, 1.0f);
        }
        yield return new WaitForSeconds(1.0f);

        //TODO: NOBITA
        yield return StartCoroutine(StartNewScene(m_LoginScenePrefab));
        //PlayerPrefs.SetString("USER_TOKEN", "");
        if (!PlayerPrefs.GetString("USER_TOKEN", "").Equals(""))
        {
            UserDataManagerBehavior.GetInstance().isFirstLoginAndShowActivation = false;
            Debug.Log("da dang nhap");

            if (UserDataManagerBehavior.GetInstance() != null)
                UserDataManagerBehavior.GetInstance().currentParentSessionIDToken = PlayerPrefs.GetString("USER_TOKEN", "");

            if (CustomEventManager.GetInstance() != null)
                CustomEventManager.GetInstance().ParentLoggedInWithToken();

            UserDataManagerBehavior.GetInstance().currentParentName = PlayerPrefs.GetString("ACTIVATION_PARENT_NAME", string.Empty);
            UserDataManagerBehavior.GetInstance().currentParentPhone = PlayerPrefs.GetString("ACTIVATION_PARENT_PHONE", string.Empty);
            UserDataManagerBehavior.GetInstance().currentParentEmail = PlayerPrefs.GetString("ACTIVATION_PARENT_EMAIL", string.Empty);
            UserDataManagerBehavior.GetInstance().currentParentAvatar = PlayerPrefs.GetString("ACTIVATION_PARENT_AVATAR", string.Empty);
        }
        else
        {
            UserDataManagerBehavior.GetInstance().isFirstLoginAndShowActivation = true;
            Debug.Log("chua dang nhap");
        }

        FadeLoadingScreenOut();
    }

    //method for other behaviors to call on when loading screen manip is needed
    public void FadeLoadingScreenIn()
    {
        m_LoadingScreen.FadeLoadingImageAlpha(0.0f, 1.0f, m_LoadingScreenFadingDuration);
    }

    //method for other behaviors to call on when loading screen manip is needed
    public void FadeLoadingScreenOut()
    {
        float start_alpha = m_LoadingScreen.m_CurrentImgAlpha;
        m_LoadingScreen.FadeLoadingImageAlpha(start_alpha, 0.0f, m_LoadingScreenFadingDuration);
    }

    public void DestroyCurrentScene()
    {
        if (currentObject != null)
            Destroy(currentObject);

        if (previousScene != null && previousScene.IsValid())
        {
            Debug.Log("Unloading Scene: " + previousScene.ToString());
            previousScene.ReleaseAsset();
        }

        if (currentScene != null && currentScene.IsValid())
        {
            previousScene = currentScene;

            //foreach (SceneMappingData data in sceneMappingDatas)
            //{
            //    if (data.root.AssetGUID == current_active_activity.AssetGUID)
            //    {
            //        previousScene = data.back;
            //        break;
            //    }
            //}
        }

        if (currentScene != null && currentScene.IsValid())
        {
            Debug.Log("Unloading Scene: " + currentScene.ToString());
            currentScene.ReleaseAsset();
        }
    }

    Coroutine openMainWorldMap = null;
    public void OpenMainWorldMapScene(GameObject scene_to_destroy)
    {
        if (openMainWorldMap != null)
            return;

        openMainWorldMap = StartCoroutine(OpenMainWorldMapSceneSequence(scene_to_destroy));
    }

    IEnumerator OpenMainWorldMapSceneSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_MainWorldMapScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
        openMainWorldMap = null;
    }

    bool is_shop_scene_ready = true;
    public void OpenCustomizeShopScene(GameObject scene_to_destroy)
    {
        if (is_shop_scene_ready == false)
            return;

        is_shop_scene_ready = false;
        StartCoroutine(OpenShopSceneSequence(scene_to_destroy));
    }

    IEnumerator OpenShopSceneSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        yield return StartCoroutine(StartNewScene(m_CustomizeScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
        is_shop_scene_ready = true;
    }



    public void OpenParentZone(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenParentZoneSceneSequence(scene_to_destroy));
    }

    IEnumerator OpenParentZoneSceneSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_ParentZoneScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    Coroutine goToNextCor = null;
    public void GoToNextActivity(GameObject go, int back_lvl = 1, AssetReference next_override = null)
    {
        if (goToNextCor != null)
            return;

        goToNextCor = StartCoroutine(GoToNextActivitySequence(go, back_lvl, next_override));
    }

    /*[HideInInspector] */
    public AssetReference current_active_scene = null;
    IEnumerator GoToNextActivitySequence(GameObject caller_scene, int back_lvl, AssetReference next_override)
    {
        Debug.Log("Go to next activity sequence!");
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        if (next_override == null)
        {
            Debug.Log("Look up app mapping");
            bool is_found = false;
            AssetReference next_scene = null;
            //AssetReference back_scene = null;
            foreach (SceneMappingData data in sceneMappingDatas)
            {
                if (data.root.AssetGUID == current_active_scene.AssetGUID)
                {
                    Debug.Log("Found next scene!");
                    is_found = true;
                    next_scene = data.next;
                    //back_scene = data.back;
                    break;
                }
            }

            yield return null;
            goToNextCor = null;
            if (is_found == false)
            {
                Debug.Log("Is found: False");
                BackToLastScene(caller_scene, is_using_pop_up: false, backing_level: back_lvl);
            }
            else
            {
                if (next_scene != null)
                {
                    //Debug.Log("Next scene != null");
                    //if (next_scene.AssetGUID..Contains("Milestone"))
                    //{
                    //Debug.Log("Milestone!");
                    //StartCoroutine(GetStarLesson(back_scene.editorAsset.name));
                    StartCoroutine(StartNewScene(next_scene));
                    //SendEventFirebase.SendEventLessonComplete(back_scene.editorAsset.name, );
                    //}
                    //else if (next_scene.editorAsset.name.Contains("KidZone"))
                    //{
                    //    Debug.Log("Kidzone");
                    //    StartCoroutine(GetStarUnit(back_scene.editorAsset.name));
                    //    StartCoroutine(StartNewScene(next_scene));
                    //SendEventFirebase.SendEventUnitComplete();
                    //}
                }
                else
                    BackToLastScene(caller_scene, is_using_pop_up: false, backing_level: back_lvl);
            }
        }
        else
        {
            Debug.Log("Next overide != null!");
            StartCoroutine(StartNewScene(next_override));
        }
    }

    public void OpenParentCheck(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenParentCheckSceneSequence(scene_to_destroy));
    }

    IEnumerator OpenParentCheckSceneSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_ParentCheckToSettingsScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    public void OpenParentCheckToIAP(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenParentCheckToIAPSceneSequence(scene_to_destroy));
    }

    IEnumerator OpenParentCheckToIAPSceneSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        PopupManagerBehavior.GetInstance().HidePremiumPopUp();
        StartCoroutine(StartNewScene(m_ParentCheckToIAPScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    public void OpenIAPScreen(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenIAPScreenSequence(scene_to_destroy));
    }

    IEnumerator OpenIAPScreenSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_IAPScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    public void OpenActivationClickActiveScreen(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenActivationClickActiveScreenSequence(scene_to_destroy));
    }

    IEnumerator OpenActivationClickActiveScreenSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_ActivationClickActiveScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    public void OpenActivationWhenLoginScreen(GameObject scene_to_destroy)
    {
        StartCoroutine(OpenActivationWhenLoginScreenSequence(scene_to_destroy));
    }

    IEnumerator OpenActivationWhenLoginScreenSequence(GameObject scene_to_destroy)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        GlobalCurtain.GetInstance().HideCurtain();
        if (scene_to_destroy)
        {
            Destroy(scene_to_destroy);
        }

        DestroyCurrentScene();
        /*yield return */
        StartCoroutine(StartNewScene(m_ActivationWhenLoginScene));
        //yield return new WaitForSeconds(1.0f);
        //FadeLoadingScreenOut();
    }

    //NOTE: do not use these, these are just notes to keep track of app mapsite
    //public enum SceneDepthLevels
    //{
    //    ACTIVITY = 0,
    //    ACTIVITY_LIST = 1,
    //    LESSON_LIST = 2,
    //    KIDZONE = 3,
    //}
    public void BackFromIAP()
    {
        if (previousScene != null && m_ParentCheckToIAPScene != null && previousScene.AssetGUID.Equals(m_ParentCheckToIAPScene.AssetGUID))
        {
            OpenMainWorldMapScene(null);
        }
        else
        {
            BackToLastScene();
        }
    }

    GameObject currentCaller = null;
    public void BackToLastScene(GameObject caller = null, bool is_using_pop_up = false, int backing_level = 1)
    {
        Debug.Log("Scene Manager received back signal!!! Flag: " + is_using_pop_up);

        if (is_using_pop_up == true)
        {
            m_BackToLastScenePopUp.transform.parent.gameObject.SetActive(true);
            m_BackToLastScenePopUp.SetActive(true);
            currentCaller = caller;
        }
        else
        {
            if (backToLastSceneSequence != null)
                return;

            backToLastSceneSequence = StartCoroutine(BackToLastSceneSequence(caller, backing_level));
        }
    }

    public void OnBackToLastSceneConfirmed()
    {
        if (backToLastSceneSequence != null)
            return;
        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("game_activity", "on_confirm_back");
        if (currentCaller != null)
        {
            SendEventFirebase.SendEventActivityPlay(currentCaller.name, "game", UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
        }
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
        //string actNameOrigin = currentCaller.name;
        //string[] nameAct = actNameOrigin.Split("Clone");
        //SendEventFirebase.SendEventActivityPlay(currentCaller.name, UserDataManagerBehavior.GetInstance().GetRecordTime().ToString());
        backToLastSceneSequence = StartCoroutine(BackToLastSceneSequence(currentCaller));
    }

    Coroutine backToLastSceneSequence = null;
    IEnumerator BackToLastSceneSequence(GameObject caller = null, int backing_level = 1)
    {
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);

        yield return new WaitForEndOfFrame();
        if (backing_level == 1)
        {
            if (caller != null)
                Destroy(caller);

            if (currentObject != null)
                Destroy(currentObject);

            if (previousScene != null)
            {
                backToLastSceneSequence = null;
                yield return StartCoroutine(StartNewScene(previousScene));
            }
            else
            {
                Debug.LogError("Error: last scene is null. Scene changing failed.");
            }

            backToLastSceneSequence = null;
        }
        else if (backing_level == 2) // end lesson
        {
            Debug.Log("End of lesson!");
            if (caller != null)
                Destroy(caller);

            if (currentObject != null)
                Destroy(currentObject);

            if (previousScene != null)
            {
                backToLastSceneSequence = null;
                //GetStarLesson();
                yield return StartCoroutine(StartNewScene(previousScene));
            }
            else
            {
                Debug.LogError("Error: last scene is null. Scene changing failed.");
            }

            if (currentObject != null &&
                currentObject.GetComponent<ActivityChoosingWindowBehavior>() != null)
            {
                currentObject.GetComponent<ActivityChoosingWindowBehavior>().OnBackToLessonButtonClicked();
            }

            backToLastSceneSequence = null;
        }
        else if (backing_level == 3) // end unit
        {
            if (caller != null)
                Destroy(caller);

            if (currentObject != null)
                Destroy(currentObject);

            if (previousScene != null)
            {
                backToLastSceneSequence = null;
                //SendEventFirebase.SendEventUnitComplete(previousScene.editorAsset.name, 0ddd);
                yield return StartCoroutine(StartNewScene(previousScene));
            }
            else
            {
                Debug.LogError("Error: last scene is null. Scene changing failed.");
            }

            if (currentObject != null &&
                currentObject.GetComponent<ActivityChoosingWindowBehavior>() != null)
            {
                currentObject.GetComponent<ActivityChoosingWindowBehavior>().OnBackToWorldMapClicked();
            }

            backToLastSceneSequence = null;
        }
        yield return new WaitForEndOfFrame();
        //FadeLoadingScreenOut();
        backToLastSceneSequence = null;
    }

    bool log_out_in_progress = false;
    public void OnLogOut()
    {
        if (log_out_in_progress == true)
            return;

        PlayerPrefs.DeleteKey("USER_TOKEN");
        PlayerPrefs.DeleteAll();
        StartCoroutine(OnLogOutSequence());
    }

    IEnumerator OnLogOutSequence()
    {
        log_out_in_progress = true;
        FadeLoadingScreenIn();
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);

        PlayerPrefs.DeleteKey("USER_TOKEN");
        PlayerPrefs.DeleteAll();

        if (currentObject != null)
            Destroy(currentObject);
        yield return StartCoroutine(StartNewScene(m_LoginScenePrefab));

        if (currentObject != null &&
                currentObject.GetComponent<LogInSceneScreensManager>() != null)
        {
            currentObject.GetComponent<LogInSceneScreensManager>().OnCurrentUserClicked();
        }

        yield return new WaitForEndOfFrame();
        //FadeLoadingScreenOut();
        log_out_in_progress = false;
    }

    public bool CheckIfDuplucateMapping(SceneMappingData data)
    {
        bool ret = false;
        foreach (SceneMappingData d in sceneMappingDatas)
        {
            if (data.root.AssetGUID == d.root.AssetGUID)
            {
                ret = true;
                break;
            }
        }
        return ret;
    }

    public IEnumerator ChangeToNewScene(AssetReference boatPrefab, GameObject caller_scene)
    {
        FadeLoadingScreenIn();
        Debug.Log("SceneManagerBehavior.ChangeToNewScene: " + boatPrefab + ", caller: " + caller_scene?.name);
        yield return new WaitForSeconds(m_LoadingScreenFadingDuration);
        if (caller_scene != null) Destroy(caller_scene);
        yield return StartCoroutine(StartNewScene(boatPrefab));
        //yield return new WaitForSeconds(1.0f);

        for (var i = 0; i < LoadedPrefabs.Count; i++)
        {
            var af = LoadedPrefabs[i];
            if (af.IsValid())
            {
                Debug.Log("Release Asset: " + af);
                Addressables.Release(af);
                i--;
            }
        }
        //FadeLoadingScreenOut();
    }

    IEnumerator GetStarLesson(string name)
    {
        yield return null;
        DataBaseInterface.GetInstance().GetJSONRequest(
           DataBaseInterface.GET_LESSON_DETAIL_URI + UserDataManagerBehavior.GetInstance().currentSelectedLessonID,
           callback_flag =>
           {
               if (callback_flag == false)
                   Debug.Log("Error: get lesson detail failed.");
           },
           server_reply =>
           {
               int sum = 0;
               LessonDetailInfo lessonDetail = server_reply.data.lesson;
               for (int i = 0; i < lessonDetail.activities.Length; i++)
               {
                   sum += int.Parse(lessonDetail.activities[i].archive_star);
                   //Debug.Log("Sum: " + sum);
               }
               Debug.Log("End Lesson: " + sum);
               SendEventFirebase.SendEventLessonComplete(name, sum);
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

    IEnumerator GetStarUnit(string name)
    {
        yield return null;
        DataBaseInterface.GetInstance().GetJSONRequest(
           DataBaseInterface.GET_UNIT_DETAIL_URI + UserDataManagerBehavior.GetInstance().currentSelectedUnitID,
           callback_flag =>
           {
               if (callback_flag == false)
                   Debug.Log("Error: get lesson detail failed.");
           },
           server_reply =>
           {
               int sum = 0;
               UnitDetailsInfo unitDetailsInfo = server_reply.data.unit;
               for (int i = 0; i < unitDetailsInfo.lessons.Length; i++)
               {
                   DataBaseInterface.GetInstance().GetJSONRequest(
                   DataBaseInterface.GET_LESSON_DETAIL_URI + unitDetailsInfo.lessons[i]._id,
                   callback_flag =>
                   {
                       if (callback_flag == false)
                           Debug.Log("Error: get lesson detail failed.");
                   },
                   server_reply =>
                   {
                       int sum1 = 0;
                       LessonDetailInfo lessonDetail = server_reply.data.lesson;
                       for (int i = 0; i < lessonDetail.activities.Length; i++)
                       {
                           sum1 += int.Parse(lessonDetail.activities[i].archive_star);
                           //Debug.Log("Sum: " + sum);
                       }
                       sum += sum1;
                   },
                   UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
                   );
               }

               Debug.Log("End Unit: " + sum);
               if (sum == 63)
               {
                   SendEventFirebase.SendEventUnitComplete(name, sum);
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
                   SendEventFirebase.SendEventUnitComplete(name, sum, "not_passed");
                   /*
                   SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
               }
           },
           UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
           );
    }

    //Sử dụng để truyên dữ liệu từ story sang InterActivezone
    public string StoryId = "";
}

[System.Serializable]
public class SceneMappingData
{
    public string id = "";
    public string lesson_id = "";
    public AssetReference root = null;
    public AssetReference back = null;
    public AssetReference next = null;

    public SceneMappingData(string new_id, AssetReference r, AssetReference b, AssetReference n)
    {
        id = new_id;
        root = r;
        back = b;
        next = n;
    }
}