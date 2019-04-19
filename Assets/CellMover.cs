using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMover : MonoBehaviour
{
    public Vector2 desiredPosition;
    Vector2 myPos;
    float maxSpeed = 30f;
    public ParticleSystem CM;
    bool Moving = false;

    void Start()
    {

    }


    void Update()
    {
        if(myPos == desiredPosition)
        {
            Moving = false;
        }
        else
        {
            Moving = true;
        }
        if (Moving)
        {
            CM.Play();
        }
        myPos = transform.position;
        transform.position = Vector2.MoveTowards(myPos, desiredPosition, maxSpeed * Time.deltaTime);

    }
}