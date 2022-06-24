using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class AuthorizationManager : MonoBehaviour
{
    #region Singleton
    private static AuthorizationManager _Instance;
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
    public static AuthorizationManager GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: AuthorizationManager instance is null.");

        return _Instance;
    }
    #endregion

    public string iOSAppSettingsUrl = "app-settings:";
    public string androidAppSettingIntent = "android.settings.APPLICATION_DETAILS_SETTINGS";
    public GameObject microPopUp;

    public bool isMicPermitted = false;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        isMicPermitted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#endif

#if UNITY_IOS
        isMicPermitted = Application.HasUserAuthorization(UserAuthorization.Microphone);
#endif

        microPopUp.SetActive(false);
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (microPopUp.activeSelf == true)
        {
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone) == true)
            {
                isMicPermitted = true;
                microPopUp.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
#endif

#if UNITY_IOS
        if (microPopUp.activeSelf == true)
        {
            if (Application.HasUserAuthorization(UserAuthorization.Microphone) == true)
            {
                isMicPermitted = true;
                microPopUp.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
#endif
    }

    public void TriggerMicroPopUp()
    {
#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) == false)
        {
            this.GetComponent<AudioSource>().Play();
            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
#endif

#if UNITY_IOS
        if (Application.HasUserAuthorization(UserAuthorization.Microphone) == false)
        {
            this.GetComponent<AudioSource>().Play();
            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
#endif
    }

    public void RequestMicroAuthorization()
    {
        microPopUp.SetActive(false);
        //Time.timeScale = 1.0f;

        if (PlayerPrefs.HasKey("MIC_AUTHORIZATION") == false)
            RequestMicAuthoSequence();
        else
            RequestMicAuthoViaSettings();
    }

    public void RequestMicAuthoViaSettings()
    {
#if UNITY_IOS
        Debug.Log("Sending iOS open settings request.");
        Application.OpenURL(iOSAppSettingsUrl);
#endif
#if UNITY_ANDROID
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string packageName = currentActivityObject.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intentObject = new AndroidJavaObject("android.content.Intent", androidAppSettingIntent, uriObject))
            {
                //Debug.Log("Sending android intend.");
                intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
#endif

#if UNITY_ANDROID
        //Permission.RequestUserPermission(Permission.Microphone);
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) == false)
        {
            //TODO: open settings
            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            isMicPermitted = true;
            microPopUp.SetActive(false);
            Time.timeScale = 1.0f;
        }
#endif

#if UNITY_IOS
        if (Application.HasUserAuthorization(UserAuthorization.Microphone) == false)
        {
            //TODO: open settings
            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            isMicPermitted = true;
            microPopUp.SetActive(false);
            Time.timeScale = 1.0f;
        }
#endif
    }

    IEnumerator RequestMicAuthoSequenceIOS()
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone) == false)
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            isMicPermitted = true;
            microPopUp.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void RequestMicAuthoSequence()
    {
        PlayerPrefs.SetInt("MIC_AUTHORIZATION", 1);
        PlayerPrefs.Save();

#if UNITY_IOS
        StartCoroutine(RequestMicAuthoSequenceIOS());
#endif

#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) == false)
        {
            Permission.RequestUserPermission(Permission.Microphone);

            microPopUp.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            isMicPermitted = true;
            microPopUp.SetActive(false);
            Time.timeScale = 1.0f;
        }
#endif
    }
}
