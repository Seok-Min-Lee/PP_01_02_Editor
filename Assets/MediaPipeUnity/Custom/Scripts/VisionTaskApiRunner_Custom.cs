// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using System.Collections;
using UnityEngine;

public abstract class VisionTaskApiRunner_Custom<TTask> : BaseRunner_Custom where TTask : BaseVisionTaskApi
{
    //[SerializeField] protected Mediapipe.Unity.Screen screen;
    public Mediapipe.Unity.Screen screen;

  private Coroutine _coroutine;
  protected TTask taskApi;

  public Mediapipe.Tasks.Vision.Core.RunningMode runningMode;

  public override void Play()
  {
    if (_coroutine != null)
    {
      Stop();
    }
    base.Play();
    _coroutine = StartCoroutine(Run());
  }

  public override void Pause()
  {
    base.Pause();
    ImageSourceProvider.ImageSource.Pause();
  }

  public override void Resume()
  {
    base.Resume();
    var _ = StartCoroutine(ImageSourceProvider.ImageSource.Resume());
  }

  public override void Stop()
  {
    base.Stop();
    StopCoroutine(_coroutine);
    ImageSourceProvider.ImageSource.Stop();
    taskApi?.Close();
    taskApi = null;
  }

  protected abstract IEnumerator Run();

  protected static void SetupAnnotationController<T>(AnnotationController_Custom<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation_Custom
  {
    annotationController.isMirrored = expectedToBeMirrored;
    annotationController.imageSize = new Vector2Int(imageSource.textureWidth, imageSource.textureHeight);
  }
}
