using Assets.Scripts.Helpers;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    public GameObject Asteroid;
    public float SpawnRate = 1;

    private float progress = 0;

    void Start()
    {
        progress = 0;
    }

    void Update()
    {
        progress += Time.deltaTime;

        if (progress >= SpawnRate)
        {
            progress = 0;
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        var radius = gameObject.transform.localScale.y / 2;
        var position = Geometry.GetRandomPointOnCircle(radius);

        var force = (Random.value * 10f) + 5f;
        var trajectory = Geometry.GetRandomPointOnCircle(force);
        var result = Instantiate(Asteroid, position, Quaternion.identity);
        var body = result.GetComponent<Rigidbody2D>();

        body.AddForce(trajectory, ForceMode2D.Impulse);
    }
}
