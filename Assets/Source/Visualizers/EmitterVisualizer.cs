using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;

    private float setScalar;
    private Vector3 startPos;
    private Vector3 startPosNormal;

    private void Start()
    {
        setScalar = transform.position.magnitude;
        startPos = transform.localPosition;
        startPosNormal = startPos.normalized;
    }

    private void Update()
    {
        if (setScalar > 0)
        {
            setScalar -= (SCALE_DOWN_SPEED * Time.deltaTime);
        }
    }

    public void VisualizeValue(float value)
    {
        setScalar = Mathf.Max(setScalar, value);
        transform.position = startPos + ((setScalar * 10f) * startPosNormal);
    }
}
