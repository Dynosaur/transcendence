using UnityEngine;

public class PlayerTransformer : MonoBehaviour {

    private float playerSpeed = 0.01F;

    public void Update() {
        Forward(Input.GetAxis("Vertical"));
        Sideways(Input.GetAxis("Horizontal"));
        RotateHorizontal(Input.GetAxis("Mouse X"));
    }

    private void Forward(float amount) {
        transform.Translate(Vector3.forward * amount * playerSpeed);
    }

    private void Sideways(float amount) {
        transform.Translate(Vector3.right * amount * playerSpeed);
    }

    private void RotateHorizontal(float amount) {
        transform.Rotate(Vector3.up * amount);
    }
}
