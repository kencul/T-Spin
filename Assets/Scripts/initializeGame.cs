using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class initializeGame : MonoBehaviour
{
    public KeyCode start;
    public TextMeshProUGUI title;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(start))
        {
            title.text = "";
            GameManager.Instance.initGame();
            Destroy(this);
        }
    }
}
