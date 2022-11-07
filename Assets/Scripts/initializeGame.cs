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
        //When start key is pressed, disable title text, run game set up, and destroy this script
        if (Input.GetKeyDown(start))
        {
            title.text = "";
            title.enabled = false;
            GameManager.Instance.initGame();
            Destroy(this);
        }
    }
}
