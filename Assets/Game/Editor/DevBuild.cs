using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// Development player build for playtests (Windows x64). Development builds include the
    /// debug panel, tuning and cheats; release builds compile them out.
    /// Menu: ColonizeEarth/Build/Windows Development Build.
    /// Batch: -executeMethod Game.EditorTools.DevBuild.BuildWindowsDevelopment
    /// </summary>
    public static class DevBuild
    {
        public const string OutputPath = "Builds/WindowsDev/ColonizeEarth.exe";

        [MenuItem("ColonizeEarth/Build/Windows Development Build")]
        public static void BuildWindowsDevelopment()
        {
            ProjectSetup.CreateBootScene();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ProjectSetup.BootScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,
            };
            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
                Debug.Log($"DevBuild: succeeded, {summary.totalSize / (1024 * 1024)} MB at {OutputPath}");
            else
                Debug.LogError($"DevBuild: {summary.result} with {summary.totalErrors} error(s).");

            if (Application.isBatchMode) EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
