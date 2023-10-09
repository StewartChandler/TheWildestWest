using UnityEngine;

public class RotationOscillator : MonoBehaviour
{
    private float rotationX;
    private Vector3 initialRotation;

    private void Start()
    {
        rotationX = Random.Range(5, 40);
        initialRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotationX, initialRotation.y, initialRotation.z);

    }
}
