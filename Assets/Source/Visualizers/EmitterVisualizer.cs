using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;

    public ParticleSystem Particles;

    private float baseSpeed;
    private float setScalar;
    private Vector3 startPos;
    private Vector3 startPosNormal;

    private void Start()
    {
        startPos = transform.localPosition;
        startPosNormal = startPos.normalized;
        baseSpeed = Particles.main.startSpeedMultiplier;
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
        transform.localPosition = startPos - ((setScalar * 5f) * startPosNormal);
        Color particleColor = new Color(value, value, 1f - value);
        ParticleSystem.MainModule mainModule = Particles.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
        mainModule.startSpeedMultiplier = baseSpeed + (value * 100f);
    }
}
