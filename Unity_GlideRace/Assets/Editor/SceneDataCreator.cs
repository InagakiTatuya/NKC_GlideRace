using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ファイル名を定数で管理するクラスを作成するスクリプトの例
/// </summary>
public static  class SceneDataCreator
{
    // コマンド名
    private const string COMMAND_NAME = "Tools/Create/Example";

    private static int settingScenes;

    /// <summary>
    /// ファイル名を定数で管理するクラスを作成します
    /// </summary>
    [MenuItem(COMMAND_NAME)]
    public static void Create()
    {
        if (!CanCreate())
        {
            var errormessage = "実行中またはコンパイル中では実行できません。";
            EditorUtility.DisplayDialog(string.Empty, errormessage, "OK");
            return; 
        }
        settingScenes = EditorBuildSettings.scenes.Length;
        if (settingScenes == 0)
        {
            var errormessage = "シーンが登録されていません。";
            EditorUtility.DisplayDialog(string.Empty, errormessage, "OK");
            return;
        }
        CreateScript();
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static void CreateScript()
    {
        string[] scenesName = new string[settingScenes];
        for (int i = 0; i < settingScenes; i++)
        {
            scenesName[i] = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
        }
        ConstantsClassCreator.Create("ScenesNames", "ProjectSettingに設定されているシーンを定数で管理するクラス", scenesName);
    }

    /// <summary>
    /// 定数で管理するクラスを作成できるかどうかを取得します
    /// </summary>
    [MenuItem(COMMAND_NAME, true)]
    private static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }
}