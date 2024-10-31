using System;
using System.Linq;

namespace JG.UPM.PropertyRefs
{
	internal class ReflectionProvider : IRegistryProvider
	{
		public bool ContainsComponent(UnityEngine.Object component)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return false;
#else
			return true;
#endif
		}

		public bool ContainsProperty(UnityEngine.Object component, string propertyName)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return false;
#else
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				return false;
			}
			
			var supportedProperties = ReflectionUtils.GetAvailableProperties(component);
			return supportedProperties.Any(
				property => property.Name == propertyName
					&& ReflectionUtils.IsSupportedProperty(property)
			);
#endif
		}
		
		public Type GetPropertyType(UnityEngine.Object component, string propertyName)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return null;
#else
			if (!ContainsProperty(component, propertyName))
			{
				return null;
			}
			
			var property = component.GetType().GetProperty(propertyName);
			return property == null ? null : property.PropertyType;
#endif
		}
		
		public object GetValue(UnityEngine.Object component, string propertyName)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return false;
#else
			if (!ContainsProperty(component, propertyName))
			{
				return null;
			}
			
			var property = component.GetType().GetProperty(propertyName);
			return property == null ? null : property.GetValue(component);
#endif
		}

		
		public void SetValue(UnityEngine.Object component, string propertyName, object value)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return;
#else
			if (!ContainsProperty(component, propertyName)) { return; }
			
			var property = component.GetType().GetProperty(propertyName);
			if (property == null) { return; }

			try { property.SetValue(component, value); }
			catch (Exception)
			{
				//
			}
#endif
		}
	}
}