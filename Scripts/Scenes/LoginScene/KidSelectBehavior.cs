using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KidSelectBehavior : MonoBehaviour
{
    [Header("Scene manager")]
    public LogInSceneScreensManager m_SceneManager;
    public GameObject m_CreateNewKidWindow;

    [Header("Users Choosing Settings")]
    public GameObject m_UserAvatarPrefab;
    public List<GameObject> m_UserAvatarsList = new List<GameObject>();
    public Transform m_UserButtonsHolder;

    [Header("Select failed popup")]
    public GameObject m_SelectFailedPopup;
    public Button m_CloseAppButton;

    //internals
    string currentChosenKidAccountID = "";
    ReturnedAccount[] accounts = null;

    public void OnGetKidsList()
    {
        foreach (Transform child in m_UserButtonsHolder)
            Destroy(child.gameObject);
        m_UserAvatarsList.Clear();

        UserDataManagerBehavior.GetInstance().OnGetKidsList(
            null,
            server_reply =>
            {
                if (server_reply != null)
                {
                    accounts = server_reply.data.accounts;

                    foreach (ReturnedAccount account in server_reply.data.accounts)
                    {
                        //Debug.Log(account.avatar);
                        GameObject kid_avatar = Instantiate(m_UserAvatarPrefab, m_UserButtonsHolder);
                        m_UserAvatarsList.Add(kid_avatar);
                        kid_avatar.GetComponent<KidChoosingAvatarBehavior>().OnSetDisplayData(account.avatar, account.name, account._id);
                    }

                    currentChosenKidAccountID = server_reply.data.accounts[0]._id;

                    if (accounts.Length == 1)
                    {
                        OnSetChosenKid(currentChosenKidAccountID);
                        OnUserChoosingClicked();
                        m_SceneManager.OnLogInSuccessWithAChild();
                    }
                    else if (accounts.Length > 1)
                    {
                        m_SceneManager.OnLogInSuccessWithListChild();

                        //TODO: change these later
                        OnSetChosenKid(currentChosenKidAccountID);
                        OnUserChoosingClicked();
                        m_SceneManager.OnLogInSuccessWithAChild();
                    }
                }
                //else
                //{
                //    //PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
                //    //Debug.Log("Get kid list failed, error: " + server_reply.message);
                //}
            }
            );
    }

    public void OnSetChosenKid(string id)
    {
        currentChosenKidAccountID = id;

        foreach (ReturnedAccount account in accounts)
        {
            if (account._id.Equals(currentChosenKidAccountID) == true)
            {
                UserDataManagerBehavior.GetInstance().currentSelectedKidID = account._id;
                UserDataManagerBehavior.GetInstance().currentSelectedKidName = account.name;
                UserDataManagerBehavior.GetInstance().currentSelectedKidXP = account.xp.ToString();
                UserDataManagerBehavior.GetInstance().currentSelectedKidStars = account.star.ToString();
                UserDataManagerBehavior.GetInstance().currentSelectedKidAvatarURL = account.avatar;
                UserDataManagerBehavior.GetInstance().currentSelectedKidGender = account.gender.ToString();

                break;
            }
        }
    }

    public void OnUserChoosingClicked()
    {
        UserDataManagerBehavior.GetInstance().OnKidChosen(
            currentChosenKidAccountID,
            flag =>
            {
                if (flag == false)
                {
                    m_SelectFailedPopup.SetActive(true);
                    m_CloseAppButton.onClick.AddListener(() =>
                    {
                        CustomEventManager.GetInstance().OnEmergencyClose();
                    });
                }
            },
            ret_message =>
            {
                if (ret_message.statusCode == DataBaseInterface.SELECT_KID_SUCCESS)
                {
                    UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken = ret_message.data.new_token;

                    m_SceneManager.OnUserChoosingClicked();
                }
            }
            );
    }

    public void OnAddNewUserClicked()
    {
        m_CreateNewKidWindow.SetActive(true);
    }

    public void OnGetKidsListShowLast()
    {
        foreach (Transform child in m_UserButtonsHolder)
            Destroy(child.gameObject);
        m_UserAvatarsList.Clear();

        UserDataManagerBehavior.GetInstance().OnGetKidsList(
            null,
            server_reply =>
            {
                accounts = server_reply.data.accounts;

                foreach (ReturnedAccount account in server_reply.data.accounts)
                {
                    //Debug.Log(account.avatar);
                    GameObject kid_avatar = Instantiate(m_UserAvatarPrefab, m_UserButtonsHolder);
                    m_UserAvatarsList.Add(kid_avatar);
                    kid_avatar.GetComponent<KidChoosingAvatarBehavior>().OnSetDisplayData(account.avatar, account.name, account._id);
                }

                currentChosenKidAccountID = server_reply.data.accounts[0]._id;

                if (accounts.Length == 1)
                {
                    OnSetChosenKid(currentChosenKidAccountID);
                    OnUserChoosingClicked();
                    m_SceneManager.OnLogInSuccessWithAChild();
                }
                else if (accounts.Length > 1)
                {
                    m_SceneManager.OnLogInSuccessWithListChild();
                }
            }
            );
    }
}
