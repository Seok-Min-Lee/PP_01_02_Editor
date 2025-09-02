// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Unity;

public static class ImageSourceProvider_Custom
{
  //private static WebCamSource _WebccamSource;
  private static StaticImageSource_Custom _ImageSource_Custom;
  private static VideoSource _VideoSource;
  private static WebCamSource_Custom _WebCamSource_Custom;

  public static ImageSource ImageSource { get; private set; }

  public static ImageSourceType CurrentSourceType
  {
    get
    {
      if (ImageSource is WebCamSource_Custom)
      {
        return ImageSourceType.WebCamera;
      }
      if (ImageSource is StaticImageSource_Custom)
      {
        return ImageSourceType.Image;
      }
      if (ImageSource is VideoSource)
      {
        return ImageSourceType.Video;
      }
      return ImageSourceType.Unknown;
    }
  }

  internal static void Initialize(WebCamSource_Custom webCamSource_Custom, StaticImageSource_Custom staticImageSource, VideoSource videoSource)
  {
    _WebCamSource_Custom = webCamSource_Custom;
    _ImageSource_Custom = staticImageSource;
    _VideoSource = videoSource;
  }
  public static void Switch(ImageSourceType imageSourceType)
  {
    switch (imageSourceType)
    {
      case ImageSourceType.WebCamera:
        {
          ImageSource = _WebCamSource_Custom;
          break;
        }
      case ImageSourceType.Image:
        {
          ImageSource = _ImageSource_Custom;
          break;
        }
      case ImageSourceType.Video:
        {
          ImageSource = _VideoSource;
          break;
        }
      case ImageSourceType.Unknown:
      default:
        {
          throw new System.ArgumentException($"Unsupported source type: {imageSourceType}");
        }
    }
  }
}
