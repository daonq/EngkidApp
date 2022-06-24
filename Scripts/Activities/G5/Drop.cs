using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace G5
{
    public class Drop: MonoBehaviour
    {
        public Text txt;
        public Image glow;
        public void Change()
        {
            StartCoroutine(Resize());
        }
        public IEnumerator Resize()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            float with = txt.GetComponent<RectTransform>().rect.width;
            GetComponent<RectTransform>().sizeDelta = new Vector2(1.2f * with, GetComponent<RectTransform>().rect.height);
            glow.GetComponent<RectTransform>().sizeDelta = new Vector2(1.2f * with, glow.GetComponent<RectTransform>().rect.height);
        }
    }
}
