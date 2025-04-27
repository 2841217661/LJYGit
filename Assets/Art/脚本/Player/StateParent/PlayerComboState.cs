using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PlayerComboState : PlayerState
{
    public Vector3 rotateDir; //当前角色的朝向与输入的朝向差向量


    public PlayerComboState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rotateDir = Vector3.zero;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

    }

    //假设在第一次攻击后紧接着第二次攻击，但是玩家输入方向改变，应当将Player也进行对应方向改变
    //如果，在进行攻击时周围有敌人，则将player以较快的速度旋转面向敌人
    protected virtual void HandleRotate()
    {
        if (rotateDir != Vector3.zero)
        {
            //连击姿态中进行旋转处理
            // 计算目标旋转
            Quaternion newRotation = Quaternion.LookRotation(rotateDir);
            // 平滑旋转到目标
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.comboRotateSpeed * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
        }
    }

    //当玩家攻击时如果周围有敌人，则将自身朝向敌人
    protected virtual void SetRotateDir()
    {
        Vector3 nearEnemyPosition = playerManager.DetectionEnemyPositionInAttack();

        //这里有一个问题，如果没有检测到敌人，则会返回zero，但是如果敌人又刚好在zero，这里的判断会跳过(概率几乎为0)
        if (nearEnemyPosition != Vector3.zero) 
        {
            //// 朝向最近的敌人（只绕 Y 轴）
            //Vector3 direction = nearEnemyPosition - playerManager.transform.position;
            //direction.y = 0f;

            //rotateDir = direction;

            Vector3 dir = nearEnemyPosition - playerManager.transform.position;
            dir.y = 0;
            playerManager.transform.rotation = Quaternion.LookRotation(dir);
            rotateDir = Vector3.zero;
        }
        else //没有敌人，根据当前是输入和相机的方向判断
        {
            // 计算目标旋转方向
            Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                        + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0f;

            // 没有相机输入
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = playerManager.transform.forward;
            }

            rotateDir = targetRotationDirection;
        }
    }

    //当攻击时如果攻击到敌人，阻碍player移动
    protected virtual void PreventMovement()
    {
        //如果触碰到敌人，禁止rootMotion
        if (playerManager.AttackTouchEnemyDetecte())
        {
            useRootMotion = false;
        }
        else
        {
            useRootMotion = true;
        }
    }
}