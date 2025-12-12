using System;
using UnityEngine;

[Serializable]
public class JsonData<T> where T : JsonData<T>
{
    public static void Save(T data, string path)
    {
        try
        {
            string jsonString = JsonUtility.ToJson(data, true);
            //Debug.Log("json: " + jsonString);
            System.IO.File.WriteAllText(path, jsonString);
            Debug.Log("Save successed. \n" + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.StackTrace);
        }
    }
    public static T Load(string fullPath)
    {
        try
        {
            string jsonString = System.IO.File.ReadAllText(fullPath);
            T config = JsonUtility.FromJson<T>(jsonString);
            Debug.Log(string.Format($"Load {0} successed. \n{1}", typeof(T), fullPath));
            return config;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.StackTrace);
            return null;
        }
    }
}

