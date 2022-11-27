using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CHealthDisplay : MonoBehaviour
{
    [SerializeField] private CHealth Health = null;
    [SerializeField] private GameObject HealthBarParent = null;
    [SerializeField] private Image HealthBarImage = null;

    private void Awake()
    {
        Health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        Health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        HealthBarParent.SetActive(true);
    }
    private void OnMouseExit()
    {
        HealthBarParent.SetActive(false);
    }

    private void HandleHealthUpdated(int current_health, int max_health)
    {
        HealthBarImage.fillAmount = (float)current_health / max_health;
    }
}
