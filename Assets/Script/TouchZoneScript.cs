using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchZoneScript : MonoBehaviour
{

    public List<Sprite> CodeSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
           // Debug.Log("EnemyIn");
            EnemyScript es = col.gameObject.GetComponent<EnemyScript>();
            if (es != null)
            {
                es.SetSprite(CodeSprite[es.InputCode]);
                GameManager.GameMainManager.NewEnemyInZone(es);
            }
        }
    }

}
