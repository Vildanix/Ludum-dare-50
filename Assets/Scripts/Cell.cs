using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Cell : MonoBehaviour
{
    [SerializeField] float animationSpeed = 1.0f;
    [SerializeField] float lowVisibilityMultiplier = 3.0f;
    [SerializeField] PointOfInterest poiRef;

    IObjectPool<Cell> pool;
    float animationProgress = 0;
    VISIBILITY_LEVEL visibilityLevel = VISIBILITY_LEVEL.HIDDEN;
    bool canRemove = false;
    FleetController playerFleet;

    private enum VISIBILITY_LEVEL { HIDDEN = 0, LOW = 3, MEDIUM = 5, FULL = 10};



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

    private void Update()
    {
        UpdateAnimationTimer();
        var clampedSize = Math.Clamp(animationProgress, 0, 1);
        transform.localScale = new Vector3(clampedSize, clampedSize, clampedSize);
        HandleVisibility();
        HandleCellRemoval();
    }

    private void HandleVisibility()
    {
        if (playerFleet == null)
        {
            visibilityLevel = VISIBILITY_LEVEL.HIDDEN;
            return;
        }

        if (Vector3.Distance(transform.position, playerFleet.transform.position) < playerFleet.GetVisibilityDistance())
        {
            visibilityLevel = VISIBILITY_LEVEL.FULL;
            return;
        }

        if (Vector3.Distance(transform.position, playerFleet.transform.position) < playerFleet.GetVisibilityDistance() * lowVisibilityMultiplier || poiRef != null)
        {
            visibilityLevel = VISIBILITY_LEVEL.LOW;
            return;
        }

        visibilityLevel = VISIBILITY_LEVEL.HIDDEN;
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
