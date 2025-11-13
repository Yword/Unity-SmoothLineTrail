using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 amount;

    private void Update()
    {
        transform.Rotate(amount * Time.deltaTime);
    }
}
