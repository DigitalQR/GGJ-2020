using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ligament : MonoBehaviour
{
    private LigamentChain m_ligamentChain = null;

    private float m_health = 1f;
    public bool Alive { get; private set; } = true;

    private void Start()
    {
        m_ligamentChain = GetComponent<LigamentChain>();
    }

    private void Update()
    {        
        //TODO: Update the ligament chain with new settings on how it should behave.
    }

    public void ApplyDamage(float damage)
    {
        m_health -= damage;
        if(m_health <= 0f)
        {
            m_health = 0f;
            Alive = false;
        }
    }
}
