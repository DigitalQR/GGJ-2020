using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (objectHit.TryGetComponent(out LigamentChain ligChain) || objectHit.TryGetComponent(out Head head))
            {
                Transform newLigament = Instantiate(m_boneBase, hit.point, Quaternion.identity);
                newLigament.SetParent(objectHit);
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
}
