using common;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using UniRx;
namespace V2
{
    public class Review : MonoBehaviour
    {
        public SkeletonGraphic explosive;
        public GameObject king;
        public GameObject fish;
        private Vector3 _oldPos;
        private GameObject[] _allFish;
        private Vector3[] _allFishPos;
        private Vector3 _posking;
        private void Awake()
        {
            _oldPos = fish.transform.localPosition;
            _allFish = new GameObject[fish.transform.childCount];
            _allFishPos = new Vector3[fish.transform.childCount];
            for(int i = 0; i < fish.transform.childCount; i++)
            {
                _allFish[i] = fish.transform.GetChild(i).gameObject;
                _allFishPos[i] = fish.transform.GetChild(i).transform.localPosition;
            }
            _posking = king.transform.localPosition;
           // OnExplosive();
        }
        void Start()
        {
            king.SetActive(false);
            fish.SetActive(false);
            king.transform.localPosition = new Vector3(0, -1288, 0);
            int i = 0;
            _allFish.ToObservable().Subscribe(x => {
                x.transform.localPosition = new Vector3(i >= 6 ? 3000 : 300, x.transform.localPosition.y, 0);
                i++;
            });
        }
        public void OnExplosive()
        {
            explosive.AnimationState.SetAnimation(0, "explosive", false);
            LeanTween.delayedCall(2, WinGame);
        }
        public void WinGame()
        {
            king.SetActive(true);
            LeanTween.moveLocalY(king, _posking.y, 1);
            fish.SetActive(true);
            fish.transform.localPosition = _oldPos;
            int i = 0;
            _allFish.ToObservable().Subscribe(x => {
                LeanTween.moveLocal(x, _allFishPos[i++], 2).setDelay(0.5f*Random.Range(1,8));
            });
        }
        public void close()
        {
            Start();
        }
    }
}
