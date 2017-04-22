using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float GravityPower = 1;
    public float GravityRange = 10;

    void Start()
    {

    }

    void Update()
    {
        var selfPosition = GetSelfPosition2d();
        var selfRadius = GetSelfRadius();

        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        var items = GameObject.FindGameObjectsWithTag("Item");

        AddGravityForce(player, selfPosition, selfRadius);

        foreach (var item in items)
        {
            var body = item.GetComponent<Rigidbody2D>();
            AddGravityForce(body, selfPosition, selfRadius);
        }
    }

    private Vector2 GetSelfPosition2d()
    {
        var selfPosition = this.gameObject.transform.position;
        return new Vector2(selfPosition.x, selfPosition.y);
    }

    private float GetSelfRadius()
    {
        return GetComponent<CircleCollider2D>().radius;
    }

    private void AddGravityForce(Rigidbody2D body, Vector2 selfPosition, float selfRadius)
    {
        var differenceFromCore = (selfPosition - body.position);
        var distanceFromSurface = differenceFromCore.magnitude - selfRadius;

        var percent = Mathf.Min(1, Mathf.Exp(distanceFromSurface * (-1f / GravityRange)));
        //Debug.Log(string.Format("from core {0}, from surface {1}, percent {2}", differenceFromCore.magnitude, distanceFromSurface, percent));

        body.AddForce(differenceFromCore.normalized * GravityPower * percent, ForceMode2D.Force);
    }
}
