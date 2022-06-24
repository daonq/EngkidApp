using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCoupling : MonoBehaviour
{
    public GameObject partner;
    public bool isReversed = false;
    public bool useUpdate = false;


    private void OnEnable()
    {
        if (isReversed == false)
            partner.SetActive(true);
        else
            partner.SetActive(false);
    }

    private void OnDisable()
    {
        if (isReversed == false)
            partner.SetActive(false);
        else
            partner.SetActive(true);
    }

    private void Update()
    {
        if (useUpdate == true)
        {
            if (isReversed == false)
                partner.SetActive(this.gameObject.activeSelf);
            else
                partner.SetActive(!this.gameObject.activeSelf);
        }
    }
}
