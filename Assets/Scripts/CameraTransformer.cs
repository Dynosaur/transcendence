using UnityEngine;

public class CameraTransformer {

    private GameObject camera;
    private Vector3 cameraOffset;

    private int maxCameraDepression = 80;
    private int maxCameraElevation = 280;

    public CameraTransformer(GameObject cameraObject) {
        camera = cameraObject;
        cameraOffset = new Vector3(0, 3, 0);
    }

    public void RotateVertical(float amount) {
        var x = camera.transform.rotation.eulerAngles.x;
        if (x > 0 && x < 180 && amount < 0 && x - amount > maxCameraDepression)
            return;
        if (x > 180 && x < 360 && amount > 0 && x - amount < maxCameraElevation)
            return;
        Quaternion q = camera.transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x - amount, q.eulerAngles.y, 0);
        camera.transform.rotation = q;
    }

}
