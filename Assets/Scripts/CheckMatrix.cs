using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMatrix : MonoBehaviour
{
    public int x;
    public int y;
    public Board board;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Dot[,] boardDotMatrix = board.GetAllDotMatrix();
            Debug.Log(boardDotMatrix[x, y]);
        }
    }
}
