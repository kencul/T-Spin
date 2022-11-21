using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoardManagerJagged : MonoBehaviour
{
    //Singleton Instance
    public static BoardManagerJagged Instance;

    //[y, x] ***IMPORTANT & CONFUSING***, AXES FLIPPED FOR LINECLEARING LOGIC
    //Bottom left of board = [0,0]
    //one square = 0.4f, 0.4f
    //Bottom of board = -3.8f
    //Far Left of board = -1.8f
    //https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/jagged-arrays

    private GameObject[][] boardGameObjects = new GameObject[25][];

    void Awake()
    {
        //Singleton Instance
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        //Init jagged array
        for (int i = 0; i < boardGameObjects.Length; i++)
        {
            boardGameObjects[i] = new GameObject[10];
        }
    }

    /// <summary>
    /// Converts a transform.position into the matching (y,x) index of the playfield
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public (int, int) getGridIndex(Vector3 pos)
    {
        //https://docs.unity3d.com/ScriptReference/Mathf.Round.html
        int column = (int)(Mathf.Round((pos.x + 1.8f) / 0.4f));
        int row = (int)(Mathf.Round((pos.y + 3.8f) / 0.4f));
        return (row, column);
    }

    /// <summary>
    /// Assigns the reference to each child of the mino to its corresponding index in the jagged array
    /// Checks each line that was changed if it was filled, and runs the deleteLines method on those filled
    /// </summary>
    /// <param name="minoChildren"></param>
    public void placeInBoard(List<GameObject> minoChildren)
    {
        List<int> rowsChanged = new();
        List<int> filledLines = new();

        //Assign each cube to the mutidimensional array
        foreach (GameObject i in minoChildren)
        {
            //Debug.Log(i.transform.position);
            (int, int) gridIndex = getGridIndex(i.transform.position);
            boardGameObjects[gridIndex.Item1][gridIndex.Item2] = i;

            if (!rowsChanged.Contains(gridIndex.Item1))
            {
                rowsChanged.Add(gridIndex.Item1);
            }
        }

        //Check if any of the modified rows are completed
        foreach (int row in rowsChanged)
        {
            if (checkIfLineFilled(row))
                filledLines.Add(row);
        }

        //Delete GameObjects of completed lines
        //https://www.techiedelight.com/check-list-is-empty-csharp/
        if (filledLines.Any())
        {
            GameManager.Instance.PlayClip("LineClear");
            deleteLines(filledLines);
        }
        else
            GameManager.Instance.PlayClip("Place");
    }
    
    /// <summary>
    /// T-Spin check overload
    /// </summary>
    /// <param name="minoChildren"></param>
    /// <param name="T_Spin"></param>
    public void placeInBoard(List<GameObject> minoChildren, bool T_Spin)
    {
        List<int> rowsChanged = new();
        List<int> filledLines = new();

        //Assign each cube to the mutidimensional array
        foreach (GameObject i in minoChildren)
        {
            //Debug.Log(i.transform.position);
            (int, int) gridIndex = getGridIndex(i.transform.position);
            boardGameObjects[gridIndex.Item1][gridIndex.Item2] = i;

            if (!rowsChanged.Contains(gridIndex.Item1))
            {
                rowsChanged.Add(gridIndex.Item1);
            }
        }

        //Check if any of the modified rows are completed
        foreach (int row in rowsChanged)
        {
            if (checkIfLineFilled(row))
                filledLines.Add(row);
        }

        //Add number of rows cleared by T-Spin to score
        GameManager.Instance.Score += filledLines.Count;

        //Delete GameObjects of completed lines
        //https://www.techiedelight.com/check-list-is-empty-csharp/
        if (filledLines.Any())
        {
            GameManager.Instance.PlayClip("LineClear");
            deleteLines(filledLines);
        }
        else
            GameManager.Instance.PlayClip("Place");
    }

    /// <summary>
    /// Checks each tile of the input row
    /// Returns false if any tiles are open, true if all are filled
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool checkIfLineFilled(int row)
    {
        for (int j = 9; j >= 0; j--)
        {
            if (boardGameObjects[row][j] == null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Deletes the gameobject of the lines of the input;
    /// Checks the parent of each child deleted, and deletes the parent if it has no children left;
    /// Runs shiftLines with the lines deleted passed
    /// </summary>
    /// <param name="lines"></param>
    public void deleteLines(List<int> lines)
    {
        foreach (int line in lines)
        {
            List<Transform> parents = new();
            foreach (GameObject GO in boardGameObjects[line])
            {
                parents.Add(GO.transform.parent);
                Destroy(GO);
            }

            Debug.Log(parents.Any());
            /********************************************************************************/
            foreach (Transform parent in parents)
            {
                //Check if it was the power up
                if (parent.gameObject.CompareTag("power"))
                {
                    Debug.Log("POWERINGUP");
                    GameManager.Instance.PlayClip("Powerup");
                    GameManager.Instance.PoweredUp = true;
                }
            //NOT WORKING
                if (parent.childCount == 1)
                    Destroy(parent.gameObject);
                }
            }
        }

        shiftLines(lines);
    }

    /// <summary>
    /// Goes through all lines above the deleted lines inputted, and shifts them down to fill the space deleted
    /// </summary>
    /// <param name="linesDeleted"></param>
    public void shiftLines(List<int> linesDeleted)
    {
        //Sort deleted lines so method can run from the bottom of the playfield to the top
        //https://stackoverflow.com/questions/3738639/sorting-a-listint
        linesDeleted.Sort();
        int linesUnder = 1;
        //Starts from row above the lowest deleted to very top row
        for (int i = linesDeleted[0] + 1; i < boardGameObjects.Length; i++)
        {
            //Skip shifting line if line was deleted and shift lines above by one extra line, tracked by linesUnder
            if (linesUnder < 4 && linesDeleted.Contains(i))
            {
                linesUnder++;
                continue;
            }

            //Shifts the gameObjects' positions
            foreach (GameObject GO in boardGameObjects[i])
                if (GO != null)
                    GO.transform.position += new Vector3(0, linesUnder*-0.4f, 0);

            //Shift the references of the GameObjects in the jagged array
            //https://learn.microsoft.com/en-us/dotnet/api/system.array.copy?view=netcore-3.1#System_Array_Copy_System_Array_System_Int64_System_Array_System_Int64_System_Int64_
            //https://stackoverflow.com/questions/64461259/shifting-a-2d-array-in-c-sharp
            Array.Copy(boardGameObjects[i], 0, boardGameObjects[i - linesUnder], 0, 10);
            //Make the location of the original row all null
            Array.Copy(new GameObject[10], 0, boardGameObjects[i], 0, 10);
        }
    }

    /*Get all x indexes that need to be scanned
     * then go down in y index and scan all x indexes
     */
    /// <summary>
    /// Input children that need to be scanned (those that are exposed on the bottom);
    /// Go down 1 tile and check if any of the children overlap something
    /// Return how many tiles the mino can move down without obstruction
    /// </summary>
    /// <param name="minoChildren"></param>
    /// <returns></returns>
    public int distanceBelow(List<GameObject> minoChildren)
    {
        Dictionary<int, (GameObject, int)> xToScan = new();

        //Make a dictionary of (X index, tuple(childGO, Y index)) of all input children
        foreach (GameObject i in minoChildren)
        {
            (int, int) gridIndex = getGridIndex(i.transform.position);
            xToScan.Add(gridIndex.Item2, (i, gridIndex.Item1));
        }

        //start scanning one y unit below each cube of the player mino
        //if it is out of bounds or occupied, and return the distance to it in number of cubes
        for (int j = 1; true; j++)
        {
            foreach (int x in xToScan.Keys)
            {
                if (xToScan[x].Item2 - j < 0 || boardGameObjects[xToScan[x].Item2 - j][x] != null)
                    return j - 1;
            }
        }
    }


    /// <summary>
    /// Takes the location of the mino and the requested rotation, and attempt to find a valid rotation location based on SRS rotation table (References SRSWallKick class)
    /// </summary>
    /// <param name="childrenGO"></param>
    /// <param name="rot"></param>
    /// <param name="rotMode"></param>
    /// <returns></returns>
    public (bool, (int, int)) checkRotation(List<GameObject> childrenGO, int rot, (int, int) rotMode)
    {
        //Generate grid indexes of all 4 children after being rotated
        List<(int, int)> gridIndexesRotated = new();
        foreach (GameObject go in childrenGO)
        {
            gridIndexesRotated.Add(getGridIndex(applyZAxisRotationAboutParent(go, rot)));
        }

        //check if rotation is valid, and check all SRS wall kicks
        foreach ((int, int) wallKick in SRSWallKick.ReturnKickTable(childrenGO[0].transform.parent.gameObject.tag, rotMode))
        {
            if (checkTranslatedMino(wallKick, gridIndexesRotated))
                return (true, wallKick);
        }
        return (false, (0, 0)); //if no rotations are valid, fail rotation
    }

    /// <summary>
    /// Takes children and translates them by input index. Checks if the translation is valid
    /// </summary>
    /// <param name="translation"></param>
    /// <param name="indexes"></param>
    /// <returns></returns>
    bool checkTranslatedMino((int, int) translation, List<(int, int)> indexes)
    {
        foreach ((int, int) index in indexes) //check all 4 children
        {
            (int, int) indexToCheck = (index.Item1 + translation.Item2, index.Item2 + translation.Item1);

            if (indexToCheck.Item1 < 0 || indexToCheck.Item1 > 24)  //index under floor or above ceiling?
                return false;

            if (indexToCheck.Item2 < 0 || indexToCheck.Item2 > 9) //index past side walls?
                return false;

            if (boardGameObjects[indexToCheck.Item1][indexToCheck.Item2] != null) //index overlapping another object?
                return false;
        }
        return true; //if all 4 children pass all checks, valid translation
    }

    /// <summary>
    /// Takes a direction and a children to translate sideways, and checks if it can move in that direction (dir = 1 - move right, dir = -1 - move left)
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="childGO"></param>
    /// <returns></returns>
    public bool checkSideMovement(int dir, List<GameObject> childGO)
    {
        List<(int, int)> GOIndexes = new();
        
        //Get the indexes of all the children, and check if each child is not touching the walls
        switch (dir)
        {
            //If moving right
            case 1:
                foreach (GameObject GO in childGO)
                {
                    (int, int) originalIndex = getGridIndex(GO.transform.position);
                    //If touching right wall, return false
                    if (getGridIndex(GO.transform.position).Item2 == 9)
                        return false;
                    //Add child's index AFTER moving right to list
                    GOIndexes.Add((originalIndex.Item1, originalIndex.Item2 + dir));
                }
                break;
            //If moving left
            case -1:
                foreach (GameObject GO in childGO)
                {
                    (int, int) originalIndex = getGridIndex(GO.transform.position);
                    //If touching left wall, return false
                    if (getGridIndex(GO.transform.position).Item2 == 0)
                        return false;
                    //Add child's index AFTER moving left to list
                    GOIndexes.Add((originalIndex.Item1, originalIndex.Item2 + dir));
                }
                break;
        }

        //For each index after movement, check if the index is open in the array
        foreach ((int, int) index in GOIndexes)
        {
            if (boardGameObjects[index.Item1][index.Item2] != null)
                return false;
        }

        //If movement doesn't get blocked by neither the walls or other blocks, returns true
        return true;

    }

    /// <summary>
    /// Takes a child cube and a rotation, and applies the rotation to the child around the origin of the parent.
    /// Allows to calculate the position of the child after a rotation without actually rotating it
    /// </summary>
    /// <param name="GO"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public Vector3 applyZAxisRotationAboutParent(GameObject GO, int rot)
    {
        //https://answers.unity.com/questions/1751620/rotating-around-a-pivot-point-using-a-quaternion.html
        //Creates a quaternion of the roation, find the vector between the child and parent, apply the rotation to that vector, and add back the parent vector
        Quaternion rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        return rotation * (GO.transform.position - GO.transform.parent.position) + GO.transform.parent.position;
    }


    /// <summary>
    /// Checks if the provided children already overlap another child.
    /// Used to check for top out/GameOver
    /// </summary>
    /// <param name="Children"></param>
    /// <returns></returns>
    public bool CheckOverlap (List<GameObject> Children)
    {
        foreach (GameObject child in Children)
        {
            //Get indexes of the children
            (int, int) index = getGridIndex(child.transform.position);
            //If the index of the child is occupied, gameover
            if (BoardManagerJagged.Instance.boardGameObjects[index.Item1][index.Item2] != null)
                return true;
        }
        return false;
    }

    public bool checkTSpin(GameObject centerChild)
    {
        List<(int, int)> skews = new()
        {
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1)
        };
        List<(int, int)> indexesToCheck = new();
        int invalidIndexes = 0;
        (int, int) index = getGridIndex(centerChild.transform.position);

        //Find all 4 indexes to check, abort early if 2 is invalid
        foreach ((int, int) skew in skews)
        {
            //index to check is above floor
            if ((index.Item1 + skew.Item1) >= 0 && (index.Item2 + skew.Item2) <= 25)
            {
                //index to check is within left and right borders
                if ((index.Item2 + skew.Item2) >= 0 && (index.Item2 + skew.Item2) <= 9)
                    indexesToCheck.Add((index.Item1 + skew.Item1, index.Item2 + skew.Item2));
                else
                {
                    invalidIndexes++;
                    if (invalidIndexes == 2)
                    {
                        Debug.Log("2 invalid indexes, t-spin failed!");
                        return false;
                    }
                }
            }
            else
            {
                invalidIndexes++;
                if (invalidIndexes == 2)
                {
                    Debug.Log("2 invalid indexes, t-spin failed!");
                    return false;
                }
            }
        }

        //Debug.Log(indexesToCheck.Count);

        //Check each index. If 3 of them are filled, return true
        int indexesFilled = 0;
        foreach ((int, int) tindex in indexesToCheck)
        {
            //Debug.LogFormat("({0}, {1})", tindex.Item1, tindex.Item2);
            if (boardGameObjects[tindex.Item1][tindex.Item2] != null)
            {
                //Debug.Log("index filled");
                indexesFilled++;
                //Debug.Log(indexesFilled);
                if(indexesFilled == 3)
                {
                    //Debug.Log("T-Spin Successful!");
                    return true;
                }
            }
        }

        return false;
    }
}
