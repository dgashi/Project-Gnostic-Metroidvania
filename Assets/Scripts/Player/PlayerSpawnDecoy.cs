using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnDecoy : MonoBehaviour
{
    public GameObject decoyPrefab;
    private GameObject decoy;


    private void Start()
    {
        decoy = Instantiate(decoyPrefab);
    }

    public void SpawnDecoy()
    {
        decoy.SetActive(false);
        decoy.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        decoy.SetActive(true);
    }
}
