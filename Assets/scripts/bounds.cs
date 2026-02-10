using UnityEngine;

public class Bounds : MonoBehaviour
{
    public Transform player; // Перетащи перса сюда в инспекторе
    public BoxCollider2D mapBounds; // Перетащи объект с границами карты
    
    private float camVertExtent; // Половина высоты камеры
    private float camHorizExtent; // Половина ширины камеры
    
    private float leftBound, rightBound, bottomBound, topBound;
    
    void Start()
    {
        // Считаем размеры камеры (ортографической!)
        camVertExtent = Camera.main.orthographicSize;
        camHorizExtent = camVertExtent * Screen.width / Screen.height;
        
        // Границы карты с учётом размера камеры (чтоб не вылезала краями)
        leftBound = mapBounds.bounds.min.x + camHorizExtent;
        rightBound = mapBounds.bounds.max.x - camHorizExtent;
        bottomBound = mapBounds.bounds.min.y + camVertExtent;
        topBound = mapBounds.bounds.max.y - camVertExtent;
    }
    
    void LateUpdate() // LateUpdate — чтоб после движения перса
    {
        Vector3 targetPos = player.position;
        targetPos.z = transform.position.z; // Z не трогаем (-10 обычно)
        
        // Клампим позицию камеры в границы
        targetPos.x = Mathf.Clamp(targetPos.x, leftBound, rightBound);
        targetPos.y = Mathf.Clamp(targetPos.y, bottomBound, topBound);
        
        transform.position = targetPos; // Или с Lerp для плавности: Vector3.Lerp(transform.position, targetPos, 0.1f);
    }
}