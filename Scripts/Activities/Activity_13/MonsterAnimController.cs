using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimController : MonoBehaviour
{
    [Header("Anim Sprites")]
    public Sprite m_NormalSprite;
    public Sprite m_GotHitSprite;
    public Sprite m_DefenedSprite;
    public Sprite m_DeathSprite;

    public void GetHit(bool is_taken_damage = true)
    {
        if (is_taken_damage)
            this.GetComponent<SpriteRenderer>().sprite = m_GotHitSprite;
        else
            this.GetComponent<SpriteRenderer>().sprite = m_DefenedSprite;
        StartCoroutine(DelayedResetSprite());
    }

    IEnumerator DelayedResetSprite()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<SpriteRenderer>().sprite = m_NormalSprite;
    }

    public void Death()
    {
        this.GetComponent<SpriteRenderer>().sprite = m_DeathSprite;
    }
}
