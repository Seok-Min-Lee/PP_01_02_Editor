// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Tasks.Vision.FaceLandmarker;

namespace Mediapipe.Unity.Sample.FaceLandmarkDetection
{
  public class FaceLandmarkDetectionConfig_Custom
  {
    public Tasks.Core.BaseOptions_Custom.Delegate Delegate { get; set; } =
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
          Tasks.Core.BaseOptions_Custom.Delegate.CPU;
#else
        //Tasks.Core.BaseOptions.Delegate.GPU;
        Tasks.Core.BaseOptions_Custom.Delegate.GPU;
#endif
    public Tasks.Vision.Core.RunningMode RunningMode { get; set; } = Tasks.Vision.Core.RunningMode.LIVE_STREAM;

    public int NumFaces { get; set; } = 3;
    public float MinFaceDetectionConfidence { get; set; } = 0.5f;
    public float MinFacePresenceConfidence { get; set; } = 0.5f;
    public float MinTrackingConfidence { get; set; } = 0.5f;
    public bool OutputFaceBlendshapes { get; set; } = true;
    public bool OutputFacialTransformationMatrixes { get; set; } = true;
    public string ModelPath => OutputFaceBlendshapes ? "face_landmarker_v2_with_blendshapes.bytes" : "face_landmarker_v2.bytes";

    public FaceLandmarkerOptions_Custom GetFaceLandmarkerOptions(FaceLandmarkerOptions_Custom.ResultCallback resultCallback = null)
    {
      return new FaceLandmarkerOptions_Custom(
        new Tasks.Core.BaseOptions_Custom(Delegate, modelAssetPath: ModelPath),
        runningMode: RunningMode,
        numFaces: NumFaces,
        minFaceDetectionConfidence: MinFaceDetectionConfidence,
        minFacePresenceConfidence: MinFacePresenceConfidence,
        minTrackingConfidence: MinTrackingConfidence,
        outputFaceBlendshapes: OutputFaceBlendshapes,
        outputFaceTransformationMatrixes: OutputFacialTransformationMatrixes,
        resultCallback: resultCallback
      );
    }
  }
}
