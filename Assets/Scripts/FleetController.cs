using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FleetController : MonoBehaviour
{
    [Header("Visibility settings")]
    [SerializeField] float baseVisibility = 2.0f;
    [SerializeField] float currentVisibility = 0f;
    [SerializeField] float visibilityChangeSpeed = 0.1f;

    [Header("Movement settings")]
    [SerializeField, Min(0)] float movementSpeed = 1.0f;
    [SerializeField, Min(0)] float rotationSpeed = 1.0f;
    [SerializeField, Min(0), Tooltip("Angle difference from ideal direction below which ships are alowed to move forward")]
    float slippingAngle = 5.0f;
    [SerializeField, Min(0)] float waypointReachTreshold = 0.02f;

    [SerializeField] Grid grid;
    [SerializeField] LayerMask gridLayer;
    [SerializeField] Camera mainCamera;

    Vector3 targetPosition;
    Cell currentCell;
    List<Cell> movementWaypoints;

    [Header("Child references")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform fleetRoot;

    public UnityEvent onMove;
    public UnityEvent<Vector3> onFire;
    public UnityEvent<PointOfInterest> onPointOfInterestEvent;

    // Start is called before the first frame update
    void Start()
    {
        movementWaypoints = new List<Cell>();
        targetPosition = transform.position;
        currentCell = grid.GetCellAtPosition(transform.position);
    }

    public void Initialize()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementSelection();
        HandleMovementAnimation();
        HandleVisibilityChange();
    }

    private void HandleVisibilityChange()
    {
        if (currentVisibility < baseVisibility)
        {
            currentVisibility = Mathf.Lerp(currentVisibility, baseVisibility, Time.deltaTime * visibilityChangeSpeed);
        }
    }

    private void HandleMovementAnimation()
    {
        if (movementWaypoints == null || movementWaypoints.Count == 0) return;
        
        var nextWaypoint = movementWaypoints.First();
        MoveToCell(nextWaypoint);
        if (Vector3.Distance(transform.position, nextWaypoint.transform.position) < waypointReachTreshold)
        {
            currentCell = nextWaypoint;
            movementWaypoints.RemoveAt(0);
            onMove.Invoke();
            var potentialPoI = currentCell.GetPointOfInterest();
            if (potentialPoI != null)
            {
                onPointOfInterestEvent.Invoke(potentialPoI);
            }
        }
    }

    private void HandleMovementSelection()
    {
        var gridRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(gridRay, out RaycastHit hit, 100f, gridLayer))
        {
            var selectedCell = grid.GetCellAtPosition(hit.point);

            if (selectedCell && !HasValidPath())
            {
                var path = grid.GetPathBetweenCells(currentCell, selectedCell);
                HighlightCellPath(path);
                
                if (path != null && Input.GetMouseButtonDown(0))
                {
                    movementWaypoints = path;
                }
            } else
            {
                HighlightCellPath(movementWaypoints);
            }
        }
        grid.SetCenterPosition(transform.position);
    }

    private void HighlightCellPath(List<Cell> path)
    {
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.Select(p => p.transform.position).ToArray());
    }

    private bool HasValidPath()
    {
        return movementWaypoints != null && movementWaypoints.Count > 0;
    }

    internal float GetVisibilityDistance()
    {
        return currentVisibility;
    }

    void MoveToCell(Cell cell)
    {
        var direction = cell.transform.position - transform.position;
        direction.Normalize();
        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        fleetRoot.rotation = Quaternion.RotateTowards(fleetRoot.rotation, targetRotation, rotationSpeed);

        if (Quaternion.Angle(fleetRoot.rotation, targetRotation) <= slippingAngle)
        {
            transform.position += direction * Time.deltaTime * movementSpeed;
        }
    }
}
