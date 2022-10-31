using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMinoTransforms : MonoBehaviour
{
    //public bool leftPressed;
    //public bool rightPressed;
    //public bool testPressed;

    private int _rotaMode = 0;

    public int rotaMode
    {
        get => _rotaMode;
        set
        {
            switch (value)
            {
                case 4:
                    _rotaMode = 0;
                    break;
                case -1:
                    _rotaMode = 3;
                    break;
                default:
                    _rotaMode = value;
                    break;
            }
        }
    }

    //GameObject Assignments Based on rotaModes class
    private Dictionary<int, List<GameObject>> rightChildren = new();
    private Dictionary<int, List<GameObject>> leftChildren = new();
    private Dictionary<int, List<GameObject>> bottomChildren = new();
    private Dictionary<int, GameObject> lowestChild = new();

    public List<GameObject> childrenGO = new();
    public GameObject ghostPiece;
    public List<GameObject> ghostPieceChildren = new();

    private int distanceUnder;
    public int fallRate = 2;
    Coroutine gravityCoroutine;
    private WaitForSeconds delay;

    Coroutine softDropCoroutine;

    Coroutine moveRightCoroutine;
    Coroutine moveLeftCoroutine;
    bool rightKeyDown;
    bool leftKeyDown;
    bool rightCoroutineOn;
    bool leftCoroutineOn;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            childrenGO.Add(transform.GetChild(i).gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Check if spawned mino already overlaps
        foreach (GameObject child in childrenGO)
        {
            (int, int) index = BoardManagerJagged.Instance.getGridIndex(child.transform.position);
            if(BoardManagerJagged.Instance.boardGameObjects[index.Item1][index.Item2] != null)
            {
                GameManager.Instance.gameover = true;
                RestartManager.Instance.gameovered = true;
                Destroy(this);
            }
        }

        //add the leftmost and rightmost child gameobject of each orientation
        //then add the child gameobjects that have an exposed bottom
        for (int i = 0; i < 4; i++)
        {
            //leftRightChild dictionary assignment
            List<GameObject> rightChildrenGO = new();
            foreach (int j in rotaModes.ReturnRightChildren(this.tag, i))
                rightChildrenGO.Add(childrenGO[j]);
            rightChildren.Add(i, rightChildrenGO);

            List<GameObject> leftChildrenGO = new();
            foreach (int j in rotaModes.ReturnLeftChildren(this.tag, i))
                leftChildrenGO.Add(childrenGO[j]);
            leftChildren.Add(i, leftChildrenGO);

            //bottomChild dictionary assignment
            List<GameObject> bottomChildrenGO = new();
            foreach (int j in rotaModes.ReturnBottomChildren(this.tag, i))
                bottomChildrenGO.Add(childrenGO[j]);
            bottomChildren.Add(i, bottomChildrenGO);

            //lowestChild dictionary assignment
            lowestChild.Add(i, childrenGO[rotaModes.ReturnLowestChild(this.tag, i)]);
        }

        ghostPiece = Instantiate(GameManager.Instance.minoPrefabDict[this.tag], transform.position, Quaternion.identity, GameManager.Instance.minoParent.transform);
        for (int i = 0; i < ghostPiece.transform.childCount; i++)
        {
            ghostPieceChildren.Add(ghostPiece.transform.GetChild(i).gameObject);
        }

        foreach (GameObject child in ghostPieceChildren)
        {
            SpriteRenderer render = child.GetComponent<SpriteRenderer>();
            render.color = new Color(1f, 1f, 1f, 0.2f);
        }

        delay = new WaitForSeconds(fallRate);
        updateDistanceUnder();

        //.Log(GameManager.Instance.softDropOn);
        if (GameManager.Instance.softDropOn)
        {
            softDropCoroutine = StartCoroutine(softDrop());
        }
        else
        {
            gravityCoroutine = StartCoroutine(gravity());
        }

        updateGhostPiece();

        //make mino fall every {fallRate} seconds
        //InvokeRepeating("fallDown", fallRate, fallRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.Left))
        {
            leftKeyDown = true;
            if (moveRightCoroutine != null)
            {
                StopCoroutine(moveRightCoroutine);
                rightCoroutineOn = false;
            }
            moveLeftCoroutine = StartCoroutine(moveSide(-1));
            leftCoroutineOn = true;
        }
        else if (Input.GetKeyUp(GameManager.Instance.Left))
        {
            leftKeyDown = false;
            if(moveLeftCoroutine != null)
            {
                StopCoroutine(moveLeftCoroutine);
                leftCoroutineOn = false;
            }
            if (rightKeyDown && !rightCoroutineOn)
            {
                moveRightCoroutine = StartCoroutine(moveSide(1));
                rightCoroutineOn = true;
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.Right))
        {
            rightKeyDown = true;
            if (moveLeftCoroutine != null)
            {
                StopCoroutine(moveLeftCoroutine);
                leftCoroutineOn = false;
            }
            moveRightCoroutine = StartCoroutine(moveSide(1));
            rightCoroutineOn = true;
        }
        else if (Input.GetKeyUp(GameManager.Instance.Right))
        {
            rightKeyDown = false;
            if (moveRightCoroutine != null)
            {
                StopCoroutine(moveRightCoroutine);
                rightCoroutineOn = false;
            }
            if (leftKeyDown && !leftCoroutineOn)
            {
                moveLeftCoroutine = StartCoroutine(moveSide(-1));
                leftCoroutineOn = true;
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.rotRight))
        {
            (bool, (int, int)) result = BoardManagerJagged.Instance.checkRotation(childrenGO, -90, (rotaMode, rotaModeTunneling(rotaMode + 1)));
            if (result.Item1)
            {
                rotaMode += 1;
                transform.rotation = Quaternion.Euler(0, 0, -90 * rotaMode);
                transform.position += new Vector3(0.4f * result.Item2.Item1, 0.4f * result.Item2.Item2, 0);
                updateDistanceUnder();
                if(!GameManager.Instance.softDropOn)
                    restartCoroutine();
                updateGhostPiece();
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.rotLeft))
        {
            //List<GameObject> childrenGO, Vector3 rot, (int, int) rotMode
            (bool, (int,int)) result = BoardManagerJagged.Instance.checkRotation(childrenGO, 90, (rotaMode, rotaModeTunneling(rotaMode - 1)));
            if (result.Item1)
            {
                rotaMode -= 1;
                transform.rotation = Quaternion.Euler(0, 0, -90 * rotaMode);
                transform.position += new Vector3(0.4f * result.Item2.Item1, 0.4f * result.Item2.Item2, 0);
                updateDistanceUnder();
                if (!GameManager.Instance.softDropOn)
                    restartCoroutine();
                updateGhostPiece();
            }
        }
        else if(Input.GetKeyDown(GameManager.Instance.SoftDrop))
        {
            if (distanceUnder > 0)
            {
                StopCoroutine(gravityCoroutine);
                softDropCoroutine = StartCoroutine(softDrop());
                GameManager.Instance.softDropOn = true;
            }
            else
            {
                placeMino();
            }
        }
        else if (Input.GetKeyUp(GameManager.Instance.SoftDrop))
        {
            if (softDropCoroutine != null)
            {
                StopCoroutine(softDropCoroutine);
                gravityCoroutine = StartCoroutine(gravity());
                GameManager.Instance.softDropOn = false;
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.HardDrop))
        {
            int blocksToFloor = BoardManagerJagged.Instance.distanceBelow(bottomChildren[rotaMode]);
            transform.position += new Vector3(0, blocksToFloor * -0.4f, 0);
            placeMino();
        }
    }

    void restartCoroutine()
    {
        StopCoroutine(gravityCoroutine);
        gravityCoroutine = StartCoroutine(gravity());
    }

    IEnumerator gravity ()
    {
        yield return delay;

        while (distanceUnder > 0)
        {
            transform.position += new Vector3(0, -0.4f, 0);
            distanceUnder--;
            //Debug.Log("falling...");
            yield return delay;
        }
        placeMino();
        yield break;
    }

    IEnumerator softDrop()
    {
        moveDown();
        int failedMoves = 0;
        yield return new WaitForSeconds(0.25f);
        while (true)
        {
            if (moveDown())
            {
                failedMoves = 0;
            }
            else
            {
                failedMoves++;
                if (failedMoves > 20)
                {
                    placeMino();
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator moveSide(int dir)
    {
        switch (dir)
        {
            case 1:
                moveToRight();
                break;
            case -1:
                moveToLeft();
                break;
        }
        yield return new WaitForSeconds(0.2f);
        while (true)
        {
            switch (dir)
            {
                case 1:
                    moveToRight();
                    break;
                case -1:
                    moveToLeft();
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void moveToRight ()
    {
        if (BoardManagerJagged.Instance.checkSideMovement(1, rightChildren[rotaMode]))
        {
            transform.position += new Vector3(0.4f, 0, 0);
            updateDistanceUnder();
            updateGhostPiece();
        }
    }
    
    void moveToLeft ()
    {
        if (BoardManagerJagged.Instance.checkSideMovement(-1, leftChildren[rotaMode]))
        {
            transform.position += new Vector3(-0.4f, 0, 0);
            updateDistanceUnder();
            updateGhostPiece();
        }
    }

    bool moveDown()
    {
        if (distanceUnder <= 0)
        {
            return false;
        }
        else
        {
            transform.position += new Vector3(0, -0.4f, 0);
            distanceUnder--;
            return true;
        }
    }

    void updateDistanceUnder()
    {
        distanceUnder = BoardManagerJagged.Instance.distanceBelow(bottomChildren[rotaMode]);
    }

    void updateGhostPiece()
    {
        ghostPiece.transform.rotation = transform.rotation;
        ghostPiece.transform.position = transform.position + new Vector3(0, distanceUnder * -0.4f, 0);
    }

    void placeMino()
    {
        //StopCoroutine(gravityCoroutine);
        GameManager.Instance.piecePlaced();
        BoardManagerJagged.Instance.placeInBoard(childrenGO);
        endScript();
    }

    public void endScript()
    {
        Destroy(ghostPiece);
        Destroy(this);
    }

    int rotaModeTunneling(int num)
    {
        switch (num)
        {
            case 4:
                return 0;
            case -1:
                return 3;
            default:
                return num;
        }
    }
}



//int maxIndex = 0;
//float maxX = childrenGO[0].transform.position.x;
//for (int i = 0; i < childrenGO.Count; i++)
//{
//    GameObject k = childrenGO[i];
//    if (k.transform.position.x > maxX)
//    {
//        maxIndex = i;
//        maxX = k.transform.position.x;
//    }
//}