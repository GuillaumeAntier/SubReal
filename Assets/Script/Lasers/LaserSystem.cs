using UnityEngine;

public class LaserSystem : MonoBehaviour
{
    [Header("Laser Settings")]
    public Transform laserOrigin;          
    public float maxDistance = 100f;       
    public Color laserColor = Color.red;   
    public float laserWidth = 0.1f;        
    
    [Header("Behavior")]
    public Vector3 initialDirection;       
    public Transform resetPoint;    
    public Transform playerTransform;       

    private LineRenderer lineRenderer;
    private Vector3 currentDirection;

    void Start()
    {
        initialDirection = initialDirection.normalized;
        currentDirection = initialDirection;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        lineRenderer.positionCount = 1;
        
        if (laserOrigin == null)
        {
            laserOrigin = transform;
        }
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
{
    Vector3 startPos = laserOrigin.position;
    Vector3 direction = currentDirection;

    lineRenderer.positionCount = 1; 
    lineRenderer.SetPosition(0, startPos);

    for (int i = 0; i < 10; i++) 
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos, direction, out hit, maxDistance))
        {
            lineRenderer.positionCount += 1; 
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

            LaserDetector detector = hit.collider.GetComponent<LaserDetector>();
            if (detector != null)
            {
                detector.RegisterLaserHit();
            }

            LaserMirror mirror = hit.collider.GetComponent<LaserMirror>();
            if (mirror != null && mirror.isReflective)
            {
                startPos = hit.point;  
                direction = mirror.ReflectLaser(direction, hit.normal);  
                continue; 
            }

            if (hit.collider.gameObject.name.Contains("Cube") || hit.collider.CompareTag("Player"))
            {
                if (hit.collider.CompareTag("Player") && resetPoint != null && playerTransform != null)
                {
                    playerTransform.position = resetPoint.position; 
                }
                break;  
            }
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, startPos + direction * maxDistance);
            break; 
        }
    }
}


    public void ToggleLaser(bool active)
    {
        lineRenderer.enabled = active;
    }

    public void ResetLaser()
    {
        currentDirection = initialDirection;
    }

    public void SetLaserDirection(Vector3 newDirection)
    {
        currentDirection = newDirection.normalized;
    }
}