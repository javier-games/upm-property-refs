namespace JG.UPM.PropertyRefs
{
	public interface IRegistryProvider
	{
		bool ContainsComponent(UnityEngine.Object component);
		bool ContainsProperty(UnityEngine.Object component, string propertyName);
		System.Type GetPropertyType(UnityEngine.Object component, string propertyName);
		object GetValue(UnityEngine.Object component, string propertyName);
		void SetValue(UnityEngine.Object component, string propertyName, object value);
	}
}