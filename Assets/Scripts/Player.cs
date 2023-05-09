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
    GameObject selector, selected;
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
        mov = Vector2Int.zero;
        mov += Input.GetKey(minusX) ? new Vector2Int(-1, 0) : Vector2Int.zero;
        mov += Input.GetKey(plusX) ? new Vector2Int(1, 0) : Vector2Int.zero;
        mov += Input.GetKey(minusY) ? new Vector2Int(0, -1) : Vector2Int.zero;
        mov += Input.GetKey(plusY) ? new Vector2Int(0, 1) : Vector2Int.zero;

        if (Vector3.Distance(selector.transform.position, movPoint) >= 0.05f)
        {
            selector.transform.position = Vector3.MoveTowards(
                selector.transform.position, movPoint, speed * Time.deltaTime
            );

            if (selected)
            {
                selected.transform.position = Vector3.MoveTowards(
                selected.transform.position, selector.transform.position, speed * Time.deltaTime);

                SetTargetAngle(selected, movPoint);
            }
        }

        if ((mov != Vector2.zero) && (!Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)) ||
                (Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z))
                .name.Contains(gameObject.name) == name.Contains(gameObject.name) && !selected)))
        {
            adding = false;

            if (Vector3.Distance(selector.transform.position, movPoint) < 0.05f)
            {
                movPoint += new Vector3Int(mov.x, 0, mov.y);

                movPoint = new Vector3Int(
                    Mathf.Clamp(movPoint.x, 0, max.x),
                    movPoint.y,
                    Mathf.Clamp(movPoint.z, 0, max.y));
            }

            if (selected && (Board.UpdateCord(new Vector2Int(movPoint.x, movPoint.z), selected)))
            {
                selected.name = gameObject.name + " Ship: " + movPoint;
            }
        }

        if (Input.GetKeyDown(select))
        {
            if (selected)
            {
                adding = false;
                selected = null;
            }
            else if (!adding && !Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                adding = true;
            }
            else if (Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                adding = false;
                GameObject g = Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z));

                if (ships.Contains(g))
                {
                    selected = g;
                }
            }
            else if (adding && !Board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                adding = false;
                GameObject ship = Instantiate(Resources.Load<GameObject>("Prefabs/" + selectedShip), movPoint,
                Quaternion.Euler(0, (name.Contains("1")) ? 90 : -90, 0));//GameObject.CreatePrimitive(PrimitiveType.Sphere);

                if (!Board.tryAdd(ship, movPoint))
                {
                    Destroy(ship);
                }
                else
                {
                    ship.name = gameObject.name + " Ship: " + movPoint;

                    foreach (Transform child in ship.transform.GetChild(0))
                    {
                        child.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 1f) : new Color(1, 1, 0, 1f);
                    }

                    ships.Add(ship);
                }
            }
            else
            {
                adding = false;
            }
        }
    }

    public void SetTargetAngle(GameObject obj, Vector3 mov)
    {
        Quaternion targetRotation = Quaternion.LookRotation(mov - obj.transform.position, Vector3.up);
        Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, targetRotation, 360);
        obj.transform.rotation = newRotation;
    }


    public void SetSelector(Vector3Int vec, Vector2Int maxvalues)
    {
        selector.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 0.2f) : new Color(1, 1, 0, 0.2f);
        //cam.LookAt = selector.transform;
        movPoint = vec;
        selector.transform.position = vec;
        max = maxvalues;

    }
}
