using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class Extensions
{
    public static List<Tile> RemoveAllTiles(this List<Tile> tileList)
    {
        if (tileList == null) return null;

        foreach (Tile child in new List<Tile>(tileList))
        {
            child.gameObject.SetActive(false);
            tileList.Remove(child);
        }
        return tileList;
    }
    public static void RemoveAllDot(this Dot[,] allDotMatrix)
    {
        for (int x = 0; x < allDotMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < allDotMatrix.GetLength(1); y++)
            {
                allDotMatrix[x, y].gameObject.SetActive(false);
            }
        }
        Array.Clear(allDotMatrix, 0, allDotMatrix.Length);
    }
    public static int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return min;
        }
        else if (input > max)
        {
            return max;
        }
        return input;
    }
}
