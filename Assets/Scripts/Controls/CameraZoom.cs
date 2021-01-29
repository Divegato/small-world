using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float CameraSizeMin = 5f;
    public float CameraSizeMax = 10000f;

    void Start()
    {

    }

    void Update()
    {
        var currentSize = Camera.main.orthographicSize;
        currentSize *= 1f - Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize = Mathf.Clamp(currentSize, CameraSizeMin, CameraSizeMax);
    }
}
