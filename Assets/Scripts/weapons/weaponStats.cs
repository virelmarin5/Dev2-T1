using UnityEngine;

public abstract class weaponStats : ScriptableObject
{
    [Header("Model")]
    public GameObject weaponModel;

    [Header("Damage")]
    [Range(.1f,5)][SerializeField] public float attackRate;

    [Header("Throw Settings")]
    [Range(5, 40)][SerializeField] public float throwForce = 25f;

    public abstract void Attack(weaponManager manager);
}