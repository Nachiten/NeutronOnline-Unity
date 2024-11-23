using UnityEngine;

public class MouseWorldVisual : MonoBehaviour
{
    [SerializeField] private LayerMask mousePlaneLayerMask;
    
    private Camera mainCamera;
    
    // PRIVATE Singleton
    private static MouseWorldVisual Instance;
    
    private const float RAYCAST_DISTANCE = 100f;
    
    private void Awake()
    { 
        Instance = this;      
        
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition(out bool valid);
        
        if (valid)
            transform.position = mouseWorldPosition;
    }
    
    private static Vector3 GetMouseWorldPosition(out bool valid)
    {
        Vector2 mouseScreenPosition = InputManager.Instance.GetMouseScreenPosition();
        
        Ray ray = Instance.mainCamera.ScreenPointToRay(mouseScreenPosition);
        
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, RAYCAST_DISTANCE, Instance.mousePlaneLayerMask);
        
        System.Array.Sort(raycastHits, (x, y) => x.distance.CompareTo(y.distance));
        
        foreach (RaycastHit raycastHit in raycastHits)
        {
            // If there is a renderer, and it is enabled, return the point
            if (raycastHit.transform.TryGetComponent(out Renderer renderer) && renderer.enabled)
            {
                valid = true;
                return raycastHit.point;
            }
        }
        
        valid = false;
        return Vector3.zero;
    }
    
    public static GridPosition GetMouseGridPosition()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition(out bool valid);
        
        return valid ? LevelGrid.Instance.GetGridPos(mouseWorldPosition) : GridPosition.Null;
    }
}