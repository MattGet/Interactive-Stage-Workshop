using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using sourcenity;

public class ReactingLights : MonoBehaviour
{

    public bool UseAudioColor = false;
    public AudioPeer AudioVisualiser;
    public VideoPlayer videoSource;
    public Light[] lights;
	public GameObject laserParent;
    public ShowLaserEffect[] lasers;
    public Color averageColor;
    private Texture2D tex;
    private bool enable;
    public GameObject Vlights;
    public Light[] vlights;
    public GameObject[] objects;
    bool createTexture = false;


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
        if (!UseAudioColor)
        {
            videoSource.frameReady += NewFrame;
            videoSource.sendFrameReadyEvents = true;
        }

        ToggleLights(false);
    }

    public void Update()
    {
        if (UseAudioColor)
        {

        }
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
        foreach (ShowLaserEffect L in lasers)
        {
            L.gameObject.SetActive(On);
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

    private void SetAudioColor()
    {
        Color temp = new Color(AudioVisualiser.AmplitudeBuffer * 255, AudioVisualiser.AmplitudeBuffer * 255, AudioVisualiser.AmplitudeBuffer * 255, AudioVisualiser.AmplitudeBuffer * 255);
        ApplyColor(temp);
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
        ApplyColor(averageColor);
    }

    private void ApplyColor(Color color)
    {
        if (!videoSource.isPlaying)
        {
            color = Color.clear;
        }
        foreach (Light light in lights)
        {
            Color setcolor;
            float a = color.a;
            if (color.a <= 0.25)
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
        foreach (ShowLaserEffect laser in lasers)
        {
            Color setcolor2;
            float a2 = color.a;
            if (color.a <= 0.5)
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
        foreach (Light light in vlights)
        {
            Color setcolor3;
            float a3 = color.a;
            if (color.a <= 0.25)
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
}
