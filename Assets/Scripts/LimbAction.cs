using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LimbAction : MonoBehaviour
{
	[SerializeField]
	private int m_ActionIndex = 0;

	[SerializeField]
	private bool m_Invert = false;

	[SerializeField]
	private LimbActionGroup m_TargetGroup = null;
	
	public float Value
	{
		get => m_TargetGroup.GetValue(m_ActionIndex, m_Invert);
	}
}
