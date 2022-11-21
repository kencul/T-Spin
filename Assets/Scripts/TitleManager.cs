using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;

    [SerializeField] KeyCode space;

    void Awake()
    {
        //Singleton Instance
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(space))
            SceneManager.LoadScene("PlayScene");
    }
}
