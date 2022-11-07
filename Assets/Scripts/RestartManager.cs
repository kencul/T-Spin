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
        //When gameovered, pressing space causes the scene to be reloaded, and restart the game from the title screen
        if (gameovered && Input.GetKeyDown(space))
            SceneManager.LoadScene("SampleScene");
    }
}
