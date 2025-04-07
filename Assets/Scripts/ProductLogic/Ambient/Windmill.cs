using UnityEngine;

public class Windmill : MonoBehaviour
{
    private float _coefficent = 10;

    private void Update()
    {
        transform.Rotate(Vector3.zero.AddZ(_coefficent) * Time.deltaTime);
    }
}
