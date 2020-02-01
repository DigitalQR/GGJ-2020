using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceApply : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_Force;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		if (TryGetComponent(out Rigidbody body))
		{
			body.AddForceAtPosition(m_Force, body.centerOfMass);
		}
    }
}
