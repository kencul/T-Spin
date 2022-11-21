using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class initializeGame : MonoBehaviour
{
    public KeyCode start;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI countDown;
    Coroutine countDownCoroutine;


    private void Start()
    {
        countDownCoroutine = StartCoroutine(CountDown());
    }

    // Update is called once per frame
    /*void Update()
    {
        //When start key is pressed, disable title text, run game set up, and destroy this script
        if (Input.GetKeyDown(start))
        {
            title.text = "";
            title.enabled = false;
            GameManager.Instance.initGame();
            Destroy(this);
        }
    }*/

    //Hard coded countdown with text
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1.0f);
        countDown.text = "3";
        GameManager.Instance.PlayClip("CountDown");
        yield return new WaitForSeconds(0.5f);
        countDown.text = "";
        yield return new WaitForSeconds(0.5f);
        countDown.text = "2";
        GameManager.Instance.PlayClip("CountDown");
        yield return new WaitForSeconds(0.5f);
        countDown.text = "";
        yield return new WaitForSeconds(0.5f);
        countDown.text = "1";
        GameManager.Instance.PlayClip("CountDown");
        yield return new WaitForSeconds(0.5f);
        countDown.text = "";
        yield return new WaitForSeconds(0.5f);
        countDown.text = "GO!";
        GameManager.Instance.PlayClip("LoadStart");
        yield return new WaitForSeconds(0.5f);
        countDown.text = "";
        GameManager.Instance.initGame();
        Destroy(this);
    }
}