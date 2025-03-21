using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 10f;
    public float smoothTime = 0.2f;

    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        targetPosition = transform.position;
        targetZoom = Vector3.Distance(transform.position, transform.position + transform.forward * maxZoom);
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        ApplySmoothMovement();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;

        moveDirection.y = 0; // Keep movement horizontal
        targetPosition += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Lock X rotation to 45 degrees
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = 45f;
        transform.eulerAngles = eulerAngles;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    private void ApplySmoothMovement()
    {
        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Calculate the desired position for zooming along the local Z-axis
        Vector3 zoomOffset = transform.forward * targetZoom;
        Vector3 desiredPosition = targetPosition - zoomOffset;

        // Smoothly adjust the camera's position for zooming
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}