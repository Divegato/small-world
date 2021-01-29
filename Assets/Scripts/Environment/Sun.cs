using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Sun : MonoBehaviour
{
    public Light2D Light;
    public float Radius = 1000;
    public float Quantity = 100;

    void Start()
    {
        for (float i = 0; i < Quantity; i++)
        {
            var angle = 2 * Mathf.PI * (i / Quantity);
            var position = new Vector2(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius);

            Instantiate(Light, position, Quaternion.identity, gameObject.transform);
        }
    }
}
