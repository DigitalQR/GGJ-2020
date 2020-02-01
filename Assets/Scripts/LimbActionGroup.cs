using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LimbActionGroup : MonoBehaviour
{
	[SerializeField]
	private int m_GroupCount = 4;
	
	private float[] m_CurrentValues = null;

	private void Awake()
	{
		if (m_CurrentValues == null)
		{
			m_CurrentValues = new float[m_GroupCount];
		}
	}

	public float GetValue(int group, bool invert)
	{
		return invert ? 1.0f - m_CurrentValues[group] : m_CurrentValues[group];
	}

	public void OnActionGroup_0(InputValue value)
	{
		m_CurrentValues[0] = Mathf.Clamp01(value.Get<float>());
	}

	public void OnActionGroup_1(InputValue value)
	{
		m_CurrentValues[1] = Mathf.Clamp01(value.Get<float>());
	}

	public void OnActionGroup_2(InputValue value)
	{
		m_CurrentValues[2] = Mathf.Clamp01(value.Get<float>());
	}

	public void OnActionGroup_3(InputValue value)
	{
		m_CurrentValues[3] = Mathf.Clamp01(value.Get<float>());
	}
}
