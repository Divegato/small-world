using UnityEngine;
using UnityEngine.UI;

public class BlocksCount : MonoBehaviour
{
    private Text reference;

    // Use this for initialization
    void Start()
    {
        reference = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var build = player.GetComponent<BuildControls>();

        reference.text = "Blocks: " + build.BlockCount;
    }
}
