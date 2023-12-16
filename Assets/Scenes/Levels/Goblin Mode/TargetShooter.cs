using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShooter : MonoBehaviour
{
    [SerializeField] Camera cam;

    public float hits = 0;
    public bool win = false;
    public GameObject youWinText;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                Target target = hit.collider.gameObject.GetComponent<Target>();
                if(target != null)
                {
                    hits++;
                    if(hits > 15)
                    {
                        win = true;
                        youWinText.SetActive(true);
                        Invoke(nameof(TriggerWin), 1f);
                    }
       
                    audioSource.Play();
                    target.Hit();
                }
            }
        }
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }
}
