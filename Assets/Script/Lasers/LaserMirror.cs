using UnityEngine;

public class LaserMirror : MonoBehaviour
{ 
    public bool isReflective = true;    
    public Material reflectiveMaterial;  
    
    private Renderer rend;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (reflectiveMaterial != null && isReflective)
        {
            rend.material = reflectiveMaterial;
        }
    }
    
    public Vector3 ReflectLaser(Vector3 incomingDirection, Vector3 surfaceNormal)
    {
        if (!isReflective)
        {
            return Vector3.zero; 
        }
        return Vector3.Reflect(incomingDirection, surfaceNormal);
    }
}