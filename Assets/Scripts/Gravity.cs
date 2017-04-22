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
        var self = this.gameObject.transform.position;
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        var vector2 = new Vector2(self.x, self.y);

        var difference = vector2 - player.position;
        var percent = Mathf.Min(1, Mathf.Exp(difference.magnitude * (-1f / GravityRange)));

        player.AddForce(difference.normalized * GravityPower * percent, ForceMode2D.Force);
    }
}
