using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace CAM_9
{
    public class AutoBook : MonoBehaviour
    {
        public GameObject Front;
        private GameObject _currentPage;
        private GameObject _NewPage;
        public GameObject Back;
        public float time = 5;
        public Vector3 PosBack = new Vector3(340, 420, 0);
        public Vector3 PosFront = new Vector3(50, 1715, 0);
        public GameObject[] pages;
        private int stt = 0;
        private Vector3 _PosOld;
        private void Awake()
        {
            _PosOld = Front.transform.localPosition;
        }
        private void DeActive()
        {
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].SetActive(false);
            }
        }
        void Start()
        {
            Begin();
        }
        public void Begin()
        {
            DeActive();
            _currentPage = pages[stt];
            _currentPage.SetActive(true);
            _NewPage = pages[stt+1];
            _NewPage.SetActive(true);
            Front.SetActive(false);
            Front.transform.localPosition = _PosOld;
        }
        private void Change()
        {
            Front.SetActive(true);
            _currentPage.transform.SetParent(Back.transform);
            _currentPage.transform.SetAsFirstSibling();
            _currentPage.transform.localPosition = PosBack;
            LeanTween.moveLocal(Front,PosFront,time).setOnComplete(() =>
            {
                Front.SetActive(false);
            });
        }
        public void FirstPage()
        {
            Begin();
            Change();
            LeanTween.dispatchEvent(Cam9Event.SOUND, "paper");
        }
        private void NextPage(int n)
        {
            if (n > stt)
            {
                _currentPage.GetComponent<Page>().BackToBegin();
                stt = n;
                LeanTween.delayedCall(1, FirstPage);
            }
        }
        public void goWrite(int w,int id)
        {
            if (_NewPage.GetComponent<Page>() != null)
            {
                NextPage(id);
                LeanTween.delayedCall(3, () => {
                    _NewPage.GetComponent<Page>().Write(w);
                });
            }
        }
        public void Reset()
        {
            stt = 0;
            pages.ToObservable().Subscribe(x => {
                x.transform.SetParent(transform);
                x.GetComponent<Page>().Reset();
            });
            Back.transform.parent.SetAsLastSibling();
            pages[0].transform.SetAsLastSibling();
            Begin();
        }
    }
}
