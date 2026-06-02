using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabIconGenerator : EditorWindow
{
    private string rootFolder = "Assets/Art/3D/Prefabs";
    private float brightnessBoost;

    [MenuItem("Tools/Prefab Icon Generator")]
    public static void ShowWindow()
    {
        GetWindow<PrefabIconGenerator>("Prefab Icon Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Icon Generator", EditorStyles.boldLabel);

        rootFolder = EditorGUILayout.TextField("Root Folder", rootFolder);
        brightnessBoost = EditorGUILayout.Slider("Brightness Boost", brightnessBoost, 1f, 10f);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Icons"))
        {
            GenerateIcons();
        }
    }

    private void GenerateIcons()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { rootFolder });

        int generated = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            Texture2D preview = GetPreview(prefab);

            if (preview == null)
            {
                Debug.LogWarning($"No preview for {prefab.name}");
                continue;
            }

            SaveIcon(path, preview);
            generated++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"Generated {generated} prefab icons.");
    }

    private Texture2D GetPreview(GameObject prefab)
    {
        Texture2D preview = AssetPreview.GetAssetPreview(prefab);

        int attempts = 0;
        while (preview == null && attempts < 20)
        {
            System.Threading.Thread.Sleep(100);
            preview = AssetPreview.GetAssetPreview(prefab);
            attempts++;
        }

        return preview;
    }

    private void SaveIcon(string prefabPath, Texture2D source)
    {
        Texture2D readable = MakeReadable(source);
        Texture2D adjusted = BoostBrightness(readable, brightnessBoost);

        byte[] png = adjusted.EncodeToPNG();
        if (png == null) return;

        string directory = Path.GetDirectoryName(prefabPath);
        string name = Path.GetFileNameWithoutExtension(prefabPath);

        string outputPath = Path.Combine(directory, name + "_icon.png");

        File.WriteAllBytes(outputPath, png);

        AssetDatabase.ImportAsset(outputPath);

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(outputPath);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    // Fix GPU-only texture issue
    private Texture2D MakeReadable(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }

    // Brightness fix
    private Texture2D BoostBrightness(Texture2D source, float boost)
    {
        Texture2D result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);

        Color[] pixels = source.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];

            c.r *= boost;
            c.g *= boost;
            c.b *= boost;

            pixels[i] = c;
        }

        result.SetPixels(pixels);
        result.Apply();

        return result;
    }
}