using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Cell : MonoBehaviour
{
    [Header("Animation settings")]
    [SerializeField] float animationSpeed = 1.0f;
    [SerializeField] float scannerVisibilityMultiplier = 3.0f;

    [Header("Unity references")]
    [SerializeField] PointOfInterest poiRef;
    [SerializeField] GameObject backgroundRef;
    [SerializeField] MeshRenderer cellRenderer;

    IObjectPool<Cell> pool;
    float animationProgress = 0;
    VISIBILITY_LEVEL visibilityLevel = VISIBILITY_LEVEL.HIDDEN;
    bool canRemove = false;
    FleetController playerFleet;

    // LOW level is for grid in movable range
    // MEDIUM is for grid with unidentified POI
    // FULL is for tiles within ship visible range
    private enum VISIBILITY_LEVEL { HIDDEN = 0, LOW = 3, HIGH = 5, POI = 10};

    [Header("Visibility level colors")]
    [SerializeField] Material lowVisibilityMat;
    [SerializeField] Material highVisibilityMat;
    [SerializeField] Material poiVisibilityMat;

    [SerializeField] List<PointOfInterest> poiEventPrefabs;
    [SerializeField] List<GameObject> backgroundPrefabs;

    static HashSet<(int, int)> collectedPoIs = new HashSet<(int, int)>();

    private void Start()
    {
        playerFleet = GameObject.FindGameObjectWithTag("Player")?.GetComponent<FleetController>();
    }

    private void OnEnable()
    {
        animationProgress = 0;
        visibilityLevel = VISIBILITY_LEVEL.HIDDEN;
        canRemove = false;
    }

    public void SetPool(IObjectPool<Cell> pool)
    {
        this.pool = pool;
    }

    public static void InitializePointsOfInteres()
    {
        collectedPoIs = new HashSet<(int, int)>();
    }

    private void Update()
    {
        UpdateAnimationTimer();
        var clampedSize = Mathf.Clamp(animationProgress, 0, 1);
        transform.localScale = new Vector3(clampedSize, clampedSize, clampedSize);
        HandleVisibility();
        HandleCellRemoval();
    }

    public void InitContent(int x, int y, float poiProbability, float backgroundProbability)
    {
        float localProbability = Mathf.PerlinNoise(x / 2f, y / 2f);
        if (poiRef)
        {
            DestroyPoI();
        }

        if (backgroundRef)
        {
            Destroy(backgroundRef);
            backgroundRef = null;
        }

        if (localProbability < poiProbability && !collectedPoIs.Contains((x, y)))
        {
            var poiTemplateIndex = Random.Range(0, poiEventPrefabs.Count);
            poiRef = Instantiate(poiEventPrefabs[poiTemplateIndex], transform);
            poiRef.SetupEvent((x, y));
        }
        else if (localProbability < backgroundProbability)
        {
            var backgroundTemplateIndex = Random.Range(0, backgroundPrefabs.Count);
            backgroundRef = Instantiate(backgroundPrefabs[backgroundTemplateIndex], transform);
        }
    }

    private void DestroyPoI()
    {
        Destroy(poiRef.gameObject);
        poiRef = null;
    }

    public PointOfInterest GetPointOfInterest()
    {
        if (poiRef == null) return null;
        var poi = poiRef;
        DestroyPoI();
        collectedPoIs.Add(poi.GetCoordinates());
        return poi;
    }

    public bool HasPointOfInterest()
    {
        return poiRef != null;
    }


    private void HandleVisibility()
    {
        if (playerFleet == null)
        {
            SetVisibilityLevel(VISIBILITY_LEVEL.HIDDEN);
            return;
        }

        if (poiRef != null)
        {
            SetVisibilityLevel(VISIBILITY_LEVEL.POI);
            return;
        }

        if (Vector3.Distance(transform.position, playerFleet.transform.position) < playerFleet.GetVisibilityDistance())
        {
            SetVisibilityLevel(VISIBILITY_LEVEL.HIGH);
            return;
        }

        if (Vector3.Distance(transform.position, playerFleet.transform.position) < playerFleet.GetVisibilityDistance() * scannerVisibilityMultiplier)
        {
            SetVisibilityLevel(VISIBILITY_LEVEL.LOW);
            return;
        }

        SetVisibilityLevel(VISIBILITY_LEVEL.HIDDEN);
    }

    private void SetVisibilityLevel(VISIBILITY_LEVEL level)
    {
        visibilityLevel = level;
        switch (level)
        {
            case VISIBILITY_LEVEL.POI:
                cellRenderer.material =poiVisibilityMat;
                break;
            case VISIBILITY_LEVEL.HIGH:
                cellRenderer.material = highVisibilityMat;
                break;
            case VISIBILITY_LEVEL.LOW:
                cellRenderer.material = lowVisibilityMat;
                break;
        }
    }

    private void HandleCellRemoval()
    {
        if (canRemove && animationProgress <= 0.01f)
        {
            if (pool != null)
            {
                pool.Release(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }

    private void UpdateAnimationTimer()
    {
        if (canRemove)
        {
            animationProgress -= Time.deltaTime * animationSpeed;
            return;
        }

        if (visibilityLevel > VISIBILITY_LEVEL.HIDDEN && animationProgress < 1.0f)
        {
            animationProgress += Time.deltaTime * animationSpeed;
            return;
        }

        if (visibilityLevel == VISIBILITY_LEVEL.HIDDEN && animationProgress > 0f)
        {
            animationProgress -= Time.deltaTime * animationSpeed;
        }
    }

    public void RemoveCell()
    {
        canRemove = true;
    }
}
