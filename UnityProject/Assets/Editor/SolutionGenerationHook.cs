///--------------------------------------------------------------------------------------------------------------
/// <file>    SolutionGenerationHook.cs                           </file>
/// <author>  Blake Martin                                       </author>
/// <date>    Last Edited: 12/03/2022                            </date>
///--------------------------------------------------------------------------------------------------------------
/// <summary>
///     This editor scripting is based on the following github repository:
///     https://github.com/sailro/UnityExternal/blob/master/UnityProject/Assets/Editor/SolutionGenerationHook.cs
/// </summary>
///--------------------------------------------------------------------------------------------------------------
/// <remarks>
///     This implementation is based on the original C code written by Rosenstein
///     and published in the book "An Introduction to Chaotic Dynamical Systems" by Strogatz.
///     This implementation is based on the C code found in the Tisean package:
///     https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html
/// </remarks>
///--------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;

/// <summary>
/// This class is used to hook into the solution generation process and modify the generated project file.
/// </summary>
[InitializeOnLoad]
public class SolutionGenerationHook
{
    /// <summary>
    /// This is the constructor for the SolutionGeneration class.
    /// </summary>
    static SolutionGenerationHook()
    {
        // Register the callback for the emailClientLib project when the solution is loaded.
        ProjectFilesGenerator.SolutionFileGeneration += (name, content) =>
        {
            // Email Client Lib Project Info:
            const string EmailClientLibAssemblyName = "EmailClientLib";
            const string EmailClientLibProjectFilePath = @"..\Subsystems\EmailClientLib\EmailClientLib.csproj";
            const string EmailClientLibProjectGuid = "{F200702D-6716-411F-8A11-4D75A7B2E1E9}";
            const string EmailClientLibCSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

            // Add the EmailClientLib project to the solution
            content = AddProjectToSolution(content, EmailClientLibAssemblyName, EmailClientLibProjectFilePath, EmailClientLibProjectGuid, EmailClientLibCSharpProjectTypeGuid);

            // Return the modified solution file content
            return content;
        };

        // Register the callback for the L1D2 project when the solution is loaded.
        ProjectFilesGenerator.SolutionFileGeneration += (name, content) =>
        {
            // L1D2 Project Info:
            const string L1D2AssemblyName = "L1D2";
            const string L1D2ProjectFilePath = @"..\Subsystems\L1D2\L1D2.csproj";
            const string L1D2ProjectGuid = "{5A236C0A-F657-4285-9922-D18294769868}";
            const string L1D2CSharpProjectTypeGuid = "{FAE14EC1-311F-11D3-BF4B-11C14F79EFBC}";

            // Add the L1D2 project to the solution
            content = AddProjectToSolution(content, L1D2AssemblyName, L1D2ProjectFilePath, L1D2ProjectGuid, L1D2CSharpProjectTypeGuid);

            return content;
        };
    }

    /// <summary>
    /// This method is used to add a project to the solution.
    /// </summary>
    /// <param name="solutionContent">The solution content.</param>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="projectFilePath">The path to the project file.</param>
    /// <param name="projectGuid">The project GUID.</param>
    /// <param name="projectTypeGuid">The project type GUID.</param>
    /// <returns>The solution content with the project added.</returns>
    /// </summary>
    private static string AddProjectToSolution(string content, string projectName, string projectFilePath, string projectGuid, string csharpProjectTypeGuid)
    {
        if (content.Contains("\"" + projectName + "\""))
            return content; // already added

        var signature = new StringBuilder();

        // Add the project to the solution
        signature.AppendLine(string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", csharpProjectTypeGuid, projectName, projectFilePath, projectGuid));
        signature.AppendLine("Global");

        var regex = new Regex("^Global", RegexOptions.Multiline);
        return regex.Replace(content, signature.ToString());
    }
}
