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
    
    [Header("Visual Settings")]
    public Color textColor = new Color(0.9f, 0.9f, 0.9f);
    public Color backgroundColor = new Color(0, 0, 0, 0.6f);
    public Vector2 backgroundPadding = new Vector2(15, 10);
    
    private GameObject uiElement;
    private bool isInitialized = false;
    
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
        if (!isInitialized) return;
        
        if (showOnlyWhenHoldingMirror && uiElement != null)
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
            canvas = FindObjectOfType<Canvas>();
            
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
        RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
        
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
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
        controlsText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"); 
        controlsText.fontSize = 16;
        controlsText.color = textColor;
        controlsText.alignment = TextAlignmentOptions.Center; 
        controlsText.enableWordWrapping = false;
        
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
        if (controlsText == null) return;
        
        string controlsString = "<b>ROTATION DU MIROIR</b>\n\n";
        controlsString += $"<color=#f8e473>Gauche:</color> {GetKeyName(mirrorRotationSystem.rotateLeftKey)}\n";
        controlsString += $"<color=#f8e473>Droite:</color> {GetKeyName(mirrorRotationSystem.rotateRightKey)}\n";
        controlsString += $"<color=#f8e473>Haut:</color> {GetKeyName(mirrorRotationSystem.rotateUpKey)}\n";
        controlsString += $"<color=#f8e473>Bas:</color> {GetKeyName(mirrorRotationSystem.rotateDownKey)}";
        
        controlsText.text = controlsString;
        
        if (isInitialized)
            AdjustSize();
    }
    
    void AdjustSize()
    {
        if (controlsText == null) return;
        
        controlsText.ForceMeshUpdate();
        Vector2 textSize = controlsText.GetRenderedValues(false);
        
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(
            textSize.x + backgroundPadding.x * 2,
            textSize.y + backgroundPadding.y * 2
        );
    }
    
    string GetKeyName(KeyCode key)
    {
        return $"<color=#ffffff><b>{key.ToString()}</b></color>";
    }
    
    public void UpdateKeys(KeyCode left, KeyCode right, KeyCode up, KeyCode down)
    {
        if (mirrorRotationSystem != null)
        {
            mirrorRotationSystem.rotateLeftKey = left;
            mirrorRotationSystem.rotateRightKey = right;
            mirrorRotationSystem.rotateUpKey = up;
            mirrorRotationSystem.rotateDownKey = down;
        }
        
        UpdateControls();
    }
}