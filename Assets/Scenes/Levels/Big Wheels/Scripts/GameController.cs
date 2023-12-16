using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum State {
        WIN, LOSE, ACTIVE
    }

    private VehicleSwitcher switcher;

    public LayerMask winLayer;

    public GameObject winScreen;
    public GameObject loseScreen;

    public Transform startPosition;

    private State state = State.ACTIVE;

    // Start is called before the first frame update
    void Start()
    {
        switcher = GetComponent<VehicleSwitcher>();

        switcher.Restart(startPosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.ACTIVE)
        {
            if (switcher.ActiveVehicle() && switcher.ActiveVehicle().GetComponent<Rigidbody2D>().IsTouchingLayers(winLayer))
            {
                ShowWin();
            }
            else if (switcher.ActiveVehicle().GetComponent<Vehicle>().IsDamaged())
            {
                ShowLose();
            }

            bool noneWithFuel = true;
            foreach (GameObject instance in switcher.instances)
            {
                Vehicle vehicle = instance.GetComponent<Vehicle>();
                if (vehicle.fuel && vehicle.fuel.IsNotEmpty())
                {
                    noneWithFuel = false;
                }
            }

            if(noneWithFuel)
            {
                Invoke("ShowLose", 5f);
            }
        }
    }

    private void ShowLose()
    {
        if(state == State.ACTIVE)
        {
            state = State.LOSE;
            loseScreen.SetActive(true);

            Invoke(nameof(TriggerLose), 1f);
        }
        
    }

    private void ShowWin()
    {
        if (state == State.ACTIVE)
        {
            state = State.WIN;
            winScreen.SetActive(true);

            Invoke(nameof(TriggerWin), 1f);
        }
    }

    private void Restart()
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);

        switcher.Restart(startPosition.position);

        Invoke("ResetState", 1f);
    }

    private void ResetState()
    {
        state = State.ACTIVE;
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }

    void TriggerLose()
    {
        LevelManager.GetInstance().Lose();
    }
}
