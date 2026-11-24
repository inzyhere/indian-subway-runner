using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace IndianSubwayRunner.Editor
{
    public static class AndroidBuild
    {
        private const string ApplicationIdentifier = "com.inzyhere.indiansubwayrunner";
        private const string ApkFileName = "IndianSubwayRunner.apk";
        private const string BuildDirectoryName = "Builds";

        [MenuItem("Indian Subway Runner/Build Android APK")]
        public static void BuildApk()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string buildDirectory = Path.Combine(projectRoot, BuildDirectoryName);
            string apkPath = Path.Combine(buildDirectory, ApkFileName);

            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            if (scenes.Length == 0)
            {
                throw new InvalidOperationException("No enabled scenes in EditorBuildSettings. Run Indian Subway Runner/Generate Game Scenes first.");
            }

            PlayerSettings.companyName = "inzy";
            PlayerSettings.productName = "Indian Subway Runner";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

            Directory.CreateDirectory(buildDirectory);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Android APK build failed: " + report.summary.result);
            }

            Debug.Log("Android APK built at " + apkPath);
        }

        public static void BuildCiApk()
        {
            SceneBootstrapGenerator.GenerateAll();
            BuildApk();
        }
    }
}
