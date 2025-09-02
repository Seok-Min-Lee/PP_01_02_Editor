// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.Unity
{
  public class AutoFit_Custom : MonoBehaviour
  {
    [System.Serializable]
    public enum FitMode
    {
      Expand,
      Shrink,
      FitWidth,
      FitHeight,
    }

    [SerializeField] private FitMode _fitMode;

    private void LateUpdate()
    {
      var rectTransform = GetComponent<RectTransform>();
      if (rectTransform.rect.width == 0 || rectTransform.rect.height == 0)
      {
        return;
      }

      var parentRect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect;
      var (width, height) = GetBoundingBoxSize(rectTransform);

      var ratio = parentRect.width / width;
      var h = height * ratio;

      if (_fitMode == FitMode.FitWidth || (_fitMode == FitMode.Expand && h >= parentRect.height) || (_fitMode == FitMode.Shrink && h <= parentRect.height))
      {
        rectTransform.offsetMin *= ratio;
        rectTransform.offsetMax *= ratio;
        return;
      }

      ratio = parentRect.height / height;

      rectTransform.offsetMin *= ratio;
      rectTransform.offsetMax *= ratio;
    }

    private (float, float) GetBoundingBoxSize(RectTransform rectTransform)
    {
      var rect = rectTransform.rect;
      var center = rect.center;
      var topLeftRel = new Vector2(rect.xMin - center.x, rect.yMin - center.y);
      var topRightRel = new Vector2(rect.xMax - center.x, rect.yMin - center.y);
      var rotatedTopLeftRel = rectTransform.localRotation * topLeftRel;
      var rotatedTopRightRel = rectTransform.localRotation * topRightRel;
      var wMax = Mathf.Max(Mathf.Abs(rotatedTopLeftRel.x), Mathf.Abs(rotatedTopRightRel.x));
      var hMax = Mathf.Max(Mathf.Abs(rotatedTopLeftRel.y), Mathf.Abs(rotatedTopRightRel.y));
      return (2 * wMax, 2 * hMax);
    }
        //
        public void Refresh()
        {
            var rectTransform = GetComponent<RectTransform>();
            //if (rectTransform.rect.width == 0 || rectTransform.rect.height == 0)
            //{
            //    return;
            //}

            //var parentRect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect;
            //var (width, height) = GetBoundingBoxSize(rectTransform);

            //var ratio = parentRect.width / width;
            //var h = height * ratio;

            //if (_fitMode == FitMode.FitWidth || (_fitMode == FitMode.Expand && h >= parentRect.height) || (_fitMode == FitMode.Shrink && h <= parentRect.height))
            //{
            //    rectTransform.offsetMin *= ratio;
            //    rectTransform.offsetMax *= ratio;
            //    return;
            //}

            //ratio = parentRect.height / height;

            //rectTransform.offsetMin *= ratio;
            //rectTransform.offsetMax *= ratio;
            rectTransform.GetComponent<RawImage>().SetNativeSize();
            rectTransform.sizeDelta = GetBestFitClampSize(rectTransform, transform.parent.GetComponent<RectTransform>());
        }
        // target�� parent�� �� ä�쵵�� �ϴ� SizeDelta���� ��ȯ�Ѵ�.
        // parent�� ������ ���� ä��� ���� �����̱� ������ target�� �Ϻ� �߸� �� �ִ�.
        // �ش� �޼ҵ带 ���� �ϱ� ���� target�� Image.SetNativeSize()�� �����Ѵ�.
        public static Vector2 GetBestFitCropSize(RectTransform target, RectTransform parent)
        {
            float targetWidth = target.rect.width;
            float targetHeight = target.rect.height;

            float parentWidth = parent.rect.width;
            float parentHeight = parent.rect.height;

            float ratio = parentWidth / targetWidth;

            if (targetHeight * ratio < parentHeight)
            {
                ratio = parentHeight / targetHeight;
            }

            return new Vector2(targetWidth * ratio, targetHeight * ratio);
        }
        public static Vector2 GetBestFitCropSize(RectTransform target, Vector2 parent)
        {
            float targetWidth = target.rect.width;
            float targetHeight = target.rect.height;

            float parentWidth = parent.x;
            float parentHeight = parent.y;

            float ratio = parentWidth / targetWidth;

            if (targetHeight * ratio < parentHeight)
            {
                ratio = parentHeight / targetHeight;
            }

            return new Vector2(targetWidth * ratio, targetHeight * ratio);
        }

        // target�� parent�� ä�쵵�� �ϴ� SizeDelta ���� ��ȯ�Ѵ�.
        // target�� ������ ������ �ϴ� ���� �����̱� ������ parent�� ������ ���� �� �ִ�.
        // �ش� �޼ҵ带 ���� �ϱ� ���� target�� Image.SetNativeSize()�� �����Ѵ�.
        public static Vector2 GetBestFitClampSize(RectTransform target, RectTransform parent)
        {
            float targetWidth = target.rect.width;
            float targetHeight = target.rect.height;

            float parentWidth = parent.rect.width;
            float parentHeight = parent.rect.height;

            float ratioPositive;
            float ratioNegative;
            if (targetWidth > targetHeight)
            {
                ratioPositive = parentWidth / targetWidth;
                ratioNegative = targetHeight * ratioPositive > parentHeight ? parentHeight / (targetHeight * ratioPositive) : 1;
            }
            else
            {
                ratioPositive = parentHeight / targetHeight;
                ratioNegative = targetWidth * ratioPositive > parentWidth ? parentWidth / (targetWidth * ratioPositive) : 1;
            }

            return new Vector2(targetWidth * ratioPositive * ratioNegative, targetHeight * ratioPositive * ratioNegative);
        }
        public static Vector2 GetBestFitClampSize(RectTransform target, Vector2 parent)
        {
            float targetWidth = target.rect.width;
            float targetHeight = target.rect.height;

            float parentWidth = parent.x;
            float parentHeight = parent.y;

            float ratioPositive;
            float ratioNegative;
            if (targetWidth > targetHeight)
            {
                ratioPositive = parentWidth / targetWidth;
                ratioNegative = targetHeight * ratioPositive > parentHeight ? parentHeight / (targetHeight * ratioPositive) : 1;
            }
            else
            {
                ratioPositive = parentHeight / targetHeight;
                ratioNegative = targetWidth * ratioPositive > parentWidth ? parentWidth / (targetWidth * ratioPositive) : 1;
            }

            return new Vector2(targetWidth * ratioPositive * ratioNegative, targetHeight * ratioPositive * ratioNegative);
        }
    }
}
