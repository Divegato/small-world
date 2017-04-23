using System;
using UnityEngine;

public class CloseHelp : MonoBehaviour
{
    private DateTime start;

    void Start()
    {
        start = DateTime.Now;
    }

    void Update()
    {
        if ((DateTime.Now - start) > TimeSpan.FromSeconds(10))
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Help"))
            {
                Destroy(item);
            }
        }
    }
}
