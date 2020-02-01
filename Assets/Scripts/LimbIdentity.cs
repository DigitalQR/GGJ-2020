using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbIdentity : MonoBehaviour
{
    public LimbController LimbController = null;
    public int LigamentIndex = 0;
    public bool IsLastLigament { get { return 0 == LigamentIndex; } }
}
