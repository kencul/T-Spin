using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string currentPiece;
    List<string> Pieces = new() { "i", "o", "t", "l", "j", "s", "z" };
    List<string> RNGPieces = new() { };
    List<string> nextMinos = new();
    public int pieceItr = 0;

    public string heldPiece = "";
    public bool holdOK = true;

    public KeyCode Place;
    public KeyCode Hold;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode rotRight;
    public KeyCode rotLeft;

    //Mino Prefab References
    public GameObject IMino;
    public GameObject OMino;
    public GameObject SMino;
    public GameObject ZMino;
    public GameObject LMino;
    public GameObject JMino;
    public GameObject TMino;
    Dictionary<string, GameObject> minoPrefabDict = new();

    //References to visual GameObjects
    private GameObject playerMino;
    private GameObject holdMino;
    private GameObject nextMino1;
    private GameObject nextMino2;
    private GameObject nextMino3;
    private GameObject nextMino4;
    private GameObject nextMino5;
    List<GameObject> nextMinoList;

    Dictionary<string, Vector3> spawnLocation = new()
    {
        { "i", new Vector3(0, 4.4f, 0) },
        { "o", new Vector3(0, 4.8f, 0) },
        { "t", new Vector3(-0.2f, 4.6f, 0) },
        { "l", new Vector3(-0.2f, 4.6f, 0) },
        { "j", new Vector3(-0.2f, 4.6f, 0) },
        { "z", new Vector3(-0.2f, 4.6f, 0) },
        { "s", new Vector3(-0.2f, 4.6f, 0) }
    };

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

    void Awake()
    {
        //Singleton Instance
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // Start is called before the first frame update
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
        foreach (KeyValuePair<string, GameObject> kvp in tempPrefabDict)
        {
            minoPrefabDict.Add(kvp.Key, kvp.Value);
            //Debug.LogFormat("{0} added", kvp.Key);
        }

        //Instantiate list of next Mino GameObjects
        nextMinoList = new() { nextMino1, nextMino2, nextMino3, nextMino4, nextMino5 };

        SevenBagPRG();
        NextPiece();
        updateMinos();
    }

    // Update is called once per frame
    void Update()
    {
        //Place a piece
        if (Input.GetKeyDown(Place))
        {
            NextPiece();
            updateMinos();
            holdOK = true;
        }

        //Hold Piece
        if (Input.GetKeyDown(Hold) && holdOK)
        {
            if (heldPiece == "")
            {
                heldPiece = currentPiece;
                NextPiece();
            }
            else
            {
                (heldPiece, currentPiece) = (currentPiece, heldPiece);
            }
            updateMinos();
            holdOK = false;
        }
    }

    public void updateMinos()
    {
        //Spawn Player Mino with script
        if (playerMino != null)
        {
            Destroy(playerMino);
            //Debug.LogFormat("Deleting {0}", playerMino);
        }
        playerMino = Instantiate(minoPrefabDict[currentPiece], spawnLocation[currentPiece], Quaternion.identity);
        playerMino.AddComponent<playerMinoTransforms>();

        //Spawn Hold Mino
        if (heldPiece != "" && holdMino != minoPrefabDict[heldPiece])
        {
            if (holdMino != null)
            {
                Destroy(holdMino);
            }
            holdMino = Instantiate(minoPrefabDict[heldPiece], new Vector3(-3.08f, 2.6f, 0)+centerMino[heldPiece], Quaternion.identity);
        }

        //Spawn Next Minos
        if (nextMino1 != minoPrefabDict[nextMinos[0]] && nextMino2 != minoPrefabDict[nextMinos[1]])
        {
            for (int i=0; i < nextMinoList.Count; i++)
            {
                if (nextMinoList[i] != null)
                {
                    Destroy(nextMinoList[i]);
                }
                nextMinoList[i] = Instantiate(minoPrefabDict[nextMinos[i]], new Vector3(3.06f, 2.97f-.9f*i, 0)+centerMino[nextMinos[i]], new Quaternion(0, 0, 0, 0));
            }
        }
    }

    public void NextPiece()
    {
        //Create queue of next 5 minos
        while (nextMinos.Count < 6)
        {
            int i = pieceItr;
            pieceItr += 1;
            if (i < 7)
            {
                currentPiece = RNGPieces[i];
                nextMinos.Add(RNGPieces[i]);
            }
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
