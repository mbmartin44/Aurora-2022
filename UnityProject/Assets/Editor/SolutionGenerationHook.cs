using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;

[InitializeOnLoad]
public class SolutionGenerationHook
{

	static SolutionGenerationHook()
	{
		ProjectFilesGenerator.SolutionFileGeneration += (name, content) =>
		{
			const string assemblyName = "EmailClientLib";
			const string projectFilePath = @"..\Subsystems\EmailClientLib\EmailClientLib.csproj";
			const string EmailClientLibProjectGuid = "{F200702D-6716-411F-8A11-4D75A7B2E1E9}";
			const string EmailClientLibCSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
			content = AddProjectToSolution(content, assemblyName, projectFilePath, EmailClientLibProjectGuid, EmailClientLibCSharpProjectTypeGuid);

			Debug.Log("SolutionGenerationHook:" + name);
			return content;
		};
	}

	private static string AddProjectToSolution(string content, string projectName, string projectFilePath, string projectGuid, string csharpProjectTypeGuid)
	{
		if (content.Contains("\"" + projectName + "\""))
			return content; // already added

		var signature = new StringBuilder();

		signature.AppendLine(string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", csharpProjectTypeGuid, projectName, projectFilePath, projectGuid));
		signature.AppendLine("Global");

		var regex = new Regex("^Global", RegexOptions.Multiline);
		return regex.Replace(content, signature.ToString());
	}
}
