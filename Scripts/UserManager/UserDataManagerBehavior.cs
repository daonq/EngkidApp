using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class UserDataManagerBehavior : MonoBehaviour
{
    #region Singleton
    private static UserDataManagerBehavior _Instance;
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static UserDataManagerBehavior GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: UserManagerBehavior instance is null.");

        return _Instance;
    }
    #endregion

    [Header("User States")]
    EngKidAPI.UserLogInStates m_UserLogInState = EngKidAPI.UserLogInStates.NOT_LOGGED_IN;
    bool m_IsFirstStartUp = true;

    //internals
    [HideInInspector] public string currentParentSessionIDToken = "";
    string currentForgetPasswordToken = "";
    string currentParentVerifyIDToken = "";
    [HideInInspector] public string currentParentID = "";
    [HideInInspector] public string currentParentPassword = "";
    [SerializeField] public string currentSelectedKidIDToken = "";
    [HideInInspector] public string currentSelectedKidID = "";
    [HideInInspector] public string currentSelectedKidName = "";
    [HideInInspector] public string currentSelectedKidGender = "";
    [HideInInspector] public string currentSelectedKidAge = "";
    [HideInInspector] public string currentSelectedKidXP = "";
    [HideInInspector] public string currentSelectedKidStars = "";
    [HideInInspector] public string currentSelectedKidAvatarURL = "";
    [SerializeField] public string currentSelectedUnitID = "";
    [SerializeField] public string currentSelectedLessonID = "";
    [SerializeField] public string currentSelectedActivityID = "";
    [HideInInspector] public string currentSkinName = string.Empty;
    [HideInInspector] public string currentParentName = string.Empty;
    [HideInInspector] public string currentParentEmail = string.Empty;
    [HideInInspector] public string currentParentPhone = string.Empty;
    [HideInInspector] public string currentParentAvatar = string.Empty;
    [HideInInspector] public bool isFirstLoginAndShowActivation;

    //[SerializeField] public Texture userAvatarSprite = null;

    [HideInInspector] public KidAccount currentKidAccount = new KidAccount();
    [HideInInspector] public List<MilestoneInfo> currentMilestoneInfoList = new List<MilestoneInfo>();
    [SerializeField] public UnitDetailsInfo unitDetailsInfo = null;
    [HideInInspector] public int currentLessonIndex = 0;

    /*[HideInInspector]*/
    public List<string> user_premium_permission = new List<string>();
    [HideInInspector] public const string TRIAL_PERMISSION = "TRIAL";
    [HideInInspector] public const string KIDZONE_PERMISSION = "KIDZONE";
    [HideInInspector] public const string INTERACTIVE_ZONE_PERMISSION = "INTERACTIVE_ZONE";

    public const string DEFAULT_GIRL_SKIN_NAME = "girl1";
    public const string DEFAULT_BOY_SKIN_NAME = "boy1";

    private System.DateTime recordTime;

    public void SetRecordTime(System.DateTime time)
    {
        recordTime = time;
        Debug.Log("Start record time: " + time);
    }

    public int GetRecordTime()
    {
        int t = (int)(System.DateTime.Now - recordTime).TotalSeconds;
        if (t > 0) return t;
        return 0;
    }

    private void Start()
    {
        currentSkinName = string.Empty;
        user_premium_permission.Clear();
        FB.Mobile.SetAdvertiserTrackingEnabled(true);
    }

    private void OnEnable()
    {

    }

    public bool IsPremiumAccount()
    {
        return user_premium_permission.Count > 0 && !user_premium_permission.Contains(TRIAL_PERMISSION);
    }

    public void SetPremiumPermission(string[] data_in)
    {
        if (data_in != null)
        {
            user_premium_permission.Clear();
            foreach (string permission in data_in)
            {
                user_premium_permission.Add(permission);
            }
        }

        //TODO: may be removed later
        if (user_premium_permission.Contains(TRIAL_PERMISSION) == true)
        {
            if (user_premium_permission.Contains(KIDZONE_PERMISSION))
                user_premium_permission.Remove(KIDZONE_PERMISSION);

            if (user_premium_permission.Contains(INTERACTIVE_ZONE_PERMISSION))
                user_premium_permission.Remove(INTERACTIVE_ZONE_PERMISSION);
        }
        //-----------------------------------------------------------------
    }

    public void OnLogOut()
    {
        m_UserLogInState = EngKidAPI.UserLogInStates.NOT_LOGGED_IN;

        currentParentID = "";
        currentParentPassword = "";
        currentSelectedKidIDToken = "";
        currentSelectedKidID = "";
        currentSelectedKidName = "";
        currentSelectedKidGender = "";
        currentSelectedKidAge = "";
        currentSelectedKidXP = "";
        currentSelectedKidStars = "";
        currentSelectedKidAvatarURL = "";
        currentSelectedUnitID = "";
        currentSelectedLessonID = "";
        currentSelectedActivityID = "";
        currentSkinName = string.Empty;
        currentParentName = string.Empty;
        currentParentEmail = string.Empty;
        currentParentPhone = string.Empty;


        currentKidAccount = new KidAccount();
        currentMilestoneInfoList.Clear();
        unitDetailsInfo = null;
    }

    public int GetMileStoneIndexFromID(string id)
    {
        //Debug.Log("mile stone ID: " + id + " /" + currentMilestoneInfoList.Count);

        int ret = -1;
        foreach (MilestoneInfo milestone in currentMilestoneInfoList)
        {
            if (milestone._id.Equals(id))
            {
                ret = currentMilestoneInfoList.IndexOf(milestone);
                break;
            }
        }

        //Debug.Log("Found milestone at index: " + ret);
        return ret;
    }

    public int GetMileStoneProgressMarkFromID(string id)
    {
        //Debug.Log("mile stone ID: " + id + " /" + currentMilestoneInfoList.Count);
        int ret = -1;
        //Debug.Log("mile stone ID: " + id + " has mark: " + int.Parse(currentMilestoneInfoList[GetMileStoneIndexFromID(id)].progress_mark));
        ret = int.Parse(currentMilestoneInfoList[GetMileStoneIndexFromID(id)].progress_mark);

        return ret;
    }

    public bool CheckIfAppHasBeenUsedBefore()
    {
        return m_IsFirstStartUp;
    }

    public EngKidAPI.UserLogInStates GetUserLogInState()
    {
        return m_UserLogInState;
    }

    public void SetUserLogInState(EngKidAPI.UserLogInStates state)
    {
        m_UserLogInState = state;
    }

    public void OnParentLogIn(string parent_id, string parent_pw, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        ParentLogInInfo parent_login_info = new ParentLogInInfo(parent_id, parent_pw);
        DataBaseInterface.GetInstance().PostJSONRequest(DataBaseInterface.LOGIN_URI,
                        JsonUtility.ToJson(parent_login_info),
                        log_in_flag =>
                        {
                            if (callback != null)
                            {
                                callback.Invoke(log_in_flag);
                            }
                        },
                        server_reply =>
                        {
                            if (ret_message != null && server_reply != null)
                            {
                                ret_message.Invoke(server_reply);
                                currentParentVerifyIDToken = server_reply.data.verify_token;
                                //Debug.Log(currentParentVerifyIDToken);

                                //log in section
                                currentParentSessionIDToken = server_reply.data.token;
                                //Debug.Log(currentParentSessionIDToken);
                                Debug.Log("___________________parent information!!!!!!!!!!");
                                currentParentName = server_reply.data.userData.name;
                                currentParentPhone = server_reply.data.userData.phone;
                                currentParentEmail = server_reply.data.userData.email;
                                currentParentAvatar = server_reply.data.userData.avatar;
                                PlayerPrefs.SetString("ACTIVATION_PARENT_NAME", currentParentName);
                                PlayerPrefs.SetString("ACTIVATION_PARENT_PHONE", currentParentPhone);
                                PlayerPrefs.SetString("ACTIVATION_PARENT_EMAIL", currentParentEmail);
                                PlayerPrefs.SetString("ACTIVATION_PARENT_AVATAR", currentParentAvatar);

                                SendEventFirebase.SendEventLoginSuccess(currentParentEmail, currentParentPhone);
                            }
                        }
                        );

        //Debug.Log(JsonUtility.ToJson(parent_login_info));
    }

    public void OnParentActivation(string activation_code, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        VerifyEmailInfo verify_info = new VerifyEmailInfo(currentParentVerifyIDToken, activation_code);
        //Debug.Log(verify_info.code + " " + verify_info.token);
        DataBaseInterface.GetInstance().PostJSONRequest(DataBaseInterface.VERIFY_EMAIL_URI,
            JsonUtility.ToJson(verify_info),
            verify_flag =>
            {
                if (callback != null)
                {
                    callback.Invoke(verify_flag);
                }
            },
            reply_message =>
            {
                if (ret_message != null)
                {
                    ret_message.Invoke(reply_message);
                }
            }
            );
    }

    public void OnCreateKid(KidInfo kid_info, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.CREATE_KID_URI,
            JsonUtility.ToJson(kid_info),
            verify_flag =>
            {
                if (callback != null)
                {
                    callback.Invoke(verify_flag);
                }
            },
            reply_message =>
            {
                if (ret_message != null)
                {
                    ret_message.Invoke(reply_message);
                }
            },
            currentParentSessionIDToken
            );
    }

    public void OnForgetPasswordClicked(string email, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(DataBaseInterface.FORGET_PASSWORD_URI,
                        JsonUtility.ToJson(new ForgetPasswordInfo(email)),
                        forget_pw_flag =>
                        {
                            if (callback != null)
                            {
                                callback.Invoke(forget_pw_flag);
                            }
                        },
                        return_token =>
                        {
                            currentForgetPasswordToken = return_token.data.reset_token;
                        });
    }

    public void OnSixDigitsCodeConfirmed(string six_digits_code, System.Action<bool> callback = null)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(DataBaseInterface.CONFIRMING_FORGET_PASSWORD_SIX_DIGITS_CODE_URI,
            JsonUtility.ToJson(new ChangePasswordCodeConfirmingInfo(six_digits_code, currentForgetPasswordToken)),
            confirm_code_flag =>
            {
                if (callback != null)
                    callback.Invoke(confirm_code_flag);
            }
            );
    }

    public void OnSetNewPassword(string new_password, System.Action<bool> callback = null)
    {
        DataBaseInterface.GetInstance().PutJSONRequest(DataBaseInterface.SET_NEW_PASSWORD_URI,
            JsonUtility.ToJson(new SetNewPasswordInfo(new_password, currentForgetPasswordToken)),
            confirm_code_flag =>
            {
                if (callback != null)
                    callback.Invoke(confirm_code_flag);
            }
            );
    }

    public void OnUpdateChildrenInformation(string new_name, string new_avatar, string new_age_group_id, System.Action<bool> callback = null)
    {
        DataBaseInterface.GetInstance().PutJSONRequest(DataBaseInterface.UPDATE_CHILD_INFORMATION,
            JsonUtility.ToJson(new ChildInfo(new_name, new_avatar, new_age_group_id)),
            confirm_code_flag =>
            {
                if (callback != null)
                    callback.Invoke(confirm_code_flag);
            },
            null,
            currentSelectedKidIDToken
            );
    }

    public void OnChangePassword(string new_pass, string current_pass, System.Action<bool> callback = null)
    {
        DataBaseInterface.GetInstance().PutJSONRequest(DataBaseInterface.CHANGE_PASSWORD,
            JsonUtility.ToJson(new ChangePassword(current_pass, new_pass)),
            confirm_code_flag =>
            {
                if (callback != null)
                    callback.Invoke(confirm_code_flag);
            },
            null,
            currentSelectedKidIDToken
            );
    }

    public void OnUpdateParentInformation(string new_name, string new_avatar, System.Action<bool> callback = null)
    {
        DataBaseInterface.GetInstance().PutJSONRequest(DataBaseInterface.UPDATE_PARENT_INFORMATION,
            JsonUtility.ToJson(new UpdateDataParent(new_name, new_avatar)),
            confirm_code_flag =>
            {
                if (callback != null)
                    callback.Invoke(confirm_code_flag);
            },
            null,
            currentSelectedKidIDToken
            );
    }

    public void OnPostLearnTimeRecord(string module_name, string info)
    {
        int timeCurrent = (int)(System.DateTime.Now - recordTime).TotalSeconds;
        if (timeCurrent < 0) return;

        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.LEARN_TIME_RECORD,
            JsonUtility.ToJson(new LearnTimeRecord(timeCurrent.ToString(), module_name, info)),
            null,
            null,
            currentSelectedKidIDToken
            );

        Debug.Log("End record time: " + timeCurrent);
    }

    public void OnParentSignUp(ParentSignUpInfo info, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(DataBaseInterface.SIGNUP_URI,
            JsonUtility.ToJson(info),
            check_flag =>
            {
                if (callback != null)
                    callback.Invoke(check_flag);
            },
            server_reply_message =>
            {
                if (ret_message != null)
                {
                    ret_message.Invoke(server_reply_message);
                    currentParentVerifyIDToken = server_reply_message.data.verify_token;
                    //Debug.Log(currentParentVerifyIDToken);

                    //log in section
                    currentParentSessionIDToken = server_reply_message.data.token;
                    //Debug.Log(currentParentSessionIDToken);

                    //Send event firebase
                    SendEventFirebase.SendEventSignupSuccess(info.email, info.phone);
                }
            },
            "",
            DataBaseInterface.PopUpLevel.NONE
            );
    }

    public void OnGetKidsList(System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        //Debug.Log(currentParentSessionIDToken);
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_KIDS_LIST_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                {
                    //Debug.Log("Error: pulling kids list from server failed.");
                    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
                }

                if (callback != null)
                    callback.Invoke(callback_flag);
            },
            server_reply =>
            {
                //foreach (ReturnedAccount account in server_reply.data.accounts)
                //{
                //    Debug.Log(account.name);
                //}

                if (ret_message != null)
                    ret_message.Invoke(server_reply);
            },
            currentParentSessionIDToken
            );
    }

    public void OnRefreshKidsList(System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        //Debug.Log(currentParentSessionIDToken);
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.GET_KIDS_LIST_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: pulling kids list from server failed.");

                if (callback != null)
                    callback.Invoke(callback_flag);
            },
            server_reply =>
            {
                foreach (ReturnedAccount account in server_reply.data.accounts)
                {
                    if (account._id == currentSelectedKidID)
                    {
                        Debug.Log("Refreshed account: " + account.name);
                        currentSelectedKidName = account.name;
                        currentSelectedKidXP = account.xp.ToString();
                        currentSelectedKidStars = account.star.ToString();
                        currentSelectedKidAvatarURL = account.avatar;
                        currentSelectedKidAge = account.age_group_id.ToString();
                        currentSelectedKidGender = account.gender.ToString();
                        

                        if (isFirstLoginAndShowActivation)
                        {
                            //GlobalCurtain.GetInstance().TriggerCurtain();
                            //SceneManagerBehavior.GetInstance().OpenActivationWhenLoginScreen(null);
                            isFirstLoginAndShowActivation = false;
                        }
                    }
                }

                //if (ret_message != null)
                //    ret_message.Invoke(server_reply);
            },
            currentParentSessionIDToken
            );
    }

    public void OnKidChosen(string kid_id, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null)
    {
        DataBaseInterface.GetInstance().PostJSONRequest(
            DataBaseInterface.SELECT_KID_URI,
            JsonUtility.ToJson(new SelectKidInfo(kid_id)),
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: select kid failed.");

                if (callback != null)
                    callback.Invoke(callback_flag);
            },
            server_reply =>
            {
                if (ret_message != null)
                    ret_message.Invoke(server_reply);
                //GetSkinNameFromServer();
                StartCoroutine(DelayGetSkinNameFromServer());
            },
            currentParentSessionIDToken
            );
    }

    public void CheckAppUpdate()
    {
        DataBaseInterface.GetInstance().GetJSONRequest(
                DataBaseInterface.CHECK_UPDATE_URI,
                callback_flag =>
                {

                },
                server_reply =>
                {
#if UNITY_ANDROID
                    if (server_reply.data.config.android.Equals(Application.version) == false)
                    {
                        PopupManagerBehavior.GetInstance().TriggerUpdatePopUp();
                    }
#endif
#if UNITY_IOS
                    if (server_reply.data.config.ios.Equals(Application.version) == false)
                    {
                        PopupManagerBehavior.GetInstance().TriggerUpdatePopUp();
                    } 
#endif
                },
                currentSelectedKidIDToken, 
                DataBaseInterface.PopUpLevel.CANT_REACH_SERVER
                );
    }    

    public KidAccount GetCurrentKid()
    {
        return currentKidAccount;
    }

    public void UpdateCurrentKid(ReturnedAccount account)
    {
        currentKidAccount.diamond = account.diamond;
        currentKidAccount.star = account.star;
        currentKidAccount.xp = account.xp;
    }

    IEnumerator DelayGetSkinNameFromServer()
    {
        yield return null;
        yield return null;
        yield return null;
        GetSkinNameFromServer();
    }

    public void GetSkinNameFromServer()
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
                if (server_reply.data.account != null)
                {
                    if (string.IsNullOrEmpty(server_reply.data.account.outfit.body))
                    {
                        if (server_reply.data.account.gender == 1)
                        {
                            currentSkinName = DEFAULT_BOY_SKIN_NAME;
                        }
                        else
                        {
                            currentSkinName = DEFAULT_GIRL_SKIN_NAME;
                        }
                    }
                    else
                    {
                        //currentSkinName = server_reply.data.account.outfit.body;
                        GetAllCustomizeMascotInfoFromServer(server_reply.data.account.outfit.body);
                    }
                }
                else
                {
                    if (currentSelectedKidGender.Equals("1"))
                    {
                        currentSkinName = DEFAULT_BOY_SKIN_NAME;
                    }
                    else
                    {
                        currentSkinName = DEFAULT_GIRL_SKIN_NAME;
                    }
                }
            },
                currentSelectedKidIDToken
            );
    }

    public void GetAllCustomizeMascotInfoFromServer(string _str)
    {
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.CUSTOMIZE_MASCOT_GET_ALL_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get kid information fail.");
            },
            server_reply =>
            {
                for (int i = 0; i < server_reply.data.list.Length; i++)
                {
                    if (_str.Equals(server_reply.data.list[i]._id))
                    {
                        currentSkinName = server_reply.data.list[i].resource.id.Replace(" ", string.Empty);

                        Debug.Log("currentSkinName: " + currentSkinName);
                    }
                }
            },
                currentSelectedKidIDToken
                , DataBaseInterface.PopUpLevel.CANT_REACH_SERVER
            );
    }

    [HideInInspector] public string opponent_skin_name = "";
    public void GetOpponentCostumeFromServer(string costume_id, string opponent_id)
    {
        Debug.Log("Getting opponent information.");
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.CUSTOMIZE_MASCOT_GET_ALL_URI,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get opponent information fail.");
            },
            server_reply =>
            {
                for (int i = 0; i < server_reply.data.list.Length; i++)
                {
                    if (costume_id.Equals(server_reply.data.list[i]._id))
                    {
                        opponent_skin_name = server_reply.data.list[i].resource.id.Replace(" ", string.Empty);

                        Debug.Log("opponent_skin_name: " + opponent_skin_name);
                    }
                }
            },
                currentSelectedKidIDToken/*opponent_id*/
                , DataBaseInterface.PopUpLevel.CANT_REACH_SERVER
            );
    }
}

public class ParentLogInInfo : MonoBehaviour
{
    public string user_name = "";
    public string password = "";

    public ParentLogInInfo(string id, string pw)
    {
        user_name = id;
        password = pw;
    }

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }
}

public class ForgetPasswordInfo : MonoBehaviour
{
    public string email = "";

    public ForgetPasswordInfo(string mail)
    {
        email = mail;
    }

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class ForgetPasswordTokenInfo : ReturnedMessage
{
    public ForgetPasswordTokenData data;

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }

    public static ForgetPasswordTokenInfo CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<ForgetPasswordTokenInfo>(json_data);
    }
}

[System.Serializable]
public class ForgetPasswordTokenData
{
    [SerializeField] public string reset_token;

    public static ForgetPasswordTokenData CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<ForgetPasswordTokenData>(json_data);
    }
}

public class ChangePasswordCodeConfirmingInfo
{
    public string code = "";
    public string token = "";

    public ChangePasswordCodeConfirmingInfo(string new_code, string new_token)
    {
        code = new_code;
        token = new_token;
    }

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }

    public static ChangePasswordCodeConfirmingInfo CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<ChangePasswordCodeConfirmingInfo>(json_data);
    }
}

public class SetNewPasswordInfo
{
    public string password = "";
    public string token = "";

    public SetNewPasswordInfo(string new_pw, string new_token)
    {
        password = new_pw;
        token = new_token;
    }

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }

    public static SetNewPasswordInfo CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<SetNewPasswordInfo>(json_data);
    }
}

[System.Serializable]
public class ParentSignUpInfo
{
    public string email = "";
    public string phone = "";
    public string password = "";
    public string language = "";

    public ParentSignUpInfo(string em, string cell, string pw, string lang)
    {
        email = em;
        phone = cell;
        password = pw;
        language = lang;
    }

    public string GetJSONString()
    {
        return JsonUtility.ToJson(this);
    }

    public static ParentSignUpInfo CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<ParentSignUpInfo>(json_data);
    }
}

public class VerifyEmailInfo
{
    public string token = "";
    public string code = "";

    public VerifyEmailInfo(string tok, string cod)
    {
        token = tok;
        code = cod;
    }
}

public class SelectKidInfo
{
    public string account_id = "";

    public SelectKidInfo(string id)
    {
        account_id = id;
    }
}

public class ChildInfo
{
    public string name = "";
    public string avatar = "";
    public string age_group_id = "";

    public ChildInfo(string nam, string avata, string age)
    {
        name = nam;
        avatar = avata;
        age_group_id = age;
    }
}

public class ChangePassword
{
    public string password = "";
    public string new_password = "";

    public ChangePassword(string pass, string new_pass)
    {
        password = pass;
        new_password = new_pass;
    }
}

public class UpdateDataParent
{
    public string name = "";
    public string avatar = "";

    public UpdateDataParent(string new_name, string new_avatar)
    {
        name = new_name;
        avatar = new_avatar;
    }
}

public class LearnTimeRecord
{
    public string amount = "";
    public string module_name = "";
    public string info = "";

    public LearnTimeRecord(string _amount, string _module, string _info)
    {
        amount = _amount;
        module_name = _module;
        info = _info;
    }
}
