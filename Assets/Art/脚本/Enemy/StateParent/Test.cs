using UnityEngine;
using static EnumManager;

public class Test : MonoBehaviour
{
    public Transform testPoint1;
    public Transform testPoint2;
    public Transform testPoint3;
    public Transform testPoint4;
    public Transform testPoint5;
    public Transform testPoint6;
    public Transform testPoint7;
    public Transform testPoint8;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            RandomMoveToTestPoint();
        }
    }

    [Header("�������")]
    public float attackRadius; //������ⷶΧ
    public Vector3 attackOffsetPoint; //�������Բ��������λ�õ�ƫ����
    public LayerMask attackTargetLayer; //���Թ����Ķ���㼶
    //�������
    public bool AttackDetecte(int _attackType)
    {
        // ������ת��Ϊ DamageType
        PlayerDamageType damageType = _attackType switch
        {
            0 => PlayerDamageType.None,
            1 => PlayerDamageType.Light,  // ���
            2 => PlayerDamageType.Medium,   // �еȹ���
            3 => PlayerDamageType.Heavy,    // �ػ�
            4 => PlayerDamageType.FlyAway, //����
            5 => PlayerDamageType.AirLight, //�������
            _ => PlayerDamageType.None, //Ĭ���޹�������
        };

        // ���㹥��ƫ��λ��
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;

        Collider[] players = Physics.OverlapSphere(transform.position + offsetPoint, attackRadius, attackTargetLayer);

        if (players.Length == 0)
        {
            Debug.Log("�޶���");
            return false;
        }

        foreach (Collider player in players)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                Debug.Log("��");
            }

            //��ȡ������ҵķ�������
            Vector3 hitDir = playerManager.transform.position - transform.position;
            hitDir.y = 0;
            hitDir = hitDir.normalized;

            player.GetComponent<PlayerManager>().TakeDamage(hitDir, damageType);
        }

        return true;
    }

    public void RandomMoveToTestPoint()
    {
        // �����в��Ե�Ž�һ������
        Transform[] points = new Transform[]
        {
        testPoint1, testPoint2, testPoint3, testPoint4,
        testPoint5, testPoint6, testPoint7, testPoint8
        };

        // ���ѡ��һ������
        int randomIndex = Random.Range(0, points.Length);

        // ���������Ƿ����
        if (points[randomIndex] != null)
        {
            // ֱ�Ӱѵ�ǰλ�����õ�������λ��
            transform.position = points[randomIndex].position;

            Debug.Log("ѡ��ĵ�Ϊ:" + randomIndex);

            AttackDetecte(1);
        }
        else
        {
            Debug.LogWarning("���ѡ�еĲ��Ե�Ϊ�գ�");
        }
    }

}
