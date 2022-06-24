using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InteractiveZoneManager : MonoBehaviour
{
    [Header("Kid Info")]
    public GameObject kidInfo;
    public Text kidEXP;

    [Header("Screen page")]
    public GameObject MainMenu;
    public GameObject SpeakingListPage;
    public GameObject PracticePage;
    public GameObject DuelListPage;
    public GameObject duelListBackBtn;
    public GameObject DuelPage;
    public Image m_CurrentKid;
    public Text m_CurrentKidName;
    public GameObject curtain;

    public AudioClip bgMusic;

    public UsableDataInfo currentLesson;
    public DuelInfo currentDuel;

    private void Start()
    {
        //AuthorizationManager.GetInstance().TriggerMicroPopUp();

        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SetupCurrentKid();
        BGMManagerBehavior.GetInstance().PlayBGM(bgMusic);

        duelListBackBtn.SetActive(false);
    }

    public void RefeshKidInfo()
    {
        UserDataManagerBehavior.GetInstance().OnRefreshKidsList(
            callback =>
            {
                if (callback == true)
                {
                    kidEXP.text = UserDataManagerBehavior.GetInstance().currentSelectedKidXP + " XP";
                }
            },
            null
            );
    }

    public void OnBackToKidZone()
    {
        BGMManagerBehavior.GetInstance().PlayBGM(BGMManagerBehavior.GetInstance().m_KidZoneBGM);
        SceneManagerBehavior.GetInstance().OpenMainWorldMapScene(this.gameObject);
    }

    public void ShowDuelListPage()
    {
        StartCoroutine(ShowDuelListSequence());
    }

    IEnumerator ShowDuelListSequence()
    {
        kidInfo.SetActive(false);
        kidInfo.transform.parent.gameObject.SetActive(false);
        curtain.SetActive(true);
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_close", loop: true);
        yield return new WaitForSeconds(0.5f);

        SpeakingListPage.SetActive(false);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(true);
        DuelPage.SetActive(false);
        DuelListPage.GetComponent<ListDuel>().GetListDuelFromServer();
        duelListBackBtn.SetActive(true);
    }

    public void ShowPracticePage()
    {
        kidInfo.SetActive(false);
        kidInfo.transform.parent.gameObject.SetActive(false);
        curtain.SetActive(false);
        SpeakingListPage.SetActive(false);
        PracticePage.SetActive(true);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(false);
        PracticePage.GetComponent<PracticeManager>().GetDataFromServer();

        duelListBackBtn.SetActive(false);
    }

    public void ShowDuelPage()
    {
        kidInfo.SetActive(false);
        kidInfo.transform.parent.gameObject.SetActive(false);
        curtain.SetActive(false);
        SpeakingListPage.SetActive(false);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(true);
        DuelPage.GetComponent<DuelMode>().GetDataFromServer();

        duelListBackBtn.SetActive(false);
    }

    IEnumerator GetAvatarImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            m_CurrentKid.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public void SetupCurrentKid()
    {
        StartCoroutine(GetAvatarImage(UserDataManagerBehavior.GetInstance().currentSelectedKidAvatarURL));
        m_CurrentKidName.text = UserDataManagerBehavior.GetInstance().currentSelectedKidName;
        GetKidInfoFromServer();
    }

    public void BackFromDuelList()
    {
        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(true);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(false);
        SpeakingListPage.GetComponent<ListLesson>().PlayAnimIntro();
        BGMManagerBehavior.GetInstance().ResumeBGM();

        duelListBackBtn.SetActive(false);
    }

    public void BackFromPracticeReview()
    {
        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(true);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(false);
        SpeakingListPage.GetComponent<ListLesson>().PlayAnimIntro();
        BGMManagerBehavior.GetInstance().ResumeBGM();

        duelListBackBtn.SetActive(false);
    }

    public void BackFromPractice()
    {
        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(true);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(false);
        BGMManagerBehavior.GetInstance().ResumeBGM();
        SpeakingListPage.GetComponent<ListLesson>().PlayAnimIntro();

        duelListBackBtn.SetActive(false);
    }

    public void BackFromDuelReview()
    {
        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(false);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(true);
        DuelPage.SetActive(false);
        //MainMenu.SetActive(true);
        BGMManagerBehavior.GetInstance().ResumeBGM();

        duelListBackBtn.SetActive(false);
    }

    public void BackFromDuelMode()
    {
        kidInfo.SetActive(false);
        kidInfo.transform.parent.gameObject.SetActive(false);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(false);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(true);
        DuelPage.SetActive(false);
        //MainMenu.SetActive(true);
        BGMManagerBehavior.GetInstance().ResumeBGM();
        duelListBackBtn.SetActive(true);
    }

    public void BackFromDuelPopup()
    {
        kidInfo.SetActive(true);
        kidInfo.transform.parent.gameObject.SetActive(true);
        RefeshKidInfo();
        curtain.SetActive(true);
        SpeakingListPage.SetActive(true);
        PracticePage.SetActive(false);
        DuelListPage.SetActive(false);
        DuelPage.SetActive(false);
        //MainMenu.SetActive(true);
        BGMManagerBehavior.GetInstance().ResumeBGM();

        duelListBackBtn.SetActive(false);
    }

    public void GetKidInfoFromServer()
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
                Debug.Log("__________Thien_diamond: " + server_reply.data.account.diamond);
                //Debug.Log("thanh cong == " + server_reply.data.account.diamond);
                UserDataManagerBehavior.GetInstance().UpdateCurrentKid(server_reply.data.account);
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }
}
