using UnityEngine;
using UnityExtendedEditor.ExtendedAttributes.Editor;

// ReSharper disable once CheckNamespace
namespace UnityExtendedEditor.ExtendedAttributes {
    public class MonoBehaviourExample : MonoBehaviour {
        [MinMaxSlider(0, 10)]
        public Vector2 minMaxSlider;
    }
}
