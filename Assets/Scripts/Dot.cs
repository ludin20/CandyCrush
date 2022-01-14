using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public bool isMatched = false;
    public int col;
    public int row;
    public int targetX;
    public int targetY;
    public int prevCol;
    public int prevRow;


    private FindMatches find;
    private Board board;

    private GameObject otherDot;

    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    public float swipeResist = 1f;
    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        find = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //col = targetX;
        //prevRow = row;
        //prevCol = col;

    }   

    // Update is called once per frame
    void Update()
    {
        //findMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }
        targetX = col;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move towards target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
            find.findAllMatches();

        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allDots[col, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move towards target
            tempPos = new Vector2(transform.position.x,targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
            if (board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
            find.findAllMatches();
        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            //board.allDots[col, row] = this.gameObject;
        }
    }

    public IEnumerator checkMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().col = col;
                row = prevRow;
                col = prevCol;
                yield return new WaitForSeconds(.5f);
                board.currentState = Board.gameState.move;
            }
            else
            {
                board.destroyMatches();
                
            }
            otherDot = null;
        }

    }

    private void OnMouseDown()
    {
        if(board.currentState == Board.gameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //Debug.Log(firstTouchPos);
    }

    private void OnMouseUp()
    {
        if(board.currentState == Board.gameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            calculateAngle();
        }
        else
        {
            board.currentState = Board.gameState.move;
        }
        
    }

    private void calculateAngle()
    {
        if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y ) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngle);
            movePieces();
            board.currentState = Board.gameState.wait;
        }
        
    }
        
    private void movePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && col < board.width-1){
            //Right Swipe
            otherDot = board.allDots[col + 1, row];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().col -= 1;
            col += 1;

        } else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1){
            //Up Swipe
            otherDot = board.allDots[col, row + 1];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().row -=1;
            row += 1;
           
        } else if((swipeAngle > 135 || swipeAngle <= -135) && col > 0){
            //Left Swipe
            otherDot = board.allDots[col - 1, row];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().col += 1;
            col -= 1;
        } else if(swipeAngle < -45 && swipeAngle >= -135 && row > 0){
            //Down Swipe
            otherDot = board.allDots[col, row - 1];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().row +=1;
            row -= 1;
        } 

        StartCoroutine(checkMoveCo());
    }

    private void findMatches()
    {
        if (col > 0 && col < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[col - 1, row];
            GameObject rightDot1 = board.allDots[col + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[col, row + 1];
            GameObject downDot1 = board.allDots[col, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }


}
