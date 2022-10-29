using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class playerMino : MonoBehaviour
{
    private Rigidbody2D rb2D;

    public bool leftPressed;
    public bool rightPressed;
    public bool testPressed;

    public KeyCode test;

    public List<GameObject> childrenGO = new();
    public Component[] childBoxColliders;


    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        for (int i = 0; i < transform.childCount; i++)
            childrenGO.Add(transform.GetChild(i).gameObject);
        childBoxColliders = GetComponentsInChildren<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(childBoxColliders.Length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.Left))
            leftPressed = true;
        if (Input.GetKeyDown(GameManager.Instance.Right))
            rightPressed = true;
        if (Input.GetKeyDown(test))
            testPressed = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (testPressed)
        //{
        //    transform.position= transform.position(transform.right * 50);
        //}

        if (leftPressed)
        {
            int minIndex=0;
            float minX = childrenGO[0].transform.position.x;
            for(int i=0; i < childrenGO.Count; i++)
            {
                GameObject k = childrenGO[i];
                if(k.transform.position.x < minX)
                {
                    minIndex = i;
                    minX = k.transform.position.x;
                }
            }
            //Debug.Log(minX);
            //Debug.Log(childrenGO[minIndex].transform.position.x);
            Debug.Log(isRaycastChildrenOK(childrenGO[minIndex], Vector2.left, 0.4f));
            if (isRaycastChildrenOK(childrenGO[minIndex], Vector2.left, 0.4f))
            {
                rb2D.MovePosition(rb2D.position + new Vector2(-0.4f, 0));
            }
            leftPressed = false;
        }
        else if (rightPressed)
        {
            int maxIndex = 0;
            float maxX = childrenGO[0].transform.position.x;
            for (int i = 0; i < childrenGO.Count; i++)
            {
                GameObject k = childrenGO[i];
                if (k.transform.position.x > maxX)
                {
                    maxIndex = i;
                    maxX = k.transform.position.x;
                }
            }

            if (isRaycastChildrenOK(childrenGO[maxIndex], transform.right, 0.4f))
            {
                rb2D.MovePosition(rb2D.position + new Vector2(0.4f, 0));
            }
            rightPressed = false;
        }
    }

    bool isRaycastChildrenOK(GameObject child, Vector3 direction, float distance)
    {
        Debug.Log(child.gameObject.name);
        BoxCollider2D k = child.GetComponent<BoxCollider2D>();
        Debug.Log("processing child");
        RaycastHit2D[] hit = new RaycastHit2D[1];
        Debug.Log(k.Cast(direction, hit, distance, true));
        Debug.Log(hit[0].collider.gameObject.name);
        return k.Cast(direction, hit, distance, true) == 0;
    }
}
