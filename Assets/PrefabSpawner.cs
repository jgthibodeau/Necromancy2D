using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner: MonoBehaviour
{
    [System.Serializable]
    public struct SpawnWeight
    {
        public GameObject spawnable;
        [Range(0,100)]
        public int weight;
    }
    public SpawnWeight[] spawnWeights;
    private GameObject[] spawnables;
    private int[] weights;

    public List<GameObject> spawnedObjects;

    public bool spawnOnTimer = true;
    public int initialSpawnCount;

    [Range(1, 100)]
    public int spawnCountWhenZero;

    public int maxToSpawn;
    public float coolDown;
    public float currentCoolDown;

    public float minSpawnRadius, maxSpawnRadius;

    public bool canSpawnInCamera;

    private bool spawning;

    public bool active;

    void Start()
    {
        spawnables = new GameObject[spawnWeights.Length];
        weights = new int[spawnWeights.Length];
        for (int i = 0; i < spawnWeights.Length; i++)
        {
            spawnables[i] = spawnWeights[i].spawnable;
            weights[i] = spawnWeights[i].weight;
        }
        
        SpawnAmount(initialSpawnCount, true);
    }

    void Update()
    {
        DebugExtension.DebugWireSphere(transform.position, Color.blue, minSpawnRadius);
        DebugExtension.DebugWireSphere(transform.position, Color.blue, maxSpawnRadius);

        CleanNullSpawns();

        if (spawnOnTimer)
        {
            SpawnOnTimer();
        }
    }

    public int SpawnCount()
    {
        return spawnedObjects.Count;
    }

    void CleanNullSpawns()
    {
        spawnedObjects.RemoveAll(item => item == null || IsDeadEnemy(item));
    }

    bool IsDeadEnemy(GameObject go)
    {
        return go.GetComponent<Enemy>() != null && !go.GetComponent<Enemy>().IsAlive();
    }

    void SpawnOnTimer()
    {
        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
        else if (CanSpawn())
        {
            int spawnTimes = 1;
            if (spawnedObjects.Count == 0)
            {
                spawnTimes = spawnCountWhenZero;
            }

            Spawn(spawnTimes, false);
        }
    }

    public bool CanSpawn()
    {
        //Debug.Log("spawning " + spawning + ", cooldown ok " + (currentCoolDown <= 0) + ", count ok " + (spawnedObjects.Count < maxToSpawn) + ", can spawn "+(!spawning && currentCoolDown <= 0 && spawnedObjects.Count < maxToSpawn));
        return active && !spawning && currentCoolDown <= 0 && spawnedObjects.Count < maxToSpawn;
    }

    public void SpawnAmount(int count, bool overrideSpawnCheck)
    {
        for (int i = 0; i < count; i++)
        {
            Spawn(overrideSpawnCheck);
        }
    }

    public void Spawn(bool overrideSpawnCheck)
    {
        if (spawnedObjects.Count < maxToSpawn)
        {
            int index = Util.GetRandomWeightedIndex(weights);
            StartCoroutine(Spawn(index, overrideSpawnCheck));
        }
    }
    
    private IEnumerator Spawn(int index, bool overrideSpawnCheck)
    {
        spawning = true;
        currentCoolDown = coolDown;

        if (index >= spawnables.Length)
        {
            index = spawnables.Length - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        Vector2 pos;
        do
        {
            pos = (Vector2)transform.position + Random.insideUnitCircle * Random.Range(minSpawnRadius, maxSpawnRadius);
            yield return null;
        } while (!IsValidSpawnLocation(pos, overrideSpawnCheck));

        //Vector3 rot = new Vector3(0, 0, Random.Range(0, 360));
        //GameObject inst = GameObject.Instantiate(spawnables[index], pos, Quaternion.Euler(rot));
        GameObject inst = GameObject.Instantiate(spawnables[index], pos, Quaternion.identity);

        Debug.Log("spawned " + inst);

        spawnedObjects.Add(inst);

        spawning = false;
    }

    public bool IsValidSpawnLocation(Vector3 pos, bool overrideSpawnCheck)
    {
        if (overrideSpawnCheck || canSpawnInCamera)
        {
            return true;
        }
        Vector3 cameraPoint = Camera.main.WorldToViewportPoint(pos);
        return cameraPoint.x < -0.1f || cameraPoint.x > 1.1f || cameraPoint.y < -0.1f && cameraPoint.y > 1.1f;
    }
}
