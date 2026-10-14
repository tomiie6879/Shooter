using UnityEngine;

public sealed class SpiritController : MonoBehaviour
{
    private ISpiritAttack[] attacks;
    public bool HasAttack
    {
        get
        {
            foreach (var component in GetComponentsInChildren<MonoBehaviour>(true))
                if (component is ISpiritAttack) return true;
            return false;
        }
    }
    public void Initialize(SpiritContext context, SpiritData data)
    {
        var found = new System.Collections.Generic.List<ISpiritAttack>();
        foreach (var component in GetComponentsInChildren<MonoBehaviour>(true))
            if (component is ISpiritAttack attack)
            {
                attack.Initialize(context, data);
                attack.SetCombatEnabled(false);
                found.Add(attack);
            }
        attacks = found.ToArray();
    }
    public void SetCombatEnabled(bool value)
    {
        if (attacks == null) return;
        foreach (var attack in attacks) attack.SetCombatEnabled(value);
    }
    private void OnDisable() => SetCombatEnabled(false);
}
