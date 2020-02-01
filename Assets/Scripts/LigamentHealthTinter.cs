using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ligament))]
public class LigamentHealthTinter : MonoBehaviour
{
	[SerializeField]
	private Renderer m_TargetRenderer = null;

	private Color m_BaseColour;
	private Ligament m_Ligament;

	private void Start()
    {
		m_Ligament = GetComponent<Ligament>();
		m_BaseColour = m_TargetRenderer.material.color;
	}

	private void Update()
    {
		if (m_Ligament != null)
		{
			m_TargetRenderer.material.color = m_BaseColour * m_Ligament.NormalizeHealth;
		}
    }
}
