using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public float pressAmount = 0.05f;   
    public float pressSpeed = 5f;        
    public Door door;

    private Vector3 startPos;
    private Vector3 targetPos; 

    private bool isPressed = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    void Update()
    {
        if (isPressed)
        {
            targetPos = startPos - new Vector3(0, pressAmount, 0);
        }
        else
        {
            targetPos = startPos;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.rigidbody)
        {
            isPressed = true;
            door.OpenDoor(); 
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.rigidbody)
        {
            isPressed = false;
            door.CloseDoor();
        }
    }
}
