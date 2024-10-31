using UnityEngine;
using UnityEngine.UI;

public class ShowScore : MonoBehaviour
{
    void FixedUpdate()
    {
        GetComponent<Text>().text = Data.score.ToString("000");
    }
}
