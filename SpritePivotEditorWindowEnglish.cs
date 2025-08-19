using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Sprites;

public class SpritePivotEditorWindowEnglish : EditorWindow
{
    private Texture2D selectedTexture;
    private Vector2 customPivot = new Vector2(0.5f, 0.5f);
    private Vector2 scrollPosition;
    private bool useCustomPivot = true;
    private SpriteAlignment selectedAlignment = SpriteAlignment.Center;

    [MenuItem("Tools/Sprite Pivot Batch Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpritePivotEditorWindowEnglish>("Sprite Pivot Batch Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite Pivot Batch Editor", EditorStyles.boldLabel);

        // Select Atlas
        EditorGUILayout.Space();
        GUILayout.Label("Select Atlas:", EditorStyles.boldLabel);
        selectedTexture = (Texture2D)EditorGUILayout.ObjectField("Atlas", selectedTexture, typeof(Texture2D), false);

        if (selectedTexture == null)
        {
            EditorGUILayout.HelpBox("Please select a texture atlas", MessageType.Info);
            return;
        }

        // Pivot Settings
        EditorGUILayout.Space();
        GUILayout.Label("Pivot Settings:", EditorStyles.boldLabel);

        // Choose between preset pivot or custom pivot
        EditorGUILayout.BeginHorizontal();
        useCustomPivot = EditorGUILayout.Toggle("Use Custom Pivot", useCustomPivot);
        if (!useCustomPivot)
        {
            selectedAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup(selectedAlignment);
        }
        EditorGUILayout.EndHorizontal();

        // Custom Pivot Settings
        if (useCustomPivot)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Custom Pivot:", EditorStyles.boldLabel);
            customPivot = EditorGUILayout.Vector2Field("Pivot Point (0-1)", customPivot);

            // Clamp pivot values between 0-1
            customPivot.x = Mathf.Clamp01(customPivot.x);
            customPivot.y = Mathf.Clamp01(customPivot.y);

            // Preset Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Center"))
            {
                customPivot = new Vector2(0.5f, 0.5f);
            }
            if (GUILayout.Button("Bottom Center"))
            {
                customPivot = new Vector2(0.5f, 0f);
            }
            if (GUILayout.Button("Top Center"))
            {
                customPivot = new Vector2(0.5f, 1f);
            }
            if (GUILayout.Button("Bottom Left"))
            {
                customPivot = new Vector2(0f, 0f);
            }
            if (GUILayout.Button("Bottom Right"))
            {
                customPivot = new Vector2(1f, 0f);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Preview Current Pivot
        EditorGUILayout.Space();
        if (useCustomPivot)
        {
            GUILayout.Label($"Current Pivot: ({customPivot.x:F2}, {customPivot.y:F2})");
        }
        else
        {
            GUILayout.Label($"Current Pivot: {selectedAlignment}");
        }

        // Apply Button
        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Pivot to All Sprites", GUILayout.Height(40)))
        {
            ApplyPivotToAllSprites();
        }

        // Display Sprite List and Current Pivots
        EditorGUILayout.Space();
        GUILayout.Label("Atlas Sprite List:", EditorStyles.boldLabel);

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            // Use new API to get sprite data
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();

            var spriteRects = dataProvider.GetSpriteRects();
            foreach (var spriteRect in spriteRects)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(spriteRect.name, GUILayout.Width(150));
                GUILayout.Label($"Pivot: ({spriteRect.pivot.x:F0}, {spriteRect.pivot.y:F0})");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        // Display Warning Message
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This operation will modify the pivot points of all sprites in the atlas and cannot be undone. It is recommended to backup your project first.", MessageType.Warning);
    }

    private void ApplyPivotToAllSprites()
    {
        if (selectedTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select an atlas first", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null)
        {
            EditorUtility.DisplayDialog("Error", "Unable to get texture importer", "OK");
            return;
        }

        if (textureImporter.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorUtility.DisplayDialog("Error", "This texture is not a multiple sprite atlas", "OK");
            return;
        }

        // Use new API to get sprite data
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
        dataProvider.InitSpriteEditorDataProvider();

        var spriteRects = dataProvider.GetSpriteRects();

        if (spriteRects == null || spriteRects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "This atlas has no sprites", "OK");
            return;
        }

        // Modify pivot of all sprites
        foreach (var spriteRect in spriteRects)
        {
            if (useCustomPivot)
            {
                spriteRect.alignment = SpriteAlignment.Custom;
                spriteRect.pivot = customPivot;
            }
            else
            {
                spriteRect.alignment = selectedAlignment;
            }
        }

        // Apply changes
        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();

        // Reimport texture
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        EditorUtility.DisplayDialog("Success", $"Successfully modified pivot of {spriteRects.Length} sprites", "OK");

        // Refresh window
        Repaint();
    }
}