using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Stories;
public class TapStoryBehavior : MonoBehaviour, IPointerClickHandler
{
    private AudioSource audiosource;
    public AudioClip clip;
    public int Type = 1;
    public const int DELAY = 1;
    public const int TAP_NONE_ALPHA = 0;
    public const int TAP_ALPHA = 1;
    public GameObject[] highline;
    public bool isAlpha = false;
    public bool isON = false;
    public int id;
    void Start()
    {
        audiosource = gameObject.AddComponent<AudioSource>();
        audiosource.playOnAwake = false;
        audiosource.loop = false;
        if(Type== TAP_NONE_ALPHA) transform.GetChild(0).gameObject.SetActive(false);
        if (Type == TAP_ALPHA && highline != null)
        {
            sethighLine(false);
        }
        if (!isAlpha)
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.005f;
        }
    }
    public void sethighLine(bool ishighLine)
    {
        highline.ToObservable().Subscribe(x => x.SetActive(ishighLine));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clip != null)
        {
            if (StoryEvent.isHelp)
            {
                if (StoryEvent.IdHelp == id)
                {
                    Tap();
                    //transform.parent.parent.GetChild(2).GetComponent<HelpTap>().Stop();
                    transform.parent.parent.GetComponentsInChildren<HelpTap>()[0].Stop();
                }
            } else Tap();
        }
    }
    public void Tap()
    {
        if (clip != null)
        {
            audiosource.clip = clip;
            if (isON) audiosource.Play();
        }
        Glow(false);
    }
    public void Glow(bool isGlow)
    {
        if (Type == TAP_NONE_ALPHA)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            LeanTween.delayedCall(DELAY, () => {
                if (!isGlow) transform.GetChild(0).gameObject.SetActive(false);
            });
        }
        if (Type == TAP_ALPHA && highline != null)
        {
            sethighLine(true);
            LeanTween.delayedCall(DELAY, () => {
               if(!isGlow) sethighLine(false);
            });
        }
    }
    public void callOn()
    {
        isON = true;
    }
    public void callOff()
    {
        isON = false;
        sethighLine(false);
    }
}