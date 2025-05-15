using Content.Shared.Weapons.Melee;
using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.Weapons.Melee.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class BerserkComponent : Component
{
    [DataField, ViewVariables]
    public float AttackRate = 4;

    [DataField, ViewVariables]
    public bool AutoAttack = true;

    public float? OriginalAttackRate { get; set; }
    public bool? OriginalAutoAttack { get; set; }
}
