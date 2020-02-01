using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigamentHierarchy : MonoBehaviour
{
	/*
	[SerializeField]
	private PlaceholderLigament m_RootLigament = null;

	public float CalculateDistanceFromRoot(PlaceholderLigament Ligament)
	{
		Debug.AssertFormat(Ligament.GetComponentInParent<LigamentHierarchy>() == this, "Trying to query hierarchy which doesn't contain Ligament");

		if (Ligament.GetComponentInParent<LigamentHierarchy>() == this)
		{
			if (Ligament.gameObject == m_RootLigament.gameObject)
				return 0.0f;

			PlaceholderLigament parentLigament = Ligament.transform.parent.GetComponent<PlaceholderLigament>();
			float distSqrd = (parentLigament.transform.position - Ligament.transform.position).magnitude;

			return distSqrd + CalculateDistanceFromRoot(parentLigament);
		}

		return 0.0f;
	}

	public float CalculateDistance(PlaceholderLigament a, PlaceholderLigament b)
	{
		return CalculateDistanceFromRoot(b) - CalculateDistanceFromRoot(a);
	}
	*/
}
