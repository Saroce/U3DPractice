using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace UIPractice
{
    /// <summary>
    /// 自定义圆 
    /// </summary>
    public class CircleImage : Image
    {
        [SerializeField]
        private int chunkCount = 30;
        [SerializeField]
        private float percent = 1;

        public float TestFloat = 666f;
    
        private readonly Color GRAY_COLOR = new Color(0.2353f, 0.2353f, 0.2353f, 1);
        private List<Vector3> vertexList = new List<Vector3>();

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            vertexList.Clear();
        
            AddVertex(vh);
            AddTriangle(vh);
        }

        private void AddVertex(VertexHelper vh) {
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            // 正常颜色显示三角块的数量
            var normalColorChunkCount = (int)(chunkCount * percent);

            var uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            var uvWidth = uv.z - uv.x;
            var uvHeight = uv.w - uv.y;
            var uvCenter = new Vector2(uvWidth * 0.5f, uvHeight * 0.5f);
            var convertRatio = new Vector2(uvWidth / width, uvHeight / height);

            var radian = (float)(2 * Math.PI) / chunkCount;
            var radius = (width > height ? height : width) * 0.5f;

            var pivot = rectTransform.pivot;
            // 算出pivot偏移后的原点位置
            var originPos = new Vector2((0.5f - pivot.x) * width, (0.5f - pivot.y) * height);
            var vertPos = Vector2.zero;
            // 添加原点
            var colorTemp = GetOriginColor();
            var origin = GetUIVertex(colorTemp, originPos, vertPos, uvCenter, convertRatio);
            vh.AddVert(origin);

            var normalColorVertexCount = normalColorChunkCount + 1;
            var curRadian = 0f;
            var posTemp = Vector2.zero;
            for (var i = 0; i < chunkCount + 1; i++) {
                // 算出相对于原点的x，y值
                var x = Mathf.Cos(curRadian) * radius;
                var y = Mathf.Sin(curRadian) * radius;
            
                curRadian += radian;
                if (i < normalColorVertexCount)
                {
                    colorTemp = color;
                }
                else
                {
                    colorTemp = GRAY_COLOR;
                }
                posTemp = new Vector2(x, y);
                var vertex = GetUIVertex(colorTemp, posTemp + originPos, posTemp, uvCenter, convertRatio);
                vh.AddVert(vertex);
                vertexList.Add(vertex.position);
            }
        }

        private void AddTriangle(VertexHelper vh) {
            var id = 1;
            for (var i = 0; i < chunkCount; i++) {
                vh.AddTriangle(id, 0, id + 1);
                ++id;
            }
        }
    
        private Color GetOriginColor() {
            var colorTemp = (Color.white - GRAY_COLOR) * percent;
            return new Color((GRAY_COLOR.r + colorTemp.r), (GRAY_COLOR.g + colorTemp.g), (GRAY_COLOR.b + colorTemp.b), 1);
        }

        private UIVertex GetUIVertex(Color color32, Vector3 pos, Vector2 uvPos, Vector2 uvCenter, Vector2 uvScale) {
            var vertex = new UIVertex {
                color = color32,
                position = pos,
                uv0 = new Vector2(uvPos.x * uvScale.x + uvCenter.x, uvPos.y * uvScale.y + uvCenter.y)
            };
            return vertex;
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera,
                out var localPoint);
            // 射线法检测点是否在多边形内
            return (GetCrossNums(localPoint) & 1) == 1;
        }

        private int GetCrossNums(Vector2 localPoint) {
            var vertCount = vertexList.Count;

            var crossNums = 0;
            for (var i = 0; i < vertCount; i++) {
                var vertex1 = vertexList[i];
                var vertex2 = vertexList[(i + 1) % vertCount];

                if (localPoint.y < Math.Min(vertex1.y, vertex2.y) || localPoint.y > Math.Max(vertex1.y, vertex2.y)) {
                    continue;
                }
            
                // 求出localPoint的y值在线端上的x
                var x = (localPoint.y - vertex1.y) * (vertex1.x - vertex2.x) / (vertex1.y - vertex2.y) + vertex1.x;
                if (x > localPoint.x) {
                    crossNums++;
                }
            }
            return crossNums;
        }
    }
}
