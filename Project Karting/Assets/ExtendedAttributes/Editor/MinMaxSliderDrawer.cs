using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace UnityExtendedEditor.ExtendedAttributes.Editor {
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        private const string kVectorMinName = "x";
        private const string kVectorMaxName = "y";
        //private const float kFloatFieldWidth = 16f;
        private const float kSpacing = 2f;
        private const float kRoundingValue = 100f;

        private static readonly int controlHash = "Foldout".GetHashCode();
        private static readonly GUIContent unsupported = EditorGUIUtility.TrTextContent("Unsupported field type");

        private bool pressed;

        private float Round(float value, float roundingValue)
        {
            return roundingValue == 0 ? value : Mathf.Round(value * roundingValue) / roundingValue;
        }

        private float FlexibleFloatFieldWidth(float min, float max)
        {
            var n = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
            return 35f + (Mathf.Floor(Mathf.Log10(Mathf.Abs(n)) + 1) * 2.5f);
        }

        private void SetVectorValue(SerializedProperty property, ref float min, ref float max, bool round)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    {
                        if (round)
                        {
                            min = Round(min, kRoundingValue);
                            max = Round(max, kRoundingValue);
                        }
                        property.vector2Value = new Vector2(min, max);
                    }
                    break;
                case SerializedPropertyType.Vector2Int:
                    {
                        property.vector2IntValue = new Vector2Int((int)min, (int)max);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float min, max;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    {
                        var v = property.vector2Value;
                        min = v.x;
                        max = v.y;
                    }
                    break;
                case SerializedPropertyType.Vector2Int:
                    {
                        var v = property.vector2IntValue;
                        min = v.x;
                        max = v.y;
                    }
                    break;
                default:
                    EditorGUI.LabelField(position, label, unsupported);
                    return;
            }

            var attr = attribute as MinMaxSliderAttribute;

            float ppp = EditorGUIUtility.pixelsPerPoint;
            float spacing = kSpacing * ppp;
            float fieldWidth = ppp * (/*attr.DataFields && attr.FlexibleFields ?*/
                FlexibleFloatFieldWidth(attr.Min, attr.Max)/* : kFloatFieldWidth*/);

            var indent = EditorGUI.indentLevel;

            int id = GUIUtility.GetControlID(controlHash, FocusType.Keyboard, position);
            var r = EditorGUI.PrefixLabel(position, id, label);

            Rect sliderPos = r;

            if (attr.DataFields)
            {
                sliderPos.x += fieldWidth + spacing;
                sliderPos.width -= (fieldWidth + spacing) * 2;
            }

            if (Event.current.type == EventType.MouseDown &&
                sliderPos.Contains(Event.current.mousePosition))
            {
                pressed = true;
                min = Mathf.Clamp(min, attr.Min, attr.Max);
                max = Mathf.Clamp(max, attr.Min, attr.Max);
                SetVectorValue(property, ref min, ref max, attr.Round);
                GUIUtility.keyboardControl = 0; // TODO keep focus but stop editing
            }

            if (pressed && Event.current.type == EventType.MouseUp)
            {
                if (attr.Round)
                {
                    SetVectorValue(property, ref min, ref max, true);
                }
                pressed = false;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel = 0;
            EditorGUI.MinMaxSlider(sliderPos, ref min, ref max, attr.Min, attr.Max);
            EditorGUI.indentLevel = indent;
            if (EditorGUI.EndChangeCheck())
            {
                SetVectorValue(property, ref min, ref max, false);
            }

            if (attr.DataFields)
            {
                Rect minPos = r;
                minPos.width = fieldWidth;

                var vectorMinProp = property.FindPropertyRelative(kVectorMinName);
                EditorGUI.showMixedValue = vectorMinProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel = 0;
                min = EditorGUI.DelayedFloatField(minPos, min);
                EditorGUI.indentLevel = indent;
                if (EditorGUI.EndChangeCheck())
                {
                    if (attr.Bound)
                    {
                        min = Mathf.Max(min, attr.Min);
                        min = Mathf.Min(min, max);
                    }
                    SetVectorValue(property, ref min, ref max, attr.Round);
                }
                vectorMinProp.Dispose();

                Rect maxPos = position;
                maxPos.x += maxPos.width - fieldWidth;
                maxPos.width = fieldWidth;

                var vectorMaxProp = property.FindPropertyRelative(kVectorMaxName);
                EditorGUI.showMixedValue = vectorMaxProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel = 0;
                max = EditorGUI.DelayedFloatField(maxPos, max);
                EditorGUI.indentLevel = indent;
                if (EditorGUI.EndChangeCheck())
                {
                    if (attr.Bound)
                    {
                        max = Mathf.Min(max, attr.Max);
                        max = Mathf.Max(max, min);
                    }
                    SetVectorValue(property, ref min, ref max, attr.Round);
                }
                vectorMaxProp.Dispose();

                EditorGUI.showMixedValue = false;
            }
        }
    }
}
