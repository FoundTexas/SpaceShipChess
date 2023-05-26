using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] float actionRate, mineingRate, destructRate;

    float cooldown = 0;

    private void FixedUpdate()
    {
        cooldown -= cooldown > 0 ? Time.deltaTime : 0;
    }
    public void FoundTarget(GameObject target)
    {
        if (!target.name.Contains(transform.parent.name) && (target.name.Contains("DamageObject")))
        {
            HealthBar hptar;
            if (TryGetComponent<HealthBar>(out hptar))
            {
                int colonIndex = target.name.IndexOf(":");
                string numStr = target.name.Substring(colonIndex + 1);
                int num = int.Parse(numStr);
                hptar.ChangeHealth(num * -1);
                target.SetActive(true);
                Destroy(target, 0.3f);
            }
        }
        else if (cooldown <= 0)
        {
            cooldown = actionRate;
            Debug.Log(target.name);

            if (target.name.Contains(transform.parent.name)) FriendlyAction(target);
            else HostileAction(target);
        }
    }

    public virtual void FriendlyAction(GameObject target)
    {
        HealthBar hptar;
        if (target.TryGetComponent<HealthBar>(out hptar))
        {
            hptar.ChangeHealth(1);
        }
    }
    public virtual void HostileAction(GameObject target)
    {
        HealthBar hptar;
        if (target.TryGetComponent<HealthBar>(out hptar))
        {
            hptar.ChangeHealth(-10);
        }
    }

}
