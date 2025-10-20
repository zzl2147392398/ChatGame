using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesMain : MonoBehaviour
{
    //处理游戏进度
    public static int CurSelectIndex = 1;
    public Transform characterObj;
    private float duration = 1f;
    public GameObject SelectPanel;
    void LoadClothesDataFromJson()
    {
        ClothesDataBase.Instance.allClothes.Clear();
        TextAsset jsonText = Resources.Load<TextAsset>("ClothesData/ClothesList");
        if (jsonText == null)
        {
            Debug.LogError("未找到聊天数据文件 ClothesList.json");
            return;
        }
        // 反序列化
        ClothesDataBase.Instance.clothesConfigs = JsonConvert.DeserializeObject<List<ClothesConfig>>(jsonText.text);
        for (int i = 0; i < ClothesDataBase.Instance.clothesConfigs.Count; i++)
        {
            jsonText = Resources.Load<TextAsset>(ClothesDataBase.Instance.clothesConfigs[i].SelectTableName);
            if (jsonText != null)
            {
                Debug.Log("进来开始查找表======"+ ClothesDataBase.Instance.clothesConfigs[i].SelectTableName);
                List<ClothesItem> ClothesItems = JsonConvert.DeserializeObject<List<ClothesItem>>(jsonText.text);
                for (int j = 0; j < ClothesItems.Count; j++)
                {
                    ClothesDataBase.Instance.allClothes.Add(ClothesItems[i]);
                }
               
            }
        }
        //foreach (var group in chatGroups)
        //{
        //    chatInfoDic[group.NPCID] = group;
        //}
        //tarsche = chatInfoDic.Count; // 更新总卡牌数
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadClothesDataFromJson();
        characterObj = GameObject.Find("Body").transform;
        SelectPanel.SetActive(true);
        //1,创建数据
        //2,初始画角色装扮
        //3,显示角色出场动画
        // 角色动效
        var endscale = characterObj.localScale;
        characterObj.localScale =new Vector3(characterObj.localScale.x*2,characterObj.localScale.y*2,1);// Vector3.one * 2f;
        Sequence seq = DOTween.Sequence();
        seq.Append(characterObj.DOScale(endscale, duration).SetEase(Ease.OutBack))
           .Append(characterObj.DOMoveY(0, duration));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
