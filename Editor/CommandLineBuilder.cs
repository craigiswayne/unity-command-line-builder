using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// This class provides methods to build a Unity project from the command line.
public class CommandLineBuild
    {
        // This is the main method that will be executed from the command line.
        public static void Build()
        {
            Debug.Log("CommandLineBuild: Build starting...");

            // --- Get Build Configuration from Command-Line Arguments ---

            // Get scenes from command line argument, or fall back to Build Settings.
            var scenes = GetScenesToBuild();

            // If no scenes are found, log an error and exit.
            if (scenes == null || scenes.Length == 0)
            {
                Debug.LogError("CommandLineBuild: No scenes specified or found. Use the -sceneList argument or enable scenes in Build Settings.");
                EditorApplication.Exit(1); // Exit with an error code.
                return;
            }

            // Get the build target and output path from helper methods.
            var buildTarget = GetBuildTarget();
            var outputPath = GetArgument("-outputPath");

            // If the output path is not provided, log an error and exit.
            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogError("CommandLineBuild: No output path specified. Use -outputPath <path>.");
                EditorApplication.Exit(1);
                return;
            }
            
            // --- Apply Custom Settings Based on Command-Line Flags ---

            // Check for the -compressionMode flag and apply settings for WebGL.
            if (buildTarget == BuildTarget.WebGL)
            {
                var compressionMode = GetArgument("-compressionMode");
                if (!string.IsNullOrEmpty(compressionMode))
                {
                    var compressionFormat = GetWebGLCompressionFormat(compressionMode);
                    PlayerSettings.WebGL.compressionFormat = compressionFormat;
                    Debug.Log($"CommandLineBuild: Setting WebGL compression to {compressionFormat}.");
                }
                else
                {
                    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                }
            }

            Debug.Log($"CommandLineBuild: Target: {buildTarget}");
            Debug.Log($"CommandLineBuild: Output Path: {outputPath}");
            Debug.Log("CommandLineBuild: Scenes to build:");
            foreach (var scene in scenes)
            {
                Debug.Log($"- {scene}");
            }

            // --- Execute the Build ---

            // Configure the build options.
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = buildTarget,
                options = BuildOptions.None // Use BuildOptions.Development for development builds
            };

            // Start the build and get the report.
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            // --- Handle Build Result ---

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"CommandLineBuild: Build succeeded! Size: {summary.totalSize / 1024 / 1024} MB. Time: {summary.totalTime.Minutes}m {summary.totalTime.Seconds}s.");
                EditorApplication.Exit(0); // Exit with a success code.
            }
            else
            {
                Debug.LogError($"CommandLineBuild: Build failed with {summary.totalErrors} errors.");
                Debug.LogError($"CommandLineBuild: See full report for details:\n{report}");
                EditorApplication.Exit(1); // Exit with an error code.
            }
        }
        
        private static string[] GetScenesToBuild()
        {
            var sceneListArg = GetArgument("-sceneList");
            if (!string.IsNullOrEmpty(sceneListArg))
            {
                Debug.Log("CommandLineBuild: Found -sceneList argument. Using specified scenes.");
                // Split the comma-separated string into an array of scene paths.
                return sceneListArg.Split(',');
            }
            else
            {
                Debug.Log("CommandLineBuild: No -sceneList argument provided. Using enabled scenes from Build Settings.");
                // Get all scenes listed and enabled in the Build Settings.
                return EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path)
                    .ToArray();
            }
        }
        
        private static string GetArgument(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
        
        private static BuildTarget GetBuildTarget()
        {
            const BuildTarget fallbackBuildTarget = BuildTarget.WebGL;
            
            var buildTargetString = GetArgument("-buildTarget");
            if (string.IsNullOrEmpty(buildTargetString))
            {
                Debug.LogWarning("CommandLineBuild: No build target specified with -buildTarget. Defaulting to StandaloneWindows64.");
                return fallbackBuildTarget;
            }

            // Attempt to parse the build target string.
            if (System.Enum.TryParse(buildTargetString, true, out BuildTarget target))
            {
                return target;
            }

            Debug.LogError($"CommandLineBuild: Invalid build target: {buildTargetString}. Defaulting to WebGL.");
            return fallbackBuildTarget;
        }
        
        // New helper function to parse the compression format for WebGL.
        private static WebGLCompressionFormat GetWebGLCompressionFormat(string compressionMode)
        {
            switch (compressionMode.ToLower())
            {
                case "brotli":
                    return WebGLCompressionFormat.Brotli;
                case "gzip":
                    return WebGLCompressionFormat.Gzip;
                default:
                    Debug.LogWarning($"CommandLineBuild: Invalid compression mode: {compressionMode}. Defaulting to Brotli.");
                    return WebGLCompressionFormat.Disabled;
            }
        }
    }
