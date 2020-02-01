using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

[RequireComponent(typeof(LigamentChain))]
public class LimbController : MonoBehaviour
{
#if GGJ_DEBUG
	[SerializeField]
	private bool m_ApplyDebugOverrides = true;

	[SerializeField]
    private LigamentChain m_debugEndTransform = null;

	[SerializeField]
	private LimbAction m_debugAction = null;
#endif

	[SerializeField]
	private float m_ExtendAngle = 0.0f;

	[SerializeField]
	private float m_RetractAngle = 0.0f;

	[SerializeField]
	private float m_ActionSnapiness = 1.0f;

	private LigamentChain m_LimbEnd;

	private float m_ActionValue = 0.5f;
	private LimbAction m_AssignedAction = null;

    private List<LigamentChain> m_ligamentChains = new List<LigamentChain>();    
    private float m_limbHealth = 1f;
	
    private void Start()
    {
        BuildLimb(null);
    }

	public void AssignAction(LimbAction action)
	{
		m_AssignedAction = action;
	}

	public int LigamentChainCount
	{
		get => m_ligamentChains.Count;
	}

	public Vector3 ArmDirection
	{
		get => (m_LimbEnd.transform.position - transform.position).normalized;
	}

    private void Update()
    {
		float targetValue = m_AssignedAction != null ? m_AssignedAction.Value : 0.5f;
		
		if (targetValue < m_ActionValue)
		{
			m_ActionValue = Mathf.Max(targetValue, m_ActionValue - m_ActionSnapiness * Time.deltaTime);
		}
		else if (targetValue > m_ActionValue)
		{
			m_ActionValue = Mathf.Min(targetValue, m_ActionValue + m_ActionSnapiness * Time.deltaTime);
		}

		SetExtendTarget(m_ActionValue);
	}

	public void BuildLimb(LigamentChain endJoint)
	{
#if GGJ_DEBUG
		if (m_ApplyDebugOverrides)
		{
			if (m_debugAction != null)
			{
				AssignAction(m_debugAction);
			}

			endJoint = m_debugEndTransform == null ? endJoint : m_debugEndTransform;
		}
#endif

		BuildLimb(GetComponent<LigamentChain>(), endJoint);
	}

	/// Builds limb going up the hierarchy from start ot end.
	/// Assumption made that end is always root and start is always a tree leaf.
	private void BuildLimb(LigamentChain start, LigamentChain end)
    {
		Assert.IsNotNull(start, "Build limb start is null.");
        Assert.IsNotNull(end, "Build limb end is null.");

		m_ligamentChains.Clear();

		LigamentChain current = end;
		int count = 0;

        // Build limb starting at end going up the chain until we reach this scripts transform.
        while (current != start && current != null)
		{
			bool hasLimbIdentity = current.TryGetComponent(out LimbIdentity limbIdentity);
			Assert.IsFalse(hasLimbIdentity);
            Assert.IsTrue(current.TryGetComponent(out Ligament ligament));
			
			if (!hasLimbIdentity)
			{
				m_ligamentChains.Add(current);
				LimbIdentity newLimbIdentity = current.gameObject.AddComponent<LimbIdentity>();
				newLimbIdentity.LimbController = this;
				newLimbIdentity.LigamentIndex = count;
			}

			current = current.transform.parent.GetComponent<LigamentChain>();
			count++;
        }

		// Work out elbow location
		Vector3 elbowLocation = Vector3.zero;
		if (m_ligamentChains.Count != 0)
		{
			foreach (var ligChain in m_ligamentChains)
			{
				elbowLocation += ligChain.transform.position;
			}
			elbowLocation /= m_ligamentChains.Count;
		}
		else
		{
			elbowLocation = (start.transform.position + end.transform.position) * 0.5f;
		}

		// TODO - Handle if they just built a straight line?

		bool rootHasJoin = end.TryGetComponent(out LigamentChain rootJoint);
		Assert.IsTrue(rootHasJoin);
		if (rootHasJoin)
		{
			m_ligamentChains.Add(rootJoint);
			LimbIdentity newLimbIdentity = end.gameObject.AddComponent<LimbIdentity>();
			newLimbIdentity.LimbController = this;
			newLimbIdentity.LigamentIndex = count;
		}
		m_LimbEnd = end;

#if GGJ_DEBUG
		Debug.Log($"Number of ligament chains in limb: {m_ligamentChains.Count}");
#endif
	}
	
	public void SetExtendTarget(float target)
	{
		float angle = Mathf.LerpAngle(m_RetractAngle / m_ligamentChains.Count, m_ExtendAngle / m_ligamentChains.Count, target);
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.right);

		foreach (LigamentChain ligChain in m_ligamentChains)
		{
			ligChain.RotateTowards(rotation);
		}
	}
}
