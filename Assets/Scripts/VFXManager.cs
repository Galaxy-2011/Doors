using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Post Processing")]
    public PostProcessVolume postProcessVolume;
    public float chromaticAmplitude = 1f;
    public float vignetteAmplitude = 0.5f;

    [Header("Screen Shake")]
    public float traumaDecaySpeed = 1.5f;
    public float maxShakeAngle = 10f;
    public float maxShakeOffset = 0.5f;
    public float shakeFrequency = 25f;

    private float trauma = 0f;
    private ChromaticAberration chromatic;
    private Vignette vignette;
    private Camera mainCamera;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out chromatic);
            postProcessVolume.profile.TryGetSettings(out vignette);
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.localPosition;
            originalCameraRot = mainCamera.transform.localRotation;
        }
    }

    private void Update()
    {
        // Decay trauma over time
        if (trauma > 0)
        {
            trauma = Mathf.Max(0, trauma - traumaDecaySpeed * Time.deltaTime);
            ApplyScreenShake();
        }
    }

    public void AddTrauma(float amount)
    {
        trauma = Mathf.Clamp01(trauma + amount);
    }

    public void ChromaticAberrationPulse(float intensity = 1f, float duration = 0.5f)
    {
        if (chromatic != null)
        {
            StopCoroutine("ChromaticPulseCoroutine");
            StartCoroutine(ChromaticPulseCoroutine(intensity, duration));
        }
    }

    public void VignettePulse(float intensity = 1f, float duration = 0.5f)
    {
        if (vignette != null)
        {
            StopCoroutine("VignettePulseCoroutine");
            StartCoroutine(VignettePulseCoroutine(intensity, duration));
        }
    }

    public void ScreenDistortion(float duration = 1f, float intensity = 1f)
    {
        ChromaticAberrationPulse(intensity, duration);
        VignettePulse(intensity * 0.75f, duration);
        AddTrauma(intensity * 0.5f);
    }

    private void ApplyScreenShake()
    {
        if (mainCamera == null) return;

        float shake = trauma * trauma; // Square for more dramatic effect at high trauma
        
        // Perlin noise for smooth random movement
        float offsetX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0) - 0.5f) * maxShakeOffset * shake;
        float offsetY = (Mathf.PerlinNoise(0, Time.time * shakeFrequency) - 0.5f) * maxShakeOffset * shake;
        float angleZ = (Mathf.PerlinNoise(Time.time * shakeFrequency, Time.time * shakeFrequency) - 0.5f) * maxShakeAngle * shake;

        mainCamera.transform.localPosition = originalCameraPos + new Vector3(offsetX, offsetY, 0);
        mainCamera.transform.localRotation = originalCameraRot * Quaternion.Euler(0, 0, angleZ);
    }

    private IEnumerator ChromaticPulseCoroutine(float intensity, float duration)
    {
        float startIntensity = chromatic.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentIntensity = Mathf.Lerp(intensity * chromaticAmplitude, startIntensity, t);
            chromatic.intensity.value = currentIntensity;
            yield return null;
        }

        chromatic.intensity.value = startIntensity;
    }

    private IEnumerator VignettePulseCoroutine(float intensity, float duration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentIntensity = Mathf.Lerp(intensity * vignetteAmplitude, startIntensity, t);
            vignette.intensity.value = currentIntensity;
            yield return null;
        }

        vignette.intensity.value = startIntensity;
    }
}