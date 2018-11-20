using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(FadeAudio))]
[RequireComponent(typeof(AudioSource))]
public class SummonCircle : MonoBehaviour {
    private OutlineController outlineController;
    private Player player;
    public Image skeletonSummonImage;

    public SummonCount summonCount;

    public FadeAudio fadeAudio;
    
    public int maxSummons;
    public List<Enemy> summons = new List<Enemy>();

    public bool summonEnabled;
    public bool corpseExplosion;
    public bool corpseExplosionTriggered;
    public bool attack;

    public GameObject summonEffectEnabled;
    public GameObject corpseExplosionEffectEnabled;
    public GameObject captureEffectDisabled;

    public float circleRange;

    public float timeToSummonSkeletons;
    public int numberSkeletonsToSummon;
    public int numberSkeletonsToSummonAtOnce = 1;
    private float currentTimeToSummonSkeletons;
    private bool summoningSkeletons;
    public float minSkeletonRadius, maxSkeletonRadius;
    public GameObject skeletonPrefab;

    public AudioClip summonClip, skeletonClip;
    private AudioSource audioSource;

    public GameObject corpseExplosionPrefab;
    
    // Use this for initialization
    void Start ()
    {
        player = MyGameManager.instance.GetPlayer();
        outlineController = MyGameManager.instance.GetOutlineController();
        fadeAudio = GetComponent<FadeAudio>();
        audioSource = GetComponent<AudioSource>();
        
        ReadSummonSpots();
    }
    
    public List<SummonSpotGroup> summonSpotGroups;
    public List<SummonSpotGroup> summonSpotAttackGroups;
    private SummonSpotGroup currentSummonSpotGroup;
    public int currrentSummonSpotGroupIndex = 0;
    public List<int> maxSummonsPerGroup;
    void ReadSummonSpots()
    {
        maxSummonsPerGroup = new List<int>(summonSpotGroups.Count + summonSpotAttackGroups.Count);

        foreach (SummonSpotGroup summonSpotGroup in summonSpotGroups)
        {
            summonSpotGroup.Initialize();
            maxSummonsPerGroup.Add(summonSpotGroup.SummonSpotCount());
        }
        foreach (SummonSpotGroup summonSpotGroup in summonSpotAttackGroups)
        {
            summonSpotGroup.Initialize();
            maxSummonsPerGroup.Add(summonSpotGroup.SummonSpotCount());
        }

        int prevMax = maxSummonsPerGroup[0];
        for (int i=0; i< maxSummonsPerGroup.Count; i++) {
            int max = maxSummonsPerGroup[i];
            if (prevMax != max)
            {
                Debug.LogError("Summon spots groups have different count of spots" + prevMax + " " + max);
                Application.Quit();
            }
        }

        currentSummonSpotGroup = summonSpotGroups[0];
    }

    public void SetSummonSpotGroup(int i, bool attack)
    {
        if (i >= summonSpotGroups.Count)
        {
            i = summonSpotGroups.Count - 1;
        } else if (i < 0)
        {
            i = 0;
        }
        //Debug.Log("Settings summon spot group to " + i + " " + attack);

        SummonSpotGroup newGroup;
        if (attack)
        {
            newGroup = summonSpotAttackGroups[i];
        } else
        {
            newGroup = summonSpotGroups[i];
        }

        if (newGroup != currentSummonSpotGroup)
        {
            currentSummonSpotGroup.MoveTo(newGroup, attack);
            currentSummonSpotGroup = newGroup;
            currrentSummonSpotGroupIndex = i;
        }
    }

    public bool generateSummonSpots = false;
    public Transform initialSummonSpot;
    public float summonSpotRowDistance = 3;
    public float summonSpotDistance = 3;
    public float summonSpotYOffset = -1;
    public int summonSpotYOffsetPower = 2;
    public int numberOfSummonSpotRows = 4;
    public int summonSpotsInInitialRow = 1;
    public int numberOfExtraSummonSpotsPerRow = 2;
    public int maxSummonSpotsPerRow = 10;
    public int currrentSummonSpotGroup;
    public GameObject summonSpotRowPrefab;
    public GameObject summonSpotPrefab;
    
    SummonSpot GetFreeSummonSpot()
    {
        return currentSummonSpotGroup.GetFreeSummonSpot();
    }

    public bool attacking = false;
    public void Attack()
    {
        attacking = true;
        SetSummonSpotGroup(currrentSummonSpotGroupIndex, true);
    }

    public void StopAttack()
    {
        attacking = false;
        SetSummonSpotGroup(currrentSummonSpotGroupIndex, false);
    }

    int SummonCount()
    {
        return currentSummonSpotGroup.FilledSpotCount();
    }

    // Update is called once per frame
    Enemy previousClosestCorpse;
    void Update ()
    {
        summonCount.maxSummons = maxSummonsPerGroup[0];
        summonCount.SetSummonCount(SummonCount());

        //GenerateSummonSpots();
        MoveSummonCircle();

        player.GetAnimator().SetBool("Summon", false);
        //if (Util.GetButtonDown("SummonGroup1"))
        //{
        //    SetSummonSpotGroup(0);
        //}
        //if (Util.GetButtonDown("SummonGroup2"))
        //{
        //    SetSummonSpotGroup(1);
        //}
        if (Util.GetButton("Shield"))
        {
            SetSummonSpotGroup(1, attacking);
        }
        else
        {
            SetSummonSpotGroup(0, attacking);
        }

        if (!player.IsDead())
        {
            summonEnabled = Util.GetButton("Summon");
            //corpseExplosionTriggered = Util.GetButtonDown("CorpseExplosion");
            //corpseExplosion = Util.GetButton("CorpseExplosion");
            if (Util.GetButton("CorpseExplosion"))
            {
                if (!corpseExplosionTriggered && !corpseExplosion)
                {
                    corpseExplosion = true;
                    corpseExplosionTriggered = true;
                } else
                {
                    corpseExplosion = true;
                    corpseExplosionTriggered = false;
                }
            } else
            {
                corpseExplosion = false;
                corpseExplosionTriggered = false;
            }

            if (summonEnabled)
            {
                foreach(Enemy enemy in overlayedEnemies)
                {
                    if (CanSummon(enemy))
                    {
                        Debug.Log("Summoning " + enemy);
                        Summon(enemy);
                        overlayedEnemies.Remove(enemy);
                        break;
                    }
                }
            }
            Enemy closestCorpse = null;
            float distance = Mathf.Infinity;
            foreach (Enemy enemy in overlayedEnemies)
            {
                if (CanExplode(enemy))
                {
                    float currentDistance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestCorpse = enemy;
                    }
                }
            }

            if (closestCorpse != previousClosestCorpse)
            {
                if (previousClosestCorpse != null)
                {
                    previousClosestCorpse.RemoveDeadOutlineColor();
                }
                if (closestCorpse != null)
                {
                    closestCorpse.SetDeadOutlineColor(2);
                }
                previousClosestCorpse = closestCorpse;
            }

            if (closestCorpse != null)
            {

                if (corpseExplosionTriggered)
                {
                    player.GetAnimator().SetTrigger("Explode");
                    Debug.Log("Exploding " + closestCorpse);
                    closestCorpse.Explode();
                    overlayedEnemies.Remove(closestCorpse);
                }
            }
        }
        else
        {
            summonEnabled = false;
        }

        fadeAudio.enabled = summonEnabled;

        DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.white, circleRange);
        UpdateGraphics();
        
        CleanNullSummons();

        //List<GameObject> summonGameObjects = summons.ConvertAll(new System.Converter<Enemy, GameObject>(EnemyToObject));
        //outlineController.SetObjects(OutlineController.HIGHLIGHT_TYPE.RESSURECTED, summonGameObjects);

        if (summonEnabled && summons.Count < numberSkeletonsToSummon)
        {
            if (!summoningSkeletons)
            {
                summoningSkeletons = true;
                currentTimeToSummonSkeletons = timeToSummonSkeletons;
            }
            SummonSkeletons();
        } else
        {
            summoningSkeletons = false;
        }

        if (summoningSkeletons)
        {
            skeletonSummonImage.fillAmount = 1f - currentTimeToSummonSkeletons / timeToSummonSkeletons;
        } else
        {
            skeletonSummonImage.fillAmount = 0;
        }
    }

    public static GameObject EnemyToObject(Enemy e)
    {
        return e.gameObject;
    }

    public bool HasSummons()
    {
        return summons.Count > 0;
    }

    void SummonSkeletons()
    {
        if (currentTimeToSummonSkeletons > 0)
        {
            currentTimeToSummonSkeletons -= Time.deltaTime;
        } else
        {
            currentTimeToSummonSkeletons = timeToSummonSkeletons;
            for (int i=0; i< numberSkeletonsToSummonAtOnce && summons.Count < numberSkeletonsToSummon; i++)
            {
                Vector2 position = (Vector2)transform.position + Random.insideUnitCircle * Random.Range(minSkeletonRadius, maxSkeletonRadius);
                //Vector3 position = summonSpots[i].transform.position;
                //Vector3 rotation = new Vector3(0, 0, Random.Range(0, 360));
                GameObject skeletonInstance = GameObject.Instantiate(skeletonPrefab, position, Quaternion.identity);
                Summon(skeletonInstance.GetComponent<Enemy>());
                audioSource.PlayOneShot(skeletonClip);
            }
        }
    }
    void CleanNullSummons()
    {
        summons.RemoveAll(item => item == null);
    }
    
    void UpdateGraphics()
    {
        captureEffectDisabled.SetActive(!summonEnabled);
        if (summonEnabled)
        {
            corpseExplosionEffectEnabled.SetActive(false);
            summonEffectEnabled.SetActive(true);
            captureEffectDisabled.SetActive(false);
        }
        else if (corpseExplosion)
        {
            corpseExplosionEffectEnabled.SetActive(true);
            summonEffectEnabled.SetActive(false);
            captureEffectDisabled.SetActive(false);
        }
        else
        {
            corpseExplosionEffectEnabled.SetActive(false);
            summonEffectEnabled.SetActive(false);
            captureEffectDisabled.SetActive(true);
        }
    }

    List<Enemy> overlayedEnemies = new List<Enemy>();
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (!overlayedEnemies.Contains(enemy))
        {
            overlayedEnemies.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (overlayedEnemies.Contains(enemy))
        {
            overlayedEnemies.Remove(enemy);
        }
    }
    //void OnTriggerStay2D(Collider2D other)
    //{
    //    Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
    //    if (CanSummon(enemy))
    //    {
    //        Debug.Log("Summoning " + enemy);
    //        Summon(enemy);
    //    }

    //    if (CanExplode(enemy))
    //    {
    //        Debug.Log("Exploding " + enemy);
    //        enemy.Explode();
    //    }
    //}

    bool CanSummon(Enemy enemy)
    {
        Debug.Log("Trying to summon " + enemy);
        return enemy != null && enemy.CanResurrect();
    }

    void Summon(Enemy enemy)
    {
        if (currentSummonSpotGroup.Summon(enemy))
        {
            player.GetAnimator().SetBool("Summon", true);
            summons.Add(enemy);
            enemy.summonCircle = this;

            audioSource.PlayOneShot(summonClip);
        }
        //SummonSpot summonSpot = GetFreeSummonSpot();
        //if (summonSpot != null)
        //{
        //    player.GetAnimator().SetBool("Summon", true);

        //    summons.Add(enemy);
        //    summonSpot.enemy = enemy;
        //    enemy.Resurrect(summonSpot);
        //    enemy.summonCircle = this;

        //    //outlineController.AddObject(OutlineController.HIGHLIGHT_TYPE.RESSURECTED, enemy.gameObject);
        //}
    }

    public void Remove(Enemy enemy)
    {
        if (summons.Contains(enemy))
        {
            summons.Remove(enemy);
        }
    }

    bool CanExplode(Enemy enemy)
    {
        Debug.Log("Trying to explode " + enemy);
        return enemy != null && enemy.CanExplode();
    }
    
    public float axisAmount = 5f;
    void MoveSummonCircle()
    {
        Vector3 position = transform.position;

        if (Util.GetCurrentInputDeviceType() == TeamUtility.IO.InputDevice.Joystick)
        {
            Vector2 joystick = new Vector2(Util.GetAxis("Horizontal Right"), Util.GetAxis("Vertical Right"));
            joystick = Vector2.ClampMagnitude(joystick, 1);
            position.x = player.transform.position.x + joystick.x * axisAmount;
            position.y = player.transform.position.y + joystick.y * axisAmount;
        }
        else
        {
            Vector3 mouse = Util.MouseInWorld();
            position.x = mouse.x;
            position.y = mouse.y;
        }

        transform.position = position;
    }
}
