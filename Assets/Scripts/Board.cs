using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int BOARD_MIN_SIZE = 0;
    private const int BOARD_MAX_SIZE = 8;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Dot dotPrefab;
    [SerializeField] private Transform dotParent;
    private List<Tile> currentBoard = new List<Tile>();
    private List<Dot> matchedDotList = new List<Dot>();
    private Dot[,] allDotMatrix;
    private ObjectPool tilePool;
    private ObjectPool dotPool;
    private float timer = 0, maxTimer = 1f, delay = 0f, maxDelay = 0.5f;
    public int Width
    {
        get => width;
        set
        {
            width = Mathf.Clamp(value, BOARD_MIN_SIZE, BOARD_MAX_SIZE);
        }
    }
    public int Height
    {
        get => height;
        set
        {
            height = Mathf.Clamp(value, BOARD_MIN_SIZE, BOARD_MAX_SIZE);
        }
    }
    private void Awake()
    {
        tilePool = ObjectPool.CreateInstance(tilePrefab, 64);
        dotPool = ObjectPool.CreateInstance(dotPrefab, 64);
    }
    private void Start()
    {
        InitializeBoard(width, height);
    }
    private void Update()
    {
        if (dotPool.CheckAvaiableObjectList())
        {
            timer += Time.deltaTime;
            if (timer > maxTimer)
            {
                timer = 0;
                RefillBoard();
            }
        }
    }
    public void RefillBoard()
    {
        List<PoolableObject> poolableObjectList = dotPool.GetAvailableObjectList();
        // for (int i = poolableObjectList.Count; i > 0; i--)
        // {
        //     Dot matchedDot = dotPool.GetObject().GetComponent<Dot>();
        //     matchedDot.Column = height - 1;
        //     Vector2 tilePosition = CalculateTilePosition(width, height, matchedDot.Row, matchedDot.Column);
        //     matchedDot.transform.position = tilePosition;
        //     allDotMatrix[matchedDot.Row, matchedDot.Column] = matchedDot;
        // }
        //delay -= Time.deltaTime;
        // if (delay < 0)
        // {
        //     delay = maxDelay;
        //     Dot matchedDot = dotPool.GetObject().GetComponent<Dot>();
        //     matchedDot.Column = height - 1;
        //     Vector2 tilePosition = CalculateTilePosition(width, height, matchedDot.Row, matchedDot.Column);
        //     matchedDot.transform.position = tilePosition;
        //     allDotMatrix[matchedDot.Row, matchedDot.Column] = matchedDot;

        // }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDotMatrix[x, y] == null)
                {
                    Dot matchedDot = dotPool.GetObject().GetComponent<Dot>();
                    matchedDot.Row = x;
                    matchedDot.Column = height - 1;
                    Vector2 tilePosition = CalculateTilePosition(width, height, matchedDot.Row, matchedDot.Column);
                    matchedDot.transform.position = tilePosition;
                    //matchedDot.TempPosition = tilePosition;   
                    allDotMatrix[matchedDot.Row, matchedDot.Column] = matchedDot;
                }
            }
        }
    }
    private bool CheckPreviousPosition(Vector2 tilePosition, Vector2 previousTilePosition)
    {
        return tilePosition == previousTilePosition;
    }
    private void InitializeBoard(int width, int height)
    {
        currentBoard = currentBoard.RemoveAllTiles();
        if (allDotMatrix != null)
        {
            allDotMatrix.RemoveAllDot();
        }
        allDotMatrix = new Dot[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 tilePosition = CalculateTilePosition(width, height, x, y);

                Tile tileInstance = tilePool.GetObject().GetComponent<Tile>();
                if (tileInstance != null)
                {
                    tileInstance.transform.position = tilePosition;
                    tileInstance.name = $"Tile: ({x} , {y})";
                    tileInstance.gameObject.SetActive(true);
                    currentBoard.Add(tileInstance);
                }
                Dot dotInstance = dotPool.GetObject().GetComponent<Dot>();
                if (dotInstance != null)
                {
                    dotInstance.Init(x, y, this);
                    while (CheckBoardMatches(x, y, dotInstance))
                        dotInstance.ChangeDotColor();
                    dotInstance.transform.position = tilePosition;
                    dotInstance.TempPosition = tilePosition;
                    allDotMatrix[x, y] = dotInstance;
                }
            }
        }
    }

    /* 
    -Calculate the tile position with the offset to make sure all tiles fit the screen, 
    so we don't need to change position of camera
    */
    private Vector2 CalculateTilePosition(int width, int height, int x, int y)
    {
        int averageNumberOfTile = (width + height) / 2;
        Vector2 offset = -Vector2.one * (averageNumberOfTile - 1) * 0.5f;
        Vector2 tilePosition = new Vector2(x, y) + offset;
        return tilePosition;
    }
    public Dot[,] GetAllDotMatrix()
    {
        //Debug.Log("GetAllDotMatrix");
        return allDotMatrix;
    }
    public void MatchedDotListAdd(Dot matchedDot)
    {
        matchedDotList.Add(matchedDot);
    }
    private bool CheckBoardMatches(int row, int column, Dot dot)
    {
        //Check matches at 2 dot from rows
        if (row > 1)
        {
            if (allDotMatrix[row - 1, column].Color == dot.Color && allDotMatrix[row - 2, column].Color == dot.Color)
            {
                return true;
            }
        }
        //Check matches at 2 dot from columns
        if (column > 1)
        {
            if (allDotMatrix[row, column - 1].Color == dot.Color && allDotMatrix[row, column - 2].Color == dot.Color)
            {
                return true;
            }
        }
        return false;
    }
}
