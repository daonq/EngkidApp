using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KidAvatarChoosingSelector : MonoBehaviour
{
    public Text m_LabelText;
    public KidSelectBehavior m_SelectBehavior;

    public ListDuel listDuel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("ScrollingElement"))
        {
            KidChoosingAvatarBehavior avatarBehavior = other.GetComponent<KidChoosingAvatarBehavior>();
            //Debug.Log(avatarBehavior.m_KidName);
            if (avatarBehavior != null)
            {
                m_LabelText.text = avatarBehavior.m_KidName;
                m_SelectBehavior.OnSetChosenKid(avatarBehavior.m_KidAccountID);
            }
            else
            {
                ItemDuel duelItem = other.GetComponent<ItemDuel>();
                if (duelItem != null)
                {
                    m_LabelText.text = duelItem.GetDuel().name;
                    listDuel.setCurrentDuel(duelItem.GetDuel());
                }

            }
        }
    }
}
