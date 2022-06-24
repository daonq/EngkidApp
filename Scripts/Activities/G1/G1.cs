using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using Stories;
namespace G1
{
    public class Event
    {
        public const int ACTION = 3;
        public const int SOUND = 5;
        public const int DROP = 2;
        public const int CHECK = 4;
        public const string BEGIN = "Begin";
        public const string DRAG = "Drag";
        public const int FAIL = 6;
        public const int FAIL_MAX = 7;
        public const int HELP = 8;
        public const int TIME_HELP = 15;
        public static bool LOCK = false;
    }
    [System.Serializable]
    public class StatusPage
    {
        public int status=0;
        public int Fail = 0;
        public string content = "";
        public float getScore()
        {
            float score = 0;
            if (Fail == 0) score = 1;
            if (Fail == 1) score = 0.6f;
            if (Fail >= 2) score =  0.3f;
            return score;
        }
    }
    [System.Serializable]
    public class savePage
    {
        public StatusPage[] statuspage;
    }
    public class G1 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brige;
        public Tutorial tutorial;
        public ScrollRect scrollRect;
        public Book book;
        public Contents[] contents;
        private Dictionary<string, int> _dicClips = new Dictionary<string, int>();
        private int _count=0;
        public Drag[] drags;
        public savePage[] saveAlbum;
        public GameObject[] taps;
        private Vector3 _oldPosBook;
        private GameObject _box;
        private void Awake()
        {
            _dicClips.Add("back", 0);
            _dicClips.Add("tap", 1);
            _dicClips.Add("yes", 2);
            _dicClips.Add("no", 3);
            _dicClips.Add("appear", 4);
            _dicClips.Add("paper", 5);
            _dicClips.Add("click", 6);
            _dicClips.Add("drag", 1);
            _dicClips.Add("yes1", 7);
            _dicClips.Add("yes2", 8);
            _dicClips.Add("yes3", 9);
            _dicClips.Add("no1", 10);
            _dicClips.Add("no2", 11);
            _dicClips.Add("no3", 12);
            LeanTween.addListener(gameObject, Event.ACTION, OnEvent);
            LeanTween.addListener(gameObject, Event.DROP, OnDropEvent);
            LeanTween.addListener(gameObject, Event.SOUND, OnSoundEvent);
            LeanTween.addListener(gameObject, Event.CHECK, OnCheckEvent);
            LeanTween.addListener(gameObject, Event.FAIL_MAX,OnFailEvent);
            LeanTween.addListener(gameObject, Event.HELP, OnHelpEvent);
            LeanTween.addListener(gameObject, Event.FAIL, OnUpdateEvent);
            LeanTween.addListener(gameObject, StoryEvent.TouchPage, OnEventTouchPage);
            _oldPosBook = book.transform.localPosition;
            contents.ToObservable().Subscribe(x => {
                x.pageContent[0].setContent();
                x.pageContent[1].setContent();
            });
        }
        public void removeListenner()
        {
            LeanTween.removeListener(gameObject, Event.ACTION,OnEvent);
            LeanTween.removeListener(gameObject, Event.DROP,OnDropEvent);
            LeanTween.removeListener(gameObject, Event.SOUND,OnSoundEvent);
            LeanTween.removeListener(gameObject, Event.CHECK,OnCheckEvent);
            LeanTween.removeListener(gameObject, Event.FAIL_MAX,OnFailEvent);
            LeanTween.removeListener(gameObject, Event.HELP,OnHelpEvent);
            LeanTween.removeListener(gameObject, Event.FAIL,OnUpdateEvent);
        }
        public void Again()
        {
            StopAllCoroutines();
            LeanTween.cancel(book.pages[0].btnDone.gameObject);
            LeanTween.cancel(book.pages[1].btnDone.gameObject);
            book.pages[0].box.parent.GetChild(3).gameObject.SetActive(true);
            book.pages[1].box.parent.GetChild(3).gameObject.SetActive(true);
            scrollRect.gameObject.SetActive(true);
            book.transform.localPosition = _oldPosBook;
            saveAlbum.ToObservable().Subscribe(x => {
                x.statuspage[0].status = 0;
                x.statuspage[0].Fail = 0;
                x.statuspage[0].content = "";
                x.statuspage[1].status = 0;
                x.statuspage[1].Fail = 0;
                x.statuspage[1].content = "";
            });
            _count = 0;
            contents.ToObservable().Subscribe(x => {
                x.pageContent[0].Box.transform.GetChild(2).GetComponent<Drag>().close();
                x.pageContent[0].Box.transform.GetChild(2).GetComponent<Drag>().enabled = true;
                x.pageContent[0].Box.transform.GetChild(2).GetComponent<Drag>().transform.parent.gameObject.SetActive(true);
                x.pageContent[0].Box.transform.GetChild(0).gameObject.SetActive(false);
                x.pageContent[0].Box.transform.GetChild(1).gameObject.SetActive(false);

                x.pageContent[1].Box.transform.GetChild(0).gameObject.SetActive(false);
                x.pageContent[1].Box.transform.GetChild(2).GetComponent<Drag>().close();
                x.pageContent[1].Box.transform.GetChild(2).GetComponent<Drag>().enabled = true;
                x.pageContent[1].Box.transform.GetChild(2).GetComponent<Drag>().transform.parent.gameObject.SetActive(true);
                x.pageContent[1].Box.transform.GetChild(1).gameObject.SetActive(false);
                LeanTween.alpha(x.pageContent[0].Box.GetComponent<RectTransform>(), 1, 0.1f);
                LeanTween.alpha(x.pageContent[1].Box.GetComponent<RectTransform>(), 1, 0.1f).setOnComplete(() =>
                {
                    setAlpha(x.pageContent[0].Box.transform.GetChild(2).GetComponent<Image>());
                    setAlpha(x.pageContent[1].Box.transform.GetChild(2).GetComponent<Image>());
                });
            });
            book.pages[0].txt.gameObject.SetActive(false);
            book.pages[1].txt.gameObject.SetActive(false);
            book.pages[0].isEnd = false;
            book.pages[1].isEnd = false;
            Begin();
            scrollRect.horizontalScrollbar.value = 0;
        }
        void setAlpha(Image a1)
        {
            a1.color = new Color(a1.color.r, a1.color.g, a1.color.b, 0);
        }
        void Start()
        {
            LeanTween.delayedCall(0.5f, () => {
                Begin();
                tutorial.gameObject.SetActive(false);
            });
            scrollRect.horizontalScrollbar.value = 0;
        }
        public void OpenTutorial()
        {
            tutorial.gameObject.SetActive(true);
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            StartCoroutine(tutorial.Play());
        }
        public void CloseTutorial()
        {
            StopCoroutine(tutorial.Play());
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            tutorial.gameObject.SetActive(false);
        }
        void OnSoundEvent(LTEvent e)
        {
            soundManager.Play(_dicClips[e.data as string]);
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case Event.BEGIN:
                    tutorial.isTutorial = false;
                    CloseTutorial();
                    break;
                default:
                    break;
            }
        }
        void OnDropEvent(LTEvent e)
        {
            Frame frame = e.data as Frame;
            frame.transform.GetChild(0).gameObject.SetActive(true);
            Button btnDone = frame.transform.parent.GetChild(4).GetComponent<Button>();
            btnDone.interactable = true;
            saveAlbum[_count].statuspage[frame.id].status = 1;
            saveAlbum[_count].statuspage[frame.id].content = frame.transform.GetChild(0).GetComponent<Text>().text;
            saveAlbum[_count].statuspage[frame.id].Fail = frame.Fail;
            LeanTween.cancel(frame.transform.GetChild(0).gameObject);
            if(btnDone.interactable) StartCoroutine(NoTap(btnDone.gameObject));
            var dragBack = drags.Where(x => x.transform.GetChild(0).GetComponent<Text>().text == frame.OldContent).ToArray();
            if (dragBack != null  && dragBack.Length > 0 && frame.OldContent!="" && dragBack[0].id==(_count+1))
            {
                dragBack[0].close();
            }
        }
        private IEnumerator NoTap(GameObject btn)
        {
            btn.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(Event.TIME_HELP);
            if (btn.GetComponent<Button>().interactable)
            {
                LeanTween.scale(btn, 1.2f * Vector3.one, 0.5f)
                        .setDelay(0.5f).setRepeat(5)
                        .setOnComplete(() => StartCoroutine(NoTap(btn)));
            }
            
        }
        void Begin()
        {
            book.Close();
            LeanTween.delayedCall(1.5f, () => {
                setContent(_count);
                book.Open();
                LeanTween.dispatchEvent(Event.SOUND, "paper");
                if (tutorial.isTutorial)
                {
                    LeanTween.delayedCall(soundManager.PlayByTime(0) + 1, () => {
                        tutorial.gameObject.SetActive(true);
                        OpenTutorial();
                    });
                }
            });
        }
        void setContent(int stt)
        {
            book.stt.transform.GetChild(0).GetComponent<Text>().text = contents[stt].page;
            book.stt.SetActive(false);
            ContentInPage(0);
            ContentInPage(1);
        }
        void ContentInPage(int k)
        {
            book.pages[k].pic.GetComponent<Image>().sprite = contents[_count].pageContent[k].pic;
            book.pages[k].box.GetComponent<Image>().enabled = true;
            book.pages[k].box.GetComponent<Frame>().content = contents[_count].pageContent[k].sentences;
            if (book.pages[k].box.GetComponent<Frame>().content=="")
            {
                book.pages[k].btnDone.interactable = false;
            }
            book.pages[k].box.GetComponent<Frame>().Fail = 0;
            book.pages[k].clipByTime = contents[_count].pageContent[k].clipByTime;
            book.pages[k].pos = contents[_count].pageContent[k].pos;
            book.pages[k].txt.text = contents[_count].pageContent[k].sentences;
            book.pages[k].txt.gameObject.SetActive(false);
        }
        void OnCheckEvent(LTEvent e)
        {
            StopAllCoroutines();
            LeanTween.cancelAll();
            var btn = e.data as GameObject;
            btn.transform.localScale = Vector3.one;
            int length = book.pages.Select(x => x.btnDone).Where(y => !y.gameObject.activeSelf).ToArray().Length;
            Frame frame = book.pages.Where(y => y.btnDone.gameObject == btn).ToArray()[0].box.GetComponent<Frame>();
            LeanTween.alpha(frame.drag.transform.parent.GetComponent<RectTransform>(), 0.001f,0f);
            frame.drag.transform.parent.gameObject.SetActive(false);
            frame.drag.enabled = false;
            
            saveAlbum[_count].statuspage[frame.id].status = 2;
            saveAlbum[_count].statuspage[frame.id].Fail = frame.Fail;
            saveAlbum[_count].statuspage[frame.id].content = frame.transform.GetChild(0).GetComponent<Text>().text;
            //scrollRect.normalizedPosition = Vector2.zero;
            
            if (length == 2)
            {
                //if (scrollRect.horizontalScrollbar.value < 0.6f) scrollRect.horizontalScrollbar.value += 0.25f;
                var DropFull = saveAlbum.Where(x => x.statuspage[0].status == 2 && x.statuspage[1].status == 2).ToArray();
                if (DropFull.Length == contents.Length) StartCoroutine(OpenAlbum());
                if (_count == 0) scrollRect.horizontalScrollbar.value = 0.3f;
                if (_count == 1) scrollRect.horizontalScrollbar.value = 0.55f;
                if (_count == 2) scrollRect.horizontalScrollbar.value = 0.65f;
            } 
            else
            {
                if (_count == 0) scrollRect.horizontalScrollbar.value = 0.2f;
                if (_count == 1) scrollRect.horizontalScrollbar.value = 0.45f;
                if (_count == 2) scrollRect.horizontalScrollbar.value = 0.55f;
            }
        }
        void OnUpdateEvent(LTEvent e)
        {
            Frame frame = e.data as Frame;
            saveAlbum[_count].statuspage[frame.id].Fail = frame.Fail;
            saveAlbum[_count].statuspage[frame.id].content = "";
        }
        private void OnEventTouchPage(LTEvent e)
        {
            if (!Event.LOCK)
            {
                StopAllCoroutines();
                LeanTween.cancelAll();
                book.closeGlow();
                if (_box != null) _box.transform.GetChild(2).GetComponent<Drag>().StopScale();
                switch (e.data as string)
                {
                    case StoryEvent.PAGERIGHT:
                        if (_count < contents.Length - 1)
                        {
                            _count++;
                            book.clear();
                            book.Next();
                            setContent(_count);
                            LeanTween.delayedCall(1.1f, checkStatus);
                        }
                        break;
                    case StoryEvent.PAGELEFT:
                        if (_count >= 1)
                        {
                            _count--;
                            book.clear();
                            book.Back();
                            setContent(_count);
                            LeanTween.delayedCall(1.1f, checkStatus);
                        }
                        break;
                    default:
                        break;
                }
            }
            
        }
        public void checkStatus()
        {
            statusShow(0);
            statusShow(1);
        }
        public void statusShow(int n)
        {
            if (saveAlbum[_count].statuspage[n].status == 1)
            {
                book.pages[n].txt.gameObject.SetActive(false);
                book.pages[n].btnDone.interactable = (saveAlbum[_count].statuspage[n].content!="")?true:false;
                book.pages[n].box.gameObject.SetActive(true);
                book.pages[n].box.GetComponent<Frame>().transform.GetChild(0).gameObject.SetActive(true);
                book.pages[n].box.GetComponent<Frame>().transform.GetChild(0).GetComponent<Text>().text = saveAlbum[_count].statuspage[n].content;
            }
            if (saveAlbum[_count].statuspage[n].status == 2)
            {
                book.pages[n].txt.gameObject.SetActive(true);
                book.pages[n].btnDone.gameObject.SetActive(false);
                book.pages[n].box.gameObject.SetActive(false);
                book.pages[n].txt.text = saveAlbum[_count].statuspage[n].content;
            }
        }
        private void OnFailEvent(LTEvent e)
        {
            OnGlowDrag(e.data as Frame,false);
        }
        private void OnHelpEvent(LTEvent e)
        {
            OnGlowDrag(e.data as Frame,true);
        }
        public void OnGlowDrag(Frame frame,bool isScale)
        {
           
            Contents _content = contents.Where(x => x.pageContent[frame.id].sentences == frame.content).ToArray()[0];
            _box = _content.pageContent[frame.id].Box;
            /*
            LeanTween.moveX(scrollRect.content, _content.pageContent[frame.id].pos.x, 1).setOnComplete(() => {
                if (!isScale)
                {
                    _box.transform.GetChild(1).gameObject.SetActive(true);
                }
                else ScaleLabel(_box, _content.pageContent[frame.id].pos.x);
            });*/
            if (!isScale)
            {
                _box.transform.GetChild(1).gameObject.SetActive(true);
            }
            else ScaleLabel(_box, _content.pageContent[frame.id].pos.x);
        }
        private void ScaleLabel(GameObject box,float pos)
        {
            //LeanTween.moveX(scrollRect.content, pos, 1);
            LeanTween.scale(box.transform.GetChild(2).transform.GetChild(0).gameObject, 1.2f * Vector3.one, 0.5f).setDelay(0.5f).setRepeat(5).setOnComplete(() => {
                    box.transform.GetChild(2).transform.GetChild(0).transform.localScale = Vector3.one;
                    LeanTween.delayedCall((float) Event.TIME_HELP,()=> ScaleLabel(box,pos));
              });
        }
        private IEnumerator OpenAlbum()
        {
            yield return new WaitForSeconds(3);
            book.pages[0].isEnd = true;
            book.pages[1].isEnd = true;
            taps[0].SetActive(false);
            taps[1].SetActive(false);
            scrollRect.gameObject.SetActive(false);
            book.transform.localPosition = new Vector3(-50,-100,0);
            contents.ToObservable().Subscribe(x => {
                x.pageContent[0].Box.transform.GetChild(2).GetComponent<Drag>().close();
                x.pageContent[1].Box.transform.GetChild(2).GetComponent<Drag>().close();
                x.pageContent[0].Box.transform.GetChild(2).GetComponent<Drag>().enabled = false;
                x.pageContent[1].Box.transform.GetChild(2).GetComponent<Drag>().enabled = false;
            });
            book.hide();
            book.pages[0].pic.transform.GetChild(0).gameObject.SetActive(false);
            book.pages[1].pic.transform.GetChild(0).gameObject.SetActive(false);
            book.pages[0].txt.text="";
            book.pages[1].txt.text = "";
            yield return new WaitForSeconds(1);
            for (int i = 0; i < contents.Length; i++)
            {
                book.pages[0].txt.text = "";
                book.pages[1].txt.text = "";
                book.pages[0].txt.gameObject.SetActive(false);
                book.pages[1].txt.gameObject.SetActive(false);
                book.pages[1].pic.gameObject.SetActive(false);
                book.pages[0].end();
                book.pages[1].end();
                book.stt.gameObject.SetActive(false);
                book.Next();
                LeanTween.dispatchEvent(Event.SOUND,"paper");
                //yield return new WaitForSeconds(1);
                setContent(i);
                book.pages[0].pic.sprite = contents[i].pageContent[0].pic;
                book.pages[1].pic.sprite = contents[i].pageContent[1].pic;
                book.pages[0].pic.transform.GetChild(0).gameObject.SetActive(true);
                book.pages[1].pic.transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitForSeconds(1.5f);
                book.stt.gameObject.SetActive(true);
                book.pages[1].pic.gameObject.SetActive(true);
                book.pages[0].txt.gameObject.SetActive(false);
                book.pages[1].txt.gameObject.SetActive(false);

                book.pages[0].txt.text = contents[i].pageContent[0].sentences;
                book.pages[1].txt.text = contents[i].pageContent[1].sentences;
                yield return new WaitForSeconds(0.5f);
                book.pages[0].txt.gameObject.SetActive(true);
                book.Play(contents[i].pageContent[0].clipByTime.clip);
                book.pages[0].txt.GetComponent<Write>().Begin();
                book.pages[0].pic.transform.GetChild(0).gameObject.SetActive(true);
                book.pages[1].pic.transform.GetChild(0).gameObject.SetActive(false);
                contents.ToObservable().Subscribe(x => {
                    x.pageContent[0].Box.transform.GetChild(1).gameObject.SetActive(false);
                    x.pageContent[1].Box.transform.GetChild(1).gameObject.SetActive(false);
                });
                OnGlowDrag(book.pages[0].box.GetComponent<Frame>(),false);
                yield return new WaitForSeconds(book.pages[0].clipByTime.time+1.2f);
                book.pages[1].txt.gameObject.SetActive(true);
                book.Play(contents[i].pageContent[1].clipByTime.clip);
                book.pages[1].txt.GetComponent<Write>().Begin();
                book.pages[0].pic.transform.GetChild(0).gameObject.SetActive(false);
                book.pages[1].pic.transform.GetChild(0).gameObject.SetActive(true);
                contents.ToObservable().Subscribe(x => {
                    x.pageContent[0].Box.transform.GetChild(1).gameObject.SetActive(false);
                    x.pageContent[1].Box.transform.GetChild(1).gameObject.SetActive(false);
                });
                OnGlowDrag(book.pages[1].box.GetComponent<Frame>(),false);
                yield return new WaitForSeconds(book.pages[1].clipByTime.time+1.2f);
                book.pages[0].pic.transform.GetChild(0).gameObject.SetActive(false);
                book.pages[1].pic.transform.GetChild(0).gameObject.SetActive(false);
                book.pages[0].txt.gameObject.SetActive(false);
                book.pages[1].txt.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(1);
            book.pages[0].txt.text = "";
            book.pages[1].txt.text = "";
            book.pages[0].pic.transform.parent.GetComponent<Image>().enabled = false;
            book.pages[1].pic.transform.parent.GetComponent<Image>().enabled = false;
            book.pages[0].pic.gameObject.SetActive(false);
            book.pages[1].pic.gameObject.SetActive(false);
            book.stt.gameObject.SetActive(false);
            book.Close();
            yield return new WaitForSeconds(1);
            End();
        }
        public void End()
        {
            StopCoroutine(OpenAlbum());
            var _score = saveAlbum.Select(x => x.statuspage[0].getScore()+x.statuspage[1].getScore()).Aggregate((x,y)=>x+y);
            brige._score = Mathf.CeilToInt(_score);
            brige.ShowPopup();
        }
    } 
 }

