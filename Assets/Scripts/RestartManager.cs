using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    public static RestartManager Instance;
    public bool gameovered = false;
    public KeyCode space;

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
        space = GameManager.Instance.HardDrop;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameovered && Input.GetKeyDown(space))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
