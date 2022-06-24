using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ReplayPageScript : MonoBehaviour
{
    [Header("Manager")]
    public InteractiveZoneManager bigManager;

    [Header("List Sentence")]
    public GameObject m_ListBox;
    public GameObject m_Cell;

    [Header("UI Control")]
    public GameObject playButton;
    public GameObject pauseButton;

    [Header("UI Control")]
    public SkeletonGraphic playerAnim;
    public ScrollRect scrollRect;

    AudioSource audioSource;
    AudioClip myClip;

    List<SentenceAI> dataSentence = new List<SentenceAI>();
    // Start is called before the first frame update

    int currentSentence = 0;
    bool endList = true;

    IEnumerator playAudioProcess;

    void Start()
    {
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().PauseBGM();

        audioSource = GetComponent<AudioSource>();
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        //Test();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(List<SentenceAI> list)
    {
        RenewPage();
        dataSentence = list;
        for (int i = 0; i < dataSentence.Count; i++)
        {
            GameObject cell = Instantiate(m_Cell, m_ListBox.transform);
            cell.GetComponent<ItemReplay>().SetSentence(dataSentence[i]);
        }
    }

    private void Test()
    {
        SentenceAI item = new SentenceAI();
        item.result.textReference = "Wow! There is a mouse!";
        item.audio_url = "https://ai-pronunciation.x3english.com/data/gop/20210825/20210825204021-50c3dbec.wav";
        dataSentence.Add(item);

        item = new SentenceAI();
        item.result.textReference = "He has white hair.";
        item.audio_url = "https://ai-pronunciation.x3english.com/data/gop/20210825/20210825203936-1d91d725.wav";
        dataSentence.Add(item);

        item = new SentenceAI();
        item.result.textReference = "He has white hair.";
        item.audio_url = "https://ai-pronunciation.x3english.com/data/gop/20210825/20210825204042-e89d1c16.wav";
        dataSentence.Add(item);

        item = new SentenceAI();
        item.result.textReference = "He runs and jumps on the floor.";
        item.audio_url = "https://ai-pronunciation.x3english.com/data/gop/20210825/20210825203951-e23fca2e.wav";
        dataSentence.Add(item);

        item = new SentenceAI();
        item.result.textReference = "And my mum jumps on a chair.";
        item.audio_url = "https://ai-pronunciation.x3english.com/data/gop/20210825/20210825204059-bac546ca.wav";
        dataSentence.Add(item);

        for (int i = 0; i < dataSentence.Count; i++)
        {
            GameObject cell = Instantiate(m_Cell, m_ListBox.transform);
            cell.GetComponent<ItemReplay>().SetSentence(dataSentence[i]);
        }
    }

    

    IEnumerator GetAudioClip(string link)
    {
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // set current clip
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;

                    HighlightCurrent();
                    // play the clip
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    PlayNext();
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // set current clip
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = myClip;

                    HighlightCurrent();
                    // play the clip
                    audioSource.Play();
                    yield return new WaitForSeconds(myClip.length);
                    PlayNext();
                }
            }
        }

    }

    private void PlayNext()
    {
        if (currentSentence + 1 < dataSentence.Count)
        {
            endList = false;
            currentSentence = currentSentence + 1;
            playAudioProcess = GetAudioClip(dataSentence[currentSentence].audio_url);
            StartCoroutine(playAudioProcess);
        }
        else
        {
            EndList();
        }
    }

    private void PlayCurrent()
    {
        if (currentSentence < dataSentence.Count)
        {
            endList = false;
            playAudioProcess = GetAudioClip(dataSentence[currentSentence].audio_url);
            StartCoroutine(playAudioProcess);
        }
        else
        {
            EndList();
        }
    }

    private void EndList()
    {
        endList = true;
        playButton.SetActive(true);
        pauseButton.SetActive(false);

        currentSentence = -1;
        HighlightCurrent();
        StopCoroutine(playAudioProcess);

        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
    }

    private void HighlightCurrent()
    {
        for (int i = 0; i < m_ListBox.transform.childCount; i++)
        {
            if (i == currentSentence)
            {
                m_ListBox.transform.GetChild(i).GetComponent<ItemReplay>().SetHighlight(true);

                scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(m_ListBox.transform.GetChild(i).GetComponent<RectTransform>());
            }
            else
            {
                m_ListBox.transform.GetChild(i).GetComponent<ItemReplay>().SetHighlight(false);
            }
        }
    }

    public Vector2 GetSnapToPositionToBringChildIntoView(RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }

    public void OnPlayClick()
    {
        playButton.SetActive(false);
        pauseButton.SetActive(true);

        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("talk", 1);
        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_talk");

        if (endList)
        {
            currentSentence = -1;
            PlayNext();
        }
        else
        {
            PlayCurrent();
        }
    }

    public void OnPauseClick()
    {
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        audioSource.Pause();
        StopCoroutine(playAudioProcess);

        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic", 1);
        playerAnim.gameObject.GetComponent<EngKidUIAvatarController>().PlayAnimationLooped("idle_with_mic");
    }

    public void OnBackClick()
    {
        if (playAudioProcess != null)
        {
            StopCoroutine(playAudioProcess);
        }
        gameObject.SetActive(false);
        audioSource.Stop();
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().ResumeBGM();
        bigManager.BackFromPracticeReview();
    }

    public void OnBackClickFromDuel()
    {
        if (playAudioProcess != null)
        {
            StopCoroutine(playAudioProcess);
        }
        gameObject.SetActive(false);
        audioSource.Stop();
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().ResumeBGM();
        bigManager.BackFromDuelReview();
    }

    public void OnHomeClick()
    {
        if (playAudioProcess != null)
        {
            StopCoroutine(playAudioProcess);
        }
        audioSource.Stop();
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().ResumeBGM();
        bigManager.OnBackToKidZone();
    }

    public void OnChallengeClick()
    {
        if (playAudioProcess != null)
        {
            StopCoroutine(playAudioProcess);
        }
        audioSource.Stop();
        if (BGMManagerBehavior.GetInstance())
            BGMManagerBehavior.GetInstance().ResumeBGM();
        gameObject.SetActive(false);
        bigManager.ShowDuelListPage();
    }

    public void RenewPage()
    {
        currentSentence = 0;
        endList = false;
        foreach (Transform child in m_ListBox.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
