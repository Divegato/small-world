using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;

public class BuildControls : MonoBehaviour
{
    public GameObject Spawn;

    public GameObject Cursor;

    public int BlockCount = 0;

    public float BuildDistance = 5;

    private Light2D CursorLight;

    void Start()
    {
        BlockCount = 100;
        Cursor = Instantiate(Cursor);
        Cursor.SetActive(false);

        CursorLight = Cursor.GetComponent<Light2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BlockCount > 0 && false)
            {
                var mousePoint = Input.mousePosition;
                mousePoint.z = 10f;
                var target = Camera.main.ScreenToWorldPoint(mousePoint);

                if (Vector3.Distance(target, gameObject.transform.position) < 5)
                {
                    BlockCount--;
                    Spawner.BuildBlock(Spawn, new Vector3(target.x, target.y, 0));
                }
            }
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10);
        var distance = (Vector2)transform.position - hit.point;

        if (hit && distance.magnitude < BuildDistance && hit.transform.gameObject.TryGetComponent<Deconstructable>(out var deconstructable))
        {
            Cursor.transform.parent = hit.transform;
            Cursor.transform.position = hit.point;
            Cursor.transform.localRotation = Quaternion.identity;
            Cursor.transform.localPosition = new Vector3(Mathf.Round(Cursor.transform.localPosition.x + 0.5f) - 0.5f, Mathf.Round(Cursor.transform.localPosition.y + 0.5f) - 0.5f, 0);

            var color = Color.cyan;
            if (Input.GetMouseButtonDown(1))
            {
                color = Color.red;

                var target = hit.transform.gameObject;

                if (target.TryGetComponent<SpriteShapeController>(out var spriteShape))
                {
                    var closestPoint = 0;
                    var closestDistance = 99999f;
                    for (int i = 0; i < spriteShape.spline.GetPointCount(); i++)
                    {
                        var point = spriteShape.spline.GetPosition(i);
                        var pointDistance = (Cursor.transform.localPosition - point).magnitude;

                        if (pointDistance < closestDistance)
                        {
                            closestDistance = pointDistance;
                            closestPoint = i;
                        }
                    }

                    if (closestDistance > 0.1)
                    {
                        spriteShape.spline.InsertPointAt(closestPoint, Cursor.transform.localPosition + new Vector3(0.5f, -0.5f, 0));
                        spriteShape.spline.InsertPointAt(closestPoint, Cursor.transform.localPosition + new Vector3(-0.5f, -0.5f, 0));
                        spriteShape.spline.InsertPointAt(closestPoint, Cursor.transform.localPosition + new Vector3(-0.5f, 0.5f, 0));
                        spriteShape.spline.InsertPointAt(closestPoint, Cursor.transform.localPosition + new Vector3(0.5f, 0.5f, 0));
                    }
                }

                //var results = new RaycastHit2D[5];
                //var hits = Physics2D.Raycast(ray.origin, ray.direction, new ContactFilter2D(), results);

                //if (hits > 0)
                //{
                //foreach (var hit in results)
                //{
                //if (hit && hit.transform.localScale.x * hit.transform.localScale.y <= 1.5 && hit.transform.tag == "Item")
                //{
                //Destroy(hit.transform.gameObject);
                //BlockCount++;
                //}
                //}
                //}
            }

            CursorLight.color = color;
            Cursor.SetActive(true);

            for (int i = 0; i < 4; i++)
            {
                var a1 = i * (Mathf.PI / 2) + hit.transform.rotation.z;
                var a2 = (i + 1) * (Mathf.PI / 2) + hit.transform.rotation.z;

                var sin1 = Mathf.Sin(a1) + hit.point.x;
                var cos1 = Mathf.Cos(a1) + hit.point.y;
                var sin2 = Mathf.Sin(a2) + hit.point.x;
                var cos2 = Mathf.Cos(a2) + hit.point.y;

                Debug.DrawLine(new Vector3(sin1, cos1, 0), new Vector3(sin2, cos2, 0), color);
            }

            Debug.DrawLine(transform.position, hit.point, color);
            Debug.DrawLine(hit.point, hit.transform.position, color);
        }
        else
        {
            Cursor.SetActive(false);
        }
    }
}
