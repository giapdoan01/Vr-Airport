using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class HDROptimizer : EditorWindow
{
    private int hdrMaxSize = 1024;
    
    [MenuItem("Tools/HDR Skybox Optimizer")]
    static void ShowWindow()
    {
        GetWindow<HDROptimizer>("HDR Optimizer");
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("HDR Skybox Optimizer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "HDR files are usually very large (4K = ~14MB).\n" +
            "This tool will resize and compress them.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        hdrMaxSize = EditorGUILayout.IntPopup("Max HDR Size", hdrMaxSize, 
            new string[] { "512", "1024", "2048" },
            new int[] { 512, 1024, 2048 });
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Analyze HDR Files", GUILayout.Height(30)))
        {
            AnalyzeHDRFiles();
        }
        
        if (GUILayout.Button("Optimize HDR Files", GUILayout.Height(30)))
        {
            OptimizeHDRFiles();
        }
    }
    
    void AnalyzeHDRFiles()
    {
        string[] hdrFiles = AssetDatabase.FindAssets("t:Texture")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.EndsWith(".hdr") || path.EndsWith(".exr"))
            .ToArray();
        
        Debug.Log("=== HDR FILES ANALYSIS ===");
        Debug.Log($"Found {hdrFiles.Length} HDR/EXR files\n");
        
        long totalSize = 0;
        
        foreach (string path in hdrFiles)
        {
            FileInfo fileInfo = new FileInfo(path);
            long sizeInBytes = fileInfo.Length;
            float sizeInMB = sizeInBytes / (1024f * 1024f);
            totalSize += sizeInBytes;
            
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            string resolution = texture != null ? $"{texture.width}x{texture.height}" : "Unknown";
            
            Debug.Log($"{path}");
            Debug.Log($"  Size: {sizeInMB:F2} MB");
            Debug.Log($"  Resolution: {resolution}\n");
        }
        
        float totalMB = totalSize / (1024f * 1024f);
        Debug.Log($"<b>TOTAL HDR SIZE: {totalMB:F2} MB</b>");
        Debug.Log("=========================\n");
        
        EditorUtility.DisplayDialog("Analysis Complete", 
            $"Found {hdrFiles.Length} HDR files\nTotal size: {totalMB:F2} MB", 
            "OK");
    }
    
    void OptimizeHDRFiles()
    {
        string[] hdrFiles = AssetDatabase.FindAssets("t:Texture")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.EndsWith(".hdr") || path.EndsWith(".exr"))
            .ToArray();
        
        int optimized = 0;
        
        foreach (string path in hdrFiles)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                // Shape: Cubemap for skybox
                importer.textureShape = TextureImporterShape.TextureCube;
                
                // Size
                importer.maxTextureSize = hdrMaxSize;
                importer.isReadable = false;
                importer.mipmapEnabled = true;
                
                // Default settings
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                defaultSettings.maxTextureSize = hdrMaxSize;
                defaultSettings.format = TextureImporterFormat.BC6H; // HDR compression
                defaultSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(defaultSettings);
                
                // Android
                TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                androidSettings.overridden = true;
                androidSettings.name = "Android";
                androidSettings.maxTextureSize = hdrMaxSize;
                androidSettings.format = TextureImporterFormat.ASTC_HDR_6x6;
                importer.SetPlatformTextureSettings(androidSettings);
                
                // iOS
                TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
                iosSettings.overridden = true;
                iosSettings.name = "iPhone";
                iosSettings.maxTextureSize = hdrMaxSize;
                iosSettings.format = TextureImporterFormat.ASTC_HDR_6x6;
                importer.SetPlatformTextureSettings(iosSettings);
                
                importer.SaveAndReimport();
                
                Debug.Log($"✓ Optimized HDR: {path}");
                optimized++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n<b>Optimized {optimized} HDR files</b>\n");
        
        EditorUtility.DisplayDialog("Optimization Complete", 
            $"Optimized {optimized} HDR files", 
            "OK");
    }
}
