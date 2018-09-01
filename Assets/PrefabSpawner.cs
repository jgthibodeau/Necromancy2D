using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner: MonoBehaviour {
    public List<GameObject> spawnables;
    public List<GameObject> spawnedObjects;

    public int initialSpawnCount;

    public int maxToSpawn;
    public float coolDown;
    public float currentCoolDown;

    public float maxX, maxY, minX, minY;

    void Start()
    {
        for(int i=0; i< initialSpawnCount; i++)
        {
            Spawn(true);
        }
    }

    void Update()
    {
        spawnedObjects.RemoveAll(item => item == null);

        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
    }

    public bool CanSpawn()
    {
        return currentCoolDown <= 0 && spawnedObjects.Count < maxToSpawn;
    }

    public bool Spawn(int index)
    {
        return Spawn(index, false);
    }

    private bool Spawn(int index, bool overrideSpawnCheck)
    {
        if (!overrideSpawnCheck && !CanSpawn())
        {
            return false;
        }

        if (index >= spawnables.Count)
        {
            index = spawnables.Count - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        GameObject inst = GameObject.Instantiate(spawnables[index]);

        Vector3 pos = inst.transform.position;
        pos.x = Random.Range(minX, maxX);
        pos.y = Random.Range(minY, maxY);
        pos.z = 0;

        inst.transform.position = pos;
        Vector3 rot = inst.transform.eulerAngles;
        rot.z = Random.Range(0, 360);
        inst.transform.eulerAngles = rot;

        spawnedObjects.Add(inst);

        currentCoolDown = coolDown;

        return true;
    }

    public bool Spawn()
    {
        return Spawn(false);
    }

    private bool Spawn(bool overrideSpawnCheck)
    {
        int index = Random.Range(0, spawnables.Count);
        return Spawn(index, overrideSpawnCheck);
    }
}
