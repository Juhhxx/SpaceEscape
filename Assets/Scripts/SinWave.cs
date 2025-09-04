using NaughtyAttributes;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class SinWave : MonoBehaviour
{

    [SerializeField] private LineRenderer sinWaveRenderer;
    [SerializeField] private int points;
    [SerializeField] private Vector2 minMaxAmplitude = new Vector2(0, 2);
    [SerializeField] private float amplitude = 1;
    [SerializeField] private float amplitudeStep = 0.1f;    
    public float Amplitude { get => amplitude; }
    [SerializeField] private Vector2 minMaxFrequency = new Vector2(0, 2);
    [SerializeField] private float frequency = 1;
    [SerializeField] private float frequencyStep = 0.1f;
    public float Frequency { get => frequency; }
    public int shiftMarginOfError = 2;
    [SerializeField] private float shift = 1;
    [SerializeField] private float shiftStep = 0.1f;
    public float Shift { get => shift % (2f * Mathf.PI); }
    [SerializeField] private Vector2 xLimits = new Vector2(0, 1);

    [SerializeField] private bool notTarget;
    [ShowIf("notTarget")]
    [SerializeField] private SinWave targetWave;

    [SerializeField] private int seed = 0;

    //6.28318531

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seed = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second; // Use the current time as a seed
        System.Random random = new System.Random(seed); // 

        if(notTarget)
        {
            random = new System.Random(seed + 1);
        }

        amplitude = GetSeededRandomValue(random, minMaxAmplitude.x, minMaxAmplitude.y, amplitudeStep);
        frequency = GetSeededRandomValue(random, minMaxFrequency.x, minMaxFrequency.y, frequencyStep);
        shift = GetSeededRandomValue(random, 0, 2 * Mathf.PI, shiftStep * (2f * Mathf.PI / 10));


        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        //Draw();
    }

    public void Draw()
    {
        float xStart = xLimits.x;
        float Tau = 2 * Mathf.PI;
        float xFinish = xLimits.y;

        sinWaveRenderer.positionCount = points;
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = amplitude * Mathf.Sin((Tau * frequency * x) + shift);
            sinWaveRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));
        }

        Debug.Log(Shift);
    }

    public void AddAmplitude()
    {
        amplitude += amplitudeStep;

        if(amplitude > minMaxAmplitude.y + 0.1f)
        {
            amplitude = minMaxAmplitude.y;
        }
        Draw();
    }

    public void DecreaseAmplitude()
    {
        amplitude -= amplitudeStep;
        if (amplitude < minMaxAmplitude.x - 0.1f)
        {
            amplitude = minMaxAmplitude.x;
        }
        Draw();
    }

    public void AddFrequency()
    {
        frequency += frequencyStep;
        if (frequency > minMaxFrequency.y + 0.1f)
        {
            frequency = minMaxFrequency.y;
        }
        Draw();
    }

    public void DecreaseFrequency()
    {
        frequency -= frequencyStep;
        if (frequency < minMaxFrequency.x - 0.1f)
        {
            frequency = minMaxFrequency.x;
        }
        Draw();
    }

    public void AddShift()
    {
        shift += shiftStep * (2f * Mathf.PI / 10);
        Draw();
    }

    public void DecreaseShift()
    {
        shift -= shiftStep * (2f * Mathf.PI / 10);
        Draw();
    }

    public void CheckIfTarget()
    {
        
        if(targetWave != null)
        {
            Debug.Log("Amplitude: " + (Mathf.Approximately(Amplitude, targetWave.Amplitude)) + " Amplitude: " + Amplitude + " Amplitude Shift: " + targetWave.Amplitude);
            Debug.Log("Frequency: " + (Mathf.Approximately(Frequency, targetWave.Frequency)) + " Frequency: " + Frequency + " Frequency Shift: " + targetWave.Frequency);
            Debug.Log("Shift: " + (Mathf.Abs(Shift - targetWave.Shift) < (2f * Mathf.PI / 10) * (float)shiftMarginOfError) + " Shift: " + Shift + " Shift Shift: " + targetWave.Shift);

            if ((Mathf.Approximately(Amplitude, targetWave.Amplitude)) && 
                (Mathf.Approximately(Frequency, targetWave.Frequency)) && 
                (Mathf.Abs(Shift - targetWave.Shift) < (2f * Mathf.PI / 10) * (float)shiftMarginOfError))
            {
                Debug.Log("Correct");
            }
            else
            {
                Debug.Log("Incorrect");
            }
        }

    }

    private float GetSeededRandomValue(System.Random random, float min, float max, float step)
    {
        int steps = Mathf.RoundToInt((max - min) / step); // Number of possible steps
        int randomStep = random.Next(0, steps + 1); // Get a random step index
        return min + (randomStep * step); // Convert step index to actual value
    }
}
