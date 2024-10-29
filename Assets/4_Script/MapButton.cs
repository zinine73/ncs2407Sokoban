using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumber;
    [SerializeField] private Image imageCleared;
    private int number;

    /// <summary>
    /// 맵 번호 지정하기
    /// </summary>
    /// <param name="value">맵 번호</param>
    public void SetNumber(int value)
    {
        number = value;
        textNumber.text = $"{number:00}";
    }

    /// <summary>
    /// 클리어 여부 지정하기
    /// </summary>
    /// <param name="value">클리어 여부</param>
    public void SetCleared(bool value)
    {
        imageCleared.gameObject.SetActive(value);
    }

    /// <summary>
    /// 맵 데이터 부르고 시작하기
    /// </summary>
    public void LoadMap()
    {
        GameManager.instance.StartPlay(number);
    }
}
