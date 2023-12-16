using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FuelController : MonoBehaviour
{
    [SerializeField]
    private float total;

    [SerializeField]
    private Color color;

    [SerializeField]
    private string text;

    [SerializeField]
    private GameObject gaugePrefab;

    private Slider slider;

    public float remaining;

    private GameObject gauges;

    public void CreateUI()
    {
        GameObject gauge = Instantiate(gaugePrefab);

        slider = gauge.GetComponent<Slider>();

        Image bar = gauge.transform.Find("Container/Bar").GetComponent<Image>();
        bar.color = color;

        TextMeshProUGUI textUI = gauge.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        textUI.text = text;

        gauges = GameObject.Find("Canvas/UI");

        gauge.transform.SetParent(gauges.transform);
        gauge.transform.localScale = Vector3.one;

        remaining = total;

        Use(0);
    }

    public void Use(float rate)
    {
        remaining -= Mathf.Clamp(Time.deltaTime * rate, 0, total);
        UpdateGauge();
    }   

    public bool IsNotEmpty()
    {
        return remaining > 0;
    }

    public bool IsEmpty()
    {
        return remaining <= 0;
    }

    public void Add(float amount) 
    {
        remaining = Mathf.Clamp(remaining + amount, 0, total);
        UpdateGauge();
    }

    public void Restart()
    {
        remaining = total;
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        slider.value = remaining / total;
    }
}
