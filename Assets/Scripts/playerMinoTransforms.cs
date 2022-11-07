using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMinoTransforms : MonoBehaviour
{
    //Property for the rotation mode of the player mino
    private int _rotaMode = 0;
    public int RotaMode
    {
        get => _rotaMode;
        set
        {
            //Tunneling of the rotaMode to ensure it stays within 0-3
            //if it goes to 4, set to 0, if it goes to -1, go to 3
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

    //List of reference to all 4 children in the player mino
    private List<GameObject> childrenGO = new();
    //Reference to ghost piece, and list of children
    private GameObject ghostPiece;
    private List<GameObject> ghostPieceChildren = new();

    //Int for how many blocks under mino is open
    private int distanceUnder;

    //Gravity logic: int for gravity speed, and coroutine reference
    [SerializeField] int fallRate = 2;
    Coroutine gravityCoroutine;
    private WaitForSeconds delay;

    //Reference to coroutine for soft drop
    Coroutine softDropCoroutine;

    //move right and left coroutine, and booleans to determine which key is pressed down, and which coroutine is running
    //https://answers.unity.com/questions/300864/how-to-stop-a-co-routine-in-c-instantly.html
    Coroutine moveRightCoroutine;
    Coroutine moveLeftCoroutine;
    bool rightKeyDown;
    bool leftKeyDown;
    bool rightCoroutineOn = false;
    bool leftCoroutineOn = false;

    //Instantiate the list of references to the children of the player mino
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            childrenGO.Add(transform.GetChild(i).gameObject);
    }

    void Start()
    {
        //Check if spawned mino already overlaps with a mino, and cause a gameover
        if (BoardManagerJagged.Instance.CheckOverlap(childrenGO))
        {
            GameManager.Instance.Gameover = true;
            RestartManager.Instance.gameovered = true;
            Destroy(this);
        }

        /*For each rotation mode:
         * add all the far right child to the rightChildren dictionary (rotationMode, list of ref. to children)
         * add all the far left child to the leftChildren dictionary (rotationMode, list ref. to children)
         * add all the children with an exposed bottom to the bottomChildren dictionary (rotationMode, list ref. to children)
         * (these 3 need a temporary list which is then assigned to the dictionary)
         * add the lowest child to the lowestChild dictionary (rotationMode, ref. to child)
         * All accessed from dictionaries in rotaModes class
        */
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

        //Create a ghost piece identical to this mino
        ghostPiece = Instantiate(GameManager.Instance.minoPrefabDict[this.tag], transform.position, Quaternion.identity, GameManager.Instance.minoParent.transform);
        //Add references to all children of the ghost piece into the ghostPieceChildren list
        for (int i = 0; i < ghostPiece.transform.childCount; i++)
        {
            ghostPieceChildren.Add(ghostPiece.transform.GetChild(i).gameObject);
        }
        //Set the spirte renderer of each child of the ghost piece to be 0.2 opacity
        //https://forum.unity.com/threads/unity-4-3-how-to-change-the-opacity-of-a-2d-sprite.223146/
        foreach (GameObject child in ghostPieceChildren)
        {
            SpriteRenderer render = child.GetComponent<SpriteRenderer>();
            render.color = new Color(1f, 1f, 1f, 0.2f);
        }
        //Initialization of gravity or softDrop Coroutine
        //instantiate delay of fallRate
        delay = new WaitForSeconds(fallRate);
        //Find the distance under player (req. for gravity and softDrop)
        updateDistanceUnder();
        //If softdrop was still on when last piece was placed, start this piece with softDrop on
        if (GameManager.Instance.softDropOn)
            softDropCoroutine = StartCoroutine(SoftDrop());
        //if not start with simple gravity
        else
            gravityCoroutine = StartCoroutine(Gravity());

        //Initialize right and left key down to match its current physical state
        rightKeyDown = Input.GetKey(GameManager.Instance.Right);
        leftKeyDown = Input.GetKey(GameManager.Instance.Left);

        //If both keys are held down, ignore
        if (rightKeyDown && leftKeyDown)
            return;
        //If right key is down already, start moving right
        else if (rightKeyDown)
        {
            moveRightCoroutine = StartCoroutine(MoveSide(1));
            rightCoroutineOn = true;
        }
        //If left key "    "    "   ,   "       "   left
        else if (leftKeyDown)
        {
            moveLeftCoroutine = StartCoroutine(MoveSide(-1));
            leftCoroutineOn = true;
        }

        //Move Ghost Piece to proper placement
        updateGhostPiece();
    }

    /// <summary>
    /// Update runs every frame
    /// One big if else if for every single key input
    /// </summary>
    void Update()
    {
        //If left key is down, stop moveRight Coroutine if it is running, and start moveLeft Coroutine
        if (Input.GetKeyDown(GameManager.Instance.Left))
        {
            leftKeyDown = true;
            if (moveRightCoroutine != null)
            {
                StopCoroutine(moveRightCoroutine);
                rightCoroutineOn = false;
            }
            moveLeftCoroutine = StartCoroutine(MoveSide(-1));
            leftCoroutineOn = true;
        }
        //If left key is lifted, stop moveLeft Coroutine if running, and start moving right if rightKey is down and not moving right already
        else if (Input.GetKeyUp(GameManager.Instance.Left))
        {
            leftKeyDown = false;
            if(leftCoroutineOn)
            {
                StopCoroutine(moveLeftCoroutine);
                leftCoroutineOn = false;
            }
            if (rightKeyDown && !rightCoroutineOn)
            {
                moveRightCoroutine = StartCoroutine(MoveSide(1));
                rightCoroutineOn = true;
            }
        }

        //If right key is down, stop moveleft Coroutine if its running, and start moveRight Coroutine
        if (Input.GetKeyDown(GameManager.Instance.Right))
        {
            rightKeyDown = true;
            if (moveLeftCoroutine != null)
            {
                StopCoroutine(moveLeftCoroutine);
                leftCoroutineOn = false;
            }
            moveRightCoroutine = StartCoroutine(MoveSide(1));
            rightCoroutineOn = true;
        }
        //If right key is lifted, stop moveRight Coroutine if running, and start moving left if leftKey is down and not moving left already
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
                moveLeftCoroutine = StartCoroutine(MoveSide(-1));
                leftCoroutineOn = true;
            }
        }

        //When rotate right key is pressed
        if (Input.GetKeyDown(GameManager.Instance.rotRight))
        {
            //check if rotation is available, and where the piece should be translated after the rotaion based on SRS
            (bool, (int, int)) result = BoardManagerJagged.Instance.checkRotation(childrenGO, -90, (RotaMode, rotaModeTunneling(RotaMode + 1)));
            //if a rotation is available
            if (result.Item1)
            {
                //update rotaMode, rotate and translate player mino, update distance under
                RotaMode += 1;
                transform.rotation = Quaternion.Euler(0, 0, -90 * RotaMode);
                transform.position += new Vector3(0.4f * result.Item2.Item1, 0.4f * result.Item2.Item2, 0);
                updateDistanceUnder();
                //restart the softDrop if rotated, which restarts the timer for auto placing
                if(!GameManager.Instance.softDropOn)
                    restartCoroutine();
                //match the rotation and position of the ghost piece
                updateGhostPiece();
            }
        }
        //Same logic as above, for opposite rotation
        else if (Input.GetKeyDown(GameManager.Instance.rotLeft))
        {
            (bool, (int,int)) result = BoardManagerJagged.Instance.checkRotation(childrenGO, 90, (RotaMode, rotaModeTunneling(RotaMode - 1)));
            if (result.Item1)
            {
                RotaMode -= 1;
                transform.rotation = Quaternion.Euler(0, 0, -90 * RotaMode);
                transform.position += new Vector3(0.4f * result.Item2.Item1, 0.4f * result.Item2.Item2, 0);
                updateDistanceUnder();
                if (!GameManager.Instance.softDropOn)
                    restartCoroutine();
                updateGhostPiece();
            }
        }
        
        //If softDrop/Down key is pressed
        if(Input.GetKeyDown(GameManager.Instance.SoftDrop))
        {
            //If not touching the ground, stop gravity and start softDrop Coroutine, and set global softDrop to true
            if (distanceUnder > 0)
            {
                StopCoroutine(gravityCoroutine);
                softDropCoroutine = StartCoroutine(SoftDrop());
                GameManager.Instance.softDropOn = true;
            }
            //If touching the ground, place the mino
            else
            {
                placeMino();
            }
        }
        //If softDrop/Down key is lifted
        else if (Input.GetKeyUp(GameManager.Instance.SoftDrop))
        {
            //If softDropCoroutine is running, stop the softDrop Coroutine if running, and start gravity
            if (softDropCoroutine != null)
            {
                StopCoroutine(softDropCoroutine);
                gravityCoroutine = StartCoroutine(Gravity());
            }
            //Set global softDrop to false
            GameManager.Instance.softDropOn = false; ;
        }

        //If hardDrop key is pressed, update distanceUnder, move playerMino to the ground according to distanceUnder, and place the mino
        if (Input.GetKeyDown(GameManager.Instance.HardDrop))
        {
            updateDistanceUnder();
            transform.position += new Vector3(0, distanceUnder * -0.4f, 0);
            placeMino();
        }
    }
    /// <summary>
    /// Stop gravityCoroutine and start it again
    /// </summary>
    void restartCoroutine()
    {
        StopCoroutine(gravityCoroutine);
        gravityCoroutine = StartCoroutine(Gravity());
    }

    /// <summary>
    /// every (int delay) seconds, move mino down one block if distance under is greater than 0
    /// place mino if mino is touching the ground
    /// </summary>
    /// <returns></returns>
    IEnumerator Gravity()
    {
        yield return delay;
        //Move mino down one block as long as there is space under mino
        while (distanceUnder > 0)
        {
            transform.position += new Vector3(0, -0.4f, 0);
            distanceUnder--;
            yield return delay;
        }
        placeMino();
        yield break;
    }

    /// <summary>
    /// Instantly move mino down one block, and wait 0.25 seconds;
    /// Then start moving the mino down every 0.03 seconds;
    /// Places mino after mino doesn't move for 20 attempts
    /// </summary>
    /// <returns></returns>
    IEnumerator SoftDrop()
    {
        //instantly move mino down one tile
        MoveDown();
        int failedMoves = 0;
        yield return new WaitForSeconds(0.25f);
        //Do this forever every 0.03 seconds
        while (true)
        {
            //Attempt to move down one tile, and set failedMoves to 0 if successful
            if (MoveDown())
                failedMoves = 0;
            //If failed, increment failedMoves counter, and place mino if it was the 21st failed attempt
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

    /// <summary>
    /// Input dir, where 1 = right, -1 = left;
    /// Moves mino to the side, waits 0.2 sec, then moves every 0.05 sec
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    IEnumerator MoveSide(int dir)
    {
        switch (dir)
        {
            case 1:
                MoveToRight();
                break;
            case -1:
                MoveToLeft();
                break;
        }
        yield return new WaitForSeconds(0.2f);
        while (true)
        {
            switch (dir)
            {
                case 1:
                    MoveToRight();
                    break;
                case -1:
                    MoveToLeft();
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Check if mino can move right;
    /// If ok, move mino, update distanceUnder and ghostPiece position
    /// </summary>
    void MoveToRight ()
    {
        if (BoardManagerJagged.Instance.checkSideMovement(1, rightChildren[RotaMode]))
        {
            transform.position += new Vector3(0.4f, 0, 0);
            updateDistanceUnder();
            updateGhostPiece();
        }
    }
    
    /// <summary>
    /// Check if mino can move left;
    /// If ok, move mino, update distanceUnder and ghostPiece position
    /// </summary>
    void MoveToLeft ()
    {
        if (BoardManagerJagged.Instance.checkSideMovement(-1, leftChildren[RotaMode]))
        {
            transform.position += new Vector3(-0.4f, 0, 0);
            updateDistanceUnder();
            updateGhostPiece();
        }
    }

    /// <summary>
    /// Checks distanceUnder;
    /// If mino cannot move down, returns false;
    /// If mino can move down, moves the mino, updates distance under, and returns true
    /// </summary>
    /// <returns></returns>
    bool MoveDown()
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

    /// <summary>
    /// Fetches from BoardManagerJagged class how many tiles are open under mino, and assigns it to the distanceUnder variable
    /// </summary>
    void updateDistanceUnder()
    {
        distanceUnder = BoardManagerJagged.Instance.distanceBelow(bottomChildren[RotaMode]);
    }

    /// <summary>
    /// Matches the rotation of the ghost mino to that of the player mino, and moves position to the lowest position available
    /// </summary>
    void updateGhostPiece()
    {
        ghostPiece.transform.rotation = transform.rotation;
        ghostPiece.transform.position = transform.position + new Vector3(0, distanceUnder * -0.4f, 0);
    }

    /// <summary>
    /// Calls methods in GameManager and BoardManagerJagged class for placing a mino
    /// Calls method to end the script
    /// </summary>
    void placeMino()
    {
        GameManager.Instance.piecePlaced();
        BoardManagerJagged.Instance.placeInBoard(childrenGO);
        endScript();
    }

    /// <summary>
    /// Destroys the ghost piece, and deletes this script component
    /// </summary>
    public void endScript()
    {
        Destroy(ghostPiece);
        Destroy(this);
    }

    /// <summary>
    /// Same logic as set property of the RotaModes variable
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
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