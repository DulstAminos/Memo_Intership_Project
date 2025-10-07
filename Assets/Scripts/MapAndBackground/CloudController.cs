using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float speed = 0.25f;
    public float leftLimitation = -5.5f;
    public float rightLimitation = 7f;

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        position.x += -speed * Time.deltaTime;
        if (position.x < leftLimitation)
        {
            position.x = rightLimitation;
        }
        transform.position = position;
    }
}
