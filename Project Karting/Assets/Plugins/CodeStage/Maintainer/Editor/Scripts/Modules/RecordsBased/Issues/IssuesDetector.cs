#region copyright
// -------------------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// -------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System.Collections.Generic;
	using Core;
	using Detectors;
	using Settings;
	using System;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;
	using Object = UnityEngine.Object;

	internal static class IssuesDetector
	{
		private static MissingReferenceDetector missingReferenceDetector;
		private static MissingComponentDetector missingComponentDetector;
		internal static DuplicateComponentDetector duplicateComponentDetector;
		private static MissingPrefabDetector missingPrefabDetector;
		private static InconsistentTerrainDataDetector inconsistentTerrainDataDetector;
		private static HugePositionDetector hugePositionDetector;
		private static EmptyLayerNameDetector emptyLayerNameDetector;
		private static DuplicateLayersDetector duplicateLayersDetector;

#if UNITY_2019_1_OR_NEWER
		private static ShaderErrorDetector shaderErrorDetector;
#endif

		private static AssetInfo currentAsset;
		private static RecordLocation currentLocation;
		private static GameObject currentGameObject;

		public static void Init(List<IssueRecord> issues)
		{
			missingReferenceDetector = new MissingReferenceDetector(issues);

			missingComponentDetector = new MissingComponentDetector(issues);
			duplicateComponentDetector = new DuplicateComponentDetector(issues);
			inconsistentTerrainDataDetector = new InconsistentTerrainDataDetector(issues);
			missingPrefabDetector = new MissingPrefabDetector(issues);

			hugePositionDetector = new HugePositionDetector(issues);
			emptyLayerNameDetector = new EmptyLayerNameDetector(issues);

			duplicateLayersDetector = new DuplicateLayersDetector(issues);
#if UNITY_2019_1_OR_NEWER
			shaderErrorDetector = new ShaderErrorDetector(issues);
#endif
		}

		#region Scene

		public static void SceneStart(AssetInfo asset)
		{
			currentLocation = RecordLocation.Scene;
			currentAsset = asset;

			missingReferenceDetector.TryDetectIssuesInSceneSettings(currentAsset);
		}

		public static void SceneEnd(AssetInfo asset)
		{
			currentLocation = RecordLocation.Unknown;
			currentAsset = null;
			currentGameObject = null;
		}
		
		#endregion

		#region Prefab Asset

		public static void StartPrefabAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Prefab;
			currentAsset = asset;
		}

		public static void EndPrefabAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Unknown;
			currentAsset = null;
			currentGameObject = null;
		}
		
		#endregion

		#region Game Object (both from Scenes and Prefab Assets)
		
		public static bool StartGameObject(GameObject target, bool inPrefabInstance, out bool skipTree)
		{
			skipTree = false;

			if (!ProjectSettings.Issues.touchInactiveGameObjects)
			{
				if (currentLocation == RecordLocation.Scene)
				{
					if (!target.activeInHierarchy) return false;
				}
				else
				{
					if (!target.activeSelf) return false;
				}
			}

			if (inPrefabInstance)
			{
				if (missingPrefabDetector.TryDetectIssue(currentLocation, currentAsset.Path, target))
				{
					skipTree = true;
					return false;
				}
			}

			currentGameObject = target;
			missingComponentDetector.StartGameObject();
			duplicateComponentDetector.StartGameObject();

			hugePositionDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
			emptyLayerNameDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);

			return true;
		}

		public static void EndGameObject(GameObject target)
		{
			missingComponentDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
			inconsistentTerrainDataDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
		}

		#endregion
		
		#region Game Object's Component
		
		public static void ProcessComponent(Component component, int orderIndex)
		{
#if !UNITY_2019_1_OR_NEWER
			if (missingComponentDetector.CheckAndRecordNullComponent(component))
			{
				return;
			}
#else
			if (component == null) return;
#endif

			if ((component.hideFlags & HideFlags.HideInInspector) != 0)
			{
				return;
			}

			if (!ProjectSettings.Issues.touchDisabledComponents)
			{
				if (EditorUtility.GetObjectEnabled(component) == 0) return;
			}

			var componentType = component.GetType();
			var componentName = componentType.Name;

			if (CSFilterTools.HasEnabledFilters(ProjectSettings.Issues.componentIgnoresFilters))
			{
				if (CSFilterTools.IsValueMatchesAnyFilterOfKind(componentName, ProjectSettings.Issues.componentIgnoresFilters, FilterKind.Type)) return;
			}

			duplicateComponentDetector.ProcessComponent(component, componentType);

			var shouldCheckPropertiesForDuplicate = duplicateComponentDetector.IsPropertiesHashCalculationRequired();
			if (shouldCheckPropertiesForDuplicate)
			{
				// skipping duplicate search for non-standard components with invisible properties
				var baseType = componentType.BaseType;
				if (baseType != null)
				{
					if (baseType.Name == "MegaModifier")
					{
						shouldCheckPropertiesForDuplicate = false;
						duplicateComponentDetector.SkipComponent();
					}
				}
			}

			var shouldTraverseProperties = missingReferenceDetector.Enabled || shouldCheckPropertiesForDuplicate;
			if (shouldTraverseProperties)
			{
				var initialInfo = new SerializedObjectTraverseInfo(component);

				CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
				{
					if (CSReflectionTools.IsPropertyIsSubclassOf<UnityEventBase>(property))
					{
						missingReferenceDetector.TryDetectUnityEventIssues(currentLocation, currentAsset.Path,
							currentGameObject, componentType, componentName, orderIndex, property);

						info.skipCurrentTree = true;
						return;
					}

					missingReferenceDetector.TryDetectIssue(currentLocation, currentAsset.Path, currentGameObject, componentType, componentName, orderIndex, property);

					if (shouldCheckPropertiesForDuplicate) duplicateComponentDetector.ProcessProperty(property);
				});
			}

			if (shouldCheckPropertiesForDuplicate)
			{
				duplicateComponentDetector.TryDetectIssue(currentLocation, currentAsset.Path, currentGameObject, componentType, componentName, orderIndex);
			}

			if (component is Terrain)
			{
				inconsistentTerrainDataDetector.ProcessTerrainComponent((Terrain)component, componentType, componentName, orderIndex);
			}
			else if (component is TerrainCollider)
			{
				inconsistentTerrainDataDetector.ProcessTerrainColliderComponent((TerrainCollider)component);
			}
			//Debug.Log("ProcessComponent: " + target.name + ":" + component);
		}

		#endregion
		
		#region Mono Script
		
		public static void ProcessMonoScript(AssetInfo asset, MonoScript target)
		{
			currentLocation = RecordLocation.Asset;
			
			var shouldTraverseProperties = missingReferenceDetector.Enabled;
			if (shouldTraverseProperties)
			{
				var componentTraverseInfo = new SerializedObjectTraverseInfo(target, false);
				string lastScriptPropertyName;

				CSTraverseTools.TraverseObjectProperties(componentTraverseInfo, (info, sp) =>
				{
					lastScriptPropertyName = CSTraverseTools.TryGetMonoScriptDefaultPropertyName(sp);
					
					string propertyName;

					if (lastScriptPropertyName != null)
					{
						propertyName = lastScriptPropertyName;
						lastScriptPropertyName = string.Empty;
					}
					else
					{
						propertyName = sp.propertyPath;
					}
					
					missingReferenceDetector.TryDetectSerializedPropertyIssue(asset.Path,
						"MonoScript", sp, propertyName);
				});
			}

			currentLocation = RecordLocation.Unknown;
		}
		
		#endregion
		
		#region Material

		public static void ProcessMaterial(AssetInfo asset, Material target)
		{
			currentLocation = RecordLocation.Asset;

			var shouldTraverseProperties = missingReferenceDetector.Enabled;
			if (shouldTraverseProperties)
			{
				CSTraverseTools.TraverseMaterialTexEnvs(new SerializedObjectTraverseInfo(target),
					(traverseInfo, texEnv, texEnvName, texEnvTexture) =>
					{
						missingReferenceDetector.TryDetectSerializedPropertyIssue(asset.Path,
							"Material", texEnvTexture, texEnvName);
					});
			}

			currentLocation = RecordLocation.Unknown;
		}

		#endregion

		#region Settings Asset

		public static void ProcessSettingsAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Asset;

			var kind = asset.SettingsKind;

			missingReferenceDetector.TryDetectIssuesInSettingsAsset(asset, kind);

			if (kind == AssetSettingsKind.TagManager)
			{
				duplicateLayersDetector.TryDetectIssue();
			}

			currentLocation = RecordLocation.Unknown;
		}
		
		#endregion
		
		#region Shader asset

#if UNITY_2019_1_OR_NEWER
		public static void ProcessShader(AssetInfo asset, Shader shader)
		{
			shaderErrorDetector.TryDetectIssue(asset, shader);
		}
#endif
		
		#endregion
		
		#region Unity Object Asset (generic)

		public static void ProcessUnityObjectAsset(AssetInfo asset, Object unityObject)
		{
			currentLocation = RecordLocation.Asset;

			if (missingComponentDetector.TryDetectUnityObjectAssetIssue(asset.Path,
				unityObject))
			{
				return;
			}

			var shouldTraverseProperties = missingReferenceDetector.Enabled;
			if (shouldTraverseProperties)
			{
				var initialInfo = new SerializedObjectTraverseInfo(unityObject);
				CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
				{
					if (CSReflectionTools.IsPropertyIsSubclassOf<UnityEventBase>(property))
					{
						missingReferenceDetector.TryDetectUnityObjectUnityEventIssue(asset.Path,
							info.TraverseTarget.GetType().Name, property);

						info.skipCurrentTree = true;
						return;
					}

					missingReferenceDetector.TryDetectSerializedPropertyIssue(asset.Path,
						info.TraverseTarget.GetType().Name, property);
				});
			}

			currentLocation = RecordLocation.Unknown;
		}
		
		#endregion
	}
}
