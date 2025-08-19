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

    [MenuItem("Tools/ͼ�������޸Ĺ���")]
    public static void ShowWindow()
    {
        GetWindow<SpritePivotEditorWindow>("ͼ�������޸Ĺ���");
    }

    void OnGUI()
    {
        GUILayout.Label("ͼ�����������޸Ĺ���", EditorStyles.boldLabel);

        // ѡ��ͼ��
        EditorGUILayout.Space();
        GUILayout.Label("ѡ��ͼ��:", EditorStyles.boldLabel);
        selectedTexture = (Texture2D)EditorGUILayout.ObjectField("ͼ��", selectedTexture, typeof(Texture2D), false);

        if (selectedTexture == null)
        {
            EditorGUILayout.HelpBox("��ѡ��һ��ͼ������", MessageType.Info);
            return;
        }

        // ��������ѡ��
        EditorGUILayout.Space();
        GUILayout.Label("��������:", EditorStyles.boldLabel);

        // ѡ��ʹ��Ԥ�����Ļ����Զ�������
        EditorGUILayout.BeginHorizontal();
        useCustomPivot = EditorGUILayout.Toggle("ʹ���Զ�������", useCustomPivot);
        if (!useCustomPivot)
        {
            selectedAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup(selectedAlignment);
        }
        EditorGUILayout.EndHorizontal();

        // �Զ�����������
        if (useCustomPivot)
        {
            EditorGUILayout.Space();
            GUILayout.Label("�Զ�������:", EditorStyles.boldLabel);
            customPivot = EditorGUILayout.Vector2Field("���ĵ� (0-1)", customPivot);

            // ��������ֵ��0-1��Χ��
            customPivot.x = Mathf.Clamp01(customPivot.x);
            customPivot.y = Mathf.Clamp01(customPivot.y);

            // Ԥ�谴ť
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("����"))
            {
                customPivot = new Vector2(0.5f, 0.5f);
            }
            if (GUILayout.Button("�ײ�����"))
            {
                customPivot = new Vector2(0.5f, 0f);
            }
            if (GUILayout.Button("��������"))
            {
                customPivot = new Vector2(0.5f, 1f);
            }
            if (GUILayout.Button("����"))
            {
                customPivot = new Vector2(0f, 0f);
            }
            if (GUILayout.Button("����"))
            {
                customPivot = new Vector2(1f, 0f);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Ԥ����ǰ����
        EditorGUILayout.Space();
        if (useCustomPivot)
        {
            GUILayout.Label($"��ǰ����: ({customPivot.x:F2}, {customPivot.y:F2})");
        }
        else
        {
            GUILayout.Label($"��ǰ����: {selectedAlignment}");
        }

        // Ӧ�ð�ť
        EditorGUILayout.Space();
        if (GUILayout.Button("Ӧ�����ĵ�������Ƭ", GUILayout.Height(40)))
        {
            ApplyPivotToAllSprites();
        }

        // ��ʾ�����б�͵�ǰ����
        EditorGUILayout.Space();
        GUILayout.Label("ͼ����Ƭ�б�:", EditorStyles.boldLabel);

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            // ʹ���µ�API��ȡ��������
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();

            var spriteRects = dataProvider.GetSpriteRects();
            foreach (var spriteRect in spriteRects)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(spriteRect.name, GUILayout.Width(150));
                GUILayout.Label($"����: ({spriteRect.pivot.x:F0}, {spriteRect.pivot.y:F0})");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        // ��ʾ������Ϣ
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("�˲��������޸�ͼ����������Ƭ�����ĵ㣬�޷������������ȱ�����Ŀ��", MessageType.Warning);
    }

    private void ApplyPivotToAllSprites()
    {
        if (selectedTexture == null)
        {
            EditorUtility.DisplayDialog("����", "����ѡ��һ��ͼ��", "ȷ��");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null)
        {
            EditorUtility.DisplayDialog("����", "�޷���ȡ��������", "ȷ��");
            return;
        }

        if (textureImporter.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorUtility.DisplayDialog("����", "�������Ƕྫ��ͼ��", "ȷ��");
            return;
        }

        // ʹ���µ�API��ȡ��������
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
        dataProvider.InitSpriteEditorDataProvider();

        var spriteRects = dataProvider.GetSpriteRects();

        if (spriteRects == null || spriteRects.Length == 0)
        {
            EditorUtility.DisplayDialog("����", "��ͼ��û����Ƭ", "ȷ��");
            return;
        }

        // �޸�������Ƭ������
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

        // Ӧ���޸�
        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();

        // ���µ�������
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        EditorUtility.DisplayDialog("�ɹ�", $"�ѳɹ��޸� {spriteRects.Length} ����Ƭ������", "ȷ��");

        // ˢ�´���
        Repaint();
    }
}