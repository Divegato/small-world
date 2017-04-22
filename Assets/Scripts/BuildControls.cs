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
    }
}
