using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public enum gameState
    {
        wait,
        move
    }

     
    public gameState currentState = gameState.move;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;
    private FindMatches findMatches;
    public GameObject destroyEffect;




    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width,height];
        allDots = new GameObject[width, height];
        Setup();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Creating the board
    private void Setup()
    {
        for (int i = 0;i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j+offset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos,Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ',' + j + " )";
                int dotToUse = Random.Range(0, dots.Length);

                int maxIterations = 0;
                while (matchesAt(i, j, dots[dotToUse]) && maxIterations<100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                GameObject Dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                Dot.GetComponent<Dot>().row = j;
                Dot.GetComponent<Dot>().col = i;
                Dot.transform.parent = this.transform;
                Dot.name = "Dot: ( " + i + ',' + j + " )";
                allDots[i, j] = Dot;
            }
        }
    }

    private bool matchesAt(int col,int row,GameObject obj)
    {
        if (col > 1 && row > 1)
        {
            if (allDots[col - 1, row].tag == obj.tag && allDots[col - 2, row].tag == obj.tag)
            {
                return true;
            }
            if (allDots[col, row - 1].tag == obj.tag && allDots[col, row - 2].tag == obj.tag)
            {
                return true;
            }

        }
        else if (col <= 1  || row <= 1){
            if (row > 1)
            {
                if(allDots[col,row-1].tag == obj.tag && allDots[col,row-2].tag == obj.tag)
                {
                    return true;
                }
            }
            if (col > 1)
            {
                if (allDots[col-1, row].tag == obj.tag && allDots[col-2, row].tag == obj.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void destroyMatchesAt(int col,int row)
    {
        if (allDots[col, row].GetComponent<Dot>().isMatched)
        {
            findMatches.currentMatches.Remove(allDots[col, row]);
            GameObject particule = Instantiate(destroyEffect,allDots[col,row].transform.position,Quaternion.identity);
            Destroy(particule, .5f);
            Destroy(allDots[col, row]);
            allDots[col, row] = null;
        }
    }

    public void destroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    destroyMatchesAt(i,j)
;
                }
            }
        }
        StartCoroutine(decreaceRowCo());
    }

    private IEnumerator decreaceRowCo()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount>0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(fillBoardCo());
    }

    private void refillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j+offset);
                    int dotToUse = Random.Range(0,dots.Length);
                    GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    allDots[i, j] = dot;
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().col = i;
                }
            }
        }
    }

    private bool matchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator fillBoardCo()
    {
        refillBoard();

        yield return new WaitForSeconds(.5f);

        while (matchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            destroyMatches();
        }

        yield return new WaitForSeconds(.5f);
        currentState = gameState.move;
    }
}
