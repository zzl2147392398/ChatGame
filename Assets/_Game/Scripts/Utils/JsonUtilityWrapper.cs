using System;
using System.Collections.Generic;
using UnityEngine;

// JsonUtility 不支持 List<T>，需自定义包装
public static class JsonUtilityWrapper
{
    public static List<T> FromJsonList<T>(string json)
    {
        string newJson = "{ \"list\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.list;
    }

    [Serializable]
    public class Wrapper<T>
    {
        public List<T> list;
    }
}