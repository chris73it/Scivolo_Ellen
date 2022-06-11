using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] FloatReference avatarCurrentHealth;
    [SerializeField] FloatReference avatarMaxHealth;

    void Awake()
    {
        avatarCurrentHealth.Value = avatarMaxHealth.Value;
    }

    public void UpdateHealthBar()
    {
        if (healthBarImage.fillAmount != avatarCurrentHealth.Value / avatarMaxHealth.Value)
        {
            healthBarImage.fillAmount = avatarCurrentHealth.Value / avatarMaxHealth.Value;
        }
    }
}
