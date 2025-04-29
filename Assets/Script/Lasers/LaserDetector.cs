using UnityEngine;
using UnityEngine.Events;

public class LaserDetector : MonoBehaviour
{
    [Header("Detector Settings")]
    public Color activeColor = Color.green;    
    public Color inactiveColor = Color.red;   
    
    [Header("Events")]
    public UnityEvent onLaserHit;              
    public UnityEvent onLaserExit;             
    
    private Renderer rend;
    private bool isHit = false;
    private float checkInterval = 0.1f;        
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = inactiveColor;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            if (isHit)
            {
                isHit = false;  
                Invoke("CheckLaserExit", checkInterval * 0.9f);
            }
        }
    }

    void CheckLaserExit()
    {
        if (!isHit)
        {
            if (rend != null)
            {
                rend.material.color = inactiveColor;
            }
            onLaserExit.Invoke();
        }
    }

    public void RegisterLaserHit()
    {
        if (!isHit)
        {
            isHit = true;
            if (rend != null)
            {
                rend.material.color = activeColor;
            }
            onLaserHit.Invoke();
        }
        else
        {
            isHit = true; 
        }
    }
}