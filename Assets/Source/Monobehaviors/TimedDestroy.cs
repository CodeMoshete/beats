using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float DestroyTime;

    void Update()
    {
        DestroyTime -= Time.deltaTime;
        if (DestroyTime <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
