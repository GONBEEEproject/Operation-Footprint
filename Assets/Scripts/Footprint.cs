using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Footprint : MonoBehaviour
{
    [SerializeField]
    private Transform cube;

    private float footLevel;
    private float threshold = 5;
    private float freez;

    private const float FREEZTIME = 0.3f;
    private const int COUNT = 3;

    private AudioSource source;
    [SerializeField]
    private AudioClip normal, beam, bupigan;

    [SerializeField]
    private Text nowLevel, setLevel;

    private Queue<float> sqrAccels = new Queue<float>();

    private void Start()
    {
        Input.gyro.enabled = true;
        setLevel.text = threshold.ToString("0.000");

        source = GetComponent<AudioSource>();

        for (int i = 0; i < COUNT; i++)
        {
            sqrAccels.Enqueue(0);
        }
    }

    private void Update()
    {
        float x = Input.acceleration.x;
        float y = Input.acceleration.y;
        float z = Input.acceleration.z;

        sqrAccels.Dequeue();
        sqrAccels.Enqueue(Input.acceleration.sqrMagnitude);

        cube.rotation = Input.gyro.attitude;

        Calc();

        Check();
    }

    private void Calc()
    {
        footLevel = sqrAccels.Average();
        nowLevel.text = footLevel.ToString("0.000");
    }

    private void Check()
    {
        if (freez >= 0)
        {
            freez -= Time.deltaTime;
            return;
        }
        if (footLevel >= threshold)
        {
            Shot();
            freez = FREEZTIME;
        }
    }

    public void OnSlide(float value)
    {
        threshold = value;
        setLevel.text = threshold.ToString("0.000");
    }

    public void Shot()
    {
        source.PlayOneShot(source.clip);
    }

    public void Normal(bool isOn)
    {
        if (isOn) source.clip = normal;
    }

    public void Beam(bool isOn)
    {
        if (isOn) source.clip = beam;
    }

    public void Bupigan(bool isOn)
    {
        if (isOn) source.clip = bupigan;
    }
}