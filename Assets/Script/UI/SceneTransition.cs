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
        Debug.Log("[SceneTransition] Awake called");
        if (instance == null)
        {
            Debug.Log("[SceneTransition] Creating new instance");
            instance = this;
            DontDestroyOnLoad(gameObject);

            SetupReferences();

            if (backgroundAudioSource != null)
            {
                Debug.Log($"[SceneTransition] Setting original volume: {backgroundAudioSource.volume}");
                originalVolume = backgroundAudioSource.volume;
            }
            else
            {
                Debug.LogWarning("[SceneTransition] Background audio source is null");
            }

            if (transitionAudioSource == null)
            {
                Debug.Log("[SceneTransition] Creating transition audio source");
                transitionAudioSource = gameObject.AddComponent<AudioSource>();
            }

            transitionAudioSource.playOnAwake = false;
            transitionAudioSource.loop = false;
        }
        else
        {
            Debug.Log("[SceneTransition] Instance already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }

    private void SetupReferences()
    {
        Debug.Log("[SceneTransition] Setting up references");
        
        if (transitionImage != null)
        {
            Debug.Log("[SceneTransition] Setting up transition image");
            Color color = transitionImage.color;
            color.a = 0;
            transitionImage.color = color;
        }
        else
        {
            Debug.LogWarning("[SceneTransition] Transition image is null");
        }

        if (sceneNameText != null)
        {
            Debug.Log("[SceneTransition] Setting up scene name text");
            Color color = sceneNameText.color;
            color.a = 0;
            sceneNameText.color = color;
            sceneNameText.rectTransform.anchoredPosition = sceneNameStartPosition;
        }
        else
        {
            Debug.LogWarning("[SceneTransition] Scene name text is null");
        }
    }

    public static void FadeToScene(string sceneName)
    {
        Debug.Log($"[SceneTransition] FadeToScene called for scene: {sceneName}");
        if (instance == null)
        {
            Debug.LogError("[SceneTransition] Instance is null! Loading scene directly");
            SceneManager.LoadScene(sceneName);
            return;
        }

        instance.StartCoroutine(instance.FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        Debug.Log($"[SceneTransition] Starting scene transition to: {sceneName}");

        if (objectToHide != null)
        {
            Debug.Log("[SceneTransition] Hiding object: " + objectToHide.name);
            objectToHide.SetActive(false);
        }

        Debug.Log("[SceneTransition] Starting fade in");
        yield return StartCoroutine(FadeIn());

        if (sceneTransitionSound != null && transitionAudioSource != null)
        {
            Debug.Log("[SceneTransition] Playing transition sound");
            transitionAudioSource.Stop();
            transitionAudioSource.clip = sceneTransitionSound;
            transitionAudioSource.volume = 1f;
            transitionAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("[SceneTransition] Transition sound or audio source is null");
        }

        if (sceneNameText != null)
        {
            Debug.Log("[SceneTransition] Animating scene name");
            sceneNameText.text = sceneName;
            yield return StartCoroutine(AnimateSceneName());
        }
        else if (sceneTransitionSound != null)
        {
            Debug.Log("[SceneTransition] Waiting for transition sound");
            yield return new WaitForSeconds(sceneTransitionSound.length);
        }
        else
        {
            Debug.Log("[SceneTransition] No scene name text or transition sound, continuing immediately");
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log($"[SceneTransition] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);

        yield return null;

        if (!string.IsNullOrEmpty(objectToHideName))
        {
            Debug.Log($"[SceneTransition] Finding object to hide: {objectToHideName}");
            objectToHide = GameObject.Find(objectToHideName);
        }

        Debug.Log("[SceneTransition] Reassigning references");
        ReassignMissingReferences();

        Debug.Log("[SceneTransition] Starting fade out");
        yield return StartCoroutine(FadeOut());

        if (objectToHide != null)
        {
            Debug.Log("[SceneTransition] Showing hidden object");
            objectToHide.SetActive(true);
        }
    }

    private IEnumerator AnimateSceneName()
    {
        Debug.Log("[SceneTransition] Starting scene name animation");
        if (sceneNameText == null)
        {
            Debug.LogError("[SceneTransition] Scene name text is null");
            yield break;
        }

        sceneNameText.rectTransform.anchoredPosition = sceneNameStartPosition;
        Color textColor = sceneNameText.color;
        textColor.a = 0;
        sceneNameText.color = textColor;

        Debug.Log("[SceneTransition] Fading in scene name");
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

        Debug.Log("[SceneTransition] Displaying scene name");
        yield return new WaitForSeconds(sceneNameDisplayDuration);

        Debug.Log("[SceneTransition] Fading out scene name");
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
        Debug.Log("[SceneTransition] Scene name animation complete");
    }

    private IEnumerator FadeIn()
    {
        Debug.Log("[SceneTransition] Starting fade in");
        if (transitionImage == null)
        {
            Debug.LogError("[SceneTransition] Transition image is null");
            yield break;
        }

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
            else if (backgroundAudioSource == null)
            {
                Debug.Log("[SceneTransition] No background audio to fade");
            }

            yield return null;
        }

        color.a = 1;
        transitionImage.color = color;

        if (backgroundAudioSource != null)
        {
            Debug.Log("[SceneTransition] Muting background audio");
            backgroundAudioSource.volume = 0;
        }
    }

    private IEnumerator FadeOut()
    {
        Debug.Log("[SceneTransition] Starting fade out");
        if (transitionImage == null)
        {
            Debug.LogError("[SceneTransition] Transition image is null");
            yield break;
        }

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
            Debug.Log("[SceneTransition] Restored background audio volume");
            backgroundAudioSource.volume = originalVolume;
        }
        else
        {
            Debug.Log("[SceneTransition] No background audio to restore");
        }

        if (transitionAudioSource != null)
        {
            Debug.Log("[SceneTransition] Stopping transition audio");
            transitionAudioSource.Stop();
        }
    }

    private void OnEnable()
    {
        Debug.Log("[SceneTransition] OnEnable - Subscribing to scene loaded event");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Debug.Log("[SceneTransition] OnDisable - Unsubscribing from scene loaded event");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneTransition] Scene loaded: {scene.name}");
        if (!string.IsNullOrEmpty(objectToHideName))
        {
            Debug.Log($"[SceneTransition] Looking for object to hide: {objectToHideName}");
            objectToHide = GameObject.Find(objectToHideName);
        }

        ReassignMissingReferences();
    }

    private void ReassignMissingReferences()
    {
        Debug.Log("[SceneTransition] Reassigning missing references");
        
        if (transitionImage == null)
        {
            transitionImage = GameObject.Find("TransitionImage")?.GetComponent<Image>();
            Debug.Log(transitionImage != null 
                ? "[SceneTransition] Found TransitionImage" 
                : "[SceneTransition] TransitionImage not found");
        }

        if (sceneNameText == null)
        {
            sceneNameText = GameObject.Find("SceneNameText")?.GetComponent<TextMeshProUGUI>();
            Debug.Log(sceneNameText != null 
                ? "[SceneTransition] Found SceneNameText" 
                : "[SceneTransition] SceneNameText not found");
        }

        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = GameObject.Find("BackgroundMusic")?.GetComponent<AudioSource>();
            Debug.Log(backgroundAudioSource != null 
                ? "[SceneTransition] Found BackgroundMusic" 
                : "[SceneTransition] BackgroundMusic not found");
        }
        
        if (transitionAudioSource == null)
        {
            transitionAudioSource = GameObject.Find("TransitionAudio")?.GetComponent<AudioSource>();
            Debug.Log(transitionAudioSource != null 
                ? "[SceneTransition] Found TransitionAudio" 
                : "[SceneTransition] TransitionAudio not found");
            
            if (transitionAudioSource == null)
            {
                Debug.Log("[SceneTransition] Creating transition audio source");
                transitionAudioSource = gameObject.AddComponent<AudioSource>();
                transitionAudioSource.playOnAwake = false;
                transitionAudioSource.loop = false;
            }
        }
    }
}
