using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public string targetSceneName; 
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
        
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Nom de scène cible non assigné!");
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
    
    void ChangeScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log("Changement vers la scène: " + targetSceneName);
            
            // Vérifier si la scène existe dans les scènes du build
            if (SceneUtility.GetBuildIndexByScenePath("Scenes/" + targetSceneName) != -1)
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("La scène '" + targetSceneName + "' n'existe pas ou n'est pas incluse dans les scènes du build!");
            }
        }
        else
        {
            Debug.LogError("Changement de scène impossible: nom de scène non assigné");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}