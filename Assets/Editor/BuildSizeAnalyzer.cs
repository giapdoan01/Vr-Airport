using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;

public class BuildSizeAnalyzer : EditorWindow
{
    [MenuItem("Tools/Build Size Analyzer")]
    static void ShowWindow()
    {
        GetWindow<BuildSizeAnalyzer>("Build Analyzer");
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Build Size Analyzer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This will build your project and show detailed size breakdown.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Build & Analyze (Current Platform)", GUILayout.Height(40)))
        {
            BuildAndAnalyze();
        }
    }
    
    void BuildAndAnalyze()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
        
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No scenes in build settings!", "OK");
            return;
        }
        
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = scenes;
        options.target = EditorUserBuildSettings.activeBuildTarget;
        
        // Set build path based on platform
        switch (options.target)
        {
            case BuildTarget.Android:
                options.locationPathName = "Builds/Android/game.apk";
                break;
            case BuildTarget.iOS:
                options.locationPathName = "Builds/iOS";
                break;
            case BuildTarget.StandaloneWindows64:
                options.locationPathName = "Builds/Windows/game.exe";
                break;
            default:
                options.locationPathName = "Builds/game";
                break;
        }
        
        options.options = BuildOptions.None;
        
        Debug.Log("Starting build...\n");
        
        BuildReport report = BuildPipeline.BuildPlayer(options);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("=== BUILD SUCCESS ===\n");
            
            // Summary
            Debug.Log($"<b>Platform:</b> {report.summary.platform}");
            Debug.Log($"<b>Total Size:</b> {report.summary.totalSize / (1024 * 1024)} MB");
            Debug.Log($"<b>Total Time:</b> {report.summary.totalTime}\n");
            
            // Detailed breakdown
            Debug.Log("=== SIZE BREAKDOWN ===\n");
            
            var assetSizes = report.packedAssets
                .SelectMany(pa => pa.contents)
                .GroupBy(asset => System.IO.Path.GetExtension(asset.sourceAssetPath))
                .Select(group => new {
                    Extension = group.Key,
                    TotalSize = group.Sum(a => (long)a.packedSize),
                    Count = group.Count()
                })
                .OrderByDescending(x => x.TotalSize)
                .ToList();
            
            foreach (var item in assetSizes)
            {
                float sizeMB = item.TotalSize / (1024f * 1024f);
                Debug.Log($"{item.Extension}: {sizeMB:F2} MB ({item.Count} files)");
            }
            
            Debug.Log("\n=== TOP 20 LARGEST ASSETS ===\n");
            
            var topAssets = report.packedAssets
                .SelectMany(pa => pa.contents)
                .OrderByDescending(asset => asset.packedSize)
                .Take(20)
                .ToList();
            
            foreach (var asset in topAssets)
            {
                float sizeMB = asset.packedSize / (1024f * 1024f);
                Debug.Log($"{sizeMB:F2} MB - {asset.sourceAssetPath}");
            }
            
            EditorUtility.DisplayDialog("Build Complete!", 
                $"Build size: {report.summary.totalSize / (1024 * 1024)} MB\n\n" +
                "Check Console for detailed breakdown.", 
                "OK");
        }
        else
        {
            Debug.LogError("Build failed!");
            EditorUtility.DisplayDialog("Build Failed", 
                "Build failed! Check Console for errors.", 
                "OK");
        }
    }
}
