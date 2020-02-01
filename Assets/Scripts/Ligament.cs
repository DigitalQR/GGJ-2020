using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LigamentChain))]
public class Ligament : MonoBehaviour
{
	[SerializeField]
	private float m_MaxHealth = 1.0f;

    private LigamentChain m_LigamentChain = null;

	[SerializeField]
	private float m_CurrentHealth;

    private void Start()
	{
		m_LigamentChain = GetComponent<LigamentChain>();
		m_CurrentHealth = m_MaxHealth;
    }
	
	public float Health
	{
		get => Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
	}

	public float MaxHealth
	{
		get => m_MaxHealth;
	}

	public float NormalizeHealth
	{
		get => Mathf.Clamp01(m_CurrentHealth / m_MaxHealth);
	}

	public bool IsDead
	{
		get => m_CurrentHealth <= 0.0f;
	}
	
	public bool IsAlive
	{
		get => !IsDead;
	}

	public void ApplyDamage(float amount)
	{
		m_CurrentHealth -= amount;
	}

	private void Update()
	{
		if (IsDead)
		{
			m_LigamentChain.DetachChain(true);
			Destroy(gameObject);
		}
		else
		{
			m_LigamentChain.SetResponsiveness(NormalizeHealth);
		}
	}
}
