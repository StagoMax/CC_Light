using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeControler : MonoBehaviour
{
    [SerializeField]
    private bool streetLampLightEnabled;
    // Start is called before the first frame update
    //时间常数 调整时间变化的速度
    [SerializeField]
    private float timeMultiplier;
    [SerializeField]
    private float startHour;
    [SerializeField]
    private TextMeshProUGUI timetext;
    [SerializeField]
    private float sunriseHour;
    private DateTime currentTime;
    [SerializeField]
    private float sunsetHour;

    [SerializeField]
    private Light sunLight;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;
    private TimeSpan sunriseToSunsetDuration;
    private bool isSunsrise;
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
 
        sunriseTime=TimeSpan.FromHours(sunriseHour);
        sunsetTime=TimeSpan.FromHours(sunsetHour);

       sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSunLight();
    }
    private void UpdateTimeOfDay()
    {
        currentTime=currentTime.AddSeconds(Time.deltaTime*timeMultiplier);
        if(timetext!=null)
        {
            timetext.text = currentTime.ToString("HH:mm:ss");
        }
    }
    private void RotateSunLight()
    {
        float sunLightRotation;
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            isSunsrise = true;
            TimeSpan timSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            isSunsrise =false;
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        if (streetLampLightEnabled)
        {
            GameObject[] streetLamps = GameObject.FindGameObjectsWithTag("StreetLampLight");
            foreach(GameObject streetLamp in streetLamps)
            {
                Light streetLampLight = streetLamp.GetComponent<Light>();
                if (streetLampLight != null)
                {
                    if (isSunsrise)
                    {
                        streetLampLight.enabled = false;
                    }
                    else
                    {
                        streetLampLight.enabled = true;
                    }
                }
            }
        }
        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.up);
        Debug.Log("sunLight rotation  " + sunLight.transform.rotation);
    }
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime,TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;
        Debug.Log($"diff为{diff.ToString()}");
        if (diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24);
        
        }
        return diff;
    }
}
