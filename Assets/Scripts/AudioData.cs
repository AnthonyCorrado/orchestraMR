using UnityEngine;
using System.Collections;

public class AudioData : MonoBehaviour {

    public float rmsValue;
    public float dbValue;
    public float pitchValue;

    int qSamples = 64;
    float refValue = 0.1f;
    float threshold = 0.02f;

    float[] _samples;
    float[] _spectrum;
    float _fSample;

    AudioSource audioSource;

    int debugCount = 0;
    //public float sensitivity = 100.0f;
    //public float loudness = 0.0f;
    //public float frequency = 0.0f;
    //public float threshold = 1.0f;
    //public int note = 0;
    //public int sampleRate = 11024;

    //float outputSampleRate;

    string[] notesArray = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // Use this for initialization
    void Start() {
        _samples = new float[qSamples];
        _spectrum = new float[qSamples];
        _fSample = AudioSettings.outputSampleRate;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        debugCount++;
        AnalyzeSound();
        //loudness = GetAveragedVolume() * sensitivity;
        //frequency = GetFundamentalFrequency();
        //note = (int)GetNote();

        //Debug.Log("note: " + notesArray[note % 12]);
        // Debug.Log("frequency: " + frequency);
    }

    void AnalyzeSound()
    {
        audioSource.GetOutputData(_samples, 0);
        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum = +_samples[i] * _samples[i];
        }
        //Debug.Log("sum is this: " + sum);
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160)
        {
            dbValue = -160;
        }

        audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float note;
        float maxV = 0;
        var maxN = 0;
        for (int i = 0; i <qSamples; i++)
        {
            //Debug.Log("spec: " + _spectrum[i]);
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > threshold))
            {
                continue;
            }
            maxV = _spectrum[i];
            maxN = i;

        }
        float freqN = maxN;
        if (maxN > 0 && maxN < qSamples - 1)
        {
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);

            pitchValue = freqN * (_fSample / 2) / qSamples;
        }
        if (debugCount % 10 == 0)
        {
            //note = GetNote(pitchValue);
            Debug.Log("note value: " + pitchValue);
        }
        //GetNote(pitchValue);
    }
    //float GetAveragedVolume()
    //{
    //    float[] data = new float[256];
    //    float a = 0;
    //    audioSource.GetOutputData(data, 0);
    //    foreach(float s in data)
    //    {
    //        a += Mathf.Abs(s);
    //    }
    //    return a / 256;
    //}

    //float GetFundamentalFrequency()
    //{
    //    float fundamentalFrequency = 0.0f;
    //    float[] spectrum = new float[256];
    //    Debug.Log("pre: " + spectrum);
    //    audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

    //    Debug.Log("post: " + spectrum);
    //    float s = 0.0f;
    //    int i = 0;

    //    for(int j = 1; j < spectrum.Length; j++)
    //    {

    //        //Debug.Log("spectrum return: " + spectrum[j]);
    //        //if (s < spectrum[j])
    //        //{
    //        //    s = spectrum[j];
    //        //    i = j;
    //        //}
    //    }
    //    fundamentalFrequency = i * sampleRate / spectrum.Length;
    //    return fundamentalFrequency;
    //}

    float GetNote(float frequency)
    {
        var roundedNoteFreq = (12 * Mathf.Log(frequency / 440f)) / Mathf.Log(2);
        return roundedNoteFreq;
    }
}
