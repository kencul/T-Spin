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

    static Dictionary<int, List<int>> IMinoR = new()
    {
        { 0, new() { 3 } },
        { 1, new() { 0, 1, 2, 3 } },
        { 2, new() { 0 } },
        { 3, new() { 0, 1, 2, 3 } }
    };
    
    static Dictionary<int, List<int>> IMinoL = new()
    {
        { 0, new() { 0 } },
        { 1, new() { 0, 1, 2, 3 } },
        { 2, new() { 3 } },
        { 3, new() { 0, 1, 2, 3 } }
    };

    static Dictionary<int, List<int>> IBottomChildren = new()
    {
        { 0, new() { 0, 1, 2, 3 } },
        { 1, new() { 3 } },
        { 2, new() { 3, 2, 1, 0 } },
        { 3, new() { 0 } }
    };

    static Dictionary<int, int> ILowestChild = new()
    {
        { 0, 0 },
        { 1, 3 },
        { 2, 3 },
        { 3, 0 }
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

    static Dictionary<int, List<int>> OMinoR = new()
    {
        { 0, new() { 3, 2 } },
        { 1, new() { 1, 3 } },
        { 2, new() { 0, 1 } },
        { 3, new() { 0, 2 } }
    };

    static Dictionary<int, List<int>> OMinoL = new()
    {
        { 0, new() { 0, 1 } },
        { 1, new() { 0, 2 } },
        { 2, new() { 3, 2 } },
        { 3, new() { 3, 1 } }
    };

    static Dictionary<int, List<int>> OBottomChildren = new()
    {
        { 0, new() { 0, 2 } },
        { 1, new() { 2, 3 } },
        { 2, new() { 3, 1 } },
        { 3, new() { 1, 0 } }
    };

    static Dictionary<int, int> OLowestChild = new()
    {
        { 0, 0 },
        { 1, 2 },
        { 2, 3 },
        { 3, 1 }
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

    static Dictionary<int, List<int>> TMinoR = new()
    {
        { 0, new() { 3, 2 } },
        { 1, new() { 0, 2, 3 } },
        { 2, new() { 0, 2 } },
        { 3, new() { 3, 1, 0 } }
    };

    static Dictionary<int, List<int>> TMinoL = new()
    {
        { 0, new() { 0, 2 } },
        { 1, new() { 0, 1, 3 } },
        { 2, new() { 3, 2 } },
        { 3, new() { 3, 2, 0 } }
    };

    static Dictionary<int, List<int>> TBottomChildren = new()
    {
        { 0, new() { 0, 1, 3 } },
        { 1, new() { 3, 2 } },
        { 2, new() { 3, 2, 0 } },
        { 3, new() { 2, 0 } }
    };

    static Dictionary<int, int> TLowestChild = new()
    {
        { 0, 0 },
        { 1, 3 },
        { 2, 2 },
        { 3, 0 }
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

    static Dictionary<int, List<int>> LMinoR = new()
    {
        { 0, new() { 3, 2 } },
        { 1, new() { 0, 1, 3 } },
        { 2, new() { 0, 3 } },
        { 3, new() { 0, 1, 2 } }
    };

    static Dictionary<int, List<int>> LMinoL = new()
    {
        { 0, new() { 0, 3 } },
        { 1, new() { 0, 1, 2 } },
        { 2, new() { 3, 2 } },
        { 3, new() { 3, 1, 0 } }
    };

    static Dictionary<int, List<int>> LBottomChildren = new()
    {
        { 0, new() { 0, 1, 2} },
        { 1, new() { 2, 3 } },
        { 2, new() { 3, 1, 0 } },
        { 3, new() { 3, 0 } }
    };

    static Dictionary<int, int> LLowestChild = new()
    {
        { 0, 0 },
        { 1, 2 },
        { 2, 3 },
        { 3, 0 }
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

    static Dictionary<int, List<int>> JMinoR = new()
    {
        { 0, new() { 3, 1 } },
        { 1, new() { 1, 2, 3 } },
        { 2, new() { 0, 1 } },
        { 3, new() { 0, 2, 3 } }
    };

    static Dictionary<int, List<int>> JMinoL = new()
    {
        { 0, new() { 0, 1 } },
        { 1, new() { 0, 2, 3 } },
        { 2, new() { 3, 1 } },
        { 3, new() { 3, 2, 1 } }
    };

    static Dictionary<int, List<int>> JBottomChildren = new()
    {
        { 0, new() { 0, 2, 3 } },
        { 1, new() { 3, 1 } },
        { 2, new() { 3, 2, 1 } },
        { 3, new() { 1, 0 } }
    };

    static Dictionary<int, int> JLowestChild = new()
    {
        { 0, 0 },
        { 1, 3 },
        { 2, 1 },
        { 3, 1 }
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

    static Dictionary<int, List<int>> SMinoR = new()
    {
        { 0, new() { 3, 1 } },
        { 1, new() { 0, 2, 3 } },
        { 2, new() { 0, 2 } },
        { 3, new() { 3, 1, 0 } }
    };

    static Dictionary<int, List<int>> SMinoL = new()
    {
        { 0, new() { 0, 2 } },
        { 1, new() { 0, 1, 3 } },
        { 2, new() { 3, 1 } },
        { 3, new() { 3, 2, 0 } }
    };

    static Dictionary<int, List<int>> SBottomChildren = new()
    {
        { 0, new() { 0, 1, 3 } },
        { 1, new() { 1, 3 } },
        { 2, new() { 3, 2, 0 } },
        { 3, new() { 2, 0 } }
    };

    static Dictionary<int, int> SLowestChild = new()
    {
        { 0, 0 },
        { 1, 3 },
        { 2, 3 },
        { 3, 0 }
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

    static Dictionary<int, List<int>> ZMinoR = new()
    {
        { 0, new() { 2, 3 } },
        { 1, new() { 0, 2, 3 } },
        { 2, new() { 0, 1 } },
        { 3, new() { 0, 1, 3 } }
    };

    static Dictionary<int, List<int>> ZMinoL = new()
    {
        { 0, new() { 0, 1 } },
        { 1, new() { 0, 1, 3 } },
        { 2, new() { 3, 2 } },
        { 3, new() { 3, 2, 0 } }
    };

    static Dictionary<int, List<int>> ZBottomChildren = new()
    {
        { 0, new() { 0, 1, 3 } },
        { 1, new() { 3, 2 } },
        { 2, new() { 3, 2, 0 } },
        { 3, new() { 0, 1 } }
    };

    static Dictionary<int, int> ZLowestChild = new()
    {
        { 0, 1 },
        { 1, 3 },
        { 2, 2 },
        { 3, 0 }
    };


    public static List<int> ReturnRightChildren (string s, int mode)
    {
        switch (s)
        {
            case "i":
                return IMinoR[mode];
            case "o":
                return OMinoR[mode];
            case "t":
                return TMinoR[mode];
            case "l":
                return LMinoR[mode];
            case "j":
                return JMinoR[mode];
            case "z":
                return ZMinoR[mode];
            case "s":
                return SMinoR[mode];
            default:
                Debug.LogFormat("ERROR: {0} not part of rotaMods", s);
                return new List<int> { 0, 1, 2, 3 };
        }
    }

    public static List<int> ReturnLeftChildren(string s, int mode)
    {
        switch (s)
        {
            case "i":
                return IMinoL[mode];
            case "o":
                return OMinoL[mode];
            case "t":
                return TMinoL[mode];
            case "l":
                return LMinoL[mode];
            case "j":
                return JMinoL[mode];
            case "z":
                return ZMinoL[mode];
            case "s":
                return SMinoL[mode];
            default:
                Debug.LogFormat("ERROR: {0} not part of rotaMods", s);
                return new List<int> { 0, 1, 2, 3 };
        }
    }

    public static List<int> ReturnBottomChildren(string s, int mode)
    {
        switch (s)
        {
            case "i":
                return IBottomChildren[mode];
            case "o":
                return OBottomChildren[mode];
            case "t":
                return TBottomChildren[mode];
            case "l":
                return LBottomChildren[mode];
            case "j":
                return JBottomChildren[mode];
            case "z":
                return ZBottomChildren[mode];
            case "s":
                return SBottomChildren[mode];
            default:
                Debug.LogFormat("ERROR: {0} not part of rotaMods", s);
                return new List<int> { 0, 1, 2, 3 };
        }
    }

    public static int ReturnLowestChild(string s, int mode)
    {
        switch (s)
        {
            case "i":
                return ILowestChild[mode];
            case "o":
                return OLowestChild[mode];
            case "t":
                return TLowestChild[mode];
            case "l":
                return LLowestChild[mode];
            case "j":
                return JLowestChild[mode];
            case "z":
                return ZLowestChild[mode];
            case "s":
                return SLowestChild[mode];
            default:
                Debug.LogFormat("ERROR: {0} not part of rotaMods", s);
                return 0;
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
