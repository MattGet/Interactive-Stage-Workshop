using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using FireworksMania.Core.Definitions;
using FireworksMania.Core.Messaging;

public class InputMenu : MonoBehaviour
{
    [Header("Video Main")]
    public Button cancel;
    public Button OK;
    public Button Paste;
    public TMP_InputField inputField;
    public TMP_Dropdown Quality;
    public AudioClip clicks;
    public AudioClip errors;
    public AudioSource audioplay;
    public Sprite ToggleOn;
    public Sprite ToggleOff;

    [Header("Video Controls")]
    public Button play;
    public Button pause;
    public Button stop;
    public Button Back10;
    public Button Forward10;
    public Toggle loop;
    public Sprite loopoff;
    public Sprite loopon;
    public Slider volume;
    public Slider VidTime;
    public TMP_Text TimeValue;
    public float volnumb = 1;

    [Header("Reacting Lights Settings")]
    public ReactingLights lights;
    public Button ToggleMode;
    public Button LightsToggle;
    public Button LasersToggle;
    


    [Header("Lights Settings")]
    public GameObject LightSettings;
    public Slider SAlpha;
    public Slider SColor;
    public Slider SBuffer;
    public Slider SEnhancer;
    public TMP_Dropdown ColorMode;

    [Header("Laser Settings")]
    public GameObject LaserSettings;
    public Slider SLAlpha;
    public Slider SLColor;
    public Slider SLBuffer;
    public Slider SLEnhancer;
    public TMP_Dropdown SLColorMode;

    private StageVideoManager videocontroller;
    private bool whiledisplay = false;

    private void Awake()
    {
        Hide();
    }



    public void Update()
    {
        if (whiledisplay && Cursor.visible == false || Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Playclick()
    {
        if (!audioplay.gameObject.activeInHierarchy) return;
        audioplay.clip = clicks;
        audioplay.Play();
    }
    public void Playerror()
    {
        if (!audioplay.gameObject.activeInHierarchy) return;
        audioplay.clip = errors;
        audioplay.Play();
    }

    public void Show(string inputtext, StageVideoManager start)
    {
        gameObject.SetActive(true);
        inputField.text = inputtext;
        whiledisplay = true;
        videocontroller = start;

        videocontroller.VideoPrepared.AddListener(StartTime);

        Messenger.Broadcast(new MessengerEventChangeUIMode(true, false));

        if (videocontroller.player.isLooping)
        {
            loop.image.sprite = loopon;
        }
        else
        {
            loop.image.sprite = loopoff;
        }
        volume.value = volnumb;
        Svolume(volnumb);
        if (videocontroller.player.isPrepared || videocontroller.isPlaying)
        {
            StartTime();
        }

        CQuality(Quality.value);
        CColorMode(((int)lights.colorMode));
        LASCColorMode(((int)lights.LAScolorMode));
        SBuffer.value = lights.AudioVisualiser.BufferMultiplier;

        SAlpha.value = lights.AlphaMulti;
        SColor.value = lights.ColorMulti;
        SEnhancer.value = lights.ColorEnhancer;
        SLAlpha.value = lights.LASAlphaMulti;
        SLColor.value = lights.LASColorMulti;
        SLEnhancer.value = lights.LASColorEnhancer;
        OnOpen();

    }

    private void OnOpen()
    {
        if (lights.UseLights)
        {
            LightsToggle.image.sprite = ToggleOn;
            LightSettings.SetActive(true);
        }
        else
        {
            LightsToggle.image.sprite = ToggleOff;
            LightSettings.SetActive(false);
        }

        if (lights.UseLasers)
        {
            LasersToggle.image.sprite = ToggleOn;
            LaserSettings.SetActive(true);
        }
        else
        {
            LasersToggle.image.sprite = ToggleOff;
            LaserSettings.SetActive(false);
        }

        if (lights.UseAudioColor)
        {
            ToggleMode.image.sprite = ToggleOn;
        }
        else
        {
            ToggleMode.image.sprite = ToggleOff;
        }
    }

    public void ToggleColorMode()
    {
        if (lights.UseAudioColor)
        {
            lights.UseAudioColor = false;
            ToggleMode.image.sprite = ToggleOff;
            LightSettings.SetActive(false);
            LaserSettings.SetActive(false);
        }
        else
        {
            lights.UseAudioColor = true;
            ToggleMode.image.sprite = ToggleOn;
            if (lights.UseLasers) LaserSettings.SetActive(true);
            if (lights.UseLights) LightSettings.SetActive(true);
        }
        Playclick();
    }

    public void ToggleLights()
    {
        lights.UseLights = !lights.UseLights;
        lights.ToggleLights(lights.UseLights);
        if (lights.UseAudioColor) LightSettings.SetActive(lights.UseLights);
        if (lights.UseLights)
        {
            LightsToggle.image.sprite = ToggleOn;
        }
        else
        {
            LightsToggle.image.sprite = ToggleOff;
        }
        Playclick();
    }

    public void ToggleLasers()
    {
        lights.UseLasers = !lights.UseLasers;
        lights.ToggleLasers(lights.UseLasers);
        if (lights.UseAudioColor) LaserSettings.SetActive(lights.UseLasers);
        if (lights.UseLasers)
        {
            LasersToggle.image.sprite = ToggleOn;
        }
        else
        {
            LasersToggle.image.sprite = ToggleOff;
        }
        Playclick();
    }

    public void SetBufferM(float buff)
    {
        lights.AudioVisualiser.BufferMultiplier = buff;
    }

    public void SetAlpha(float color)
    {
        lights.AlphaMulti = color;
    }

    public void SetColorM(float color)
    {
        lights.ColorMulti = color;
    }

    public void SetEnhancer(float Enh)
    {
        lights.ColorEnhancer = Enh;
    }

    public void LASSetAlpha(float color)
    {
        lights.LASAlphaMulti = color;
    }

    public void LASSetColorM(float color)
    {
        lights.LASColorMulti = color;
    }

    public void LASSetEnhancer(float Enh)
    {
        lights.LASColorEnhancer = Enh;
    }

    public void StartTime()
    {
        if (videocontroller.player != null)
        {
            Debug.Log($"Starting Time Counter max time = {videocontroller.player.length}");
            VidTime.maxValue = (float)videocontroller.player.length;
            StartCoroutine(updatetime());
        }
    }

    public void CQuality(int id)
    {
        videocontroller.Quality(id);
    }

    public void CColorMode(int id)
    {
        lights.SetColorMode(id);
    }

    public void LASCColorMode(int id)
    {
        lights.SetColorMode(id, true);
    }


    public void Svolume(float value)
    {
        //Debug.Log("New Slider Value " + value);
        videocontroller.player.GetTargetAudioSource(0).volume = value;
        volnumb = value;
        if (value == 0)
        {
            lights.volumeMulti = 0;
        }
        else
        {
            lights.volumeMulti = 1 / value;
        }

        //Debug.Log("confirmedslidervol = " + videocontroller.player.GetDirectAudioVolume(0) + "/" + volnumb);
    }

    public void CLoop(bool value)
    {
        if (videocontroller != null)
        {
            if (value)
            {
                Playclick();
                loop.image.sprite = loopon;
                //Debug.Log("Set to loop");
                videocontroller.player.isLooping = true;
            }
            else
            {
                Playclick();
                loop.image.sprite = loopoff;
                //Debug.Log("Stopped looping");
                videocontroller.player.isLooping = false;
            }
        }
        else
        {
            Playerror();
        }
    }

    public void CBack()
    {
        if (videocontroller == null)
        {
            return;
        }
        Playclick();
        videocontroller.player.time = videocontroller.player.time - 10;
    }

    public void CForward()
    {
        if (videocontroller == null)
        {
            return;
        }
        Playclick();
        videocontroller.player.time = videocontroller.player.time + 10;
    }

    public void CPlay()
    {
        if (videocontroller != null)
        {
            if (!videocontroller.player.isPlaying && !videocontroller.player.isPaused)
            {
                //Debug.Log("Force Starting");
                videocontroller.Quality(Quality.value);
                videocontroller.forcestartvideo(inputField.text);
                if (lights.UseLights) lights.ToggleLights(true);
                if (lights.UseLasers) lights.ToggleLasers(true);
                if (lights.UseLasers && !videocontroller.IsAnimating) videocontroller.ToggleAnimations();
                Playclick();
            }
            else if (videocontroller.player.isPaused)
            {
                videocontroller.player.Play();
                if (lights.UseLights) lights.ToggleLights(true);
                if (lights.UseLasers) lights.ToggleLasers(true);
                if (lights.UseLasers && !videocontroller.IsAnimating) videocontroller.ToggleAnimations();
                Playclick();
                videocontroller.isPlaying = true;
            }
            else
            {
                Playerror();
            }

        }
        else
        {
            Playerror();
        }
    }
    public void CStop()
    {
        if (videocontroller != null)
        {
            if (videocontroller.player.isPlaying || videocontroller.player.isPaused)
            {

                Playclick();
                videocontroller.Stop();
                lights.ToggleLights(false);
                lights.ToggleLasers(false);
                if (lights.UseLasers && videocontroller.IsAnimating) videocontroller.ToggleAnimations();
                videocontroller.onlytriggeronce = false;
                videocontroller.isPlaying = false;
            }
            else
            {
                Playerror();
            }

        }
        else
        {
            Playerror();
        }
    }
    public void CPause()
    {
        if (videocontroller != null)
        {
            if (videocontroller.player.isPlaying && !videocontroller.player.isPaused)
            {
                videocontroller.player.Pause();
                videocontroller.player.playbackSpeed = 1;
                videocontroller.isSeeking = false;
                if (lights.UseLasers && videocontroller.IsAnimating) videocontroller.ToggleAnimations();
                Playclick();
            }
            else
            {
                Playerror();
            }

        }
        else
        {
            Playerror();
        }
    }

    private IEnumerator updatetime()
    {
        do
        {
            VidTime.SetValueWithoutNotify((float)videocontroller.player.time);
            TimeValue.text = SecToMinString(videocontroller.player.time);
            yield return new WaitForSeconds(1);
        } while (whiledisplay);
        yield break;
    }

    private string SecToMinString(double seconds)
    {
        string Time = "";
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        if (t.Hours != 0)
        {
            Time = $"{t.Hours}:{t.Minutes.ToString("00")}:{t.Seconds.ToString("00")}";
            return Time;
        }
        if (t.Hours == 0)
        {
            Time = $"{t.Minutes.ToString("00")}:{t.Seconds.ToString("00")}";
            return Time;
        }
        if (t.Minutes == 0)
        {
            Time = $"{seconds.ToString("00")}";
            return Time;
        }
        Time = $"{t.Minutes.ToString("00")}:{t.Seconds.ToString("00")}";
        return Time;
    }

    public void CScrubber(float time)
    {
        if (videocontroller == null)
        {
            return;
        }
        if (videocontroller.player.isPrepared || videocontroller.player.isPlaying || videocontroller.player.isPaused)
        {
            videocontroller.player.time = time;
            Debug.Log($"Setting time to: {time}");
        }
    }

    public void PasteClick()
    {
        inputField.text = GUIUtility.systemCopyBuffer;
        Playclick();
    }

    public void OKClick()
    {
        Playclick();
        Hide();
        if (videocontroller.isPlaying || videocontroller.player.isPlaying || videocontroller.player.isPaused)
        {
            return;
        }
        videocontroller.OK(inputField.text);
        videocontroller.Quality(Quality.value);
    }
    public void CancelClick()
    {
        Playclick();
        videocontroller.Cancel();
        Hide();
    }

    public void Hide()
    {
        whiledisplay = false;
        Messenger.Broadcast(new MessengerEventChangeUIMode(false, true));
        if (videocontroller != null)
        {
            videocontroller.VideoPrepared.RemoveListener(StartTime);
        }
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (videocontroller != null)
        {
            videocontroller.VideoPrepared.RemoveListener(StartTime);
        }
    }
}
