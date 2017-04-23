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
        var selfSize = GetComponent<Renderer>().bounds;

        var x = (Random.value > .5 ? -1 : 0) * selfSize.extents.x;
        var y = (Random.value > .5 ? -1 : 0) * selfSize.extents.y;

        var position = new Vector3(x, y, 0);

        var result = Instantiate(Asteroid, position, Quaternion.identity);
        var body = result.GetComponent<Rigidbody2D>();

        var randomVelocity = (Random.value * 10f) + 5f;
        body.AddForce(Random.insideUnitCircle * randomVelocity, ForceMode2D.Impulse);
    }
}
