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
        var selfSize = gameObject.transform;

        var angle = Random.value * Mathf.PI * 2;

        var x = Mathf.Cos(angle) * selfSize.localScale.x / 2;
        var y = Mathf.Sin(angle) * selfSize.localScale.y / 2;

        var position = new Vector3(x, y, 0);

        var result = Instantiate(Asteroid, position, Quaternion.identity);
        var body = result.GetComponent<Rigidbody2D>();

        var randomVelocity = (Random.value * 10f) + 5f;
        body.AddForce(Random.insideUnitCircle * randomVelocity, ForceMode2D.Impulse);
    }
}
