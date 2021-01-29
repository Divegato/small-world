using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float RotationsPerMinute = 1;

    void Update()
    {
        var rotationsPerFrame = (RotationsPerMinute / 60f) * Time.deltaTime;
        var angle = (2f * Mathf.PI) * rotationsPerFrame;

        if (TryGetComponent<Rigidbody2D>(out var rigidbody))
        {
            rigidbody.MoveRotation(angle);
        }
        else
        {
            gameObject.transform.Rotate(Vector3.forward, angle);
        }
    }
}
