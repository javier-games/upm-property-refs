using System.Xml.Linq;
using UnityEditor;

namespace JG.UPM.PropertyRefs.Editor
{
	internal class CSharpProjectPostprocessor : AssetPostprocessor
	{
		/// <summary>
		/// Patch for the Monogum .csproj files to include the registry as additional file.
		/// This fixes the IDEs errors.
		/// </summary>
		public static string OnGeneratedCSProject(string path, string content)
		{
			var registryPath = RegistryUtils.GetSettingsFilePath();
			if (string.IsNullOrEmpty(registryPath)) { return content; }
	
			var xDoc = XDocument.Parse(content);
			var nsMsbuild = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
			var project = xDoc.Element(nsMsbuild + "Project");
	
			if (project == null) return content;
	
			var itemGroup = new XElement(nsMsbuild + "ItemGroup");
			project.Add(itemGroup);
	
			var include = new XAttribute("Include", registryPath);
			var item = new XElement(nsMsbuild + "AdditionalFiles", include);
			itemGroup.Add(item);
	
			content = xDoc.ToString();
			return content;
		}
	}
}