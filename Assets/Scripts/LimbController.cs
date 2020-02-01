using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

[RequireComponent(typeof(LigamentChain))]
public class LimbController : MonoBehaviour
{
#if GGJ_DEBUG
    [SerializeField]
    private LigamentChain m_debugEndTransform = null;
#endif

	[SerializeField]
	private float m_ExtendAngle = 0.0f;

	[SerializeField]
	private float m_RetractAngle = 0.0f;
	
	private LigamentChain m_LimbEnd;
	private Transform m_ElbowArchTarget;
	private Transform m_ElbowHandTarget;

	private KeyCode m_keyExtend = KeyCode.A;
    private KeyCode m_keyRetract = KeyCode.D;

    private List<LigamentChain> m_ligamentChains = new List<LigamentChain>();    
    private float m_limbHealth = 1f;

    public void BindExtendKey(KeyCode keyCode)
    {
        m_keyExtend = keyCode;
    }

    public void BindRetractKey(KeyCode keyCode)
    {
        m_keyRetract = keyCode;
    }

    private void Start()
    {
        BuildLimb(null);
    }

	public Vector3 ArmDirection
	{
		get => (m_LimbEnd.transform.position - transform.position).normalized;
	}

	public Vector3 ElbowDirection
	{
		get
		{
			Vector3 elbowBaseCentre = (m_ElbowHandTarget.transform.position + transform.position) * 0.5f;
			return (m_ElbowArchTarget.transform.position - elbowBaseCentre).normalized;
		}
	}

	public Vector3 ElbowAxis
	{
		get
		{
			Vector3 elbowArmDir = (m_ElbowHandTarget.transform.position - transform.position).normalized;
			return Vector3.Cross(ElbowDirection, elbowArmDir);
		}
	}

    private void Update()
    {
        if(Input.GetKeyDown(m_keyExtend))
        {
			SetExtendTarget(1.0f);
        }
        else if(Input.GetKeyDown(m_keyRetract))
		{
			SetExtendTarget(0.0f);
		}

#if GGJ_DEBUG
		Vector3 midPoint = (transform.position + m_ElbowHandTarget.position) * 0.5f;
		Debug.DrawLine(transform.position, m_ElbowHandTarget.position, Color.green);
		Debug.DrawLine(transform.position, m_ElbowArchTarget.position, Color.yellow);
		Debug.DrawLine(m_ElbowArchTarget.position, m_ElbowHandTarget.position, Color.yellow);

		Debug.DrawLine(midPoint, m_ElbowArchTarget.position, Color.blue);
		Debug.DrawLine(midPoint, midPoint + ElbowAxis * 5.0f, Color.red);
#endif
	}

	public void BuildLimb(LigamentChain endJoint)
	{
#if GGJ_DEBUG
		endJoint = m_debugEndTransform == null ? endJoint : m_debugEndTransform;
#endif

		BuildLimb(GetComponent<LigamentChain>(), endJoint);
	}

	/// Builds limb going up the hierarchy from start ot end.
	/// Assumption made that end is always root and start is always a tree leaf.
	private void BuildLimb(LigamentChain start, LigamentChain end)
    {
        Assert.IsNotNull(start, "Build limb start is null.");
        Assert.IsNotNull(end, "Build limb end is null.");

		LigamentChain current = end;

        // Build limb starting at end going up the chain until we reach this scripts transform.
        while (current != start)
		{
			bool hasLimbIdentity = current.TryGetComponent(out LimbIdentity limbIdentity);
			Assert.IsFalse(hasLimbIdentity);
            Assert.IsTrue(current.TryGetComponent(out Ligament ligament));
			
			if (!hasLimbIdentity)
			{
				m_ligamentChains.Add(current);
				current.gameObject.AddComponent<LimbIdentity>();
			}

			current = current.transform.parent.GetComponent<LigamentChain>();
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

		m_ElbowArchTarget = new GameObject().transform;
		m_ElbowArchTarget.position = elbowLocation;
		m_ElbowArchTarget.parent = transform;
		m_ElbowArchTarget.gameObject.name = "Elbow Arch";

		m_ElbowHandTarget = new GameObject().transform;
		m_ElbowHandTarget.position = end.transform.position;
		m_ElbowHandTarget.parent = transform;
		m_ElbowArchTarget.gameObject.name = "Elbow Hand";

		// TODO - Handle if they just built a straight line?

		bool rootHasJoin = end.TryGetComponent(out LigamentChain rootJoint);
		Assert.IsTrue(rootHasJoin);
		if (rootHasJoin)
		{
			m_ligamentChains.Add(rootJoint);
		}
		m_LimbEnd = end;

#if GGJ_DEBUG
		Debug.Log($"Number of ligament chains in limb: {m_ligamentChains.Count}");
#endif
	}
	
	public void SetExtendTarget(float target)
	{
		float angle = Mathf.LerpAngle(m_RetractAngle / m_ligamentChains.Count, m_ExtendAngle / m_ligamentChains.Count, target);
		Vector3 axis = ElbowAxis;
#if GGJ_DEBUG
		//Debug.Log($"Limb Extend Target: {angle} ({m_ligamentChains.Count})" );
#endif
		
		//Quaternion rotation = Quaternion.AngleAxis(angle, axis);
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.right);

		foreach (LigamentChain ligChain in m_ligamentChains)
		{
			ligChain.RotateTowards(rotation);
		}
	}
}
