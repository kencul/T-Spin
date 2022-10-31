using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class BoardManagerJagged : MonoBehaviour
{
    /*public KeyCode arrayDebug;
    public GameObject testPrefab;
    public GameObject testParent;

    public TextMeshProUGUI debugText;

    private string debugString;*/

    public static BoardManagerJagged Instance;

    //[y, x] ***IMPORTANT & CONFUSING***, AXES FLIPPED FOR LINECLEARING LOGIC
    //Bottom left of board = [0,0]
    //one square = 0.4f, 0.4f
    //Bottom of board = -3.8f
    //Far Left of board = -1.8f

    public GameObject[][] boardGameObjects = new GameObject[25][];

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
        //Debug.Log(boardGameObjects.Length);
        //Instantiate(testPrefab, new Vector3(0.4f - 3.8f, 0.4f - 1.8f, 0), Quaternion.identity, testParent.transform);

        //Init jagged array
        for (int i = 0; i < boardGameObjects.Length; i++)
        {
            boardGameObjects[i] = new GameObject[10];
        }
    }

    void Update()
    {
        /*for (int i = 0; i < testParent.transform.childCount; i++)
            Destroy(testParent.transform.GetChild(i).gameObject);*/

        /*if (Input.GetKeyDown(arrayDebug))
        {
            *//*Debug.Log(boardGameObjects.GetLength(0));
            Debug.Log(boardGameObjects.GetLength(1));*//*
            Debug.Log("running debug");
            for(int i = 0; i < boardGameObjects.GetLength(0); i++)
            {
                for(int j = 0; j < boardGameObjects.GetLength(1); j++)
                {
                    if (boardGameObjects[i, j] != null)
                    {
                        debugString += "- ";
                        //Debug.Log("array index filled");
                    }
                    else if (boardGameObjects[i,j] == null)
                    {
                        debugString += "_ ";
                    }
                    Debug.Log(j);
                }
                debugString += "</indent>";
            }
            debugText.text = debugString;
            debugString = "";
        }*/
    }

    //takes a transform.position and returns the (y,x) index of the playfield
    public (int, int) getGridIndex(Vector3 pos)
    {
        int column = (int)(Mathf.Round((pos.x + 1.8f) / 0.4f));
        int row = (int)(Mathf.Round((pos.y + 3.8f) / 0.4f));
        return (row, column);
    }



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

        //Debug.Log(rowsChanged.Count);

        //Check if any of the modified rows are completed
        foreach (int row in rowsChanged)
        {
            if (checkIfLineFilled(row))
                filledLines.Add(row);
        }

        //Delete Cubes of completed lines
        if (filledLines.Any())
            deleteLines(filledLines);
    }

    public bool checkIfLineFilled(int row)
    {
        for (int j = 9; j >= 0; j--)
        {
            if (boardGameObjects[row][j] == null)
            {
                //Debug.Log("no lines filled");
                return false;
            }
        }
        return true;
    }

    public void deleteLines(List<int> lines)
    {
        foreach (int line in lines)
        {
            List<Transform> parents = new();
            GameObject[] toClear = Enumerable.Range(0, 10)
                 .Select(x => boardGameObjects[line][x])
                 .ToArray();

            foreach (GameObject GO in toClear)
            {
                parents.Add(GO.transform.parent);
                Destroy(GO);
            }

            foreach (Transform parent in parents)
            {
                if (parent.childCount == 0)
                {
                    Debug.Log("destroying parent...");
                    Destroy(parent.gameObject);
                }
            }

        }

        shiftLines(lines);
    }

    public void shiftLines(List<int> linesDeleted)
    {
        linesDeleted.Sort();
        int linesUnder = 1;
        for (int i = linesDeleted[0] + 1; i < boardGameObjects.Length; i++)
        {
            //Skip shifting line if line was deleted and shift lines by one more from next time
            if (linesUnder < 4 && linesDeleted.Contains(i))
            {
                linesUnder++;
                continue;
            }

            foreach (GameObject GO in boardGameObjects[i])
                if (GO != null)
                    GO.transform.position += new Vector3(0, linesUnder*-0.4f, 0);

            Array.Copy(boardGameObjects[i], 0, boardGameObjects[i - linesUnder], 0, 10);
            Array.Copy(new GameObject[10], 0, boardGameObjects[i], 0, 10);

            /*GameObject[] toMove = Enumerable.Range(0, 10)
                 .Select(x => boardGameObjects[i][x])
                 .ToArray();
            Array.Copy(toMove, 0, boardGameObjects, 20 * i - 10 * linesUnder, 10);*/
        }
    }

    /*public void clearLine(int row)
    {
        Debug.Log("clearing line " + row);
        //Get one ror of the multidimensional array as an array
        //https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp/51241629#51241629
        GameObject[] toClear = Enumerable.Range(0, 10)
                 .Select(x => boardGameObjects[row][x])
                 .ToArray();
        foreach (GameObject GO in toClear)
        {
            Destroy(GO);
        }

        Enumerable.Range(0, 10).Select(x => boardGameObjects[row][x] = null);


        //Translate down all children above cleared line
        int[] rowsToMove = Enumerable.Range(row, 25 - row - 1)
                 .ToArray();
        foreach (int floatingRow in rowsToMove)
        {
            GameObject[] toMove = Enumerable.Range(0, 10)
                     .Select(x => boardGameObjects[floatingRow][x])
                     .ToArray();

            foreach (GameObject GO in boardGameObjects[floatingRow])
                if (GO != null)
                    GO.transform.position += new Vector3(0, -0.4f, 0);
        }

        //Copy the rows above the cleared line in the jagged array down
        Array.Copy(boardGameObjects[boardGameObjects, 10, boardGameObjects, 0, 10 * 24);
        Array.Copy(new GameObject[10, 1], 0, boardGameObjects, 24 * 10, 10);
        return;
    }*/

    /*Get all x indexes that need to be scanned
     * then go down in y index and scan all x indexes
     */
    public int distanceBelow(List<GameObject> minoChildren)
    {
        Dictionary<int, (GameObject, int)> xToScan = new();

        //Make a dictionary of (X index, tuple(childGO, Y index)), and the highest row that must be checked
        foreach (GameObject i in minoChildren)
        {
            (int, int) gridIndex = getGridIndex(i.transform.position);
            xToScan.Add(gridIndex.Item2, (i, gridIndex.Item1));
        }

        //start scanning one y unit below each cube of the player mino if it is out of bounds or occupied, and return the distance to it in number of cubes
        for (int j = 1; true; j++)
        {
            foreach (int x in xToScan.Keys)
            {
                if (xToScan[x].Item2 - j < 0 || boardGameObjects[xToScan[x].Item2 - j][x] != null)
                    return j - 1;
            }
        }
    }

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
            {
                return (true, wallKick);
            }
        }
        return (false, (0, 0)); //if no rotations are valid, fail rotation
    }

    bool checkTranslatedMino((int, int) translation, List<(int, int)> indexes)
    {
        foreach ((int, int) index in indexes) //check all 4 children
        {
            (int, int) indexToCheck = (index.Item1 + translation.Item2, index.Item2 + translation.Item1);
            /*if (indexToCheck.Item1 < 0 || indexToCheck.Item1 > 24 ||  //index under floor or above ceiling?
                indexToCheck.Item2 < 0 || indexToCheck.Item2 > 9 || //index past side walls?
                boardGameObjects[indexToCheck.Item1, indexToCheck.Item2] != null) //index overlapping?
            {
                return false; //if child matches any, wallKick fails
            }*/
            if (indexToCheck.Item1 < 0 || indexToCheck.Item1 > 24)  //index under floor or above ceiling?
            {
                //Debug.Log("Too high or low");
                return false;
            }

            if (indexToCheck.Item2 < 0 || indexToCheck.Item2 > 9) //index past side walls?
            {
                //Debug.Log("Too far to the right of left");
                return false;
            }

            if (boardGameObjects[indexToCheck.Item1][indexToCheck.Item2] != null) //index overlapping?
            {
                //Debug.Log("Overlapping a block");
                return false; //if child matches any, wallKick fails
            }
        }
        return true; //if all 4 children pass, valid wallKick
    }

    public bool checkSideMovement(int dir, List<GameObject> childGO)
    {
        List<(int, int)> GOIndexes = new();
        foreach (GameObject GO in childGO)
        {
            (int, int) originalIndex = getGridIndex(GO.transform.position);
            //Debug.Log(originalIndex.Item2);
            if (originalIndex.Item2 + dir < 0 || originalIndex.Item2 + dir > 9)
                return false;
            GOIndexes.Add((originalIndex.Item1, originalIndex.Item2 + dir));
        }

        foreach ((int, int) index in GOIndexes)
        {
            if (boardGameObjects[index.Item1][index.Item2] != null)
                return false;
        }

        return true;

    }

    public Vector3 applyZAxisRotationAboutParent(GameObject GO, int rot)
    {
        Quaternion rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        return rotation * (GO.transform.position - GO.transform.parent.position) + GO.transform.parent.position;
        //return theoreticalRotation;
    }
}
