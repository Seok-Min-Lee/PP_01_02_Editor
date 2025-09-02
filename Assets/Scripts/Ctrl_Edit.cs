using Mediapipe.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Edit : MonoBehaviour
{
    [SerializeField] private RawImage baseImage;

    [SerializeField] private Texture2D[] faceFilterTextures;

    [SerializeField] private Material faceFilterMaterial;
    [SerializeField] private FaceLandmarkerRunner_Custom runner;

    Texture2D sampleTexture;
    private void Start()
    {
        byte[] textureRaw = System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/»õ Æú´õ (2)/8.jpeg");
        sampleTexture = new Texture2D(0, 0);
        sampleTexture.LoadImage(textureRaw);

        faceFilterMaterial.mainTexture = faceFilterTextures[StaticValues.filterNo];
    }

    public void DrawMagazine()
    {
        Texture2D texture = sampleTexture;

        runner.ChangeImage(texture as Texture);
        runner.screen.Initialize(ImageSourceProvider_Custom.ImageSource);
        runner.screen.GetComponent<AutoFit_Custom>().Refresh();

        baseImage.texture = texture;

        GameObject.Find("Face Base Image").GetComponent<RawImage>().texture = texture;
    }
}
