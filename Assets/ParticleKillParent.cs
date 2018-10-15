using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKillParent : MonoBehaviour {
    List<ParticleKillChild> particleKillChildren;
    public int numberOfChildren;

    void Start()
    {
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule mm = ps.main;
            mm.stopAction = ParticleSystemStopAction.Callback;

            ParticleKillChild particleKillChild = ps.GetComponent<ParticleKillChild>();
            if (particleKillChild == null)
            {
                particleKillChild = ps.gameObject.AddComponent<ParticleKillChild>();
            }
            particleKillChild.particleKillParent = this;
            numberOfChildren++;
        }
    }

    public void ChildDead()
    {
        numberOfChildren--;
        if (numberOfChildren <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
