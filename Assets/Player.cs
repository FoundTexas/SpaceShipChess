using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] KeyCode minusX, plusX, minusY, plusY, select;
    bool adding;

    CinemachineVirtualCamera cam;
    public Vector3Int movPoint;
    Vector2Int max, mov;

    GameObject selector, selected;
    public List<GameObject> ships = new List<GameObject>();

    private void Awake()
    {
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

            if (selected)
            {
                if (Board.UpdateCord(new Vector2Int(movPoint.x, movPoint.z), selected))
                {
                    selected.name = gameObject.name + " Ship: " + movPoint;
                }
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
                GameObject ship = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                if (!Board.tryAdd(ship, movPoint))
                {
                    Destroy(ship);
                }
                else
                {
                    ship.name = gameObject.name + " Ship: " + movPoint;
                    ship.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 1f) : new Color(1, 1, 0, 1f);
                    ships.Add(ship);
                }
            }
            else
            {
                adding = false;
            }
        }
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
