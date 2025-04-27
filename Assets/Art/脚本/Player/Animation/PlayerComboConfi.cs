using UnityEngine;

[System.Serializable]
public class PlayerComboConfi
{
    public string comboName; //连击名字
    public float exitTime; //可退出时间:该动画播放时间达到该时间且未达到最大时间，进行移动输入可以打断连击
    public float nextComboTime; //可衔接下一个连击的最小时间(小于最大动画时间)
    public float preComboInputTime; //可预输入时间
    public float attackDetecteTime; //进行攻击检测时间
}
