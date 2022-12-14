using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using sourcenity;
using System.Linq;
using DG.Tweening;

public class ReactingLights : MonoBehaviour
{
    [Header("MAIN")]
    public StageVideoManager VideoManager;
    public AudioPeer AudioVisualiser;
    public VideoPlayer videoSource;
    public Light[] lights;
	public GameObject laserParent;
    public ShowLaserEffect[] lasers;
    public GameObject Vlights;
    public Light[] vlights;
    public GameObject[] objects;
    public float volumeMulti = 1;
    public int sensitivity;

    private Texture2D tex;
    private bool enable;

    [Header("light Settings")]
    public Color averageColor;
    public ColorMode colorMode = ColorMode.One;
    [Range(0, 2)]
    public float AlphaMulti = 1;
    [Range(0, 1)]
    public float ColorMulti = 1;
    public float ColorEnhancer = 1.5f;
    public float LightSensitivity = 0.25f;
    public Color LColor1 = new Color();
    public Color LColor2 = new Color();

    [Header("Laser Settings")]
    public Color LASaverageColor;
    public ColorMode LAScolorMode = ColorMode.One;
    [Range(0, 2)]
    public float LASAlphaMulti = 1;
    [Range(0, 1)]
    public float LASColorMulti = 1;
    public float LASColorEnhancer = 1.5f;
    public float LaserSensitivity = 0.5f;
    public Color LZColor1 = new Color();
    public Color LZColor2 = new Color();


    [Header("Toggle Settings")]
    bool createTexture = false;
    public bool UseAudioColor = false;
    public bool UseLights = true;
    public bool UseLasers = true;
    public bool UseObjects = true;



    public enum ColorMode { 
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Custom,
    }




    public enum VideoSide{
		up,
		left,
		right,
		down,
		center
	}

	public VideoSide videoSide = VideoSide.center;

    private void Start()
    {
        videoSource.frameReady += NewFrame;
        videoSource.sendFrameReadyEvents = true;
    }

    private void Awake()
    {
        ToggleLights(false);
        ToggleLasers(false);
    }

    private int curr = 0;

    public void Update()
    {
        if (UseAudioColor && VideoManager.isPlaying)
        {
            if (curr <= 0)
            {
                if (UseLasers)
                {
                    Color Laser = SetAudioColor(LAScolorMode, LASColorMulti, LASAlphaMulti, LASColorEnhancer, false);
                    ApplyColor(Laser, true, false);
                }
                if (UseLights)
                {
                    Color Light = SetAudioColor(colorMode, ColorMulti, AlphaMulti, ColorEnhancer, true);
                    ApplyColor(Light, false, true);
                }
                curr = sensitivity;
            }
            else { curr--; }
        }
    }

    private void OnDestroy()
    {
        videoSource.frameReady -= NewFrame;
    }

    public void ToggleLights(bool On)
    {
        foreach (Light L in lights)
        {
            L.gameObject.SetActive(On);
        }
        foreach (Light L in vlights)
        {
            L.gameObject.SetActive(On);
        }
    }

    public void ToggleLasers(bool On)
    {
        foreach (ShowLaserEffect L in lasers)
        {
            L.gameObject.SetActive(On);
        }
    }


    public void SetColorMode(int input, bool SetLaser = false)
    {
        ColorMode Mode = new ColorMode();
        switch (input)
        {
            case 0:
                Mode = ColorMode.One;
                break;
            case 1:
                Mode = ColorMode.Two;
                break;
            case 2:
                Mode = ColorMode.Three;
                break;
            case 3:
                Mode = ColorMode.Four;
                break;
            case 4:
                Mode = ColorMode.Five;
                break;
            case 5:
                Mode = ColorMode.Six;
                break;
            case 6:
                Mode = ColorMode.Custom;
                break;
            default:
                Mode = ColorMode.One;
                break;
        }
        if (!SetLaser)
        {
            colorMode = Mode;
        }
        else
        {
            LAScolorMode = Mode;
        }
    }

    private void OnValidate()
    {
        if (Vlights != null)
        {
            vlights = Vlights.GetComponentsInChildren<Light>();
        }
        if (laserParent != null)
        {
            lasers = laserParent.GetComponentsInChildren<ShowLaserEffect>();
        }
    }

    private Color SetAudioColor(ColorMode colorMode, float ColorMulti, float AlphaMulti, float ColorEnhancer, bool isLight)
    {
        float avrg1 = Mathf.Clamp01(((AudioVisualiser.audioBandBuffer[0] + AudioVisualiser.audioBandBuffer[1] + AudioVisualiser.audioBandBuffer[2]) / 3) * volumeMulti * ColorMulti);
        float avrg2 = Mathf.Clamp01(((AudioVisualiser.audioBandBuffer[3] + AudioVisualiser.audioBandBuffer[4] + AudioVisualiser.audioBandBuffer[5]) / 3) * volumeMulti * ColorMulti);
        float avrg3 = Mathf.Clamp01(((AudioVisualiser.audioBandBuffer[6] + AudioVisualiser.audioBandBuffer[7]) / 2) * volumeMulti * ColorMulti);
        float avrg4 = Mathf.Clamp01(((AudioVisualiser.audioBandBuffer[0] + AudioVisualiser.audioBandBuffer[1] + AudioVisualiser.audioBandBuffer[2] + AudioVisualiser.audioBandBuffer[3])/4) * volumeMulti * ColorMulti);
        float avrg5 = Mathf.Clamp01(((AudioVisualiser.audioBandBuffer[4] + AudioVisualiser.audioBandBuffer[5] + AudioVisualiser.audioBandBuffer[6] + AudioVisualiser.audioBandBuffer[7])/4) * volumeMulti * ColorMulti);
        float Avalue = Mathf.Clamp01(AudioVisualiser.AmplitudeBuffer * AlphaMulti * volumeMulti);
        float[] values = new float[5] { avrg1, avrg2, avrg3, avrg4, avrg5 };
        for (int i = 0; i < 3; i++)
        {
            if (values[i] == values.Max())
            {
                values[i] = Mathf.Clamp01(values[i] * ColorEnhancer);
            }
            if (values[i] < 0.01)
            {
                values[i] = 0;
            }
        }
        Color temp = new Color(values[0], values[1], values[2], Avalue);
        switch (colorMode)
        {
            case ColorMode.One:
                temp = new Color(values[0], values[1], values[2], Avalue);
                break;
            case ColorMode.Two:
                temp = new Color(values[1], values[0], values[2], Avalue);
                break;
            case ColorMode.Three:
                temp = new Color(values[1], values[2], values[0], Avalue);
                break;
            case ColorMode.Four:
                temp = new Color(values[0], values[2], values[1], Avalue);
                break;
            case ColorMode.Five:
                temp = new Color(values[2], values[0], values[1], Avalue);
                break;
            case ColorMode.Six:
                temp = new Color(values[2], values[1], values[0], Avalue);
                break;
            case ColorMode.Custom:
                
                if (isLight)
                {
                    float additive = Mathf.Clamp01(Avalue * 1.15f);
                    if (values[3] > values[4]) { temp = new Color(LColor1.r * additive, LColor1.g * additive, LColor1.b * additive, Avalue); }
                    else { temp = new Color(LColor2.r * additive, LColor2.g * additive, LColor2.b * additive, Avalue); }
                }
                else {
                    float additive = Mathf.Clamp01(Avalue * 1.5f);
                    if (values[3] > values[4]) { temp = new Color(LZColor1.r * additive, LZColor1.g * additive, LZColor1.b * additive, Avalue); }
                    else { temp = new Color(LZColor2.r * additive, LZColor2.g * additive, LZColor2.b * additive, Avalue); }
                }
                break;
        }
        //Debug.Log($"Applying Color, R: {avrg1}, G: {avrg2}, B: {avrg3}, A: {AudioVisualiser.AmplitudeBuffer * AlphaMulti}");
        return temp;
    }


    private void NewFrame(VideoPlayer vplayer, long frame)
    {
        if (!createTexture)
        {
            createTexture = true;
            switch (videoSide)
            {
                case VideoSide.up:
                    tex = new Texture2D(videoSource.texture.width / 2, 20);
                    break;
                case VideoSide.down:
                    tex = new Texture2D(videoSource.texture.width / 2, 20);
                    break;
                case VideoSide.left:
                    tex = new Texture2D(20, videoSource.texture.height / 2);
                    break;
                case VideoSide.right:
                    tex = new Texture2D(20, videoSource.texture.height / 2);
                    break;
                case VideoSide.center:
                    tex = new Texture2D(videoSource.texture.height / 2, videoSource.texture.height / 2);
                    break;
            }
        }
        RenderTexture.active = (RenderTexture)videoSource.texture;
        switch (videoSide)
        {
            case VideoSide.up:
                tex.ReadPixels(new Rect((videoSource.texture.width / 2), 0, videoSource.texture.width / 2, 20), 0, 0);
                break;
            case VideoSide.down:
                tex.ReadPixels(new Rect((videoSource.texture.width / 2), videoSource.texture.height - 20, videoSource.texture.width / 2, 20), 0, 0);
                break;
            case VideoSide.left:
                tex.ReadPixels(new Rect(0, 0, 20, videoSource.texture.height / 2), 0, 0);
                break;
            case VideoSide.right:
                tex.ReadPixels(new Rect(videoSource.texture.width - 20, 0, 20, videoSource.texture.height / 2), 0, 0);
                break;
            case VideoSide.center:
                tex.ReadPixels(new Rect((videoSource.texture.width / 2) - (videoSource.texture.width / 2), (videoSource.texture.height / 2) - (videoSource.texture.height / 2), videoSource.texture.width / 2, videoSource.texture.height / 2), 0, 0);
                break;
        }

        tex.Apply();
        averageColor = AverageColorFromTexture(tex);

        if (!UseAudioColor)
        {
            ApplyColor(averageColor, true, true);
        }

        ScreenColor(averageColor);
    }

    private void ScreenColor(Color color)
    {
        if (!videoSource.isPlaying)
        {
            color = Color.clear;
        }
        foreach (Light light in lights)
        {
            Color setcolor;
            float a = color.a;
            if (color.a <= LightSensitivity)
            {
                a = 0;
            }
            if (color.grayscale <= 0.1)
            {
                a = 0;
            }
            setcolor = new Color(color.r, color.g, color.b, a);
            light.color = setcolor;
        }
    }

    private void ApplyColor(Color color, bool IsLaser, bool IsLight)
    {
        float Avalue = Mathf.Clamp01(AudioVisualiser.AmplitudeBuffer * volumeMulti);
        if (UseLasers && IsLaser)
        {
            foreach (ShowLaserEffect laser in lasers)
            {
                Color setcolor2;
                float a2 = color.a;
                if (Avalue <= LaserSensitivity)
                {
                    a2 = 0;
                }
                if (color.grayscale <= 0.1)
                {
                    a2 = 0;
                }
                setcolor2 = new Color(color.r, color.g, color.b, a2);
                laser.mainColor = setcolor2;
            }
        }
        
        if (UseLights && IsLight)
        {
            foreach (Light light in vlights)
            {
                Color setcolor3;
                float a3 = color.a;
                if (Avalue <= LightSensitivity)
                {
                    a3 = 0;
                }
                if (color.grayscale <= 0.1)
                {
                    a3 = 0;
                }
                setcolor3 = new Color(color.r, color.g, color.b, a3);
                light.color = setcolor3;
            }
        }
        if (UseObjects && IsLight)
        {
            if (Application.isPlaying)
            {
                foreach (GameObject temp in objects)
                {
                    MeshRenderer mr = temp.GetComponent<MeshRenderer>();
                    foreach (Material mat in mr.sharedMaterials)
                    {
                        mat.SetColor("_EmissionColor", color);
                    }
                }
            }
        }
        
    }

    Color32 AverageColorFromTexture(Texture2D tex)
	{

		Color32[] texColors = tex.GetPixels32();

		int total = texColors.Length;

		float r = 0;
		float g = 0;
        float b = 0;
        float a = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

            a += texColors[i].a;
        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , (byte)(a / total));

	}

    public void ToggleLaserUpdates(bool On) {
        foreach (ShowLaserEffect laser in lasers) {
            laser.DoAnimate = On;
        }
    }
}
