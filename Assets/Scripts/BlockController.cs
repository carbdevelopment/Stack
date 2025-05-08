using UnityEngine;

public class BlockController : MonoBehaviour
{
    private float moveSpeed;
    private int direction = 1;
    private bool isMoving = true;
    private bool isFallingPiece = false;
    private float leftLimit = -2.5f;
    private float rightLimit = 2.5f;

    void Start() => moveSpeed = GameManager.Instance.moveSpeed;

    void Update()
    {
        if (isMoving && !isFallingPiece)
            Move();
    }

    void Move()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;
        
        if (transform.position.x >= rightLimit)
            direction = -1;
        else if (transform.position.x <= leftLimit)
            direction = 1;
    }

    public void SetDirection(int newDirection) => direction = newDirection;

    public void Stop() => isMoving = false;
    
    public void SetAsFallingPiece()
    {
        isFallingPiece = true;
        isMoving = false;
    }
}