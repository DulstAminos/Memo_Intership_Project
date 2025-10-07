using UnityEngine;

public class BoundaryLimitation : MonoBehaviour
{
    public float mapWidth = 10f;
    public float mapHeight = 10f;
    public bool wrapHorizontally = true;
    public bool wrapVertically = true;

    private float lastChangeTime = 0f;
    public float cooldownTime = 0.2f;
    private void LateUpdate()
    {
        if (Time.time - lastChangeTime >= cooldownTime)
        {

            Vector2 pos = transform.position;

            if (wrapHorizontally)
            {
                if (pos.x > mapWidth / 2)
                {
                    pos.x -= mapWidth;
                }
                else if (pos.x < -mapWidth / 2)
                {
                    pos.x += mapWidth;
                }
            }

            if (wrapVertically)
            {
                if (pos.y > mapHeight / 2)
                {
                    pos.y -= mapHeight;
                }
                else if (pos.y < -mapHeight / 2)
                {
                    pos.y += mapHeight;
                }
            }

            transform.position = pos;
        }
    }
}
