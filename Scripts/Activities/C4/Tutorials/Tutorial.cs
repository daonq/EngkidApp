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
    public class Tutorial : MonoBehaviour
    {
        public Hand hand;
        public Window window;
        public Transform[] Points;
        public SoundManager soundManager;
        public Contents[] contents;
        public Text txt;
        public SkeletonGraphic book;
        public Image txtcontent;
        public GameObject closeBook;
        public Image picDrag;
        public Box box;
        public bool isBegin = true;
        public IEnumerator Begin()
        {
            hand.setBegin();
            isBegin = true;
            txtcontent.gameObject.SetActive(false);
            closeBook.SetActive(false);
            //float time01 =  soundManager.PlayByTime(0);
            txt.text = "The mouse jumps on my sister’s skirt.";
            book.gameObject.SetActive(true);
            hand.Drag();
            book.AnimationState.SetAnimation(0, "idle", false);
            yield return new WaitForSeconds(1);
            hand.Tap();
            book.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            hand.Drag();
            book.transform.GetChild(0).gameObject.SetActive(false);
            book.AnimationState.SetAnimation(0, "open book", false);
            LeanTween.moveLocal(hand.gameObject, Points[3].localPosition, 1);
            yield return new WaitForSeconds(1);
            txtcontent.gameObject.SetActive(true);
            closeBook.SetActive(true);
            yield return new WaitForSeconds(1);
            hand.Drag();
            LeanTween.moveLocal(hand.gameObject, Points[4].localPosition, 1);
            yield return new WaitForSeconds(1);
            hand.Tap();
            txtcontent.gameObject.SetActive(false);
            closeBook.SetActive(false);
            book.AnimationState.SetAnimation(0, "close_book", false);
            //float time03 = soundManager.PlayByTime(2);
            //yield return new WaitForSeconds(time03);


            LeanTween.moveLocal(hand.gameObject,Points[0].localPosition,1);
            //yield return new WaitForSeconds(time01);
            window.Next();
            window.setGlowBtn(false);
            hand.Tap();
            txt.text = contents[1].sentences;
            yield return new WaitForSeconds(1);
            hand.Drag();
            LeanTween.moveLocal(hand.gameObject,Points[1].localPosition,1);
            yield return new WaitForSeconds(1);
            txt.text = "The mouse jumps on my sister’s skirt.";
            window.Back();
            window.setGlowBtn(true);
            hand.Tap();
            txt.text = "The mouse jumps on my dad’s head.";
            //float time02 = soundManager.PlayByTime(1);
            //yield return new WaitForSeconds(time02);
            window.closeGlowBtn();
            hand.Drag();
            LeanTween.moveLocal(hand.gameObject, Points[2].localPosition, 1);
            
            hand.Drag();
            LeanTween.moveLocal(hand.gameObject, Points[5].localPosition, 1);
            yield return new WaitForSeconds(1);
            picDrag.transform.localPosition = new Vector3(-471, -199, 0);
            picDrag.transform.localScale = 0.75f*Vector3.one;
            window.Alpha();
            window.Setglow();
            hand.Drag();
            box.glow();
            LeanTween.moveLocal(picDrag.gameObject, new Vector3(-317, 230, 0), 1);
            LeanTween.moveLocal(hand.gameObject,Points[6].localPosition,1);
            yield return new WaitForSeconds(1);
            hand.Tap();
            picDrag.transform.localScale = Vector3.zero;
            box.Content();
            hand.setBegin();
            window.closeGlow();
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(close());
        }
        public IEnumerator close()
        {
            StopCoroutine(Begin());
            yield return new WaitForSeconds(0.3f);
            LeanTween.cancelAll();
            StopAllCoroutines();
            book.AnimationState.SetAnimation(0, "idle", false);
            window.closeAlpha();
            gameObject.SetActive(false);
            box.Clear();
            if (isBegin)
            {
                LeanTween.dispatchEvent(Event.ACTION, Event.BEGIN);
                isBegin = false;
            }
            LeanTween.dispatchEvent(Event.SOUND, "tap");
        }
    }
}
