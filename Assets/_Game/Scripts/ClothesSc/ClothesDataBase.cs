using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ClothesItem
{
    public int id;
    public string Icon;
    public ClothesType type;
}

public class ClothesConfig
{
    public int SeclectSeq;//顺序
    public string SelectTableName;//按照顺序展示的服装名称
    public string SelectIcon;
}

public enum ClothesType
{
    Hair,
    Face,
    Clothes,
    Bottom,
    Shoes,
    Accessories
}



[CreateAssetMenu(fileName = "ClothesDataBase", menuName = "Game/Scripts/ClothesSc/ClothesDataBase")]
public class ClothesDataBase : ScriptableObject
{
    public List<ClothesItem> allClothes = new List<ClothesItem>();

    public List<ClothesConfig> clothesConfigs = new List<ClothesConfig>();

    private static ClothesDataBase _instance;
    public static ClothesDataBase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<ClothesDataBase>("ClothesData/ClothesDataBase");
            return _instance;
        }
    }

    public List<ClothesItem> GetClothesByType(ClothesType type)
    {
        return allClothes.Where(c => c.type == type).ToList();
    }
}
