using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            EnemyScript es = col.gameObject.GetComponent<EnemyScript>();
            if (es != null && es.EnemyState==EnemyScript.State.InZone)
            {
                es.EnemyState = EnemyScript.State.Follow;
                es.CodeSprite.enabled = false;
                GameManager.GameMainManager.EnemyTouchPlayer(es);
            }
        }
    }
}
