using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    public float maxHealth = 100f;
    float currentHealth;

    Image hpBar;

    private void Start() {
        currentHealth = maxHealth;

        hpBar = transform.Find("Canvas").Find("HPBar").GetComponent<Image>();

        ChangeHealth(Random.Range(maxHealth/2, maxHealth));
    }

    public void ChangeHealth(float value)
    {
        hpBar.fillAmount = value/maxHealth;
        hpBar.color = gradient.Evaluate(hpBar.fillAmount);
    }
}
