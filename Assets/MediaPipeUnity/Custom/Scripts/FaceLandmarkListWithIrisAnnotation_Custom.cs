// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
    #pragma warning restore IDE0065

    public sealed class FaceLandmarkListWithIrisAnnotation_Custom : HierarchicalAnnotation_Custom
    {
        [SerializeField] private FaceLandmarkListAnnotation_Custom _faceLandmarkListAnnotation;
        [SerializeField] private IrisLandmarkListAnnotation_Custom _leftIrisLandmarkListAnnotation;
        [SerializeField] private IrisLandmarkListAnnotation_Custom _rightIrisLandmarkListAnnotation;

        private const int _FaceLandmarkCount = 468;
        private const int _IrisLandmarkCount = 5;

#region Peopulley Custom
        [SerializeField] private FaceLandmarkListAnnotation_Custom _customLandmarkListAnnotation;
        [SerializeField] private MeshFilter faceMeshFilter;

        [SerializeField] private float customRatioNose;
        [SerializeField] private float customRatioEyesFirst;
        [SerializeField] private float customRatioEyesSecond;
        public List<Vector3> customVertices { get; private set; }

        private List<Vector2> uvs;
        private int makeupIndex;
                
        private void Start()
        {
            uvs = new List<Vector2>();
            for (int i = 0; i < _FaceLandmarkCount; i++)
            {
                uvs.Add(new Vector3((float)u[i], (float)v[i], 0));
            }

            faceMeshFilter.mesh.SetUVs(0, uvs);
            faceMeshFilter.mesh.SetTriangles(_triangles_reverse, 0);

            // Initialize
            ChangeMakeup(StaticValues.filterNo);
        }
        public void ChangeMakeup(int index)
        {
            makeupIndex = index;
        }
        public void ChangeFaceMeshTexture(Texture texture)
        {
            faceMeshFilter.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
#endregion

        public override bool isMirrored
        {
            set
            {
                _faceLandmarkListAnnotation.isMirrored = value;
                _leftIrisLandmarkListAnnotation.isMirrored = value;
                _rightIrisLandmarkListAnnotation.isMirrored = value;
                _customLandmarkListAnnotation.isMirrored = value;
                base.isMirrored = value;
            }
        }

        public override RotationAngle rotationAngle
        {
            set
            {
                _faceLandmarkListAnnotation.rotationAngle = value;
                _leftIrisLandmarkListAnnotation.rotationAngle = value;
                _rightIrisLandmarkListAnnotation.rotationAngle = value;
                _customLandmarkListAnnotation.rotationAngle = value;
                base.rotationAngle = value;
            }
        }

        public void SetFaceLandmarkColor(Color color)
        {
            _faceLandmarkListAnnotation.SetLandmarkColor(color);
            _customLandmarkListAnnotation.SetLandmarkColor(color);
        }

        public void SetIrisLandmarkColor(Color color)
        {
            _leftIrisLandmarkListAnnotation.SetLandmarkColor(color);
            _rightIrisLandmarkListAnnotation.SetLandmarkColor(color);
        }

        public void SetFaceLandmarkRadius(float radius)
        {
            _faceLandmarkListAnnotation.SetLandmarkRadius(radius);
            _customLandmarkListAnnotation.SetLandmarkRadius(radius);
        }

        public void SetIrisLandmarkRadius(float radius)
        {
            _leftIrisLandmarkListAnnotation.SetLandmarkRadius(radius);
            _rightIrisLandmarkListAnnotation.SetLandmarkRadius(radius);
        }

        public void SetFaceConnectionColor(Color color)
        {
            _faceLandmarkListAnnotation.SetConnectionColor(color);
        }

        public void SetFaceConnectionWidth(float width)
        {
            _faceLandmarkListAnnotation.SetConnectionWidth(width);
        }

        public void SetIrisCircleColor(Color color)
        {
            _leftIrisLandmarkListAnnotation.SetCircleColor(color);
            _rightIrisLandmarkListAnnotation.SetCircleColor(color);
        }

        public void SetIrisCircleWidth(float width)
        {
            _leftIrisLandmarkListAnnotation.SetCircleWidth(width);
            _rightIrisLandmarkListAnnotation.SetCircleWidth(width);
        }

        public void Draw(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            if (ActivateFor(target))
            {
                var (faceLandmarks, leftLandmarks, rightLandmarks, customLandmarks) = PartitionLandmarkList_custom(target);

                DrawFaceLandmarkList(faceLandmarks, visualizeZ);
                DrawLeftIrisLandmarkList(leftLandmarks, visualizeZ, circleVertices);
                DrawRightIrisLandmarkList(rightLandmarks, visualizeZ, circleVertices);
                DrawCustomLandmarkList(customLandmarks, visualizeZ);
            }
        }

        public void Draw(NormalizedLandmarkList target, bool visualizeZ = false, int circleVertices = 128)
        {
            if (ActivateFor(target))
            {
                Draw(target.Landmark, visualizeZ, circleVertices);
            }
        }

        public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            if (ActivateFor(target))
            {
                var (faceLandmarks, leftLandmarks, rightLandmarks, customLandmarks) = PartitionLandmarkList_custom(target);

                DrawFaceLandmarkList(faceLandmarks, visualizeZ);
                DrawLeftIrisLandmarkList(leftLandmarks, visualizeZ, circleVertices);
                DrawRightIrisLandmarkList(rightLandmarks, visualizeZ, circleVertices);
                DrawCustomLandmarkList(customLandmarks, visualizeZ);
            }
        }
        public void Draw_Custom(
            IReadOnlyList<mptcc.NormalizedLandmark> target, 
            out List<Vector3> meshVertices, 
            out List<Vector3> customVertices, 
            bool visualizeZ = false, 
            int circleVertices = 128
        )
        {
            meshVertices = new List<Vector3>();
            customVertices = new List<Vector3>();

            if (ActivateFor(target))
            {
                var (faceLandmarks, leftLandmarks, rightLandmarks, customLandmarks) = PartitionLandmarkList_custom(target);

                DrawLeftIrisLandmarkList(leftLandmarks, visualizeZ, circleVertices);
                DrawRightIrisLandmarkList(rightLandmarks, visualizeZ, circleVertices);

                customVertices.AddRange(DrawCustomLandmarkList_Custom(customLandmarks, visualizeZ));
                meshVertices.AddRange(DrawFaceLandmarkList_Custom(faceLandmarks, visualizeZ));
            }
        }
        public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false, int circleVertices = 128)
        {
            if (ActivateFor(target))
            {
                // meshVertices: 매쉬로 만들 점들          
                // customVetices: 임의로 추가한 점들
                List<Vector3> _meshVertices;
                List<Vector3> _customVertices;
                try
                {
                    // 랜드마크 추가
                    float mouseWidth = target.landmarks[308].x - target.landmarks[78].x;
                    float mouseHeight = target.landmarks[14].y - target.landmarks[13].y;
                    if (target.landmarks.Count > 468 && mouseHeight > mouseWidth * 0.75f)
                    {
                        mptcc.NormalizedLandmark newLandmark = new mptcc.NormalizedLandmark(
                            x: (target.landmarks[308].x + target.landmarks[78].x) * 0.5f,
                            y: (target.landmarks[14].y + target.landmarks[13].y) * 0.5f,
                            z: 0,
                            visibility: target.landmarks[14].visibility,
                            presence: target.landmarks[14].presence,
                            name: "MouseCenter"
                        );
                        target.landmarks.Add(newLandmark);
                    }

                    Draw_Custom(
                        target: target.landmarks,
                        visualizeZ: visualizeZ,
                        circleVertices: circleVertices,
                        meshVertices: out _meshVertices,
                        customVertices: out _customVertices
                    );
                }
                catch (System.Exception e)
                {
                    Debug.Log("[#####]" + target.landmarks.Count);
                    Debug.LogError(e);
                    return;
                }

                // 보정
                List<Vector3> vertices = _meshVertices;

                (Vector3, Vector3) nosePointPair = CustomizeNose(vertices[174], vertices[399]);
                vertices[174] = nosePointPair.Item1;
                vertices[399] = nosePointPair.Item2;

                nosePointPair = CustomizeNose(vertices[198], vertices[420]);
                vertices[198] = nosePointPair.Item1;
                vertices[420] = nosePointPair.Item2;

                nosePointPair = CustomizeNose(vertices[209], vertices[429]);
                vertices[209] = nosePointPair.Item1;
                vertices[429] = nosePointPair.Item2;

                nosePointPair = CustomizeNose(vertices[129], vertices[358]);
                vertices[129] = nosePointPair.Item1;
                vertices[358] = nosePointPair.Item2;


                nosePointPair = CustomizeNose(vertices[131], vertices[360]);
                vertices[131] = nosePointPair.Item1;
                vertices[360] = nosePointPair.Item2;

                nosePointPair = CustomizeNose(vertices[49], vertices[279]);
                vertices[49] = nosePointPair.Item1;
                vertices[279] = nosePointPair.Item2;

                nosePointPair = CustomizeNose(vertices[102], vertices[331]);
                vertices[102] = nosePointPair.Item1;
                vertices[331] = nosePointPair.Item2;

                //
                //vertices[362] = Vector3.Lerp(vertices[362], vertices[463], customRatioEyesFirst);
                vertices[398] = Vector3.Lerp(vertices[398], vertices[414], customRatioEyesFirst);
                vertices[384] = Vector3.Lerp(vertices[384], vertices[286], customRatioEyesFirst);
                vertices[385] = Vector3.Lerp(vertices[385], vertices[258], customRatioEyesFirst);
                vertices[386] = Vector3.Lerp(vertices[386], vertices[257], customRatioEyesFirst);
                vertices[387] = Vector3.Lerp(vertices[387], vertices[259], customRatioEyesFirst);
                vertices[388] = Vector3.Lerp(vertices[388], vertices[260], customRatioEyesFirst);
                vertices[466] = Vector3.Lerp(vertices[466], vertices[467], customRatioEyesFirst);
                //vertices[263] = Vector3.Lerp(vertices[263], vertices[359], customRatioEyesFirst);
                //vertices[133] = Vector3.Lerp(vertices[133], vertices[243], customRatioEyesFirst);
                vertices[173] = Vector3.Lerp(vertices[173], vertices[190], customRatioEyesFirst);
                vertices[157] = Vector3.Lerp(vertices[157], vertices[56], customRatioEyesFirst);
                vertices[158] = Vector3.Lerp(vertices[158], vertices[28], customRatioEyesFirst);
                vertices[159] = Vector3.Lerp(vertices[159], vertices[27], customRatioEyesFirst);
                vertices[160] = Vector3.Lerp(vertices[160], vertices[29], customRatioEyesFirst);
                vertices[161] = Vector3.Lerp(vertices[161], vertices[30], customRatioEyesFirst);
                vertices[246] = Vector3.Lerp(vertices[246], vertices[247], customRatioEyesFirst);
                //vertices[33] = Vector3.Lerp(vertices[33], vertices[130], customRatioEyesFirst);

                //
                //vertices[463] = CustomizeEyes(vertices[362], vertices[463], vertices[464]);
                vertices[414] = CustomizeEyes(vertices[398], vertices[414], vertices[413]);
                vertices[286] = CustomizeEyes(vertices[384], vertices[286], vertices[441]);
                vertices[258] = CustomizeEyes(vertices[385], vertices[258], vertices[442]);
                vertices[257] = CustomizeEyes(vertices[386], vertices[257], vertices[443]);
                vertices[259] = CustomizeEyes(vertices[387], vertices[259], vertices[444]);
                vertices[260] = CustomizeEyes(vertices[388], vertices[260], vertices[445]);
                vertices[467] = CustomizeEyes(vertices[466], vertices[467], vertices[342]);
                //vertices[359] = CustomizeEyes(vertices[263], vertices[359], vertices[446]);
                //vertices[243] = CustomizeEyes(vertices[133], vertices[243], vertices[244]);
                vertices[190] = CustomizeEyes(vertices[173], vertices[190], vertices[189]);
                vertices[56] = CustomizeEyes(vertices[157], vertices[56], vertices[221]);
                vertices[28] = CustomizeEyes(vertices[158], vertices[28], vertices[222]);
                vertices[27] = CustomizeEyes(vertices[159], vertices[27], vertices[223]);
                vertices[29] = CustomizeEyes(vertices[160], vertices[29], vertices[224]);
                vertices[30] = CustomizeEyes(vertices[161], vertices[30], vertices[225]);
                vertices[247] = CustomizeEyes(vertices[246], vertices[247], vertices[113]);
                //vertices[130] = CustomizeEyes(vertices[33], vertices[130], vertices[226]);

                faceMeshFilter.mesh.SetVertices(vertices);
                customVertices = _customVertices;

                (Vector3, Vector3) CustomizeNose(Vector3 a, Vector3 b)
                {
                    Vector3 center = (a + b) * 0.5f;

                    return (Vector3.Lerp(center, a, customRatioNose), Vector3.Lerp(center, b, customRatioNose));
                }

                Vector3 CustomizeEyes(Vector3 a, Vector3 b, Vector3 c)
                {
                    Vector3 p1 = Vector3.Lerp(a, b, customRatioEyesSecond);
                    Vector3 p2 = Vector3.Lerp(b, c, customRatioEyesSecond);

                    return Vector3.Lerp(p1, p2, customRatioEyesSecond);
                }

                Quaternion GetRotation(Vector3 from, Vector3 to)
                {
                    Vector3 direction = to - from;
                    return Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }
        }
        private void DrawCustomLandmarkList(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false)
        {
            _customLandmarkListAnnotation.Draw(target, visualizeZ);
        }

        private void DrawCustomLandmarkList(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
        {
            _customLandmarkListAnnotation.Draw(target, visualizeZ);
        }
        private List<Vector3> DrawCustomLandmarkList_Custom(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
        {
            return _customLandmarkListAnnotation.Draw_Custom(target, visualizeZ);
        }
        private void DrawFaceLandmarkList(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false)
        {
            _faceLandmarkListAnnotation.Draw(target, visualizeZ);
        }

        private void DrawFaceLandmarkList(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
        {
            _faceLandmarkListAnnotation.Draw(target, visualizeZ);
        }

        private List<Vector3> DrawFaceLandmarkList_Custom(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
        {
            return _faceLandmarkListAnnotation.Draw_Custom(target, visualizeZ);
        }
        private void DrawLeftIrisLandmarkList(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            // does not deactivate if the target is null as long as face landmarks are present.
            _leftIrisLandmarkListAnnotation.Draw(target, visualizeZ, circleVertices);
        }

        private void DrawLeftIrisLandmarkList(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            // does not deactivate if the target is null as long as face landmarks are present.
            _leftIrisLandmarkListAnnotation.Draw(target, visualizeZ, circleVertices);
        }

        private void DrawRightIrisLandmarkList(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            // does not deactivate if the target is null as long as face landmarks are present.
            _rightIrisLandmarkListAnnotation.Draw(target, visualizeZ, circleVertices);
        }

        private void DrawRightIrisLandmarkList(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false, int circleVertices = 128)
        {
            // does not deactivate if the target is null as long as face landmarks are present.
            _rightIrisLandmarkListAnnotation.Draw(target, visualizeZ, circleVertices);
        }

        private static (IReadOnlyList<T>, IReadOnlyList<T>, IReadOnlyList<T>) PartitionLandmarkList<T>(IReadOnlyList<T> landmarks)
        {
            if (landmarks == null)
            {
                return (null, null, null);
            }

            var enumerator = landmarks.GetEnumerator();
            var faceLandmarks = new List<T>(_FaceLandmarkCount);
            for (var i = 0; i < _FaceLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    faceLandmarks.Add(enumerator.Current);
                }
            }
            if (faceLandmarks.Count < _FaceLandmarkCount)
            {
                return (null, null, null);
            }

            var leftIrisLandmarks = new List<T>(_IrisLandmarkCount);
            for (var i = 0; i < _IrisLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    leftIrisLandmarks.Add(enumerator.Current);
                }
            }
            if (leftIrisLandmarks.Count < _IrisLandmarkCount)
            {
                return (faceLandmarks, null, null);
            }

            var rightIrisLandmarks = new List<T>(_IrisLandmarkCount);
            for (var i = 0; i < _IrisLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    rightIrisLandmarks.Add(enumerator.Current);
                }
            }
            return rightIrisLandmarks.Count < _IrisLandmarkCount ? (faceLandmarks, leftIrisLandmarks, null) : (faceLandmarks, leftIrisLandmarks, rightIrisLandmarks);
        }
        //
        private static (IReadOnlyList<T>, IReadOnlyList<T>, IReadOnlyList<T>, IReadOnlyList<T>) PartitionLandmarkList_custom<T>(IReadOnlyList<T> landmarks)
        {
            if (landmarks == null)
            {
                return (null, null, null, null);
            }

            var enumerator = landmarks.ToList().GetEnumerator();
            var faceLandmarks = new List<T>(_FaceLandmarkCount);
            for (var i = 0; i < _FaceLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    faceLandmarks.Add(enumerator.Current);
                }
            }
            if (faceLandmarks.Count < _FaceLandmarkCount)
            {
                return (null, null, null, null);
            }

            var leftIrisLandmarks = new List<T>(_IrisLandmarkCount);
            for (var i = 0; i < _IrisLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    leftIrisLandmarks.Add(enumerator.Current);
                }
            }
            if (leftIrisLandmarks.Count < _IrisLandmarkCount)
            {
                return (faceLandmarks, null, null, null);
            }

            var rightIrisLandmarks = new List<T>(_IrisLandmarkCount);
            for (var i = 0; i < _IrisLandmarkCount; i++)
            {
                if (enumerator.MoveNext())
                {
                    rightIrisLandmarks.Add(enumerator.Current);
                }
            }

            if (rightIrisLandmarks.Count < _IrisLandmarkCount)
            {
                return (faceLandmarks, leftIrisLandmarks, null, null);
            }

            var customLandmarks = new List<T>(1);
            for (var i = 0; i < 1; i++)
            {
                if (enumerator.MoveNext())
                {
                    customLandmarks.Add(enumerator.Current);
                }
            }

            return customLandmarks.Count < 1 ? (faceLandmarks, leftIrisLandmarks, rightIrisLandmarks, null) : (faceLandmarks, leftIrisLandmarks, rightIrisLandmarks, customLandmarks);
        }
        //private readonly List<int> _lines = new List<int> {10, 338, 338, 297, 297, 332, 332, 284, 284, 251, 251, 389, 389, 356, 356, 454, 454, 323, 323, 361, 361, 288, 288, 397, 397, 365, 365, 379, 379, 378, 378, 400, 400, 377, 377, 152, 152, 148, 148, 176, 176, 149, 149, 150, 150, 136, 136, 172, 172, 58, 58, 132, 132, 93, 93, 234, 234, 127, 127, 162, 162, 21, 21, 54, 54, 103, 103, 67, 67, 109, 109, 10, 33, 7, 7, 163, 163, 144, 144, 145, 145, 153, 153, 154, 154, 155, 155, 133, 33, 246, 246, 161, 161, 160, 160, 159, 159, 158, 158, 157, 157, 173, 173, 133, 46, 53, 53, 52, 52, 65, 65, 55, 70, 63, 63, 105, 105, 66, 66, 107, 263, 249, 249, 390, 390, 373, 373, 374, 374, 380, 380, 381, 381, 382, 382, 362, 263, 466, 466, 388, 388, 387, 387, 386, 386, 385, 385, 384, 384, 398, 398, 362, 276, 283, 283, 282, 282, 295, 295, 285, 300, 293, 293, 334, 334, 296, 296, 336, 78, 95, 95, 88, 88, 178, 178, 87, 87, 14, 14, 317, 317, 402, 402, 318, 318, 324, 324, 308, 78, 191, 191, 80, 80, 81, 81, 82, 82, 13, 13, 312, 312, 311, 311, 310, 310, 415, 415, 308, 61, 146, 146, 91, 91, 181, 181, 84, 84, 17, 17, 314, 314, 405, 405, 321, 321, 375, 375, 291, 61, 185, 185, 40, 40, 39, 39, 37, 37, 0, 0, 267, 267, 269, 269, 270, 270, 409, 409, 291,
        //};
        //private readonly List<int> _triangles = new List<int> { 173, 155, 133, 246, 33, 7, 382, 398, 362, 263, 466, 249, 308, 415, 324, 78, 95, 191, 356, 389, 264, 127, 34, 162, 368, 264, 389, 139, 162, 34, 267, 0, 302, 37, 72, 0, 11, 302, 0, 11, 0, 72, 349, 451, 350, 120, 121, 231, 452, 350, 451, 232, 231, 121, 267, 302, 269, 37, 39, 72, 303, 269, 302, 73, 72, 39, 357, 343, 350, 128, 121, 114, 277, 350, 343, 47, 114, 121, 350, 452, 357, 121, 128, 232, 453, 357, 452, 233, 232, 128, 299, 333, 297, 69, 67, 104, 332, 297, 333, 103, 104, 67, 175, 152, 396, 175, 171, 152, 377, 396, 152, 148, 152, 171, 381, 384, 382, 154, 155, 157, 398, 382, 384, 173, 157, 155, 280, 347, 330, 50, 101, 118, 348, 330, 347, 119, 118, 101, 269, 303, 270, 39, 40, 73, 304, 270, 303, 74, 73, 40, 9, 336, 151, 9, 151, 107, 337, 151, 336, 108, 107, 151, 344, 278, 360, 115, 131, 48, 279, 360, 278, 49, 48, 131, 262, 431, 418, 32, 194, 211, 424, 418, 431, 204, 211, 194, 304, 408, 270, 74, 40, 184, 409, 270, 408, 185, 184, 40, 272, 310, 407, 42, 183, 80, 415, 407, 310, 191, 80, 183, 322, 270, 410, 92, 186, 40, 409, 410, 270, 185, 40, 186, 347, 449, 348, 118, 119, 229, 450, 348, 449, 230, 229, 119, 434, 432, 430, 214, 210, 212, 422, 430, 432, 202, 212, 210, 313, 314, 18, 83, 18, 84, 17, 18, 314, 17, 84, 18, 307, 375, 306, 77, 76, 146, 291, 306, 375, 61, 146, 76, 259, 387, 260, 29, 30, 160, 388, 260, 387, 161, 160, 30, 286, 414, 384, 56, 157, 190, 398, 384, 414, 173, 190, 157, 418, 424, 406, 194, 182, 204, 335, 406, 424, 106, 204, 182, 367, 416, 364, 138, 135, 192, 434, 364, 416, 214, 192, 135, 391, 423, 327, 165, 98, 203, 358, 327, 423, 129, 203, 98, 298, 301, 284, 68, 54, 71, 251, 284, 301, 21, 71, 54, 4, 275, 5, 4, 5, 45, 281, 5, 275, 51, 45, 5, 254, 373, 253, 24, 23, 144, 374, 253, 373, 145, 144, 23, 320, 321, 307, 90, 77, 91, 375, 307, 321, 146, 91, 77, 280, 425, 411, 50, 187, 205, 427, 411, 425, 207, 205, 187, 421, 313, 200, 201, 200, 83, 18, 200, 313, 18, 83, 200, 335, 321, 406, 106, 182, 91, 405, 406, 321, 181, 91, 182, 405, 321, 404, 181, 180, 91, 320, 404, 321, 90, 91, 180, 17, 314, 16, 17, 16, 84, 315, 16, 314, 85, 84, 16, 425, 266, 426, 205, 206, 36, 423, 426, 266, 203, 36, 206, 369, 396, 400, 140, 176, 171, 377, 400, 396, 148, 171, 176, 391, 269, 322, 165, 92, 39, 270, 322, 269, 40, 39, 92, 417, 465, 413, 193, 189, 245, 464, 413, 465, 244, 245, 189, 257, 258, 386, 27, 159, 28, 385, 386, 258, 158, 28, 159, 260, 388, 467, 30, 247, 161, 466, 467, 388, 246, 161, 247, 248, 456, 419, 3, 196, 236, 399, 419, 456, 174, 236, 196, 333, 298, 332, 104, 103, 68, 284, 332, 298, 54, 68, 103, 285, 8, 417, 55, 193, 8, 168, 417, 8, 168, 8, 193, 340, 261, 346, 111, 117, 31, 448, 346, 261, 228, 31, 117, 285, 417, 441, 55, 221, 193, 413, 441, 417, 189, 193, 221, 327, 460, 326, 98, 97, 240, 328, 326, 460, 99, 240, 97, 277, 355, 329, 47, 100, 126, 371, 329, 355, 142, 126, 100, 309, 392, 438, 79, 218, 166, 439, 438, 392, 219, 166, 218, 381, 382, 256, 154, 26, 155, 341, 256, 382, 112, 155, 26, 360, 279, 420, 131, 198, 49, 429, 420, 279, 209, 49, 198, 365, 364, 379, 136, 150, 135, 394, 379, 364, 169, 135, 150, 355, 277, 437, 126, 217, 47, 343, 437, 277, 114, 47, 217, 443, 444, 282, 223, 52, 224, 283, 282, 444, 53, 224, 52, 281, 275, 363, 51, 134, 45, 440, 363, 275, 220, 45, 134, 431, 262, 395, 211, 170, 32, 369, 395, 262, 140, 32, 170, 337, 299, 338, 108, 109, 69, 297, 338, 299, 67, 69, 109, 335, 273, 321, 106, 91, 43, 375, 321, 273, 146, 43, 91, 348, 450, 349, 119, 120, 230, 451, 349, 450, 231, 230, 120, 467, 359, 342, 247, 113, 130, 446, 342, 359, 226, 130, 113, 282, 283, 334, 52, 105, 53, 293, 334, 283, 63, 53, 105, 250, 458, 462, 20, 242, 238, 461, 462, 458, 241, 238, 242, 276, 353, 300, 46, 70, 124, 383, 300, 353, 156, 124, 70, 325, 292, 324, 96, 95, 62, 308, 324, 292, 78, 62, 95, 283, 276, 293, 53, 63, 46, 300, 293, 276, 70, 46, 63, 447, 264, 345, 227, 116, 34, 372, 345, 264, 143, 34, 116, 352, 345, 346, 123, 117, 116, 340, 346, 345, 111, 116, 117, 1, 19, 274, 1, 44, 19, 354, 274, 19, 125, 19, 44, 248, 281, 456, 3, 236, 51, 363, 456, 281, 134, 51, 236, 425, 426, 427, 205, 207, 206, 436, 427, 426, 216, 206, 207, 380, 381, 252, 153, 22, 154, 256, 252, 381, 26, 154, 22, 391, 393, 269, 165, 39, 167, 267, 269, 393, 37, 167, 39, 199, 428, 200, 199, 200, 208, 421, 200, 428, 201, 208, 200, 330, 329, 266, 101, 36, 100, 371, 266, 329, 142, 100, 36, 422, 432, 273, 202, 43, 212, 287, 273, 432, 57, 212, 43, 290, 250, 328, 60, 99, 20, 462, 328, 250, 242, 20, 99, 258, 286, 385, 28, 158, 56, 384, 385, 286, 157, 56, 158, 342, 446, 353, 113, 124, 226, 265, 353, 446, 35, 226, 124, 257, 386, 259, 27, 29, 159, 387, 259, 386, 160, 159, 29, 430, 422, 431, 210, 211, 202, 424, 431, 422, 204, 202, 211, 445, 342, 276, 225, 46, 113, 353, 276, 342, 124, 113, 46, 424, 422, 335, 204, 106, 202, 273, 335, 422, 43, 202, 106, 306, 292, 307, 76, 77, 62, 325, 307, 292, 96, 62, 77, 366, 447, 352, 137, 123, 227, 345, 352, 447, 116, 227, 123, 302, 268, 303, 72, 73, 38, 271, 303, 268, 41, 38, 73, 371, 358, 266, 142, 36, 129, 423, 266, 358, 203, 129, 36, 327, 294, 460, 98, 240, 64, 455, 460, 294, 235, 64, 240, 294, 331, 278, 64, 48, 102, 279, 278, 331, 49, 102, 48, 303, 271, 304, 73, 74, 41, 272, 304, 271, 42, 41, 74, 427, 436, 434, 207, 214, 216, 432, 434, 436, 212, 216, 214, 304, 272, 408, 74, 184, 42, 407, 408, 272, 183, 42, 184, 394, 430, 395, 169, 170, 210, 431, 395, 430, 211, 210, 170, 395, 369, 378, 170, 149, 140, 400, 378, 369, 176, 140, 149, 296, 334, 299, 66, 69, 105, 333, 299, 334, 104, 105, 69, 417, 168, 351, 193, 122, 168, 6, 351, 168, 6, 168, 122, 280, 411, 352, 50, 123, 187, 376, 352, 411, 147, 187, 123, 319, 320, 325, 89, 96, 90, 307, 325, 320, 77, 90, 96, 285, 295, 336, 55, 107, 65, 296, 336, 295, 66, 65, 107, 404, 320, 403, 180, 179, 90, 319, 403, 320, 89, 90, 179, 330, 348, 329, 101, 100, 119, 349, 329, 348, 120, 119, 100, 334, 293, 333, 105, 104, 63, 298, 333, 293, 68, 63, 104, 323, 454, 366, 93, 137, 234, 447, 366, 454, 227, 234, 137, 16, 315, 15, 16, 15, 85, 316, 15, 315, 86, 85, 15, 429, 279, 358, 209, 129, 49, 331, 358, 279, 102, 49, 129, 15, 316, 14, 15, 14, 86, 317, 14, 316, 87, 86, 14, 8, 285, 9, 8, 9, 55, 336, 9, 285, 107, 55, 9, 329, 349, 277, 100, 47, 120, 350, 277, 349, 121, 120, 47, 252, 253, 380, 22, 153, 23, 374, 380, 253, 145, 23, 153, 402, 403, 318, 178, 88, 179, 319, 318, 403, 89, 179, 88, 351, 6, 419, 122, 196, 6, 197, 419, 6, 197, 6, 196, 324, 318, 325, 95, 96, 88, 319, 325, 318, 89, 88, 96, 397, 367, 365, 172, 136, 138, 364, 365, 367, 135, 138, 136, 288, 435, 397, 58, 172, 215, 367, 397, 435, 138, 215, 172, 438, 439, 344, 218, 115, 219, 278, 344, 439, 48, 219, 115, 271, 311, 272, 41, 42, 81, 310, 272, 311, 80, 81, 42, 5, 281, 195, 5, 195, 51, 248, 195, 281, 3, 51, 195, 273, 287, 375, 43, 146, 57, 291, 375, 287, 61, 57, 146, 396, 428, 175, 171, 175, 208, 199, 175, 428, 199, 208, 175, 268, 312, 271, 38, 41, 82, 311, 271, 312, 81, 82, 41, 444, 445, 283, 224, 53, 225, 276, 283, 445, 46, 225, 53, 254, 339, 373, 24, 144, 110, 390, 373, 339, 163, 110, 144, 295, 282, 296, 65, 66, 52, 334, 296, 282, 105, 52, 66, 346, 448, 347, 117, 118, 228, 449, 347, 448, 229, 228, 118, 454, 356, 447, 234, 227, 127, 264, 447, 356, 34, 127, 227, 336, 296, 337, 107, 108, 66, 299, 337, 296, 69, 66, 108, 151, 337, 10, 151, 10, 108, 338, 10, 337, 109, 108, 10, 278, 439, 294, 48, 64, 219, 455, 294, 439, 235, 219, 64, 407, 415, 292, 183, 62, 191, 308, 292, 415, 78, 191, 62, 358, 371, 429, 129, 209, 142, 355, 429, 371, 126, 142, 209, 345, 372, 340, 116, 111, 143, 265, 340, 372, 35, 143, 111, 388, 390, 466, 161, 246, 163, 249, 466, 390, 7, 163, 246, 352, 346, 280, 123, 50, 117, 347, 280, 346, 118, 117, 50, 295, 442, 282, 65, 52, 222, 443, 282, 442, 223, 222, 52, 19, 94, 354, 19, 125, 94, 370, 354, 94, 141, 94, 125, 295, 285, 442, 65, 222, 55, 441, 442, 285, 221, 55, 222, 419, 197, 248, 196, 3, 197, 195, 248, 197, 195, 197, 3, 359, 263, 255, 130, 25, 33, 249, 255, 263, 7, 33, 25, 275, 274, 440, 45, 220, 44, 457, 440, 274, 237, 44, 220, 300, 383, 301, 70, 71, 156, 368, 301, 383, 139, 156, 71, 417, 351, 465, 193, 245, 122, 412, 465, 351, 188, 122, 245, 466, 263, 467, 246, 247, 33, 359, 467, 263, 130, 33, 247, 389, 251, 368, 162, 139, 21, 301, 368, 251, 71, 21, 139, 374, 386, 380, 145, 153, 159, 385, 380, 386, 158, 159, 153, 379, 394, 378, 150, 149, 169, 395, 378, 394, 170, 169, 149, 351, 419, 412, 122, 188, 196, 399, 412, 419, 174, 196, 188, 426, 322, 436, 206, 216, 92, 410, 436, 322, 186, 92, 216, 387, 373, 388, 160, 161, 144, 390, 388, 373, 163, 144, 161, 393, 326, 164, 167, 164, 97, 2, 164, 326, 2, 97, 164, 354, 370, 461, 125, 241, 141, 462, 461, 370, 242, 141, 241, 0, 267, 164, 0, 164, 37, 393, 164, 267, 167, 37, 164, 11, 12, 302, 11, 72, 12, 268, 302, 12, 38, 12, 72, 386, 374, 387, 159, 160, 145, 373, 387, 374, 144, 145, 160, 12, 13, 268, 12, 38, 13, 312, 268, 13, 82, 13, 38, 293, 300, 298, 63, 68, 70, 301, 298, 300, 71, 70, 68, 340, 265, 261, 111, 31, 35, 446, 261, 265, 226, 35, 31, 380, 385, 381, 153, 154, 158, 384, 381, 385, 157, 158, 154, 280, 330, 425, 50, 205, 101, 266, 425, 330, 36, 101, 205, 423, 391, 426, 203, 206, 165, 322, 426, 391, 92, 165, 206, 429, 355, 420, 209, 198, 126, 437, 420, 355, 217, 126, 198, 391, 327, 393, 165, 167, 98, 326, 393, 327, 97, 98, 167, 457, 438, 440, 237, 220, 218, 344, 440, 438, 115, 218, 220, 382, 362, 341, 155, 112, 133, 463, 341, 362, 243, 133, 112, 457, 461, 459, 237, 239, 241, 458, 459, 461, 238, 241, 239, 434, 430, 364, 214, 135, 210, 394, 364, 430, 169, 210, 135, 414, 463, 398, 190, 173, 243, 362, 398, 463, 133, 243, 173, 262, 428, 369, 32, 140, 208, 396, 369, 428, 171, 208, 140, 457, 274, 461, 237, 241, 44, 354, 461, 274, 125, 44, 241, 316, 403, 317, 86, 87, 179, 402, 317, 403, 178, 179, 87, 315, 404, 316, 85, 86, 180, 403, 316, 404, 179, 180, 86, 314, 405, 315, 84, 85, 181, 404, 315, 405, 180, 181, 85, 313, 406, 314, 83, 84, 182, 405, 314, 406, 181, 182, 84, 418, 406, 421, 194, 201, 182, 313, 421, 406, 83, 182, 201, 366, 401, 323, 137, 93, 177, 361, 323, 401, 132, 177, 93, 408, 407, 306, 184, 76, 183, 292, 306, 407, 62, 183, 76, 408, 306, 409, 184, 185, 76, 291, 409, 306, 61, 76, 185, 410, 409, 287, 186, 57, 185, 291, 287, 409, 61, 185, 57, 436, 410, 432, 216, 212, 186, 287, 432, 410, 57, 186, 212, 434, 416, 427, 214, 207, 192, 411, 427, 416, 187, 192, 207, 264, 368, 372, 34, 143, 139, 383, 372, 368, 156, 139, 143, 457, 459, 438, 237, 218, 239, 309, 438, 459, 79, 239, 218, 352, 376, 366, 123, 137, 147, 401, 366, 376, 177, 147, 137, 4, 1, 275, 4, 45, 1, 274, 275, 1, 44, 1, 45, 428, 262, 421, 208, 201, 32, 418, 421, 262, 194, 32, 201, 327, 358, 294, 98, 64, 129, 331, 294, 358, 102, 129, 64, 367, 435, 416, 138, 192, 215, 433, 416, 435, 213, 215, 192, 455, 439, 289, 235, 59, 219, 392, 289, 439, 166, 219, 59, 328, 462, 326, 99, 97, 242, 370, 326, 462, 141, 242, 97, 326, 370, 2, 97, 2, 141, 94, 2, 370, 94, 141, 2, 460, 455, 305, 240, 75, 235, 289, 305, 455, 59, 235, 75, 448, 339, 449, 228, 229, 110, 254, 449, 339, 24, 110, 229, 261, 446, 255, 31, 25, 226, 359, 255, 446, 130, 226, 25, 449, 254, 450, 229, 230, 24, 253, 450, 254, 23, 24, 230, 450, 253, 451, 230, 231, 23, 252, 451, 253, 22, 23, 231, 451, 252, 452, 231, 232, 22, 256, 452, 252, 26, 22, 232, 256, 341, 452, 26, 232, 112, 453, 452, 341, 233, 112, 232, 413, 464, 414, 189, 190, 244, 463, 414, 464, 243, 244, 190, 441, 413, 286, 221, 56, 189, 414, 286, 413, 190, 189, 56, 441, 286, 442, 221, 222, 56, 258, 442, 286, 28, 56, 222, 442, 258, 443, 222, 223, 28, 257, 443, 258, 27, 28, 223, 444, 443, 259, 224, 29, 223, 257, 259, 443, 27, 223, 29, 259, 260, 444, 29, 224, 30, 445, 444, 260, 225, 30, 224, 260, 467, 445, 30, 225, 247, 342, 445, 467, 113, 247, 225, 250, 309, 458, 20, 238, 79, 459, 458, 309, 239, 79, 238, 290, 305, 392, 60, 166, 75, 289, 392, 305, 59, 75, 166, 460, 305, 328, 240, 99, 75, 290, 328, 305, 60, 75, 99, 376, 433, 401, 147, 177, 213, 435, 401, 433, 215, 213, 177, 250, 290, 309, 20, 79, 60, 392, 309, 290, 166, 60, 79, 411, 416, 376, 187, 147, 192, 433, 376, 416, 213, 192, 147, 341, 463, 453, 112, 233, 243, 464, 453, 463, 244, 243, 233, 453, 464, 357, 233, 128, 244, 465, 357, 464, 245, 244, 128, 412, 343, 465, 188, 245, 114, 357, 465, 343, 128, 114, 245, 437, 343, 399, 217, 174, 114, 412, 399, 343, 188, 114, 174, 363, 440, 360, 134, 131, 220, 344, 360, 440, 115, 220, 131, 456, 420, 399, 236, 174, 198, 437, 399, 420, 217, 198, 174, 456, 363, 420, 236, 198, 134, 360, 420, 363, 131, 134, 198, 361, 401, 288, 132, 58, 177, 435, 288, 401, 215, 177, 58, 353, 265, 383, 124, 156, 35, 372, 383, 265, 143, 35, 156, 255, 249, 339, 25, 110, 7, 390, 339, 249, 163, 7, 110, 261, 255, 448, 31, 228, 25, 339, 448, 255, 110, 25, 228, 14, 317, 13, 14, 13, 87, 312, 13, 317, 82, 87, 13, 317, 402, 312, 87, 82, 178, 311, 312, 402, 81, 178, 82, 402, 318, 311, 178, 81, 88, 310, 311, 318, 80, 88, 81, 318, 324, 310, 88, 80, 95, 415, 310, 324, 191, 95, 80 };
        private readonly List<int> _triangles_reverse = new List<int> { 80, 95, 191, 324, 310, 415, 95, 80, 88, 310, 324, 318, 81, 88, 80, 318, 311, 310, 88, 81, 178, 311, 318, 402, 82, 178, 81, 402, 312, 311, 178, 82, 87, 312, 402, 317, 13, 87, 82, 317, 13, 312, 87, 13, 14, 13, 317, 14, 228, 25, 110, 255, 448, 339, 25, 228, 31, 448, 255, 261, 110, 7, 163, 249, 339, 390, 7, 110, 25, 339, 249, 255, 156, 35, 143, 265, 383, 372, 35, 156, 124, 383, 265, 353, 58, 177, 215, 401, 288, 435, 177, 58, 132, 288, 401, 361, 198, 134, 131, 363, 420, 360, 134, 198, 236, 420, 363, 456, 174, 198, 217, 420, 399, 437, 198, 174, 236, 399, 420, 456, 131, 220, 115, 440, 360, 344, 220, 131, 134, 360, 440, 363, 174, 114, 188, 343, 399, 412, 114, 174, 217, 399, 343, 437, 245, 114, 128, 343, 465, 357, 114, 245, 188, 465, 343, 412, 128, 244, 245, 464, 357, 465, 244, 128, 233, 357, 464, 453, 233, 243, 244, 463, 453, 464, 243, 233, 112, 453, 463, 341, 147, 192, 213, 416, 376, 433, 192, 147, 187, 376, 416, 411, 79, 60, 166, 290, 309, 392, 60, 79, 20, 309, 290, 250, 177, 213, 215, 433, 401, 435, 213, 177, 147, 401, 433, 376, 99, 75, 60, 305, 328, 290, 75, 99, 240, 328, 305, 460, 166, 75, 59, 305, 392, 289, 75, 166, 60, 392, 305, 290, 238, 79, 239, 309, 458, 459, 79, 238, 20, 458, 309, 250, 225, 247, 113, 467, 445, 342, 247, 225, 30, 445, 467, 260, 224, 30, 225, 260, 444, 445, 30, 224, 29, 444, 260, 259, 29, 223, 27, 443, 259, 257, 223, 29, 224, 259, 443, 444, 223, 28, 27, 258, 443, 257, 28, 223, 222, 443, 258, 442, 222, 56, 28, 286, 442, 258, 56, 222, 221, 442, 286, 441, 56, 189, 190, 413, 286, 414, 189, 56, 221, 286, 413, 441, 190, 244, 243, 464, 414, 463, 244, 190, 189, 414, 464, 413, 232, 112, 233, 341, 452, 453, 112, 232, 26, 452, 341, 256, 232, 22, 26, 252, 452, 256, 22, 232, 231, 452, 252, 451, 231, 23, 22, 253, 451, 252, 23, 231, 230, 451, 253, 450, 230, 24, 23, 254, 450, 253, 24, 230, 229, 450, 254, 449, 25, 226, 130, 446, 255, 359, 226, 25, 31, 255, 446, 261, 229, 110, 24, 339, 449, 254, 110, 229, 228, 449, 339, 448, 75, 235, 59, 455, 305, 289, 235, 75, 240, 305, 455, 460, 2, 141, 94, 370, 2, 94, 141, 2, 97, 2, 370, 326, 97, 242, 141, 462, 326, 370, 242, 97, 99, 326, 462, 328, 59, 219, 166, 439, 289, 392, 219, 59, 235, 289, 439, 455, 192, 215, 213, 435, 416, 433, 215, 192, 138, 416, 435, 367, 64, 129, 102, 358, 294, 331, 129, 64, 98, 294, 358, 327, 201, 32, 194, 262, 421, 418, 32, 201, 208, 421, 262, 428, 45, 1, 44, 1, 275, 274, 1, 45, 4, 275, 1, 4, 137, 147, 177, 376, 366, 401, 147, 137, 123, 366, 376, 352, 218, 239, 79, 459, 438, 309, 239, 218, 237, 438, 459, 457, 143, 139, 156, 368, 372, 383, 139, 143, 34, 372, 368, 264, 207, 192, 187, 416, 427, 411, 192, 207, 214, 427, 416, 434, 212, 186, 57, 410, 432, 287, 186, 212, 216, 432, 410, 436, 57, 185, 61, 409, 287, 291, 185, 57, 186, 287, 409, 410, 185, 76, 61, 306, 409, 291, 76, 185, 184, 409, 306, 408, 76, 183, 62, 407, 306, 292, 183, 76, 184, 306, 407, 408, 93, 177, 132, 401, 323, 361, 177, 93, 137, 323, 401, 366, 201, 182, 83, 406, 421, 313, 182, 201, 194, 421, 406, 418, 84, 182, 181, 406, 314, 405, 182, 84, 83, 314, 406, 313, 85, 181, 180, 405, 315, 404, 181, 85, 84, 315, 405, 314, 86, 180, 179, 404, 316, 403, 180, 86, 85, 316, 404, 315, 87, 179, 178, 403, 317, 402, 179, 87, 86, 317, 403, 316, 241, 44, 125, 274, 461, 354, 44, 241, 237, 461, 274, 457, 140, 208, 171, 428, 369, 396, 208, 140, 32, 369, 428, 262, 173, 243, 133, 463, 398, 362, 243, 173, 190, 398, 463, 414, 135, 210, 169, 430, 364, 394, 210, 135, 214, 364, 430, 434, 239, 241, 238, 461, 459, 458, 241, 239, 237, 459, 461, 457, 112, 133, 243, 362, 341, 463, 133, 112, 155, 341, 362, 382, 220, 218, 115, 438, 440, 344, 218, 220, 237, 440, 438, 457, 167, 98, 97, 327, 393, 326, 98, 167, 165, 393, 327, 391, 198, 126, 217, 355, 420, 437, 126, 198, 209, 420, 355, 429, 206, 165, 92, 391, 426, 322, 165, 206, 203, 426, 391, 423, 205, 101, 36, 330, 425, 266, 101, 205, 50, 425, 330, 280, 154, 158, 157, 385, 381, 384, 158, 154, 153, 381, 385, 380, 31, 35, 226, 265, 261, 446, 35, 31, 111, 261, 265, 340, 68, 70, 71, 300, 298, 301, 70, 68, 63, 298, 300, 293, 38, 13, 82, 13, 268, 312, 13, 38, 12, 268, 13, 12, 160, 145, 144, 374, 387, 373, 145, 160, 159, 387, 374, 386, 72, 12, 38, 12, 302, 268, 12, 72, 11, 302, 12, 11, 164, 37, 167, 267, 164, 393, 37, 164, 0, 164, 267, 0, 241, 141, 242, 370, 461, 462, 141, 241, 125, 461, 370, 354, 164, 97, 2, 326, 164, 2, 97, 164, 167, 164, 326, 393, 161, 144, 163, 373, 388, 390, 144, 161, 160, 388, 373, 387, 216, 92, 186, 322, 436, 410, 92, 216, 206, 436, 322, 426, 188, 196, 174, 419, 412, 399, 196, 188, 122, 412, 419, 351, 149, 169, 170, 394, 378, 395, 169, 149, 150, 378, 394, 379, 153, 159, 158, 386, 380, 385, 159, 153, 145, 380, 386, 374, 139, 21, 71, 251, 368, 301, 21, 139, 162, 368, 251, 389, 247, 33, 130, 263, 467, 359, 33, 247, 246, 467, 263, 466, 245, 122, 188, 351, 465, 412, 122, 245, 193, 465, 351, 417, 71, 156, 139, 383, 301, 368, 156, 71, 70, 301, 383, 300, 220, 44, 237, 274, 440, 457, 44, 220, 45, 440, 274, 275, 25, 33, 7, 263, 255, 249, 33, 25, 130, 255, 263, 359, 3, 197, 195, 197, 248, 195, 197, 3, 196, 248, 197, 419, 222, 55, 221, 285, 442, 441, 55, 222, 65, 442, 285, 295, 125, 94, 141, 94, 354, 370, 94, 125, 19, 354, 94, 19, 52, 222, 223, 442, 282, 443, 222, 52, 65, 282, 442, 295, 50, 117, 118, 346, 280, 347, 117, 50, 123, 280, 346, 352, 246, 163, 7, 390, 466, 249, 163, 246, 161, 466, 390, 388, 111, 143, 35, 372, 340, 265, 143, 111, 116, 340, 372, 345, 209, 142, 126, 371, 429, 355, 142, 209, 129, 429, 371, 358, 62, 191, 78, 415, 292, 308, 191, 62, 183, 292, 415, 407, 64, 219, 235, 439, 294, 455, 219, 64, 48, 294, 439, 278, 10, 108, 109, 337, 10, 338, 108, 10, 151, 10, 337, 151, 108, 66, 69, 296, 337, 299, 66, 108, 107, 337, 296, 336, 227, 127, 34, 356, 447, 264, 127, 227, 234, 447, 356, 454, 118, 228, 229, 448, 347, 449, 228, 118, 117, 347, 448, 346, 66, 52, 105, 282, 296, 334, 52, 66, 65, 296, 282, 295, 144, 110, 163, 339, 373, 390, 110, 144, 24, 373, 339, 254, 53, 225, 46, 445, 283, 276, 225, 53, 224, 283, 445, 444, 41, 82, 81, 312, 271, 311, 82, 41, 38, 271, 312, 268, 175, 208, 199, 428, 175, 199, 208, 175, 171, 175, 428, 396, 146, 57, 61, 287, 375, 291, 57, 146, 43, 375, 287, 273, 195, 51, 3, 281, 195, 248, 51, 195, 5, 195, 281, 5, 42, 81, 80, 311, 272, 310, 81, 42, 41, 272, 311, 271, 115, 219, 48, 439, 344, 278, 219, 115, 218, 344, 439, 438, 172, 215, 138, 435, 397, 367, 215, 172, 58, 397, 435, 288, 136, 138, 135, 367, 365, 364, 138, 136, 172, 365, 367, 397, 96, 88, 89, 318, 325, 319, 88, 96, 95, 325, 318, 324, 196, 6, 197, 6, 419, 197, 6, 196, 122, 419, 6, 351, 88, 179, 89, 403, 318, 319, 179, 88, 178, 318, 403, 402, 153, 23, 145, 253, 380, 374, 23, 153, 22, 380, 253, 252, 47, 120, 121, 349, 277, 350, 120, 47, 100, 277, 349, 329, 9, 55, 107, 285, 9, 336, 55, 9, 8, 9, 285, 8, 14, 86, 87, 316, 14, 317, 86, 14, 15, 14, 316, 15, 129, 49, 102, 279, 358, 331, 49, 129, 209, 358, 279, 429, 15, 85, 86, 315, 15, 316, 85, 15, 16, 15, 315, 16, 137, 234, 227, 454, 366, 447, 234, 137, 93, 366, 454, 323, 104, 63, 68, 293, 333, 298, 63, 104, 105, 333, 293, 334, 100, 119, 120, 348, 329, 349, 119, 100, 101, 329, 348, 330, 179, 90, 89, 320, 403, 319, 90, 179, 180, 403, 320, 404, 107, 65, 66, 295, 336, 296, 65, 107, 55, 336, 295, 285, 96, 90, 77, 320, 325, 307, 90, 96, 89, 325, 320, 319, 123, 187, 147, 411, 352, 376, 187, 123, 50, 352, 411, 280, 122, 168, 6, 168, 351, 6, 168, 122, 193, 351, 168, 417, 69, 105, 104, 334, 299, 333, 105, 69, 66, 299, 334, 296, 149, 140, 176, 369, 378, 400, 140, 149, 170, 378, 369, 395, 170, 210, 211, 430, 395, 431, 210, 170, 169, 395, 430, 394, 184, 42, 183, 272, 408, 407, 42, 184, 74, 408, 272, 304, 214, 216, 212, 436, 434, 432, 216, 214, 207, 434, 436, 427, 74, 41, 42, 271, 304, 272, 41, 74, 73, 304, 271, 303, 48, 102, 49, 331, 278, 279, 102, 48, 64, 278, 331, 294, 240, 64, 235, 294, 460, 455, 64, 240, 98, 460, 294, 327, 36, 129, 203, 358, 266, 423, 129, 36, 142, 266, 358, 371, 73, 38, 41, 268, 303, 271, 38, 73, 72, 303, 268, 302, 123, 227, 116, 447, 352, 345, 227, 123, 137, 352, 447, 366, 77, 62, 96, 292, 307, 325, 62, 77, 76, 307, 292, 306, 106, 202, 43, 422, 335, 273, 202, 106, 204, 335, 422, 424, 46, 113, 124, 342, 276, 353, 113, 46, 225, 276, 342, 445, 211, 202, 204, 422, 431, 424, 202, 211, 210, 431, 422, 430, 29, 159, 160, 386, 259, 387, 159, 29, 27, 259, 386, 257, 124, 226, 35, 446, 353, 265, 226, 124, 113, 353, 446, 342, 158, 56, 157, 286, 385, 384, 56, 158, 28, 385, 286, 258, 99, 20, 242, 250, 328, 462, 20, 99, 60, 328, 250, 290, 43, 212, 57, 432, 273, 287, 212, 43, 202, 273, 432, 422, 36, 100, 142, 329, 266, 371, 100, 36, 101, 266, 329, 330, 200, 208, 201, 428, 200, 421, 208, 200, 199, 200, 428, 199, 39, 167, 37, 393, 269, 267, 167, 39, 165, 269, 393, 391, 22, 154, 26, 381, 252, 256, 154, 22, 153, 252, 381, 380, 207, 206, 216, 426, 427, 436, 206, 207, 205, 427, 426, 425, 236, 51, 134, 281, 456, 363, 51, 236, 3, 456, 281, 248, 44, 19, 125, 19, 274, 354, 19, 44, 1, 274, 19, 1, 117, 116, 111, 345, 346, 340, 116, 117, 123, 346, 345, 352, 116, 34, 143, 264, 345, 372, 34, 116, 227, 345, 264, 447, 63, 46, 70, 276, 293, 300, 46, 63, 53, 293, 276, 283, 95, 62, 78, 292, 324, 308, 62, 95, 96, 324, 292, 325, 70, 124, 156, 353, 300, 383, 124, 70, 46, 300, 353, 276, 242, 238, 241, 458, 462, 461, 238, 242, 20, 462, 458, 250, 105, 53, 63, 283, 334, 293, 53, 105, 52, 334, 283, 282, 113, 130, 226, 359, 342, 446, 130, 113, 247, 342, 359, 467, 120, 230, 231, 450, 349, 451, 230, 120, 119, 349, 450, 348, 91, 43, 146, 273, 321, 375, 43, 91, 106, 321, 273, 335, 109, 69, 67, 299, 338, 297, 69, 109, 108, 338, 299, 337, 170, 32, 140, 262, 395, 369, 32, 170, 211, 395, 262, 431, 134, 45, 220, 275, 363, 440, 45, 134, 51, 363, 275, 281, 52, 224, 53, 444, 282, 283, 224, 52, 223, 282, 444, 443, 217, 47, 114, 277, 437, 343, 47, 217, 126, 437, 277, 355, 150, 135, 169, 364, 379, 394, 135, 150, 136, 379, 364, 365, 198, 49, 209, 279, 420, 429, 49, 198, 131, 420, 279, 360, 26, 155, 112, 382, 256, 341, 155, 26, 154, 256, 382, 381, 218, 166, 219, 392, 438, 439, 166, 218, 79, 438, 392, 309, 100, 126, 142, 355, 329, 371, 126, 100, 47, 329, 355, 277, 97, 240, 99, 460, 326, 328, 240, 97, 98, 326, 460, 327, 221, 193, 189, 417, 441, 413, 193, 221, 55, 441, 417, 285, 117, 31, 228, 261, 346, 448, 31, 117, 111, 346, 261, 340, 193, 8, 168, 8, 417, 168, 8, 193, 55, 417, 8, 285, 103, 68, 54, 298, 332, 284, 68, 103, 104, 332, 298, 333, 196, 236, 174, 456, 419, 399, 236, 196, 3, 419, 456, 248, 247, 161, 246, 388, 467, 466, 161, 247, 30, 467, 388, 260, 159, 28, 158, 258, 386, 385, 28, 159, 27, 386, 258, 257, 189, 245, 244, 465, 413, 464, 245, 189, 193, 413, 465, 417, 92, 39, 40, 269, 322, 270, 39, 92, 165, 322, 269, 391, 176, 171, 148, 396, 400, 377, 171, 176, 140, 400, 396, 369, 206, 36, 203, 266, 426, 423, 36, 206, 205, 426, 266, 425, 16, 84, 85, 314, 16, 315, 84, 16, 17, 16, 314, 17, 180, 91, 90, 321, 404, 320, 91, 180, 181, 404, 321, 405, 182, 91, 181, 321, 406, 405, 91, 182, 106, 406, 321, 335, 200, 83, 18, 313, 200, 18, 83, 200, 201, 200, 313, 421, 187, 205, 207, 425, 411, 427, 205, 187, 50, 411, 425, 280, 77, 91, 146, 321, 307, 375, 91, 77, 90, 307, 321, 320, 23, 144, 145, 373, 253, 374, 144, 23, 24, 253, 373, 254, 5, 45, 51, 275, 5, 281, 45, 5, 4, 5, 275, 4, 54, 71, 21, 301, 284, 251, 71, 54, 68, 284, 301, 298, 98, 203, 129, 423, 327, 358, 203, 98, 165, 327, 423, 391, 135, 192, 214, 416, 364, 434, 192, 135, 138, 364, 416, 367, 182, 204, 106, 424, 406, 335, 204, 182, 194, 406, 424, 418, 157, 190, 173, 414, 384, 398, 190, 157, 56, 384, 414, 286, 30, 160, 161, 387, 260, 388, 160, 30, 29, 260, 387, 259, 76, 146, 61, 375, 306, 291, 146, 76, 77, 306, 375, 307, 18, 84, 17, 314, 18, 17, 84, 18, 83, 18, 314, 313, 210, 212, 202, 432, 430, 422, 212, 210, 214, 430, 432, 434, 119, 229, 230, 449, 348, 450, 229, 119, 118, 348, 449, 347, 186, 40, 185, 270, 410, 409, 40, 186, 92, 410, 270, 322, 183, 80, 191, 310, 407, 415, 80, 183, 42, 407, 310, 272, 40, 184, 185, 408, 270, 409, 184, 40, 74, 270, 408, 304, 194, 211, 204, 431, 418, 424, 211, 194, 32, 418, 431, 262, 131, 48, 49, 278, 360, 279, 48, 131, 115, 360, 278, 344, 151, 107, 108, 336, 151, 337, 107, 151, 9, 151, 336, 9, 40, 73, 74, 303, 270, 304, 73, 40, 39, 270, 303, 269, 101, 118, 119, 347, 330, 348, 118, 101, 50, 330, 347, 280, 155, 157, 173, 384, 382, 398, 157, 155, 154, 382, 384, 381, 171, 152, 148, 152, 396, 377, 152, 171, 175, 396, 152, 175, 67, 104, 103, 333, 297, 332, 104, 67, 69, 297, 333, 299, 128, 232, 233, 452, 357, 453, 232, 128, 121, 357, 452, 350, 121, 114, 47, 343, 350, 277, 114, 121, 128, 350, 343, 357, 39, 72, 73, 302, 269, 303, 72, 39, 37, 269, 302, 267, 121, 231, 232, 451, 350, 452, 231, 121, 120, 350, 451, 349, 72, 0, 11, 0, 302, 11, 0, 72, 37, 302, 0, 267, 34, 162, 139, 389, 264, 368, 162, 34, 127, 264, 389, 356, 191, 95, 78, 324, 415, 308, 249, 466, 263, 362, 398, 382, 7, 33, 246, 133, 155, 173 };
        private readonly List<double> u = new List<double> { 0.4999769926, 0.5000259876, 0.4999740124, 0.4821130037, 0.5001509786, 0.499909997, 0.4995230138, 0.2897120118, 0.4999549985, 0.4999870062, 0.5000230074, 0.5000230074, 0.500015974, 0.5000230074, 0.4999769926, 0.4999769926, 0.4999769926, 0.4999769926, 0.4999679923, 0.4998160005, 0.4737730026, 0.1049069986, 0.365929991, 0.3387579918, 0.3111200035, 0.2746579945, 0.3933619857, 0.3452340066, 0.3700940013, 0.31932199, 0.2979030013, 0.2477920055, 0.3968890011, 0.2800979912, 0.1063100025, 0.2099249959, 0.3558079898, 0.4717510045, 0.4741550088, 0.4397850037, 0.414617002, 0.4503740072, 0.4287709892, 0.3749710023, 0.4867169857, 0.485300988, 0.2577649951, 0.4012230039, 0.4298189878, 0.421351999, 0.2768959999, 0.4833700061, 0.3372119963, 0.2963919938, 0.169294998, 0.4475800097, 0.3923900127, 0.3544900119, 0.0673049986, 0.4427390099, 0.4570980072, 0.3819740117, 0.3923889995, 0.2770760059, 0.4225519896, 0.3859190047, 0.383103013, 0.3314310014, 0.2299239933, 0.3645009995, 0.2296220064, 0.1732870042, 0.4728789926, 0.4468280077, 0.4227620065, 0.4453079998, 0.3881030083, 0.4030390084, 0.403629005, 0.4600419998, 0.4311580062, 0.4521819949, 0.475387007, 0.4658280015, 0.4723289907, 0.4730870128, 0.4731220007, 0.473033011, 0.4279420078, 0.4264790118, 0.4231620133, 0.4183090031, 0.3900949955, 0.0139539996, 0.4999139905, 0.413199991, 0.4096260071, 0.468080014, 0.4227289855, 0.463079989, 0.372119993, 0.3345620036, 0.4116710126, 0.2421759963, 0.2907769978, 0.3273380101, 0.3995099962, 0.4417279959, 0.429764986, 0.4121980071, 0.288955003, 0.2189369947, 0.4127820134, 0.2571350038, 0.4276849926, 0.4483399987, 0.1785600036, 0.2473080009, 0.2862670124, 0.3328279853, 0.3687559962, 0.3989639878, 0.4764100015, 0.189241007, 0.2289620042, 0.4907259941, 0.4046700001, 0.0194690004, 0.4262430072, 0.3969930112, 0.2664699852, 0.4391210079, 0.0323139988, 0.4190540016, 0.4627830088, 0.2389789969, 0.198220998, 0.1075500026, 0.1836100072, 0.1344099939, 0.3857640028, 0.4909670055, 0.3823849857, 0.1743990034, 0.3187850118, 0.3433640003, 0.3961000144, 0.1878850013, 0.4309870005, 0.3189930022, 0.2662479877, 0.5000230074, 0.4999769926, 0.3661699891, 0.3932070136, 0.4103730023, 0.1949930042, 0.3886649907, 0.3659619987, 0.3433640003, 0.3187850118, 0.3014149964, 0.0581329986, 0.3014149964, 0.4999879897, 0.4158380032, 0.4456819892, 0.4658440053, 0.4999229908, 0.2887189984, 0.3352789879, 0.4405120015, 0.128294006, 0.4087719917, 0.455606997, 0.4998770058, 0.3754369915, 0.1142100021, 0.4486620128, 0.4480200112, 0.447111994, 0.4448319972, 0.4300119877, 0.406787008, 0.4007380009, 0.3923999965, 0.3678559959, 0.2479230016, 0.4527699947, 0.4363920093, 0.4161640108, 0.4133859873, 0.2280180007, 0.468268007, 0.4113619924, 0.4999890029, 0.4791539907, 0.4999740124, 0.4321120083, 0.4998860061, 0.499913007, 0.4565489888, 0.3445490003, 0.3789089918, 0.3742929995, 0.3196879923, 0.3571549952, 0.295284003, 0.4477500021, 0.4109860063, 0.3139509857, 0.3541280031, 0.3245480061, 0.1890960038, 0.2797769904, 0.1338230073, 0.3367680013, 0.4298839867, 0.4555279911, 0.4371140003, 0.4672879875, 0.4147120118, 0.377045989, 0.3441079855, 0.3128759861, 0.2835260034, 0.2412459999, 0.1029860005, 0.2676120102, 0.2978790104, 0.3334339857, 0.3664270043, 0.3960120082, 0.4201210141, 0.0075610001, 0.4329490066, 0.4586389959, 0.4734660089, 0.4760879874, 0.4684720039, 0.4339909852, 0.4835180044, 0.4824829996, 0.4264500141, 0.4389989972, 0.4500670135, 0.2897120118, 0.2766700089, 0.5178620219, 0.7102879882, 0.5262269974, 0.8950930238, 0.6340699792, 0.6612420082, 0.6888800263, 0.7253419757, 0.6066300273, 0.6547660232, 0.6299059987, 0.68067801, 0.7020969987, 0.752211988, 0.6029180288, 0.719901979, 0.8936929703, 0.7900819778, 0.6439980268, 0.5282490253, 0.525849998, 0.5602149963, 0.5853840113, 0.5496259928, 0.5712280273, 0.6248520017, 0.5130500197, 0.5150970221, 0.7422469854, 0.5986310244, 0.5703380108, 0.5786319971, 0.7230870128, 0.5164459944, 0.6628010273, 0.7036240101, 0.830704987, 0.5523859859, 0.6076099873, 0.6454290152, 0.9326949716, 0.5572609901, 0.5429019928, 0.6180260181, 0.6075909734, 0.7229430079, 0.5774139762, 0.6140829921, 0.6169070005, 0.6685090065, 0.7700920105, 0.635536015, 0.7703909874, 0.8267220259, 0.5271210074, 0.5531719923, 0.5772380233, 0.5546919703, 0.6118969917, 0.5969610214, 0.596370995, 0.5399580002, 0.5688419938, 0.5478180051, 0.5246130228, 0.5340899825, 0.5276709795, 0.5269129872, 0.5268779993, 0.526966989, 0.572058022, 0.573521018, 0.5768380165, 0.5816910267, 0.6099449992, 0.9860460162, 0.5867999792, 0.590372026, 0.531915009, 0.5772680044, 0.5369150043, 0.6275429726, 0.6655859947, 0.5883539915, 0.7578240037, 0.7092499733, 0.6726840138, 0.6004089713, 0.5582659841, 0.5703039765, 0.5881659985, 0.7110450268, 0.781069994, 0.587247014, 0.7428699732, 0.5721560121, 0.5518680215, 0.821442008, 0.7527019978, 0.7137569785, 0.6671130061, 0.6311010122, 0.6008620262, 0.5234810114, 0.8107479811, 0.7710459828, 0.5091270208, 0.5952929854, 0.9805309772, 0.5734999776, 0.6029949784, 0.733529985, 0.5606110096, 0.9676859975, 0.5809850097, 0.5377280116, 0.7609660029, 0.8017789721, 0.8924409747, 0.8163509965, 0.8655949831, 0.6140739918, 0.5089529753, 0.6179419756, 0.8256080151, 0.6812149882, 0.6566359997, 0.6039000154, 0.8120859861, 0.5680130124, 0.6810079813, 0.7337520123, 0.6338300109, 0.6067929864, 0.5896599889, 0.8050159812, 0.6113349795, 0.6340379715, 0.6566359997, 0.6812149882, 0.6985849738, 0.9418669939, 0.6985849738, 0.5841770172, 0.5543180108, 0.5341539979, 0.7112179995, 0.6646299958, 0.5590999722, 0.8717060089, 0.5912340283, 0.5443410277, 0.6245629787, 0.8857700229, 0.551338017, 0.5519800186, 0.5528879762, 0.555167973, 0.5699440241, 0.5932030082, 0.5992619991, 0.6075999737, 0.6319379807, 0.7520329952, 0.5472260118, 0.5635439754, 0.5838410258, 0.5866140127, 0.7719150186, 0.5315970182, 0.5883709788, 0.5207970142, 0.5679849982, 0.5432829857, 0.6553170085, 0.6210089922, 0.6255599856, 0.6801980138, 0.6427639723, 0.7046629786, 0.5520120263, 0.5890719891, 0.6859449744, 0.6457350254, 0.675342977, 0.8108580112, 0.7201219797, 0.8661519885, 0.663187027, 0.5700820088, 0.5445619822, 0.5627589822, 0.5319870114, 0.5852710009, 0.6229529977, 0.655896008, 0.6871320009, 0.7164819837, 0.7587569952, 0.8970130086, 0.7323920131, 0.702113986, 0.6665250063, 0.6335049868, 0.6038759947, 0.5796579719, 0.9924399853, 0.567192018, 0.5413659811, 0.526564002, 0.5239130259, 0.5315290093, 0.5660359859, 0.5163109899, 0.5174720287, 0.5735949874, 0.5606979728, 0.5497559905, 0.7102879882, 0.7233300209 };
        private readonly List<double> v = new List<double>
        {0.652534008, 0.5474870205, 0.6023719907, 0.471979022, 0.5271559954, 0.4982529879, 0.4010620117, 0.3807640076, 0.3123980165, 0.2699189782, 0.1070500016, 0.6662340164, 0.6792240143, 0.6923480034, 0.6952779889, 0.7059339881, 0.7193850279, 0.7370190024, 0.7813709974, 0.5629810095, 0.5739099979, 0.2541409731, 0.4095759988, 0.4130250216, 0.4094600081, 0.3891310096, 0.4037060142, 0.3440110087, 0.3460760117, 0.3472650051, 0.3535910249, 0.4108099937, 0.8427550197, 0.3755999804, 0.3999559879, 0.3913530111, 0.5344060063, 0.6504039764, 0.6801919937, 0.6572290063, 0.6665409803, 0.6808609962, 0.6826909781, 0.7278050184, 0.5476289988, 0.52739501, 0.3144900203, 0.4551720023, 0.5486149788, 0.5337409973, 0.5320569873, 0.4995869994, 0.2828829885, 0.293242991, 0.1938139796, 0.3026099801, 0.3538879752, 0.6967840195, 0.7301050425, 0.5728260279, 0.5847920179, 0.6947109699, 0.6942030191, 0.2719320059, 0.5632330179, 0.2813640237, 0.2558400035, 0.1197140217, 0.2320029736, 0.1891139746, 0.2995409966, 0.2787479758, 0.6661980152, 0.6685270071, 0.6738899946, 0.5800659657, 0.6939610243, 0.7065399885, 0.6939530373, 0.557139039, 0.692366004, 0.692366004, 0.692366004, 0.7791900039, 0.7362259626, 0.7178570032, 0.7046259642, 0.6952779889, 0.6952779889, 0.7035399675, 0.711845994, 0.7200629711, 0.639572978, 0.5600340366, 0.580147028, 0.6953999996, 0.7018229961, 0.6015349627, 0.5859850049, 0.5937839746, 0.4734140038, 0.4960730076, 0.546965003, 0.1476759911, 0.2014459968, 0.2565270066, 0.7489210367, 0.2616760135, 0.1878340244, 0.1089010239, 0.3989520073, 0.4354109764, 0.3989700079, 0.3554400206, 0.4379609823, 0.5369360447, 0.4575539827, 0.4571939707, 0.4676749706, 0.4607120156, 0.447206974, 0.4326549768, 0.405806005, 0.5239239931, 0.3489509821, 0.5624009967, 0.4851329923, 0.401564002, 0.4204310179, 0.5487970114, 0.3769770265, 0.5189579725, 0.644356966, 0.3871549964, 0.5057469606, 0.7797449827, 0.8319380283, 0.5407550335, 0.7402570248, 0.3336830139, 0.883153975, 0.5793780088, 0.5085729957, 0.3976709843, 0.3962349892, 0.4005969763, 0.7102169991, 0.588537991, 0.9440649748, 0.8982850313, 0.8697010279, 0.1905760169, 0.9544529915, 0.3988220096, 0.3955370188, 0.3910800219, 0.3421019912, 0.3622840047, 0.3559709787, 0.3553569913, 0.3583400249, 0.3631560206, 0.3190760016, 0.3874490261, 0.6184340119, 0.6241959929, 0.5660769939, 0.6206409931, 0.3515239954, 0.819945991, 0.8528199792, 0.9024189711, 0.7919409871, 0.3738939762, 0.451801002, 0.9089900255, 0.9241920114, 0.6150220037, 0.6952779889, 0.7046320438, 0.7158080339, 0.7307940125, 0.7668089867, 0.6856729984, 0.6810690165, 0.677703023, 0.663918972, 0.6013330221, 0.4208499789, 0.3598870039, 0.368713975, 0.692366004, 0.6835719943, 0.3526710272, 0.8043270111, 0.4698250294, 0.4426540136, 0.4396370053, 0.493588984, 0.8669170141, 0.8217290044, 0.8192009926, 0.745438993, 0.5740100145, 0.7801849842, 0.570737958, 0.6042699814, 0.6215809584, 0.8624770045, 0.5087230206, 0.775308013, 0.8125529885, 0.7039929628, 0.6462999582, 0.7146580219, 0.6827009916, 0.6447330117, 0.4665219784, 0.5486229658, 0.5588960052, 0.5299249887, 0.3352199793, 0.3227779865, 0.3201509714, 0.3223320246, 0.3331900239, 0.3827859759, 0.4687629938, 0.4245600104, 0.433175981, 0.4338780046, 0.4261159897, 0.416696012, 0.410228014, 0.4807770252, 0.5695179701, 0.4790890217, 0.5457440019, 0.563830018, 0.5550569892, 0.5823619962, 0.5629839897, 0.5778490305, 0.3897989988, 0.3964949846, 0.4004340172, 0.3682529926, 0.3633729815, 0.4719480276, 0.3807640076, 0.5739099979, 0.2541409731, 0.4095759988, 0.4130250216, 0.4094600081, 0.3891310096, 0.4037050009, 0.3440110087, 0.3460760117, 0.3472650051, 0.3535910249, 0.410804987, 0.8428629637, 0.3755999804, 0.3999599814, 0.3913540244, 0.5344879627, 0.6504039764, 0.68019104, 0.6572290063, 0.6665409803, 0.6808609962, 0.6826919913, 0.7280989885, 0.5472819805, 0.5272519588, 0.3145070076, 0.4549790025, 0.5485750437, 0.5336229801, 0.5320540071, 0.4996389747, 0.2829179764, 0.2932710052, 0.1938139796, 0.3025680184, 0.3538879752, 0.6967070103, 0.7301050425, 0.5728260279, 0.5847920179, 0.6947109699, 0.6942030191, 0.2719630003, 0.563166976, 0.2813869715, 0.2558860183, 0.1199139953, 0.2320209742, 0.1892489791, 0.2995560169, 0.2787550092, 0.6661980152, 0.6685270071, 0.6738899946, 0.5800659657, 0.6939610243, 0.7065399885, 0.6939530373, 0.557139039, 0.692366004, 0.692366004, 0.692366004, 0.7791410089, 0.7362259626, 0.7178570032, 0.7046259642, 0.6952779889, 0.6952779889, 0.7035399675, 0.711845994, 0.7200629711, 0.6399099827, 0.5600340366, 0.6953999996, 0.7018229961, 0.6015369892, 0.5859349966, 0.5937860012, 0.473352015, 0.4959509969, 0.5468620062, 0.1476759911, 0.2015079856, 0.2565810084, 0.7490049601, 0.26167202, 0.1878709793, 0.1090440154, 0.3989520073, 0.4354050159, 0.3989319801, 0.355445981, 0.4376519918, 0.5365700126, 0.4575560093, 0.4571819901, 0.4676269889, 0.4606729746, 0.4471539855, 0.4324730039, 0.4056270123, 0.5239260197, 0.3489590287, 0.5627180338, 0.4850239754, 0.401564002, 0.4200000167, 0.5486879945, 0.3769770265, 0.5190169811, 0.644356966, 0.3871600032, 0.5053850412, 0.7797529697, 0.8319380283, 0.540760994, 0.740260005, 0.3336870074, 0.8832460046, 0.5794379711, 0.50831604, 0.3976749778, 0.3962349892, 0.4005969763, 0.7102169991, 0.5885390043, 0.9445649981, 0.8982850313, 0.8697010279, 0.3988220096, 0.3955370188, 0.3910620213, 0.3421080112, 0.3622840047, 0.3559709787, 0.3553569913, 0.3583400249, 0.3631560206, 0.3190760016, 0.3874490261, 0.6241070032, 0.5660769939, 0.6206400394, 0.8199750185, 0.8528710008, 0.9026319981, 0.7919409871, 0.3738939762, 0.4515839815, 0.9241920114, 0.6150289774, 0.6952779889, 0.7046320438, 0.7158080339, 0.7307940125, 0.7670350075, 0.6856759787, 0.6810690165, 0.677703023, 0.663500011, 0.6013150215, 0.4203950167, 0.3598279953, 0.368713975, 0.692366004, 0.6835780144, 0.3524829745, 0.8044409752, 0.4425650239, 0.4934790134, 0.8192549944, 0.7455149889, 0.5740180016, 0.7803120017, 0.5707190037, 0.6043379903, 0.6215299964, 0.8625919819, 0.5086370111, 0.775357008, 0.8126400113, 0.7039780021, 0.646304965, 0.7146669626, 0.6827049851, 0.6445969939, 0.4663259983, 0.5483759642, 0.5587849617, 0.5301400423, 0.3351770043, 0.3227789998, 0.3201630116, 0.3223459721, 0.3332009912, 0.3827869892, 0.4687690139, 0.4245470166, 0.4331629872, 0.433866024, 0.4260879755, 0.4165869951, 0.4099450111, 0.4807770252, 0.56941998, 0.4788990021, 0.546118021, 0.563830018, 0.5550569892, 0.5823290348, 0.5630539656, 0.5778770447, 0.3898069859, 0.3953319788, 0.3997510076, 0.3682529926, 0.3633729815
        };
    }
}
