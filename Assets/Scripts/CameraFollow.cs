using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.1f;
    private float targetY;

    void Start() => targetY = transform.position.y;

    void Update()
    {
        Vector3 desiredPosition = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    public void MoveUp(float height) => targetY += height;
}

