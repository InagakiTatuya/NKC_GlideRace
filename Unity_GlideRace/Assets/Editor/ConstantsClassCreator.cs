using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

/// <summary>
/// 定数を管理するクラスを生成するクラス
/// </summary>
public static class ConstantsClassCreator
{

    //無効な文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
  {
    " ", "!", "\"", "#", "$",
    "%", "&", "\'", "(", ")",
    "-", "=", "^",  "~", "\\",
    "|", "[", "{",  "@", "`",
    "]", "}", ":",  "*", ";",
    "+", "/", "?",  ".", ">",
    ",", "<"
  };

    //定数の区切り文字
    private const char DELIMITER = '_';

    /// <summary>
    /// 定数を管理するクラスを自動生成する
    /// </summary>
    /// <param name="className">クラスの名前</param>
    /// <param name="classInfo">なんのクラスか説明するコメント文</param>
    /// <param name="variableDic">定数名とその値をセットで登録したDictionary</param>
    /// <typeparam name="T">定数の型、stringかintかfloat</typeparam>
    public static void Create(string className, string classInfo, string[] value)
    {
        //コメント文とクラス名を入力
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("/// <summary>");
        builder.AppendFormat("/// {0}", classInfo).AppendLine();
        builder.AppendLine("/// </summary>");
        builder.AppendFormat("namespace {0}", className).AppendLine("{");
        builder.Append("\t").AppendFormat("public enum SceneName").AppendLine("{");
        for (int i = 0; i < value.Length; i++)
        {
            builder.Append("\t\t").AppendFormat("{0},", value[i]).AppendLine("");
        }

        builder.Append("\t").AppendLine("}");
        builder.AppendLine("}");

        //書き出し、ファイル名はクラス名.cs
        string exportPath = "Assets/Scripts/Scripts/Constants/AutoCreating/" + className + ".cs";

        //書き出し先のディレクトリが無ければ作成
        string directoryName = Path.GetDirectoryName(exportPath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(exportPath, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        Debug.Log(className+"の作成が完了しました");
    }
}