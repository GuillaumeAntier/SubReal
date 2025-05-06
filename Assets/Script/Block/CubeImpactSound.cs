using UnityEngine;

public class CubeImpactSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource impactAudioSource;
    [SerializeField] private AudioClip impactSound;
    
    [Header("Impact Settings")]
    [SerializeField] private float minImpactForce = 0.1f;  
    [SerializeField] private float maxImpactForce = 5f;   
    [SerializeField] private float cooldownTime = 0.1f;   
    
    private float lastImpactTime;
    
    private void Start()
    {
        if (impactAudioSource == null)
        {
            impactAudioSource = GetComponent<AudioSource>();
            if (impactAudioSource == null)
            {
                impactAudioSource = gameObject.AddComponent<AudioSource>();
                impactAudioSource.playOnAwake = false;
                impactAudioSource.spatialBlend = 1.0f; // Son 3D
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastImpactTime < cooldownTime)
            return;
            
        float impactForce = collision.relativeVelocity.magnitude;
        
        if (impactForce >= minImpactForce && impactSound != null && impactAudioSource != null)
        {
            float volume = Mathf.Clamp01(impactForce / maxImpactForce);
            
            impactAudioSource.pitch = Random.Range(0.9f, 1.1f); 
            impactAudioSource.volume = volume;
            impactAudioSource.clip = impactSound;
            impactAudioSource.Play();
            
            lastImpactTime = Time.time;
        }
    }
}