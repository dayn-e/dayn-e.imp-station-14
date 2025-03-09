using Content.Server._Impstation.Weapons.Melee.Systems;
using Content.Shared._Impstation.Chemistry.Components.SolutionManager;
using Content.Shared.Weapons.Melee;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Impstation.EntityEffects.Effects;

public sealed partial class UnarmedAttackModifier : EntityEffect
{
    /// <summary>
    /// How much the entities' attack rate is multiplied by.
    /// </summary>
    [DataField]
    public float AttackRateModifier { get; set; } = 4;


    /// <summary>
    /// How long the modifier applies (in seconds).
    /// Is scaled by reagent amount if used with an EntityEffectReagentArgs.
    /// </summary>
    [DataField]
    public float StatusLifetime = 2f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-unarmed-attack-modifier",
            ("chance", Probability),
            ("attackrate", AttackRateModifier),
            ("time", StatusLifetime));
    }

    /// <summary>
    /// Remove reagent at set rate, changes the attack speed modifiers
    /// </summary>
    public override void Effect(EntityEffectBaseArgs args)
    {
        var metabolismStatus = args.EntityManager.EnsureComponent<UnarmedAttackModifierMetabolismComponent>(args.TargetEntity);
        var weaponStatus = args.EntityManager.EnsureComponent<MeleeWeaponComponent>(args.TargetEntity);

        if (!metabolismStatus.BaseAttackTracked)
        {
            metabolismStatus.BaseAttackRate = weaponStatus.AttackRate;
            metabolismStatus.BaseAttackTracked = true;
        }

        var isModified = !metabolismStatus.AttackRateModifier.Equals(AttackRateModifier);

        metabolismStatus.AttackRateModifier = AttackRateModifier;

        var statusLifetime = StatusLifetime;
        if (args is EntityEffectReagentArgs reagentArgs)
            statusLifetime *= reagentArgs.Scale.Float();


        IncreaseTimer(metabolismStatus, statusLifetime);

        if (isModified)
        {
            args.EntityManager.System<UnarmedAttackModifierMetabolismSystem>()
                .RefreshUnarmedAttackModifier(args.TargetEntity, metabolismStatus, weaponStatus);
        }
    }

    public void IncreaseTimer(UnarmedAttackModifierMetabolismComponent status, float time)
    {
        var gameTiming = IoCManager.Resolve<IGameTiming>();

        var offsetTime = Math.Max(status.ModifierTimer.TotalSeconds, gameTiming.CurTime.TotalSeconds);

        status.ModifierTimer = TimeSpan.FromSeconds(offsetTime + time);
        status.Dirty();
    }
}
