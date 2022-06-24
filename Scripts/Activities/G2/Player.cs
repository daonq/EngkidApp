using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G2
{
    public class Player: MonoBehaviour
    {
        public string[] status;
        public AudioClip[] clips;
        public AudioSource _audio;
        private SkeletonGraphic _bone;
        public SkeletonGraphic gold;
        private SkeletonGraphic _stone;
        private Vector3 _oldPos;
        void Awake()
        {
            _bone = GetComponent<SkeletonGraphic>();
            _oldPos = transform.localPosition;
            _stone = transform.GetChild(0).GetComponent<SkeletonGraphic>();
        }
        public void Begin()
        {
            transform.localPosition = _oldPos;
        }
        public IEnumerator OnPlayer(int stt,bool isLoop)
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = clips[stt];
                _audio.Play();
                _audio.loop = isLoop;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        public IEnumerator Jump()
        {
            yield return _bone.AnimationState.SetAnimation(0, status[5], false);
            LeanTween.dispatchEvent(Event.SOUND,"jump");
            yield return new WaitForSeconds(0.5f);
            yield return _bone.AnimationState.SetAnimation(0,status[4],true);
        }
        public IEnumerator Happy()
        {
            yield return _bone.AnimationState.SetAnimation(0, status[3], false);
            yield return new WaitForSeconds(0.5f);
            yield return _bone.AnimationState.SetAnimation(0, status[4], true);
        }
        public IEnumerator goWalk(float time,bool isNext)
        {
            clear();
            yield return new WaitForSeconds(0.1f);
            yield return _bone.AnimationState.SetAnimation(0, status[isNext ? 8 : 9], true);
            yield return StartCoroutine(OnPlayer(0, true));
            yield return new WaitForSeconds(time);
            _audio.loop = false;
            yield return _bone.AnimationState.SetAnimation(0, status[4], true);
        }
        public IEnumerator Oops()
        {
            clear();
            yield return new WaitForSeconds(0.1f);
            yield return _bone.AnimationState.SetAnimation(0, status[6], false);
            yield return new WaitForSeconds(1);
            yield return _bone.AnimationState.SetAnimation(0, status[0], false);
            yield return StartCoroutine(OnPlayer(2, false));
            _stone.gameObject.SetActive(true);
            yield return _stone.AnimationState.SetAnimation(0, "stone", false);
            yield return new WaitForSeconds(1f);
            _audio.loop = false;
            _stone.gameObject.SetActive(false);
            yield return _bone.AnimationState.SetAnimation(0,status[1],false);
            yield return new WaitForSeconds(1f);
            yield return _bone.AnimationState.SetAnimation(0, status[2], false);
            yield return new WaitForSeconds(1f);
            yield return _bone.AnimationState.SetAnimation(0, status[4], true);
        }
        public IEnumerator talk(float time)
        {
            clear();
            yield return new WaitForSeconds(0.1f);
            yield return _bone.AnimationState.SetAnimation(0,status[7],true);
            Debug.Log("talk:" + time);
            yield return new WaitForSeconds(time);
            _audio.loop = false;
            yield return _bone.AnimationState.SetAnimation(0,status[4],true);
        }
        public IEnumerator talkBegin()
        {
            clear();
            _bone.AnimationState.SetAnimation(0, status[7], true);
            yield return new WaitForSeconds(1f);
            _bone.AnimationState.SetAnimation(0, status[7], true);
            yield return new WaitForSeconds(1);
            _bone.AnimationState.SetAnimation(0, status[7], true);
            yield return new WaitForSeconds(1.5f);
            _audio.loop = false;
            yield return _bone.AnimationState.SetAnimation(0, status[4], true);
        }
        public IEnumerator Win()
        {
            clear();
            yield return new WaitForSeconds(0.1f);
            yield return gold.AnimationState.SetAnimation(0, "open", false);
            yield return StartCoroutine(OnPlayer(1, false));
            yield return new WaitForSeconds(1.5f);
            _audio.loop = false;
            LeanTween.dispatchEvent(Event.SOUND, "jumpWin");
            yield return gold.AnimationState.SetAnimation(0, "idle_open", true);
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(JumpWin());
        }
        public IEnumerator JumpWin()
        {
            clear();
            yield return new WaitForSeconds(0.1f);
            LeanTween.dispatchEvent(Event.SOUND, "win");
            yield return _bone.AnimationState.SetAnimation(0, status[3], true);
            yield return OnPlayer(3, false);
            yield return new WaitForSeconds(2);
            _audio.loop = false;
        }
        public void clear()
        {
            _bone.AnimationState.ClearTracks();
            _bone.Skeleton.SetToSetupPose();
            _audio.loop = false;
        }
    }
}
