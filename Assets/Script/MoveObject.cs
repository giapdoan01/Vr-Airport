using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float distanceToCamera;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;

                    distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);

                    Vector3 worldPoint = ray.GetPoint(distanceToCamera);
                    offset = transform.position - worldPoint;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPoint = ray.GetPoint(distanceToCamera);
            transform.position = worldPoint + offset;
        }
    }
}
