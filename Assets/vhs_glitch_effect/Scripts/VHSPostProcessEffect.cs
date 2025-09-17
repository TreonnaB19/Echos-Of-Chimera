using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(VideoPlayer))]
public class VHSPostProcessEffect : MonoBehaviour
{
    public Shader shader;
    public VideoClip VHSClip;

    private Material _material;
    private VideoPlayer _player;
    private RenderTexture _rt;
    private float _yScanline;
    private float _xScanline;

    private void OnEnable()
    {
        if (shader == null)
        {
            enabled = false;
            return;
        }

        _material = new Material(shader) { hideFlags = HideFlags.DontSave };

        _player = GetComponent<VideoPlayer>();
        _player.source = VideoSource.VideoClip;
        _player.clip = VHSClip;
        _player.isLooping = true;
        _player.audioOutputMode = VideoAudioOutputMode.None;

        // Allocate a stable RT for the video to render into (prevents “red flash” frames)
        int w = (VHSClip != null && VHSClip.width  > 0) ? (int)VHSClip.width  : 640;
        int h = (VHSClip != null && VHSClip.height > 0) ? (int)VHSClip.height : 360;
        _rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            useMipMap = false
        };
        _rt.Create();

        _player.renderMode = VideoRenderMode.RenderTexture;
        _player.targetTexture = _rt;

        _player.prepareCompleted += OnPrepared;
        _player.loopPointReached += OnLoop;

        _player.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    private void OnLoop(VideoPlayer vp)
    {
        // randomize on loop
        _yScanline = Random.value;
        _xScanline = Random.value;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Pass-through until the video is ready (prevents null frame flashes)
        if (_material == null || _rt == null || _player == null || !_player.isPrepared || _player.texture == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        _material.SetTexture("_VHSTex", _rt);

        _yScanline += Time.deltaTime * 0.01f;
        _xScanline -= Time.deltaTime * 0.1f;

        if (_yScanline >= 1f) _yScanline = Random.value;
        if (_xScanline <= 0f || Random.value < 0.05f) _xScanline = Random.value;

        _material.SetFloat("_yScanline", _yScanline);
        _material.SetFloat("_xScanline", _xScanline);

        Graphics.Blit(source, destination, _material);
    }

    private void OnDisable()
    {
        if (_player != null)
        {
            _player.prepareCompleted -= OnPrepared;
            _player.loopPointReached -= OnLoop;
            _player.targetTexture = null;
        }

        if (_rt != null)
        {
            _rt.Release();
            DestroyImmediate(_rt);
            _rt = null;
        }

        if (_material != null)
        {
            DestroyImmediate(_material);
            _material = null;
        }
    }
}
