using UnityEngine;

public class CameraTransformer : MonoBehaviour {

    private Vector3 cameraOffset;
    private GameObject player;

    private int maxCameraDepression = -45;
    private int maxCameraElevation = 45;

    public void Start() {
        cameraOffset = new Vector3(0, 3, 0);
        player = GameObject.Find("Player");
    }

    public void Update() {
        transform.position = player.transform.position + cameraOffset;
        Debug.Log(transform.rotation.z);
        RotateVertical(Input.GetAxis("Mouse Y"));
        RotateHorizontal(Input.GetAxis("Mouse X"));
    }

    private void RotateVertical(float amount) {
        if (amount > 0 && -transform.rotation.x * 180 >= maxCameraElevation) {
            return;
        }
        if (amount < 0 && -transform.rotation.x * 180 <= maxCameraDepression) {
            return;
        }
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x - amount, q.eulerAngles.y, 0);
        transform.rotation = q;
    }

    private void RotateHorizontal(float amount) {
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y + amount, 0);
        transform.rotation = q;
    }

}
