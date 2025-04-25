using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI References")]
    public Image crosshair;
    public TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private bool canInteract = false;
    private GameObject currentInteractable;

    void Start()
    {
        playerCamera = Camera.main;
        
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            canInteract = true;
            currentInteractable = hit.collider.gameObject;
            
            if (crosshair != null)
                crosshair.color = Color.green;
                
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
                interactionText.text = "Press E to grab";
            }
        }
        else
        {
            canInteract = false;
            currentInteractable = null;
            
            if (crosshair != null)
                crosshair.color = Color.white;
                
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}