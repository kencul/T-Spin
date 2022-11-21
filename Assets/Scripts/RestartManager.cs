using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using TMPro;

public class RestartManager : MonoBehaviour
{
    public static RestartManager Instance;
    public bool gameovered = false;
    //public KeyCode space;
    Coroutine goToTitleCoroutine;
    [SerializeField] TextMeshProUGUI textBox;

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
        //space = GameManager.Instance.HardDrop;
    }

    // Update is called once per frame
    void Update()
    {
        //When gameovered, pressing space causes the scene to be reloaded, and restart the game from the title screen
        if (gameovered)
        {
            textBox.text = "GAME OVER";
            goToTitleCoroutine = StartCoroutine(goToTitle());
        }
    }

    IEnumerator goToTitle()
    {
        GameManager.Instance.PlayClip("GameOver");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("TitleScene");
    }
}
