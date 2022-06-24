using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace G3
{
    public class SceneBySpine : MonoBehaviour
    {
        public string[] status;
        public AudioClip[] clips;
        public AudioClip[] Subclips;
        public AudioSource source;
        public float DelaySleep = 10;
        private int _scene;
        public IEnumerator PlayStatus(int i, bool isLoop, bool isLoopSound)
        {
            yield return new WaitForSeconds(0.01f);
            gameObject.SetActive(true);
            if (i < status.Length)
            {
                GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, status[i], isLoop);
            }
            yield return StartCoroutine(onPlayClip(i, isLoopSound));
        }
        public IEnumerator onPlayClip(int stt, bool isloop)
        {
            if (clips[stt] != null )
            {
                source.clip = clips[stt];
                source.Play();
            }
            yield return new WaitForSeconds(0.01f);
            source.loop = isloop;
        }
        public IEnumerator onPlaySuClip(int stt, bool isloop)
        {
            yield return new WaitForSeconds(0.01f);
            if (Subclips[stt] != null)
            {
                source.clip = Subclips[stt];
                source.Play();
            }
            source.loop = isloop;
        }
        public IEnumerator yawning()
        {
            StartCoroutine(PlayStatus(8, false, false));
            yield return new WaitForSeconds(2f);
            StartCoroutine(PlayStatus(1, true, false));
            yield return new WaitForSeconds(DelaySleep);
            StartCoroutine(yawning());
        }
        public IEnumerator yawningeating()
        {
            bool rd = Shuffle.randomBoolean();
            int stt = rd ? 3 : 4;
            StartCoroutine(PlayStatus(stt, false, false));
            yield return new WaitForSeconds(5f);
            StartCoroutine(PlayStatus(1, false, false));
            yield return new WaitForSeconds(DelaySleep);
            StartCoroutine(yawningeating());
        }
        public IEnumerator Begin(int scene)
        {
            _scene = scene;
            yield return new WaitForSeconds(0.5f);
            switch (scene)
            {
                case Event.WAKE_UP:
                    StartCoroutine(PlayStatus(0, true, true));
                    yield return new WaitForSeconds(3);
                    break;
                case Event.TOOTHBRUSH:
                    GetComponent<SkeletonGraphic>().timeScale = 1;
                    StartCoroutine(PlayStatus(1, true, true));
                    yield return new WaitForSeconds(3);
                    StartCoroutine(yawning());
                    break;
                case Event.TOWEL:
                    GetComponent<SkeletonGraphic>().timeScale = 1;
                    StartCoroutine(PlayStatus(1, true, true));
                    yield return new WaitForSeconds(3);
                    StartCoroutine(yawning());
                    break;
                case Event.CLOTHES:
                    StartCoroutine(PlayStatus(5, true, true));
                    yield return new WaitForSeconds(3);
                    StartCoroutine(gaiDauInClothes());
                    break;
                case Event.SCHOOL:
                    PlayStatus(6, true, false);
                    yield return new WaitForSeconds(3);
                    StartCoroutine(gaiDauInSchool());
                    yield return new WaitForSeconds(0.5f);
                    break;
                case Event.EATING:
                    yield return PlayStatus(1,false,false);
                    yield return new WaitForSeconds(1);
                    yield return yawningeating();
                    break;
                case Event.BUS:
                    StartCoroutine(PlayStatus(1,false,false));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        public void Close()
        {
            StopAllCoroutines();
        }
        public void OnReset()
        {
            GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();
            GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();
            //GetComponent<SkeletonGraphic>().Update(0f);
        }
        public IEnumerator gaiDauInClothes ()
        {
            StartCoroutine(PlayStatus(4, false,false));
            GetComponent<SkeletonGraphic>().timeScale = 1;
            yield return new WaitForSeconds(2);
            StartCoroutine(PlayStatus(5, false,false));
            GetComponent<SkeletonGraphic>().timeScale = 0.2f;
            yield return new WaitForSeconds(DelaySleep);
            StartCoroutine(gaiDauInClothes());
        }
        public IEnumerator gaiDauInSchool()
        {
            GetComponent<SkeletonGraphic>().timeScale = 1f;
            StartCoroutine(PlayStatus(10, false,false));
            yield return new WaitForSeconds(3);
            GetComponent<SkeletonGraphic>().timeScale = 0.2f;
            StartCoroutine(PlayStatus(7, false,false));
            yield return new WaitForSeconds(DelaySleep);
            StartCoroutine(gaiDauInSchool());
        }
        public IEnumerator End()
        {
            Close();
            yield return new WaitForSeconds(0.5f);
            switch (_scene)
            {
                case Event.WAKE_UP:
                    StartCoroutine(onPlaySuClip(0, false));
                    transform.GetChild(0).gameObject.SetActive(true);
                    yield return new WaitForSeconds(1);
                    transform.GetChild(0).gameObject.SetActive(false);
                    StartCoroutine(PlayStatus(1, false,false));
                    yield return new WaitForSeconds(1);
                    StartCoroutine(PlayStatus(2, true,true));
                    yield return new WaitForSeconds(1);
                    break;
                case Event.TOOTHBRUSH:
                    GetComponent<SkeletonGraphic>().timeScale = 1f;
                    StartCoroutine(PlayStatus(0, true,true));
                    yield return new WaitForSeconds(2f);
                    GetComponent<SkeletonGraphic>().timeScale = 0.5f;
                    StartCoroutine(PlayStatus(4, false,false));
                    yield return new WaitForSeconds(2f);
                    StartCoroutine(PlayStatus(2, true,false));
                    yield return new WaitForSeconds(1f);
                    break;
                case Event.TOWEL:
                    GetComponent<SkeletonGraphic>().timeScale = 1f;
                    StartCoroutine(PlayStatus(7, true, true));
                    yield return new WaitForSeconds(1.5f);
                    GetComponent<SkeletonGraphic>().timeScale = 0.5f;
                    OnReset();
                    StartCoroutine(PlayStatus(5, false, false));
                    //yield return new WaitForSeconds(1f);
                    OnReset();;
                    StartCoroutine(PlayStatus(2,false,false));
                    yield return new WaitForSeconds(1f);
                    break;
                case Event.CLOTHES:
                    StartCoroutine(onPlayClip(5, false));
                    GetComponent<SkeletonGraphic>().timeScale = 0.5f;
                    yield return new WaitForSeconds(1);
                    StartCoroutine(PlayStatus(3, false, false));
                    yield return new WaitForSeconds(1);
                    StartCoroutine(PlayStatus(6, true,false));
                    yield return new WaitForSeconds(1);
                    break;
                case Event.SCHOOL:
                    StartCoroutine(PlayStatus(9, false,false));
                    GetComponent<SkeletonGraphic>().timeScale = 0.5f;
                    yield return new WaitForSeconds(2);
                    StartCoroutine(PlayStatus(8, true,false));
                    break;
                case Event.EATING:
                    StopCoroutine(yawningeating());
                    yield return StartCoroutine(PlayStatus(0, true, false));
                    yield return new WaitForSeconds(1);
                    yield return StartCoroutine(PlayStatus(1, false, false));
                    yield return new WaitForSeconds(0.5f);
                    break;
                case Event.BUS:
                    StartCoroutine(PlayStatus(0,false,false));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(1f);
            Close();
            source.loop = false;
            LeanTween.dispatchEvent(Event.ACTION, Event.CHANGE);
        }
    }
}
