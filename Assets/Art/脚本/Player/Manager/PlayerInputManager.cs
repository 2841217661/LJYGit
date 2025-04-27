using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    public Vector2 movementInput; //ˮƽ��������

    private float horizontalInput; //ˮƽ����
    private float verticalInput; //��ֱ����

    private float horizontalRawInput;
    private float verticalRawInput;

    public Vector2 movementRawInput;

    public float moveAmount; //�����С
    public float moveRawAmount; //�����С,�޹���

    public float cameraHorizontalInput;
    public float cameraVerticalInput;

    public bool runInput;


    public void GetAllInput()
    {
        GetMoveInput();
        ChangeRunInput();
    }

    //�ƶ�����
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

    //���������л�
    private void ChangeRunInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            runInput = !runInput;
        }
    }

    //��������
    public bool DodgeInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    //��Ծ����
    public bool JumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    //���������
    public bool Mouse0DownInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    //��ס������
    public bool Mouse0Input()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }


    //̧��������
    public bool Mouse0UpInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    //�������Ҽ�
    public bool Mouse1DownInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    //��ס����Ҽ�
    public bool Mouse1Input()
    {
        return Input.GetKey(KeyCode.Mouse1);
    }

    //̧������Ҽ�
    public bool Mouse1UpInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse1);
    }

    //��C
    public bool CDownInput()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    //��E
    public bool EDownInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
