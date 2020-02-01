using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LigamentChain : MonoBehaviour
{
	[SerializeField]
	private Transform m_BoneStart = null;

	[SerializeField]
	private Transform m_BoneEnd = null;

	[Header("Control")]
	[SerializeField]
	private float m_FullResponseSpringiness = 2000.0f;

	[SerializeField]
	private float m_FullResponseDamper = 0.0f;

	[SerializeField]
	private float m_NoResponseSpringiness = 100.0f;

	[SerializeField]
	private float m_NoResponseDamper = 0.0f;

	private Rigidbody m_Body;
	private ConfigurableJoint m_Joint;
	private Quaternion m_BaseRotation;
	private bool m_HasInitalized = false;

	private LigamentChain m_ParentChain = null;

	public Transform BoneStart { get { return m_BoneStart; } }
	public Transform BoneEnd { get { return m_BoneEnd; } }

	private void Awake()
	{
		if (!m_HasInitalized)
		{
			m_HasInitalized = true;
			m_Body = GetComponent<Rigidbody>();
			m_Joint = GetComponent<ConfigurableJoint>();

			Debug.AssertFormat(m_Joint != null, "LigamentChain needs joint on init");
		}
	}

	private void Start()
    {
		m_BaseRotation = transform.localRotation;
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
			//m_Joint.targetRotation = m_BaseRotation;

			m_Joint.autoConfigureConnectedAnchor = true;
			m_Body.position = transform.position = parent.m_BoneEnd.position - (m_BoneStart.position - transform.position);

			m_Joint.configuredInWorldSpace = false;
			m_Joint.connectedAnchor = parent.m_BoneEnd.position - transform.position;
			//m_Joint.anchor = parent.m_BoneEnd.position - transform.position;
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

	/// <summary>
	/// Remove all remaining parts from this chain
	/// </summary>
	public void DetachChain(bool ragdollChildren)
	{
		for (int i = 0; i < transform.childCount; ++i)
		{
			Transform child = transform.GetChild(i);

			if (ragdollChildren)
			{
				if (child.TryGetComponent(out LigamentChain chain))
				{
					chain.DetachFromParentChain();
					chain.transform.parent = null;
					Destroy(chain.m_Joint);
					--i;
				}
			}
		}
	}

	public void RotateTowards(Quaternion rotation)
	{
		Quaternion localSpaceRot = rotation;// Quaternion.Inverse(m_Body.rotation) * rotation;

		m_Joint.targetRotation = localSpaceRot * m_BaseRotation;// Quaternion.Inverse(transform.localRotation) * rotation * m_BaseRotation;
		return;

		/*
		// https://forum.unity.com/threads/quaternion-wizardry-for-configurablejoint.8919/

		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = m_Joint.axis;
		var forward = Vector3.Cross(m_Joint.axis, m_Joint.secondaryAxis).normalized;
		var up = Vector3.Cross(forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		if (false)//space == Space.World)
		{
			//resultRotation *= m_BaseRotation * Quaternion.Inverse(rotation);
		}
		else
		{
			resultRotation *= Quaternion.Inverse(rotation) * m_BaseRotation;
		}

		// Transform back into joint space
		resultRotation *= worldToJointSpace;

		// Set target rotation to our newly calculated rotation
		m_Joint.targetRotation = resultRotation;
		*/

		//m_Joint.targetRotation = rotation;// m_BaseRotation * rotation;
	}

	public void SetResponsiveness(float resp)
	{
		if (m_Joint != null)
		{
			float springiness = Mathf.Lerp(m_NoResponseSpringiness, m_FullResponseSpringiness, resp);
			float damper = Mathf.Lerp(m_NoResponseDamper, m_FullResponseDamper, resp);

			var drive = m_Joint.slerpDrive;
			drive.positionSpring = springiness;
			drive.positionDamper = damper;
			m_Joint.slerpDrive = drive;
		}
	}
}
