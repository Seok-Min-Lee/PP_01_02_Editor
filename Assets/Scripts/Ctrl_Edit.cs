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

    [SerializeField] private RenderTexture rTex;

    Texture2D sampleTexture;
    private void Start()
    {
        Debug.Log("Client is Available? " + Client.Instance == null);

        byte[] textureRaw = StaticValues.studioDataRaw != null ? 
            StaticValues.studioDataRaw.TextureRaw :
            System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/새 폴더 (2)/8.jpeg");

        sampleTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
        sampleTexture.LoadImage(textureRaw);
        sampleTexture.wrapMode = TextureWrapMode.Clamp;

        faceFilterMaterial.mainTexture = faceFilterTextures[StaticValues.filterNo];
    }
    public void OnClickHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("01_Title");
    }
    public void DrawMagazine()
    {
        Texture2D texture = sampleTexture;

        runner.ChangeImage(texture as Texture);
        runner.screen.Initialize(ImageSourceProvider_Custom.ImageSource);
        runner.screen.GetComponent<AutoFit_Custom>().Refresh();

        baseImage.texture = texture;

        GameObject.Find("Face Base Image").GetComponent<RawImage>().texture = texture;
        Invoke("SendEditorDataRaw", 1f);
    }
    public void Finish()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            // 기존 이미지 사라짐
            CanvasGroup baseObjCG = baseImageRT.GetComponent<CanvasGroup>();
            float t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime;
                baseObjCG.alpha = 1 - t;

                yield return null;
            }

            baseObjCG.transform.SetSiblingIndex(baseObjCG.transform.GetSiblingIndex() - 1);
            baseObjCG.alpha = 1f;

            // 전/후 이미지 양쪽으로 이동
            t = 0f;
            Vector2 baseCurrent = baseImageRT.anchoredPosition;
            Vector2 baseTarget = new Vector2(-617, -25);
            Vector2 editCurrent = editImageRT.anchoredPosition;
            Vector2 editTarget = new Vector2(617, -25);
            while (t < 1)
            {
                t += Time.deltaTime;

                baseImageRT.anchoredPosition = Vector2.Lerp(baseCurrent, baseTarget, t);
                editImageRT.anchoredPosition = Vector2.Lerp(editCurrent, editTarget, t);

                yield return null;
            }

            // 안내문구 및 버튼 출력
            t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime;
                textGroupCG.alpha = t;

                yield return null;
            }

            textGroupCG.alpha = 1f;
        }
    }
    private void SendEditorDataRaw()
    {
        Texture2D tex = ConvertRenderTextureToTexutre2D(rTex);

        EditorDataRaw edr = new EditorDataRaw(
            id: -1,
            password: StaticValues.password,
            filterNo: StaticValues.filterNo,
            isDisplayed: false,
            registerDateTime: string.Empty,
            displayDateTime: string.Empty,
            textureRaw: tex.EncodeToJPG(),
            studioId: -1
        );

        StaticValues.editorDataRaw = edr;
        Client.Instance.RequestAddEditorData(edr);
    }
    private Texture2D ConvertRenderTextureToTexutre2D(RenderTexture renderTexture)
    {
        RenderTexture _rTex = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(renderTexture, _rTex);

        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(_rTex.width, _rTex.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, _rTex.width, _rTex.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(_rTex);

        return texture;
    }
    [SerializeField] private RectTransform baseImageRT;
    [SerializeField] private RectTransform editImageRT;
    [SerializeField] private CanvasGroup textGroupCG;
}
