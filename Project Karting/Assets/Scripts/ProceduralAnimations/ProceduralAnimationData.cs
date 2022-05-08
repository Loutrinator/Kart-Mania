using System;
using DG.Tweening;
using UnityEngine;

namespace ProceduralAnimations {
    [Serializable]
    public class AnimationPose {
        public Vector3 position;
        public Quaternion rotation;
    }
    
    [Serializable]
    public class AnimationTransition {
        public string toPose;
        public float duration = 1;

        public Ease positionEasing;
        public Ease rotationEasing;
    }
    
    [CreateAssetMenu(fileName = "Animation Data")]
    public class ProceduralAnimationData : ScriptableObject {
        public SerializableDictionary<string, AnimationPose> poses;
        public SerializableDictionary<string, AnimationTransition> transitions;

        public void PlayTransition(Transform objectTransform, string transitionKey, Action onComplete = null) {
            var animationTransition = transitions[transitionKey];
            var destPose = poses[animationTransition.toPose];
            
            var tween = objectTransform.DOMove(destPose.position, animationTransition.duration)
                .SetEase(animationTransition.positionEasing);
            if(onComplete != null)
                tween.OnComplete(onComplete.Invoke);
            
            objectTransform.DORotateQuaternion(destPose.rotation, animationTransition.duration)
                .SetEase(animationTransition.rotationEasing);
        }
    }
}