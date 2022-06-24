using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentScalerTrigger : MonoBehaviour
{
    public bool grayOutOfFocus = true;
    public ScrollViewContentScaler scrollViewContentScaler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            scrollViewContentScaler.OnScrollViewElementMoveToFocus(other.transform.gameObject);
            ColorBlock cb = other.GetComponent<Button>().colors;
            cb.normalColor = Color.white;
            other.GetComponent<Button>().colors = cb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            scrollViewContentScaler.OnScrollViewElementMoveOutOfFocus(other.transform.gameObject);
            ColorBlock cb = other.GetComponent<Button>().colors;
            if (grayOutOfFocus == true)
                cb.normalColor = Color.gray;
            else
                cb.normalColor = Color.white;
            other.GetComponent<Button>().colors = cb;
        }
    }
}
