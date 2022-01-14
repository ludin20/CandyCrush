using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void findAllMatches()
    {
        StartCoroutine(findAllMatchesCo());
    }

    private IEnumerator findAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    if(i>0 && i < board.width - 1)
                    {
                        GameObject leftdot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if(leftdot != null && rightDot != null)
                        {
                            if(leftdot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (!currentMatches.Contains(leftdot))
                                {
                                    currentMatches.Add(leftdot);
                                }
                                leftdot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }
                    if (j>0 && j < board.height - 1)
                    {
                        GameObject downdot = board.allDots[i, j-1];
                        GameObject upDot = board.allDots[i, j+1];
                        if(downdot != null && upDot != null)
                        {
                            if(downdot.tag == currentDot.tag && upDot.tag == currentDot.tag)
                            {
                                if (!currentMatches.Contains(downdot))
                                {
                                    currentMatches.Add(downdot);
                                }
                                downdot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }
                }
            }
        }
    }
}
