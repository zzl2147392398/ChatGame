using System;
using System.Collections.Generic;
using UnityEngine;

// JsonUtility ��֧�� List<T>�����Զ����װ
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