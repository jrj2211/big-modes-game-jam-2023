using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleData
{
    public GameObject prefab;
    public bool active;

    [HideInInspector]
    public GameObject instance;
}

[System.Serializable]
public class VehicleTransition
{
    public GameObject animation;
    public int from;
    public int to;
}

public class VehicleSwitcher : MonoBehaviour
{
    [SerializeField]
    private VehicleData[] vehicles;

    [SerializeField]
    private CameraController cameraController;

    [HideInInspector]
    private GameObject currentVehicle;
    private int currentVehicleId;

    public VehicleTransition[] transitions;

    public GameObject[] instances;

    // Start is called before the first frame update
    void Start() 
    {
        instances = new GameObject[vehicles.Length];

        // Create instances from the prefabs
        for (int i = 0; i < vehicles.Length; i++) {
            VehicleData vehicle = vehicles[i];

            vehicle.instance = Instantiate(vehicle.prefab, transform.position, Quaternion.identity);
            vehicle.instance.SetActive(false);
            vehicle.instance.GetComponent<FuelController>().CreateUI();
            vehicle.instance.transform.SetParent(this.transform);

            instances[i] = vehicle.instance;
        }
    }

    public void activateById(int id)
    {
        GameObject nextVehicle = instances[id];

        // Switching to same vehicle so ignore
        if (nextVehicle == currentVehicle)
        {
            return;
        }

        // Activate the new vehicle
        nextVehicle.SetActive(true);

        VehicleTransition transition = getTransition(currentVehicleId, id);

        if(transition != null)
        {
            GameObject animation = Instantiate(transition.animation, Vector2.zero, Quaternion.identity);
            animation.transform.SetParent(nextVehicle.transform);
        }

        if (currentVehicle)
        {
            // Remove previous vehicle
            Vector2 velocity = currentVehicle.GetComponent<Rigidbody2D>().velocity;

            // Copy over position and rotation from previous vehicle
            nextVehicle.transform.localPosition = currentVehicle.transform.localPosition;
            nextVehicle.transform.localRotation = currentVehicle.transform.localRotation;
            nextVehicle.GetComponent<Rigidbody2D>().angularVelocity = 0;

            // Apply the force from the previous vehicle
            nextVehicle.GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);

            // Set previous vehicle as inactive
            currentVehicle.SetActive(false);
        }

        // Update to new vehicle
        currentVehicle = nextVehicle;

        // All behavior when vehicle becomes active
        currentVehicle.GetComponent<Vehicle>().OnSwitch();

        // Update camera target
        cameraController.target = currentVehicle;

        // Save the vehicle ID
        currentVehicleId = id;
    }

    VehicleTransition getTransition(int from, int to)
    {
        foreach(VehicleTransition transition in transitions)
        {
            if(transition.from == from && transition.to == to)
            {
                return transition;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Switch to next vehicle when Enter is pressed
            int nextVehicleId = currentVehicleId + 1;
            if(nextVehicleId >= vehicles.Length)
            {
                nextVehicleId = 0;
            }
            activateById(nextVehicleId);
        }
    }

    public int GetInitialVehicle()
    {
        // Find vehicle set as the active default
        for (int i = 0; i < vehicles.Length; i++)
        {
            VehicleData vehicle = vehicles[i];
            if (vehicle.active)
            {
                return i;
            }
        }

        return 0;
    }

    public GameObject ActiveVehicle()
    {
        return currentVehicle;
    }

    public void Restart(Vector3 position)
    {
        // Reset all vehicles
        foreach (GameObject vehicle in instances)
        {
            vehicle.GetComponent<Vehicle>().Restart();
        }

        // Disable current vehicle if active
        if(currentVehicle)
        {
            currentVehicle.SetActive(false);
            currentVehicle = null;
        }
     
        // Find the initial vehicle and activate it
        int initalVehicle = GetInitialVehicle();
        activateById(initalVehicle);
        currentVehicle.transform.position = position;
        currentVehicle.SetActive(true);
    }
}
