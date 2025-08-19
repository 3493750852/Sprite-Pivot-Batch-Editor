using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Sprites;

public class SpritePivotEditorWindow : EditorWindow
{
    private Texture2D selectedTexture;
    private Vector2 customPivot = new Vector2(0.5f, 0.5f);
    private Vector2 scrollPosition;
    private bool useCustomPivot = true;
    private SpriteAlignment selectedAlignment = SpriteAlignment.Center;

    [MenuItem("Tools/图集轴心修改工具")]
    public static void ShowWindow()
    {
        GetWindow<SpritePivotEditorWindow>("图集轴心修改工具");
    }

    void OnGUI()
    {
        GUILayout.Label("图集轴心批量修改工具", EditorStyles.boldLabel);

        // 选择图集
        EditorGUILayout.Space();
        GUILayout.Label("选择图集:", EditorStyles.boldLabel);
        selectedTexture = (Texture2D)EditorGUILayout.ObjectField("图集", selectedTexture, typeof(Texture2D), false);

        if (selectedTexture == null)
        {
            EditorGUILayout.HelpBox("请选择一个图集纹理", MessageType.Info);
            return;
        }

        // 轴心设置选项
        EditorGUILayout.Space();
        GUILayout.Label("轴心设置:", EditorStyles.boldLabel);

        // 选择使用预设轴心还是自定义轴心
        EditorGUILayout.BeginHorizontal();
        useCustomPivot = EditorGUILayout.Toggle("使用自定义轴心", useCustomPivot);
        if (!useCustomPivot)
        {
            selectedAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup(selectedAlignment);
        }
        EditorGUILayout.EndHorizontal();

        // 自定义轴心设置
        if (useCustomPivot)
        {
            EditorGUILayout.Space();
            GUILayout.Label("自定义轴心:", EditorStyles.boldLabel);
            customPivot = EditorGUILayout.Vector2Field("轴心点 (0-1)", customPivot);

            // 限制轴心值在0-1范围内
            customPivot.x = Mathf.Clamp01(customPivot.x);
            customPivot.y = Mathf.Clamp01(customPivot.y);

            // 预设按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("中心"))
            {
                customPivot = new Vector2(0.5f, 0.5f);
            }
            if (GUILayout.Button("底部中心"))
            {
                customPivot = new Vector2(0.5f, 0f);
            }
            if (GUILayout.Button("顶部中心"))
            {
                customPivot = new Vector2(0.5f, 1f);
            }
            if (GUILayout.Button("左下"))
            {
                customPivot = new Vector2(0f, 0f);
            }
            if (GUILayout.Button("右下"))
            {
                customPivot = new Vector2(1f, 0f);
            }
            EditorGUILayout.EndHorizontal();
        }

        // 预览当前轴心
        EditorGUILayout.Space();
        if (useCustomPivot)
        {
            GUILayout.Label($"当前轴心: ({customPivot.x:F2}, {customPivot.y:F2})");
        }
        else
        {
            GUILayout.Label($"当前轴心: {selectedAlignment}");
        }

        // 应用按钮
        EditorGUILayout.Space();
        if (GUILayout.Button("应用轴心到所有切片", GUILayout.Height(40)))
        {
            ApplyPivotToAllSprites();
        }

        // 显示精灵列表和当前轴心
        EditorGUILayout.Space();
        GUILayout.Label("图集切片列表:", EditorStyles.boldLabel);

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            // 使用新的API获取精灵数据
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();

            var spriteRects = dataProvider.GetSpriteRects();
            foreach (var spriteRect in spriteRects)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(spriteRect.name, GUILayout.Width(150));
                GUILayout.Label($"轴心: ({spriteRect.pivot.x:F0}, {spriteRect.pivot.y:F0})");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        // 显示警告信息
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("此操作将会修改图集中所有切片的轴心点，无法撤销。建议先备份项目。", MessageType.Warning);
    }

    private void ApplyPivotToAllSprites()
    {
        if (selectedTexture == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择一个图集", "确定");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null)
        {
            EditorUtility.DisplayDialog("错误", "无法获取纹理导入器", "确定");
            return;
        }

        if (textureImporter.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorUtility.DisplayDialog("错误", "该纹理不是多精灵图集", "确定");
            return;
        }

        // 使用新的API获取精灵数据
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
        dataProvider.InitSpriteEditorDataProvider();

        var spriteRects = dataProvider.GetSpriteRects();

        if (spriteRects == null || spriteRects.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "该图集没有切片", "确定");
            return;
        }

        // 修改所有切片的轴心
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

        // 应用修改
        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();

        // 重新导入纹理
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        EditorUtility.DisplayDialog("成功", $"已成功修改 {spriteRects.Length} 个切片的轴心", "确定");

        // 刷新窗口
        Repaint();
    }
}