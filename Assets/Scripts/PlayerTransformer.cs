using UnityEngine;

public class PlayerTransformer {

    private GameObject playerModel;
    private float playerSpeed = 0.01F;


    public PlayerTransformer(GameObject playerObject) {
        playerModel = playerObject;
    }

    public void Forward(float amount) {
        playerModel.transform.Translate(Vector3.forward * amount * playerSpeed);
    }

    public void Sideways(float amount) {
        playerModel.transform.Translate(Vector3.right * amount * playerSpeed);
    }

    public void RotateHorizontal(float amount) {
        playerModel.transform.Rotate(Vector3.up * amount);
    }

}
