using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.Chemistry.Components.SolutionManager;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class UnarmedAttackModifierMetabolismComponent : Component
{
    [AutoNetworkedField, ViewVariables]
    public float AttackRateModifier { get; set; }

    /// <summary>
    /// What the entity's base attack is
    /// </summary>
    [DataField]
    public float BaseAttackRate { get; set; }

    /// <summary>
    /// Whether the base attack is kept
    /// </summary>
    [DataField]
    public bool BaseAttackTracked { get; set; } = false;

    /// <summary>
    /// When the current modifier is expected to end.
    /// </summary>
    [AutoNetworkedField, ViewVariables]
    public TimeSpan ModifierTimer { get; set; } = TimeSpan.Zero;
}
