using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ClothesItem
{
    public List<int> idList;
    public string Icon;
    public ClothesType type;
    public int index;
}

public class ClothesConfig
{
    public int SeclectSeq;//˳��
    public string SelectTableName;//����˳��չʾ�ķ�װ����
    public string SelectIcon;
}

public enum ClothesType
{
    None,
    Hair,
    Clothes,
    HairAcs,
    BcBg,
}



[CreateAssetMenu(fileName = "ClothesDataBase", menuName = "Game/Scripts/ClothesSc/ClothesDataBase")]
public class ClothesDataBase : ScriptableObject
{
    private Dictionary<ClothesType,Dictionary<int,ClothesItem>> _clothesDic = new Dictionary<ClothesType,  Dictionary<int, ClothesItem>>();

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

    public void AddClothesItem(ClothesItem item)
    {
        // 先检查allClothes中是否已存在相同的数据
        bool existsInAllClothes = allClothes.Any(c => c.idList[0] == item.idList[0] && c.type == item.type);//不需要全部遍历因为套装不可能重复
        if (existsInAllClothes)
        {
            Debug.Log($"已存在相同数据，跳过添加: id={item.idList[0]}, type={item.type}");
            return;
        }

        _clothesDic.TryGetValue(item.type, out var dic);
        if (dic == null)
        {
           _clothesDic.Add(item.type, new Dictionary<int, ClothesItem>());
           _clothesDic[item.type].Add(item.idList[0], item);
        }
        else 
        {
            dic.TryGetValue(item.idList[0], out var existingItem);
            if (existingItem==null)
            {
                dic.Add(item.idList[0], item);
            }
        }
        
        // 添加到allClothes
        allClothes.Add(item);
    }    
    //private UnityAction<ClothesItem> _onClothesChange;

    //public void AddEventListener(UnityAction<ClothesItem> OnAction)
    //{
    //    _onClothesChange += OnAction;
    //}

    //public void RemoveEventListener(UnityAction<ClothesItem> OnAction)
    //{
    //    _onClothesChange -= OnAction;
    //}
}
