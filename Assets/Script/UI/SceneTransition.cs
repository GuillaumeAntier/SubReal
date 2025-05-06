using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneTransition : MonoBehaviour
{
    [Header("Paramètres de transition")]
    [SerializeField] private float transitionDuration = 1.0f;
    [SerializeField] private Image transitionImage;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Animation du nom de scène")]
    [SerializeField] private TextMeshProUGUI sceneNameText;
    [SerializeField] private float sceneNameFadeDuration = 0.5f;
    [SerializeField] private float sceneNameDisplayDuration = 2.0f;
    [SerializeField] private Vector2 sceneNameStartPosition = new Vector2(0, -50);
    [SerializeField] private Vector2 sceneNameEndPosition = new Vector2(0, 50);
    [SerializeField] private AnimationCurve sceneNameMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    public string objectToHideName;
    private GameObject objectToHide;
    private static SceneTransition instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (transitionImage != null)
            {
                Color color = transitionImage.color;
                color.a = 0;
                transitionImage.color = color;
            }
            
            if (sceneNameText != null)
            {
                Color color = sceneNameText.color;
                color.a = 0;
                sceneNameText.color = color;
                sceneNameText.rectTransform.anchoredPosition = sceneNameStartPosition;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void FadeToScene(string sceneName)
    {
        if (instance == null)
        {
            Debug.LogError("SceneTransition n'est pas initialisé! Assurez-vous d'avoir un GameObject avec le composant SceneTransition dans votre scène.");
            SceneManager.LoadScene(sceneName);
            return;
        }
        
        instance.StartCoroutine(instance.FadeAndLoadScene(sceneName));
    }
    
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
            Debug.Log("Object hidden");
        }
        
        yield return StartCoroutine(FadeIn());
        
        if (sceneNameText != null)
        {
            sceneNameText.text = sceneName;
            yield return StartCoroutine(AnimateSceneName());
        }
        
        SceneManager.LoadScene(sceneName);
        
        yield return null; // Attendre une frame pour que la scène soit chargée
        
        // Trouver l'objet à masquer dans la nouvelle scène
        if (!string.IsNullOrEmpty(objectToHideName))
        {
            objectToHide = GameObject.Find(objectToHideName);
            if (objectToHide == null)
            {
                Debug.LogWarning("Impossible de trouver l'objet à masquer '" + objectToHideName + "' dans la nouvelle scène.");
            }
        }
        
        yield return StartCoroutine(FadeOut());
        
        if (objectToHide != null)
        {
            objectToHide.SetActive(true);
            Debug.Log("Object shown");
        }
    }
    
    private IEnumerator AnimateSceneName()
    {
        if (sceneNameText == null) yield break;
        
        sceneNameText.rectTransform.anchoredPosition = sceneNameStartPosition;
        Color textColor = sceneNameText.color;
        textColor.a = 0;
        sceneNameText.color = textColor;
        
        float elapsedTime = 0;
        while (elapsedTime < sceneNameFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / sceneNameFadeDuration);
            
            textColor.a = t;
            sceneNameText.color = textColor;
            
            Vector2 newPos = Vector2.Lerp(sceneNameStartPosition, Vector2.zero, sceneNameMoveCurve.Evaluate(t));
            sceneNameText.rectTransform.anchoredPosition = newPos;
            
            yield return null;
        }
        
        yield return new WaitForSeconds(sceneNameDisplayDuration);
        
        elapsedTime = 0;
        while (elapsedTime < sceneNameFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / sceneNameFadeDuration);
            
            textColor.a = 1 - t;
            sceneNameText.color = textColor;
            
            Vector2 newPos = Vector2.Lerp(Vector2.zero, sceneNameEndPosition, sceneNameMoveCurve.Evaluate(t));
            sceneNameText.rectTransform.anchoredPosition = newPos;
            
            yield return null;
        }
        
        textColor.a = 0;
        sceneNameText.color = textColor;
    }
    
    private IEnumerator FadeIn()
    {
        if (transitionImage == null) yield break;
        
        float elapsedTime = 0;
        Color color = transitionImage.color;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);
            
            color.a = t;
            transitionImage.color = color;
            
            yield return null;
        }
        
        color.a = 1;
        transitionImage.color = color;
    }
    
    private IEnumerator FadeOut()
    {
        if (transitionImage == null) yield break;
        
        float elapsedTime = 0;
        Color color = transitionImage.color;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);
            
            color.a = 1 - t;
            transitionImage.color = color;
            
            yield return null;
        }
        
        color.a = 0;
        transitionImage.color = color;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(objectToHideName))
        {
            objectToHide = GameObject.Find(objectToHideName);
        }
    }
}