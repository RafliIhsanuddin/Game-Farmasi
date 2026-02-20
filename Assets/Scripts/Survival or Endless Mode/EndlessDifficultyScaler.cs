using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EndlessDifficultyScaler : MonoBehaviour
{
    public enum DifficultyGrowthMode
    {
        Increment,
        Multiplier
    }

    [Header("Base Enemy Count")]
    [SerializeField] private int baseWave1Count = 10;
    [SerializeField] private int baseWave2Count = 18;

    // NEW (wave 3)
    [SerializeField] private int baseWave3Count = 26;


    [Header("Growth Mode")]
    [SerializeField] private DifficultyGrowthMode growthMode =
        DifficultyGrowthMode.Increment;


    [Header("Per Cycle Increment")]
    [SerializeField] private int wave1Increment = 3;
    [SerializeField] private int wave2Increment = 5;

    // NEW
    [SerializeField] private int wave3Increment = 7;


    [Header("Per Cycle Multiplier")]
    [SerializeField] private float wave1Multiplier = 1.1f;
    [SerializeField] private float wave2Multiplier = 1.1f;

    // NEW
    [SerializeField] private float wave3Multiplier = 1.1f;


    public int CurrentCycle { get; private set; } = 0;


    // ========================================
    // WAVE 1 (UNCHANGED)
    // ========================================
    public int GetWave1Count()
    {
        int value;

        if (growthMode == DifficultyGrowthMode.Increment)
        {
            value =
                baseWave1Count +
                wave1Increment * CurrentCycle;
        }
        else
        {
            float fVal =
                baseWave1Count *
                Mathf.Pow(wave1Multiplier, CurrentCycle);

            value =
                Mathf.RoundToInt(fVal);
        }

        return Mathf.Max(1, value);
    }


    // ========================================
    // WAVE 2 (UNCHANGED)
    // ========================================
    public int GetWave2Count()
    {
        int value;

        if (growthMode == DifficultyGrowthMode.Increment)
        {
            value =
                baseWave2Count +
                wave2Increment * CurrentCycle;
        }
        else
        {
            float fVal =
                baseWave2Count *
                Mathf.Pow(wave2Multiplier, CurrentCycle);

            value =
                Mathf.RoundToInt(fVal);
        }

        return Mathf.Max(1, value);
    }


    // ========================================
    // WAVE 3 (NEW, SAME PATTERN)
    // ========================================
    public int GetWave3Count()
    {
        int value;

        if (growthMode == DifficultyGrowthMode.Increment)
        {
            value =
                baseWave3Count +
                wave3Increment * CurrentCycle;
        }
        else
        {
            float fVal =
                baseWave3Count *
                Mathf.Pow(wave3Multiplier, CurrentCycle);

            value =
                Mathf.RoundToInt(fVal);
        }

        return Mathf.Max(1, value);
    }


    // ========================================
    // ADVANCE (UNCHANGED)
    // ========================================
    public void AdvanceCycle()
    {
        CurrentCycle++;

        Debug.Log(
            $"[EndlessDifficulty] Advance → Cycle = {CurrentCycle + 1}");
    }


    // ========================================
    // RESET (UNCHANGED)
    // ========================================
    public void ResetDifficulty()
    {
        CurrentCycle = 0;

        Debug.Log(
            "[EndlessDifficulty] Reset → Cycle = 1");
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(EndlessDifficultyScaler))]
public class EndlessDifficultyScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;

        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromMonoBehaviour(
                (EndlessDifficultyScaler)target),
            typeof(EndlessDifficultyScaler),
            false);

        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Base Enemy Count",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("baseWave1Count"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("baseWave2Count"));

        // NEW
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("baseWave3Count"));


        EditorGUILayout.Space();


        EditorGUILayout.LabelField(
            "Growth Mode",
            EditorStyles.boldLabel);

        var modeProp =
            serializedObject.FindProperty("growthMode");

        EditorGUILayout.PropertyField(modeProp);

        var mode =
            (EndlessDifficultyScaler.DifficultyGrowthMode)
            modeProp.enumValueIndex;


        EditorGUILayout.Space();


        if (mode ==
            EndlessDifficultyScaler.DifficultyGrowthMode.Increment)
        {
            EditorGUILayout.LabelField(
                "Per Cycle Increment",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave1Increment"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave2Increment"));

            // NEW
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave3Increment"));
        }
        else
        {
            EditorGUILayout.LabelField(
                "Per Cycle Multiplier",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave1Multiplier"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave2Multiplier"));

            // NEW
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("wave3Multiplier"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif