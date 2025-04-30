using UnityEngine;

public class LaserMirror : MonoBehaviour 
{
    [Header("Mirror Settings")]
    public bool isReflective = true;
    public Material reflectiveMaterial;
    
    [Header("Debug Visualization")]
    public bool visualizeReflection = false;
    public float debugLineLength = 2f;
    public Color debugColor = Color.cyan;
    
    private Renderer rend;
    private Vector3 lastIncomingDirection;
    private Vector3 lastReflectedDirection;
    private Vector3 lastHitPoint;
    private bool wasHitThisFrame = false;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (reflectiveMaterial != null && isReflective)
        {
            rend.material = reflectiveMaterial;
        }
    }
    
    void LateUpdate()
    {
        wasHitThisFrame = false;
    }
    
    void OnDrawGizmos()
    {
        if (visualizeReflection && Application.isPlaying && wasHitThisFrame)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(lastHitPoint, -lastIncomingDirection.normalized * debugLineLength);
            
            Gizmos.color = debugColor;
            Gizmos.DrawRay(lastHitPoint, lastReflectedDirection.normalized * debugLineLength);
        }
    }
    
    public Vector3 ReflectLaser(Vector3 incomingDirection, Vector3 surfaceNormal)
    {
        if (!isReflective)
        {
            return Vector3.zero;
        }
        
        lastIncomingDirection = incomingDirection;
        lastReflectedDirection = Vector3.Reflect(incomingDirection, surfaceNormal);
        lastHitPoint = transform.position;
        wasHitThisFrame = true;
        
        return lastReflectedDirection;
    }
    
}