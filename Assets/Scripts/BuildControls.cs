using UnityEngine;

public class BuildControls : MonoBehaviour
{
    public GameObject Spawn;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePoint = Input.mousePosition;
            mousePoint.z = 10f;
            Instantiate(Spawn, Camera.main.ScreenToWorldPoint(mousePoint), Quaternion.identity);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

            if (hit && hit.transform.localScale.x * hit.transform.localScale.y <= 1.5 && hit.transform.tag == "Item")
            {
                Destroy(hit.transform.gameObject);
                // Add to inventory
            }
        }
    }
}
