using CustomizeMascot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataBaseInterface : MonoBehaviour
{
    #region Singleton
    private static DataBaseInterface _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static DataBaseInterface GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: DataBaseInterface instance is null.");

        return _Instance;
    }
    #endregion

    [Header("Constants")]
    public const string INCORRECT_LOG_IN = "100";
    public const string LOG_IN_SUCCESSFUL = "200";
    public const string EMAIL_UNVERIFIED_CODE = "101";
    public const string EXISTED_PHONE = "102";
    public const string EXISTED_EMAIL = "103";
    public const string SIGN_UP_SUCCESSFUL = "200";
    public const string VERIFY_EMAIL_FAILED = "400";
    public const string VERIFY_EMAIL_SUCCESS = "200";
    public const string SELECT_KID_SUCCESS = "200";
    public const string SELECT_KID_FAILED = "400";

    //public const string SERVER_DOMAIN = "https://stg-engkid.x3english.com/api/";
    public const string SERVER_DOMAIN = "https://app.engkid.com/api/";
    public const string SERVER_DOMAIN_API_LEARN = "https://api-learn.x3english.com/api/";

    public const string USER_PREMIUM_STATUS_URI = SERVER_DOMAIN + "v1/accounts/permissions";

    public const string CHECK_UPDATE_URI = SERVER_DOMAIN + "v1/configs/app-version";

    public const string LOGIN_URI = SERVER_DOMAIN + "v1/auth/login";
    public const string SIGNUP_URI = SERVER_DOMAIN + "v1/auth/register-anonymous";
    public const string FORGET_PASSWORD_URI = SERVER_DOMAIN + "v1/auth/forget-password";
    public const string CONFIRMING_FORGET_PASSWORD_SIX_DIGITS_CODE_URI = SERVER_DOMAIN + "v1/auth/verify-code";
    public const string SET_NEW_PASSWORD_URI = SERVER_DOMAIN + "v1/auth/new-password";
    public const string VERIFY_EMAIL_URI = SERVER_DOMAIN + "v1/auth/verify-email";
    public const string CREATE_KID_URI = SERVER_DOMAIN + "v1/accounts";
    public const string GET_KIDS_LIST_URI = SERVER_DOMAIN + "v1/accounts";
    public const string SELECT_KID_URI = SERVER_DOMAIN + "v1/accounts/select";

    public const string GET_MILESTONES_DETAIL_URI = SERVER_DOMAIN + "v1/milestones";
    public const string GET_UNIT_DETAIL_URI = SERVER_DOMAIN + "v1/units/detail?id=";
    public const string GET_LESSON_DETAIL_URI = SERVER_DOMAIN + "v1/lessons/detail?id=";
    public const string LESSON_COMPLETE = SERVER_DOMAIN + "v1/lessons/complete";
    public const string ACTIVITY_SCORING_URI = SERVER_DOMAIN + "v1/activity-records";

    public const string INTERACTIVE_ZONE_GET_LIST_LESSON = SERVER_DOMAIN + "v1/interactive-lessons";
    public const string INTERACTIVE_ZONE_GET_LIST_DUEL = SERVER_DOMAIN + "v1/interactive-lessons/challenge-accounts?lesson_id=";
    public const string INTERACTIVE_ZONE_GET_SPEAKING_PRACTICE = SERVER_DOMAIN + "v1/interactive-lessons/start-practice?lesson_id=";
    public const string INTERACTIVE_ZONE_POST_AI_RECORD_TO_BACKEND = SERVER_DOMAIN + "v1/interactive-lessons/record-sentence";
    public const string INTERACTIVE_ZONE_SPEAKING_FINAL = SERVER_DOMAIN + "v1/interactive-lessons/finish-practice";
    public const string INTERACTIVE_ZONE_GET_SPEAKING_DUEL = SERVER_DOMAIN + "v1/interactive-lessons/start-challenge?";
    public const string INTERACTIVE_ZONE_DUEL_FINAL = SERVER_DOMAIN + "v1/interactive-lessons/finish-challenge";

    public const string INTERACTIVE_ZONE_AI_SERVER = "https://ai-pronunciation.x3english.com/api/x3/recognize";
    public const string GET_KID_DETAIL_INFO = SERVER_DOMAIN + "v1/accounts/detail";

    public const string LIBRARY_GET_ALL_STORIES_URI = SERVER_DOMAIN + "v1/library-stories/";//?sort_by=createdAt&order_by=1";
    public const string LIBRARY_GET_STORY_URI = SERVER_DOMAIN + "v1/library-stories/detail?id=";
    public const string LIBRARY_FINISH_STORIES_URI = SERVER_DOMAIN + "v1/library-stories/finish/";

    public const string CUSTOMIZE_MASCOT_GET_ALL_URI = SERVER_DOMAIN + "v1/costumes";
    public const string CUSTOMIZE_MASCOT_POST_BUY_URI = SERVER_DOMAIN + "v1/costumes/buy";
    public const string CUSTOMIZE_MASCOT_POST_EQUIP_URI = SERVER_DOMAIN + "v1/costumes/equip";

    public const string IAP_PAYMENT_SUCCESS_URI = SERVER_DOMAIN_API_LEARN + "payment/engkid";
    public const string IAP_CREAT_OEDER_URI = SERVER_DOMAIN_API_LEARN + "payment/engkid/createOrder";
    public const string IAP_UPDATE_ORDER_URI = SERVER_DOMAIN_API_LEARN + "payment/engkid/updateOrder";

    public const string ACTIVE_LESSON_URI = SERVER_DOMAIN_API_LEARN + "activation/engkid";

    public const string WEEKLY_REPORTS_GET_URI = SERVER_DOMAIN + "v1/weekly-reports";
    public const string REPORT_PROGRESS_GET_URI = SERVER_DOMAIN + "v1/progress-reports";
    public const string RANKING_XP_REPORT_WEEK = SERVER_DOMAIN + "v1/xp-week-records/rank";

    public const string CHANGE_PASSWORD = SERVER_DOMAIN + "v1/users/password";
    public const string UPDATE_PARENT_INFORMATION = SERVER_DOMAIN + "v1/users";
    public const string UPDATE_CHILD_INFORMATION = SERVER_DOMAIN + "v1/accounts";
    public const string ACCOUNT_PRODUCT = SERVER_DOMAIN + "v1/account-products";
    public const string ACCOUNT_DETAIL = SERVER_DOMAIN + "v1/accounts/detail";

    public const string LEARN_TIME_RECORD = SERVER_DOMAIN + "v1/learn-time-records";

    #region public calls
    public enum PopUpLevel
    {
        NONE = -1,
        DEFAULT = 0,
        CANT_REACH_SERVER = 1
    }

    public void PostJSONRequest(string uri, string json_request, System.Action<bool> callback = null, System.Action<ReturnedMessage> message = null, string authorization_token = "", PopUpLevel popUpLevel = PopUpLevel.CANT_REACH_SERVER)
    {
        StartCoroutine(PostJSONRquestSequence(uri, json_request, return_value =>
        {
            if (callback != null)
                callback.Invoke(return_value);

            //if (popUpLevel == PopUpLevel.CANT_REACH_SERVER)
            //{
            //    if (return_value == false)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}
        },
        return_message =>
        {
            //Debug.Log(return_message);
            if (message != null)
                message.Invoke(return_message);

            //if (return_message == null)
            //    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
        },
        authorization_token
        ));
    }

    public void PutJSONRequest(string uri, string json_request, System.Action<bool> callback = null, System.Action<ReturnedMessage> message = null, string authorization_token = "", PopUpLevel popUpLevel = PopUpLevel.CANT_REACH_SERVER)
    {
        StartCoroutine(PutJSONRquestSequence(uri, json_request, return_value =>
        {
            if (callback != null)
                callback.Invoke(return_value);

            //if (popUpLevel == PopUpLevel.CANT_REACH_SERVER)
            //{
            //    if (return_value == false)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}
        },
        return_message =>
        {
            //Debug.Log(return_message);
            if (message != null)
                message.Invoke(return_message);

            //if (return_message == null)
            //    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
        },
        authorization_token
        ));
    }

    public void GetJSONRequest(string uri, System.Action<bool> callback = null, System.Action<ReturnedMessage> message = null, string authorization_token = "", PopUpLevel popUpLevel = PopUpLevel.CANT_REACH_SERVER)
    {
        StartCoroutine(GetJSONRquestSequence(uri, return_value =>
        {
            if (callback != null)
                callback.Invoke(return_value);

            //if (popUpLevel == PopUpLevel.CANT_REACH_SERVER)
            //{
            //    if (return_value == false)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}
        },
        return_message =>
        {
            //Debug.Log(return_message);
            if (message != null)
                message.Invoke(return_message);

            //if (return_message == null)
            //    PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
        },
        authorization_token
        ));
    }


    #endregion

    #region internals
    IEnumerator PostJSONRquestSequence(string uri, string json_request, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null, string authorization_token = "")
    {
        var request = new UnityWebRequest(uri, "POST");
        request.timeout = 30;
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json_request);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        if (authorization_token.Equals("") == false)
        {
            request.SetRequestHeader("Authorization", "Bearer " + authorization_token);
        }
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request not completed: " + request.error);

            //if (request.result == UnityWebRequest.Result.ConnectionError)
            //{
            //    PopupManagerBehavior.GetInstance().TriggerConnectionPopUp();
            //}
            //else if (request.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //    if (raw_message.statusCode != INCORRECT_LOG_IN &&
            //        raw_message.statusCode != EMAIL_UNVERIFIED_CODE &&
            //        raw_message.statusCode != EXISTED_EMAIL &&
            //        raw_message.statusCode != EXISTED_PHONE /*&&
            //        raw_message.statusCode != "400"*/)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}
            
            if (callback != null)
                callback.Invoke(false);
        }
        else
        {
            Debug.Log("Request performed successfully!");
            if (callback != null)
                callback.Invoke(true);
        }

        if (ret_message != null)
        {
            Debug.Log("___________Thientran__________Full JSON text: " + request.downloadHandler.text);
            ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //Debug.Log(raw_message.message);
            //Debug.Log(raw_message.data.verify_token);
            ret_message.Invoke(raw_message);
        }
    }

    IEnumerator PutJSONRquestSequence(string uri, string json_request, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null, string authorization_token = "")
    {
        var request = new UnityWebRequest(uri, "PUT");
        request.timeout = 30;
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json_request);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        if (authorization_token.Equals("") == false)
        {
            request.SetRequestHeader("Authorization", "Bearer " + authorization_token);
        }
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request not completed: " + request.error);

            //if (request.result == UnityWebRequest.Result.ConnectionError)
            //{
            //    PopupManagerBehavior.GetInstance().TriggerConnectionPopUp();
            //}
            //else if (request.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //    if (raw_message.statusCode != INCORRECT_LOG_IN &&
            //        raw_message.statusCode != EMAIL_UNVERIFIED_CODE &&
            //        raw_message.statusCode != EXISTED_EMAIL &&
            //        raw_message.statusCode != EXISTED_PHONE /*&&
            //        raw_message.statusCode != "400"*/)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}

            if (callback != null)
                callback.Invoke(false);
        }
        else
        {
            Debug.Log("Request performed successfully!");
            if (callback != null)
                callback.Invoke(true);
        }

        if (ret_message != null)
        {
            ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //Debug.Log(raw_message.message);
            ret_message.Invoke(raw_message);
        }
    }

    IEnumerator GetJSONRquestSequence(string uri, System.Action<bool> callback = null, System.Action<ReturnedMessage> ret_message = null, string authorization_token = "")
    {
        var request = new UnityWebRequest(uri, "GET");
        request.timeout = 30;
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        if (authorization_token.Equals("") == false)
        {
            request.SetRequestHeader("Authorization", "Bearer " + authorization_token);
        }
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request not completed: " + request.error);

            //if (request.result == UnityWebRequest.Result.ConnectionError)
            //{
            //    PopupManagerBehavior.GetInstance().TriggerConnectionPopUp();
            //}
            //else if (request.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //    if (raw_message.statusCode != INCORRECT_LOG_IN &&
            //        raw_message.statusCode != EMAIL_UNVERIFIED_CODE &&
            //        raw_message.statusCode != EXISTED_EMAIL &&
            //        raw_message.statusCode != EXISTED_PHONE /*&&
            //        raw_message.statusCode != "400"*/)
            //        PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            //}

            if (callback != null)
                callback.Invoke(false);
        }
        else
        {
            //Debug.Log("Request performed successfully!");
            if (callback != null)
                callback.Invoke(true);
        }

        if (ret_message != null)
        {
            ReturnedMessage raw_message;
            if (uri.Contains("interactive-lessons/start"))
            {
                string first = request.downloadHandler.text.Replace("[[", "[{\"turn_play\":[");
                string second = first.Replace("],[", "]},{\"turn_play\":[");
                string third = second.Replace("]]", "]}]");
                raw_message = JsonUtility.FromJson<ReturnedMessage>(third);

                //Debug.Log(third);
            }
            else
            {
                raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            }
            //ReturnedMessage raw_message = JsonUtility.FromJson<ReturnedMessage>(request.downloadHandler.text);
            //Debug.Log(raw_message.message);
            //Debug.Log(authorization_token);
            //Debug.Log(request.downloadHandler.text);
            //Debug.Log("Full JSON text: " + request.downloadHandler.text);
            ret_message.Invoke(raw_message);
        }
    }
    #endregion

    [Header("Debuging")]
    public string m_AccountEMail = "sonnguyen@x3english.com";
    public bool RESET_ACCOUNT = false;

    public bool BYPASS_ACTIVITY = false;
}

#region master reply messages
[System.Serializable]
public class ReturnedMessage
{
    [SerializeField] public ReturnedData data;

    [SerializeField] public string statusCode = "";
    [SerializeField] public string error = "";
    [SerializeField] public string message = "";

    //Server anh Linh Tran
    [SerializeField] public string type = string.Empty;
    [SerializeField] public int err = 0;
    [SerializeField] public string msg = string.Empty;
}

[System.Serializable]
public class ActivityRecordInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public int archived_star = 0;
}

[System.Serializable]
public class AccountActivityResultData
{
    [SerializeField] public ActivityRecordInfo activity;
    [SerializeField] public string archived_star = "";
}

[System.Serializable]
public class ConfigData
{
    [SerializeField] public string ios = "";
    [SerializeField] public string android = "";
}

[System.Serializable]
public class ReturnedData
{
    [SerializeField] public string token = "";
    [SerializeField] public string verify_token = "";
    [SerializeField] public string reset_token = "";
    [SerializeField] public string new_token = "";

    //update
    [SerializeField] public ConfigData config;

    //sign in
    [SerializeField] public ReturnedAccount[] accounts;

    //premium status
    [SerializeField] public string[] permissions;

    //milestone
    [SerializeField] public MilestoneInfo[] milestones;
    [SerializeField] public UnitResultInfo[] unit_results;
    [SerializeField] public AccountActivityResultData[] account_activity_results;
    [SerializeField] public AccountProgressInfo account_progress;

    //unit
    [SerializeField] public UnitDetailsInfo unit;

    //lesson
    [SerializeField] public LessonDetailInfo lesson;

    //activity

    //interactive zone
    [SerializeField] public UsableDataInfo[] list;
    [SerializeField] public ReadbookData[] read_story_list;

    [SerializeField] public DuelInfo[] account_list;
    [SerializeField] public Rounds[] rounds;
    [SerializeField] public string record_id;
    [SerializeField] public Competitor user_account;
    [SerializeField] public Competitor competitor_account;
    [SerializeField] public LessonLearned[] account_records;

    [SerializeField] public float gained_xp;
    [SerializeField] public float average_score;
    [SerializeField] public bool is_winner;

    //library zone
    //[SerializeField] public StoryInfosList[] list;

    //kid info
    //[SerializeField] public KidAccount account;
    [SerializeField] public ReturnedAccount account;

    //Customize Mascot
    //[SerializeField] public List<ImageItemInfo> list = new List<ImageItemInfo>();
    [SerializeField] public int total;

    //Login
    [SerializeField] public ParentUserData userData;

    //In App Purchase
    [SerializeField] public string order_code;
    [SerializeField] public string email;
    [SerializeField] public IAPPaymentSuccessCode code;
    [SerializeField] public string uid;

    //Activation
    [SerializeField] public ActivationCurrentRecordData current_record;

    //Report Progress
    [SerializeField] public WeekRecord[] week_star_records;
    [SerializeField] public WeekRecord[] week_xp_records;
    [SerializeField] public WeekRecord[] week_learn_time_records;

    //Report Skill
    [SerializeField] public SkillStatistic[] skill_statistics;
    [SerializeField] public ProgressResult[] progress_results;

    //Children Infomation
    [SerializeField] public AccountProduct[] account_products;

    public static ReturnedData CreateFromJSON(string json_data)
    {
        return JsonUtility.FromJson<ReturnedData>(json_data);
    }
}
#endregion

#region milestone and unit and lesson of Kidzone
#region milestone
[System.Serializable]
public class MilestoneInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public string title = "";
    [SerializeField] public string language = "";
    [SerializeField] public string progress_mark = "";
    [SerializeField] public CheckPointExamInfo checkpoint_exam;
    [SerializeField] public UnitListInfo[] units;

    public MilestoneInfo(string id, string tit, string lang, string pro_mark, CheckPointExamInfo exam, UnitListInfo[] unit_info)
    {
        _id = id;
        title = tit;
        language = lang;
        progress_mark = pro_mark;
        checkpoint_exam = exam;
        units = unit_info;
    }
}

[System.Serializable]
public class CheckPointExamInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public string title = "";

    public CheckPointExamInfo(string id, string tit)
    {
        _id = id;
        title = tit;
    }
}

[System.Serializable]
public class UnitListInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public string title = "";
    [SerializeField] public string language = "";
    [SerializeField] public AncestorInfo ancestor;
    [SerializeField] public string progress_mark = "";

    public UnitListInfo(string id, string tit, string lang, AncestorInfo parent_info, string prog)
    {
        _id = id;
        title = tit;
        language = lang;
        ancestor = parent_info;
        progress_mark = prog;
    }
}

[System.Serializable]
public class AncestorInfo
{
    [SerializeField] public string mile_stone_id = "";
    [SerializeField] public string unit_id = "";
    [SerializeField] public string lesson_id = "";

    public AncestorInfo(string parent_id)
    {
        mile_stone_id = parent_id;
    }
}

[System.Serializable]
public class UnitResultInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public CurrentUnit unit;
    [SerializeField] public string archived_star = "";
}

[System.Serializable]
public class CurrentUnit
{
    [SerializeField] public string _id = "";
    [SerializeField] public string title = "";
}
#endregion

#region Unit
[System.Serializable]
public class ThumbnailInfo
{
    [SerializeField] public string value = "";

    public ThumbnailInfo(string new_url)
    {
        value = new_url;
    }
}

[System.Serializable]
public class ResourceData
{
    [SerializeField] public string src = "";
}

[System.Serializable]
public class LessonListInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public ResourceData resource;
    [SerializeField] public string title = "";
    [SerializeField] public string type = "";
    [SerializeField] public string children_total_number = "";
    [SerializeField] public string language = "vi";
    [SerializeField] public string progress_mark = "";
    [SerializeField] public AncestorInfo ancestor;
    [SerializeField] public ThumbnailInfo thumbnail;
}

[System.Serializable]
public class UnitDetailsInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public string title = "";
    [SerializeField] public string archive_star = "";
    [SerializeField] public string children_total_number = "";
    [SerializeField] public string language = "vi";
    [SerializeField] public string progress_mark = "";
    [SerializeField] public AncestorInfo ancestor;
    [SerializeField] public ThumbnailInfo thumbnail;
    [SerializeField] public LessonListInfo[] lessons;
}

[System.Serializable]
public class LessonDetailInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public ResourceData resource;
    [SerializeField] public string title = "";
    [SerializeField] public string children_total_number = "";
    [SerializeField] public string language = "vi";
    [SerializeField] public string progress_mark = "";
    [SerializeField] public AncestorInfo ancestor;
    [SerializeField] public string __v = "";
    [SerializeField] public ThumbnailInfo thumbnail;
    [SerializeField] public AcivitiesListInfo[] activities;
}

[System.Serializable]
public class AcivitiesListInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public ResourceData resource;
    [SerializeField] public string title = "";
    [SerializeField] public string archive_star = "";
    [SerializeField] public string archive_xp = "";
    [SerializeField] public string language = "vi";
    [SerializeField] public string progress_mark = "";
    [SerializeField] public AncestorInfo ancestor;
    [SerializeField] public string __v = "";
    [SerializeField] public ThumbnailInfo thumbnail;
}
#endregion

[System.Serializable]
public class AccountProgressInfo
{
    [SerializeField] public string mile_stone_mark = "";
    [SerializeField] public string unit_mark = "";
    [SerializeField] public string lesson_mark = "";
    [SerializeField] public string activity_mark = "";
    [SerializeField] public string _id = "";
    [SerializeField] public string account_id = "";
    [SerializeField] public string created_at = "";
    [SerializeField] public string __v = "";
}
#endregion

[System.Serializable]
public class ReadbookData
{
    [SerializeField] public string story_id = "";
}

[System.Serializable]
public class DeleteAccount
{
    [SerializeField] public string email = "";
    [SerializeField] public string secret_key = "123456";

    public DeleteAccount(string mail, string key)
    {
        email = mail;
        secret_key = key;
    }
}

[System.Serializable]
public class UsableDataInfo
{
    //common
    [SerializeField] public string title = "";
    [SerializeField] public IndexThumbnail index_thumbnail;
    [SerializeField] public IndexThumbnail detail_thumbnail;
    [SerializeField] public Description description;
    [SerializeField] public string _id = "";
    [SerializeField] public bool isLearn = false;

    //library zone
    [SerializeField] public PersonelInfo author;
    [SerializeField] public PersonelInfo illustrator;
    [SerializeField] public PersonelInfo publisher;
    [SerializeField] public AudioInfo background_music;
    [SerializeField] public string read_times = "";
    [SerializeField] public string createdAt = "";
    [SerializeField] public string updatedAt = "";
    [SerializeField] public int __v;
    [SerializeField] public PageData[] pages;

    //customizemascot
    [SerializeField] public ResourceInfo resource;
    [SerializeField] public ImageInfo image;
    [SerializeField] public int gender;
    [SerializeField] public bool is_paid_product;
    [SerializeField] public int price;
    [SerializeField] public string name;
}

[System.Serializable]
public class PageData
{
    [SerializeField] public string _id = "";
    [SerializeField] public ImageData illustration_image;
    [SerializeField] public PageDataContent[] content;
}

[System.Serializable]
public class PageDataContent
{
    [SerializeField] public PageDataContentAbstractData text;
    [SerializeField] public AudioInfo audio;
    [SerializeField] public StoryTellerData story_teller;

    public PageDataContent()
    {
        text = new PageDataContentAbstractData();
        audio = new AudioInfo();
        story_teller = new StoryTellerData();
    }
}

[System.Serializable]
public class StoryTellerData
{
    [SerializeField] public PageDataContentAbstractData avatar;

    public StoryTellerData()
    {
        avatar = new PageDataContentAbstractData();
    }
}

[System.Serializable]
public class PageDataContentAbstractData
{
    [SerializeField] public string value = "";
    public PageDataContentAbstractData()
    {
        value = "";
    }
}

[System.Serializable]
public class ImageData
{
    [SerializeField] public string value = "";
    [SerializeField] public string type = "";
}

[System.Serializable]
public class PersonelInfo
{
    [SerializeField] public string name = "";
}

[System.Serializable]
public class AudioInfo
{
    [SerializeField] public string value = "";

    public AudioInfo()
    {
        value = "";
    }
}

[System.Serializable]
public class IndexThumbnail
{
    [SerializeField] public string value = "";
}

[System.Serializable]
public class Description
{
    [SerializeField] public string value = "";
}

[System.Serializable]
public class DuelInfo
{
    [SerializeField] public string _id = "";
    [SerializeField] public string name = "";
    [SerializeField] public string avatar = "";
}

[System.Serializable]
public class Rounds
{
    [SerializeField] public TurnPlay[] turn_play;
}

[System.Serializable]
public class TurnPlay
{
    [SerializeField] public Sentence sentence;
    [SerializeField] public CompetitorResult competitor_result;
    [SerializeField] public bool is_user_turn;
}

[System.Serializable]
public class CompetitorResult
{
    [SerializeField] public string audio_url = "";
    [SerializeField] public ActualCompetitorResult result;
}

[System.Serializable]
public class ActualCompetitorResult
{
    [SerializeField] public string score = "";
}

[System.Serializable]
public class Sentence
{
    [SerializeField] public string content;
    [SerializeField] public string _id;
    [SerializeField] public float duration;
    [SerializeField] public Sample sample;
}

[System.Serializable]
public class SpeakingAI
{
    [SerializeField] public string record_id;
    [SerializeField] public string sentence_id;
    [SerializeField] public string ai_result;
}

[System.Serializable]
public class FinalPractice
{
    [SerializeField] public string record_id = "";

    public FinalPractice(string id)
    {
        record_id = id;
    }
}
[System.Serializable]
public class Competitor
{
    [SerializeField] public string name;
    [SerializeField] public string avatar;
    [SerializeField] public string gender;
    [SerializeField] public Outfit outfit;
}

[System.Serializable]
public class Outfit
{
    [SerializeField] public string body;
}

[System.Serializable]
public class Sample
{
    [SerializeField] public float score;
    [SerializeField] public AudioSample audio_low_speed;
    [SerializeField] public AudioSample audio_high_speed;
    [SerializeField] public AudioSample audio;
}

[System.Serializable]
public class AudioSample
{
    [SerializeField] public string value;
}

[System.Serializable]
public class SentenceAI
{
    [SerializeField] public string audio_url = "";
    [SerializeField] public ResultAI result = new ResultAI();
    //[SerializeField] public string text = "";
    //[SerializeField] public float total_score = 0.0f;
    //[SerializeField] public WordAI[] result;
}

[System.Serializable]
public class ResultAI
{
    [SerializeField] public string textReference = "";
    [SerializeField] public float score = 0.0f;
    [SerializeField] public WordAI[] words;
}

[System.Serializable]
public class WordAI
{
    [SerializeField] public string wordRef = "";
    [SerializeField] public float wordScore = 0.0f;
}

[System.Serializable]
public class LessonLearned
{
    [SerializeField] public string lesson_id = "";
}

[System.Serializable]
public class KidAccount
{
    [SerializeField] public float star;
    [SerializeField] public float diamond;
    [SerializeField] public float xp;

}

#region parent account and kid profiles
[System.Serializable]
public class ReturnedAccount
{
    [SerializeField] public UserInfoData user;
    [SerializeField] public UserOutfitData outfit;
    [SerializeField] public string name = "";
    [SerializeField] public string avatar = "";
    [SerializeField] public int gender;
    [SerializeField] public int star;
    [SerializeField] public int diamond;
    [SerializeField] public int xp;
    [SerializeField] public string[] inventory; //TODO: might have to change this later
    [SerializeField] public string _id = "";
    [SerializeField] public int age_group_id;
    [SerializeField] public string language = "";
    [SerializeField] public string created_at = "";
    [SerializeField] public string updatedAt = "";
    [SerializeField] public int __v;
}

[System.Serializable]
public class UserInfoData
{
    [SerializeField] public string email = "";
    [SerializeField] public string _id = "";
}

[System.Serializable]
public class UserOutfitData
{
    //[SerializeField] public UserOutfitTopData top;
    //[SerializeField] public UserOutfitBodyData body;
    [SerializeField] public string body;
}

[System.Serializable]
public class UserOutfitTopData
{
    [SerializeField] public string hat = "";
}

[System.Serializable]
public class UserOutfitBodyData
{
    [SerializeField] public string shirt = "";
    [SerializeField] public string dress = "";
    [SerializeField] public string pants = "";
}

[System.Serializable]
public class ParentUserData
{
    [SerializeField] public string name;
    [SerializeField] public string email;
    [SerializeField] public string phone;
    [SerializeField] public string avatar;
    [SerializeField] public string language;
}

[System.Serializable]
public class IAPPaymentSuccessCode
{
    [SerializeField] public bool status;
    [SerializeField] public string[] error;
}

[System.Serializable]
public class ActivationCurrentRecordData
{
    [SerializeField] public string language;
    [SerializeField] public string expired_date;
    [SerializeField] public bool is_unlimited;
    [SerializeField] public string _id;
    [SerializeField] public string account_id;
    [SerializeField] public string product_key;
    [SerializeField] public string createdAt;
    [SerializeField] public string updatedAt;
    [SerializeField] public int __v;
}

[System.Serializable]
public class WeekRecord
{
    [SerializeField] public int number;
    [SerializeField] public string name;
    [SerializeField] public int amount;
}

[System.Serializable]
public class Milestone
{
    public string title;
    public string _id;
    public int progress_mark;
}

[System.Serializable]
public class ProgressResult
{
    public Milestone milestone;
    public double complete_percentage;
}

[System.Serializable]
public class SkillStatistic
{
    public string name;
    public int type_id;
    public int gained_stars;
    public int total_stars;
}

[System.Serializable]
public class AccountProduct
{
    public string language;
    public string expired_date;
    public bool is_unlimited;
    public string _id;
    public string account_id;
    public string product_key;
    public System.DateTime createdAt;
    public System.DateTime updatedAt;
    public int __v;
    public string duration_days;
}

#endregion