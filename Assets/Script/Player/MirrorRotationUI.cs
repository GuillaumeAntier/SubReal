using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MirrorRotationUI : MonoBehaviour
{
    [Header("References")]
    public ObjectGrab objectGrabSystem;
    public MirrorRotation mirrorRotationSystem;
    
    [Header("UI Settings")]
    public Transform uiCanvas;
    public TextMeshProUGUI controlsText;
    public Image controlsBackground;
    public bool showOnlyWhenHoldingMirror = true;
    [Tooltip("Position de l'UI sur l'écran (0,0 = bas gauche, 1,1 = haut droit)")]
    public Vector2 screenPosition = new Vector2(0.5f, 0.5f);
    
    [Header("Visual Settings")]
    public Color textColor = new Color(0.9f, 0.9f, 0.9f);
    public Color backgroundColor = new Color(0, 0, 0, 0.6f);
    public Vector2 backgroundPadding = new Vector2(15, 10);
    
    private GameObject uiElement;
    private bool isInitialized = false;
    private RectTransform rectTransform;
    
    void Start()
    {
        if (objectGrabSystem == null)
            objectGrabSystem = GetComponent<ObjectGrab>();
        
        if (mirrorRotationSystem == null)
            mirrorRotationSystem = GetComponent<MirrorRotation>();
        
        if (controlsText == null)
            CreateUI();
        else
            UpdateControls();
        
        isInitialized = true;
    }
    
    void Update()
    {
        if (!isInitialized || uiElement == null) return;
        
        if (showOnlyWhenHoldingMirror)
        {
            GameObject heldObject = objectGrabSystem.GetHeldObject();
            bool holdingMirror = (heldObject != null && heldObject.GetComponent<LaserMirror>() != null);
            
            if (uiElement.activeSelf != holdingMirror)
                uiElement.SetActive(holdingMirror);
        }
    }
    
    void CreateUI()
    {
        Canvas canvas = null;
        if (uiCanvas != null)
        {
            canvas = uiCanvas.GetComponent<Canvas>();
        }
        
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
            
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("MirrorControlsCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }
        
        uiElement = new GameObject("MirrorControlsUI");
        uiElement.transform.SetParent(canvas.transform, false);
        rectTransform = uiElement.AddComponent<RectTransform>();
        
        // Positionnement basé sur screenPosition
        rectTransform.anchorMin = screenPosition;
        rectTransform.anchorMax = screenPosition;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(uiElement.transform, false);
        controlsBackground = bgObj.AddComponent<Image>();
        controlsBackground.color = backgroundColor;
        
        RectTransform bgRect = controlsBackground.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = Vector2.zero;
        
        GameObject textObj = new GameObject("ControlsText");
        textObj.transform.SetParent(uiElement.transform, false);
        controlsText = textObj.AddComponent<TextMeshProUGUI>();
        
        // Tentative de chargement de la police avec gestion d'erreur
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (fontAsset == null)
        {
            Debug.LogWarning("Police LiberationSans SDF non trouvée. Utilisation de la police par défaut.");
            fontAsset = TMP_Settings.defaultFontAsset;
        }
        controlsText.font = fontAsset;
        
        controlsText.fontSize = 16;
        controlsText.color = textColor;
        controlsText.alignment = TextAlignmentOptions.Center; 
        controlsText.textWrappingMode = TextWrappingModes.NoWrap;
        
        RectTransform textRect = controlsText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(-backgroundPadding.x * 2, -backgroundPadding.y * 2);
        
        UpdateControls();
        
        AdjustSize();
        
        uiElement.SetActive(!showOnlyWhenHoldingMirror);
    }
    
    void UpdateControls()
    {
        if (controlsText == null || mirrorRotationSystem == null) return;
        
        string controlsString = "<b>ROTATION DU MIROIR</b>\n\n";
        controlsString += $"<color=#f8e473>Maintenir {GetKeyName(mirrorRotationSystem.mouseRotationKey)} + Mouvement souris</color>";
        
        controlsText.text = controlsString;
        
        if (isInitialized)
            AdjustSize();
    }
    
    void AdjustSize()
    {
        if (controlsText == null || rectTransform == null) return;
        
        controlsText.ForceMeshUpdate();
        Vector2 textSize = controlsText.GetRenderedValues(false);
        
        rectTransform.sizeDelta = new Vector2(
            textSize.x + backgroundPadding.x * 2,
            textSize.y + backgroundPadding.y * 2
        );
    }
    
    string GetKeyName(KeyCode key)
    {
        return $"<color=#ffffff><b>{key}</b></color>";
    }
    
    public void SetPosition(Vector2 newPosition)
    {
        screenPosition = newPosition;
        
        if (rectTransform != null)
        {
            rectTransform.anchorMin = screenPosition;
            rectTransform.anchorMax = screenPosition;
        }
    }
}