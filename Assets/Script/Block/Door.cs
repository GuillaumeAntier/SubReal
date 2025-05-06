using UnityEngine;

public class Door : MonoBehaviour
{
    public float openDistance = 2.4f;      
    public float openSpeed = 2f;        
    private Vector3 closedPos;           
    private Vector3 openPos;             

    private bool isOpen = false;
    private bool wasOpen = false;  

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + new Vector3(0, openDistance, 0);
        
        if (doorAudioSource == null)
        {
            doorAudioSource = GetComponent<AudioSource>();
            if (doorAudioSource == null)
            {
                doorAudioSource = gameObject.AddComponent<AudioSource>();
                doorAudioSource.playOnAwake = false;
                doorAudioSource.spatialBlend = 1.0f; 
            }
        }
    }

    void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * openSpeed);
            
            if (!wasOpen)
            {
                PlayDoorSound(true);
                wasOpen = true;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, closedPos, Time.deltaTime * openSpeed);
            
            if (wasOpen)
            {
                PlayDoorSound(false);
                wasOpen = false;
            }
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
    
    private void PlayDoorSound(bool opening)
    {
        if (doorAudioSource != null)
        {
            if (opening && doorOpenSound != null)
            {
                doorAudioSource.clip = doorOpenSound;
                doorAudioSource.Play();
            }
            else if (!opening && doorCloseSound != null)
            {
                doorAudioSource.clip = doorCloseSound;
                doorAudioSource.Play();
            }
        }
    }
}
