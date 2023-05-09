using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public void FoundTarget(GameObject target)
    {
        HealthBar hptar;
        if(target.TryGetComponent<HealthBar>(out hptar))
        {
            hptar.ChangeHealth(10);
        }
    }
}
