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

    public Button cancel;
    public Button OK;
    public Button Paste;
    public TMP_InputField inputField;
    public TMP_Dropdown Quality;
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
    private StageVideoManager videocontroller;
    private bool whiledisplay = false;
    public AudioClip clicks;
    public AudioClip errors;
    public AudioSource audioplay;
    public float volnumb = 1;
    public ReactingLights lights;
    public Slider SAlpha;
    public Slider SColor;
    public Slider SBuffer;
    public Slider SEnhancer;
    public Button ToggleMode;
    public TMP_Dropdown ColorMode;


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

        SAlpha.value = lights.AlphaMulti;
        SColor.value = lights.ColorMulti;
        SBuffer.value = lights.AudioVisualiser.BufferMultiplier;
        SEnhancer.value = lights.ColorEnhancer;
        if (lights.UseAudioColor)
        {
            ToggleMode.image.color = Color.magenta;
        }
        else
        {
            ToggleMode.image.color = Color.cyan;
        }
    }

    public void ToggleColorMode()
    {
        if (lights.UseAudioColor)
        {
            lights.UseAudioColor = false;
            ToggleMode.image.color = Color.cyan;
        }
        else
        {
            lights.UseAudioColor = true;
            ToggleMode.image.color = Color.magenta;
        }
    }
    public void SetAlpha(float color)
    {
        lights.AlphaMulti = color;
    }

    public void SetColorM(float color)
    {
        lights.ColorMulti = color;
    }

    public void SetBufferM(float buff)
    {
        lights.AudioVisualiser.BufferMultiplier = buff;
    }

    public void SetEnhancer(float Enh)
    {
        lights.ColorEnhancer = Enh;
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
                lights.ToggleLights(true);
                videocontroller.ToggleAnimations();
                Playclick();
            }
            else if (videocontroller.player.isPaused)
            {
                videocontroller.player.Play();
                lights.ToggleLights(true);
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
                videocontroller.ToggleAnimations();
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
