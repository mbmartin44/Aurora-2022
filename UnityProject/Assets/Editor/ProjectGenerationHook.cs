using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;

[InitializeOnLoad]
public class ProjectGenerationHook
{

	static ProjectGenerationHook()
	{
		ProjectFilesGenerator.ProjectFileGeneration += (name, content) =>
		{
			// Email Client Lib Project Info:
			const string EmailClientLibAssemblyName = "EmailClientLib";
			const string EmailClientLibProjectFilePath = @"..\Subsystems\EmailClientLib\EmailClientLib.csproj";
			const string EmailClientLibProjectGuid = "{F200702D-6716-411F-8A11-4D75A7B2E1E9}";

			content = RemoveAssemblyReferenceFromProject(content, EmailClientLibAssemblyName);
			content = AddProjectReferenceToProject(content, EmailClientLibAssemblyName, EmailClientLibProjectFilePath, EmailClientLibProjectGuid);
			content = AddCopyAssemblyToAssetsPostBuildEvent(content, EmailClientLibAssemblyName);

			Debug.Log("ProjectGenerationHook:" + name);
			Debug.Log("Content: " + content);
			return content;
		};

		ProjectFilesGenerator.ProjectFileGeneration += (name, content) =>
		{
			// L1D2 Project Info:
			const string L1D2AssemblyName = "L1D2";
			const string L1D2ProjectFilePath = @"..\Subsystems\L1D2\L1D2.csproj";
			const string L1D2ProjectGuid = "{5A236C0A-F657-4285-9922-D18294769868}";

			content = RemoveAssemblyReferenceFromProject(content, L1D2AssemblyName);
			content = AddProjectReferenceToProject(content, L1D2AssemblyName, L1D2ProjectFilePath, L1D2ProjectGuid);
			content = AddCopyAssemblyToAssetsPostBuildEvent(content, L1D2AssemblyName);

			Debug.Log("ProjectGenerationHook:" + name);
			Debug.Log("Content: " + content);
			return content;
		};
	}

	private static string AddCopyAssemblyToAssetsPostBuildEvent(string content, string assemblyName)
	{
		if (content.Contains("PostBuildEvent"))
			return content; // already added

		var signature = new StringBuilder();
		string AssetsPath = @"..\..\..\..\Assets\";


		var dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);

		signature.AppendLine("  <PropertyGroup>");
		signature.AppendLine("    <RunPostBuildEvent>Always</RunPostBuildEvent>");
		signature.AppendLine(string.Format(@"    <PostBuildEvent>copy /Y $(TargetDir){0}.dll {1}</PostBuildEvent>", assemblyName, AssetsPath));
		signature.AppendLine("  </PropertyGroup>");
		signature.AppendLine("</Project>");
		Debug.Log(signature.ToString());

		var regex = new Regex("^</Project>", RegexOptions.Multiline);
		return regex.Replace(content, signature.ToString());
	}

	private static string RemoveAssemblyReferenceFromProject(string content, string assemblyName)
	{
		var regex = new Regex(string.Format(@"^\s*<Reference Include=""{0}"">\r\n\s*<HintPath>.*{0}.dll</HintPath>\r\n\s*</Reference>\r\n", assemblyName), RegexOptions.Multiline);
		return regex.Replace(content, string.Empty);
	}

	private static string AddProjectReferenceToProject(string content, string projectName, string projectFilePath, string projectGuid)
	{
		if (content.Contains(">" + projectName + "<"))
			return content; // already added

		var signature = new StringBuilder();
		signature.AppendLine("  <ItemGroup>");
		signature.AppendLine(string.Format("    <ProjectReference Include=\"{0}\">", projectFilePath));
		signature.AppendLine(string.Format("      <Project>{0}</Project>", projectGuid));
		signature.AppendLine(string.Format("      <Name>{0}</Name>", projectName));
		signature.AppendLine("    </ProjectReference>");
		signature.AppendLine("  </ItemGroup>");
		signature.AppendLine("</Project>");

		var regex = new Regex("^</Project>", RegexOptions.Multiline);
		return regex.Replace(content, signature.ToString());
	}

}