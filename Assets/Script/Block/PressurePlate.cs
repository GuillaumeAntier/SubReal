using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    public float pressAmount = 0.05f;
    public float pressSpeed = 5f;
    public Door door;
    public string plateColorName = "Bleu";
    public bool requireColorMatch = true;
    
    [Header("Audio")]
    [SerializeField] private AudioSource plateAudioSource;
    [SerializeField] private AudioClip plateActivateSound;
    
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPressed = false;
    private List<GameObject> validObjectsOnPlate = new List<GameObject>();

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
        
        if (plateAudioSource == null)
        {
            plateAudioSource = GetComponent<AudioSource>();
            if (plateAudioSource == null)
            {
                plateAudioSource = gameObject.AddComponent<AudioSource>();
                plateAudioSource.playOnAwake = false;
                plateAudioSource.spatialBlend = 1.0f; 
            }
        }
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
        GameObject collidingObject = collision.gameObject;
        
        if (validObjectsOnPlate.Contains(collidingObject))
            return;

        bool isValid = false;

        if (!requireColorMatch)
        {
            if (collidingObject.CompareTag("Player") || collision.rigidbody)
            {
                isValid = true;
            }
        }
        else
        {
            ColorIdentifier colorId = collidingObject.GetComponent<ColorIdentifier>();
            if (colorId != null && colorId.colorName == plateColorName)
            {
                isValid = true;
            }
        }

        if (isValid)
        {
            validObjectsOnPlate.Add(collidingObject);
            
            if (validObjectsOnPlate.Count == 1)
            {
                PlayActivationSound();
            }
            
            isPressed = true;
            door.OpenDoor();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject collidingObject = collision.gameObject;
        
        if (validObjectsOnPlate.Contains(collidingObject))
        {
            validObjectsOnPlate.Remove(collidingObject);
            
            if (validObjectsOnPlate.Count == 0)
            {
                isPressed = false;
                door.CloseDoor();
            }
        }
    }
    
    private void PlayActivationSound()
    {
        if (plateAudioSource != null && plateActivateSound != null)
        {
            plateAudioSource.clip = plateActivateSound;
            plateAudioSource.Play();
        }
    }
}