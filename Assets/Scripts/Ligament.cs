using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LigamentChain)), RequireComponent(typeof(Collider))]
public class Ligament : MonoBehaviour
{
    private LimbController m_limbController = null;
    private LigamentChain m_ligamentChain = null;

    private float m_health = 1f;

    public void RegisterLimbController(LimbController limbController)
    {
        m_limbController = limbController;
    }

    private void Start()
    {
        m_ligamentChain = GetComponent<LigamentChain>();
    }

    private void Update()
    {        
        //TODO: Update the ligament chain with new settings on how it should behave.
    }
}
