using Content.Shared._Impstation.Chemistry.Components.SolutionManager;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Timing;

namespace Content.Server._Impstation.Weapons.Melee.Systems;

public sealed class UnarmedAttackModifierMetabolismSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public void RefreshUnarmedAttackModifiers(EntityUid targetEntity,
        UnarmedAttackModifierMetabolismComponent? metabolismComponent = null,
        MeleeWeaponComponent? attackComponent = null)
    {
        if (!Resolve(targetEntity, ref attackComponent, false) ||
            !Resolve(targetEntity, ref metabolismComponent, false))
            return;

        if (_timing.ApplyingState)
            return;

        if (MathHelper.CloseTo(
                (metabolismComponent.BaseAttackRate / metabolismComponent.AttackRateModifier),
                attackComponent.AttackRate))
            return;

        if (_timing.CurTime > metabolismComponent.ModifierTimer)
        {
            EntityManager.RemoveComponent<UnarmedAttackModifierMetabolismComponent>(targetEntity);
            return;
        }

        attackComponent.AttackRate = metabolismComponent.BaseAttackRate * metabolismComponent.AttackRateModifier;
        // Auto attack is enabled only if the weapon is "unarmed", i.e. has AltDisarm
        attackComponent.AutoAttack = attackComponent.AltDisarm;
        Dirty(targetEntity, attackComponent);
    }
}
