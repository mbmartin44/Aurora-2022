using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

using UnityEngine;

public class BuildPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            // Read.
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string targetGUID = project.GetUnityMainTargetGuid();

            AddFrameworks(project, targetGUID);
            EditInfoPlist(path);

            // Write.
            File.WriteAllText(projectPath, project.WriteToString());
            Debug.Log("Post process completed");
        }
#endif
    }

#if UNITY_IOS
    static void AddFrameworks(PBXProject project, string targetGUID)
    {
        project.AddFrameworkToProject(targetGUID, "CoreBluetooth.framework", true);
    }

    static void EditInfoPlist(string pathToBuiltProject)
    {
        var infoPlist = new PlistDocument();
        var infoPlistPath = pathToBuiltProject + "/Info.plist";
        infoPlist.ReadFromFile(infoPlistPath);

        PlistElementDict dict = infoPlist.root.AsDict();
        dict.SetString("NSBluetoothAlwaysUsageDescription", "App requires access to Bluetooth to allow you connect to device");
        dict.SetString("NSBluetoothPeripheralUsageDescription", "Biofeedback application uses Bluetooth to connect with your Callibri device");

        infoPlist.WriteToFile(infoPlistPath);
    }
#endif

}
