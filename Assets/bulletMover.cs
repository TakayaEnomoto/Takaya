using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletMover : MonoBehaviour
{
    public float spd;
    private bool LtR = false;
    private bool UtD = false;

    private int colorpick;
    private SpriteRenderer sr;
    private TrailRenderer tr;
    public Material trialcyan;
    public Material trialred;

    public float counter;
    private bool superstar = false;

    void Start()
    {
        colorpick = Random.Range(1, 100);
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        counter = 5;
    }


    void Update()
    {
        sr.color = Color.red;
        tr.material = trialred;
        
           
        if (transform.position.x <= -13)
        {
            LtR = true;
        }

        if(transform.position.y >= 12)
        {
            UtD = true;
        }


        if (LtR)
            transform.position = new Vector3(transform.position.x + spd, transform.position.y, transform.position.z);
        if (UtD)
            transform.position = new Vector3(transform.position.x, transform.position.y - spd, transform.position.z);

        if (transform.position.x >= 17)
        {
            Destroy(this);
        }
        if(transform.position.y <= -6.5f)
        {
            Destroy(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" )
        {
            GridManager.me.movement = 0;
        }
    }
}
