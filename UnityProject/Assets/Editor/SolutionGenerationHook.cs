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
            // Email Client Lib Project Info:
            const string EmailClientLibAssemblyName = "EmailClientLib";
            const string EmailClientLibProjectFilePath = @"..\Subsystems\EmailClientLib\EmailClientLib.csproj";
            const string EmailClientLibProjectGuid = "{F200702D-6716-411F-8A11-4D75A7B2E1E9}";
            const string EmailClientLibCSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

            // Add the subsystem projects to the solution:
            content = AddProjectToSolution(content, EmailClientLibAssemblyName, EmailClientLibProjectFilePath, EmailClientLibProjectGuid, EmailClientLibCSharpProjectTypeGuid);

            return content;
        };

        ProjectFilesGenerator.SolutionFileGeneration += (name, content) =>
        {
            // L1D2 Project Info:
            const string L1D2AssemblyName = "L1D2";
            const string L1D2ProjectFilePath = @"..\Subsystems\L1D2\L1D2.csproj";
            const string L1D2ProjectGuid = "{5A236C0A-F657-4285-9922-D18294769868}";
            const string L1D2CSharpProjectTypeGuid = "{FAE14EC1-311F-11D3-BF4B-11C14F79EFBC}";

            // Add the subsystem projects to the solution:
            content = AddProjectToSolution(content, L1D2AssemblyName, L1D2ProjectFilePath, L1D2ProjectGuid, L1D2CSharpProjectTypeGuid);

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
