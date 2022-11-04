using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    //RNG Generator
    private string currentPiece;
    List<string> Pieces = new() { "i", "o", "t", "l", "j", "s", "z" };
    List<string> RNGPieces = new();
    List<string> nextMinos = new();
    private int pieceItr = 0;

    //Hold Logic
    private string heldPiece = "";
    private bool holdOK = true;

    //Bool to enable Soft Drop inbetween piece drops (used in playerMinoTransforms)
    public bool softDropOn = false;

    //Keybinds
    public KeyCode HardDrop;
    public KeyCode SoftDrop;
    public KeyCode Hold;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode rotRight;
    public KeyCode rotLeft;

    //Mino Prefab References
    [SerializeField] GameObject IMino;
    [SerializeField] GameObject OMino;
    [SerializeField] GameObject SMino;
    [SerializeField] GameObject ZMino;
    [SerializeField] GameObject LMino;
    [SerializeField] GameObject JMino;
    [SerializeField] GameObject TMino;
    public Dictionary<string, GameObject> minoPrefabDict = new();

    //References to visual GameObjects
    private GameObject playerMino;
    private GameObject holdMino;
    private readonly GameObject nextMino1;
    private readonly GameObject nextMino2;
    private readonly GameObject nextMino3;
    private readonly GameObject nextMino4;
    private readonly GameObject nextMino5;
    List<GameObject> nextMinoList;

    //reference to the current player's script
    playerMinoTransforms playerScript;

    //Empty GameObject to group minos
    public GameObject minoParent;

    //Dictionary to adjust the spawn location of each mino, as the center of the prefabs are different for each
    Dictionary<string, Vector3> spawnLocation = new()
    {
        { "i", new Vector3(0, 4.0f, 0) },
        { "o", new Vector3(0, 4.4f, 0) },
        { "t", new Vector3(-0.2f, 4.2f, 0) },
        { "l", new Vector3(-0.2f, 4.2f, 0) },
        { "j", new Vector3(-0.2f, 4.2f, 0) },
        { "z", new Vector3(-0.2f, 4.2f, 0) },
        { "s", new Vector3(-0.2f, 4.2f, 0) }
    };

    //Dictionary to center all the minos in order so the hold and next minos won't overlap
    Dictionary<string, Vector3> centerMino = new()
    {
        { "i", new Vector3(0, -0.2f, 0) },
        { "o", new Vector3(0, 0, 0) },
        { "t", new Vector3(0, -0.2f, 0) },
        { "l", new Vector3(0, -0.2f, 0) },
        { "j", new Vector3(0, -0.2f, 0) },
        { "z", new Vector3(0, -0.2f, 0) },
        { "s", new Vector3(0, -0.2f, 0) }
    };


    //Property for gameover
    private bool _gameover = false;
    //CAPTALIZE
    public bool Gameover
    {
        get => _gameover;
        set
        {
            if (value)
            {
                gameoverProcess();
            }
        }
    }
    public TextMeshProUGUI gameoverText;

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
        //Instantite the tempPrefabDictionary
        Dictionary<string, GameObject> tempPrefabDict = new()
        {
            { "i", IMino },
            { "o", OMino },
            { "t", TMino },
            { "l", LMino },
            { "j", JMino },
            { "s", SMino },
            { "z", ZMino }
        };
        //use tempPrefabDictionary to instantiate minoPrefabDict
        //this needs to be done as I assign references to gameObjects
        foreach (KeyValuePair<string, GameObject> kvp in tempPrefabDict)
            minoPrefabDict.Add(kvp.Key, kvp.Value);

        //Instantiate list of next Mino GameObjects
        nextMinoList = new() { nextMino1, nextMino2, nextMino3, nextMino4, nextMino5 };
    }

    //Method to call to start the game: run RNG, generate next pieces, and spawn minos\
    public void initGame()
    {
        SevenBagPRG();
        NextPiece();
        updateMinos();
    }

    // Update is called once per frame
    void Update()
    {
        //Hold Piece logic
        //If hold key pressed and hold available, hold the piece and disable holding for the turn
        if (Input.GetKeyDown(Hold) && holdOK)
        {
            //If there is a mino already held, swap the current and held piece and delete the player
            if (heldPiece != "")
            {
                (heldPiece, currentPiece) = (currentPiece, heldPiece);
                deletePlayerMino();
            }
            //If there is no piece held, assign the current piece as held, get the next mino and destroy the mino
            else
            {
                heldPiece = currentPiece;
                NextPiece();
                deletePlayerMino();
            }
            //Reload minos and disable holding for this turn
            updateMinos();
            holdOK = false;
        }
    }

    //Method to end the game, shows text for gameover and destroys this script
    void gameoverProcess()
    {
        gameoverText.text = "GAMEOVER </indent> PRESS SPACE TO RESTART";
        Destroy(this);
    }

    //Method to place a piece, gets the next mino and updates all minos on screen
    public void piecePlaced()
    {
        NextPiece();
        updateMinos();
        holdOK = true;
    }

    //Delete Player Controlled mino (used for holding a mino)
    public void deletePlayerMino()
    {
        if (playerMino != null)
        {
            playerScript.endScript();
            Destroy(playerMino);
        }
    }

    //Method to reload minos
    public void updateMinos()
    {
        //Spawn Player Mino with script
        playerMino = Instantiate(minoPrefabDict[currentPiece], spawnLocation[currentPiece], Quaternion.identity, minoParent.transform);
        playerScript = playerMino.AddComponent<playerMinoTransforms>();

        //Spawn Hold Mino
        if (heldPiece != "" && holdMino != minoPrefabDict[heldPiece])
        {
            if (holdMino != null)
            {
                Destroy(holdMino);
            }
            holdMino = Instantiate(minoPrefabDict[heldPiece], new Vector3(-3.08f, 2.6f, 0)+centerMino[heldPiece], Quaternion.identity, minoParent.transform);
        }

        //Spawn Next Minos
        if (nextMino1 != minoPrefabDict[nextMinos[0]] && nextMino2 != minoPrefabDict[nextMinos[1]])
        {
            for (int i=0; i < nextMinoList.Count; i++)
            {
                if (nextMinoList[i] != null)
                    Destroy(nextMinoList[i]);
                //Spawn iteration of next minos skewed
                nextMinoList[i] = Instantiate(minoPrefabDict[nextMinos[i]], new Vector3(3.06f, 2.97f-.9f*i, 0)+centerMino[nextMinos[i]], Quaternion.identity, minoParent.transform);
            }
        }
    }

    //Generate the 5 next pieces, based on 7-bag rng
    //7-bag rng: ensures all 7 minos appear before a mino comes up again
    public void NextPiece()
    {
        //Create queue of next 5 minos
        while (nextMinos.Count < 6)
        {
            int i = pieceItr;
            pieceItr += 1;
            //If this method still hasn't used all of the 7-pieces generated, get the next piece
            if (i < 7)
            {
                currentPiece = RNGPieces[i];
                nextMinos.Add(RNGPieces[i]);
            }
            //If all 7 pieces have been used, generate a new set of 7-pieces, and get the first piece
            else
            {
                SevenBagPRG();
                pieceItr = 1;
                currentPiece = RNGPieces[0];
                nextMinos.Add(RNGPieces[0]);
            }
        }
        currentPiece = nextMinos[0];
        nextMinos.RemoveAt(0);

    }

    //Fisher-Yates Shuffle
    //https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
    //Takes the 7 minos, and randomizes the order with all permutations being equally probable
    public void SevenBagPRG()
    {
        int n = 7;
        RNGPieces = Pieces;
        while (n > 0)
        {
            n--;
            int k = Random.Range(0, n);
            string val = RNGPieces[k];
            RNGPieces[k] = RNGPieces[n];
            RNGPieces[n] = val;
        }
    }
}
