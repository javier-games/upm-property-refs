#if !UNITY_IOS || UNITY_EDITOR
using System.Reflection;
#endif

namespace JG.UPM.PropertyRefs
{
	public static class ReflectionUtils
	{
#if !UNITY_IOS || UNITY_EDITOR
		public static PropertyInfo[] GetAvailableProperties(UnityEngine.Object component)
		{
			return component.GetType()
				.GetProperties(
					BindingFlags.Public | BindingFlags.Instance
				);
		}

		/// <summary>
		/// Whether the property info is supported by PropertyRefs.
		/// </summary>
		/// <param name="propertyInfo">Property info to validate.</param>
		/// <returns><value>TRUE</value>> if the property works with PropertyRefs.</returns>
		public static bool IsSupportedProperty(PropertyInfo propertyInfo)
		{
			return propertyInfo.CanRead
				&& propertyInfo.CanWrite
				&& !propertyInfo.IsDefined(typeof(System.ObsoleteAttribute), true)
				&& propertyInfo.Name != "runInEditMode"
				&& propertyInfo.Name != "useGUILayout"
				&& propertyInfo.Name != "tag"
				&& propertyInfo.Name != "name"
				&& propertyInfo.Name != "hideFlags"
				&& propertyInfo.Name != "enabled";
		}
#endif
	}
}