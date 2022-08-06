using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;

    public float AudioProfileFloat;
    public float Amplitude;
    public float AmplitudeBuffer;
    public float AmplitudeHighest;

    public float[] audioBand = new float[8];
    public float[] audioBandBuffer = new float[8];
    public float[] BufferDecrease = new float[8];

    private static float[] audioSamplesLeft = new float[512];
    private static float[] audioSamplesRight = new float[512];
    private static float[] frequencyBands = new float[8];
    private static float[] bandBuffer = new float[8];

    private float[] frequencyBandHigh = new float[8];

   


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = this.gameObject.GetComponent<AudioSource>();
        AudioProfile();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void GetAmplitude()
    {
        float CurrAmp = 0;
        float CurrAmpBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            CurrAmp += audioBand[i];
            CurrAmpBuffer += audioBandBuffer[i];
        }
        if (CurrAmp > AmplitudeHighest)
        {
            AmplitudeHighest = CurrAmp;
        }
        Amplitude = CurrAmp / AmplitudeHighest;
        AmplitudeBuffer = CurrAmpBuffer / AmplitudeHighest;
    }

    void AudioProfile()
    {
        for(int i = 0; i < 8; i++)
        {
            frequencyBandHigh[i] = AudioProfileFloat;
        }
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (frequencyBands[i] > frequencyBandHigh[i])
            {
                frequencyBandHigh[i] = frequencyBands[i];
            }
            audioBand[i] = frequencyBands[i] / frequencyBandHigh[i];
            audioBandBuffer[i] = bandBuffer[i] / frequencyBandHigh[i];
        }
    }

    void BandBuffer()
    {
        for(int g = 0; g< 8; g++)
        {
            if (frequencyBands[g] > bandBuffer[g])
            {
                bandBuffer[g] = frequencyBands[g];
                BufferDecrease[g] = 0.005f;
            }
            if (frequencyBands[g] < bandBuffer[g])
            {
                bandBuffer[g] -= BufferDecrease[g];
                BufferDecrease[g] *= 1.2f;
            }
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(audioSamplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(audioSamplesRight, 1, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        float average = 0;
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += (audioSamplesLeft[count] + audioSamplesRight[count]) * (count + 1);
                count++;
            }
            average /= count;
            frequencyBands[i] = average * 10;
        }
    }
}
