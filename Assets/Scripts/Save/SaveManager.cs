using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave() => File.Exists(path);

    public static void DeleteSave()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
// C:/ Users / Admin / AppData / LocalLow / DefaultCompany / Pixel_Adventure / save.json