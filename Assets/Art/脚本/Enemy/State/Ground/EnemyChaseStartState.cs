using UnityEngine;

public class EnemyChaseStartState : EnemyGroundState
{
    private Vector3 targetRotateDir;
    private float turnSpeed = 1f; //旋转速度
    public EnemyChaseStartState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        MixDrawKinfeAndTurn();

        enemyManager.animator.SetFloat("Attack", 1f);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        //在转向过程中旋转面向追逐目标
        Rotate();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        //this -> idle :正在过渡idle中...
        if(stateInfo.IsName(animationName) && enemyManager.animator.IsInTransition(0))
        {
            ChangeState(enemyManager.idleState);
        }
    }

    //混合拔刀动画和原地旋转动画
    private void MixDrawKinfeAndTurn()
    {
        targetRotateDir = enemyManager.player.position - enemyManager.transform.position; ;
        targetRotateDir.y = 0;
        targetRotateDir = targetRotateDir.normalized;

        float angle = Vector3.SignedAngle(new Vector3(enemyManager.transform.forward.x, 0f, enemyManager.transform.forward.z).normalized, targetRotateDir, Vector3.up);
        //根据前方向量与相对于玩家的向量决定转向动画
        string name = angle switch
        {
            >= 135f => EnemyAnimationName.Turn_R_180,
            < -135 => EnemyAnimationName.Turn_L_180,
            >= 45 and < 135 => EnemyAnimationName.Turn_R_90,
            < -45 and > -135 => EnemyAnimationName.Turn_L_90,
            _ => ""
        };

        if (name != "")
        {
            //融合下半身旋转动画
            enemyManager.animator.CrossFadeInFixedTime(name, 0.2f, 1);
        }
        //播放收刀动画
        enemyManager.animator.CrossFadeInFixedTime(animationName, 0.2f);
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetRotateDir);
        enemyManager.transform.rotation = Quaternion.Lerp(
            enemyManager.transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }
}
