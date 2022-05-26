using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
    namespace Items.Editor {
        [CustomEditor(typeof(ItemManager))]
        public class ItemManagerEditor : UnityEditor.Editor
        {
            private int currentPosition;
            List<string> positionString = new List<string>();
            private static Color[] itemColors;
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                var itemManager = (ItemManager) target;
                if (itemManager == null) return;
                if (itemManager.itemProbabilities == null) itemManager.itemProbabilities = new List<ListProbability>();
                if (itemManager.items == null) itemManager.items = new List<ItemData>();
                if (itemColors == null) itemColors = new  Color[itemManager.items.Count];
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Positions");
                EditorGUILayout.Space();
                itemManager.nbPositions =  Math.Max(1,EditorGUILayout.IntField("Position count",itemManager.nbPositions));
                while (itemManager.nbPositions > positionString.Count)
                {
                    positionString.Add("Position "+ (positionString.Count+1));
                    itemManager.itemProbabilities.Add(new ListProbability());
                }

                while (itemManager.nbPositions < positionString.Count)
                {
                    positionString.RemoveAt(positionString.Count - 1);
                    itemManager.itemProbabilities.RemoveAt(positionString.Count - 1);
                }
            
                DrawItems(itemManager);

                DrawProbability(itemManager);

                if (GUILayout.Button("Set dirty")) {
                    EditorUtility.SetDirty(target);
                }

                serializedObject.ApplyModifiedProperties();
            }
        
            private static void DrawItems(ItemManager itemManager)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Items");
                EditorGUILayout.Space();

                List<ItemData> list = itemManager.items;
                itemManager.nbItems = Math.Max(0, EditorGUILayout.IntField("Size", list.Count));

                while (itemManager.nbItems > list.Count)
                {
                    list.Add(null);
                    updateColors(list.Count);
                }

                while (itemManager.nbItems < list.Count)
                {
                    list.RemoveAt(list.Count - 1);
                    updateColors(list.Count);
                }
                EditorGUI.indentLevel++;
                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    list[i] = EditorGUILayout.ObjectField("Item " + i, list[i], typeof(ItemData),
                        true) as ItemData;
                    //EditorGUI.DrawRect(GUILayoutUtility.GetRect(5,10,20,20), itemColors[i]); affichage de couleur
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            private static void updateColors(int amount)
            {
                itemColors = new Color[amount];
                for (int i = 0; i < amount; i++)
                {
                    float hue = (i + 1f) / (float) amount;
                    itemColors[i] = Color.HSVToRGB(hue, 0.7f, 1f);
                }
            }

            private void DrawProbability(ItemManager itemManager)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Probabilities");
                EditorGUILayout.Space();
                GUIContent arrayList = new GUIContent("Position");
                currentPosition = EditorGUILayout.Popup(arrayList, currentPosition, positionString.ToArray());
            
                List<string> itemNames = new List<string>();
                foreach (var item in itemManager.items)
                {
                    if(item != null) itemNames.Add(item.GetName());
                }
            
            
                //List<ItemProbability> probas = itemManager.itemProba[currentPosition];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
                {
                    itemManager.itemProbabilities[currentPosition].Add(new ItemProbability());
                }
                EditorGUILayout.EndHorizontal();
                float sumProba = 0f;
                string promptProba = "";
                for (int i = 0; i < itemManager.itemProbabilities[currentPosition].Count; i++)
                {
                    bool removed = false;
                    ItemProbability p = itemManager.itemProbabilities[currentPosition][i];
                    EditorGUILayout.BeginHorizontal();
                    GUIContent itemList = new GUIContent("Item");
                    p.itemId = EditorGUILayout.Popup(itemList, p.itemId, itemNames.ToArray());
                    if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                    {
                        itemManager.itemProbabilities[currentPosition].RemoveAt(i);
                        removed = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (!removed)
                    {
                        float value = Math.Min(1-sumProba,EditorGUILayout.Slider("Probability", itemManager.itemProbabilities[currentPosition][i].probability-sumProba, 0f, 1f));
                        sumProba += value; 
                        p.probability = sumProba;
                        promptProba += "| " + i + " : " + p.probability + " ";
                        itemManager.itemProbabilities[currentPosition][i] = p;
                        EditorGUILayout.Separator();
                    }
                
                }
                //Debug.Log(promptProba);
            }

        }
    }
    #endif
    