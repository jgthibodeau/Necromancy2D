using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner: MonoBehaviour
{
    [System.Serializable]
    public struct SpawnWeight
    {
        public GameObject spawnable;
        public int weight;
    }
    public SpawnWeight[] spawnWeights;
    private GameObject[] spawnables;
    private int[] weights;

    public List<GameObject> spawnedObjects;

    public int initialSpawnCount;

    public int maxToSpawn;
    public float coolDown;
    public float currentCoolDown;

    public float maxX, maxY;

    void Start()
    {
        //public SpawnChance[] spawnableChances;
        //private GameObject spawnables;
        //private GameObject weights;

        spawnables = new GameObject[spawnWeights.Length];
        weights = new int[spawnWeights.Length];
        for (int i=0; i< spawnWeights.Length; i++)
        {
            spawnables[i] = spawnWeights[i].spawnable;
            weights[i] = spawnWeights[i].weight;
        }

        for (int i=0; i< initialSpawnCount; i++)
        {
            Spawn(true);
        }
    }

    void Update()
    {
        Bounds b = new Bounds(transform.position, new Vector3(2 * maxX, 2 * maxY));
        DebugExtension.DebugBounds(b, Color.green);

        spawnedObjects.RemoveAll(item => item == null);

        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        } else
        {
            Spawn();
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

        if (index >= spawnables.Length)
        {
            index = spawnables.Length - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        GameObject inst = GameObject.Instantiate(spawnables[index]);

        Vector3 pos = transform.position;
        pos.x += Random.Range(-maxX, maxX);
        pos.y += Random.Range(-maxY, maxY);
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
        int index = Util.GetRandomWeightedIndex(weights);
        return Spawn(index, overrideSpawnCheck);
    }
}
