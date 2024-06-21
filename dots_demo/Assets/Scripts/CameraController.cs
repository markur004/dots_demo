using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensY = 4000f;
    public float sensX = 4000f;

    public Transform orientation;


    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //TODO INPUT SYSTEM
    private void Update()
    {
        var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        _yRotation += mouseX;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

     
        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
}
