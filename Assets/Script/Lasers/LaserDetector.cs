using UnityEngine;
using UnityEngine.Events;

public class LaserDetector : MonoBehaviour
{
    [Header("Detector Settings")]
    public Material activeMaterial;    
    public Material inactiveMaterial;   
    
    [Header("Audio")]
    [SerializeField] private AudioSource detectorAudioSource;
    [SerializeField] private AudioClip detectorActivateSound;
    
    [Header("Events")]
    public UnityEvent onLaserHit;              
    public UnityEvent onLaserExit;             
    
    private Renderer rend;
    private bool isHit = false;
    private bool soundPlayed = false;
    private float checkInterval = 0.1f;        
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && inactiveMaterial != null)
        {
            rend.material = inactiveMaterial;
        }
        
        if (detectorAudioSource == null)
        {
            detectorAudioSource = GetComponent<AudioSource>();
            if (detectorAudioSource == null)
            {
                detectorAudioSource = gameObject.AddComponent<AudioSource>();
                detectorAudioSource.playOnAwake = false;
                detectorAudioSource.spatialBlend = 1.0f; // Son 3D
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            if (isHit)
            {
                isHit = false;  
                Invoke("CheckLaserExit", checkInterval * 0.9f);
            }
        }
    }

    void CheckLaserExit()
    {
        if (!isHit)
        {
            if (rend != null && inactiveMaterial != null)
            {
                rend.material = inactiveMaterial;
            }
            soundPlayed = false; 
            onLaserExit.Invoke();
        }
    }

    public void RegisterLaserHit()
    {
        if (!isHit)
        {
            isHit = true;
            if (rend != null && activeMaterial != null)
            {
                rend.material = activeMaterial;
            }
            
            if (!soundPlayed)
            {
                PlayActivationSound();
                soundPlayed = true;
            }
            
            onLaserHit.Invoke();
        }
        else
        {
            isHit = true; 
        }
    }
    
    private void PlayActivationSound()
    {
        if (detectorAudioSource != null && detectorActivateSound != null)
        {
            detectorAudioSource.clip = detectorActivateSound;
            detectorAudioSource.Play();
        }
    }
}