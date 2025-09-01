using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Title : MonoBehaviour
{
    [SerializeField] private PasswordDigit[] digits;
    private int cursorIndex = 0;

    private void Start()
    {
        Debug.Log("Client is Available? " + Client.Instance == null);

        digits[0].SetFocus();
    }
    private void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) ||
                Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                OnClickDigitButton(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) OnClickSubmit();
        if (Input.GetKeyDown(KeyCode.Backspace)) OnClickRemove();
    }

    public void OnClickDigitButton(int num)
    {
        if (cursorIndex == digits.Length) 
        { 
            return;
        }

        digits[cursorIndex++].SetText(num + ""); 

        if (cursorIndex < digits.Length) 
        {
            digits[cursorIndex].SetFocus(); 
        }
    }
    public void OnClickRemove()
    {
        if (cursorIndex == 0)
        {
            return;
        }

        if (cursorIndex < digits.Length)
        {
            digits[cursorIndex].SetText("");
        }

        digits[--cursorIndex].SetFocus();
    }
    public void OnClickSubmit()
    {
        string pwStr = string.Empty;

        for (int i = 0; i < digits.Length; i++)
        {
            pwStr += digits[i].Value;
        }

        if (int.TryParse(pwStr, out int pw))
        {
            StaticValues.password = pw;
            Client.Instance.RequestCheckPassword(pw);
        }
        else
        {
            Debug.Log("Fail :: " + pwStr);
        }
    }
}
