using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour
{

    private Image Sche;
    private Image Bgimage;
    // Start is called before the first frame update
    void Start()
    {
        Sche = transform.Find("Aperture/Sche").GetComponent<Image>();
        Bgimage = transform.GetComponent<Image>();
        StartCoroutine(AddSche(0.5f, 10,() =>
        {
            if (Sche.fillAmount < 1)
            {
                Sche.fillAmount += 0.1f;
            }
            else {
                Sche.fillAmount = 1;
                UIManager.Instance.ShowAndCloseOtherPanel("selectpanel");
            }
        }));
    }

    void OnDestroy()
    {
        Sche.fillAmount = 0;
    }
    IEnumerator AddSche(float interval,float num, System.Action action)
    {
        while (num>=0)
        {
            Debug.Log("´¥·¢"+num);
            num -= 1;
            yield return new WaitForSeconds(interval);
            action?.Invoke();
        }
    }
}
