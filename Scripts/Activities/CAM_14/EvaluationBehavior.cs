using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationBehavior : MonoBehaviour
{
    RectTransform blackBG;

    // Start is called before the first frame update
    void Start()
    {
        blackBG = this.GetComponent<RectTransform>();
        LeanTween.alpha(blackBG.GetComponent<RectTransform>(), 0.0f, 0.0f);

        foreach (Transform child in this.transform)
        {
            child.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideEvaluationWindow()
    {
        foreach (Transform child in this.transform)
        {
            LeanTween.scale(child.gameObject, Vector3.zero, 0.25f);
        }

        LeanTween.alpha(blackBG.GetComponent<RectTransform>(), 0.0f, 0.25f);
    }

    public void ShowEvaluationWindow()
    {
        LeanTween.alpha(blackBG.GetComponent<RectTransform>(), 0.5f, 0.25f).setOnComplete(() => {
            foreach (Transform child in this.transform)
            {
                LeanTween.scale(child.gameObject, Vector3.one, 0.25f);
            }

            LeanTween.alpha(this.transform.GetChild(0).GetComponent<RectTransform>(), 1.0f, 0.0f);
            LeanTween.alpha(this.transform.GetChild(1).GetComponent<RectTransform>(), 1.0f, 0.0f);

            LeanTween.alpha(this.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>(), 0.0f, 0.0f);
            LeanTween.alpha(this.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>(), 0.0f, 0.0f);
            LeanTween.alpha(this.transform.GetChild(0).GetChild(2).GetComponent<RectTransform>(), 1.0f, 0.25f);
        });
    }
}
