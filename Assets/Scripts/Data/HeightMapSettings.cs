using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpdatableData/HeightMapSettings")]
public class HeightMapSettings : UpdatableData {

    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier = 50f;
    public AnimationCurve heightCurve;

    public float minHeight => heightMultiplier * heightCurve.Evaluate(0);
    public float maxHeight => heightMultiplier * heightCurve.Evaluate(1);

#if UNITY_EDITOR

    protected override void OnValidate() {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }

#endif

}
