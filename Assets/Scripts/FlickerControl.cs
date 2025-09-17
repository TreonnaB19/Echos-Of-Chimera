using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerControl : MonoBehaviour
{
    [Header("Targets")]
    public Renderer BulbRenderer;          // assign the lamp mesh renderer

    [Header("Light Flicker")]
    public float minIntensity = 0.6f;      // how dim it gets
    public float maxIntensity = 2.5f;      // normal/peak brightness
    public Vector2 flickerDelay = new Vector2(0.04f, 0.18f); // time between changes

    [Header("Emission Flicker")]
    public Color emissionTint = Color.white;
    public float minEmission = 0.5f;       // matches minIntensity visually
    public float maxEmission = 8f;         // HDR value; tweak to taste
    public bool smooth = true;             // smooth lerp instead of hard steps
    public float smoothSpeed = 20f;

    Light _light;
    Material _bulbMat;                     // instanced material
    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    float _targetIntensity;
    float _targetEmission;

    void Awake()
    {
        _light = GetComponent<Light>();
        if (BulbRenderer != null)
        {
            // Instance the material so we don't edit the shared one
            _bulbMat = BulbRenderer.material;
            _bulbMat.EnableKeyword("_EMISSION");
        }
        // start at max
        _light.intensity = maxIntensity;
        _targetIntensity = maxIntensity;
        _targetEmission  = maxEmission;
        ApplyEmission(_targetEmission);
        StartCoroutine(FlickerLoop());
    }

    void Update()
    {
        if (!smooth) return;

        // Smooth toward targets for a natural fluorescent feel
        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * smoothSpeed);
        float currentEmit = Mathf.Lerp(GetCurrentEmission(), _targetEmission, Time.deltaTime * smoothSpeed);
        ApplyEmission(currentEmit);
    }

    IEnumerator FlickerLoop()
    {
        var wait = new WaitForSeconds(Random.Range(flickerDelay.x, flickerDelay.y));

        while (true)
        {
            // Pick new targets
            _targetIntensity = Random.Range(minIntensity, maxIntensity);
            _targetEmission  = Mathf.Lerp(minEmission, maxEmission,
                                  Mathf.InverseLerp(minIntensity, maxIntensity, _targetIntensity));

            if (!smooth)
            {
                _light.intensity = _targetIntensity;
                ApplyEmission(_targetEmission);
            }

            // Vary timing each step
            wait = new WaitForSeconds(Random.Range(flickerDelay.x, flickerDelay.y));
            yield return wait;

            // Random occasional quick "blip" dimming
            if (Random.value < 0.12f)
            {
                float blipInt = Random.Range(minIntensity * 0.2f, minIntensity * 0.6f);
                float blipEmi = Mathf.Max(minEmission * 0.2f, 0f);
                if (smooth)
                {
                    _targetIntensity = blipInt;
                    _targetEmission  = blipEmi;
                }
                else
                {
                    _light.intensity = blipInt;
                    ApplyEmission(blipEmi);
                }
                yield return new WaitForSeconds(Random.Range(0.02f, 0.06f));
            }
        }
    }

    // Apply emission color to material
    void ApplyEmission(float value)
    {
        if (_bulbMat != null)
            _bulbMat.SetColor(EmissionColorID, emissionTint * value);
    }

    // Get current max color component of emissiom
    float GetCurrentEmission()
    {
        if (_bulbMat == null) return 0f;
        return _bulbMat.GetColor(EmissionColorID).maxColorComponent;
    }
}
