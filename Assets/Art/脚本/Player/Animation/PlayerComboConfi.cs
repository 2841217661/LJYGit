using UnityEngine;

[System.Serializable]
public class PlayerComboConfi
{
    public string comboName; //��������
    public float exitTime; //���˳�ʱ��:�ö�������ʱ��ﵽ��ʱ����δ�ﵽ���ʱ�䣬�����ƶ�������Դ������
    public float nextComboTime; //���ν���һ����������Сʱ��(С����󶯻�ʱ��)
    public float preComboInputTime; //��Ԥ����ʱ��
    public float attackDetecteTime; //���й������ʱ��
}
