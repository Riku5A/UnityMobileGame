using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeupMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Sriding());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Sriding()
    {
        float startTime = Time.time;
        float endTime = startTime + 1f;
        do
        {
            //現在の時間の割合
            float timeRate = (Time.time - startTime) / 1f;
            float updateValue = (40 * timeRate);
            transform.position += new Vector3(updateValue,0f,0f);
            yield return null;
        } while(Time.time < endTime);
    }
}
