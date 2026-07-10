using UnityEngine;

public abstract class weaponStats : ScriptableObject
{
    [Header("Model")]
    public GameObject weaponModel;

    [Header("Damage")]
    [Range(.1f,5)][SerializeField] public float attackRate;

    public abstract void Attack(weaponManager manager);
}