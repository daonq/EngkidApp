using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using BookCurlPro;

namespace Stories
{
    public class HelpTap : MonoBehaviour
    {
        public SkeletonGraphic[] bones;
        public Transform[] Pos;
        public TapStoryBehavior[] _tapStory;
        private Vector3[] _oldPos;
        public bool isStart = false;
        void Awake()
        {
            _oldPos = new Vector3[2];
            _oldPos[0] = bones[0].transform.localPosition;
            _oldPos[1] = bones[1].transform.localPosition;
            if (_tapStory.Length == 0)
            {
                _tapStory = transform.parent.GetComponentsInChildren<TapStoryBehavior>();
            }
            int i = 0;
            _tapStory.ToObservable().Subscribe(x => {
                x.id = i++; 
            });
            bones[0].transform.localScale = 0.7f * Vector3.one;
            bones[1].transform.localScale = 0.7f * Vector3.one;
        }
        private void OnEnable()
        {
            if (isStart) Begin();
        }
        public IEnumerator Play()
        {
            for (int i = 0; i < Pos.Length; i++)
            {
                yield return new WaitForSeconds(i);
                yield return bones[0].AnimationState.SetAnimation(0, "idle", false);
                yield return bones[1].AnimationState.SetAnimation(0, "idle", false);
                StoryEvent.IdHelp = i;
                LeanTween.moveLocal(bones[0].gameObject, Pos[i].localPosition, 1.5f).setOnComplete(() =>
                {
                    bones[0].AnimationState.SetAnimation(0, "drag", false);
                });
                LeanTween.moveLocal(bones[1].gameObject, Pos[i].localPosition, 1.5f).setOnComplete(() =>
                {
                    bones[1].AnimationState.SetAnimation(0, "drag", false);
                });
                yield return new WaitForSeconds(i);
                if(_tapStory.Length>0) _tapStory[i].Tap();
            }
            LeanTween.moveLocal(bones[0].gameObject,_oldPos[0],1);
            LeanTween.moveLocal(bones[1].gameObject, _oldPos[1], 1);
            yield return new WaitForSeconds(1);
        }
        public void Begin()
        {
            StoryEvent.IdHelp = 0;
            StartCoroutine(onTap(0));
        }
        public IEnumerator onTap(int i)
        {
            StoryEvent.isHelp = true;
            yield return new WaitForSeconds(0.01f);
            _tapStory.ToObservable().Subscribe(x => x.gameObject.SetActive(false));
            _tapStory[i].gameObject.SetActive(true);
            bones[0].AnimationState.SetAnimation(0, "idle", false);
            bones[1].AnimationState.SetAnimation(0, "idle", false);
            bones[0].transform.localScale = 0.7f* Vector3.one;
            LeanTween.moveLocal(bones[0].gameObject, Pos[i].localPosition,1).setOnComplete(() =>
            {
                bones[0].AnimationState.SetAnimation(0, "tap", false);
                if (_tapStory.Length > 0) _tapStory[i].Glow(true);
                LeanTween.delayedCall(1, () => {
                    bones[0].transform.localScale = Vector3.zero;
                });
            });
            bones[1].transform.localScale = 0.7f * Vector3.one;
            LeanTween.moveLocal(bones[1].gameObject, Pos[i].localPosition,1).setOnComplete(() =>
            {
                bones[1].AnimationState.SetAnimation(0, "tap", false);
                LeanTween.delayedCall(1, () => {
                    bones[1].transform.localScale = Vector3.zero;
                });
            });
        }
        public void CloseTutorial()
        {
            _tapStory.ToObservable().Subscribe(x => x.gameObject.SetActive(true));
        }
        public void OpenTutorial()
        {
            _tapStory.ToObservable().Subscribe(x => x.gameObject.SetActive(false));
        }
        public IEnumerator Check()
        {
            if (StoryEvent.IdHelp == Pos.Length - 1)
            {
                StoryEvent.isHelp = false;
                LeanTween.moveLocal(bones[0].gameObject, _oldPos[0], 0.5f);
                LeanTween.moveLocal(bones[1].gameObject, _oldPos[1], 0.5f);
                yield return new WaitForSeconds(0.5f);
                LeanTween.dispatchEvent(StoryEvent.Tutorial, StoryEvent.TUTORIAL);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                LeanTween.dispatchEvent(StoryEvent.Help, _tapStory[StoryEvent.IdHelp]);
            }
        }
        public void Stop()
        {
            StopAllCoroutines();
            StartCoroutine(Check());
        }
        public void Next(int j)
        {
            StartCoroutine(onTap(j));
        }
    }
}
