using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(FadeAudio))]
public class SummonCircle : MonoBehaviour {
    private OutlineController outlineController;
    private Player player;
    public Image skeletonSummonImage;
    public TextMeshProUGUI text;

    public FadeAudio fadeAudio;

    public int minSummons;
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
    private float currentTimeToSummonSkeletons;
    private bool summoningSkeletons;
    public float minSkeletonRadius, maxSkeletonRadius;
    public GameObject skeletonPrefab;
    public AudioSource summonSkeletonsAudioSource;

    public GameObject corpseExplosionPrefab;
    
    // Use this for initialization
    void Start ()
    {
        player = MyGameManager.instance.GetPlayer();
        outlineController = MyGameManager.instance.GetOutlineController();
        fadeAudio = GetComponent<FadeAudio>();

        //if (generateSummonSpots)
        //{
        //    GenerateSummonSpots();
        //} else
        //{
            ReadSummonSpots();
        //}
    }

    public Transform[] summonSpotBases;
    public Transform[] summonSpotAttackBases;
    public List<SummonSpotRow[]> summonSpotGroups;
    public List<SummonSpotRow[]> summonSpotAttackGroups;
    private SummonSpotRow[] currentSummonSpotGroup;
    public int currrentSummonSpotGroupIndex = 0;
    public List<int> maxSummonsPerGroup;
    void ReadSummonSpots()
    {
        summonSpotGroups = new List<SummonSpotRow[]>(summonSpotBases.Length);
        summonSpotAttackGroups = new List<SummonSpotRow[]>(summonSpotAttackBases.Length);
        maxSummonsPerGroup = new List<int>(summonSpotBases.Length + summonSpotAttackBases.Length);

        foreach (Transform summonSpotBase in summonSpotBases)
        {
            SummonSpotRow[] summonSpotRows = summonSpotBase.GetComponentsInChildren<SummonSpotRow>();
            int maxSummons = 0;
            foreach (SummonSpotRow row in summonSpotRows)
            {
                row.Initialize();
                maxSummons += row.summonSpots.Length;
            }
            summonSpotGroups.Add(summonSpotRows);
            maxSummonsPerGroup.Add(maxSummons);
        }
        foreach (Transform summonSpotBase in summonSpotAttackBases)
        {
            SummonSpotRow[] summonSpotRows = summonSpotBase.GetComponentsInChildren<SummonSpotRow>();
            int maxSummons = 0;
            foreach (SummonSpotRow row in summonSpotRows)
            {
                row.Initialize();
                maxSummons += row.summonSpots.Length;
            }
            summonSpotAttackGroups.Add(summonSpotRows);
            maxSummonsPerGroup.Add(maxSummons);
        }


        int prevMax = maxSummonsPerGroup[0];
        for (int i=0; i< maxSummonsPerGroup.Count; i++) {
            int max = maxSummonsPerGroup[i];
            if (prevMax != max)
            {
                Debug.LogError("Summon spot group has different count of spots than the previous one: " + prevMax + " " + max);
                Debug.LogError(summonSpotBases[i]);
                Application.Quit();
            }
        }

        currentSummonSpotGroup = summonSpotGroups[0];


        //summonSpotRows = summonSpotBase.GetComponentsInChildren<SummonSpotRow>();
        //maxSummons = 0;
        //foreach(SummonSpotRow row in summonSpotRows)
        //{
        //    maxSummons += row.summonSpots.Length;
        //}
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

        SummonSpotRow[] newGroup;
        if (attack)
        {
            newGroup = summonSpotAttackGroups[i];
        } else
        {
            newGroup = summonSpotGroups[i];
        }

        if (newGroup != currentSummonSpotGroup)
        {
            //Debug.Log(newGroup);
            //return;
            currrentSummonSpotGroupIndex = i;

            int newRowIndex = 0;
            int newSpotIndex = 0;
            SummonSpotRow newRow = newGroup[newRowIndex];

            foreach (SummonSpotRow oldRow in currentSummonSpotGroup)
            {
                foreach (SummonSpot oldSpot in oldRow.summonSpots)
                {
                    if (oldSpot.HasEnemy())
                    {
                        oldSpot.enemy.allyBehavior.attacking = attack;
                        if (newSpotIndex >= newRow.summonSpots.Length)
                        {
                            newRowIndex++;
                            newRow = newGroup[newRowIndex];
                            newSpotIndex = 0;
                        }
                        newRow.summonSpots[newSpotIndex].SetEnemy(oldSpot.enemy);
                        newSpotIndex++;
                    }
                }
            }

            currentSummonSpotGroup = newGroup;
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
    //public SummonSpotRow[] summonSpotRows;
    public GameObject summonSpotRowPrefab;
    public GameObject summonSpotPrefab;
    //void GenerateSummonSpots()
    //{
    //    if (!generateSummonSpots)
    //    {
    //        return;
    //    }
    //    generateSummonSpots = false;

    //    if (summonSpotRows != null)
    //    {
    //        for (int i = 0; i < summonSpotRows.Length; i++)
    //        {
    //            SummonSpotRow row = summonSpotRows[i];
    //            for (int j = 0; j < row.summonSpots.Length; j++)
    //            {
    //                GameObject.Destroy(row.summonSpots[j].gameObject);
    //            }
    //            GameObject.Destroy(row.gameObject);
    //        }
    //    }

    //    summonSpotRows = new SummonSpotRow[numberOfSummonSpotRows];

    //    int spotsPerRow = summonSpotsInInitialRow;
    //    maxSummons = 0;
    //    for (int i=0; i<numberOfSummonSpotRows; i++)
    //    {
    //        maxSummons += spotsPerRow;
    //        Vector3 rowPosition = initialSummonSpot.position;
    //        rowPosition.y += i * summonSpotRowDistance;

    //        GameObject rowInst = GameObject.Instantiate(summonSpotRowPrefab, rowPosition, Quaternion.identity, initialSummonSpot);
    //        SummonSpotRow summonSpotRow = rowInst.GetComponent <SummonSpotRow>();
    //        summonSpotRows[i] = summonSpotRow;

    //        if (i > 0)
    //        {
    //            summonSpotRows[i - 1].nextRow = summonSpotRow;
    //            summonSpotRow.prevRow = summonSpotRows[i - 1];
    //        }


    //        summonSpotRow.summonSpots = new SummonSpot[spotsPerRow];
    //        if (spotsPerRow % 2 == 0)
    //        {
    //            rowPosition.x -= summonSpotDistance * 0.5f;
    //        }
    //        for (int j=0; j<spotsPerRow; j++)
    //        {
    //            Vector3 spotPosition = rowPosition;
    //            int offSetCount = (j + 1) / 2;
    //            int spotIndex = 0;

    //            if (j % 2 == 0)
    //            {
    //                spotPosition.x -= offSetCount * summonSpotDistance;
    //                spotIndex = -offSetCount;
    //            }
    //            else
    //            {
    //                spotPosition.x += offSetCount * summonSpotDistance;
    //                spotIndex = offSetCount;
    //            }


    //            if (spotsPerRow % 2 == 0)
    //            {
    //                offSetCount = j / 2;
    //            }
    //            spotPosition.y += summonSpotYOffset * Mathf.Pow(offSetCount, summonSpotYOffsetPower);

    //            GameObject spotInst = GameObject.Instantiate(summonSpotPrefab, spotPosition, Quaternion.identity, rowInst.transform);
    //            SummonSpot summonSpot = spotInst.GetComponent<SummonSpot>();
    //            summonSpot.rowIndex = i;
    //            summonSpot.spotIndex = spotIndex;
    //            summonSpotRow.summonSpots[j] = summonSpot;
    //        }

    //        if (spotsPerRow < maxSummonSpotsPerRow)
    //        {
    //            spotsPerRow = Mathf.Clamp(spotsPerRow + numberOfExtraSummonSpotsPerRow, 0, maxSummonSpotsPerRow);
    //        }
    //    }
    //}
    
    SummonSpot GetFreeSummonSpot()
    {
        foreach (SummonSpotRow row in currentSummonSpotGroup)
        {
            SummonSpot spot = row.GetFreeSummonSpot();
            if (spot != null)
            {
                return spot;
            }
        }
        return null;
    }

    public float mouseWithinSummonsRadius = 1f;
    Vector2 mousePolygonStart;
    public bool IsMouseInSummonSpots()
    {
        return false;
        //Vector3 mouse = Util.MouseInWorld();

        //int numberOfPoints = 2 * (numberOfSummonSpotRows);
        //Vector2[] polygonPoints = new Vector2[numberOfPoints];

        //int currentPoint = 0;
        ////first row
        //SummonSpot[] spots = summonSpotRows[0].summonSpots;
        //int maxSpot;
        //Transform point1Transform;
        //Vector2 point1;
        //Transform point2Transform;
        //Vector2 point2;
        //if (spots.Length == 1)
        //{
        //    Transform pointTransform = spots[0].transform.transform;
        //    Vector2 point = pointTransform.position;
        //    point -= (Vector2)(pointTransform.up * mouseWithinSummonsRadius);
        //    point -= (Vector2)(pointTransform.right * mouseWithinSummonsRadius);
        //    polygonPoints[currentPoint++] = point;

        //    point += 2 * (Vector2)(pointTransform.right * mouseWithinSummonsRadius);
        //    polygonPoints[currentPoint++] = point;
        //} else
        //{
        //    maxSpot = spots.Length - 1;
        //    point1Transform = spots[maxSpot].transform;
        //    point1 = point1Transform.position;
        //    point2Transform = spots[maxSpot - 1].transform;
        //    point2 = point2Transform.position;

        //    point1 -= (Vector2)(point1Transform.up * mouseWithinSummonsRadius);
        //    point2 -= (Vector2)(point1Transform.up * mouseWithinSummonsRadius);

        //    if (point1Transform.localPosition.x < 0)
        //    {
        //        point1 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        point2 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        polygonPoints[currentPoint++] = point2;
        //        polygonPoints[currentPoint++] = point1;
        //    }
        //    else
        //    {
        //        point2 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        point1 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        polygonPoints[currentPoint++] = point1;
        //        polygonPoints[currentPoint++] = point2;
        //    }
        //}
        ////additional rows
        //Vector2[] otherSidePoints = new Vector2[summonSpotRows.Length - 2];
        //for (int i = 1; i < summonSpotRows.Length-1; i++)
        //{
        //    spots = summonSpotRows[i].summonSpots;
        //    maxSpot = spots.Length - 1;
        //    point1Transform = spots[maxSpot].transform;
        //    point1 = point1Transform.position;
        //    point2Transform = spots[maxSpot - 1].transform;
        //    point2 = point2Transform.position;

        //    if (point1Transform.localPosition.x < 0)
        //    {
        //        point1 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        point2 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        polygonPoints[currentPoint++] = point1;
        //        otherSidePoints[i - 1] = point2;
        //    } else
        //    {
        //        point2 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        point1 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //        polygonPoints[currentPoint++] = point2;
        //        otherSidePoints[i - 1] = point1;
        //    }
        //}
        ////top row
        //spots = summonSpotRows[summonSpotRows.Length-1].summonSpots;
        //maxSpot = spots.Length - 1;
        //point1Transform = spots[maxSpot].transform;
        //point1 = point1Transform.position;
        //point2Transform = spots[maxSpot - 1].transform;
        //point2 = point2Transform.position;

        //point1 += (Vector2)(point1Transform.up * mouseWithinSummonsRadius);
        //point2 += (Vector2)(point1Transform.up * mouseWithinSummonsRadius);

        //if (point1Transform.localPosition.x < 0)
        //{
        //    point1 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //    point2 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //    polygonPoints[currentPoint++] = point1;
        //    polygonPoints[currentPoint++] = point2;
        //}
        //else
        //{
        //    point2 -= (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //    point1 += (Vector2)(point1Transform.right * mouseWithinSummonsRadius);
        //    polygonPoints[currentPoint++] = point2;
        //    polygonPoints[currentPoint++] = point1;
        //}

        ////other side
        //for (int i= otherSidePoints.Length-1; i>=0; i--)
        //{
        //    polygonPoints[currentPoint++] = otherSidePoints[i];
        //}

        //for (int i=0; i< polygonPoints.Length-1; i++)
        //{
        //    Debug.DrawLine(polygonPoints[i], polygonPoints[i + 1], Color.cyan);
        //}
        //Debug.DrawLine(polygonPoints[0], polygonPoints[polygonPoints.Length - 1], Color.cyan);

        //return Util.PolygonContainsPoint(mouse, polygonPoints);
    }

    public bool attacking = false;
    public void Attack()
    {
        attacking = true;
        SetSummonSpotGroup(currrentSummonSpotGroupIndex, true);
        //foreach (SummonSpotRow row in currentSummonSpotGroup)
        //{
        //    foreach (SummonSpot spot in row.summonSpots)
        //    {
        //        //spot.attacking = true;
        //        if (spot.HasEnemy())
        //        {
        //            //spot.attacking = true;
        //            spot.enemy.allyBehavior.attacking = true;
        //        }
        //    }
        //}
    }

    public void StopAttack()
    {
        attacking = false;
        SetSummonSpotGroup(currrentSummonSpotGroupIndex, false);
        //SummonSpotRow[] summonSpotRows = GetCurrentSummonSpotGroup();
        //foreach (SummonSpotRow row in currentSummonSpotGroup)
        //{
        //    foreach (SummonSpot spot in row.summonSpots)
        //    {
        //        //spot.attacking = false;
        //        if (spot.HasEnemy())
        //        {
        //            spot.enemy.allyBehavior.attacking = false;
        //        }
        //    }
        //}
    }

    int SummonCount()
    {
        int count = 0;
        foreach(SummonSpotRow row in currentSummonSpotGroup)
        {
            count += row.EnemyCount();
        }
        return count;
    }

    // Update is called once per frame
    void Update ()
    {
        string summonText = SummonCount() + " / " + maxSummonsPerGroup[0];
        text.SetText(summonText);

        //GenerateSummonSpots();
        MoveSummonCircle();

        //if (Util.GetButtonDown("SummonGroup1"))
        //{
        //    SetSummonSpotGroup(0);
        //}
        //if (Util.GetButtonDown("SummonGroup2"))
        //{
        //    SetSummonSpotGroup(1);
        //}
        if (!attacking)
        {
            if (Util.GetButton("Shield"))
            {
                SetSummonSpotGroup(1, false);
            }
            else
            {
                SetSummonSpotGroup(0, false);
            }
        }

        if (!player.IsDead())
        {
            summonEnabled = Util.GetButton("Summon");
            corpseExplosionTriggered = Util.GetButtonDown("CorpseExplosion");
            corpseExplosion = Util.GetButton("CorpseExplosion");
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

        if (summonEnabled && summons.Count <= minSummons)
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
            summonSkeletonsAudioSource.Play();
            for (int i=0; i< numberSkeletonsToSummon; i++)
            {
                Vector2 position = (Vector2)transform.position + Random.insideUnitCircle * Random.Range(minSkeletonRadius, maxSkeletonRadius);
                //Vector3 position = summonSpots[i].transform.position;
                //Vector3 rotation = new Vector3(0, 0, Random.Range(0, 360));
                GameObject skeletonInstance = GameObject.Instantiate(skeletonPrefab, position, Quaternion.identity);
                Summon(skeletonInstance.GetComponent<Enemy>());
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
    
    void OnTriggerStay2D(Collider2D other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (CanSummon(enemy))
        {
            Debug.Log("Summoning " + enemy);
            Summon(enemy);
        }

        if (CanExplode(enemy))
        {
            Debug.Log("Exploding " + enemy);
            enemy.Explode();
        }
    }

    bool CanSummon(Enemy enemy)
    {
        Debug.Log("Trying to summon " + enemy);
        return summonEnabled &&
            enemy != null
            && enemy.CanResurrect();
    }

    void Summon(Enemy enemy)
    {
        SummonSpot summonSpot = GetFreeSummonSpot();
        if (summonSpot != null)
        {
            summons.Add(enemy);
            summonSpot.enemy = enemy;
            enemy.Resurrect(summonSpot);
            enemy.summonCircle = this;
            
            //outlineController.AddObject(OutlineController.HIGHLIGHT_TYPE.RESSURECTED, enemy.gameObject);
        }
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
        return corpseExplosionTriggered
            && enemy != null
            && enemy.CanExplode();
    }

    void MoveSummonCircle()
    {
        Vector3 mouse = Util.MouseInWorld();

        Vector3 position = transform.position;
        position.x = mouse.x;
        position.y = mouse.y;

        transform.position = position;
    }
}
