using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;

    VehicleSwitcher switcher;

    float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        switcher = GetComponent<VehicleSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = Mathf.Lerp(speed, switcher.ActiveVehicle().GetComponent<Vehicle>().GetSpeed(), Time.deltaTime * 5);
        speedText.text = Mathf.Round(speed).ToString();
    }
}
