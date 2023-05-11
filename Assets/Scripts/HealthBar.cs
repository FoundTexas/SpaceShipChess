using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    public float maxHealth = 100f;
    float currentHealth;

    Image hpBar;

    private void Start()
    {
        currentHealth = maxHealth;

        hpBar = transform.Find("Canvas").Find("HPBar").GetComponent<Image>();
    }

    public void ChangeHealth(float value)
    {
        currentHealth += value;
        Debug.Log("Damage: " + gameObject.name + " hp:" + currentHealth);
        hpBar.fillAmount = currentHealth / maxHealth;
        Debug.Log(hpBar.fillAmount);
        hpBar.color = gradient.Evaluate(hpBar.fillAmount);

        if (hpBar.fillAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
