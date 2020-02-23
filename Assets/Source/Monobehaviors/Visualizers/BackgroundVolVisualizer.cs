using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BackgroundVolVisualizer : MonoBehaviour, IAverageSpectrumVisualizer
{
    private const float MAX_SCALE = 2f;
    private const float FADE_THRESHOLD = 0.2f;
    private const float MAX_FADE_PCT = 0.7f;

    public List<Texture2D> backgroundTextures;
    public float transitionTime = 10f;
    public Transform background;
    public Renderer bgRenderer;
    public GameObject ErrorCanvas;

    private float transitionTimeTotal;
    private float transitionTimeCurrent;
    private Material bgMaterial;
    private int bgIndex;

    void Start()
    {
        transitionTimeTotal = transitionTime;
        transitionTimeCurrent = transitionTime;
        bgMaterial = bgRenderer.material;

        backgroundTextures = new List<Texture2D>();
        string imagesPath = Path.GetFullPath("./images");
        if (Directory.Exists(imagesPath))
        {
            string[] files = Directory.GetFiles(imagesPath);
            for (int i = 0, count = files.Length; i < count; ++i)
            {
                byte[] imageBytes = File.ReadAllBytes(files[i]);
                Texture2D tex = new Texture2D(1920, 1080);
                bool didLoad = ImageConversion.LoadImage(tex, imageBytes);
                if (didLoad)
                {
                    backgroundTextures.Add(tex);
                }
            }

            if (backgroundTextures.Count > 0)
            {
                bgMaterial.SetTexture("_MainTex", backgroundTextures[bgIndex]);
            }
            else
            {
                ErrorCanvas.SetActive(true);
            }
        }
        else
        {
            ErrorCanvas.SetActive(true);
        }
    }

    public void VisualizeValue(float[] values, float spectrumAverage)
    {
        transitionTimeCurrent -= Time.deltaTime * spectrumAverage;

        float pct = 1f - (transitionTimeCurrent / transitionTimeTotal);
        float scale = (MAX_SCALE - 1f) * pct + 1f;
        background.localScale = new Vector3(scale, scale, scale);

        if (transitionTimeCurrent <= 0f)
        {
            bgIndex = (bgIndex == backgroundTextures.Count - 1) ? 0 : bgIndex + 1;
            bgMaterial.SetTexture("_MainTex", backgroundTextures[bgIndex]);
            transitionTimeCurrent = transitionTimeTotal;
        }

        float fadePct = 1f;
        if (pct < FADE_THRESHOLD)
        {
            float subPct = pct / FADE_THRESHOLD;
            fadePct = (Mathf.Sin((subPct - 0.5f) * Mathf.PI) / 2) + 0.5f;
        }
        else if (pct > (1f - FADE_THRESHOLD))
        {
            float subPct = 1f - ((pct - (1f - FADE_THRESHOLD)) / FADE_THRESHOLD);
            fadePct = (Mathf.Sin((subPct - 0.5f) * Mathf.PI) / 2) + 0.5f;
        }
        fadePct *= MAX_FADE_PCT;

        bgMaterial.color = new Color(fadePct, fadePct, fadePct);
    }
}
