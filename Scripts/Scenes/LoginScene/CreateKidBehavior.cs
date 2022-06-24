using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateKidBehavior : MonoBehaviour
{
    [Header("Option screen")]
    public GameObject m_OptionScreen;

    [Header("Pages")]
    public List<GameObject> m_CreateKidPages = new List<GameObject>();

    [Header("Kid Info Input fields")]
    public InputField m_KidNameInput;
    public EngKidAPI.KidAgeGroups m_KidAgeGroup = EngKidAPI.KidAgeGroups.UNDER_FIVE;
    public EngKidAPI.KidGenders m_KidGender = EngKidAPI.KidGenders.MALE;
    public string m_AvatarURI = "";

    [Header("Avatar Choosing")]
    //TODO: use real images and uri links later
    public int m_AvatarPageIndex = 3;
    public List<Sprite> m_AvatarGirlsSpritesList = new List<Sprite>();
    public List<Sprite> m_AvatarBoysSpritesList = new List<Sprite>();
    public List<string> m_AvatarGirlURIList = new List<string>();
    public List<string> m_AvatarBoyURIList = new List<string>();
    public Transform m_AvatarsHolder;
    public GameObject m_AvatarPrefab;
    public ScrollViewContentScaler contentScaler;

    [Header("On finialize kid info settings")]
    public LogInSceneScreensManager m_ScreenManager;

    [Header("UI")]
    public Button m_Next;
    public List<Button> m_ListGroupAge = new List<Button>();
    public ScrollViewContentCentering contentCentering;

    [Header("UI Gender")]
    public Image boyLight;
    public Image girlLight;
    public GameObject boyActive;
    public GameObject girlActive;
    public GameObject boyNormal;
    public GameObject girlNormal;
    public Sprite standActive;
    public Sprite standNormal;
    public Image boyStand;
    public Image girlStand;

    [Header("UI Gender")]
    public List<Sprite> m_AvatarGirlList = new List<Sprite>();
    public List<Sprite> m_AvatarBoyList = new List<Sprite>();

    //internal
    int currentPageIndex = 0;
    KidInfo kidInfo;
    int currentAgeGroup;
    int currentGender;
    List<Texture2D> texList = new List<Texture2D>();

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        foreach (Texture2D tex in texList)
        {
            Destroy(tex);
        }
    }

    public IEnumerator AvatarCustomizationProcessing()
    {
        foreach (Transform child in m_AvatarsHolder.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject avatar = Instantiate(m_AvatarPrefab, m_AvatarsHolder);
            //TODO: pull avatar image from server
            if (m_KidGender == EngKidAPI.KidGenders.FEMALE)
            {
                int pos = i;
                yield return StartCoroutine(SetImage(avatar.GetComponent<Image>(), m_AvatarGirlURIList[pos]));
                avatar.GetComponent<Button>().onClick.AddListener(() =>
                {
                    avatar_index = pos;
                    m_AvatarURI = m_AvatarGirlURIList[pos];
                    Debug.Log(m_AvatarGirlURIList[pos]);

                    contentCentering.m_DefaultTarget = avatar.GetComponent<RectTransform>();
                    contentCentering.OnCenteringOnElement(avatar.GetComponent<RectTransform>());
                });
                //avatar.GetComponent<Image>().sprite = m_AvatarGirlList[i];
            }
            else
            {
                int pos = i;
                yield return StartCoroutine(SetImage(avatar.GetComponent<Image>(), m_AvatarBoyURIList[pos]));
                avatar.GetComponent<Button>().onClick.AddListener(() =>
                {
                    avatar_index = pos;
                    m_AvatarURI = m_AvatarBoyURIList[pos];
                    Debug.Log(m_AvatarBoyURIList[pos]);

                    contentCentering.m_DefaultTarget = avatar.GetComponent<RectTransform>();
                    contentCentering.OnCenteringOnElement(avatar.GetComponent<RectTransform>());
                });
                //avatar.GetComponent<Image>().sprite = m_AvatarBoyList[i];
            }

            //yield return null;
            //yield return null;
            //yield return null;
            //contentCentering.m_DefaultTarget = avatar.GetComponent<RectTransform>();
            //contentCentering.OnCenteringOnElement(avatar.GetComponent<RectTransform>());

            //yield return null;
            //yield return null;
            //yield return null;
            //Debug.Log(m_AvatarURI);
            ColorBlock cb = avatar.GetComponent<Button>().colors;
            cb.normalColor = Color.gray;
            avatar.GetComponent<Button>().colors = cb;
            LeanTween.scale(avatar, Vector3.zero, 0.1f);
        }

        yield return null;
        yield return null;
        yield return null;
        m_AvatarURI = m_AvatarBoyURIList[0];
        contentCentering.m_DefaultTarget = m_AvatarsHolder.GetChild(0).GetComponent<RectTransform>();
        contentCentering.OnCenteringOnElement(m_AvatarsHolder.GetChild(0).GetComponent<RectTransform>());
        yield return null;
        yield return StartCoroutine(contentScaler.DelayedGetContentList());
        yield return null;
        yield return StartCoroutine(DelayedCenteringAvatar());
    }

    int avatar_index = 0;
    IEnumerator DelayedCenteringAvatar(bool use_default = true)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        RectTransform defaultTarget = m_AvatarsHolder.transform.GetChild((use_default == true)? 0: avatar_index).GetComponent<RectTransform>();
        contentCentering.m_DefaultTarget = defaultTarget;
        contentCentering.OnCenteringOnElement(defaultTarget);
    }    

    public IEnumerator SetImage(Image image, string url = "", Sprite sprite = null)
    {
        if (sprite != null)
        {
            //image.sprite = sprite;
        }
        else
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                PopupManagerBehavior.GetInstance().TriggerServerConnectionPopUp();
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f); ;
                //DestroyImmediate(((DownloadHandlerTexture)www.downloadHandler).texture);
                texList.Add(tex);
            }

            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }    
    }

    public void OnPageTurn()
    {
        if (currentPageIndex == m_AvatarPageIndex)
        {
            OnConfirmKidInfo2();
            return;
        }
        if (currentPageIndex < m_CreateKidPages.Count - 1)
            currentPageIndex++;

        foreach (GameObject page in m_CreateKidPages)
        {
            page.SetActive(false);
        }
        m_CreateKidPages[currentPageIndex].SetActive(true);

        if (currentPageIndex == m_AvatarPageIndex)
        {
            foreach (Transform child in m_AvatarsHolder.transform)
            {
                Destroy(child.gameObject);
            }
            GlobalCurtain.GetInstance().TriggerCurtain();
            StartCoroutine(AvatarCustomSequence());
        }    
    }

    IEnumerator AvatarCustomSequence()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        yield return StartCoroutine(AvatarCustomizationProcessing());
        GlobalCurtain.GetInstance().OpenCurtain();
        yield return new WaitForSecondsRealtime(0.5f);
        GlobalCurtain.GetInstance().HideCurtain();
    }    

    public void OnPageTurnBack()
    {
        if (currentPageIndex == 0)
        {
            m_OptionScreen.SetActive(true);
            this.gameObject.SetActive(false);

            m_ScreenManager.OnNewUserClicked();

            return;
        }

        if (currentPageIndex > 0)
            currentPageIndex--;

        foreach (GameObject page in m_CreateKidPages)
        {
            page.SetActive(false);
        }
        m_CreateKidPages[currentPageIndex].SetActive(true);
    }

    public void OnAvatarMoveInFocus(int index)
    {
        if (m_KidGender == EngKidAPI.KidGenders.FEMALE)
        {
            m_AvatarURI = m_AvatarGirlURIList[index];
            Debug.Log(m_AvatarGirlURIList[index]);
        }   
        else
        {
            m_AvatarURI = m_AvatarBoyURIList[index];
            Debug.Log(m_AvatarBoyURIList[index]);
        }    
    }    

    public void OnConfirmKidInfo2()
    {
        kidInfo = new KidInfo();
        kidInfo.name = m_KidNameInput.text;
        kidInfo.age_group_id = (int)m_KidAgeGroup;
        kidInfo.gender = (int)m_KidGender;
        kidInfo.avatar = m_AvatarURI;
        kidInfo.language = LocalizationManagerBehavior.GetInstance().GetLanguageCode();

        if (UserDataManagerBehavior.GetInstance().GetUserLogInState() == EngKidAPI.UserLogInStates.LOGGED_IN)
        {
            m_ScreenManager.OnCreateKidAfterLogIn();
        }
        else
        {
            m_ScreenManager.OnCreateKidBeforeLogIn();
        }
    }

    public void OnConfirmKidInfo()
    {
        kidInfo = new KidInfo();
        kidInfo.name = m_KidNameInput.text;
        kidInfo.age_group_id = (int)m_KidAgeGroup;
        kidInfo.gender = (int)m_KidGender;
        kidInfo.avatar = m_AvatarURI;
        kidInfo.language = LocalizationManagerBehavior.GetInstance().GetLanguageCode();

        Debug.Log("Creating kid: " + kidInfo.avatar + " name: " + kidInfo.name);

        if (UserDataManagerBehavior.GetInstance().GetUserLogInState() == EngKidAPI.UserLogInStates.LOGGED_IN)
        {
            UserDataManagerBehavior.GetInstance().OnCreateKid(
                kidInfo,
                call_back_flag =>
                {
                    if (call_back_flag == true)
                    {
                        m_ScreenManager.OnCreateKidAfterLogIn();
                    }
                    else
                        Debug.Log("Error: cannot create new kid.");
                }
                );
        }
        else
        {
            UserDataManagerBehavior.GetInstance().OnCreateKid(
                kidInfo,
                call_back_flag =>
                {
                    if (call_back_flag == true)
                    {
                        m_ScreenManager.OnCreateKidBeforeLogIn();
                    }
                    else
                        Debug.Log("Error: cannot create new kid.");
                }
                );
            UserDataManagerBehavior.GetInstance().currentSelectedKidAge = kidInfo.age_group_id.ToString();
        }
    }

    public void OnCreateNewKid()
    {
        if (UserDataManagerBehavior.GetInstance().GetUserLogInState() == EngKidAPI.UserLogInStates.LOGGED_IN)
        {
            //adding kid
            //Debug.Log(m_CreateKidBehavior.GetKidInfo().language);
            UserDataManagerBehavior.GetInstance().OnCreateKid(
                GetKidInfo(),
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

    public KidInfo GetKidInfo()
    {
        kidInfo = new KidInfo();
        kidInfo.name = m_KidNameInput.text;
        kidInfo.age_group_id = (int)m_KidAgeGroup;
        kidInfo.gender = (int)m_KidGender;
        kidInfo.avatar = m_AvatarURI;
        kidInfo.language = LocalizationManagerBehavior.GetInstance().GetLanguageCode();
        return kidInfo;
    }

    public void SetAgeGroup(int i)
    {
        currentAgeGroup = i;
        m_KidAgeGroup = (EngKidAPI.KidAgeGroups)i;
        DisplaySeletedAge();
    }

    public void Setgender(int i)
    {
        currentGender = i;
        m_KidGender = (EngKidAPI.KidGenders)i;
        DisplaySelectedGender();
    }

    private void Start()
    {
        currentPageIndex = 0;
        foreach (GameObject page in m_CreateKidPages)
        {
            page.SetActive(false);
        }
        m_CreateKidPages[0].SetActive(true);

        foreach (Transform child in m_AvatarsHolder)
        {
            Destroy(child.gameObject);
        }

        SetAgeGroup(2);
        Setgender(1);
    }

    Coroutine recenterCor = null;
    float idleDur = 2.0f;
    private void Update()
    {
        if (m_KidNameInput.text.Length <= 0)
        {
            m_Next.interactable = false;
        }
        else
        {
            m_Next.interactable = true;
        }

        if (idleDur > 0.0f && Input.GetMouseButton(0) == false)
        {
            idleDur -= Time.deltaTime;
            if (idleDur <= 0.0f)
            {
                //idleDur = 2.0f;
                recenterCor = StartCoroutine(DelayedCenteringAvatar(false));
            }
        }
        
        if (Input.GetMouseButton(0) == true)
        {
            idleDur = 2.0f;
            if (recenterCor != null)
                StopCoroutine(recenterCor);
        }
    }

    public void DisplaySeletedAge()
    {
        for (int i = 0; i < m_ListGroupAge.Count; i++)
        {
            m_ListGroupAge[i].image.color = new Color32(255, 255, 255, 255);
        }
        m_ListGroupAge[currentAgeGroup - 1].image.color = new Color32(206, 255, 200, 255);
    }

    public void DisplaySelectedGender()
    {
        if (currentGender == 1)
        {
            boyLight.color = new Color32(255, 255, 255, 255);
            girlLight.color = new Color32(255, 255, 255, 0);
            boyActive.SetActive(true);
            girlActive.SetActive(false);
            boyNormal.SetActive(false);
            girlNormal.SetActive(true);
            boyStand.sprite = standActive;
            girlStand.sprite = standNormal;
        }
        else
        {
            boyLight.color = new Color32(255, 255, 255, 0);
            girlLight.color = new Color32(255, 255, 255, 255);
            boyActive.SetActive(false);
            girlActive.SetActive(true);
            boyNormal.SetActive(true);
            girlNormal.SetActive(false);
            boyStand.sprite = standNormal;
            girlStand.sprite = standActive;
        }

    }

}

[System.Serializable]
public class KidInfo
{
    public string name = "";
    public int age_group_id = 1;
    public int gender = 1;
    public string avatar = "";
    public string language = "vi";
}
