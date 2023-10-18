using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityChange : MonoBehaviour
{
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
