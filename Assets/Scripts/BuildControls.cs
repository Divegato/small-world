using Assets.Scripts.Helpers;
using UnityEngine;

public class BuildControls : MonoBehaviour
{
    public GameObject Spawn;

    private int blockCount = 0;

    void Start()
    {
        blockCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (blockCount > 0)
            {
                blockCount--;
                var mousePoint = Input.mousePosition;
                mousePoint.z = 10f;
                var target = Camera.main.ScreenToWorldPoint(mousePoint);

                Spawner.BuildBlock(Spawn, target);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

            if (hit && hit.transform.localScale.x * hit.transform.localScale.y <= 1.5 && hit.transform.tag == "Item")
            {
                Destroy(hit.transform.gameObject);
                blockCount++;
            }
        }
    }
}
