using UnityEngine;
using System.Collections;
using System.IO;
using JsonFx;

public static class DataSaveLoadSystem
{
    static string storagePath = Application.persistentDataPath + "/storage";
    public static void Write<T>(T data) where T : class
    {
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }
        string name = data.ToString();
        string path = storagePath + "/" + name;
        string json = JsonFx.Json.JsonWriter.Serialize(data);
        json = Encryption.EncryptString(json);
        Debug.Log(json);

        File.WriteAllText(path, json, System.Text.Encoding.UTF8);
    }

    public static void Write<T>(T data, string name) where T : class
    {
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        string path = storagePath + "/" + name;
        string json = JsonFx.Json.JsonWriter.Serialize(data);
        json = Encryption.EncryptString(json);
        Debug.Log(json);

        File.WriteAllText(path, json, System.Text.Encoding.UTF8);
    }

    public static T Read<T>(string name) where T : class
    {
        string path = storagePath + "/" + name;
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        json = Encryption.DecryptString(json);
        Debug.Log(json);
        return JsonFx.Json.JsonReader.Deserialize<T>(json);
    }
}
