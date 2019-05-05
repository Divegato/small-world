using Assets.Scripts.Helpers;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    public GameObject Asteroid;
    public float SpawnRate = 10;
    public float MaxAsteroids = 100;

    private float progress = 0;

    void Start()
    {
        progress = Random.value * SpawnRate;
    }

    void Update()
    {
        progress += Time.deltaTime;

        if (progress >= SpawnRate)
        {
            progress = 0;

            var asteroids = FindObjectsOfType<Asteroid>();

            if (asteroids.Length < MaxAsteroids)
            {
                SpawnAsteroid();
            }

            CleanupAsteroids(asteroids);
        }
    }

    private void CleanupAsteroids(Asteroid[] asteroids)
    {
        foreach (var asteroid in asteroids)
        {
            var radius = Mathf.Pow(Mathf.Pow(asteroid.transform.position.x, 2) + Mathf.Pow(asteroid.transform.position.y, 2), 0.5f);
            var maxRange = GetRadius() * 1.5f;

            if (radius > maxRange)
            {
                Destroy(asteroid.gameObject);
            }
        }
    }

    private float GetRadius()
    {
        var radius = gameObject.transform.localScale.y / 2;
        return radius;
    }

    private GameObject SpawnAsteroid()
    {
        var radius = GetRadius();
        var position = Geometry.GetRandomPointOnCircle(radius);

        var force = (Random.value * 10f) + 5f;
        var trajectory = Geometry.GetRandomPointOnCircle(force);
        var result = Instantiate(Asteroid, position, Quaternion.identity);
        var body = result.GetComponent<Rigidbody2D>();

        body.AddForce(trajectory, ForceMode2D.Impulse);

        return result;
    }
}
