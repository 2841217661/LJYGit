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
        // 初始化鼠标状态
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        // 检查是否按下鼠标中键
        if (Input.GetMouseButtonDown(2)) // 2 是鼠标中键
        {
            ToggleCursorLock();
        }
    }



    void ToggleCursorLock()
    {
        if (isCursorLocked)
        {
            // 解锁鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
        else
        {
            // 锁定鼠标并隐藏
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }
    }
}
