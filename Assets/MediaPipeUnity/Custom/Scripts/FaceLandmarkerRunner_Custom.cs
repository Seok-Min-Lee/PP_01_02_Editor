// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.FaceLandmarkDetection;
using UnityEngine;
using UnityEngine.Rendering;
using Mediapipe;

public class FaceLandmarkerRunner_Custom : VisionTaskApiRunner_Custom<FaceLandmarker_Custom>
{
  [SerializeField] private FaceLandmarkerResultAnnotationController_Custom _faceLandmarkerResultAnnotationController;

  private Mediapipe.Unity.Experimental.TextureFramePool _textureFramePool;

  public readonly FaceLandmarkDetectionConfig_Custom config = new FaceLandmarkDetectionConfig_Custom();

  public override void Stop()
  {
    base.Stop();
    _textureFramePool?.Dispose();
    _textureFramePool = null;
  }

  protected override IEnumerator Run()
  {
    Debug.Log($"Delegate = {config.Delegate}");
    Debug.Log($"Running Mode = {config.RunningMode}");
    Debug.Log($"NumFaces = {config.NumFaces}");
    Debug.Log($"MinFaceDetectionConfidence = {config.MinFaceDetectionConfidence}");
    Debug.Log($"MinFacePresenceConfidence = {config.MinFacePresenceConfidence}");
    Debug.Log($"MinTrackingConfidence = {config.MinTrackingConfidence}");
    Debug.Log($"OutputFaceBlendshapes = {config.OutputFaceBlendshapes}");
    Debug.Log($"OutputFacialTransformationMatrixes = {config.OutputFacialTransformationMatrixes}");

    yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

    var options = config.GetFaceLandmarkerOptions(config.RunningMode == Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnFaceLandmarkDetectionOutput : null);
    taskApi = FaceLandmarker_Custom.CreateFromOptions(options, GpuManager.GpuResources);
    var imageSource = ImageSourceProvider_Custom.ImageSource;

    yield return imageSource.Play();

    if (!imageSource.isPrepared)
    {
      Debug.LogError("Failed to start ImageSource, exiting...");
      yield break;
    }

    // Use RGBA32 as the input format.
    // TODO: When using GpuBuffer, MediaPipe assumes that the input format is BGRA, so maybe the following code needs to be fixed.
    _textureFramePool = new Mediapipe.Unity.Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

    // NOTE: The screen will be resized later, keeping the aspect ratio.
    screen.Initialize(imageSource);

    SetupAnnotationController(_faceLandmarkerResultAnnotationController, imageSource);

    var transformationOptions = imageSource.GetTransformationOptions();
    var flipHorizontally = transformationOptions.flipHorizontally;
    var flipVertically = transformationOptions.flipVertically;
    var imageProcessingOptions = new Mediapipe.Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

    AsyncGPUReadbackRequest req = default;
    var waitUntilReqDone = new WaitUntil(() => req.done);
    var result = FaceLandmarkerResult_Custom.Alloc(options.numFaces);

    // NOTE: we can share the GL context of the render thread with MediaPipe (for now, only on Android)
    var canUseGpuImage = options.baseOptions.delegateCase == Mediapipe.Tasks.Core.BaseOptions_Custom.Delegate.GPU &&
      SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 &&
      GpuManager.GpuResources != null;
    using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;
    //
    GameObject.Find("Ctrl").GetComponent<Ctrl_Edit>().DrawMagazine();
    //
    while (true)
    {
      if (isPaused)
      {
        yield return new WaitWhile(() => isPaused);
      }

      if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
      {
        yield return new WaitForEndOfFrame();
        continue;
      }

      // Build the input Image
      Image image;
      if (canUseGpuImage)
      {
        yield return new WaitForEndOfFrame();
        textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
        image = textureFrame.BuildGpuImage(glContext);
      }
      else
      {
        req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
        yield return waitUntilReqDone;

        if (req.hasError)
        {
          Debug.LogError($"Failed to read texture from the image source, exiting...");
          break;
        }
        image = textureFrame.BuildCPUImage();
        textureFrame.Release();
      }

      switch (taskApi.runningMode)
      {
        case Mediapipe.Tasks.Vision.Core.RunningMode.IMAGE:
          if (taskApi.TryDetect(image, imageProcessingOptions, ref result))
          {
            _faceLandmarkerResultAnnotationController.DrawNow(result);
          }
          else
          {
            _faceLandmarkerResultAnnotationController.DrawNow(default);
          }
          break;
        case Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO:
          if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
          {
            _faceLandmarkerResultAnnotationController.DrawNow(result);
          }
          else
          {
            _faceLandmarkerResultAnnotationController.DrawNow(default);
          }
          break;
        case Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM:
          taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
          break;
      }
    }
  }

  private void OnFaceLandmarkDetectionOutput(FaceLandmarkerResult_Custom result, Image image, long timestamp)
  {
    _faceLandmarkerResultAnnotationController.DrawLater(result);
  }

    //
    public void ChangeImage(Texture texture)
    {
        StaticImageSource_Custom imageSource = ImageSourceProvider_Custom.ImageSource as StaticImageSource_Custom;

        if (imageSource != null)
        {
            imageSource.Stop();
            imageSource.ChangeSource(texture);
            StartCoroutine(imageSource.Play());
        }
    }
    //
}
