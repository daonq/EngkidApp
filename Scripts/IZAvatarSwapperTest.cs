using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IZAvatarSwapperTest : MonoBehaviour
{
    public IZAvatarController controller;
    public List<Animator> avatarsList = new List<Animator>();
    public bool swapTrigger = false;
    public int avatarIndex = 0;

    private void Update()
    {
        if (swapTrigger == true)
        {
            if (avatarIndex < avatarsList.Count - 1)
                avatarIndex++;
            else
                avatarIndex = 0;

            swapTrigger = false;
            controller.SwapAvatar(avatarsList[avatarIndex]);
        }
    }
}
