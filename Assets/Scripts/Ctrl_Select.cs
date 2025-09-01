using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Select : MonoBehaviour
{
    [SerializeField] private RawImage studioImage;
    private void Start()
    {
        Debug.Log("Client is Available? " + Client.Instance == null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance.RequestStudioData(3932);
        }
    }
    public void Init()
    {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(StaticValues.studioDataRaw.TextureRaw);

        studioImage.texture = texture;
    }
}
