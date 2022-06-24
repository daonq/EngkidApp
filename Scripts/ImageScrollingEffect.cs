using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScrollingEffect : MonoBehaviour
{
    Image image;
    Material mat;
    float val = 0.0f;
    [SerializeField] float m_ScrollSpeed = 0.5f;

    private void Start()
    {
        image = this.GetComponent<Image>();
        mat = image.material;
    }

    private void Update()
    {
        val += Time.deltaTime * m_ScrollSpeed;
        if (val >= 1.0f)
            val = 0.0f;
        mat.SetTextureOffset("_MainTex", new Vector2(val, 0));
    }
}
