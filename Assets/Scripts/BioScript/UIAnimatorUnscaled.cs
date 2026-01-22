using UnityEngine;

public class UIAnimatorUnscaled : MonoBehaviour
{
    void Awake()
    {
        var anim = GetComponent<Animator>();
        if (anim != null)
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
}