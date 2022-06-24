using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBulletBehavior : MonoBehaviour
{
    ActivityWordShooter activityManager = null;
    int bulletIndex = -1;

    public void OnInit(ActivityWordShooter manager, int index)
    {
        activityManager = manager;
        bulletIndex = index;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanonBehavior canonBehavior = collision.gameObject.GetComponent<CanonBehavior>();
        if (canonBehavior != null) //bullet reached canon
        {
            //tell manager
            if (activityManager != null)
                activityManager.OnChamberingBullet(bulletIndex);

            //TODO: tell canon
            canonBehavior.OnChamberingBullet(bulletIndex);
        }
    }
}
