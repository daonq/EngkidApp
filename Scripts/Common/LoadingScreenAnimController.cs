using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenAnimController : MonoBehaviour
{
    public Spine.Unity.SkeletonGraphic graphic;
    Image parentImg;

    // Start is called before the first frame update
    void Start()
    {
        parentImg = this.transform.parent.GetComponent<Image>();
        graphic.color = parentImg.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (parentImg == null)
            parentImg = this.transform.parent.GetComponent<Image>();
        graphic.color = parentImg.color;
    }
}
