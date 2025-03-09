using System.Linq;
using Content.Shared._Impstation.Chemistry.Components.SolutionManager;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Timing;

namespace Content.Server._Impstation.Weapons.Melee.Systems;

public sealed class UnarmedAttackModifierMetabolismSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    private readonly List<Entity<UnarmedAttackModifierMetabolismComponent>> _components = new();

    public override void Initialize()
    {
        base.Initialize();

        // UpdatesOutsidePrediction = true;

        SubscribeLocalEvent<UnarmedAttackModifierMetabolismComponent, ComponentStartup>(AddComponent);
        SubscribeLocalEvent<UnarmedAttackModifierMetabolismComponent, RefreshUnarmedAttackModifierEvent>(
            OnRefreshAttackRate);
    }

    private void OnRefreshAttackRate(EntityUid targetEntity,
        UnarmedAttackModifierMetabolismComponent metabolismComponent,
        RefreshUnarmedAttackModifierEvent args)
    {
        args.ModifyAttack(metabolismComponent.AttackRateModifier);
    }

    private void AddComponent(Entity<UnarmedAttackModifierMetabolismComponent> metabolismComponent,
        ref ComponentStartup args)
    {
        _components.Add(metabolismComponent);
    }

    public void RefreshUnarmedAttackModifier(EntityUid targetEntity,
        UnarmedAttackModifierMetabolismComponent? metabolismComponent = null,
        MeleeWeaponComponent? attackComponent = null)
    {
        if (!Resolve(targetEntity, ref attackComponent, false) ||
            !Resolve(targetEntity, ref metabolismComponent, false))
        {
            return;
        }


        if (_timing.ApplyingState)
            return;


        var ev = new RefreshUnarmedAttackModifierEvent();
        RaiseLocalEvent(targetEntity, ev);

        if (MathHelper.CloseTo(ev.AttackRateModifier, attackComponent.AttackRate))
            return;

        attackComponent.AttackRate = metabolismComponent.BaseAttackRate * ev.AttackRateModifier;
        // Auto attack is enabled only if the weapon is "unarmed", i.e. has AltDisarm
        // But also remove the auto-attack if the modifier is set back to 1
        attackComponent.AutoAttack = !MathHelper.CloseTo(ev.AttackRateModifier, 1) && attackComponent.AltDisarm;
        Dirty(targetEntity, attackComponent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_components.Count == 0)
            return;

        var currentTime = _timing.CurTime;

        var toRemove = _components.Where(metabolismComponent =>
                metabolismComponent.Comp.Deleted || metabolismComponent.Comp.ModifierTimer <= currentTime)
            .ToList();

        if (toRemove.Count == 0)
            return;


        toRemove.ForEach(metabolismComponent =>
        {
            _components.Remove(metabolismComponent);
            metabolismComponent.Comp.AttackRateModifier = 1;

            RefreshUnarmedAttackModifier(metabolismComponent.Owner,
                metabolismComponent,
                EntityManager.EnsureComponent<MeleeWeaponComponent>(metabolismComponent.Owner));
        });
    }

    private sealed class RefreshUnarmedAttackModifierEvent : EntityEventArgs
    {
        public float AttackRateModifier { get; private set; } = 1;

        public void ModifyAttack(float modifier)
        {
            AttackRateModifier *= modifier;
        }
    }
}
