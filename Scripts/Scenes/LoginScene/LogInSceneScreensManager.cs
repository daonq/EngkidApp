using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LogInSceneScreensManager : MonoBehaviour
{
    [Header("Linked scene")]
    public AssetReference m_MainKidZoneScene;

    [Header("Canvas")]
    public Canvas m_MainCanvas;

    [Header("Windows List")]
    public GameObject m_OptionWindow;
    public GameObject m_LogInWindow;
    public GameObject m_UserSelectionWindow;
    public GameObject m_IntroductionSlideWindow;
    public GameObject m_SignUpWindow;
    public GameObject m_CreateKidWindow;
    public GameObject m_TestPageWindow;

    [Header("Introduction slides")]
    public List<GameObject> m_IntroSlidePagesList = new List<GameObject>();
    public List<Image> m_IntroSlidePageIndicatorsList = new List<Image>();

    public SoundHolder m_SoundHolder;
    
    private void Awake()
    {
        if(Debug.isDebugBuild) Debug.Log("LogInSceneScreensManager Loaded");
    }

    private void OnEnable()
    {
        CustomEventManager.GetInstance().onParentLoggedIn += OnLogInSuccess;
        CustomEventManager.GetInstance().onParentLoggedInWithToken += OnLogInSuccessWithToken;
    }

    private void OnDisable()
    {
        CustomEventManager.GetInstance().onParentLoggedIn -= OnLogInSuccess;
        CustomEventManager.GetInstance().onParentLoggedInWithToken -= OnLogInSuccessWithToken;
    }

    // Start is called before the first frame update
    void Start()
    {
        //TODO: adjust canvas based on screen ratio
        if (Camera.main.aspect >= 1.7)
        {
            m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        }
        //else if (Camera.main.aspect >= 1.4)
        //{
        //    m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
        //}
        else
        {
            m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        }


        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);

        CheckUserLogin();
    }

    private void Update()
    {
        //TODO: adjust canvas based on screen ratio
        if (Camera.main.aspect >= 1.7)
        {
            m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        }
        //else if (Camera.main.aspect >= 1.4)
        //{
        //    m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
        //}
        else
        {
            m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        }
    }

    public void OnTrySignIn()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(true);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnBackToCreateKid()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(true);
        m_TestPageWindow.SetActive(false);
    }

    public void OnNewUserClicked()
    {
        //TODO: lead to intro slide -> sign up
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(true);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnCurrentUserClicked()
    {
        //TODO: lead to sigh in
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(true);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnUserChoosingClicked()
    {
        StartCoroutine(SceneManagerBehavior.GetInstance().ChangeToNewScene(m_MainKidZoneScene, this.gameObject));
    }

    public void OnLogInSuccess()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(true);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);

        m_UserSelectionWindow.GetComponent<KidSelectBehavior>().OnGetKidsList();
    }
    public void OnCreateKidBeforeLogIn()
    {
        m_SignUpWindow.SetActive(true);
        m_SignUpWindow.transform.GetChild(0).gameObject.SetActive(true);
        m_UserSelectionWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
    }

    public void OnCreateKidAfterLogIn()
    {
        m_SignUpWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(true);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);

        m_UserSelectionWindow.GetComponent<KidSelectBehavior>().OnGetKidsList();
    }

    public void OnOptionWindow()
    {
        m_OptionWindow.SetActive(true);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnEmailExist()
    {
        string text = m_SignUpWindow.GetComponentInChildren<CreateParentBehavior>().m_EmailInput.text;
        m_LogInWindow.GetComponent<SignInBehavior>().m_IDInput.text = text;
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(true);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnPhoneExist()
    {
        string text = m_SignUpWindow.GetComponentInChildren<CreateParentBehavior>().m_PhoneInput.text;
        m_LogInWindow.GetComponent<SignInBehavior>().m_IDInput.text = text;
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(true);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    //private void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.isKey)
    //    {
    //        if (e.type.Equals(EventType.KeyDown))
    //        {
    //            if (e.keyCode != (KeyCode.None))
    //            {
    //                //Debug.Log("Detected key code: " + e.keyCode);
    //                m_SoundHolder.PlaySoundKeyboard();
    //            }
    //        }

    //    }

    //}

    public void OnLogInSuccessWithListChild()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(true);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnLogInSuccessWithAChild()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(false);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);
    }

    public void OnLogInSuccessWithToken()
    {
        m_OptionWindow.SetActive(false);
        m_LogInWindow.SetActive(false);
        m_UserSelectionWindow.SetActive(true);
        m_IntroductionSlideWindow.SetActive(false);
        m_SignUpWindow.SetActive(false);
        m_CreateKidWindow.SetActive(false);
        m_TestPageWindow.SetActive(false);

        UserDataManagerBehavior.GetInstance().SetUserLogInState(EngKidAPI.UserLogInStates.LOGGED_IN);
        m_UserSelectionWindow.GetComponent<KidSelectBehavior>().OnGetKidsList();
    }

    private void CheckUserLogin()
    {
        if (!PlayerPrefs.GetString("USER_TOKEN", "").Equals(""))
        {
            m_OptionWindow.SetActive(false);
            m_UserSelectionWindow.SetActive(true);
        }
        else
        {
            m_OptionWindow.SetActive(true);
            m_UserSelectionWindow.SetActive(false);
        }
    }

}
