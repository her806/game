using UnityEngine;

public class Bounds : MonoBehaviour
{
    public Transform player;
    public BoxCollider2D mapBounds;
    
    private float camVertExtent;
    private float camHorizExtent; 
    
    private float leftBound, rightBound, bottomBound, topBound;
    
    void Start()
    {

        camVertExtent = Camera.main.orthographicSize;
        camHorizExtent = camVertExtent * Screen.width / Screen.height;
        
        leftBound = mapBounds.bounds.min.x + camHorizExtent;
        rightBound = mapBounds.bounds.max.x - camHorizExtent;
        bottomBound = mapBounds.bounds.min.y + camVertExtent;
        topBound = mapBounds.bounds.max.y - camVertExtent;
    }
    
    void LateUpdate()
    {
        Vector3 targetPos = player.position;
        targetPos.z = transform.position.z; 
        
        targetPos.x = Mathf.Clamp(targetPos.x, leftBound, rightBound);
        targetPos.y = Mathf.Clamp(targetPos.y, bottomBound, topBound);
        
        transform.position = targetPos;
    }
}