using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private readonly float timeToDestroyAfterTankDie = 1;
    private GameObject tank;
    [SerializeField] private Slider slider;

    private void LateUpdate()
    {
        if (tank != null)
        {
            transform.position = tank.transform.position;
        }
    }

    public void SetupHealthBar(GameObject tankToFollow, float maxHealth, Quaternion rot)
    {
        tank = tankToFollow;
        slider.minValue = 0;
        slider.maxValue = maxHealth;

        if (tank.TryGetComponent<TankHealth>(out TankHealth tankHealth))
        {
            tankHealth.OnCurrentHealthChanged += OnCurrentHealthChanged;
            tankHealth.OnDie += OnTankDie;
        }

        transform.rotation = rot;
    }

    private void OnCurrentHealthChanged(float currentHealth)
    {
        slider.value = currentHealth;
    }

    private void OnTankDie()
    {
        if (tank.TryGetComponent<TankHealth>(out TankHealth tankHealth))
        {
            tankHealth.OnCurrentHealthChanged -= OnCurrentHealthChanged;
            tankHealth.OnDie -= OnTankDie;

            Destroy(this.gameObject, timeToDestroyAfterTankDie);
        }
    }
}
