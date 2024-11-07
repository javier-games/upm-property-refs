using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Text;

namespace JG.UPM.PropertyRefs.Editor
{
	internal static class RegistryUtils
	{
		private const string DefaultSettingsPath = "Assets/Editor/" + FileNameWithExtension;
		private const string FileExtension = ".additionalfile";
		private const string FileName = "Registry.PropertyRefsSourceGenerator";
		internal const string FileNameWithExtension = FileName + FileExtension;
		internal const string CodeGenerationEnableDirective = "PROPERTY_REFS_SOURCE_GENERATOR_ENABLED";

		internal static string GetSettingsFilePath()
		{
			var guids = AssetDatabase.FindAssets(FileName);
			var registryPaths = new List<string>();
			foreach (var guid in guids)
			{
				var filePath = AssetDatabase.GUIDToAssetPath(guid);
				if (filePath.Contains(FileNameWithExtension))
				{
					registryPaths.Add(filePath);
				}
			}

			if (registryPaths.Count > 1)
			{
				Debug.LogWarning("There is more than just one registry for properties, please remove one.");
				Debug.Log($"Using {registryPaths[0]}");
			}
			
			return registryPaths.Count > 0 ? registryPaths[0] : null;
		}
		
		internal static void RegisterComponent(System.Type type)
		{
			var assetPath = GetSettingsFilePath();
			if (string.IsNullOrEmpty(assetPath) || type == null) { return; }

			var registry = GetRegistry(assetPath);

			var typeName = type.FullName?.Replace("+", ".");
			bool Match(RegisteredComponent registeredComponent) => 
				registeredComponent.type == typeName;

			if (registry.components.Exists(Match))
			{
				Debug.Log($"Component {typeName} already registered.");
				return;
			}
			
			var component = new RegisteredComponent
			{
				type = typeName, 
				properties = new List<RegisteredProperty>()
			};
			
			registry.components.Add(component);
			SetRegistry(assetPath, registry);
		}

		internal static void RegisterProperty(System.Type type, PropertyInfo propertyInfo)
		{
			var assetPath = GetSettingsFilePath();
			if (string.IsNullOrEmpty(assetPath) || type == null || propertyInfo == null)
			{
				return;
			}

			var registry = GetRegistry(assetPath);
			var typeName = type.FullName?.Replace("+", ".");

			bool Match(RegisteredComponent registeredComponent) => 
				registeredComponent.type == typeName;

			if (!registry.components.Exists(Match))
			{
				Debug.Log($"Component {typeName} not found.");
				return;
			}

			var componentIndex = registry.components.FindIndex(Match);
			registry.components[componentIndex].properties.Add(
				new RegisteredProperty { 
					name = propertyInfo.Name, 
					type = propertyInfo.PropertyType.FullName?.Replace("+", ".") 
				}
			);

			SetRegistry(assetPath, registry);
		}

		private static Registry GetRegistry(string assetPath)
		{
			if (string.IsNullOrEmpty(assetPath))
				return new Registry(){components = new List<RegisteredComponent>()};
				
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(File.ReadAllText(assetPath));
			var registry = JsonUtility.FromJson<Registry>(stringBuilder.ToString());
			return registry ?? new Registry(){components = new List<RegisteredComponent>()};
		}

		private static void SetRegistry(string assetPath, Registry registry)
		{
			var json = JsonUtility.ToJson(registry ?? new Registry(), prettyPrint: true);
			File.WriteAllText(string.IsNullOrEmpty(assetPath)? DefaultSettingsPath: assetPath, json);
			AssetDatabase.Refresh();
		}
	}
}