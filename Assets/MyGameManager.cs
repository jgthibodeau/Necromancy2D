﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    public static MyGameManager instance = null;
    private Player player;
    private OutlineController outlineController;

    public int maxInstances = 50;
    public enum InstanceableType { BULLET, MISSLE, DEBRIS, PICKUP, EXPLOSION }
    public Dictionary<InstanceableType, List<GameObject>> instanceMap;

    int mapIndexes = 0;

    public bool isPaused = false;

    public int GetIndex()
    {
        return mapIndexes++;
    }

	void Awake(){
		if (instance == null) {
			instance = this;
            instanceMap = new Dictionary<InstanceableType, List<GameObject>> ();
        } else if (instance != this) {
			Destroy (gameObject);
		}
    }

	public void AddInstance(InstanceableType mapIndex, GameObject instanceObj) {
        List<GameObject> instances;
        if (!instanceMap.TryGetValue(mapIndex, out instances))
        {
            instances = new List<GameObject>();
            instanceMap.Add(mapIndex, instances);
        }
        
        int numberInstances = instances.Count;
		if (numberInstances > maxInstances) {
			int index = Random.Range (0, numberInstances);
			GameObject removedInstance = instances [index];
			instances.RemoveAt (index);
			GameObject.Destroy (removedInstance);
		}
		instances.Add (instanceObj);
	}

	public void RemoveInstance(InstanceableType mapIndex, GameObject instanceObj) {
        List<GameObject> instances;
        if (instanceMap.TryGetValue(mapIndex, out instances))
        {
            if (instances.Contains(instanceObj))
            {
                instances.Remove(instanceObj);
            }
            GameObject.Destroy(instanceObj);
        }
    }

    public Player GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        return player;
    }

    public OutlineController GetOutlineController()
    {
        if (outlineController == null)
        {
            outlineController = Camera.main.GetComponent<OutlineController>();
        }
        return outlineController;
    }

    public bool CanPause()
    {
        return !LevelManager.instance.IsPlayerDead();
    }
}

