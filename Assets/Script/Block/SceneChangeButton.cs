using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public string targetSceneName; 
    public float interactionDistance = 2f; 
    public KeyCode interactionKey = KeyCode.E; 
    
    public Material normalMaterial; 
    public Material highlightMaterial; 
    
    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonPressSound;
    
    private Camera playerCamera;
    private Renderer buttonRenderer;
    private bool playerInRange = false;
    
    void Start()
    {
        playerCamera = Camera.main; 
        buttonRenderer = GetComponent<Renderer>();
        
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Nom de scène cible non assigné!");
        }
        
        if (buttonAudioSource == null)
        {
            buttonAudioSource = GetComponent<AudioSource>();
            if (buttonAudioSource == null)
            {
                buttonAudioSource = gameObject.AddComponent<AudioSource>();
                buttonAudioSource.playOnAwake = false;
                buttonAudioSource.spatialBlend = 1.0f; // Son 3D
            }
        }
    }
    
    void Update()
    {
        CheckPlayerLooking();
        
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            ChangeScene();
        }
    }
    
    void CheckPlayerLooking()
    {
        if (playerCamera == null) return;
        
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.transform == transform)
            {
                playerInRange = true;
                if (buttonRenderer != null && highlightMaterial != null)
                {
                    buttonRenderer.material = highlightMaterial;
                }
                
            }
            else
            {
                ResetButton();
            }
        }
        else
        {
            ResetButton();
        }
    }
    
    void ResetButton()
    {
        playerInRange = false;
        if (buttonRenderer != null && normalMaterial != null)
        {
            buttonRenderer.material = normalMaterial;
        }
    }
    
    public void ChangeScene()
    {
        PlayButtonSound();
        
        SceneTransition.FadeToScene(targetSceneName);
    }
    
    private void PlayButtonSound()
    {
        if (buttonAudioSource != null && buttonPressSound != null)
        {
            buttonAudioSource.clip = buttonPressSound;
            buttonAudioSource.Play();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}