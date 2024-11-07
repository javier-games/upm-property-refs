using System;

namespace JG.UPM.PropertyRefs
{
	internal class HardwiredProvider : IRegistryProvider
	{
		public bool ContainsComponent(UnityEngine.Object component)
		{
#if PROPERTY_REFS_SOURCE_GENERATOR_ENABLED
			return Hardwire.HardwiredRegistry.ContainsComponent(component);
#else
			return false;
#endif
		}

		public bool ContainsProperty(UnityEngine.Object component, string propertyName)
		{
#if PROPERTY_REFS_SOURCE_GENERATOR_ENABLED
			return Hardwire.HardwiredRegistry.ContainsProperty(component, propertyName);
#else
			return false;
#endif
		}
		public Type GetPropertyType(UnityEngine.Object component, string propertyName)
		{
#if PROPERTY_REFS_SOURCE_GENERATOR_ENABLED
			return Hardwire.HardwiredRegistry.GetPropertyType(component, propertyName);
#else
			return null;
#endif
		}

		public object GetValue(UnityEngine.Object component, string propertyName)
		{
#if PROPERTY_REFS_SOURCE_GENERATOR_ENABLED
			return Hardwire.HardwiredRegistry.GetValue(component, propertyName);
#else
			return null;
#endif
		}

		public void SetValue(UnityEngine.Object component, string propertyName, object value)
		{
#if PROPERTY_REFS_SOURCE_GENERATOR_ENABLED
			Hardwire.HardwiredRegistry.SetValue(component, propertyName, value);
#endif
		}
	}
}