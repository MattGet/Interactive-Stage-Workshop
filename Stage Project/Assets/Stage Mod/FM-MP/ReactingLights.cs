using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using sourcenity;

public class ReactingLights : MonoBehaviour {

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
        ToggleLights(false);
    }

    public void ToggleLights(bool On)
    {
        foreach (Light L in lights)
        {
            L.enabled = On;
        }
        foreach (Light L in vlights)
        {
            L.enabled = On;
        }
        foreach (ShowLaserEffect L in lasers)
        {
            L.enabled = On;
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

    bool createTexture = false;
	private void NewFrame(VideoPlayer vplayer, long frame)
    {
        if (!createTexture) {
			createTexture = true;
			switch (videoSide) {
			case VideoSide.up:
				tex = new Texture2D(videoSource.texture.width/2,20);
				break;
			case VideoSide.down:
				tex = new Texture2D(videoSource.texture.width/2,20);
				break;
			case VideoSide.left:
				tex = new Texture2D(20,videoSource.texture.height/2);
				break;
			case VideoSide.right:
				tex = new Texture2D(20,videoSource.texture.height/2);
				break;
			case VideoSide.center:
				tex = new Texture2D (videoSource.texture.height / 2, videoSource.texture.height / 2);
				break;
			}
		}
		RenderTexture.active = (RenderTexture)videoSource.texture;
		switch (videoSide) {
			case VideoSide.up:
			tex.ReadPixels(new Rect((videoSource.texture.width/2),0,videoSource.texture.width/2,20),0,0);
				break;
			case VideoSide.down:
			tex.ReadPixels(new Rect((videoSource.texture.width/2),videoSource.texture.height-20,videoSource.texture.width/2,20),0,0);
				break;
			case VideoSide.left:
			tex.ReadPixels(new Rect(0,0,20,videoSource.texture.height/2),0,0);
				break;
			case VideoSide.right:
			tex.ReadPixels(new Rect(videoSource.texture.width-20,0,20,videoSource.texture.height/2),0,0);
				break;
			case VideoSide.center:
				tex.ReadPixels(new Rect((videoSource.texture.width/2)-(videoSource.texture.width/2),(videoSource.texture.height/2)-(videoSource.texture.height/2),videoSource.texture.width/2,videoSource.texture.height/2),0,0);
				break;
		}

		tex.Apply();
        averageColor = AverageColorFromTexture(tex);
		if (!videoSource.isPlaying)
		{
			averageColor = Color.clear;
		}
		foreach (Light light in lights)
        {
            Color setcolor;
            float a = averageColor.a;
            if (averageColor.a <= 0.25)
            {
                a = 0;
            }
            if (averageColor.grayscale <= 0.1)
            {
                a = 0;
            }
            setcolor = new Color(averageColor.r, averageColor.g, averageColor.b, a);
            light.color = setcolor;
        }
        foreach (ShowLaserEffect laser in lasers)
        {
            Color setcolor2;
            float a2 = averageColor.a;
            if (averageColor.a <= 0.5)
            {
                a2 = 0;
            }
            if (averageColor.grayscale <= 0.1)
            {
                a2 = 0;
            }
            setcolor2 = new Color(averageColor.r, averageColor.g, averageColor.b, a2);
            laser.mainColor = setcolor2;
        }
        foreach (Light light in vlights)
        {
            Color setcolor3;
            float a3 = averageColor.a;
            if (averageColor.a <= 0.25)
            {
                a3 = 0;
            }
            if (averageColor.grayscale <= 0.1)
            {
                a3 = 0;
            }
            setcolor3 = new Color(averageColor.r, averageColor.g, averageColor.b, a3);
            light.color = setcolor3;
        }
        if (Application.isPlaying)
        {
            foreach (GameObject temp in objects)
            {
                MeshRenderer mr = temp.GetComponent<MeshRenderer>();
                foreach (Material mat in mr.sharedMaterials)
                {
                    mat.SetColor("_EmissionColor", averageColor);
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

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 255);

	}
}
