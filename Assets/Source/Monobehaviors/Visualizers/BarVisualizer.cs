using UnityEngine;
using UnityEngine.UI;

public class BarVisualizer : MonoBehaviour, IFullSpectrumVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;
    public bool DebugMode;
    public GameObject TextObject;
    public Text IndexText;
    private float setScale;

    public void Initialize(int index)
    {
        TextObject.SetActive(DebugMode);
        IndexText.text = index.ToString();
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
