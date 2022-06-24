using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine;
using Spine.Unity;
namespace V1
{
    public class ActivityV1 : MonoBehaviour
    {
        public WordManager[] wordmanagers;
        public WordDrag[] draglist;
        public SoundManager soundManager;
        public RectTransform hand;
        public RectTransform handDrop;
        public Vector3[] handPos = new Vector3[3] { new Vector3(-81,-515,0), new Vector3(-342, -596,0), new Vector3(-854, -1260, 0) };
        public Clock clock;
        public GameObject m_Lesson;
        public GameObject resultPopup;
        private WordManager[] _wordmanagersFinish;
        private WordDrag[] _draglistFinish;
        private int _count=-1;
        private int _score;
        private Vector3[] _oldPos;
        private bool isHelp = false;
        public string[] statusHands = new string[2] { "tab", "hand 2" };
        void Awake()
        {
            _oldPos = new Vector3[draglist.Length];
            int i = 0;
            draglist.ToObservable().Subscribe(xx => { 
                xx.transform.localScale = Vector3.zero;
                _oldPos[i++] = xx.transform.localPosition;
            });
            LeanTween.addListener(gameObject,WordEvent.ACTION,OnEvent);
           LeanTween.addListener(gameObject,WordEvent.SOUND,(e) => {
               Debug.Log(e.data as string);
               soundManager.Play(soundManager.dicClips[e.data as string]);
           });
        }
        void Start()
        {
            fixRatio();
            BGMManagerBehavior.GetInstance().PauseBGM();
            int j = 0;
            draglist.ToObservable().Subscribe(xx => {
                LeanTween.delayedCall(0.5f * j++, () => {
                    LeanTween.scale(xx.gameObject, Vector3.one, 0.3f);
                    soundManager.Play(0);
                    xx.isLock = true;
                    wordmanagers[xx.id].isLock = true;
                });
            });
            LeanTween.delayedCall(0.5f * draglist.Length, () => clock.run());
            LeanTween.delayedCall(3+ 0.5f * draglist.Length,Tutorial);
        }
        public void callHand(Vector3 pos)
        {
            hand.localPosition = pos;
            hand.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, statusHands[0], false);
            hand.SetAsLastSibling();
            isHelp = false;
            LeanTween.delayedCall(1, () => {
                hand.localPosition = new Vector3(900000, 0, 0);
            });
        }
        void Tutorial()
        {
            
            draglist[0].isLock = false;
            draglist[0].isTutorial = true;
            draglist[0].Glow();
            wordmanagers[0].isTutorial = true;
            //wordmanagers.ToObservable().Subscribe(xx => xx.Clear());
            Debug.Log("tutorial");
            /* callHand(handPos[0]);
            LeanTween.delayedCall(1,tutorialDrag);*/
            tutorialDrag();
        }
        
        public void tutorialDrag()
        {
            if (!isHelp)
            {
                handDrop.localPosition = handPos[1];
                handDrop.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, statusHands[1], false);
                //draglist[0].CloseGlow();
                wordmanagers[0].Glow(1);
                LeanTween.delayedCall(1, () => {
                   // callHand(handPos[2]);
                    handDrop.localPosition = new Vector3(900000, 0, 0);
                });
                isHelp = true;
            }
        }
        void OnEvent(LTEvent e)
        {
            switch (e.data as string)
            {
                case WordEvent.TUTORIAL:
                    //tutorialDrag();
                    break;
                case WordEvent.TUTORIALDROP:                    
                    draglist.Where(d => d!=draglist[0]).ToObservable().Subscribe(x => x.isLock = false);
                    wordmanagers.Where(w=>w!=wordmanagers[0]).ToObservable().Subscribe(xx => xx.isLock = false);
                    wordmanagers[0].isLock = true;
                    wordmanagers[0].isTutorial = false;
                    draglist[0].isTutorial = false;
                    hand.localPosition = new Vector3(900000, 0, 0);
                    isHelp = false;
                    break;
                case WordEvent.TUTORIALTAP:
                    isHelp = false;
                    Tutorial();
                    break;
                case WordEvent.TUTORIALDTAPNEAR:
                    break;
                case WordEvent.TIME_END:
                    TutorialTimeOut();
                    break;
                case WordEvent.READSUBDONE:
                   draglist.Where(x=>x.transform.lossyScale!=Vector3.zero).ToArray().ToObservable().Subscribe(x => x.isLock = false);
                    if(_draglistFinish != null && _count< _draglistFinish.Length) EndGlow(_count);
                    break;
                case WordEvent.BONUS:
                    _score++;
                    draglist.Where(x => x.transform.lossyScale != Vector3.zero).ToArray().ToObservable().Subscribe(x => x.isLock = true);
                    if (_score >= wordmanagers.Length)
                    {
                        clock.GetComponent<Clock>().stop();
                        LeanTween.delayedCall(5,ShowPopup);
                    }
                    break;
                case WordEvent.TUTORIALWHENEND:
                    if (_count != -1)
                    {
                        _count++;
                        if (_count >= _draglistFinish.Length)
                        {
                            clock.GetComponent<Clock>().stop();
                            LeanTween.delayedCall(5,ShowPopup);
                        }
                    }
                    break;
                case WordEvent.CHANGEPOS:
                    Transform[] tranDraglist = draglist.Where(x=>!x.isDrag).Select(y => y.GetComponent<Transform>()).ToArray();
                    soundManager.GetComponent<DragPos>().ChangePos(tranDraglist);
                    break;
                default: 
                    break;
            }
        }
        void fixRatio()
        {
            float ratio = (float) Screen.width / (float) Screen.height;
            CanvasScaler canvasScaler = transform.GetChild(0).GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = ratio > 1.61f ? 0.5f: 0;
        }
        void TutorialTimeOut()
        {
            _count = 0;
            if (draglist[0].isTutorial)
            {
                _wordmanagersFinish = wordmanagers;
                _draglistFinish = draglist;
                draglist[0].isLock = false;
                draglist[0].isDrag = false;
                draglist[0].CloseGlow();
                draglist.ToObservable().Subscribe(x => x.isLock = false);
            }
            else
            {
                _wordmanagersFinish = wordmanagers.Where(W =>!W.isLock).ToArray();
                _draglistFinish = draglist.Where(d =>!d.isDrag).ToArray();
            }
            if(_wordmanagersFinish.Length>0) LeanTween.delayedCall(3,()=>EndGlow(_count));
        }
        void EndGlow(int count)
        {
            _wordmanagersFinish[count].Glow(1);
            _draglistFinish[count].GlowEnd(true);
            _draglistFinish[count].isLock = false;
            _draglistFinish[count].isTutorialEnd=true;
            _draglistFinish[count].isDrag = false;
            _draglistFinish.Where(_d => _d != _draglistFinish[count]).ToObservable().Subscribe(other => other.isLock = true);
        }
        int Bonus(int score)
        {
            int countStar = 0;
            float percent = (float) score / (float) wordmanagers.Length;
            if (percent>0 && percent< 0.2) countStar = 1;
            if (percent >= 0.2 && percent < 0.8) countStar = 2;
            if (percent >= 0.8) countStar = 3;
            return countStar;
        }
        void ShowPopup()
        {
            resultPopup.SetActive(true);
            resultPopup.transform.GetChild(1).transform.localScale = Vector3.zero;
            clock.stop();
            int starBonus = Bonus(_score);
            LeanTween.scale(resultPopup.transform.GetChild(1).gameObject, Vector3.one, 0.5f).setOnComplete(() => {
                resultPopup.GetComponent<ResultPopupBehavior>().m_XPText.text = "";
                resultPopup.GetComponent<ResultPopupBehavior>().m_XPText.text = "";
                resultPopup.GetComponent<ResultPopupBehavior>().OnTriggerResultPopUp(starBonus);
            });
        }
        public void OnReturnToActivityScreen()
        {
            soundManager.Play(4);
            LeanTween.delayedCall(0.3f,() => {
                //LeanTween.cancelAll();
                SceneManagerBehavior.GetInstance().BackToLastScene(this.gameObject, true);
            });
        }
        private void OnEnable()
        {
            //Cursor.visible = false;
            if (BGMManagerBehavior.GetInstance())
                BGMManagerBehavior.GetInstance().PauseBGM();
        }

        private void OnDisable()
        {
            //Cursor.visible = true;
            if (BGMManagerBehavior.GetInstance())
                BGMManagerBehavior.GetInstance().ResumeBGM();
        }
        public void PlayAgain()
        {
            _count = -1;
            _score = 0;
            soundManager.Play(4);
            draglist[0].Close();
            _draglistFinish = null;
            LeanTween.delayedCall(0.2f, () => {
                resultPopup.SetActive(false);
                int i = 0;
                draglist.ToObservable().Subscribe(xx => {
                    xx.transform.localScale = Vector3.zero;
                    xx.isLock = false;
                    xx.isTutorialEnd = false;
                    xx.isTutorial = false;
                    xx.isDrag = false;
                    xx.transform.localPosition= _oldPos[i++];
                    xx.GlowEnd(false);
                    wordmanagers[xx.id].isLock = false;
                });
                wordmanagers.ToObservable().Subscribe(xx => xx.clearForBegin());
                clock.Reset();
                Start();
            });
        }
    }
}
