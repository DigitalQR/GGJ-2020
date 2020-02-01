using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CreatureBuilder : MonoBehaviour
{
    [SerializeField]
    private float m_distanceFromCamera = 15f;

    [SerializeField]
    private Transform m_headPrefab;

    [SerializeField]
    private Transform m_boneBase;

    private Camera m_camera = null;
    private bool m_headPlacedInScene;

    private void Start()
    {
        m_camera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            LeftMouseButton();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            RightMouseButton();                       
        }
    }

    private void LeftMouseButton()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Add at specific point so we can have branches and T junctions.
            Transform objectHit = hit.transform;

            bool hitHead = objectHit.TryGetComponent(out Head head);
            bool hitLimb = objectHit.TryGetComponent(out LimbIdentity limbIdentity);

            if(hitHead)
            {
                Transform newLigament = CreateNewLigament(hit);
                MakeLimbRoot(newLigament);
            }

            if(hitLimb)
            {
                Transform newLigament = CreateNewLigament(hit);

                if (limbIdentity.IsLastLigament)
                {
                    bool newLigamentHasChain = newLigament.TryGetComponent(out LigamentChain newLigamentChain);
                    Assert.IsTrue(newLigamentHasChain);

                    limbIdentity.LimbController.BuildLimb(newLigamentChain);
                }
                else
                {
                    MakeLimbRoot(newLigament);
                }                
            }            
        }
    }

    private void RightMouseButton()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Head head))
            {
                Destroy(objectHit.gameObject);
                m_headPlacedInScene = false;
            }
        }
        else if (!m_headPlacedInScene)
        {
            Vector3 newHeadPosition = ray.direction * m_distanceFromCamera + m_camera.transform.position;
            Instantiate(m_headPrefab, newHeadPosition, Quaternion.identity);
            m_headPlacedInScene = true;
        }
    }

    private Transform CreateNewLigament(RaycastHit hit)
    {
        Transform newLigament = Instantiate(m_boneBase, hit.point, Quaternion.identity);
        newLigament.SetParent(hit.transform);

        return newLigament;
    }

    private void MakeLimbRoot(Transform ligament)
    {
        LimbController newLimbController = ligament.gameObject.AddComponent<LimbController>();
        bool newLigamentHasChain = ligament.TryGetComponent(out LigamentChain newLigamentChain);
        Assert.IsTrue(newLigamentHasChain);

        newLimbController.BuildLimb(newLigamentChain);
    }
}
