// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;

using Mediapipe.Tasks.Vision.FaceLandmarker;

namespace Mediapipe.Unity
{
  public class FaceLandmarkerResultAnnotationController_Custom : AnnotationController_Custom<MultiFaceLandmarkListAnnotation_Custom>
  {
    [SerializeField] private bool _visualizeZ = false;

    private FaceLandmarkerResult_Custom _currentTarget;

    public void DrawNow(FaceLandmarkerResult_Custom target)
    {
      target.CloneTo(ref _currentTarget);
      SyncNow();
    }

    public void DrawLater(FaceLandmarkerResult_Custom target) => UpdateCurrentTarget(target);

    protected void UpdateCurrentTarget(FaceLandmarkerResult_Custom newTarget)
    {
      if (IsTargetChanged(newTarget, _currentTarget))
      {
        newTarget.CloneTo(ref _currentTarget);
        isStale = true;
      }
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget.faceLandmarks, _visualizeZ);
    }
  }
}
