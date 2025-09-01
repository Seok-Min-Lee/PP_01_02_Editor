using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordDigit : MonoBehaviour
{
    public string Value 
    { 
        get 
        {
            return text.text;
        } 
    }
    [SerializeField] private TextMeshProUGUI text;

    private Animation animation;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        text.text = string.Empty;
        animation = text.GetComponent<Animation>();
        canvasGroup = text.GetComponent<CanvasGroup>();
    }
    public void SetText(string text)
    {
        this.text.text = text;
        animation.Stop();
        canvasGroup.alpha = 1f;
    }
    public void SetFocus()
    {
        this.text.text = "_";
        animation.Play();
    }
}
