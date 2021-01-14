using Assets.Scripts.Helpers;
using UnityEngine;

public class BuildControls : MonoBehaviour
{
    public GameObject Spawn;

    public int BlockCount = 0;

    void Start()
    {
        BlockCount = 100;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BlockCount > 0)
            {
                var mousePoint = Input.mousePosition;
                mousePoint.z = 10f;
                var target = Camera.main.ScreenToWorldPoint(mousePoint);

                if (Vector3.Distance(target, gameObject.transform.position) < 5)
                {
                    BlockCount--;
                    Spawner.BuildBlock(Spawn, target);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var results = new RaycastHit2D[5];
            var hits = Physics2D.Raycast(ray.origin, ray.direction, new ContactFilter2D(), results);

            if (hits > 0)
            {
                foreach (var hit in results)
                {
                    if (hit && hit.transform.localScale.x * hit.transform.localScale.y <= 1.5 && hit.transform.tag == "Item")
                    {
                        Destroy(hit.transform.gameObject);
                        BlockCount++;
                    }
                }
            }
        }
    }

    private void ThreeDExample()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out var hitInfo);

            if (hit && hitInfo.transform.localScale.x * hitInfo.transform.localScale.y <= 1.5 && hitInfo.transform.tag == "Item")
            {
                Destroy(hitInfo.transform.gameObject);
                BlockCount++;
            }
        }
    }
}
