using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dot : PoolableObject, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private SpriteRenderer dotVisual;
    [SerializeField] private Color[] colorArray;
    private enum DraggedDirection { Up, Down, Right, Left }
    public enum State { Idle, Moving }
    private DraggedDirection draggedDirection;
    private State state;
    private Board board;
    private Dot[,] allDotMatrix;
    private Dot otherDot;
    private new Camera camera;
    public Vector2 TempPosition { get; set; }
    public Color Color { get; set; }
    private int row;
    public int Row
    {
        get => row;
        set
        {
            int diff = 0;
            Vector2 diffVector = Vector2.zero;
            diff = value - row;
            row = value;
            diffVector.x += diff;
            TempPosition += diffVector;
            allDotMatrix[row, column] = this;
        }
    }
    private int column;
    public int Column
    {
        get => column;
        set
        {
            int diff = 0;
            Vector2 diffVector = Vector2.zero;
            diff = value - column;
            column = value;
            diffVector.y += diff;
            TempPosition += diffVector;
            allDotMatrix[row, column] = this;
        }
    }
    private bool isMatched = false;
    public bool IsMatched { get => isMatched; set => isMatched = value; }
    private void Awake()
    {
        camera = Camera.main;
    }
    private void OnEnable()
    {
        state = State.Idle;
        IsMatched = false;
        ChangeDotColor();
    }
    public void Init(int row, int column, Board board)
    {
        this.row = row;
        this.column = column;
        this.board = board;
        this.allDotMatrix = board.GetAllDotMatrix();
    }
    private void Update()
    {
        this.gameObject.name = $"Dot: ({row} , {column})";
        if (IsMatched)
        {
            allDotMatrix[row, column] = null;
            this.Row = 0;
            this.Column = 0;
            this.gameObject.SetActive(false);
        }
        CollapsColumns();
        CheckMatched(allDotMatrix);
        MovingToTempPosition();
    }

    private void CollapsColumns()
    {
        if (column < 1) return;
        Dot dotBelow = allDotMatrix[row, column - 1];
        if (dotBelow != null) return;
        allDotMatrix[row, column] = null;
        this.Column -= 1;
    }
    public void ChangeDotColor()
    {
        int colorToUse = UnityEngine.Random.Range(0, colorArray.Length);
        dotVisual.color = colorArray[colorToUse];
        Color = dotVisual.color;
    }
    /* -Check matched condition at all rows and columns */
    private void CheckMatched(Dot[,] allDotMatrix)
    {
        if (row > 0 && row < board.Width - 1)
        {
            CheckRows(allDotMatrix);
        }
        if (column > 0 && column < board.Height - 1)
        {
            CheckColumns(allDotMatrix);
        }
    }
    private void CheckRows(Dot[,] allDotMatrix)
    {
        Dot leftDot = allDotMatrix[row - 1, column];
        Dot rightDot = allDotMatrix[row + 1, column];
        if (CheckDotConditions(leftDot) && CheckDotConditions(rightDot))
        {
            leftDot.IsMatched = true;
            this.IsMatched = true;
            rightDot.IsMatched = true;
        }
    }
    private void CheckColumns(Dot[,] allDotMatrix)
    {
        Dot upDot = allDotMatrix[row, column + 1];
        Dot downDot = allDotMatrix[row, column - 1];
        if (CheckDotConditions(upDot) && CheckDotConditions(downDot))
        {
            upDot.IsMatched = true;
            this.IsMatched = true;
            downDot.IsMatched = true;
        }
    }
    private bool CheckDotConditions(Dot otherDot)
    {
        if (otherDot != null && otherDot.gameObject != this.gameObject && otherDot.Color == this.Color)
        {
            return true;
        }
        return false;
    }
    /* -TempPosition is a predictable value that dot position always follow to with a constanst speed */
    private void MovingToTempPosition()
    {
        if (TempPosition == (Vector2)this.transform.position) return;
        float speed = 4f;
        this.transform.position = Vector2.MoveTowards(this.transform.position, TempPosition, speed * Time.deltaTime);
    }
    //Pointer events
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (state == State.Moving) return;

        Vector2 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        draggedDirection = GetDragDirection(dragVectorDirection);

        MoveDot();
    }
    private DraggedDirection GetDragDirection(Vector2 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        Debug.Log(draggedDir);
        return draggedDir;
    }
    /*- Moving the dot pieces and checking the conditions to matches , if not match return their previous position with a coroutine */
    private void MoveDot()
    {
        Vector2 thisPreviousPosition = this.transform.position;
        Vector2 otherDotPreviousPosition = Vector2.zero;
        switch (draggedDirection)
        {
            case DraggedDirection.Up:
                if (column < board.Height - 1)
                {
                    otherDot = allDotMatrix[row, column + 1];
                    if (otherDot == null) return;
                    otherDotPreviousPosition = otherDot.transform.position;
                    otherDot.Column -= 1;

                    this.Column += 1;
                }
                break;
            case DraggedDirection.Down:
                if (column > 0)
                {
                    otherDot = allDotMatrix[row, column - 1];
                    if (otherDot == null) return;
                    otherDotPreviousPosition = otherDot.transform.position;
                    otherDot.Column += 1;

                    this.Column -= 1;
                }
                break;
            case DraggedDirection.Left:
                if (row > 0)
                {
                    otherDot = allDotMatrix[row - 1, column];
                    if (otherDot == null) return;
                    otherDotPreviousPosition = otherDot.transform.position;
                    otherDot.Row += 1;

                    this.Row -= 1;
                }
                break;
            case DraggedDirection.Right:
                if (row < board.Width - 1)
                {
                    otherDot = allDotMatrix[row + 1, column];
                    if (otherDot == null) return;
                    otherDotPreviousPosition = otherDot.transform.position;
                    otherDot.Row -= 1;

                    this.Row += 1;
                }
                break;
        }
        StartCoroutine(CheckMoveCoroutine(thisPreviousPosition, otherDotPreviousPosition));
    }
    /* -A coroutine to moving two dots to their previous position if also reset the moving state */
    private IEnumerator CheckMoveCoroutine(Vector2 thisPreviousPosition, Vector2 otherDotPreviousPosition)
    {
        WaitForSeconds wait = new WaitForSeconds(.5f);
        int tempRow = 0;
        int tempColumn = 0;
        this.SetState(State.Moving);
        otherDot.SetState(State.Moving);
        yield return wait;
        //Swap values
        if (!this.IsMatched && !otherDot.IsMatched)
        {
            tempRow = this.Row;
            tempColumn = this.Column;
            this.Row = otherDot.Row;
            this.Column = otherDot.Column;
            otherDot.Row = tempRow;
            otherDot.Column = tempColumn;

            this.TempPosition = thisPreviousPosition;
            otherDot.TempPosition = otherDotPreviousPosition;
        }
        yield return wait;
        this.SetState(State.Idle);
        otherDot.SetState(State.Idle);
        otherDot = null;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        ConfigOtherDotAfterMatched();
        ConfigThisDotAfterMatched();
    }

    private void ConfigThisDotAfterMatched()
    {
        if (this.state == State.Moving)
        {
            state = State.Idle;
        }
        this.transform.position = TempPosition;
        this.gameObject.name = $"Dot (Clone)";
    }

    private void ConfigOtherDotAfterMatched()
    {
        if (otherDot != null && otherDot.state == State.Moving)
        {
            otherDot.SetState(State.Idle);
            otherDot = null;
        }
    }
    public void SetState(State state)
    {
        this.state = state;
    }
    public State GetState()
    {
        return this.state;
    }
}
