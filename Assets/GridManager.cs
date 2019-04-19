using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager me;

    Cell[,] grid;
    int[,] copiedGrid;

    public bool End;

    [Header("Important Stuff")]
    int width = 5;
    int height = 7;
    public float cellLength;
    public Vector2 gridOffset;
    public GameObject cell;

    [Header("Before Play Stuff")]
    bool checkAgainX = false;
    bool checkAgainY = false;
    bool clearX = false;
    bool clearY = false;

    [Header("Score Shit")]
    TextMesh scoreText;
    public int score;
    public int combo;

    [Header("Colours")]
    public Color[] colors;

    [Header("Player Shit")]
    public int playerX = 2;
    public int playerY = 3;
    public int movement = 6;

    [Header("Passion Fruit Juice")]
    public ParticleSystem playerMove;
    public ParticleSystem gridExploder;
    public GameObject cellDestroy;
    public ParticleSystem super;

    [Header("Ene")]
    public GameObject bullet;
    public float inicounterL;
    public float inicounterU;
    private bool start = false;
    private int randomx;
    private int randomy;

    [Header("SoundEffect")]
    public AudioSource explode;
    public AudioSource swap;
    public AudioSource laser;
    public AudioSource warning;
    public AudioSource bgm;

    void Start()
    {
        score = 0;

        End = true;

        randomx = Random.Range(0, 3);
        randomy = Random.Range(0, 6);

        me = this;

        InitGrid();

        CopyGrid();

        scoreText = GetComponent<TextMesh>();

        CancelCombosX();
        CancelCombosY();
    }


    void Update()
    {
        AssignColor();
        scoreText.text = ("Score:" + score);

        CopyGrid();

        if (clearX == false || clearY == false)
            return;
        DestroyX();
        DestroyY();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].type == 0)
                    Destroy(grid[x, y].mover.gameObject);
            }
        }
        MoveCellDown();

        if (End)
            EndGame();

        Vector2Int input = Vector2Int.zero;
        input.x += Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
        input.x += Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
        input.y += Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0;
        input.y += Input.GetKeyDown(KeyCode.DownArrow) ? -1 : 0;

        if (input.x == 1 || input.x == -1)
            input.y = 0;
        if (input.y == 1 || input.y == -1)
            input.x = 0;

        if (input.magnitude > 0)
        {
            //grid[playerX, playerY].type = grid[playerX + input.x, playerY + input.y].type;
            //grid[playerX + input.x, playerY + input.y].type = 7;
            combo = 1;
            start = true;
            swap.Play();

            var tmpCell = grid[playerX, playerY];
            grid[playerX, playerY] = grid[playerX + input.x, playerY + input.y];
            grid[playerX, playerY].mover.desiredPosition = ToWorld(playerX, playerY);
            grid[playerX + input.x, playerY + input.y] = tmpCell;
            grid[playerX + input.x, playerY + input.y].mover.desiredPosition = ToWorld(playerX + input.x,
                                                                                      playerY + input.y);
            //playerMove.Play();

            playerX += input.x;
            playerY += input.y;
            if (End)
                movement--;
        }

        if (start)
        {
            inicounterL -= 1.8f * Time.deltaTime;
            inicounterU -= 1.6f * Time.deltaTime;
            if(!bgm.isPlaying)
                bgm.Play();
        }

        if(inicounterL <= 1.5f)
        {
            if (!warning.isPlaying)
                warning.Play();   
        }
        if (inicounterL <= 0)
        {
            GameObject ENE1 = Instantiate(bullet, new Vector3(-14, randomy * cellLength + gridOffset.y), Quaternion.identity);
            
            if(score >= 100)
            {
                randomy = Random.Range(0, 6);
                GameObject ENE2 = Instantiate(bullet, new Vector3(-14, randomy * cellLength + gridOffset.y), Quaternion.identity);
            }
            laser.Play();

            randomy = Random.Range(0, 6);
            inicounterL = 10;
        }
        
        if(inicounterU <= 1.5f)
        {
            if (!warning.isPlaying)
                warning.Play();
        }
        if (inicounterU <= 0)
        {
            GameObject ENE1 = Instantiate(bullet, new Vector3(randomx * cellLength + gridOffset.x, 12.5f ), Quaternion.identity);
            if(score >= 150)
            {
                randomx = Random.Range(0, 3);
                GameObject ENE2 = Instantiate(bullet, new Vector3(randomx * cellLength + gridOffset.x, 12.5f), Quaternion.identity);
            }
            if (!laser.isPlaying)
                laser.Play();


            randomx = Random.Range(0, 3);
            inicounterU = 5;
        }
        

    }


    Vector2 ToWorld(int x, int y)
    {
        return new Vector3(x * cellLength + gridOffset.x, y * cellLength + gridOffset.y);
    }

    void InitGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MakeNewGridCell(x, y);
                if (x == 2 && y == 3)
                    grid[x, y].type = 7;

            }
        }
    }
    void MakeNewGridCell(int x, int y)
    {
        grid[x, y].type = Random.Range(1, 7);
        GameObject obj = Instantiate(cell, new Vector3(x * cellLength + gridOffset.x, y * cellLength + gridOffset.y), Quaternion.identity);
        grid[x, y].sr = obj.GetComponent<SpriteRenderer>();
        grid[x, y].mover = obj.GetComponent<CellMover>();
        grid[x, y].mover.desiredPosition = ToWorld(x, y);
    }

    void DestroyCell(int x, int y)
    {
        GameObject cellDesPar = Instantiate(cellDestroy, new Vector3(x * cellLength + gridOffset.x, y * cellLength + gridOffset.y), Quaternion.identity);
    }
    void CancelCombosX()
    {
        checkAgainX = false;
        for (int y = 0; y < height; y++)
        {
            if (copiedGrid[0, y] == copiedGrid[1, y] && copiedGrid[0, y] == copiedGrid[2, y] && copiedGrid[0, y] != 0)
            {
                grid[0, y].type = Random.Range(1, 7);
                grid[1, y].type = Random.Range(1, 7);
                grid[2, y].type = Random.Range(1, 7);
                checkAgainX = true;
            }

            if (copiedGrid[1, y] == copiedGrid[2, y] && copiedGrid[1, y] == copiedGrid[3, y] && copiedGrid[1, y] != 0)
            {
                grid[1, y].type = Random.Range(1, 7);
                grid[2, y].type = Random.Range(1, 7);
                grid[3, y].type = Random.Range(1, 7);
                checkAgainX = true;

            }

            if (copiedGrid[2, y] == copiedGrid[3, y] && copiedGrid[2, y] == copiedGrid[4, y] && copiedGrid[2, y] != 0)
            {
                grid[2, y].type = Random.Range(1, 7);
                grid[3, y].type = Random.Range(1, 7);
                grid[4, y].type = Random.Range(1, 7);
                checkAgainX = true;
            }
        }
        CopyGrid();
        if (checkAgainX == true)
        {
            CancelCombosX();
            print("Checking Again on X");
        }
        else
        {
            clearX = true;
            print("All Clear On X");
        }
    }

    void CancelCombosY()
    {
        for (int x = 0; x < width; x++)
        {
            checkAgainY = false;
            if (copiedGrid[x, 0] == copiedGrid[x, 1] && copiedGrid[x, 0] == copiedGrid[x, 2] && copiedGrid[x, 0] != 0)
            {
                grid[x, 0].type = Random.Range(1, 7);
                grid[x, 1].type = Random.Range(1, 7);
                grid[x, 2].type = Random.Range(1, 7);
                checkAgainY = true;
            }

            if (copiedGrid[x, 1] == copiedGrid[x, 2] && copiedGrid[x, 1] == copiedGrid[x, 3] && copiedGrid[x, 1] != 0)
            {
                grid[x, 1].type = Random.Range(1, 7);
                grid[x, 2].type = Random.Range(1, 7);
                grid[x, 3].type = Random.Range(1, 7);
                checkAgainY = true;
            }

            if (copiedGrid[x, 2] == copiedGrid[x, 3] && copiedGrid[x, 2] == copiedGrid[x, 4] && copiedGrid[x, 2] != 0)
            {
                grid[x, 2].type = Random.Range(1, 7);
                grid[x, 3].type = Random.Range(1, 7);
                grid[x, 4].type = Random.Range(1, 7);
                checkAgainY = true;
            }

            if (copiedGrid[x, 3] == copiedGrid[x, 4] && copiedGrid[x, 3] == copiedGrid[x, 5] && copiedGrid[x, 3] != 0)
            {
                grid[x, 3].type = Random.Range(1, 7);
                grid[x, 4].type = Random.Range(1, 7);
                grid[x, 5].type = Random.Range(1, 7);
                checkAgainY = true;
            }

            if (copiedGrid[x, 4] == copiedGrid[x, 5] && copiedGrid[x, 4] == copiedGrid[x, 6] && copiedGrid[x, 4] != 0)
            {
                grid[x, 4].type = Random.Range(1, 7);
                grid[x, 5].type = Random.Range(1, 7);
                grid[x, 6].type = Random.Range(1, 7);
                checkAgainY = true;
            }
        }
        CopyGrid();
        if (checkAgainY == true)
        {
            CancelCombosY();
            print("Checking Again on Y");
        }
        else
        {
            clearY = true;
            print("All Clear On Y");
        }
    }

    void CopyGrid()
    {
        copiedGrid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                copiedGrid[x, y] = grid[x, y].type;
            }
        }
    }

    bool InsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void AssignColor()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].sr.color = colors[grid[x, y].type];
            }
        }


    }

    void DestroyX()
    {
        for (int y = 0; y < height; y++)
        {
            if (copiedGrid[0, y] == copiedGrid[1, y] && copiedGrid[0, y] == copiedGrid[2, y] && copiedGrid[0, y] != 0)
            {
                //gridExploder.Play();
                grid[0, y].type = 0;
                grid[1, y].type = 0;
                grid[2, y].type = 0;

                DestroyCell(0, y);
                DestroyCell(1, y);
                DestroyCell(2, y);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[1, y] == copiedGrid[2, y] && copiedGrid[1, y] == copiedGrid[3, y] && copiedGrid[1, y] != 0)
            {
                //gridExploder.Play();
                grid[1, y].type = 0;
                grid[2, y].type = 0;
                grid[3, y].type = 0;

                DestroyCell(1, y);
                DestroyCell(2, y);
                DestroyCell(3, y);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[2, y] == copiedGrid[3, y] && copiedGrid[2, y] == copiedGrid[4, y] && copiedGrid[2, y] != 0)
            {
                //gridExploder.Play();
                grid[2, y].type = 0;
                grid[3, y].type = 0;
                grid[4, y].type = 0;

                DestroyCell(2, y);
                DestroyCell(3, y);
                DestroyCell(4, y);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }
        }
    }

    void DestroyY()
    {
        for (int x = 0; x < width; x++)
        {
            if (copiedGrid[x, 0] == copiedGrid[x, 1] && copiedGrid[x, 0] == copiedGrid[x, 2] && copiedGrid[x, 0] != 0)
            {
                //gridExploder.Play();
                grid[x, 0].type = 0;
                grid[x, 1].type = 0;
                grid[x, 2].type = 0;

                DestroyCell(x, 0);
                DestroyCell(x, 1);
                DestroyCell(x, 2);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[x, 1] == copiedGrid[x, 2] && copiedGrid[x, 1] == copiedGrid[x, 3] && copiedGrid[x, 1] != 0)
            {
                //gridExploder.Play();
                grid[x, 1].type = 0;
                grid[x, 2].type = 0;
                grid[x, 3].type = 0;

                DestroyCell(x, 1);
                DestroyCell(x, 2);
                DestroyCell(x, 3);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[x, 2] == copiedGrid[x, 3] && copiedGrid[x, 2] == copiedGrid[x, 4] && copiedGrid[x, 2] != 0)
            {
                //gridExploder.Play();
                grid[x, 2].type = 0;
                grid[x, 3].type = 0;
                grid[x, 4].type = 0;

                DestroyCell(x, 2);
                DestroyCell(x, 3);
                DestroyCell(x, 4);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[x, 3] == copiedGrid[x, 4] && copiedGrid[x, 3] == copiedGrid[x, 5] && copiedGrid[x, 3] != 0)
            {
                //gridExploder.Play();
                grid[x, 3].type = 0;
                grid[x, 4].type = 0;
                grid[x, 5].type = 0;

                DestroyCell(x, 3);
                DestroyCell(x, 4);
                DestroyCell(x, 5);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .02f), 0.2f + (combo * .02f)), 0.1f + (combo * .2f));
            }

            if (copiedGrid[x, 4] == copiedGrid[x, 5] && copiedGrid[x, 4] == copiedGrid[x, 6] && copiedGrid[x, 4] != 0)
            {
                //gridExploder.Play();
                grid[x, 4].type = 0;
                grid[x, 5].type = 0;
                grid[x, 6].type = 0;

                DestroyCell(x, 4);
                DestroyCell(x, 5);
                DestroyCell(x, 6);
                if (combo == 1)
                    explode.Play();

                score += (2 + combo);
                movement = 6;
                combo += combo;
                CamControl.me.ShakeScreen(new Vector3(.05f + (combo * .01f), 0.2f + (combo * .01f)), 0.1f + (combo * .2f));
            }

        }
    }

    void MoveCellDown()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TryToFall(x, y);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].type == 0)
                {
                    MakeNewGridCell(x, y);
                    grid[x, y].mover.transform.position += Vector3.up * 5f;
                }
            }
        }
    }

    void TryToFall(int x, int y)
    {
        if (!InsideGrid(x, y - 1) || grid[x, y - 1].type != 0)
            return;
        int yOffset = -1;

        while (InsideGrid(x, y + yOffset - 1) && grid[x, y + yOffset - 1].type == 0)
        {
            yOffset--;
        }
        if (grid[x, y].type == 7)
            playerY += yOffset;
        grid[x, y + yOffset].type = grid[x, y].type;
        grid[x, y].type = 0;

        grid[x, y + yOffset].mover = grid[x, y].mover;
        grid[x, y + yOffset].sr = grid[x, y].sr;
        grid[x, y + yOffset].mover.desiredPosition = ToWorld(x, y + yOffset);
    }

    void EndGame()
    {
        if (movement == 0)
        {
            SceneManager.LoadScene("End");
        }
    }
}




public struct Cell
{
    public int type;
    public Color color;
    public SpriteRenderer sr;
    public CellMover mover;
}