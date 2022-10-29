using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMinoTransforms : MonoBehaviour
{
    //public bool leftPressed;
    //public bool rightPressed;
    //public bool testPressed;

    private int _rotaMode;

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

    private Dictionary<int, (GameObject, GameObject)> leftRightChild = new();

    public List<GameObject> childrenGO = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            childrenGO.Add(transform.GetChild(i).gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.name + this.tag);

        //add the leftmost and rightmost child gameobject of each orientation
        for (int i = 0; i< 4; i++)
            {
               leftRightChild.Add(i, (childrenGO[rotaModes.ReturnLRTuple(this.tag, i).Item1], childrenGO[rotaModes.ReturnLRTuple(this.tag,i).Item2]));
            }
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
            rotaMode -= 1;
            transform.rotation = Quaternion.Euler(0, 0, -90 * rotaMode);
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