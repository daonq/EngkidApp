using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Stories;
public class StoryPageBehavior : MonoBehaviour
{
    [Header("Highlight images")]
    public List<GameObject> m_HightlightsList = new List<GameObject>();

    [Header("Audio")]
    public List<AudioClip> m_HLAudioClipsList = new List<AudioClip>();
    public List<AudioClip> m_MainAudioClipsList = new List<AudioClip>();
    public AudioSource m_AudioSource;
    private BaseStoryBehavior m_BaseStoryBehavior;

    [Header("Thoại")]
    public List<Text> m_LineTextsListA = new List<Text>();
    public List<Text> m_LineTextsListB = new List<Text>();
    public List<TextAsset> m_Subtitle = new List<TextAsset>();
    SRTParser parser;

    //internals
    Coroutine highlightCor = null;
    Coroutine audioCor = null;
    Coroutine textColorCorA = null;
    Coroutine textColorCorB = null;
    List<string> og_LineTextA = new List<string>();
    List<string> og_LineTextB = new List<string>();
    private bool isReadDone = true;
    private float ratio;
    public bool isRatio = false;
    public int fontsize = 50;
    private float six_nine;
    private HelpTap _helpTap;
    void Awake()
    {
        if(Debug.isDebugBuild) Debug.Log("StoryPageBehavior Loaded");
        m_BaseStoryBehavior = FindObjectOfType<BaseStoryBehavior>();
        //tham số cho tỷ lệ màn hình 16:9
        six_nine = Mathf.Ceil(100 * ((float) 16 / (float) 9));
        ratio = Mathf.Ceil(100*((float) Screen.width / (float) Screen.height));
        HelpTap[] _helpTaps = gameObject.GetComponentsInChildren<HelpTap>();
        if(_helpTaps.Length==1) _helpTap = _helpTaps[0];
    }
    private void OnEnable()
    {
        foreach (GameObject go in m_HightlightsList)
        {
            go.SetActive(true);
            go.GetComponent<Image>().raycastTarget = false;
            go.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        if (this.GetComponent<AudioSource>() == null)
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        else
            m_AudioSource = this.GetComponent<AudioSource>();

        og_LineTextA.Clear();
        m_AudioSource.playOnAwake = false;
        foreach (Text txt in m_LineTextsListA)
        {
            og_LineTextA.Add(txt.text);
            setRatio(txt);
        }

        og_LineTextB.Clear();
        foreach (Text txt in m_LineTextsListB)
        {
            og_LineTextB.Add(txt.text);
            setRatio(txt);
        }
        if (_helpTap == null) LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);
    }
    // hàm set font size cho tỷ lệ màn hình 16/9
    private void setRatio(Text txt)
    {
        if(ratio == six_nine && isRatio)
        {
            txt.fontSize = fontsize;
        }
    }
    public void OnTriggerMainAudio()
    {

        if (m_MainAudioClipsList.Count > 0)
        {
            if (audioCor != null)
                StopCoroutine(audioCor);
            audioCor = StartCoroutine(MainAudioSequence());
        }
        else
        {
            //m_BaseStoryBehavior.OnStartCountingInactivity();
           LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);
        }
    }

    IEnumerator MainAudioSequence()
    {
        isReadDone = true;
        foreach (AudioClip clip in m_MainAudioClipsList)
        {
            //audio
            if (m_BaseStoryBehavior.isSoundEnabled == true)
            {
                m_AudioSource.Stop();
                m_AudioSource.clip = clip;
                m_AudioSource.Play();

                //thoại
                if (m_MainAudioClipsList.IndexOf(clip) < m_LineTextsListA.Count)
                {
                    parser = new SRTParser(m_Subtitle[m_MainAudioClipsList.IndexOf(clip)]);
                    textColorCorA = StartCoroutine(MainLinesSequence(m_LineTextsListA[m_MainAudioClipsList.IndexOf(clip)], parser));
                }
                if (m_MainAudioClipsList.IndexOf(clip) < m_LineTextsListB.Count)
                {
                    parser = new SRTParser(m_Subtitle[m_MainAudioClipsList.IndexOf(clip)]);
                    textColorCorB = StartCoroutine(MainLinesSequence(m_LineTextsListB[m_MainAudioClipsList.IndexOf(clip)], parser));
                }

                yield return new WaitForSeconds(clip.length);
                //m_BaseStoryBehavior.OnStartCountingInactivity();
                //LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);

            }
        }
        isReadDone = false;
        LeanTween.dispatchEvent(StoryEvent.Hightline, StoryEvent.HIGHTLINE);
        if (_helpTap != null) _helpTap.Begin(); 
        else LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);
    }

    IEnumerator MainLinesSequence(Text text, SRTParser parser)
    {
        float last_to = 0.0f;
        int j = 0;

        foreach (SubtitleBlock txtBlock in parser._subtitles)
        {
            j++;
            yield return new WaitForSeconds((float)txtBlock.From - last_to);
            string full_text = "";
            int i = 0;
            foreach (SubtitleBlock block in parser._subtitles)
            {
                i++;
                if (block == txtBlock)
                {
                    full_text += ("<b>" + block.Text + "</b> ");
                }
                else
                {
                    full_text += (block.Text + (i==(parser._subtitles.Count)?"":" "));
                }
            }
            full_text = full_text.Replace(System.Environment.NewLine, "");

            text.text = full_text;
            last_to = (float)txtBlock.To;
            yield return new WaitForSeconds((float)txtBlock.Length);
            full_text = full_text.Replace("<b>", "");
            full_text = full_text.Replace("</b> ", (j==parser._subtitles.Count?"":" "));
            text.text = full_text;
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void StopAll()
    {
        for (int i = 0; i < Math.Min(m_LineTextsListA.Count, og_LineTextA.Count); i++)
        {
            m_LineTextsListA[i].text = og_LineTextA[i];
        }

        for (int i = 0; i < Math.Min(m_LineTextsListB.Count, og_LineTextA.Count); i++)
        {
            m_LineTextsListB[i].text = og_LineTextB[i];
        }

        if (m_AudioSource != null)
            m_AudioSource.Stop();

        StopAllCoroutines();
    }

    public void TriggerHighlight(int index)
    {
        if (highlightCor != null)
            StopCoroutine(highlightCor);

        //m_BaseStoryBehavior.OnStartCountingInactivity();
        LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);
        StartCoroutine(HighlightingSequence(index));
    }

    IEnumerator HighlightingSequence(int index)
    {
        if (m_HLAudioClipsList.Count > 0 && index < m_HLAudioClipsList.Count)
        {
            //if (audioCor != null) StopCoroutine(audioCor);
            if (!isReadDone)
            {
                m_AudioSource.clip = m_HLAudioClipsList[index];
                m_AudioSource.Play();
            }
        }

        m_HightlightsList[index].SetActive(true);
        LeanTween.alpha(m_HightlightsList[index].GetComponent<RectTransform>(), 1.0f, 0.25f);
        yield return new WaitForSeconds(1.0f);
        LeanTween.alpha(m_HightlightsList[index].GetComponent<RectTransform>(), 0.0f, 0.25f).setOnComplete(() =>
            {
                m_HightlightsList[index].SetActive(false);
            }
        );
    }
}


public class SRTParser
{
    public List<SubtitleBlock> _subtitles;
    public SRTParser(string textAssetResourcePath)
    {
        var text = Resources.Load<TextAsset>(textAssetResourcePath);
        Load(text);
    }

    public SRTParser(TextAsset textAsset)
    {
        this._subtitles = Load(textAsset);
    }

    static public List<SubtitleBlock> Load(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("Subtitle file is null");
            return null;
        }

        var lines = textAsset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        var currentState = eReadState.Index;

        var subs = new List<SubtitleBlock>();

        int currentIndex = 0;
        double currentFrom = 0, currentTo = 0;
        var currentText = string.Empty;
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];

            switch (currentState)
            {
                case eReadState.Index:
                    {
                        int index;
                        if (Int32.TryParse(line, out index))
                        {
                            currentIndex = index;
                            currentState = eReadState.Time;
                        }
                    }
                    break;
                case eReadState.Time:
                    {
                        line = line.Replace(',', '.');
                        var parts = line.Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);

                        // Parse the timestamps
                        if (parts.Length == 2)
                        {
                            TimeSpan fromTime;
                            if (TimeSpan.TryParse(parts[0], out fromTime))
                            {
                                TimeSpan toTime;
                                if (TimeSpan.TryParse(parts[1], out toTime))
                                {
                                    currentFrom = fromTime.TotalSeconds;
                                    currentTo = toTime.TotalSeconds;
                                    currentState = eReadState.Text;
                                }
                            }
                        }
                    }
                    break;
                case eReadState.Text:
                    {
                        if (currentText != string.Empty)
                            currentText += "\r\n";

                        currentText += line;

                        // When we hit an empty line, consider it the end of the text
                        if (string.IsNullOrEmpty(line) || l == lines.Length - 1)
                        {
                            // Create the SubtitleBlock with the data we've aquired 
                            subs.Add(new SubtitleBlock(currentIndex, currentFrom, currentTo, currentText));

                            // Reset stuff so we can start again for the next block
                            currentText = string.Empty;
                            currentState = eReadState.Index;
                        }
                    }
                    break;
            }
        }
        return subs;
    }

    public SubtitleBlock GetForTime(float time)
    {
        if (_subtitles.Count > 0)
        {
            var subtitle = _subtitles[0];

            if (time >= subtitle.To)
            {
                _subtitles.RemoveAt(0);

                if (_subtitles.Count == 0)
                    return null;

                subtitle = _subtitles[0];
            }

            if (subtitle.From > time)
                return SubtitleBlock.Blank;

            return subtitle;
        }
        return null;
    }

    enum eReadState
    {
        Index,
        Time,
        Text
    }
}

public class SubtitleBlock
{
    static SubtitleBlock _blank;
    public static SubtitleBlock Blank
    {
        get { return _blank ?? (_blank = new SubtitleBlock(0, 0, 0, string.Empty)); }
    }
    public int Index { get; private set; }
    public double Length { get; private set; }
    public double From { get; private set; }
    public double To { get; private set; }
    public string Text { get; private set; }

    public SubtitleBlock(int index, double from, double to, string text)
    {
        this.Index = index;
        this.From = from;
        this.To = to;
        this.Length = to - from;
        this.Text = text;
    }
}