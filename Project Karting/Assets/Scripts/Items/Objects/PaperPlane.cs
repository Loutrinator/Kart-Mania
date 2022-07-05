using DG.Tweening;
using Handlers;
using SplineEditor.Runtime;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Items {
    public class PaperPlane : ItemObject {
        [SerializeField] private GameObject rendererRoot;
        [SerializeField] private float moveDistance = 300;
        [SerializeField] private float planeSpeed = 10;
        
        private BezierPath _bezierPath;
        private Transform _kartObject;
        private float _startDistance;
        public override void ResetItem() {
            rendererRoot.SetActive(false);

            _bezierPath = RaceManager.Instance.currentRace.road;
        }

        public override void OnKeyHold(PlayerRaceInfo info) {
            
        }

        public override void OnKeyDown(PlayerRaceInfo info) {
            rendererRoot.SetActive(true);
            _kartObject = info.kart.transform;

            _startDistance = info.kart.closestBezierPos.BezierDistance;
            float bezierDistance = _startDistance;
            info.kart.enabled = false;
            info.kart.kartRootModel.gameObject.SetActive(false);
            DOTween.To(() => bezierDistance, value => {
                    bezierDistance = value;
                    var bPos = _bezierPath.bezierSpline.GetBezierPos(bezierDistance, true);
                    _kartObject.position = bPos.GlobalOrigin;
                    _kartObject.rotation = bPos.Rotation;
            }, _startDistance + moveDistance, planeSpeed).OnComplete(() => {
                info.kart.kartRootModel.gameObject.SetActive(true);
                info.kart.enabled = true;
                Destroy(gameObject);
            }).SetSpeedBased().SetEase(Ease.Linear);
        }

        public override void OnKeyUp(PlayerRaceInfo info) {
            
        }
    }
}