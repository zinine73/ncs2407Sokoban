using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapStruct // 맵구조체
{
    public int SizeX;
    public int SizeY;
    public string Data;
}

public enum MapEnum // 맵에 등장하는 오브젝트들의 타입
{
    Ground, Wall, Goal, Box, BoxOnGoal, Player
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // 싱글톤
    private const float MOVE_SPEED = 7.0f; // 플레이어 움직이는 속도
    [SerializeField] private GameObject player; // 플레이어
    private Vector2 playerPosition; // 플레이어 위치
    private Vector2 playerDirection; // 플레이어 방향
    public MapStruct map; // 맵 정보

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // x,y 좌표에 따른 map에서의 위치 얻어오기
    private Vector2 GetPosition(float x, float y)
    {
        return new Vector2(x - map.SizeX * 0.5f, y - map.SizeY * 0.5f);
    }

    private void CheckKeyInput()
    {
        // 키가 눌리면
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            // x,y 축 값을 -1,0,1 로만 받아서
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            // 대각선 입력은 무시하고
            if (moveX == 0 || moveY == 0)
            {
                // 방향값을 정한다
                playerDirection = new Vector2(moveX, moveY);
                // 방향값에 따른 플레이어 애니메이션 실행
                player.GetComponent<PlayerAnimation>().SetAnimation(playerDirection);
            }   
        }
    }

    private void Update()
    {
        // 키입력 체크하고 방향, 위치값 변경
        CheckKeyInput();
        // 플레이어 이미지 위치 변경
        player.transform.position = Vector2.Lerp(player.transform.position,
            GetPosition(playerPosition.x, playerPosition.y), Time.deltaTime * MOVE_SPEED);
    }   
}
