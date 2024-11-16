using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeCamera : MonoBehaviour
{

    // https://www.youtube.com/watch?v=tu-Qe66AvtY
    // Thanks <3

    [SerializeField] private float maxShakeAngle = 10;
    [SerializeField] private float maxShakeOffsetX = 0.75f;
    [SerializeField] private float maxShakeOffsetY = 1;
    private float trauma = 0;
    [SerializeField] float traumaReductionPerSecond = 2.0f;
    private int seed = 9381784;

    private Vector3 startPos;
    private Quaternion startRot;

    //private static ScreenShakeCamera Inst { get { return _instance; } private set { _instance = value; } }
    private static ScreenShakeCamera _instance;


    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public static void TryAddShake(float addTrauma)
    {
        if (_instance != null)
        {
            _instance.AddShake(addTrauma);
        }
    }

    private void AddShake(float addTrauma)
    {
        trauma += addTrauma;
    }

    // Update is called once per frame
    void Update()
    {

        trauma = Mathf.Max(trauma - (traumaReductionPerSecond * Time.deltaTime), 0);
        float shake = trauma * trauma;

        float angle = maxShakeAngle * shake * GetRandomValue(0);
        float offsetX = maxShakeOffsetX * shake * GetRandomValue(1);
        float offsetY = maxShakeOffsetY * shake * GetRandomValue(2);

        transform.position = new Vector3(startPos.x + offsetX, startPos.y + offsetY, transform.position.z);
        //transform.rotation = Quaternion.Euler(0, 0, angle + startRot.eulerAngles.z);
    }

    private float GetRandomValue(float offset)
    {
        return Random.Range(-1.0f, 1.0f);//(Mathf.PerlinNoise(seed, Time.realtimeSinceStartup)) - 1;
    }



}
