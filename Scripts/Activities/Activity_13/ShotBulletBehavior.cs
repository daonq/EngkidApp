using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBulletBehavior : MonoBehaviour
{
    [Header("Hit Vfx")]
    public GameObject m_HitVfxPrefab;

    //internal
    int bulletIndex = -1;

    public void ShotBulletInit(Vector3 start_point, Vector3 end_point, Vector3 control_point, float flight_duration, int bullet_index)
    {
        bulletIndex = bullet_index;
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { start_point,
                                                               control_point,
                                                               control_point,
                                                               end_point });
        LeanTween.move(this.gameObject, ltPath, flight_duration).setOnComplete(() => {
            OnDeath();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: destroy when hit monster
        MonsterBehavior monster = collision.gameObject.GetComponent<MonsterBehavior>();
        if (monster != null)
        {
            bool flag = monster.OnGetHit(bulletIndex);
            if (flag == true)
            {
                GameObject vfx = Instantiate(m_HitVfxPrefab, this.transform.position, this.transform.rotation);
                Vector3 og_scale = vfx.transform.localScale;
                vfx.transform.localScale *= 0.5f;
                LeanTween.scale(vfx, og_scale, 0.075f);
                vfx.AddComponent<EngKidAPI.SelfDestroy>().InitSelfDestroy(0.1f);
                LeanTween.cancel(this.gameObject);
                OnDeath();
            }
        }
    }

    private void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
