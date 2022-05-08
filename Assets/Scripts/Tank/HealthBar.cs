using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private GameObject tank;
    [SerializeField] private Slider slider;

    private void Update()
    {
        if (tank != null)
        {
            transform.position = tank.transform.position;
        }
    }

    public void SetUpTankHealth(GameObject tankToFollow, float maxHealth)
    {
        tank = tankToFollow;
        slider.minValue = 0;
        slider.maxValue = maxHealth;

        if (tank.TryGetComponent<TankHealth>(out TankHealth tankHealth))
        {
            tankHealth.OnCurrentHealthChanged += OnCurrentHealthChanged;
            tankHealth.OnDie += OnTankDie;
        }
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
        }
    }

    public void SetRot(Quaternion rot)
    {
        transform.rotation = rot;
    }
}
