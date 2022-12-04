///--------------------------------------------------------------------------------------------------------------
/// <file>    ProjectGenerationHook.cs                           </file>
/// <author>  Blake Martin                                       </author>
/// <date>    Last Edited: 12/03/2022                            </date>
///--------------------------------------------------------------------------------------------------------------
/// <summary>
///     This editor scripting is based on the following github repository:
///     https://github.com/sailro/UnityExternal/blob/master/UnityProject/Assets/Editor/ProjectGenerationHook.cs
/// </summary>
///--------------------------------------------------------------------------------------------------------------
/// <remarks>
///     This implementation is based on the original C code written by Rosenstein
///     and published in the book "An Introduction to Chaotic Dynamical Systems" by Strogatz.
///     This implementation is based on the C code found in the Tisean package:
///     https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html
/// </remarks>
///--------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;

/// <summary>
/// This class is used to hook into the project generation process and modify the generated project file.
/// </summary>
[InitializeOnLoad]
public class ProjectGenerationHook
{

    /// <summary>
    /// This is the constructor for the ProjectGeneration class.
    /// </summary>
    static ProjectGenerationHook()
    {

        // Register the callback for when the emailClientLib project is loaded.
        ProjectFilesGenerator.ProjectFileGeneration += (name, projectInfo) =>
        {

            // Check if the project is the emailClientLib project.
            const string EmailClientLibAssemblyName = "EmailClientLib";
            const string EmailClientLibProjectFilePath = @"..\Subsystems\EmailClientLib\EmailClientLib.csproj";
            const string EmailClientLibProjectGuid = "{F200702D-6716-411F-8A11-4D75A7B2E1E9}";


            // Remove any references to EmailClientLib
            projectInfo = RemoveAssemblyReferenceFromProject(projectInfo, EmailClientLibAssemblyName);
            // Add a reference to the EmailClientLib project
            projectInfo = AddProjectReferenceToProject(projectInfo, EmailClientLibAssemblyName, EmailClientLibProjectFilePath, EmailClientLibProjectGuid);
            // Add a post build step to copy the EmailClientLib.dll from the project output to the assets folder
            projectInfo = AddCopyAssemblyToAssetsPostBuildEvent(projectInfo, EmailClientLibAssemblyName);

            Debug.Log("ProjectGenerationHook:" + name);
            return projectInfo;
        };

        // Register the callback for when the L1D2 project is loaded.
        ProjectFilesGenerator.ProjectFileGeneration += (name, projectInfo) =>
        {

            const string L1D2AssemblyName = "L1D2";
            const string L1D2ProjectFilePath = @"..\Subsystems\L1D2\L1D2.csproj";
            const string L1D2ProjectGuid = "{5A236C0A-F657-4285-9922-D18294769868}";

            // Remove any references to L1D2
            projectInfo = RemoveAssemblyReferenceFromProject(projectInfo, L1D2AssemblyName);
            // Add a reference to the L1D2 project
            projectInfo = AddProjectReferenceToProject(projectInfo, L1D2AssemblyName, L1D2ProjectFilePath, L1D2ProjectGuid);
            // Add a post build step to copy the L1D2.dll from the project output to the assets folder
            projectInfo = AddCopyAssemblyToAssetsPostBuildEvent(projectInfo, L1D2AssemblyName);

            Debug.Log("ProjectGenerationHook:" + name);
            Debug.Log("Content: " + projectInfo);
            return projectInfo;
        };
    }



    /// <summary>
    /// This method adds a post build step to the project file to copy the specified assembly to the assets folder.
    /// This is required because the Unity editor does not support loading assemblies from the project output folder.
    /// </summary>
    private static string AddCopyAssemblyToAssetsPostBuildEvent(string content, string assemblyName)
    {
        // First, check if the project file already contains a post build event.
        if (content.Contains("PostBuildEvent"))
            return content; // already added

        // This is the signature that will be added to the end of the project file
        var signature = new StringBuilder();
        // This is the path to the Assets folder within the Unity project
        string AssetsPath = @"..\..\..\..\Assets\";
        // Get the .dll name
        var dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
        // Append the csproj syntax for a post build event
        signature.AppendLine("  <PropertyGroup>");
        signature.AppendLine("    <RunPostBuildEvent>Always</RunPostBuildEvent>");
        signature.AppendLine(string.Format(@"    <PostBuildEvent>copy /Y $(TargetDir){0}.dll {1}</PostBuildEvent>", assemblyName, AssetsPath));
        signature.AppendLine("  </PropertyGroup>");
        signature.AppendLine("</Project>");
        Debug.Log(signature.ToString());
        // This is a regular expression that will find the closing Project tag
        var regex = new Regex("^</Project>", RegexOptions.Multiline);
        // This replaces the closing Project tag with the signature (and the closing Project tag)
        return regex.Replace(content, signature.ToString());
    }

    /// <summary>
    /// Remove the assembly references from the project file.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    private static string RemoveAssemblyReferenceFromProject(string content, string assemblyName)
    {
        var regex = new Regex(string.Format(@"^\s*<Reference Include=""{0}"">\r\n\s*<HintPath>.*{0}.dll</HintPath>\r\n\s*</Reference>\r\n", assemblyName), RegexOptions.Multiline);
        return regex.Replace(content, string.Empty);
    }


    /// <summary>
    /// This is the function that writes the information from the individual csprojs to the generated .sln file.
    /// This allows for linking the VS projects by reference as dependencies, meaning that the project requiring another
    /// dependency project will compile the dependency first.
    /// For more information on project references and dependencies in Visual Studio 2019,
    /// please refer to these links:
    /// https://learn.microsoft.com/en-us/visualstudio/ide/managing-references-in-a-project?view=vs-2022
    /// https://stackoverflow.com/questions/8115974/what-are-visual-studio-project-references
    ///
    private static string AddProjectReferenceToProject(string content, string projectName, string projectFilePath, string projectGuid)
    {
        // Check if the project file already contains a reference to the project
        if (content.Contains(">" + projectName + "<"))
            return content; // already added

        // This is the signature that will be added to the end of the project file
        var signature = new StringBuilder();
        // Append the csproj syntax for a project reference
        signature.AppendLine("  <ItemGroup>");
        signature.AppendLine(string.Format("    <ProjectReference Include=\"{0}\">", projectFilePath));
        signature.AppendLine(string.Format("      <Project>{0}</Project>", projectGuid));
        signature.AppendLine(string.Format("      <Name>{0}</Name>", projectName));
        signature.AppendLine("    </ProjectReference>");
        signature.AppendLine("  </ItemGroup>");
        signature.AppendLine("</Project>");

        // This is a regular expression that will find the closing Project tag
        var regex = new Regex("^</Project>", RegexOptions.Multiline);
        // Return the content with the signature appended
        return regex.Replace(content, signature.ToString());
    }

}