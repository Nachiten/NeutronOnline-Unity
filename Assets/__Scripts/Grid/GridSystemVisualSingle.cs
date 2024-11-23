using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform hoveredVisual;
    
    private Color defaultColor;

    private void Awake()
    {
        SetHovered(false);
    }

    public void SetDefaultColor(Color color)
    {
        spriteRenderer.color = color;
        defaultColor = color;
    }
    
    public void SetSpriteColor(Color color)
    {
        spriteRenderer.color = color;
    }
    
    public void ResetColor()
    {
        spriteRenderer.color = defaultColor;
    }

    public void SetHovered(bool hovered)
    {
        hoveredVisual.gameObject.SetActive(hovered);
    }
}