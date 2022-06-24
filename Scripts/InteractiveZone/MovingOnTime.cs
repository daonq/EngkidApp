using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingOnTime : MonoBehaviour
{

    float time;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float timeToReachTarget;

    Vector3 start;
    Vector3 end;

    // Start is called before the first frame update
    void Start()
    {
        start = Camera.main.transform.InverseTransformPoint(startPosition);
        end = Camera.main.transform.InverseTransformPoint(endPosition);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(start, end, time);
    }

    public void SetDestination()
    {
        time = 0;
    }
}
