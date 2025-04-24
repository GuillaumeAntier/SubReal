using UnityEngine;

public class PositionTeleportButton : MonoBehaviour
{
    public Transform teleportTarget; 
    public Transform playerTransform; 
    public float interactionDistance = 2f; 
    public KeyCode interactionKey = KeyCode.E; 
    
    public Material normalMaterial; 
    public Material highlightMaterial; 
    
    private Camera playerCamera;
    private Renderer buttonRenderer;
    private bool playerInRange = false;
    
    void Start()
    {
        playerCamera = Camera.main; 
        buttonRenderer = GetComponent<Renderer>();
        
        if (teleportTarget == null)
        {
            Debug.LogError("Cible de téléportation non assignée!");
        }
        
        if (playerTransform == null)
        {
            Debug.LogWarning("Transform du joueur non assigné. Recherche automatique du joueur...");
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log("Joueur trouvé automatiquement.");
            }
            else
            {
                Debug.LogError("Impossible de trouver le joueur. Veuillez assigner playerTransform manuellement.");
            }
        }
    }
    
    void Update()
    {
        CheckPlayerLooking();
        
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            TeleportPlayer();
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
    
    void TeleportPlayer()
    {
        if (teleportTarget != null && playerTransform != null)
        {
            Debug.Log("Téléportation du joueur vers: " + teleportTarget.position);
            
            playerTransform.position = teleportTarget.position;
            
            playerTransform.rotation = teleportTarget.rotation;
        }
        else
        {
            Debug.LogError("Téléportation impossible: cible ou joueur non assigné");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        if (teleportTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(teleportTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position, teleportTarget.position);
        }
    }
}