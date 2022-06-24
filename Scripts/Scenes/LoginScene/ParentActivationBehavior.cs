using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentActivationBehavior : MonoBehaviour
{
    [Header("Input Settings")]
    public InputField m_ActivationCodeInput;
    public Button m_ConfirmButton;
    public Button m_ResendCodeButton;

    [Header("Effects")]
    public ButtonLoadingIndicatorBehavior m_ButtonLoadingIndicatorBehavior;

    [Header("Popups")]
    public GameObject m_VerifyFailedPopup;
    public GameObject m_VerifySuccessPopup;

    [Header("Screen Navigation")]
    public LogInSceneScreensManager m_SceneManager;

    [Header("Kid info settings")]
    public CreateKidBehavior m_CreateKidBehavior;

    private void OnEnable()
    {
        m_ConfirmButton.interactable = true;
        m_ResendCodeButton.interactable = true;
        transform.localScale = Vector3.zero;
        Begin();
    }
    private void Begin()
    {
        GlobalCurtain.GetInstance().CloseCurtain();
        m_CreateKidBehavior.GetKidInfo();
        //adding kid
        UserDataManagerBehavior.GetInstance().OnCreateKid(
            m_CreateKidBehavior.GetKidInfo(),
            verify_flag =>
            {
                if (verify_flag)
                {
                    m_SceneManager.OnLogInSuccess();
                }
            },
            null
            );
    }
    public void OnActivationComplete()
    {
        //m_SceneManager.OnLogInSuccess();
        //todo
        //m_SceneManager.OnUserChoosingClicked();
        CustomEventManager.GetInstance().ParentLoggedIn();
        //todo NOBITA dan vao 1 dua
    }

    public void OnActivationCodeConfirmed()
    {
        m_ConfirmButton.interactable = false;
        m_ResendCodeButton.interactable = false;
        m_ButtonLoadingIndicatorBehavior.StartAnimation();
        
        UserDataManagerBehavior.GetInstance().OnParentActivation(m_ActivationCodeInput.text, null,
            reply_message =>
            {
                if (reply_message.statusCode.Contains(DataBaseInterface.VERIFY_EMAIL_FAILED) == true)
                {
                    m_VerifyFailedPopup.SetActive(true);

                    m_ConfirmButton.interactable = true;
                    m_ResendCodeButton.interactable = true;
                    m_ButtonLoadingIndicatorBehavior.StopAnimation();
                }
                else if (reply_message.statusCode.Contains(DataBaseInterface.VERIFY_EMAIL_SUCCESS) == true)
                {
                    m_VerifySuccessPopup.SetActive(true);
                    //UserDataManagerBehavior.GetInstance().SetUserLogInState(EngKidAPI.UserLogInStates.LOGGED_IN);
                    //CustomEventManager.GetInstance().ParentLoggedIn();

                    UserDataManagerBehavior.GetInstance().currentParentSessionIDToken = reply_message.data.token;
                    //Debug.Log(UserDataManagerBehavior.GetInstance().currentParentSessionIDToken);

                    m_CreateKidBehavior.GetKidInfo();

                    //adding kid
                    Debug.Log(m_CreateKidBehavior.GetKidInfo().language);
                    UserDataManagerBehavior.GetInstance().OnCreateKid(
                        m_CreateKidBehavior.GetKidInfo(),
                        verify_flag =>
                        {
                            if (verify_flag)
                            {
                                Debug.Log("Created kid successfully.");
                            }
                            else
                            {
                                Debug.Log("Kid is not created.");
                            }
                        },
                        null
                        );
                }
            }
            );
    }
}
