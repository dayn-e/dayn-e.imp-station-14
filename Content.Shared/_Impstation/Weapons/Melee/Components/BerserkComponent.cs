using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.Weapons.Melee.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class BerserkComponent : Component
{
    [DataField, ViewVariables]
    public float AttackRate;

    [DataField, ViewVariables]
    public bool AutoAttack;
}
