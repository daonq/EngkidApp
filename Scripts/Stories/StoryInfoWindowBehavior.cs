using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInfoWindowBehavior : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform.localScale = Vector3.zero;
        LeanTween.scale(this.gameObject, Vector3.one, 0.25f);
    }

    private void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
    }
}
