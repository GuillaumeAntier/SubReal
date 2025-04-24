using UnityEngine;

public class Door : MonoBehaviour
{
    public float openDistance = 3f;      
    public float openSpeed = 3f;        
    private Vector3 closedPos;           
    private Vector3 openPos;             

    private bool isOpen = false;         

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + new Vector3(0, openDistance, 0); 
    }

    void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * openSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, closedPos, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        isOpen = true; 
    }

    public void CloseDoor()
    {
        isOpen = false; 
    }
}
