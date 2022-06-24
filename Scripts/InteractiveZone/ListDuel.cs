using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Spine.Unity;

public class ListDuel : MonoBehaviour
{
    [Header("Manager")]
    public InteractiveZoneManager interactiveZoneManager;

    [Header("Inner")]
    public GameObject m_DuelCell;
    public GameObject m_DuelListContainer;
    public GameObject m_MicPermissionPopup;

    [Header("Audio")]
    public SoundHolder soundHolder;
    public AudioClip micIntroAudio;

    private ReturnedData dataServer;
    private DuelInfo currentDuel;

    [Header("UI")]
    public ScrollViewContentCentering contentCentering;
    public GameObject curtain;

    //internals
    int readyPingCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        readyPingCount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetListDuelFromServer()
    {
        ClearOldData();
        readyPingCount = 0;

        //Debug.Log("lay danh sach doi thu");
        DataBaseInterface.GetInstance().GetJSONRequest(
            DataBaseInterface.INTERACTIVE_ZONE_GET_LIST_DUEL + interactiveZoneManager.currentLesson._id,
            callback_flag =>
            {
                if (callback_flag == false)
                    Debug.Log("Error: get duel detail.");
            },
            server_reply =>
            {
                //Debug.Log("Success: get duel detail." + server_reply);
                StartCoroutine(DisplayDuel(server_reply.data));
            },
            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
            );
    }

    private void ClearOldData()
    {
        foreach (Transform child in m_DuelListContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public IEnumerator DisplayDuel(ReturnedData data)
    {
        Debug.Log("There are: " + data.account_list.Length + " opponents available.");
        dataServer = data;
        for (int i = 0; i < data.account_list.Length; i++)
        {
            yield return new WaitForEndOfFrame();

            GameObject cell = Instantiate(m_DuelCell, m_DuelListContainer.transform);
            cell.GetComponent<ItemDuel>().SetDuel(data.account_list[i], this);
            cell.GetComponent<ItemDuel>().SetupDuel();
            cell.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoDuelMode(cell.GetComponent<ItemDuel>().GetDuel());
                UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);
            });
            ColorBlock cb = cell.GetComponent<Button>().colors;
            cb.normalColor = Color.gray;
            cell.GetComponent<Button>().colors = cb;
        }
    }


    public void ReadyPing()
    {
        readyPingCount++;
        if (readyPingCount == m_DuelListContainer.transform.childCount)
        {
            StartCoroutine(DelayedCenteringListItem());
        }
    }

    IEnumerator DelayedCenteringListItem()
    {
        yield return null;
        if (m_DuelListContainer.transform.childCount > 0)
        {
            Button btn = m_DuelListContainer.transform.GetChild(0).GetComponent<Button>();
            ColorBlock cb = btn.GetComponent<Button>().colors;
            cb.normalColor = Color.white;
            btn.GetComponent<Button>().colors = cb;
            contentCentering.m_DefaultTarget = m_DuelListContainer.transform.GetChild(0).GetComponent<RectTransform>();
            contentCentering.OnCenteringOnElement(btn.GetComponent<RectTransform>());
        }

        //curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "out", loop: false);
        yield return new WaitForSeconds(1f);
        curtain.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_open", loop: true);
    }

    public void setCurrentDuel(DuelInfo user)
    {
        this.currentDuel = user;
    }

    public void GoDuelMode()
    {
        StartCoroutine(GoToDuelModeSequence(null));
    }

    public void GoDuelMode(DuelInfo duelInfo = null)
    {
        //#if PLATFORM_ANDROID
        //        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        //        {
        //            m_MicPermissionPopup.SetActive(true);
        //            soundHolder.PlaySound(micIntroAudio);
        //            return;
        //        }
        //#endif

        StartCoroutine(GoToDuelModeSequence(duelInfo));
    }

    IEnumerator GoToDuelModeSequence(DuelInfo duelInfo = null)
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            interactiveZoneManager.currentDuel = duelInfo == null? currentDuel:duelInfo;
            interactiveZoneManager.ShowDuelPage();
        }
        else
        {
            m_MicPermissionPopup.SetActive(true);
            soundHolder.PlaySound(micIntroAudio);
            yield return new WaitForSeconds(micIntroAudio.length);

            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                interactiveZoneManager.currentDuel = duelInfo == null ? currentDuel : duelInfo;
                interactiveZoneManager.ShowDuelPage();
            }
            else
            {
                StartCoroutine(GoToDuelModeSequence());
            }
        }
    }

    public void RequestMicPermission()
    {
        Permission.RequestUserPermission(Permission.Microphone);
    }

}
