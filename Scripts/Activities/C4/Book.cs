using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;

namespace C4
{
    public class Book: MonoBehaviour
    {
        public SkeletonGraphic book;
        public GameObject contents;
        public GameObject closeBook;
        public GameObject cover;
        private GameObject _lock;
        private void Awake()
        {
            _lock = transform.GetChild(1).gameObject;
        }
        private void Start()
        {
            closeBook.SetActive(false);
            contents.SetActive(false);
            book.AnimationState.SetAnimation(0, "idle", false);
            if (cover != null) cover.SetActive(false);
        }
        public void Open()
        {
            book.AnimationState.SetAnimation(0,"open book", false);
            LeanTween.dispatchEvent(Event.SOUND, "tap");
            if (cover != null) cover.SetActive(false);
            LeanTween.delayedCall(1,() => {
                contents.SetActive(true);
                closeBook.SetActive(true);
                LeanTween.delayedCall(0.1f, () => {
                    if (cover != null) cover.SetActive(true);
                });
               // LeanTween.dispatchEvent(Event.SOUND, "head");
            });
            book.transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<Button>().interactable = false;
        }
        public void close()
        {
            contents.SetActive(false);
            closeBook.SetActive(false);
            book.AnimationState.SetAnimation(0,"close_book", false);
            _lock.gameObject.SetActive(true);
            if (cover != null) cover.SetActive(false);
            LeanTween.dispatchEvent(Event.SOUND,"tap");
            LeanTween.delayedCall(1, () => {
                book.AnimationState.SetAnimation(0,"idle", false);
                _lock.gameObject.SetActive(false);
                GetComponent<Button>().interactable = true;
            });
        }
        public void Glow()
        {
            book.transform.GetChild(0).gameObject.SetActive(true);
        }
        public void CloseGlow()
        {
            book.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
