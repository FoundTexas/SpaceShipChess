using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] KeyCode minusX, plusX, minusY, plusY, select;
    bool adding;

    CinemachineVirtualCamera cam;
    public Vector3 movPoint;
    Vector2 max, mov;

    GameObject selector, selected;
    List<GameObject> ships = new List<GameObject>();

    private void Awake()
    {
        cam = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
        selector = transform.GetChild(0).gameObject;
        cam.Follow = selector.transform;
    }

    private void Update()
    {

        mov = Vector2.zero;
        if (Input.GetKey(minusX))
        {
            mov -= new Vector2(1, 0);
        }
        if (Input.GetKey(plusX))
        {
            mov += new Vector2(1, 0);
        }
        if (Input.GetKey(minusY))
        {
            mov -= new Vector2(0, 1);
        }
        if (Input.GetKey(plusY))
        {
            mov += new Vector2(0, 1);
        }

        if (Input.GetKeyDown(select))
        {
            if (selected)
            {
                adding = false;
                selected = null;
            }
            else if (!adding && !board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                adding = true;
            }
            else if (!adding && board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                GameObject g = board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z));

                if (ships.Contains(g))
                {
                    selected = g;
                }
            }
            else if (adding)
            {
                adding = false;
                GameObject ship = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                if (!board.tryAdd(ship, movPoint))
                {
                    Destroy(ship);
                }
                else
                {
                    ship.name = "P1 Ship: " + movPoint;
                    ship.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 1f) : new Color(1, 1, 0, 1f);
                    ships.Add(ship);
                }
            }
        }

        if (Vector3.Distance(selector.transform.position, movPoint) >= 0.05f)
        {
            selector.transform.position = Vector3.MoveTowards(
                selector.transform.position, movPoint, speed * Time.deltaTime
            );

            if (selected)
            {
                selected.transform.position = Vector3.MoveTowards(
                selected.transform.position, selector.transform.position, speed * Time.deltaTime
                );
            }
        }

        if (Vector3.Distance(selector.transform.position, movPoint) < 0.05f && (mov != Vector2.zero))
        {
            adding = false;

            if (!board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)))
            {
                movPoint += new Vector3(mov.x, 0, mov.y);

                movPoint = new Vector3(
                    Mathf.Clamp(movPoint.x, 0, max.x),
                    movPoint.y,
                    Mathf.Clamp(movPoint.z, 0, max.y));

                if (selected)
                    board.UpdateCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z), new Vector2(movPoint.x, movPoint.z), selected);
            }
            else if ((board.checkCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z)).name.Contains("1")
                == name.Contains("1")))
            {
                movPoint += new Vector3(mov.x, 0, mov.y);

                movPoint = new Vector3(
                    Mathf.Clamp(movPoint.x, 0, max.x),
                    movPoint.y,
                    Mathf.Clamp(movPoint.z, 0, max.y));

                if (selected)
                    board.UpdateCord(new Vector2(mov.x + movPoint.x, mov.y + movPoint.z), new Vector2(movPoint.x, movPoint.z), selected);
            }
        }
    }

    public void SetSelector(Vector3 vec, Vector2 maxvalues)
    {
        selector.GetComponent<Renderer>().material.color = name.Contains("1") ? new Color(1, 0, 1, 1f) : new Color(1, 1, 0, 1f);
        //cam.LookAt = selector.transform;
        movPoint = vec;
        selector.transform.position = vec;
        max = maxvalues;

    }
}
