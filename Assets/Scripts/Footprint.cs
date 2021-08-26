using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Footprint : MonoBehaviour
{
    [SerializeField]
    private Transform cube;

    private float footX, footY, footZ;
    private float thresX = 2.5f;
    private float thresY = 2.5f;
    private float thresZ = 2.5f;

    private float nowFreeze;

    private float freezeTime = 0.1f;
    private const int COUNT = 2;

    private AudioSource source;
    [SerializeField]
    private AudioClip[] sounds;

    [SerializeField]
    private Text nowX, nowY, nowZ, setX, setY, setZ, freezeText;

    private Queue<float> accelsX = new Queue<float>();
    private Queue<float> accelsY = new Queue<float>();
    private Queue<float> accelsZ = new Queue<float>();

    public enum Mode
    {
        AND, OR
    }

    private Mode nowMode = Mode.AND;

    private void Start()
    {
        Input.gyro.enabled = true;

        SlideX(2.5f);
        SlideY(2.5f);
        SlideZ(2.5f);

        SlideFreeze(0.2f);

        source = GetComponent<AudioSource>();

        for (int i = 0; i < COUNT; i++)
        {
            accelsX.Enqueue(0);
            accelsY.Enqueue(0);
            accelsZ.Enqueue(0);
        }
    }

    private void Update()
    {
        float x = Mathf.Abs(Input.acceleration.x);
        float y = Mathf.Abs(Input.acceleration.y);
        float z = Mathf.Abs(Input.acceleration.z);

        accelsX.Dequeue();
        accelsX.Enqueue(x);

        accelsY.Dequeue();
        accelsY.Enqueue(y);

        accelsZ.Dequeue();
        accelsZ.Enqueue(z);

        cube.rotation = Input.gyro.attitude;

        Calc();

        Check();
    }

    private void Calc()
    {
        footX = accelsX.Average();
        footY = accelsY.Average();
        footZ = accelsZ.Average();

        nowX.text = footX.ToString("0.000");
        nowY.text = footY.ToString("0.000");
        nowZ.text = footZ.ToString("0.000");
    }

    private void Check()
    {
        if (nowFreeze >= 0)
        {
            nowFreeze -= Time.deltaTime;
            return;
        }

        switch (nowMode)
        {
            case Mode.AND:
                if (footX >= thresX && footY >= thresY && footZ >= thresZ)
                {
                    Shot();
                    nowFreeze = freezeTime;
                }
                break;
            case Mode.OR:
                if (footX >= thresX || footY >= thresY || footZ >= thresZ)
                {
                    Shot();
                    nowFreeze = freezeTime;
                }
                break;
        }
    }

    public void SlideX(float value)
    {
        thresX = value;
        setX.text = thresX.ToString("0.000");
    }

    public void SlideY(float value)
    {
        thresY = value;
        setY.text = thresY.ToString("0.000");
    }

    public void SlideZ(float value)
    {
        thresZ = value;
        setZ.text = thresZ.ToString("0.000");
    }

    public void SlideFreeze(float value)
    {
        freezeTime = value;
        freezeText.text = freezeTime.ToString("0.000");
    }

    [ContextMenu("Shot")]
    public void Shot()
    {
        source.PlayOneShot(source.clip);
    }

    public void ChangeSound(int i)
    {
        source.clip = sounds[i];
    }

    public void ChangeMode(int i)
    {
        nowMode = (Mode)i;
    }
}