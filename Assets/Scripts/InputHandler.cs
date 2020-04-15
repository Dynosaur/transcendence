using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    private PlayerTransformer playerModel;
    private CameraTransformer cameraObject;

    private float mouseHzReact = 3;
    private float mouseVtReact = 3;

    public void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        playerModel = new PlayerTransformer(GameObject.Find("Player"));
        cameraObject = new CameraTransformer(GameObject.Find("Player Camera"));
    }

    public void Update() {
        if (Input.GetKey("escape")) {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        var mouseMovementX = Input.GetAxis("Mouse X") * mouseHzReact;
        var mouseMovementY = Input.GetAxis("Mouse Y") * mouseVtReact;
        var keyMovementX = Input.GetAxis("Vertical");
        var keyMovementY = Input.GetAxis("Horizontal");
        playerModel.RotateHorizontal(mouseMovementX);
        cameraObject.RotateVertical(mouseMovementY);
        playerModel.Forward(keyMovementX);
        playerModel.Sideways(keyMovementY);
    }

}
