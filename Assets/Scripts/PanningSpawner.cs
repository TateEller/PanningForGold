using UnityEngine;
using System.Collections.Generic;

public class PanningSpawner : MonoBehaviour
{
    public List<GameObject> panningSpots = new List<GameObject>();
    public int activePanningSpots = 0;

    private void Start()
    {
        foreach(GameObject s in panningSpots)
        {
            s.GetComponent<Animator>().SetBool("canPan", false);
        }
    }

    private void Update()
    {
        if(activePanningSpots < 3)
        {
            ActivatePanningSpot();
        }
    }
    public void ActivatePanningSpot()
    {
        int ran = Random.Range(0, panningSpots.Count);
        panningSpots[ran].GetComponent<Animator>().SetBool("canPan", true);
        activePanningSpots++;
    }

    public void DeactivatePanningSpot(GameObject spot)
    {
        if (panningSpots.Contains(spot))
        {
            spot.GetComponent<Animator>().SetBool("canPan", false);
            activePanningSpots--;
        }
    }
}
