using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : MonoBehaviour
{
    public float nbTO;
    public float nbTP;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (nbTP < 2)
            {
                nbTP += 1;
            }
            else UIManager.Instance.Win();
        }

        if (other.gameObject.tag == "Opponent")
        {
            if (nbTO < 2)
            {
                nbTO += 1;
            }
            else UIManager.Instance.Loose();
        }
    }
}
