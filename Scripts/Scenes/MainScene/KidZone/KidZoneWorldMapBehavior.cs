using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

[RequireComponent(typeof(KidZoneWorldMapAvatarController))]
[RequireComponent(typeof(KidZoneMilestoneManager))]
public class KidZoneWorldMapBehavior : MonoBehaviour
{
    [Header("BGM Settings")]
    public AudioClip m_BGMClip;

    [Header("Avatar Settings")]
    public KidZoneWorldMapAvatarController m_AvatarController;
    public Text m_CurrentKidName;
    public RawImage m_CurrentKidAvatar;
    public Text m_CurrentKidXP;
    public Text m_CurrentKidStars;

    [Header("Units Settings")]
    public Transform m_UnitGroupsHolder;
    public List<MilestoneBehavior> m_UnitGroupsList = new List<MilestoneBehavior>();
    public List<UnitBaseBehavior> m_AllUnitsList = new List<UnitBaseBehavior>();

    [Header("Milestones Settings")]
    public Transform m_MilestoneHolder;
    public List<GameObject> m_MileStonesList = new List<GameObject>();
    public Vector3 m_MovementUnitVector = new Vector3(19.21f, 0.0f, 0.0f);
    public List<Transform> m_MileStoneDisplayHoldersList = new List<Transform>();

    [Header("Popups")]
    public GameObject m_LockedPopUp;
    public Text m_LockedTxt;

    [Header("UI")]
    public GameObject relocateButtonObj;

    [Header("Onboarding")]
    public OnboardingCanvas.OnboardingScreenController onboardingScreen;

    //internals
    EngKidAPI.UnitIDs currentUnitID = EngKidAPI.UnitIDs.NULL;
    int screenSwipeID;
    int currentDisplayedMilestoneIndex = 0;
    [HideInInspector] public int currentStandingMilestoneIndex = 0;
    KidZoneMilestoneManager milestoneManager;
    Texture2D tex = null;
    bool isDraging = false;
    Vector3 lastMousePose = Vector3.zero;
    Vector3 dragDir = Vector3.zero;
    float cameraSizeFactor;
    List<Vector3> anchorList = new List<Vector3>();
    //Coroutine moveToAnchorCor = null;

    [HideInInspector] public bool is_ready = false;

    private void Awake()
    {
        if (Debug.isDebugBuild) Debug.Log("KidZoneWorldMapBehavior Loaded");
    }

    private void Start()
    {
        m_LockedPopUp.SetActive(false);

        if (m_AvatarController == null)
            m_AvatarController = this.GetComponent<KidZoneWorldMapAvatarController>();

        if (milestoneManager == null)
            milestoneManager = this.GetComponent<KidZoneMilestoneManager>();

        onboardingScreen.InitOnboardingKidZone();

        //get all available units
        m_UnitGroupsList.Clear();
        foreach (Transform child in m_UnitGroupsHolder)
        {
            MilestoneBehavior milestone = child.GetComponent<MilestoneBehavior>();
            if (milestone != null)
                m_UnitGroupsList.Add(milestone);
        }

        m_AllUnitsList.Clear();
        foreach (MilestoneBehavior group in m_UnitGroupsList)
        {
            foreach (Transform unit in group.transform)
            {
                m_AllUnitsList.Add(unit.GetComponent<UnitBaseBehavior>());
            }
        }

        BGMManagerBehavior.GetInstance().PlayBGM(m_BGMClip);

        //TODO: adjust canvas based on screen ratio
        //if (Camera.main.aspect >= 1.7)
        //{
        //foreach (Transform tf in m_MileStoneDisplayHoldersList)
        //{
        //    tf.localPosition = new Vector3(tf.localPosition.x, 0.0f, tf.localPosition.z);
        //    tf.localScale = Vector3.one;
        //}
        //}
        //else
        //{
        foreach (Transform tf in m_MileStoneDisplayHoldersList)
        {
            tf.localPosition = new Vector3(tf.localPosition.x, -0.5f, tf.localPosition.z);
            tf.localScale = Vector3.one * 0.8f;
        }
        //}

        cameraSizeFactor = Camera.main.orthographicSize / 2.0f;

        anchorList.Clear();
        foreach (GameObject go in m_MileStonesList)
        {
            anchorList.Add(Vector3.zero - go.transform.localPosition);
        }

        if (PlayerPrefs.HasKey("NEAREST_ANCHOR"))
        {
            currentDisplayedMilestoneIndex = PlayerPrefs.GetInt("NEAREST_ANCHOR");
            currentStandingMilestoneIndex = PlayerPrefs.GetInt("NEAREST_ANCHOR");
            MoveToNearestAnchor(PlayerPrefs.GetInt("NEAREST_ANCHOR"));
        }
    }

    public void TriggerLockedPopUp(UnitBaseBehavior clicked_unit)
    {
        m_LockedPopUp.SetActive(true);
        m_LockedTxt.text = "Bé cần hoàn thành <b>Unit: " + m_AllUnitsList[m_AllUnitsList.IndexOf(clicked_unit) - 1].m_UnitNumberText.text + "</b> và đạt tối thiểu 30 sao trước.";
    }

    public void OnLockedPopUpConfirmed()
    {

    }

    public void OnParentZoneClicked()
    {
        //SceneManagerBehavior.GetInstance().OpenParentZone(this.gameObject);
        SceneManagerBehavior.GetInstance().OpenParentCheck(this.gameObject);
    }

    private void Update()
    {
        foreach (Transform tf in m_MileStoneDisplayHoldersList)
        {
            tf.localPosition = new Vector3(tf.localPosition.x, -0.5f, tf.localPosition.z);
            tf.localScale = Vector3.one * 0.8f;
        }

        relocateButtonObj.SetActive(!(currentDisplayedMilestoneIndex == currentStandingMilestoneIndex));

        if (Input.GetMouseButtonDown(0) == true && LeanTween.isTweening(m_AvatarController.tweenID) == false)
        {
            //if (moveToAnchorCor != null)
            //    StopCoroutine(moveToAnchorCor);
            if (lockDrag == false)
            {
                LeanTween.cancel(m_MilestoneHolder.gameObject);
                isMovingToNearest = false;
                isDraging = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) == true || LeanTween.isTweening(m_AvatarController.tweenID) == true)
        {
            lastMousePose = Vector3.zero;
            isDraging = false;

            //if (moveToAnchorCor != null)
            //    StopCoroutine(moveToAnchorCor);
            if (lockDrag == false)
            {
                LeanTween.cancel(m_MilestoneHolder.gameObject);
                isMovingToNearest = false;
                MoveToNearestAnchor();
            }
        }

        if (isDraging == true)
        {
            Vector3 current_mouse_pos = Input.mousePosition;

            if (lastMousePose != Vector3.zero)
            {
                dragDir = Camera.main.ScreenToWorldPoint(current_mouse_pos) - Camera.main.ScreenToWorldPoint(lastMousePose); // current_mouse_pos - lastMousePose;

                if (dragDir.magnitude <= 0.1f)
                {

                }
                else
                {
                    dragDir = new Vector3(dragDir.x, 0f, 0f);

                    if (m_MilestoneHolder.transform.localPosition.x >= 0.0f && dragDir.x > 0.0f)
                        dragDir = Vector3.zero;

                    if (m_MilestoneHolder.transform.localPosition.x <= -115.26f && dragDir.x <= 0.0f)
                        dragDir = Vector3.zero;

                    m_MilestoneHolder.transform.localPosition += dragDir /** Time.deltaTime * cameraSizeFactor*/;
                }
            }

            lastMousePose = current_mouse_pos;
        }
    }

    bool isMovingToNearest = false;
    void MoveToNearestAnchor(int index = -1)
    {
        if (isMovingToNearest == true)
            return;

        isMovingToNearest = true;
        LeanTween.cancel(m_MilestoneHolder.gameObject);

        float shortest = 99999.99f;
        int closest_index = 0;
        if (index == -1)
        {
            foreach (Vector3 pos in anchorList)
            {
                float dist = Vector3.Distance(m_MilestoneHolder.transform.localPosition, pos);
                if (dist < shortest)
                {
                    shortest = dist;
                    closest_index = anchorList.IndexOf(pos);
                }
            }
        }
        else
        {
            closest_index = index;
        }

        LeanTween.moveLocal(m_MilestoneHolder.gameObject, anchorList[closest_index], 0.5f).setOnComplete(() =>
        {
            isMovingToNearest = false;
            currentDisplayedMilestoneIndex = closest_index;
            lockDrag = false;
            PlayerPrefs.SetInt("NEAREST_ANCHOR", closest_index);
            PlayerPrefs.Save();
            is_ready = true;
        });
    }

    void OnEnable()
    {
        is_ready = false;
        Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
        //Lean.Touch.LeanTouch.OnFingerSwipe += HandleScreenSwipe;
        //Lean.Touch.LeanTouch.OnFingerSwipe += HandleScreenDrag;

        UserDataManagerBehavior.GetInstance().OnRefreshKidsList(
            callback =>
            {
                if (callback == true)
                {
                    m_CurrentKidName.text = UserDataManagerBehavior.GetInstance().currentSelectedKidName;
                    m_CurrentKidStars.text = UserDataManagerBehavior.GetInstance().currentSelectedKidStars;
                    m_CurrentKidXP.text = UserDataManagerBehavior.GetInstance().currentSelectedKidXP + " XP";

                    //Debug.Log("Gender: " + UserDataManagerBehavior.GetInstance().currentSelectedKidGender);
                    if (UserDataManagerBehavior.GetInstance().currentSelectedKidGender == "1")
                    {
                        m_AvatarController.m_AvatarObject.Skeleton.SetSkin("boy1");
                    }
                    else
                    {
                        m_AvatarController.m_AvatarObject.Skeleton.SetSkin("girl1");
                    }

                    BGMManagerBehavior.GetInstance().PlayBGM(m_BGMClip);
                    BGMManagerBehavior.GetInstance().SetBGMVolume(BGMManagerBehavior.GetInstance().m_BGMAudioSource.volume, 1.0f);
                }
            },
            null
            );
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerTap -= HandleFingerTap;
        //Lean.Touch.LeanTouch.OnFingerSwipe -= HandleScreenSwipe;
        //Lean.Touch.LeanTouch.OnFingerSwipe -= HandleScreenDrag;
    }

    void HandleScreenDrag(Lean.Touch.LeanFinger finger)
    {

    }

    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        if (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject()) return;

        if (finger.IsOverGui)
        {
            Debug.Log("You just tapped the screen on top of the GUI!");
        }
        else
        {
            //Debug.Log("you just tapped some object");

            //TODO: check if tapped on unit objects
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
            Vector2 tapPos2D = new Vector2(tapPos.x, tapPos.y);
            RaycastHit2D hit = Physics2D.Raycast(tapPos2D, Vector2.zero);
            if (hit.collider != null)
            {
                //check if clicked on an unit
                UnitBaseBehavior base_behavior = hit.transform.GetComponent<UnitBaseBehavior>();
                if (base_behavior != null)
                {
                    //Debug.Log("Hit unit: " + base_behavior.m_UnitID.ToString());
                    base_behavior.OnUnitClicked();

                    //TODO: check if the unit is unlocked
                    //only allow movement to unlocked units
                    if (base_behavior.m_UnitState != EngKidAPI.UnitStates.LOCKED)
                    {
                        //TODO: move avatar
                        //play animation jump or fly based on the distance
                        bool flag = HandleAvatarMovement(base_behavior);
                        if (flag == true)
                        {
                            currentUnitID = base_behavior.m_UnitID;
                            currentStandingMilestoneIndex = (int)base_behavior.m_Milestone;
                        }
                    }
                    else
                    {
                        //TODO: pop-up to inform user to finish previous unit/test

                    }
                }
            }
        }
    }

    public bool HandleAvatarMovement(UnitBaseBehavior base_behavior)
    {
        if (base_behavior.m_UnitID == currentUnitID)
            return false;

        PlayerPrefs.SetFloat("PLAYER_POS_X", base_behavior.transform.position.x);
        PlayerPrefs.SetFloat("PLAYER_POS_Y", base_behavior.transform.position.y);
        PlayerPrefs.SetFloat("PLAYER_POS_Z", base_behavior.transform.position.z);
        PlayerPrefs.Save();

        int move_distance = Mathf.Abs((int)currentUnitID - (int)base_behavior.m_UnitID);
        if (move_distance <= 3) //jump
        {
            return m_AvatarController.JumpToNextUnit(base_behavior.transform.position + Vector3.up * 0.1f);
        }
        else //fly
        {
            return m_AvatarController.FlyToDistanceUnit(base_behavior.transform.position + Vector3.up * 0.1f);
        }
    }

    public void HandleScreenSwipe(Lean.Touch.LeanFinger finger)
    {
        if (finger.Swipe == false)
            return;

        Vector2 swipe_dir = finger.LastScreenPosition - finger.StartScreenPosition;
        if (swipe_dir.magnitude <= 0.1f)
            return;

        if (finger.StartScreenPosition.x < finger.LastScreenPosition.x)
        {
            Debug.Log("Swipe right.");
            if (currentDisplayedMilestoneIndex <= 0)
                return;

            //TODO: move right
            if (LeanTween.isTweening(screenSwipeID) == false)
            {
                currentDisplayedMilestoneIndex--;

                Vector3 new_pos = m_MilestoneHolder.transform.position + m_MovementUnitVector;
                screenSwipeID = LeanTween.move(m_MilestoneHolder.gameObject, new_pos, 0.5f).id;
            }
        }
        else if (finger.StartScreenPosition.x > finger.LastScreenPosition.x)
        {
            Debug.Log("Swipe left.");
            if (currentDisplayedMilestoneIndex >= m_MileStonesList.Count - 1)
                return;

            //TODO: move left
            if (LeanTween.isTweening(screenSwipeID) == false)
            {
                currentDisplayedMilestoneIndex++;

                Vector3 new_pos = m_MilestoneHolder.transform.position - m_MovementUnitVector;
                screenSwipeID = LeanTween.move(m_MilestoneHolder.gameObject, new_pos, 0.5f).id;
            }
        }
    }

    bool lockDrag = false;
    public void OnRelocateButtonClicked()
    {
        //move back to where the avatar is
        //if (LeanTween.isTweening(screenSwipeID) == false)
        //{
        //    int move_multiplier = currentDisplayedMilestoneIndex - currentStandingMilestoneIndex;
        //    Vector3 new_pos = m_MilestoneHolder.transform.position + (m_MovementUnitVector * move_multiplier);
        //    screenSwipeID = LeanTween.move(m_MilestoneHolder.gameObject, new_pos, 1.0f).id;

        //    currentDisplayedMilestoneIndex = currentStandingMilestoneIndex;
        //}

        lockDrag = true;
        MoveToNearestAnchor(currentStandingMilestoneIndex);
    }

    public void OnMoveToLibraryScene()
    {
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_LibraryScene, this.gameObject));
    }

    public void OnMoveToInteractiveZone()
    {
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_InteractiveZoneScene, this.gameObject));
    }

    public void OnMoveToShopScene()
    {
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(SceneManagerBehavior.GetInstance().m_CustomizeScene, this.gameObject));
    }

    public void UpdateAllMilestonesInfo(MilestoneInfo[] milestoneInfoList, UnitResultInfo[] resultInfosList, AccountProgressInfo accountProgressInfo)
    {
        //update account display
        //Debug.Log(UserDataManagerBehavior.GetInstance().currentSelectedKidName);
        m_CurrentKidName.text = UserDataManagerBehavior.GetInstance().currentSelectedKidName;
        m_CurrentKidStars.text = UserDataManagerBehavior.GetInstance().currentSelectedKidStars;
        m_CurrentKidXP.text = UserDataManagerBehavior.GetInstance().currentSelectedKidXP + " XP";
        StartCoroutine(GetAvatarImage(UserDataManagerBehavior.GetInstance().currentSelectedKidAvatarURL));

        //init all units
        for (int i = 0; i < m_UnitGroupsList.Count; i++)
        {
            m_UnitGroupsList[i].OnInitUnitsList();
        }

        //set unit data
        for (int i = 0; i < m_UnitGroupsList.Count && i < milestoneInfoList.Length; i++)
        {
            m_UnitGroupsList[i].SetUnitsData(milestoneInfoList[i]);
        }

        //set unit progression
        for (int i = 0; i <= System.Int32.Parse(accountProgressInfo.mile_stone_mark); i++)
        {
            if (i < System.Int32.Parse(accountProgressInfo.mile_stone_mark) - 1)
                m_UnitGroupsList[i].SetUnitsStates(4);
            else
                m_UnitGroupsList[i].SetUnitsStates(System.Int32.Parse(accountProgressInfo.unit_mark));

            foreach (UnitBaseBehavior unit in m_UnitGroupsList[i].m_UnitsList)
            {
                foreach (UnitResultInfo result in resultInfosList)
                {
                    if (unit.m_UnitDatabaseID.Equals(result.unit._id) == true)
                    {
                        unit.SetStarsText(result.archived_star);
                        break;
                    }
                }
            }
        }

        //TODO: set currentUnitID == max unlocked unit
        currentUnitID = EngKidAPI.UnitIDs.NULL;
        //TODO: match this with current milestone load up from DB
        //currentDisplayedMilestoneIndex = 0;
        //currentStandingMilestoneIndex = 0;
    }

    IEnumerator GetAvatarImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request URL: " + url + "\nError: " + www.error);
        }
        else
        {
            DestroyImmediate(tex);
            tex = (DownloadHandlerTexture.GetContent(www)) as Texture2D;
            m_CurrentKidAvatar.texture = tex;
            //UserDataManagerBehavior.GetInstance().userAvatarSprite = tex;
            //DestroyImmediate(((DownloadHandlerTexture)www.downloadHandler).texture);
        }


        www.Dispose();
        www = null;
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        if (tex != null)
            DestroyImmediate(tex);

        StopAllCoroutines();
    }

    public void OnCloseApp()
    {
        Debug.Log("thoat app");
        //Application.Quit();
        QuitApp();
    }

    public void OnLogoutApp()
    {
        SceneManagerBehavior.GetInstance().OnLogOut();
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
         //System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}
