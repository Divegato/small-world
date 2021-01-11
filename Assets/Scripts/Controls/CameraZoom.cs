using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float CameraSizeMin = -1000f;
    public float CameraSizeMax = -10f;

    void Start()
    {

    }

    void Update()
    {
        var currentSize = Camera.main.transform.position.z;
        currentSize *= 1f - Input.GetAxis("Mouse ScrollWheel");

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Mathf.Clamp(currentSize, CameraSizeMin, CameraSizeMax));
    }
}
