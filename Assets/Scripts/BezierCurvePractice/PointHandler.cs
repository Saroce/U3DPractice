//------------------------------------------------------------
//        File:  PointHandler.cs
//       Brief:  PointHandler
//
//      Author:  Saroce, Saroce233@163.com
//
//    Modified:  2022-04-09
//============================================================

using System;
using UnityEngine;

namespace BezierCurvePractice
{
    public class PointHandler : MonoBehaviour
    {
        private Transform target;
        private float posZ;

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100)) {
                    target = hit.transform;
                    posZ = target.position.z - Camera.main.transform.position.z;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                target = null;
            }

            if (target != null && Input.GetMouseButton(0)) {
                var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, posZ));
                target.position = targetPos;
            }
        }
    }
}