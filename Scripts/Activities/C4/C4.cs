using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
namespace C4
{
    public class Event
    {
        public const int ACTION = 3;
        public const int SOUND = 5;
        public const string BEGIN = "Begin";
        public const int DRAG = 1;
        public const int CHANGE = 2;
        public const string BEGIN_DRAG = "BeginDrag";
        public const string END_DRAG = "EndDrag";
    }
    public class C4 : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brige;
        public Contents[] contents;
        public Tutorial tutorial;
        public DragInBox dragInBox;
        public Window window;
        public DragInFrame[] dragInFrames;
        public Button btnDone;
        public Book book;
        public Transform Point;
        private float _score = 0;
        private int _countFail = 0;
        private Dictionary<string, int> _dicClips = new Dictionary<string, int>();
        private void Awake()
        {
            _dicClips.Add("back", 0);
            _dicClips.Add("tap", 1);
            _dicClips.Add("yes", 2);
            _dicClips.Add("no", 3);
            _dicClips.Add("appear", 4);
            _dicClips.Add("head", 5);
            LeanTween.addListener(gameObject,Event.ACTION,OnEvent);
            LeanTween.addListener(gameObject,Event.DRAG,OnDragEvent);
            LeanTween.addListener(gameObject,Event.SOUND,OnSoundEvent);
            LeanTween.addListener(gameObject,Event.CHANGE,OnChangeEvent);
            tutorial.contents = contents ;
            dragInBox.targets = contents.Select(x => x.frame).ToArray();
            window.pics = contents.Select(x => x.picInWindow).ToArray();
            window.contents = contents.Select(x => x.sentences).ToArray();
            contents.ToObservable().Subscribe(y => {
                y.frame.GetComponent<Frame>().setText(y.sentences);
                y.frame.GetComponent<Frame>().pics = contents.Select(s => s.picInframe).ToArray();
            });
            dragInBox.dragInFrames = dragInFrames;
            dragInFrames.ToObservable().Subscribe(x => x.enabled = false);
        }
        public void removeListenner()
        {
            LeanTween.removeListener(gameObject, Event.ACTION,OnEvent);
            LeanTween.removeListener(gameObject, Event.SOUND,OnSoundEvent);
            LeanTween.removeListener(gameObject, Event.DRAG, OnDragEvent);
            LeanTween.removeListener(gameObject, Event.CHANGE, OnChangeEvent);
        }
        public void Again()
        {
            window.close();
            Begin();
            _countFail = 0;
            _score = 0;
            dragInBox.close();
            dragInFrames.ToObservable().Subscribe(x => { 
                x.close();
                x.myframe.Start();
                x.myframe.Alpha(false);
                x.myframe.enabled = true;
                x.id = -1;
                x.myframe.Distance = 2000;
                x.myframe.isClose = false;
            });
        }
        public void Close()
        {
            StopAllCoroutines();
            LeanTween.cancelAll();
            StartCoroutine(tutorial.close());
            tutorial.gameObject.SetActive(false);
            tutorial.enabled = false;
        }
        void Start()
        {
            btnDone.interactable = false;
            OpenTutorial();
        }
        public void OpenTutorial()
        {
            tutorial.gameObject.SetActive(true);
            tutorial.enabled = true;
            StartCoroutine(tutorial.Begin());
        }
        void Begin()
        {
            dragInBox.Begin();
            window.Begin();
            book.GetComponent<Button>().interactable = true;
            dragInFrames.ToObservable().Subscribe(x =>
            {
                x.enabled = false;
            });
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
                    Begin();
                    break;
                case Event.BEGIN_DRAG:
                    book.GetComponent<Button>().interactable = false;
                    break;
                case Event.END_DRAG:
                    book.GetComponent<Button>().interactable = true;
                    break;
                default:
                    break;
            }
        }
        void OnDragEvent(LTEvent e)
        {
            OnDrag();
            int idback = (int) e.data;
            dragInFrames.Where(x=>!x.myframe.isYes).ToArray().ToObservable().Subscribe(x => {
                x.enabled = true;
            });
            book.CloseGlow();
        }
        void OnChangeEvent(LTEvent e)
        {
            Sprite sprite = e.data as Sprite;
            Contents content = contents.Where(x => x.picInWindow == sprite).ToArray()[0];
            Frame[] boxCurrent = dragInFrames.Where(x => x.myframe.content.GetComponent<Image>().sprite == content.picInframe).Select(y=>y.myframe).ToArray();
            Frame frame = content.frame.GetComponent<Frame>();
            if (boxCurrent.Length>0 && boxCurrent[0].content.GetComponent<Image>().sprite == content.picInframe)
            {
                window.Alpha();
                dragInBox.transform.localScale = Vector3.zero;
            }
            else
            {
                dragInBox.close();
            }
        }
        private void OnDrag()
        {
            int countDrag = dragInFrames.Where(x => x.myframe.content.activeSelf && x.id!=-1).ToArray().Length;
            if (countDrag == contents.Length)
            {
                btnDone.interactable = true;
            }
        }
        private void End()
        {
            if (_countFail == 0) _score =  contents.Length;
            if (_countFail == 1) _score = 0.65f * contents.Length;
            if (_countFail == 2) _score = 0.3f * contents.Length;
            if (_countFail > 2) _score = 0;
            brige._score = Mathf.CeilToInt(_score);
            brige.ShowPopup();
            _countFail = 0;
        }
        public void check()
        {
            int i = 0;
            var LengthCheck = dragInFrames.Where(x => x.GetComponent<Image>().sprite==contents[i++].picInframe).ToArray();
            _score = LengthCheck.Length;
            btnDone.interactable = false;
            book.GetComponent<Button>().interactable = false;
            book.CloseGlow();
            if (LengthCheck.Length < contents.Length)
            {
                i = 0;
                dragInFrames.ToObservable().Subscribe(x =>
                {
                    if (x.GetComponent<Image>().sprite == contents[i++].picInframe && x.id!=-1)
                    {
                        x.myframe.Yes();
                        x.enabled = false;
                        x.myframe.enabled = false;
                        x.myframe.Alpha(true);
                        x.myframe.isClose = true;
                        x.myframe.id = -1;
                    }
                    else
                    {
                        x.myframe.No();
                        x.enabled = false;
                        x.myframe.isClose = false;
                    }
                });
                _countFail++;
                LeanTween.delayedCall(1.5f, Fail);
            }
            else {
                dragInFrames.ToObservable().Subscribe(x =>
                {
                    x.myframe.Yes();
                    x.enabled = false;
                    x.myframe.enabled = false;
                    x.myframe.Alpha(true);
                    x.myframe.isClose = true;
                    x.myframe.id = -1;
                });
                LeanTween.delayedCall(soundManager.PlayByTime(4),End);
            }
        }
        private void Fail()
        {
            book.GetComponent<Button>().interactable = true;
            if (_countFail == 1|| _countFail >= 2)
            {
                soundManager.PlayByTime(_countFail == 1?5:6);
                book.Glow();
                int i = 0;
                var frameFail = dragInFrames.Where(x => x.GetComponent<Image>().sprite != contents[i++].picInframe).ToArray();
                frameFail.ToObservable().Subscribe(f =>
                {
                    f.Alpha(false);
                    f.myframe.Clear();
                    LeanTween.moveLocal(f.gameObject, Point.localPosition, 0.5f).setDelay(0.3f).setOnComplete(() =>
                    {
                        f.Alpha(true);
                        f.close();
                        if (_countFail >= 2) f.myframe.ShowText();
                    });
                });
                LeanTween.delayedCall(2, () =>
                {
                    window.stt = frameFail[0].id;
                    window.SetDefault();
                    dragInBox.close();
                    dragInBox.Fail = _countFail;
                    if (_countFail >= 2)
                    {
                        int i = 0;
                        contents.ToObservable().Subscribe(y => {
                            y.frame.GetComponent<Frame>().id = i++;
                        });
                    }
                });
            }
        }
    }
}
