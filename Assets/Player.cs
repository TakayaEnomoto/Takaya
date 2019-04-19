using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public TextMesh moveCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2((GridManager.me.playerX * 2) - 2, (GridManager.me.playerY * 2) - 3);
        moveCount.text = ("" + GridManager.me.movement);
    }
}
