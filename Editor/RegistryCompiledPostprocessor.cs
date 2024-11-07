using UnityEditor;

namespace JG.UPM.PropertyRefs.Editor
{
	internal class RegistryCompiledPostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		)
		{
			for (var i = 0; i < importedAssets.Length; i++)
			{
				var asset = importedAssets[i];
				if (asset.Contains(RegistryUtils.FileNameWithExtension))
				{
					var currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
					var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup);
					var symbolList = currentSymbols.Split(';');
					var isAdded = false;

					for (var j = 0; j < symbolList.Length; j++)
					{
						isAdded |= symbolList[j] == RegistryUtils.CodeGenerationEnableDirective;
					}

					if (!isAdded)
					{
						PlayerSettings.SetScriptingDefineSymbolsForGroup(
							currentGroup,
							currentSymbols + ";" + RegistryUtils.CodeGenerationEnableDirective
						);
					}
				}
			}

			for (var i = 0; i < deletedAssets.Length; i++)
			{
				var asset = deletedAssets[i];
				if (asset.Contains(RegistryUtils.FileNameWithExtension))
				{
					var currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
					var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup);
					var symbolList = currentSymbols.Split(';');
					var newSymbols = "";

					for (var j = 0; j < symbolList.Length; j++)
					{
						var symbol = symbolList[j];
						if (symbol == RegistryUtils.CodeGenerationEnableDirective) continue;
						newSymbols += symbol + ";";
					}

					newSymbols = newSymbols.TrimEnd(';');
					PlayerSettings.SetScriptingDefineSymbolsForGroup(currentGroup, newSymbols);
				}
			}
		}
	}
}