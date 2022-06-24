using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(UnitDisplays))]
public class UnitBaseBehavior : MonoBehaviour
{
    [Header("Display settings")]
    public bool m_IsTest = false;
    public UnitDisplays m_DisplayController;
    public Text m_UnitStarsText;
    public Text m_UnitNumberText;

    [Header("Unit settings")]
    public bool m_IsFreemium = false;
    public string m_UnitDatabaseID = "";
    public string m_UnitTitle = "";
    public EngKidAPI.MilestoneIDs m_Milestone = EngKidAPI.MilestoneIDs.MILESTONE_1;
    public EngKidAPI.UnitIDs m_UnitID = EngKidAPI.UnitIDs.NULL;
    public EngKidAPI.UnitStates m_UnitState = EngKidAPI.UnitStates.LOCKED;
    public EngKidAPI.UnitProgression m_UnitProgression = EngKidAPI.UnitProgression.NOT_STARTED;
    [Range(0, 99)] public int m_CurrentStarsAnount = 0;
    [Range(0, 99)] public int m_MaxStarsAmount = 63;

    [Header("Lessons Setting")]
    public AssetReference m_LessonWindow;

    //internal
    GameObject currentActiveLesson = null;
    
    //private void Awake()
    //{
    //    //if(Debug.isDebugBuild) Debug.Log("UnitBaseBehavior Loaded");
    //}

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (m_DisplayController == null)
            m_DisplayController = this.GetComponent<UnitDisplays>();

        if (m_IsTest == false)
        {
            m_DisplayController.InitDisplay((int)m_UnitID, m_CurrentStarsAnount, m_MaxStarsAmount, m_UnitState);
        }
        else
        {
            //TODO: display test unit info



        }
    }

    public void OnUnitClicked()
    {
        if (m_DisplayController == null)
            m_DisplayController = this.GetComponent<UnitDisplays>();
        m_DisplayController.OnUnitClicked();

        if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.KIDZONE_PERMISSION) == true)
        {
            if (m_IsTest)
            {
                PopupManagerBehavior.GetInstance().TriggerConstructionPopUp();
                return;
            }

            if (m_UnitState == EngKidAPI.UnitStates.UNLOCKED)
            {
                StartCoroutine(DelayedChangeToLessonScene());
                //Debug.Log(m_UnitDatabaseID);
                UserDataManagerBehavior.GetInstance().currentSelectedUnitID = m_UnitDatabaseID;
            }
            else
            {
                //TODO: unit is locked popup
                GameObject.FindGameObjectWithTag("KidZoneManager").GetComponent<KidZoneWorldMapBehavior>().TriggerLockedPopUp(this);
            }
        }
        else if (UserDataManagerBehavior.GetInstance().user_premium_permission.Contains(UserDataManagerBehavior.TRIAL_PERMISSION) == true)
        {
            if (m_IsFreemium == true)
            {
                if (m_IsTest)
                {
                    PopupManagerBehavior.GetInstance().TriggerConstructionPopUp();
                    return;
                }

                if (m_UnitState == EngKidAPI.UnitStates.UNLOCKED)
                {
                    StartCoroutine(DelayedChangeToLessonScene());
                    //Debug.Log(m_UnitDatabaseID);
                    UserDataManagerBehavior.GetInstance().currentSelectedUnitID = m_UnitDatabaseID;
                }
                else
                {
                    //TODO: unit is locked popup
                    GameObject.FindGameObjectWithTag("KidZoneManager").GetComponent<KidZoneWorldMapBehavior>().TriggerLockedPopUp(this);
                }
            }
            else
            {
                PopupManagerBehavior.GetInstance().TriggerPremiumPopUp();
                SendEventFirebase.SendEventProductCheckout("kid_zone");
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
            SendEventFirebase.SendEventProductCheckout("kid_zone");
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

    IEnumerator DelayedChangeToLessonScene()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_LessonWindow, null));
    }

    public void SetStarsText(string star_text)
    {
        if (m_UnitStarsText != null)
            m_UnitStarsText.text = star_text + "/" + m_MaxStarsAmount;
    }

    public void SetUnitNumberText(string number)
    {
        if (m_UnitNumberText != null)
            m_UnitNumberText.text = number;
    }    
}
