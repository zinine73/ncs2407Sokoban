using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    [SerializeField] private Button btnRestart; // 다시하기 버튼
    [SerializeField] private Button btnUndo; // 되돌리기 버튼
    [SerializeField] private GameObject panelTitle; // 타이틀 화면
    [SerializeField] private GameObject panelChooseMap; // 맵 고르기 화면
    [SerializeField] private GameObject panelPlay; // 플레이 화면
    [SerializeField] private GameObject panelGoal; // 클리어 시 화면
    [SerializeField] private MapButton mapButtonPrefab; // 맵버튼 프레팝 연결
    [SerializeField] private RectTransform contentTransform; // 스크롤뷰에서 버튼 프레팝이 들어갈 위치
    private MapButton[] mapButton = null; // 맵버튼 배열

    // 버튼이 미리 만들어져 있어야 클리어여부가 제대로 표시된다. 그래서 Start가 아닌 Awake
    private void Awake()
    {
        // 게임매니저에 있는 최대 맵 갯수만큼 배열을 만든다.
        mapButton = new MapButton[GameManager.MAX_MAP_COUNT];
        for (int i = 0; i < GameManager.MAX_MAP_COUNT; i++)
        {
            // 스크롤뷰에 맵버튼프레팝으로 클론을 만들어서 넣는다
            mapButton[i] = Instantiate(mapButtonPrefab, contentTransform);
            // 번호는 1번부터 시작하게
            mapButton[i].SetNumber(i + 1);
        }
    }

    /// <summary>
    /// 맵 클리어 정보 지정. 번호 위에 별이 표시된다.
    /// 다시하기와 되돌리기 버튼은 안눌리게 처리
    /// </summary>
    /// <param name="number">클리어 한 맵 번호</param>
    public void SetMapCleared(int number)
    {
        mapButton[number - 1].SetCleared(true);
        btnRestart.interactable = false;
        btnUndo.interactable = false;
    }

    /// <summary>
    /// 게임 상태에 따른 패널 켜고 끄기
    /// </summary>
    /// <param name="state">게임 상태</param>
    public void SetView(GameState state)
    {
        if (state == GameState.Title)
        {
            panelTitle.SetActive(true);
            panelChooseMap.SetActive(false);
            panelPlay.SetActive(false);
            panelGoal.SetActive(false);
        }
        else if (state == GameState.ChooseMap)
        {
            panelTitle.SetActive(false);
            panelChooseMap.SetActive(true);
            panelPlay.SetActive(false);
            panelGoal.SetActive(false);
            for (int i = 0; i < GameManager.MAX_MAP_COUNT; i++)
            {
                // todo 맵 클리어 상황에 따라 별 표시
            }
        }
        else if (state == GameState.Goal)
        {
            panelGoal.SetActive(true);
        }
        else // Play
        {
            panelTitle.SetActive(false);
            panelChooseMap.SetActive(false);
            panelPlay.SetActive(true);
            panelGoal.SetActive(false);
            btnRestart.interactable = false;
            btnUndo.interactable = false;
        }
    }

    /// <summary>
    /// 다시하기 버튼 켜고 끄기
    /// </summary>
    /// <param name="value">켜고 끄기 값</param>
    public void SetRestartButton(bool value)
    {
        btnRestart.interactable = value;
    }

    /// <summary>
    /// 되돌리기 버튼 켜고 끄기
    /// </summary>
    /// <param name="value">켜고 끄기 값</param>
    public void SetUndoButton(bool value)
    {
        btnUndo.interactable = value;
    }
}
