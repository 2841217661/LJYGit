using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    public Vector2 movementInput; //水平输入向量

    private float horizontalInput; //水平输入
    private float verticalInput; //垂直输入

    private float horizontalRawInput;
    private float verticalRawInput;

    public Vector2 movementRawInput;

    public float moveAmount; //输入大小
    public float moveRawAmount; //输入大小,无过渡

    public float cameraHorizontalInput;
    public float cameraVerticalInput;

    public bool runInput;


    public void GetAllInput()
    {
        GetMoveInput();
        ChangeRunInput();
    }

    //移动输入
    private void GetMoveInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        horizontalRawInput = Input.GetAxisRaw("Horizontal");
        verticalRawInput = Input.GetAxisRaw("Vertical");

        movementRawInput = new Vector2(horizontalRawInput, verticalRawInput);

        movementInput = new Vector2(horizontalInput, verticalInput);
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        moveRawAmount = Mathf.Clamp01(Mathf.Abs(horizontalRawInput) + Mathf.Abs(verticalRawInput));
    }

    //奔跑输入切换
    private void ChangeRunInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            runInput = !runInput;
        }
    }

    //闪避输入
    public bool DodgeInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    //跳跃输入
    public bool JumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    //点击鼠标左键
    public bool Mouse0DownInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    //按住鼠标左键
    public bool Mouse0Input()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }


    //抬起鼠标左键
    public bool Mouse0UpInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    //点击鼠标右键
    public bool Mouse1DownInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    //按住鼠标右键
    public bool Mouse1Input()
    {
        return Input.GetKey(KeyCode.Mouse1);
    }

    //抬起鼠标右键
    public bool Mouse1UpInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse1);
    }

    //按C
    public bool CDownInput()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    //按E
    public bool EDownInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
