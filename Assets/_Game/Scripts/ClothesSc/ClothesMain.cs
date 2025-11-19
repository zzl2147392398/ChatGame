using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // 添加此行
using UnityEngine;

public class ClothesMain : MonoBehaviour
{
    //������Ϸ����
    public static int CurSelectIndex = 0;
    public GameObject SelectPanel;
    
    void LoadClothesDataFromJson()
    {
        ClothesDataBase.Instance.allClothes.Clear();
        TextAsset jsonText = Resources.Load<TextAsset>("ClothesData/ClothesList");
        if (jsonText == null)
        {
            Debug.LogError("δ�ҵ����������ļ� ClothesList.json");
            return;
        }
        // �����л�
        ClothesDataBase.Instance.clothesConfigs = JsonConvert.DeserializeObject<List<ClothesConfig>>(jsonText.text);
        for (int i = 0; i < ClothesDataBase.Instance.clothesConfigs.Count; i++)
        {
            jsonText = Resources.Load<TextAsset>(ClothesDataBase.Instance.clothesConfigs[i].SelectTableName);
            if (jsonText != null)
            {
                Debug.Log("������ʼ���ұ�======"+ ClothesDataBase.Instance.clothesConfigs[i].SelectTableName);
                List<ClothesItem> ClothesItems = JsonConvert.DeserializeObject<List<ClothesItem>>(jsonText.text);
                for (int j = 0; j < ClothesItems.Count; j++)
                {
                    // 检查是否已存在相同的数据
                    bool exists = ClothesDataBase.Instance.allClothes.Any(c => c.idList[0] == ClothesItems[j].idList[0] && c.type == ClothesItems[j].type);
                    if (!exists)
                    {
                        ClothesDataBase.Instance.allClothes.Add(ClothesItems[j]);
                    }
                    else
                    {
                        Debug.Log($"已存在相同数据，跳过添加: id={ClothesItems[j].idList[0]}, type={ClothesItems[j].type}");
                    }
                }
               
            }
        }
        //foreach (var group in chatGroups)
        //{
        //    chatInfoDic[group.NPCID] = group;
        //}
        //tarsche = chatInfoDic.Count; // �����ܿ�����
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadClothesDataFromJson();
        CurSelectIndex = 0;
        SelectPanel.SetActive(true);
        //1,��������
        //2,��ʼ����ɫװ��
        //3,��ʾ��ɫ��������
        // ��ɫ��Ч
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
