using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class BuildControls : MonoBehaviour
{
    public SpriteAtlas Sprites;

    public GameObject Spawn;

    public GameObject Cursor;

    public int BlockCount = 0;

    public float BuildDistance = 5;

    private Light2D CursorLight;
    private Vector3Int CursorTarget;

    private Tile[] Tiles;

    void Start()
    {
        BlockCount = 100;
        Cursor = Instantiate(Cursor);
        Cursor.SetActive(false);

        CursorLight = Cursor.GetComponent<Light2D>();

        var spriteArray = new Sprite[Sprites.spriteCount];
        var count = Sprites.GetSprites(spriteArray);

        Tiles = spriteArray.Select(x =>
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = x;
            tile.colliderType = Tile.ColliderType.Grid;

            return tile;
        }).ToArray();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (BlockCount > 0)
            {
                var mousePoint = Input.mousePosition;
                mousePoint.z = 10f;
                var target = Camera.main.ScreenToWorldPoint(mousePoint);
                var buildDistance = (Vector2)transform.position - (Vector2)target;

                if (buildDistance.magnitude < 10 && buildDistance.magnitude > 2.1)
                {
                    var targets = Physics2D.OverlapCircleAll(transform.position, 10);

                    Tilemap tileMap = null;
                    var found = targets.Any(x => x.gameObject.TryGetComponent(out tileMap));
                    if (found)
                    {
                        Vector3Int tileCoordinate = tileMap.WorldToCell(target);
                        var existingSprite = tileMap.GetSprite(tileCoordinate);

                        if (existingSprite == null)
                        {
                            tileMap.SetTile(tileCoordinate, Tiles[0]);
                            BlockCount--;
                        }
                    }
                }
            }
        }

        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10);
        var distance = (Vector2)transform.position - hit.point;

        if (hit && distance.magnitude < BuildDistance && hit.transform.gameObject.TryGetComponent<Deconstructable>(out var deconstructable))
        {
            var target = hit.transform.gameObject;
            var color = Color.cyan;

            if (target.TryGetComponent<Tilemap>(out var tileMap))
            {
                var center = tileMap.GetCellCenterWorld(Vector3Int.zero) - tileMap.transform.position;
                Vector3Int tileCoordinate = tileMap.WorldToCell((Vector3)mouseWorldPos - center);
                var tileLocation = tileMap.GetCellCenterWorld(tileCoordinate);
                var localPosition = tileMap.CellToLocalInterpolated(tileCoordinate);
                CursorTarget = tileCoordinate;
                // Todo, make hex cursor
                Cursor.transform.parent = hit.transform;
                Cursor.transform.position = tileLocation;
                //Cursor.transform.localPosition = localPosition;
                Cursor.transform.localRotation = Quaternion.identity;

                if (Input.GetMouseButton(1))
                {
                    BlockCount++;
                    color = Color.red;
                    tileMap.SetTile(tileCoordinate, null);
                }

                CursorLight.color = color;
                Cursor.SetActive(true);

                Debug.DrawLine(transform.position, (Vector3)mouseWorldPos - center, color);
                Debug.DrawLine((Vector3)mouseWorldPos - center, tileLocation, color);
            }
            else if (target.TryGetComponent<SpriteShapeController>(out var spriteShape))
            {
                Cursor.transform.parent = hit.transform;
                Cursor.transform.position = hit.point;
                Cursor.transform.localRotation = Quaternion.identity;
                Cursor.transform.localPosition = new Vector3(Mathf.Round(Cursor.transform.localPosition.x + 0.5f) - 0.5f, Mathf.Round(Cursor.transform.localPosition.y + 0.5f) - 0.5f, 0);

                if (Input.GetMouseButtonDown(1))
                {
                    color = Color.red;

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

                CursorLight.color = color;
                Cursor.SetActive(true);
            }


            //for (int i = 0; i < 4; i++)
            //{
            //    var a1 = i * (Mathf.PI / 2) + hit.transform.rotation.z;
            //    var a2 = (i + 1) * (Mathf.PI / 2) + hit.transform.rotation.z;

            //    var sin1 = Mathf.Sin(a1) + hit.point.x;
            //    var cos1 = Mathf.Cos(a1) + hit.point.y;
            //    var sin2 = Mathf.Sin(a2) + hit.point.x;
            //    var cos2 = Mathf.Cos(a2) + hit.point.y;

            //    Debug.DrawLine(new Vector3(sin1, cos1, 0), new Vector3(sin2, cos2, 0), color);
            //}

            Debug.DrawLine(transform.position, mouseWorldPos, color);
            Debug.DrawLine(transform.position, hit.point, color);
            Debug.DrawLine(hit.point, hit.transform.position, color);
        }
        else
        {
            Cursor.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
#if DEBUG
        Handles.Label(Cursor.transform.position, CursorTarget.ToString());
#endif
    }
}
