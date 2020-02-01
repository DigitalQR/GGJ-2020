using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

[RequireComponent(typeof(LigamentChain))]
public class LimbController : MonoBehaviour
{
#if GGJ_DEBUG
    [SerializeField]
    private Transform m_debugEndTransform = null;
#endif

    private KeyCode m_keyExtend = KeyCode.None;
    private KeyCode m_keyRetract = KeyCode.None;

    private List<LigamentChain> m_ligamentChains = new List<LigamentChain>();
    private float m_limbHealth = 1f;

    public void BuildLimb(Transform endJoint)
    {
#if GGJ_DEBUG
        endJoint = m_debugEndTransform == null ? endJoint : m_debugEndTransform;        
#endif

        BuildLimb(endJoint, transform);
    }

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

    private void Update()
    {
        if(Input.GetKeyUp(m_keyExtend))
        {
            ExtendLimb();
        }
        else if(Input.GetKeyUp(m_keyRetract))
        {
            RetractLimb();
        }
    }

    /// Builds limb going up the hierarchy from start ot end.
    /// Assumption made that end is always root and start is always a tree leaf.
    private void BuildLimb(Transform start, Transform end)
    {
        Assert.IsNotNull(start, "Build limb start is null.");
        Assert.IsNotNull(end, "Build limb end is null.");

        Transform current = start;

        // Build limb starting at end going up the chain until we reach this scripts transform.
        while (current != end)
        {
            Assert.IsFalse(current.TryGetComponent(out LimbIdentity limbIdentity));
            Assert.IsTrue(current.TryGetComponent(out LigamentChain newJoint));
            Assert.IsTrue(current.TryGetComponent(out Ligament ligament));
            
            // Add the ligament to the limb.
            m_ligamentChains.Add(newJoint);
            current.gameObject.AddComponent<LimbIdentity>();

            current = current.parent.transform;
        }

        Assert.IsTrue(end.TryGetComponent(out LigamentChain rootJoint));
        m_ligamentChains.Add(rootJoint);

#if GGJ_DEBUG
        Debug.Log($"Number of ligament chains in limb: {m_ligamentChains.Count}");
#endif
    }

    private void ExtendLimb()
    {
        foreach(LigamentChain ligChain in m_ligamentChains)
        {
            // TODO Implement to extend
        }
    }

    private void RetractLimb()
    {
        foreach (LigamentChain ligChain in m_ligamentChains)
        {
            // TODO Implement to retract
        }
    }
}
