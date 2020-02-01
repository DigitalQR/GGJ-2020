using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint)), RequireComponent(typeof(Rigidbody))]
public class LigamentChain : MonoBehaviour
{
	[SerializeField]
	private Transform m_BoneStart = null;

	[SerializeField]
	private Transform m_BoneEnd = null;

	private Rigidbody m_Body;
	private ConfigurableJoint m_Joint;

	private LigamentChain m_ParentChain = null;

	public Transform BoneStart { get { return m_BoneStart; } }
	public Transform BoneEnd { get { return m_BoneEnd; } }

	private void Start()
    {
		m_Body = GetComponent<Rigidbody>();
		m_Joint = GetComponent<ConfigurableJoint>();
		UpdateChildren();
	}

	public void UpdateChildren()
	{
		for (int i = 0; i < transform.childCount; ++i)
		{
			Transform child = transform.GetChild(i);

			if (child.TryGetComponent(out LigamentChain chain))
			{
				if (chain.m_ParentChain != chain)
				{
					chain.DetachFromParentChain();
					chain.AttachToParentChain(this);
				}
			}
		}
	}
	
    private void Update()
	{
		if ((m_ParentChain == null && transform.parent != null) || (m_ParentChain != null && transform.parent != m_ParentChain.transform))
		{
			DetachFromParentChain();

			if (transform.parent != null && transform.parent.TryGetComponent(out LigamentChain chain))
			{
				AttachToParentChain(chain);
			}
		}
	}

	private void AttachToParentChain(LigamentChain parent)
	{
		Debug.AssertFormat(m_ParentChain == null, "AttachToParent when parent already set");
		if (m_ParentChain == null)
		{
			m_ParentChain = parent;
			
			m_Joint.connectedBody = parent.m_Body;

			m_Joint.autoConfigureConnectedAnchor = true;
			m_Body.position = transform.position = parent.m_BoneEnd.position - m_BoneStart.localPosition;
			m_Joint.anchor = parent.m_BoneEnd.position - m_Body.position;
		}
	}

	private void DetachFromParentChain()
	{
		if (m_ParentChain != null)
		{
			m_ParentChain = null;
			m_Joint.connectedBody = null;
		}
	}
}
