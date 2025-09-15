using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Error : MonoBehaviour
{
    [SerializeField] private Image timebarGuage;

    private float timer = 0f;
    private int timeLimit;

    private void Update()
    {
        if (Input.anyKey)
        {
            timer = 0f;
        }

        timer += Time.deltaTime;
        timebarGuage.fillAmount = 1 - (timer / timeLimit);

        if (timer > timeLimit)
        {
            OnClickHome();
        }
    }
    public void OnClickHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("01_Title");
    }
}
