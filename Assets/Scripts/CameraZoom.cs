using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float CameraSizeMin = 5f;
    public float CameraSizeMax = 1000f;
    public float CameraScrollRate = 10f;

    void Start()
    {

    }

    void Update()
    {
        // Lets do this by percent in the future
        var currentSize = Camera.main.orthographicSize;
        currentSize += Input.GetAxis("Mouse ScrollWheel") * -1f * CameraScrollRate;

        Camera.main.orthographicSize = Mathf.Clamp(currentSize, CameraSizeMin, CameraSizeMax);
    }
}
