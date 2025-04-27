using UnityEngine;
//当动画状态机中的动画名被改变，只需要在这里修改对应的名字即可
public static class PlayerAnimationName
{
    public static readonly string Idle = "Idle";

    public static readonly string Walk = "Walk";
    public static readonly string WalkStop = "WalkStop";

    public static readonly string Run = "Run";
    public static readonly string RunStop = "RunStop";

    public static readonly string Dodge = "Dodge";//标签
    public static readonly string DodgeBack = "DodgeBack";
    public static readonly string DodgeForword = "DodgeForword";

    public static readonly string Sprint = "Sprint";
    public static readonly string SprintStop = "SprintStop";

    public static readonly string JumpStart = "JumpStart";//标签
    public static readonly string JumpIdleStart = "JumpIdleStart";
    public static readonly string JumpForwordStart = "JumpForwordStart";

    public static readonly string JumpLoop = "JumpLoop";
    public static readonly string JumpStop = "JumpStop";

    public static readonly string Combo = "Combo"; //标签
    public static readonly string Combo_1_1 = "Combo_1_1";
    public static readonly string Combo_1_2 = "Combo_1_2";
    public static readonly string Combo_1_3 = "Combo_1_3";
    public static readonly string Combo_1_4 = "Combo_1_4";
    public static readonly string Combo_1_5 = "Combo_1_5";
    public static readonly string AirCombo_1_1 = "AirCombo_1_1";
    public static readonly string AirCombo_1_2 = "AirCombo_1_2";
    public static readonly string AirCombo_1_3 = "AirCombo_1_3";
    public static readonly string AirCombo_1_4 = "AirCombo_1_4";

    public static readonly string SprintAttack = "SprintAttack"; //标签
    public static readonly string SprintAttack_1 = "SprintAttack_1";
    public static readonly string SprintAttack_2 = "SprintAttack_2";

    public static readonly string ChargeAttack_1 = "ChargeAttack_1";

    public static readonly string AirChargeAttack_1_1Start = "AirChargeAttack_1_1Start";
    public static readonly string AirChargeAttack_1_2Start = "AirChargeAttack_1_2Start";
    public static readonly string AirChargeAttack_1_1Loop = "AirChargeAttack_1_1Loop";
    public static readonly string AirChargeAttack_1_2Loop = "AirChargeAttack_1_2Loop";
    public static readonly string AirChargeAttack_1_1End = "AirChargeAttack_1_1End";
    public static readonly string AirChargeAttack_1_2End = "AirChargeAttack_1_2End";

    public static readonly string AirChargeAttack_2Start = "AirChargeAttack_2Start"; 
    public static readonly string AirChargeAttack_2Loop = "AirChargeAttack_2Loop"; 
    public static readonly string AirChargeAttack_2End = "AirChargeAttack_2End"; 


    public static readonly string AirIdle = "AirIdle";

    public static readonly string AirDodge = "AirDodge"; //标签
    public static readonly string AirDodge_B = "AirDodge_B";
    public static readonly string AirDodge_F = "AirDodge_F";

    public static readonly string PowerAttackStart = "PowerAttackStart";
    public static readonly string PowerAttackLoop = "PowerAttackLoop";
    public static readonly string PowerAttackEnd = "PowerAttackEnd";

    public static readonly string Hit = "Hit"; //标签
    public static readonly string Hit_F = "Hit_F";
    public static readonly string Hit_L = "Hit_L";
    public static readonly string Hit_R = "Hit_R";
    public static readonly string Hit_B = "Hit_B";
    public static readonly string Hit_Large_F = "Hit_Large_F";
    public static readonly string Hit_Large_L = "Hit_Large_L";
    public static readonly string Hit_Large_R = "Hit_Large_R";
    public static readonly string Hit_Large_B = "Hit_Large_B";
    public static readonly string Hit_Down= "Hit_Down";
    public static readonly string Get_Up = "Get_Up";
}
