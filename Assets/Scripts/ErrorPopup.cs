using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private float timer = 0f;
    private int timeLimit = 3;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeLimit)
        {
            Close();
        }
    }
    public void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            timer = 0f;
            canvasGroup.blocksRaycasts = false;

            float t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime * 2;
                canvasGroup.alpha = t;

                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public void Close()
    {
        canvasGroup.gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
