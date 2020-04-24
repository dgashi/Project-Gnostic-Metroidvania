using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnBlock : MonoBehaviour
{
    public GameObject blockPrefab;
    private GameObject block;


    private void Start()
    {
        block = Instantiate(blockPrefab);
    }

    public void SpawnBlock()
    {
        block.SetActive(false);
        block.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        block.SetActive(true);
    }
}
