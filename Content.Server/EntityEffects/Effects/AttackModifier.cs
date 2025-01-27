using Content.Shared.Chemistry.Components;
using Content.Shared.Weapons.Melee;
using Content.Shared.EntityEffects;
using Content.Shared.Movement.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.EntityEffects.Effects;

public sealed partial class AttackModifier : EntityEffect
{
    /// <summary>
    /// How much the entities' attack rate is multiplied by.
    /// </summary>
    [DataField]
    public float RateModifier { get; set; } = 1;

    /// <summary>
    /// Whether the effect is meant for unarmed attacks only
    /// </summary>
    [DataField]
    public bool UnarmedOnly { get; set; } = true;

    /// <summary>
    /// How long the modifier applies (in seconds).
    /// Is scaled by reagent amount if used with an EntityEffectReagentArgs.
    /// </summary>
    [DataField]
    public float StatusLifetime = 2f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-attack-modifier",
        ("chance", Probability),
        ("attackrate", RateModifier),
        ("time", StatusLifetime));
    }

    /// <summary>
    /// Remove reagent at set rate, changes the attack speed modifiers and adds a AttackRateModifierMetabolismComponent if not already there.
    /// </summary>
    public override void Effect(EntityEffectBaseArgs args)
    {
        var meleeWeaponComponentStatus = args.EntityManager.EnsureComponent<MeleeWeaponComponent>(args.TargetEntity);
        var statusLifetime = StatusLifetime;

        if (args is EntityEffectReagentArgs reagentArgs)
        {
            statusLifetime *= reagentArgs.Scale.Float();
        }

        // Only apply on an unarmed hand
        if (UnarmedOnly && meleeWeaponComponentStatus.AltDisarm)
        {
            meleeWeaponComponentStatus.AttackRate = RateModifier;
            meleeWeaponComponentStatus.AutoAttack = true;
        }
    }
}
