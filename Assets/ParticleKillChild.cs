using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKillChild : MonoBehaviour {
    public ParticleKillParent particleKillParent;

    public void OnParticleSystemStopped()
    {
        particleKillParent.ChildDead();
    }
}
