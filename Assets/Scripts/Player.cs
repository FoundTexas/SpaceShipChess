using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    public string selectedShip = "ProtypeShip";
    List<GameObject> ships = new List<GameObject>();
    Vector3Int movPoint;
    [SerializeField] KeyCode minusX, plusX, minusY, plusY, select;
    [SerializeField] float speed = 5;

    CinemachineVirtualCamera cam;
    public GameObject selector, selected;
    Vector2Int max, mov;
    bool adding;

    private void Awake()
    {
        selectedShip = "ProtypeShip";
        cam = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
        selector = transform.GetChild(0).gameObject;
        cam.Follow = selector.transform;
    }

    private void Update()
    {
        // Check if the selector is moving
        bool isMoving = Vector3.Distance(selector.transform.position, movPoint) >= 0.05f;

        // If selector is moving, move it towards the destination point
        SelectionMovement(isMoving);

        // Calculate the movement based on the input keys pressed
        GetMovementVector();

        // Revise the desired moevement to perform to check if it is valid and handells the interactions
        CheckBoardMovement(isMoving, Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)));
        
        // Logic to execute on Input key "select"
        SelectInputAction(Board.checkCord(new Vector2(movPoint.x, movPoint.z)));
    }

    void SelectionMovement(bool isMoving)
    {
        if (isMoving)
        {
            selector.transform.position = Vector3.MoveTowards(
                selector.transform.position, movPoint, speed * Time.deltaTime);

            // If a ship is selected, move it towards the destination point as well
            if (selected)
            {
                selected.transform.position = Vector3.MoveTowards(
                    selected.transform.position, movPoint, speed * Time.deltaTime);
                // Set the target angle of the ship towards the destination point
                SetTargetAngle(selected, movPoint);
            }
        }
    }
    void GetMovementVector()
    {
        mov = Vector2Int.zero;
        mov += Input.GetKey(minusX) ? new Vector2Int(-1, 0) : Vector2Int.zero;
        mov += Input.GetKey(plusX) ? new Vector2Int(1, 0) : Vector2Int.zero;
        mov += Input.GetKey(minusY) ? new Vector2Int(0, -1) : Vector2Int.zero;
        mov += Input.GetKey(plusY) ? new Vector2Int(0, 1) : Vector2Int.zero;
    }
    void CheckBoardMovement(bool isMoving, GameObject target)
    {
        bool hasMoved = mov != Vector2Int.zero;

        if (hasMoved)
        {
            if (!isMoving && (!target || (target.name.Contains(gameObject.name) && !selected) || !target.activeInHierarchy))
            {
                adding = false;

                movPoint = new Vector3Int(
                    Mathf.Clamp(movPoint.x + mov.x, 0, max.x),
                    movPoint.y,
                    Mathf.Clamp(movPoint.z + mov.y, 0, max.y));
            }
            if (!isMoving && selected)
            {
                if (Board.UpdateCord(new Vector2Int(movPoint.x, movPoint.z), selected))
                    selected.name = gameObject.name + " Ship: " + movPoint;

                if (target)
                {
                    Debug.Log("Finding:");
                    Ship actionship;
                    if (selected.TryGetComponent<Ship>(out actionship))
                    {
                        actionship.FoundTarget(target);
                    }
                }
            }
        }
    }
    void SelectInputAction(GameObject target)
    {
        if (Input.GetKeyDown(select))
        {
            if (selected)
            {
                adding = false;
                selected = null;
            }
            else if (!adding && !target)
            {
                adding = true;
            }
            else if (target)
            {
                adding = false;
                GameObject g = target;

                if (ships.Contains(g))
                {
                    selected = g;
                }
            }
            else if (adding && !target)
            {
                adding = false;
                GameObject ship = Instantiate(Resources.Load<GameObject>("Prefabs/" + selectedShip), movPoint,
                Quaternion.Euler(0, (name.Contains("1")) ? 90 : -90, 0), transform);//GameObject.CreatePrimitive(PrimitiveType.Sphere);

                if (!Board.tryAdd(ship, movPoint))
                {
                    Destroy(ship);
                }
                else
                {
                    ship.name = gameObject.name.Replace("(Clone)", "") + " Ship " + movPoint;

                    foreach (Transform child in ship.transform.GetChild(0))
                    {
                        child.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 1f) : new Color(1, 1, 0, 1f);
                    }

                    ships.Add(ship);
                }
            }
        }
    }

    // Public Methods
    public void SetTargetAngle(GameObject obj, Vector3 mov)
    {
        Quaternion targetRotation = Quaternion.LookRotation(mov - obj.transform.position, Vector3.up);
        Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, targetRotation, 360);
        obj.transform.rotation = newRotation;
    }


    public void SetSelector(Vector3Int vec, Vector2Int maxvalues)
    {
        selector.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 0.2f) : new Color(1, 1, 0, 0.2f);
        cam.LookAt = selector.transform;
        movPoint = vec;
        selector.transform.position = vec;
        max = maxvalues;
    }
}
