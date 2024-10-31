using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JG.UPM.PropertyRefs.SourceGenerator
{
	internal static class JsonUtils
	{
		internal static RegistryData Deserialize(string jsonString) 
        {
			try
			{
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
				{
					var serializer = new DataContractJsonSerializer(typeof(RegistryData));
					return (RegistryData)serializer.ReadObject(stream);
				}
			}
			catch (System.Exception)
			{
				return null;
			}
		} 
		
		internal static string Serialize(RegistryData data)
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					var serializer = new DataContractJsonSerializer(typeof(RegistryData));
					serializer.WriteObject(stream, data);
					return Encoding.UTF8.GetString(stream.ToArray());
				}
			}
			catch (System.Exception)
			{
				return string.Empty;
			}
		}
    }
}
        