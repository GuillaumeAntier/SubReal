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

    [Header("Audio")]
    [SerializeField] private AudioSource backgroundAudioSource; 
    [SerializeField] private AudioSource transitionAudioSource; 
    [SerializeField] private AudioClip sceneTransitionSound;
    [SerializeField] private float fadeOutDuration = 1f;
    
    public string objectToHideName;
    private GameObject objectToHide;
    private static SceneTransition instance;
    private float originalVolume;
    
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
            
            if (backgroundAudioSource != null)
            {
                originalVolume = backgroundAudioSource.volume;
            }
            
            if (transitionAudioSource == null)
            {
                transitionAudioSource = gameObject.AddComponent<AudioSource>();
            }
            
            transitionAudioSource.playOnAwake = false;
            transitionAudioSource.loop = false;
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

        if (transitionAudioSource != null && sceneTransitionSound != null)
        {
            transitionAudioSource.Stop(); 
            transitionAudioSource.clip = sceneTransitionSound;
            transitionAudioSource.volume = 1f; 
            transitionAudioSource.Play();
        }
        
        yield return StartCoroutine(FadeIn());
        
        if (sceneNameText != null)
        {
            sceneNameText.text = sceneName;
            yield return StartCoroutine(AnimateSceneName());
        }
        
        SceneManager.LoadScene(sceneName);
        
        yield return null;
        
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
            
            if (backgroundAudioSource != null && backgroundAudioSource.isPlaying)
            {
                float fadeProgress = Mathf.Clamp01(elapsedTime / fadeOutDuration);
                backgroundAudioSource.volume = Mathf.Lerp(originalVolume, 0, fadeProgress);
            }
            
            yield return null;
        }
        
        color.a = 1;
        transitionImage.color = color;
        
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = 0;
        }
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
            
            if (backgroundAudioSource != null)
            {
                backgroundAudioSource.volume = Mathf.Lerp(0, originalVolume, t);
            }
            
            yield return null;
        }
        
        color.a = 0;
        transitionImage.color = color;
        
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = originalVolume;
        }
        
        if (transitionAudioSource != null)
        {
            transitionAudioSource.Stop();
        }
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