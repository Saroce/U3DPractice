//------------------------------------------------------------
//        File:  BezierCurve.cs
//       Brief:  贝塞尔曲线
//
//      Author:  Saroce, Saroce233@163.com
//
//    Modified:  2022-04-09
//============================================================

using UnityEngine;

namespace BezierCurvePractice
{
    
    [ExecuteInEditMode]
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private int accuracy = 20;

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            for (var i = 0; i < points.Length - 1; i++) {
                var current = points[i].position;
                var next = points[i + 1].position;
                Gizmos.DrawLine(current, next);
            }
        }

        private void Update() {
            var prevPos = points[0].position;
            for (var i = 0; i <= accuracy; ++i) {
                var next = DrawCurvePosition(i / (float) accuracy);
                Debug.DrawLine(prevPos, next);
                prevPos = next;
            }
        }

        /// <summary>
        /// 根据比例绘制曲线位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 DrawCurvePosition(float t) {
            switch (points.Length) {
                case 3: return QuardaticBezier(t);
                case 4: return CubicBezier(t);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 一阶贝塞尔
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 LineBezier(float t) {
            var a = points[0].position;
            var b = points[1].position;
            return a + (b - a) * t;
        }

        /// <summary>
        /// 二阶贝塞尔
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 QuardaticBezier(float t) {
            var a = points[0].position;
            var b = points[1].position;
            var c = points[2].position;

            var aa = a + (b - a) * t;
            var bb = b + (c - b) * t;

            return aa + (bb - aa) * t;
        }

        /// <summary>
        /// 三阶贝塞尔
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 CubicBezier(float t) {
            var a = points[0].position;
            var b = points[1].position;
            var c = points[2].position;
            var d = points[3].position;

            var aa = a + (b - a) * t;
            var bb = b + (c - b) * t;
            var cc = c + (d - c) * t;

            var aaa = aa + (bb - aa) * t;
            var bbb = bb + (cc - bb) * t;

            return aaa + (bbb - aaa) * t;
        }
    }
}