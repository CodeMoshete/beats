using UnityEngine;

public class BarVisualizer : MonoBehaviour, IVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;
    private float setScale;

    public void Initialize(int index)
    {
        // Intentionally empty.
    }

    private void Update()
    {
        if (setScale > 0)
        {
            setScale -= (SCALE_DOWN_SPEED * Time.deltaTime);
        }
    }

    public void VisualizeValue(float value)
    {
        setScale = Mathf.Max(setScale, value);
        transform.localScale = new Vector3(1f, setScale, 1f);
    }
}
