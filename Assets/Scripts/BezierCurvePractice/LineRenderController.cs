//------------------------------------------------------------
//        File:  LineRenderController.cs
//       Brief:  LineRenderController
//
//      Author:  Saroce, Saroce233@163.com
//
//    Modified:  2022-04-09
//============================================================

using System;
using UnityEngine;

namespace BezierCurvePractice
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BezierCurve))]
    public class LineRenderController : MonoBehaviour
    {
        [SerializeField] private int pointCount = 20;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private BezierCurve bezierCurve;

        private void Awake() {
            lineRenderer.positionCount = pointCount + 1;
        }

        private void Update() {
            for (var i = 0; i <= pointCount; i++) {
                var pos = bezierCurve.DrawCurvePosition(i / (float) pointCount);
                lineRenderer.SetPosition(i, pos);
            }
        }
    }
}