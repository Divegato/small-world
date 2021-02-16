using Assets.Scripts.Helpers;
using UnityEngine;

public class SelfLevelingController : MonoBehaviour
{
    private Rigidbody2D body;
    private Quaternion smooth = Quaternion.identity;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var force = Gravity.GetAverageGravitationalForce(body.centerOfMass + (Vector2)body.transform.position, body.mass, true);
        var target = Quaternion.LookRotation(Vector3.forward, force * -1);

        transform.rotation = SmoothDamp(transform.rotation, target, ref smooth, 1f);
    }

    private static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;

        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;

        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }
}
