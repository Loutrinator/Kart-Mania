using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Items
{
    internal struct ItemProbability
    {
        public float probability;
        public int itemId;
    }
    public class ItemManager : MonoBehaviour
    {
        [HideInInspector] public int nbItems;
        [HideInInspector] public int nbPositions;
        [HideInInspector] public List<Item> items;
        [HideInInspector] public List<List<float>> probabilities;
        
        [HideInInspector] internal List<List<ItemProbability>> itemProba;

        //private static ItemManager _instance;

        //public static ItemManager Instance => _instance;

        public ItemManager()
        {}

        /*private void Awake()
        {
            if (_instance == null){

                _instance = this;
                DontDestroyOnLoad(this.gameObject);

                //Rest of your Awake code

            } else {
                Destroy(this);
            }
        }*/

        private void Start()
        {
            GenerateProbabilities();
        }

        private void GenerateProbabilities()
        {
            itemProba = new List<List<ItemProbability>>();
            nbPositions = GameManager.Instance.nbPlayerRacing;
            //nbItems = items.Count;
            for (int i = 0; i < nbPositions; i++)
            {
                float sumProba = 0f;
                itemProba.Append(new List<ItemProbability>());
                for (int j = 0; j < nbItems; j++)
                {
                    float proba = probabilities[i][j]; //[i][j]
                    if (proba > 0)
                    {
                        sumProba += probabilities[i][j]; //[i][j]

                        if (sumProba > 1f)
                        {
                            Debug.LogError("SUM OF PROBABILITIES AT POSITION " + i + " IS OVER 1f");
                        }
                        else
                        {
                            ItemProbability currentItemProba = new ItemProbability();
                            currentItemProba.probability = sumProba;

                            itemProba[i].Append(currentItemProba);
                        }
                    }
                }

                //TODO: TRIER LES LISTES D'ITEMPROBABILITY PAR PROBABILITE
            }
        }


        [CanBeNull]
        public Item GetRandomItem(int position)
        {
            float rnd = Random.value;
            foreach (var proba in itemProba[position])
            {
                if (rnd <= proba.probability)
                {
                    return items[proba.itemId];
                }
            }
            return null;
        }
    }
    
    #region Editor
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ItemManager))]
    public class ItemManagerEditor : Editor
    {
        private int currentPosition = 0;
        List<string> positionString = new List<string>();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var itemManager = (ItemManager) target;
            if (itemManager == null) return;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Positions");
            EditorGUILayout.Space();
            itemManager.nbPositions =  Math.Max(1,EditorGUILayout.IntField("Position count",itemManager.nbPositions));
            while(itemManager.nbPositions > positionString.Count) positionString.Add("Position "+ (positionString.Count+1));
            while(itemManager.nbPositions < positionString.Count) positionString.RemoveAt(positionString.Count - 1);
            
            DrawItems(itemManager);

            DrawProbability(itemManager);
        }

        private void DrawProbability(ItemManager itemManager)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Probabilities");
            EditorGUILayout.Space();
            GUIContent arrayList = new GUIContent("Position");
            currentPosition = EditorGUILayout.Popup(arrayList, currentPosition, positionString.ToArray());

            if(itemManager.probabilities == null) itemManager.probabilities = new List<List<float>>();
            
            //List<float> list = itemManager.probabilities[currentPosition];
            //int size = Math.Max(0, EditorGUILayout.IntField("Size", list.Count));
            /*
            while (itemManager.nbItems > list.Count) list.Add(null);
            while (itemManager.nbItems < list.Count) list.RemoveAt(list.Count - 1);
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = EditorGUILayout.ObjectField("Item " + i, list[i], typeof(Item),
                    true) as Item; //(list[i], typeof(GameObject), true);
            }

            EditorGUI.indentLevel--;*/
        }

        private static void DrawItems(ItemManager itemManager)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Items");
            EditorGUILayout.Space();

            List<Item> list = itemManager.items;
            itemManager.nbItems = Math.Max(0, EditorGUILayout.IntField("Size", list.Count));

            while (itemManager.nbItems > list.Count) list.Add(null);
            while (itemManager.nbItems < list.Count) list.RemoveAt(list.Count - 1);
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = EditorGUILayout.ObjectField("Item " + i, list[i], typeof(Item),
                    true) as Item; //(list[i], typeof(GameObject), true);
            }

            EditorGUI.indentLevel--;
        }
    }
    #endif
    
    #endregion
}
