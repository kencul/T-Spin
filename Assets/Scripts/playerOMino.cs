using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerOMino : MonoBehaviour
{
    //rotation Modes:

    //0 = [1] [3]
    //    [0] [2]

    //1 = [0] [1]
    //    [2] [3]

    //2 = [2] [0]
    //    [3] [1]

    //3 = [3] [2]
    //    [1] [0]

    private int _rotaMode;

    public int rotaMode
    {
        get => _rotaMode;
        set
        {
            if (value >= 4)
                _rotaMode = 0;
            else if (value <= -1)
                _rotaMode = 3;
            else
                _rotaMode = value;
        }
    }

    private Dictionary<int, (GameObject, GameObject)> leftRightChild = new();

    public List<GameObject> childrenGO = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            childrenGO.Add(transform.GetChild(i).gameObject);

        //rotationMode  leftMostChild  rightMostChild
        leftRightChild.Add(0, (childrenGO[0], childrenGO[2]));
        leftRightChild.Add(1, (childrenGO[2], childrenGO[3]));
        leftRightChild.Add(2, (childrenGO[3], childrenGO[1]));
        leftRightChild.Add(3, (childrenGO[1], childrenGO[0]));
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.Left))
        {
            Debug.Log("Left Pressed");
            if (leftRightChild[rotaMode].Item1.transform.position.x > -1.7f)
            {
                Debug.Log("translating");
                transform.position += new Vector3(-0.4f, 0, 0);
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.Right))
        {
            if (leftRightChild[rotaMode].Item2.transform.position.x < 1.7f)
            {
                transform.position += new Vector3(0.4f, 0, 0);
            }
        }
        else if (Input.GetKeyDown(GameManager.Instance.rotRight))
        {
            rotaMode += 1;
            transform.rotation = Quaternion.Euler(0, 0, -90 * rotaMode);
        }
        else if (Input.GetKeyDown(GameManager.Instance.rotLeft))
        {
            rotaMode += 1;
            transform.rotation = Quaternion.Euler(0, 0, 90 * rotaMode);
        }
    }
}
