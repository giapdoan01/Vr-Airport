using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class GLBOptimizer : EditorWindow
{
    private Vector2 scrollPosition;
    private bool optimizeModels = true;
    private bool extractMaterials = true;
    private bool optimizeTextures = true;
    private int textureMaxSize = 1024;
    
    [MenuItem("Tools/GLB Optimizer")]
    static void ShowWindow()
    {
        GetWindow<GLBOptimizer>("GLB Optimizer");
    }
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.LabelField("GLB Model Optimizer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Options
        optimizeModels = EditorGUILayout.Toggle("Optimize Models", optimizeModels);
        extractMaterials = EditorGUILayout.Toggle("Extract Materials", extractMaterials);
        optimizeTextures = EditorGUILayout.Toggle("Optimize Textures", optimizeTextures);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Texture Settings", EditorStyles.boldLabel);
        textureMaxSize = EditorGUILayout.IntPopup("Max Texture Size", textureMaxSize, 
            new string[] { "256", "512", "1024", "2048" },
            new int[] { 256, 512, 1024, 2048 });
        
        EditorGUILayout.Space();
        
        // Buttons
        if (GUILayout.Button("1. Analyze 3D Models", GUILayout.Height(30)))
        {
            AnalyzeModels();
        }
        
        if (GUILayout.Button("2. Extract Materials from Models", GUILayout.Height(30)))
        {
            ExtractAllMaterials();
        }
        
        if (GUILayout.Button("3. Optimize Models", GUILayout.Height(30)))
        {
            OptimizeAllModels();
        }
        
        if (GUILayout.Button("4. Optimize Extracted Textures", GUILayout.Height(30)))
        {
            OptimizeExtractedTextures();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🚀 RUN ALL (Recommended)", GUILayout.Height(40)))
        {
            RunFullOptimization();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    // STEP 1: Analyze - CẬP NHẬT ĐỂ TÌM TẤT CẢ 3D MODELS
    void AnalyzeModels()
    {
        // Tìm TẤT CẢ 3D models (glb, gltf, fbx, obj)
        string[] allModels = AssetDatabase.FindAssets("t:Model")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => 
                path.EndsWith(".glb") || 
                path.EndsWith(".gltf") || 
                path.EndsWith(".fbx") ||
                path.EndsWith(".obj"))
            .ToArray();
        
        Debug.Log("=== 3D MODELS ANALYSIS ===");
        Debug.Log($"Found {allModels.Length} 3D model files\n");
        
        long totalSize = 0;
        int glbCount = 0;
        int fbxCount = 0;
        int objCount = 0;
        
        foreach (string path in allModels)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists) continue;
            
            long sizeInBytes = fileInfo.Length;
            float sizeInMB = sizeInBytes / (1024f * 1024f);
            totalSize += sizeInBytes;
            
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".glb" || ext == ".gltf") glbCount++;
            else if (ext == ".fbx") fbxCount++;
            else if (ext == ".obj") objCount++;
            
            string sizeColor = sizeInMB > 50 ? "red" : (sizeInMB > 20 ? "yellow" : "green");
            Debug.Log($"<color={sizeColor}>{Path.GetFileName(path)}: {sizeInMB:F2} MB</color>");
            Debug.Log($"  Path: {path}");
            
            // Check materials
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null)
            {
                if (importer.materialLocation == ModelImporterMaterialLocation.InPrefab)
                {
                    Debug.Log($"  → Materials: EMBEDDED ⚠️ (need extraction!)");
                }
                else
                {
                    Debug.Log($"  → Materials: External ✓");
                }
                
                Debug.Log($"  → Read/Write: {(importer.isReadable ? "ON ⚠️" : "OFF ✓")}");
                Debug.Log($"  → Mesh Compression: {importer.meshCompression}");
            }
            Debug.Log("");
        }
        
        float totalMB = totalSize / (1024f * 1024f);
        Debug.Log($"<b>SUMMARY:</b>");
        Debug.Log($"GLB/GLTF: {glbCount} files");
        Debug.Log($"FBX: {fbxCount} files");
        Debug.Log($"OBJ: {objCount} files");
        Debug.Log($"<b>TOTAL SIZE: {totalMB:F2} MB</b>");
        Debug.Log("=========================\n");
        
        EditorUtility.DisplayDialog("Analysis Complete", 
            $"Found {allModels.Length} 3D models:\n\n" +
            $"• GLB/GLTF: {glbCount}\n" +
            $"• FBX: {fbxCount}\n" +
            $"• OBJ: {objCount}\n\n" +
            $"Total size: {totalMB:F2} MB\n\n" +
            "Check Console for details.", 
            "OK");
    }
    
    // STEP 2: Extract Materials - CẬP NHẬT
    void ExtractAllMaterials()
    {
        string[] allModels = AssetDatabase.FindAssets("t:Model")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => 
                path.EndsWith(".glb") || 
                path.EndsWith(".gltf") || 
                path.EndsWith(".fbx") ||
                path.EndsWith(".obj"))
            .ToArray();
        
        int extracted = 0;
        int alreadyExternal = 0;
        
        EditorUtility.DisplayProgressBar("Extracting Materials", "Processing...", 0);
        
        for (int i = 0; i < allModels.Length; i++)
        {
            string path = allModels[i];
            
            EditorUtility.DisplayProgressBar("Extracting Materials", 
                $"Processing {Path.GetFileName(path)}...", 
                (float)i / allModels.Length);
            
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer != null)
            {
                if (importer.materialLocation == ModelImporterMaterialLocation.InPrefab)
                {
                    // Create Materials folder
                    string folderPath = Path.GetDirectoryName(path) + "/Materials";
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        string parentFolder = Path.GetDirectoryName(path);
                        string folderName = "Materials";
                        AssetDatabase.CreateFolder(parentFolder, folderName);
                    }
                    
                    // Extract materials and textures
                    importer.materialLocation = ModelImporterMaterialLocation.External;
                    importer.ExtractTextures(folderPath);
                    
                    importer.SaveAndReimport();
                    
                    Debug.Log($"✓ Extracted materials: {Path.GetFileName(path)}");
                    extracted++;
                }
                else
                {
                    alreadyExternal++;
                }
            }
        }
        
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n<b>EXTRACTION COMPLETE:</b>");
        Debug.Log($"Extracted: {extracted} models");
        Debug.Log($"Already external: {alreadyExternal} models\n");
        
        EditorUtility.DisplayDialog("Extraction Complete", 
            $"Extracted materials from {extracted} models\n" +
            $"Already external: {alreadyExternal} models\n\n" +
            "Check Project window for Materials folders.", 
            "OK");
    }
    
    // STEP 3: Optimize Models - CẬP NHẬT
    void OptimizeAllModels()
    {
        string[] allModels = AssetDatabase.FindAssets("t:Model")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => 
                path.EndsWith(".glb") || 
                path.EndsWith(".gltf") || 
                path.EndsWith(".fbx") ||
                path.EndsWith(".obj"))
            .ToArray();
        
        int optimized = 0;
        
        EditorUtility.DisplayProgressBar("Optimizing Models", "Processing...", 0);
        
        for (int i = 0; i < allModels.Length; i++)
        {
            string path = allModels[i];
            
            EditorUtility.DisplayProgressBar("Optimizing Models", 
                $"Processing {Path.GetFileName(path)}...", 
                (float)i / allModels.Length);
            
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer != null)
            {
                // ===== CRITICAL SETTINGS =====
                
                // 1. Read/Write: OFF (giảm 50% memory!)
                importer.isReadable = false;
                
                // 2. Mesh Optimization
                importer.optimizeMeshPolygons = true;
                importer.optimizeMeshVertices = true;
                
                // 3. Mesh Compression: HIGH
                importer.meshCompression = ModelImporterMeshCompression.High;
                
                // 4. Remove unused data
                importer.importBlendShapes = false;
                importer.importVisibility = false;
                importer.importCameras = false;
                importer.importLights = false;
                
                // 5. Materials: External
                importer.materialLocation = ModelImporterMaterialLocation.External;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                
                // 6. Animation: OFF (nếu không dùng)
                importer.importAnimation = false;
                
                // 7. Normals & Tangents
                importer.importNormals = ModelImporterNormals.Import;
                importer.importTangents = ModelImporterTangents.CalculateMikk;
                
                importer.SaveAndReimport();
                
                Debug.Log($"✓ Optimized: {Path.GetFileName(path)}");
                optimized++;
            }
        }
        
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n<b>Optimized {optimized} models</b>\n");
        
        EditorUtility.DisplayDialog("Optimization Complete", 
            $"Optimized {optimized} models\n\n" +
            "Settings applied:\n" +
            "• Read/Write: OFF\n" +
            "• Mesh Compression: HIGH\n" +
            "• Removed unused data", 
            "OK");
    }
    
    // STEP 4: Optimize Textures - CẬP NHẬT
    void OptimizeExtractedTextures()
    {
        // Tìm textures trong thư mục 3d
        string[] textures = AssetDatabase.FindAssets("t:Texture2D")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.Contains("/3d/") || 
                          path.Contains("/Materials/") ||
                          path.Contains("/Textures/"))
            .ToArray();
        
        if (textures.Length == 0)
        {
            EditorUtility.DisplayDialog("No Textures Found", 
                "No textures found in 3d/Materials folders.\n\n" +
                "Make sure you ran 'Extract Materials' first.", 
                "OK");
            return;
        }
        
        int optimized = 0;
        
        EditorUtility.DisplayProgressBar("Optimizing Textures", "Processing...", 0);
        
        for (int i = 0; i < textures.Length; i++)
        {
            string path = textures[i];
            
            EditorUtility.DisplayProgressBar("Optimizing Textures", 
                $"Processing {Path.GetFileName(path)}...", 
                (float)i / textures.Length);
            
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                // ===== CRITICAL SETTINGS =====
                
                // 1. Read/Write: OFF
                importer.isReadable = false;
                
                // 2. Max Size
                importer.maxTextureSize = textureMaxSize;
                
                // 3. Mipmaps: ON
                importer.mipmapEnabled = true;
                
                // 4. Compression
                importer.textureCompression = TextureImporterCompression.Compressed;
                
                // ===== PLATFORM SPECIFIC =====
                
                // Default
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                defaultSettings.maxTextureSize = textureMaxSize;
                defaultSettings.format = TextureImporterFormat.Automatic;
                defaultSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(defaultSettings);
                
                // Android
                TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                androidSettings.overridden = true;
                androidSettings.name = "Android";
                androidSettings.maxTextureSize = textureMaxSize;
                androidSettings.format = TextureImporterFormat.ASTC_6x6;
                androidSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(androidSettings);
                
                // iOS
                TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
                iosSettings.overridden = true;
                iosSettings.name = "iPhone";
                iosSettings.maxTextureSize = textureMaxSize;
                iosSettings.format = TextureImporterFormat.ASTC_6x6;
                iosSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(iosSettings);
                
                // PC
                TextureImporterPlatformSettings standaloneSettings = new TextureImporterPlatformSettings();
                standaloneSettings.overridden = true;
                standaloneSettings.name = "Standalone";
                standaloneSettings.maxTextureSize = textureMaxSize;
                standaloneSettings.format = TextureImporterFormat.DXT5Crunched;
                standaloneSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(standaloneSettings);
                
                importer.SaveAndReimport();
                
                optimized++;
            }
        }
        
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n<b>Optimized {optimized} textures</b>\n");
        
        EditorUtility.DisplayDialog("Optimization Complete", 
            $"Optimized {optimized} textures\n\n" +
            $"Max size: {textureMaxSize}x{textureMaxSize}\n" +
            "Compression: ASTC (mobile), DXT (PC)", 
            "OK");
    }
    
    // RUN ALL
    void RunFullOptimization()
    {
        if (!EditorUtility.DisplayDialog("Full Optimization", 
            "This will:\n\n" +
            "1. Extract materials from 3D models\n" +
            "2. Optimize all models\n" +
            "3. Optimize all textures\n\n" +
            "This may take several minutes.\n\n" +
            "Continue?", 
            "Yes", "Cancel"))
        {
            return;
        }
        
        Debug.Log("=== STARTING FULL OPTIMIZATION ===\n");
        
        // Step 1
        Debug.Log("STEP 1/3: Extracting materials...");
        ExtractAllMaterials();
        
        // Step 2
        Debug.Log("\nSTEP 2/3: Optimizing models...");
        OptimizeAllModels();
        
        // Step 3
        Debug.Log("\nSTEP 3/3: Optimizing textures...");
        OptimizeExtractedTextures();
        
        Debug.Log("\n=== OPTIMIZATION COMPLETE ===\n");
        
        EditorUtility.DisplayDialog("Success!", 
            "Full optimization complete!\n\n" +
            "Check Console for details.\n\n" +
            "Now build your project to see the results.", 
            "OK");
    }
}
