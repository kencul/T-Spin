using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotaModes
{
    //I Mino
    //
    //rotation Modes:

    //0 = [0] [1] [2] [3]

    //1 = [0]
    //    [1]
    //    [2]
    //    [3]

    //2 = [3] [2] [1] [0]

    //3 = [3]
    //    [2]
    //    [1]
    //    [0]

    static Dictionary<int, (int, int)> IMinoLR = new()
    {
        { 0, (0, 3) },
        { 1, (0, 0) },
        { 2, (3, 0) },
        { 3, (0, 0) }
    };


    //OMino
    //
    //rotation Modes:

    //0 = [1] [3]
    //    [0] [2]

    //1 = [0] [1]
    //    [2] [3]

    //2 = [2] [0]
    //    [3] [1]

    //3 = [3] [2]
    //    [1] [0]

    static Dictionary<int, (int, int)> OMinoLR = new()
    {
        { 0, (0, 2) },
        { 1, (2, 3) },
        { 2, (3, 1) },
        { 3, (1, 0) }
    };

    //TMino
    //
    //rotation Modes:

    //0 =     [2]
    //    [0] [1] [3]

    //1 = [0]
    //    [1] [2]
    //    [3]

    //2 = [3] [1] [0]
    //        [2]

    //3 =     [3]
    //    [2] [1]
    //        [0]

    static Dictionary<int, (int, int)> TMinoLR = new()
    {
        { 0, (0, 3) },
        { 1, (3, 2) },
        { 2, (3, 0) },
        { 3, (2, 0) }
    };

    //LMino
    //
    //rotation Modes:

    //0 =         [3]
    //    [0] [1] [2]

    //1 = [0]
    //    [1] 
    //    [2] [3]

    //2 = [2] [1] [0]
    //    [3]

    //3 = [3] [2]
    //        [1]
    //        [0] 

    static Dictionary<int, (int, int)> LMinoLR = new()
    {
        { 0, (0, 2) },
        { 1, (2, 3) },
        { 2, (3, 0) },
        { 3, (3, 0) }
    };

    //JMino
    //
    //rotation Modes:

    //0 = [1]
    //    [0] [2] [3]

    //1 = [0] [1]
    //    [2] 
    //    [3] 

    //2 = [3] [2] [0]
    //            [1]

    //3 =     [3]
    //        [2]
    //    [1] [0]

    static Dictionary<int, (int, int)> JMinoLR = new()
    {
        { 0, (0, 3) },
        { 1, (3, 1) },
        { 2, (3, 1) },
        { 3, (1, 0) }
    };

    //SMino
    //
    //rotation Modes:

    //0 =     [2] [3]
    //    [0] [1]

    //1 = [0] 
    //    [1] [2]
    //        [3] 

    //2 =     [1] [0]
    //    [3] [2]

    //3 = [3] 
    //    [2] [1]
    //        [0]

    static Dictionary<int, (int, int)> SMinoLR = new()
    {
        { 0, (0, 3) },
        { 1, (1, 3) },
        { 2, (3, 0) },
        { 3, (2, 0) }
    };

    //ZMino
    //
    //rotation Modes:

    //0 = [0] [2]
    //        [1] [3]

    //1 =     [0] 
    //    [1] [2]
    //    [3] 

    //2 = [3] [1]
    //        [2] [0]

    //3 =     [3] 
    //    [2] [1]
    //    [0]

    static Dictionary<int, (int, int)> ZMinoLR = new()
    {
        { 0, (0, 3) },
        { 1, (3, 2) },
        { 2, (3, 0) },
        { 3, (0, 1) }
    };

    public static (int,int) ReturnLRTuple (string s, int mode)
    {
        switch (s)
        {
            case "i":
                return IMinoLR[mode];
            case "o":
                return OMinoLR[mode];
            case "t":
                return TMinoLR[mode];
            case "l":
                return LMinoLR[mode];
            case "j":
                return JMinoLR[mode];
            case "z":
                return ZMinoLR[mode];
            default:
                Debug.LogFormat("ERROR: {0} not part of rotaMods", s);
                return (0, 0);
        }
    }

    //public void setIMinoLeftRightChild(Dictionary<> leftRightChild)
    //{
    //    leftRightChild.Add(0, (childrenGO[0], childrenGO[3]));
    //    leftRightChild.Add(1, (childrenGO[0], childrenGO[0]));
    //    leftRightChild.Add(2, (childrenGO[3], childrenGO[0]));
    //    leftRightChild.Add(3, (childrenGO[0], childrenGO[0]));
    //}
}
