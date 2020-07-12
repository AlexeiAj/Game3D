using UnityEngine;

public class Keys {
    public float x { get; set; }
    public float y { get; set; }
    public float mouseX { get; set; }
    public float mouseY { get; set; }
    public bool jumping { get; set; }
    public bool mouseLeft { get; set; }
    public bool mouseRight { get; set; }
    public bool shift { get; set; }
    public bool e { get; set; }

    public Joystick joystickLeft;
    public Joystick joystickRight;

    public Keys() {
        if (MenuController.instance.isAndroid()) {
            this.joystickLeft = MenuController.instance.getJoystickLeft();
            this.joystickRight = MenuController.instance.getJoystickRight();
        }
    }

    public void updateKeys(){
        if (MenuController.instance.isAndroid()) {
            x = joystickLeft.Horizontal;
            y = joystickLeft.Vertical;
            mouseX = joystickRight.Horizontal;
            mouseY = joystickRight.Vertical;
            mouseLeft = MenuController.instance.getFire();
            jumping = MenuController.instance.getJump();
        } else {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            mouseLeft = Input.GetMouseButton(0);
            mouseRight = Input.GetMouseButton(1);
            jumping = Input.GetButton("Jump");
            shift = Input.GetKey(KeyCode.LeftShift);
            e = Input.GetKey(KeyCode.E);
        }
    }
}
