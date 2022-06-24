using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
namespace G5
{
    public class G5Reuse : MonoBehaviour
    {
        [Header("setting")]
        public SoundManager soundManager;
        public BrigeToResult brigeToResult;
        public Contents[] contents;
        public G5UI UI;
        public Tutorial tutorial;
        private int _count=0;
        public PrinterInReuse[] prints;
        public BookInReuse book;
        public SkeletonGraphic mouse;
        public SkeletonGraphic Keybroad;
        public Text end;
        public PosByContents[] pos;
        private Dictionary<string, int> _dicClips = new Dictionary<string, int>();
        private ContentByTime _sPrint;
        private int _sttPrint = 0;
        private int _score = 0;
        private float TimeGLow = 15;
        private List<ContentByTime> Albums = new List<ContentByTime>();
        private AudioSource _audio;
        private Drag DragCrrent;
        public bool isTutorial = true;
        private bool _isDrop = false;
        private int PrintAgain=0;

        private void Awake()
        {
            _dicClips.Add("back", 0);
            _dicClips.Add("tap", 1);
            _dicClips.Add("drag", 2);
            _dicClips.Add("paper", 3);
            _dicClips.Add("print", 4);
            _dicClips.Add("drop", 5);
            _dicClips.Add("appear", 6);
            LeanTween.addListener(gameObject,G5Event.ACTION,OnEvent);
            LeanTween.addListener(gameObject,G5Event.DROP,OnDropEvent);
            LeanTween.addListener(gameObject,G5Event.SOUND,OnSoundEvent);
            _audio = GetComponent<AudioSource>();
        }
        public void Play(AudioClip clip)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        public void removeListenner()
        {
            LeanTween.removeListener(gameObject, G5Event.ACTION,OnEvent);
            LeanTween.removeListener(gameObject, G5Event.SOUND,OnSoundEvent);
            LeanTween.removeListener(gameObject, G5Event.DROP,OnDropEvent);
        }
        public void Again()
        {
            _score = 0;
            _count = 0;
            _sttPrint = 0;
            PrintAgain = 0;
            isTutorial = false;
            UI.btnEdit.interactable = true;
            UI.btnDone.IsActive = true;
            UI.btnBook.interactable = true;
            UI.Drags.ToObservable().Subscribe(x => {
                x.GetComponent<Drag>().close();
                x.GetComponent<Drag>().isPrint = true;
            });
            UI.Start();
            prints.ToObservable().Subscribe(x => {
                x.clear();
            });
            G5Event.IsLock = false;
            end.text = "";
            Albums = new List<ContentByTime>();
            book.Albums = new List<ContentByTime>();
        }
        void Start()
        {
            Begin();
            tutorial.gameObject.SetActive(false);
            if (isTutorial)
            {
                LeanTween.delayedCall(2, () => {
                    tutorial.gameObject.SetActive(true);
                    tutorial.Begin();
                });
            }
        }
        void Begin()
        {
            UI.Start();
            Albums = new List<ContentByTime>();
            book.Albums = new List<ContentByTime>();
            mouse.AnimationState.SetAnimation(0, "idle", true);
            end.text = "";
        }
        void OnSoundEvent(LTEvent e)
        {
            string str = e.data as string;
            if(str!=null) soundManager.Play(_dicClips[str]);
        }
        public void OpenTutorial()
        {
            tutorial.close();
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case G5Event.BEGIN:
                    UI.btnEdit.interactable = true;
                    StartCoroutine(NoTap(UI.btnEdit.gameObject));
                    break;
                case G5Event.END:
                    End();
                    break;
                case G5Event.DRAG:
                    StopAllCoroutines();
                    UI.Drop.GetChild(0).gameObject.SetActive(false);
                    break;
                case G5Event.PRINT:
                    OnPrint();
                    break;
                case G5Event.DONE:
                    OnDone();
                    break;
                default:
                    break;
            }
        }
        public void clearContent()
        {
            UI.Drop.gameObject.SetActive(true);
            UI.txtcontent.gameObject.SetActive(true);
            end.gameObject.SetActive(true);
        }
        private IEnumerator NoTap(GameObject btn)
        {
            btn.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(TimeGLow);
            soundManager.PlayByTime(1);
            LeanTween.scale(btn,1.2f * Vector3.one, 0.5f)
                        .setDelay(0.5f).setRepeat(5)
                        .setOnComplete(()=>StartCoroutine(NoTap(btn)));
        }
        private IEnumerator GlowNoTapDrop()
        {
            yield return new WaitForSeconds(TimeGLow);
            UI.Drop.GetChild(0).gameObject.SetActive(true);
            soundManager.PlayByTime(0);
            Keybroad.AnimationState.SetAnimation(0, "animation", false);
            yield return new WaitForSeconds(5);
            UI.Drop.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(GlowNoTapDrop());
        }
        public void closeScaleLoop(Button btn)
        {
            StopAllCoroutines();
            LeanTween.cancel(btn.gameObject);
            btn.interactable = false;
        }
        public void closeScaleLoopByImage(Image btn)
        {
            StopAllCoroutines();
            LeanTween.cancel(btn.gameObject);
            btn.GetComponent<G5Btn>().IsActive = false;
        }
        public void Play()
        {
            closeScaleLoop(UI.btnEdit);
            UI.Begin();
            _count = 0;
            _sttPrint = 0;
            LeanTween.dispatchEvent(G5Event.SOUND,"tap");
            mouse.AnimationState.SetAnimation(0, "move", false);
            SetContent();
            StartCoroutine(GlowNoTapDrop());
            LeanTween.delayedCall(1, () => soundManager.PlayByTime(0));
            UI.Drags.ToObservable().Subscribe(x => {
                x.GetComponent<Drag>().isPrint = true;
            });
        }
        void OnDropEvent(LTEvent e)
        {
            Transform tran = e.data as Transform;
            string txtPrint = tran.GetChild(1).GetComponent<Text>().text;
            var color = UI.Drop.GetComponent<Drop>().txt.color;
            UI.Drop.GetComponent<Drop>().txt.color = new Color(color.r, color.g, color.b,1);
            if (_count<contents.Length) _sPrint = contents[_count].contentByTime.Where(x => x.word == txtPrint).ToArray()[0];
            UI.btnPrint.IsActive = true;
            UI.btnPrint.SetStatus(1);
            UI.btnDone.IsActive = false;
            UI.btnDone.SetStatus(0);
            //&& !x.GetComponent<Drag>().isPrint
            UI.Drags.Where(x => x != tran).ToObservable().Subscribe(xx => {
                xx.GetComponent<Drag>().close();
            });
            DragCrrent = tran.GetComponent<Drag>();
            StartCoroutine(NoTap(UI.btnPrint.gameObject));
            _isDrop = false;
        }
        public void OnPrint()
        {
            closeScaleLoopByImage(UI.btnPrint.GetComponent<Image>());
            LeanTween.dispatchEvent(G5Event.SOUND,"tap");
            mouse.AnimationState.SetAnimation(0,"move", false);
            G5Event.IsLock = true;
            if (!_isDrop)
            {
                LeanTween.delayedCall(0.3f,()=>prints[_sttPrint].OutPut(_sPrint.picInPrint));
            }
            else
            {
                if (PrintAgain >= 1)
                {
                    PrintAgain = -1;
                    prints[4].OutPut(_sPrint.picInPrint);
                    LeanTween.delayedCall(3f,()=>prints[4].Normal());
                }
                else
                {
                    prints[3].OutPut(_sPrint.picInPrint);
                    LeanTween.delayedCall(3f, () => prints[3].Normal());
                }
                PrintAgain++;
            }
            if (!_isDrop)  Albums.Add(_sPrint);
            DragCrrent.isPrint = true;
            LeanTween.delayedCall(0.5f,() => {
                LeanTween.delayedCall(1.5f,()=>
                {
                    Play(_sPrint.clip);
                    LeanTween.delayedCall(_sPrint.time, () =>
                    {
                        UI.btnDone.IsActive = true;
                        UI.btnDone.SetStatus(1);
                        UI.btnPrint.IsActive = true;
                        UI.btnPrint.SetStatus(1);
                        G5Event.IsLock = false;
                        StartCoroutine(NoTap(UI.btnDone.gameObject));
                    });
                });
                if(!_isDrop)_sttPrint++;
                _isDrop = true;
            });
        }
        public void OpenBook()
        {
            LeanTween.dispatchEvent(G5Event.SOUND, "tap");
            LeanTween.pauseAll();
            StopAllCoroutines();
            mouse.AnimationState.SetAnimation(0, "move", false);
            LeanTween.delayedCall(0.3f, () => {
                if (book.Albums.Count == 0 || book.Albums == null) book.Begin();
                else book.Open();
            });
        }
        public void Hint()
        {
            LeanTween.dispatchEvent(G5Event.SOUND, "tap");
            StopAllCoroutines();
            StartCoroutine(tutorial.PlayHint());
        }
        public void CloseBook()
        {
            book.close();
            LeanTween.resumeAll();
            LeanTween.dispatchEvent(G5Event.SOUND, "back");
        }
        public void OnDone()
        {
            G5Event.IsLock = true;
            UI.btnPrint.SetStatus(0);
            closeScaleLoopByImage(UI.btnDone.GetComponent<Image>());
            UI.Drop.GetChild(1).GetComponent<Text>().text = " ";
            _count++;
            _score += _sttPrint;
            book.Albums = Albums;
            if (_count < contents.Length) LeanTween.delayedCall(3,Next);
            else
            {
                UI.btnBook.interactable = false;
                StartCoroutine(book.AutoOpen());
            }
            int i = 0;
            prints.ToObservable().Subscribe(p => {
                LeanTween.delayedCall(0.3f * (i++), () => p.FLy());
            });
            LeanTween.dispatchEvent(G5Event.SOUND, "tap");
            mouse.AnimationState.SetAnimation(0, "move", false);
        }
        private void End()
        {
            brigeToResult._score = Mathf.CeilToInt(_score);
            brigeToResult.ShowPopup();
        }
        public void Next()
        {
            UI.Begin();
            _sttPrint = 0;
            clearContent();
            UI.Drags.ToObservable().Subscribe(x => {
                x.GetComponent<Drag>().close();
                x.GetComponent<Drag>().isPrint = false;
            });
            prints.ToObservable().Subscribe(p => p.isFly = false);
            G5Event.IsLock = false;
            SetContent();
            StartCoroutine(GlowNoTapDrop());
        }
        public void SetContent()
        {
            string[] contentDrags = new string[contents[_count].contentByTime.Length];
            for(int i = 0; i < contentDrags.Length; i++)
            {
                contentDrags[i] = contents[_count].contentByTime[i].word;
            }
            UI.SetContentDrag(contentDrags);
            UI.SetContent(contents[_count].sentences);
            UI.SetPage(contents[_count].page);
            if (contents[_count].contentByTime[0].word.Length <= 6)
            {
                UI.Drop.GetComponent<Drop>().txt.text = "  "+contents[_count].contentByTime[0].word+"  ";
            } else UI.Drop.GetComponent<Drop>().txt.text = contents[_count].contentByTime[0].word;
            UI.Drop.GetComponent<Drop>().Change();
            UI.Drop.transform.localPosition = contents[_count].pos;
            var color = UI.Drop.GetComponent<Drop>().txt.color;
            UI.Drop.GetComponent<Drop>().txt.color = new Color(color.r, color.g, color.b, 0);
        }
    }
}
