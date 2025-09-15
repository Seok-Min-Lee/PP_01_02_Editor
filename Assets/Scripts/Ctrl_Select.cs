using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Select : MonoBehaviour
{
    [SerializeField] private RawImage studioImage;
    [SerializeField] private FilterButton[] filterButtons;

    [SerializeField] private Image timebarGuage;

    private float timer = 0f;
    private int timeLimit;

    private int filterNo = -1;
    private void Start()
    {
        timeLimit = ConstantValues.TIME_LIMIT_DEFAULT;

        Debug.Log("Client is Available? " + Client.Instance == null);

        Init();
    }

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
    public void OnClickSelect()
    {
        if (filterNo == -1)
        {
            return;
        }

        StaticValues.filterNo = filterNo;
        UnityEngine.SceneManagement.SceneManager.LoadScene("03_Edit");
    }
    public void OnClickFilter(int num)
    {
        for (int i = 0; i < filterButtons.Length; i++)
        {
            filterButtons[i].Highlight(i == num);
        }

        filterNo = num;
    }
    public void Init()
    {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(StaticValues.studioDataRaw.TextureRaw);

        studioImage.texture = texture;
    }
}
