﻿using System.Linq;
using DG.Tweening;
using Handlers;
using Kart;
using SplineEditor.Runtime;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Items {
    public class Rocket : ItemObject {
        [SerializeField] private Transform rocketObject;
        [SerializeField] private Transform rotationPivot;
        
        private BezierPath _bezierPath;
        private float _bezierPos;
        private bool _active;

        private KartBase _target;

        public override void ResetItem() {
            rocketObject.localPosition = Vector3.zero;
            rocketObject.localRotation = Quaternion.identity;
            _active = false;

            _bezierPath = RaceManager.Instance.currentRace.road;
        }

        public override void OnKeyHold(PlayerRaceInfo info) {
            
        }

        public override void OnKeyDown(PlayerRaceInfo info) {
            _bezierPos = _bezierPath.bezierSpline.GetClosestBezierPos(rocketObject.position).BezierDistance;
            rocketObject.parent = null;
            _active = true;

            float roadLength = _bezierPath.bezierSpline.bezierLength;
            _target = RaceManager.Instance.playersInfo.Where(i => i.kart != info.kart)
                .Aggregate((curMin, x) =>
                    x.getDistanceTraveled(roadLength) < curMin.getDistanceTraveled(roadLength) ? x : curMin)
                .kart;
        }

        public override void OnKeyUp(PlayerRaceInfo info) {
            
        }

        private void Update() {
            if (!_active) return;
            var distToTarget = Mathf.Abs(_bezierPos - _target.closestBezierPos.BezierDistance);
            if (distToTarget < 20) {
                _active = false;
                rocketObject.DOMove(_target.transform.position, 0.25f).OnUpdate(() => {
                    rocketObject.LookAt(_target.transform.position);
                }).SetEase(Ease.Linear);
                return;
            }
            _bezierPos += Time.deltaTime * 150f;
            var bPos = _bezierPath.bezierSpline.GetBezierPos(_bezierPos, true);
            var randomCoef = (Mathf.Sin(Time.time) + Mathf.Sin(Time.time * 2.6f) * 0.5f) / 1.5f;
            rocketObject.position = bPos.GlobalOrigin + bPos.LocalUp * 2 + 
                                    bPos.Normal * _bezierPath.bezierMeshExtrusion.roadWidth / 2f * randomCoef;
            rocketObject.forward = bPos.Tangent;
            rotationPivot.localRotation = Quaternion.Euler(0, 0, randomCoef * 180f);
        }
    }
}