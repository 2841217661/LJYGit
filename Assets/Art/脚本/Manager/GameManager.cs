using Cinemachine;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private bool isCursorLocked = false;

    public bool useBigEffect;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // ��ʼ�����״̬
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ƿ�������м�
        if (Input.GetMouseButtonDown(2)) // 2 ������м�
        {
            ToggleCursorLock();
        }
    }



    void ToggleCursorLock()
    {
        if (isCursorLocked)
        {
            // �������
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
        else
        {
            // ������겢����
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }
    }
}
