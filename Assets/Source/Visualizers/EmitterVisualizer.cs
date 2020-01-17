using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;

    public ParticleSystem Particles;

    private float setScalar;
    private Vector3 startPos;
    private Vector3 startPosNormal;

    private void Start()
    {
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
        transform.localPosition = startPos + ((setScalar * 30f) * startPosNormal);
        Color particleColor = new Color(1f - value, 1f - value, value);
        ParticleSystem.MainModule mainModule = Particles.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
    }
}
