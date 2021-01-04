using Assets.Scripts.Helpers;
using UnityEngine;

public class StarlightScript : MonoBehaviour
{
    public GameObject Starlight;
    public float Radius = 10000;
    public float Quantity = 100;

    void Start()
    {
        for (int i = 0; i < Quantity; i++)
        {
            var position = Geometry.GetRandomPointOnCircle(Radius);
            Instantiate(Starlight, position, Quaternion.identity);
        }
    }

    void Update()
    {

    }
}
