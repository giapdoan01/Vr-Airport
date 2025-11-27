using UnityEngine;

public class fuelTruck : MonoBehaviour
{
    [Header("Truck Movement Settings")]
    public float reverseSpeed = 5f;
    public float rotationSpeed = 50f;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    // Hàm lùi xe - đi về hướng âm X
    public void ReverseTruck()
    {
        // Di chuyển về hướng âm X
        Vector3 reverseDirection = Vector3.left; // hoặc new Vector3(-1, 0, 0)
        transform.Translate(reverseDirection * reverseSpeed * Time.deltaTime, Space.World);
        
        // Log để debug
        Debug.Log("Truck đang lùi về hướng âm X với tốc độ: " + reverseSpeed);
    }
    
    // Hàm lùi xe với thời gian cụ thể
    public void ReverseTruckForDuration(float duration)
    {
        StartCoroutine(ReverseCoroutine(duration));
    }
    
    // Coroutine để lùi xe trong thời gian nhất định
    private System.Collections.IEnumerator ReverseCoroutine(float duration)
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            Vector3 reverseDirection = Vector3.left; // Hướng âm X
            transform.Translate(reverseDirection * reverseSpeed * Time.deltaTime, Space.World);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("Hoàn thành lùi xe trong " + duration + " giây");
    }
    
    // Hàm lùi xe một khoảng cách cố định về hướng âm X
    public void ReverseTruckDistance(float distance)
    {
        StartCoroutine(ReverseDistanceCoroutine(distance));
    }
    
    // Coroutine để lùi xe một khoảng cách nhất định
    private System.Collections.IEnumerator ReverseDistanceCoroutine(float distance)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (Vector3.left * distance);
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 reverseDirection = Vector3.left;
            transform.Translate(reverseDirection * reverseSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
        
        Debug.Log("Hoàn thành lùi xe khoảng cách: " + distance + " units về hướng âm X");
    }
    
    // Hàm di chuyển về hướng dương X (tiến)
    public void ForwardTruck()
    {
        Vector3 forwardDirection = Vector3.right; // Hướng dương X
        transform.Translate(forwardDirection * reverseSpeed * Time.deltaTime, Space.World);
        Debug.Log("Truck đang tiến về hướng dương X");
    }
}
