using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Title : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Client is Available? " + Client.Instance == null);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance.RequestCheckPassword(2848);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Client.Instance.RequestCheckPassword(9999);
        }
    }

    public void OnClickDigitButton(int num)
    {

    }
}
