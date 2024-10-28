using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 가만히 있는 상황과 4방향에 해당하는 애니메이션을 플레이시킨다
    /// </summary>
    /// <param name="direction">Vector2 방향값</param>
    public void SetAnimation(Vector2 direction)
    {
        if (direction == Vector2.zero) anim.Play("Start");
        else if (direction == Vector2.down) anim.Play("Down");
        else if (direction == Vector2.left) anim.Play("Left");
        else if (direction == Vector2.right) anim.Play("Right");
        else if (direction == Vector2.up) anim.Play("Up");
    }
}
