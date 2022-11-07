using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRSWallKick
{
    //https://tetris.fandom.com/wiki/SRS


    //J, L, T, S, Z 

    static Dictionary<(int, int), List<(int, int)>> mainKickTable = new()
    {
        { (0, 1), new() { (0, 0), (-1, 0), (-1, 1), (0, -2), (-1, -2) } },
        { (1, 0), new() { (0, 0), (1, 0), (1, -1), (0, 2), (1, 2) } },
        { (1, 2), new() { (0, 0), (1, 0), (1, -1), (0, 2), (1, 2) } },
        { (2, 1), new() { (0, 0), (-1, 0), (-1, 1), (0, -2), (-1, -2) } },
        { (2, 3), new() { (0, 0), (1, 0), (1, 1), (0, -2), (1, -2) } },
        { (3, 2), new() { (0, 0), (-1, 0), (-1, -1), (0, 2), (-1, 2) } },
        { (3, 0), new() { (0, 0), (-1, 0), (-1, -1), (0, 2), (-1, 2) } },
        { (0, 3), new() { (0, 0), (1, 0), (1, 1), (0, -2), (1, -2) } }
    };

    static Dictionary<(int, int), List<(int, int)>> IKickTable = new()
    {
        { (0, 1), new() { (0, 0), (-2, 0), (1, 0), (-2, -1), (1, 2) } },
        { (1, 0), new() { (0, 0), (2, 0), (-1, 0), (2, 1), (-1, -2) } },
        { (1, 2), new() { (0, 0), (-1, 0), (2, 0), (-1, 2), (2, -1) } },
        { (2, 1), new() { (0, 0), (1, 0), (-2, 0), (1, -2), (-2, 1) } },
        { (2, 3), new() { (0, 0), (2, 0), (-1, 0), (2, 1), (-1, -2) } },
        { (3, 2), new() { (0, 0), (-2, 0), (1, 0), (-2, -1), (1, 2) } },
        { (3, 0), new() { (0, 0), (1, 0), (-2, 0), (1, -2), (-2, 1) } },
        { (0, 3), new() { (0, 0), (-1, 0), (2, 0), (-1, 2), (2, -1) } }
    };


    public static List<(int, int)> ReturnKickTable(string piece, (int, int) rotationMode)
    {
        switch (piece)
        {
            case "j":
            case "l":
            case "t":
            case "s":
            case "z":
                return mainKickTable[rotationMode];
            case "i":
                return IKickTable[rotationMode];
            default:
                return new List<(int, int)> { (0, 0) };
        }
    }
}
