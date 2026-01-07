using System;
using System.Collections.Generic;
using Server.Engines.ConPVP;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Second;
using Server.Spells.Spellweaving;
using Server.Targeting;

namespace Server.Spells
{
    public abstract class Spell : ISpell
    {
        private static readonly TimeSpan NextSpellDelay = TimeSpan.FromSeconds(0.75);
        private static readonly TimeSpan AnimateDelay = TimeSpan.FromSeconds(1.5);
        // In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only
        // Expo & Magic Arrow have enough delay and a short enough cast time to bring up
        // the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
        // of course, two MA's won't.

        private static readonly Dictionary<Type, DelayedDamageContextWrapper> _contextTable = new();

        private AnimTimer _animTimer;
        private CastTimer _castTimer;
        private CastDelayTimer _castDelayTimer;

        public Spell(Mobile caster, Item scroll, SpellInfo info)
        {
            Caster = caster;
            Scroll = scroll;
            Info = info;
        }

        public SpellState State { get; set; }

        public Mobile Caster { get; }

        public SpellInfo Info { get; }

        // Sphere51a: Track mana consumed for 50% refund on fizzle
        public int ManaConsumed { get; set; }

        // Sphere51a: Store target mobile for delayed execution
        public Mobile TargetMobile { get; set; }

        // Sphere51a: Store spell execution callback for delayed execution
        public Action SpellEffect { get; set; }

        // Sphere51a: Check if delay timer is running (for target finish logic)
        public bool HasDelayTimerRunning => _castDelayTimer != null;

        // Sphere51a: Track the spell that was interrupted by this spell (for 50% mana penalty)
        public Spell InterruptedSpell { get; set; }

        public string Name => Info.Name;
        public string Mantra => Info.Mantra;
        public Type[] Reagents => Info.Reagents;
        public Item Scroll { get; }

        public long StartCastTime { get; private set; }

        public virtual SkillName CastSkill => SkillName.Magery;
        public virtual SkillName DamageSkill => SkillName.EvalInt;

        public virtual bool RevealOnCast => true;
        public virtual bool ClearHandsOnCast => true;
        public virtual bool ShowHandMovement => true;

        public virtual bool DelayedDamage => false;

        public static readonly Type[] AOSNoDelayedDamageStackingSelf = Core.AOS ? Array.Empty<Type>() : null;

        // Null means stacking is allowed while empty indicates no stacking with self
        // More than zero means no stacking with self and other spells
        public virtual Type[] DelayedDamageSpellFamilyStacking => null;

        public virtual bool BlockedByHorrificBeast => true;
        public virtual bool BlockedByAnimalForm => true;
        // Sphere51a: Players can move freely during casting
        public virtual bool BlocksMovement => IsCasting && !Caster.Player;

        public virtual bool CheckNextSpellTime => Scroll is not BaseWand;

        public virtual int CastRecoveryBase => 6;
        public virtual int CastRecoveryFastScalar => 1;
        public virtual int CastRecoveryPerSecond => 4;
        public virtual int CastRecoveryMinimum => 0;

        public abstract TimeSpan CastDelayBase { get; }

        public virtual double CastDelayFastScalar => 1;
        public virtual double CastDelaySecondsPerTick => 0.25;
        public virtual TimeSpan CastDelayMinimum => TimeSpan.FromSeconds(0.25);

        public virtual bool IsCasting => State == SpellState.Casting;

        public virtual void OnCasterHurt()
        {
            // Sphere51a: Per specification line 293, "Players or NPCs do not have hurt-based interruptions"
            // Damage does NOT interrupt spell casting for anyone
        }

        public virtual void OnCasterKilled()
        {
            Disturb(DisturbType.Kill);
        }

        public virtual void OnConnectionChanged()
        {
            FinishSequence();
        }

        public virtual bool OnCasterMoving(Direction d)
        {
            // Sphere51a: Players can move freely during casting (no UI freeze)
            if (Caster.Player)
            {
                return true; // Allow movement - no "frozen" message
            }

            // Original ModernUO logic for NPCs (unchanged)
            if (IsCasting && BlocksMovement)
            {
                Caster.SendLocalizedMessage(500111); // You are frozen and can not move.
                return false;
            }

            return true;
        }

        public virtual bool OnCasterEquipping(Item item)
        {
            // Sphere51a: Equipping does not interrupt for players
            if (Caster.Player)
            {
                return true; // Allow equipping
            }

            // Original ModernUO logic for NPCs (unchanged)
            if (IsCasting)
            {
                Disturb(DisturbType.EquipRequest);
            }

            return true;
        }

        public virtual bool OnCasterUsingObject(IEntity entity)
        {
            if (State == SpellState.Sequencing)
            {
                Disturb(DisturbType.UseRequest);
            }

            return true;
        }

        public virtual bool OnCastInTown(Region r) => Info.AllowTown;

        public virtual void FinishSequence()
        {
            State = SpellState.None;

            if (Caster.Spell == this)
            {
                Caster.Spell = null;
            }

            Caster.Delta(MobileDelta.Flags); // Remove paralyze

            // Sphere51a: Reset swing timer when spell completes (players only)
            if (Caster.Player && Caster.NextCombatTime == long.MaxValue)
            {
                // Start fresh swing timer with weapon delay
                var weapon = Caster.Weapon;
                if (weapon is Items.BaseWeapon baseWeapon)
                {
                    var delay = (int)baseWeapon.GetDelay(Caster).TotalMilliseconds;
                    Caster.NextCombatTime = Core.TickCount + delay;
                }
                else
                {
                    // Wrestling or no weapon - use default delay
                    Caster.NextCombatTime = Core.TickCount + 2000;
                }
            }
        }

        public void StartDelayedDamageContext(Mobile m, Timer t)
        {
            var damageStacking = DelayedDamageSpellFamilyStacking;
            if (damageStacking == null)
            {
                return; // Sanity
            }

            var type = GetType();

            if (!_contextTable.TryGetValue(type, out var context))
            {
                _contextTable[type] = context = new DelayedDamageContextWrapper();

                for (int i = 0; i < damageStacking.Length; i++)
                {
                    _contextTable.Add(damageStacking[i], context);
                }
            }

            context.Add(m, t);
        }

        public bool HasDelayedDamageContext(Mobile m) =>
            DelayedDamageSpellFamilyStacking != null &&
            _contextTable.TryGetValue(GetType(), out var context) && context.Contains(m);

        public void RemoveDelayedDamageContext(Mobile m)
        {
            if (m == null || DelayedDamageSpellFamilyStacking == null)
            {
                return; // Sanity
            }

            if (_contextTable.TryGetValue(GetType(), out var contexts))
            {
                contexts.Remove(m);
            }
        }

        public void HarmfulSpell(Mobile m)
        {
            (m as BaseCreature)?.OnHarmfulSpell(Caster);
        }

        public int GetNewAosDamage(int bonus, int dice, int sides, Mobile singleTarget) =>
            GetNewAosDamage(bonus, dice, sides, true, singleTarget);

        public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool sdi = true, Mobile singleTarget = null)
        {
            if (singleTarget != null)
            {
                return GetNewAosDamage(
                    bonus,
                    dice,
                    sides,
                    Caster.Player && singleTarget.Player,
                    sdi,
                    GetDamageScalar(singleTarget)
                );
            }

            return GetNewAosDamage(bonus, dice, sides, sdi, false);
        }

        public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, bool sdi, double scalar = 1.0)
        {
            var damage = Utility.Dice(dice, sides, bonus) * 100;

            var inscribeSkill = GetInscribeFixed(Caster);
            var inscribeBonus = (inscribeSkill + 1000 * (inscribeSkill / 1000)) / 200;
            var damageBonus = inscribeBonus;

            var intBonus = Caster.Int / 10;
            damageBonus += intBonus;

            if (sdi)
            {
                var sdiBonus = AosAttributes.GetValue(Caster, AosAttribute.SpellDamage);
                // PvP spell damage increase cap of 15% from an item's magic property
                if (playerVsPlayer && sdiBonus > 15)
                {
                    sdiBonus = 15;
                }

                damageBonus += sdiBonus;
            }

            var context = TransformationSpellHelper.GetContext(Caster);

            if (context?.Spell is ReaperFormSpell spell)
            {
                damageBonus += spell.SpellDamageBonus;
            }

            damage = AOS.Scale(damage, 100 + damageBonus);

            var evalSkill = GetDamageFixed(Caster);
            var evalScale = 30 + 9 * evalSkill / 100;

            damage = AOS.Scale(damage, evalScale);

            damage = AOS.Scale(damage, (int)(scalar * 100));

            return damage / 100;
        }

        public virtual bool ConsumeReagents() =>
            Scroll != null || !Caster.Player ||
            AosAttributes.GetValue(Caster, AosAttribute.LowerRegCost) > Utility.Random(100) ||
            DuelContext.IsFreeConsume(Caster) || DuelPitController.IsFreeConsume(Caster) || Caster.Backpack?.ConsumeTotal(Info.Reagents, Info.Amounts) == -1;

        public virtual double GetInscribeSkill(Mobile m) => m.Skills.Inscribe.Value;

        public virtual int GetInscribeFixed(Mobile m) => m.Skills.Inscribe.Fixed;

        public virtual int GetDamageFixed(Mobile m) => m.Skills[DamageSkill].Fixed;

        public virtual double GetDamageSkill(Mobile m) => m.Skills[DamageSkill].Value;

        public virtual double GetResistSkill(Mobile m) => m.Skills.MagicResist.Value;

        public virtual double GetDamageScalar(Mobile target)
        {
            var scalar = 1.0;

            if (!Core.AOS) // EvalInt stuff for AoS is handled elsewhere
            {
                var casterEI = Caster.Skills[DamageSkill].Value;
                var targetRS = target.Skills.MagicResist.Value;

                /*
                if (Core.AOS)
                  targetRS = 0;
                */

                // m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

                if (casterEI > targetRS)
                {
                    scalar = 1.0 + (casterEI - targetRS) / 500.0;
                }
                else
                {
                    scalar = 1.0 + (casterEI - targetRS) / 200.0;
                }

                // magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 120 skill
                scalar += (Caster.Skills[CastSkill].Value - 100.0) / 400.0;

                if (!target.Player && !target.Body.IsHuman /*&& !Core.AOS*/)
                {
                    scalar *= 2.0; // Double magery damage to monsters/animals if not AOS
                }
            }

            (target as BaseCreature)?.AlterDamageScalarFrom(Caster, ref scalar);

            (Caster as BaseCreature)?.AlterDamageScalarTo(target, ref scalar);

            if (Core.SE)
            {
                scalar *= GetSlayerDamageScalar(target);
            }

            target.Region.SpellDamageScalar(Caster, target, ref scalar);

            if (Evasion.CheckSpellEvasion(target)) // Only single target spells an be evaded
            {
                scalar = 0;
            }

            return scalar;
        }

        public virtual double GetSlayerDamageScalar(Mobile defender)
        {
            var atkBook = Spellbook.FindEquippedSpellbook(Caster);

            var scalar = 1.0;
            if (atkBook != null)
            {
                var atkSlayer = SlayerGroup.GetEntryByName(atkBook.Slayer);
                var atkSlayer2 = SlayerGroup.GetEntryByName(atkBook.Slayer2);

                if (atkSlayer?.Slays(defender) == true || atkSlayer2?.Slays(defender) == true)
                {
                    defender.FixedEffect(0x37B9, 10, 5); // TODO: Confirm this displays on OSIs
                    scalar = 2.0;
                }

                var context = TransformationSpellHelper.GetContext(defender);

                if ((atkBook.Slayer == SlayerName.Silver || atkBook.Slayer2 == SlayerName.Silver) && context != null &&
                    context.Type != typeof(HorrificBeastSpell))
                {
                    scalar += .25; // Every necromancer transformation other than horrific beast take an additional 25% damage
                }

                if (scalar != 1.0)
                {
                    return scalar;
                }
            }

            var defISlayer = Spellbook.FindEquippedSpellbook(defender) ?? defender.Weapon as ISlayer;

            if (defISlayer != null)
            {
                var defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
                var defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);

                if (defSlayer?.Group.OppositionSuperSlays(Caster) == true ||
                    defSlayer2?.Group.OppositionSuperSlays(Caster) == true)
                {
                    scalar = 2.0;
                }
            }

            return scalar;
        }

        public virtual void DoFizzle()
        {
            Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502632); // The spell fizzles.

            if (Caster.Player)
            {
                if (Core.AOS)
                {
                    Caster.FixedParticles(0x3735, 1, 30, 9503, EffectLayer.Waist);
                }
                else
                {
                    Caster.FixedEffect(0x3735, 6, 30);
                }

                Caster.PlaySound(0x5C);
            }
        }

        public virtual bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable) =>
            !(resistable && Scroll is BaseWand);

        public void Disturb(DisturbType type, bool firstCircle = true, bool resistable = false)
        {
            if (!CheckDisturb(type, firstCircle, resistable))
            {
                return;
            }

            if (State == SpellState.None || !firstCircle && !Core.AOS && (this as MagerySpell)?.Circle == SpellCircle.First)
            {
                return;
            }

            var wasCasting = IsCasting; // Copy SpellState before resetting it to none
            State = SpellState.None;
            Caster.Spell = null;

            OnDisturb(type, wasCasting);

            if (wasCasting)
            {
                _castTimer?.Stop();
                _animTimer?.Stop();

                // Sphere51a: Stop the delay timer and handle mana penalty
                if (_castDelayTimer != null)
                {
                    _castDelayTimer.Stop();
                    _castDelayTimer = null;

                    // Sphere51a: Apply 50% mana penalty for interruption (players only)
                    if (Caster.Player && ManaConsumed > 0)
                    {
                        var manaLost = ManaConsumed / 2;
                        Caster.Mana -= manaLost;

                        DoFizzle(); // Show fizzle effect
                    }
                }

                Caster.NextSpellTime = Core.TickCount + (int)GetDisturbRecovery().TotalMilliseconds;
            }
            else
            {
                Target.Cancel(Caster);
            }

            if (Core.AOS && Caster.Player && type == DisturbType.Hurt)
            {
                DoHurtFizzle();
            }

            Caster.Delta(MobileDelta.Flags); // Remove paralyze

            // Sphere51a: Reset swing timer when spell interrupted (players only)
            if (Caster.Player && Caster.NextCombatTime == long.MaxValue)
            {
                // Start fresh swing timer with weapon delay
                var weapon = Caster.Weapon;
                if (weapon is Items.BaseWeapon baseWeapon)
                {
                    var delay = (int)baseWeapon.GetDelay(Caster).TotalMilliseconds;
                    Caster.NextCombatTime = Core.TickCount + delay;
                }
                else
                {
                    // Wrestling or no weapon - use default delay
                    Caster.NextCombatTime = Core.TickCount + 2000;
                }
            }
        }

        public virtual void DoHurtFizzle()
        {
            Caster.FixedEffect(0x3735, 6, 30);
            Caster.PlaySound(0x5C);
        }

        public virtual void OnDisturb(DisturbType type, bool message)
        {
            if (message)
            {
                Caster.SendLocalizedMessage(500641); // Your concentration is disturbed, thus ruining thy spell.
            }
        }

        public virtual bool CheckCast() => true;

        public virtual void SayMantra()
        {
            if (Scroll is BaseWand)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Info.Mantra) && Caster.Player)
            {
                Caster.PublicOverheadMessage(MessageType.Spell, Caster.SpeechHue, true, Info.Mantra, false);
            }
        }

        public bool Cast()
        {
            StartCastTime = Core.TickCount;

            // Sphere51a: Interrupt bandaging when casting spell (players only)
            if (Caster.Player)
            {
                var bc = Items.BandageContext.GetContext(Caster);
                if (bc != null)
                {
                    bc.StopHeal();
                    Caster.SendLocalizedMessage(500969); // You stop applying bandages
                }
            }

            if (Core.AOS && Caster.Spell is Spell spell && spell.State == SpellState.Sequencing)
            {
                spell.Disturb(DisturbType.NewCast);
            }

            if (!Caster.CheckAlive())
            {
                return false;
            }

            var isCasting = Caster.Spell?.IsCasting == true;
            var isWand = Scroll is BaseWand;

            if (isCasting)
            {
                // Sphere51a: Players can queue a new spell while casting
                // The interruption happens when the new spell commits resources (in CheckSequenceAndStartTimer)
                if (Caster.Player && !isWand)
                {
                    // Allow player to continue - will interrupt old spell when new spell targets
                    // Note: Caster.Spell will be overwritten below, old spell stays in casting state
                }
                else if (isWand)
                {
                    Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
                }
                else
                {
                    // NPCs cannot interrupt themselves
                    Caster.SendLocalizedMessage(502642); // You are already casting a spell.
                }
            }

            if (!isCasting || Caster.Player && !isWand)
            {
                // Continue with spell validation (moved into conditional block)
                if (BlockedByHorrificBeast &&
                     TransformationSpellHelper.UnderTransformation(Caster, typeof(HorrificBeastSpell)) ||
                     BlockedByAnimalForm && AnimalForm.UnderTransformation(Caster))
            {
                Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
            }
            else if (!isWand && (Caster.Paralyzed || Caster.Frozen))
            {
                Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
            }
            else if (CheckNextSpellTime && Core.TickCount - Caster.NextSpellTime < 0)
            {
                Caster.SendLocalizedMessage(502644); // You have not yet recovered from casting a spell.
            }
            else if (Caster is PlayerMobile mobile && mobile.PeacedUntil > Core.Now)
            {
                mobile.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
            }
            else if ((Caster as PlayerMobile)?.DuelContext?.AllowSpellCast(Caster, this) == false)
            {
            }
            // DUEL PIT SYSTEM - Spell restriction check for active duel pit sessions
            // This check enforces duel pit rules by:
            // 1. Blocking offensive spells during the 10-second countdown period (only beneficial spells allowed)
            // 2. Blocking polymorph spells during countdown (prevents form-change exploits)
            // 3. Blocking all summon spells during countdown (prevents creature spam)
            // 4. Enforcing AllowSpellcasting rule during active duels
            // Uses null-coalescing pattern to only check if player is in an active duel pit session
            else if (!Server.Engines.ConPVP.DuelPitSpellRestriction.CheckDuelSpellRestriction(Caster, this))
            {
            }
            else
            {
                var requiredMana = ScaleMana(GetMana());

                if (Caster.Mana >= requiredMana)
                {
                    // Sphere51a: Allow players to queue a new spell (Caster.Spell will be overwritten)
                    // NPCs must have Caster.Spell == null
                    var canProceed = Caster.Player ? true : Caster.Spell == null;

                    if (canProceed && Caster.CheckSpellCast(this) && CheckCast() &&
                        Caster.Region.OnBeginSpellCast(Caster, this))
                    {
                        State = SpellState.Casting;

                        // Sphere51a: Save reference to old spell before overwriting (for interruption)
                        if (Caster.Player && Caster.Spell is Spell oldSpell && oldSpell.IsCasting)
                        {
                            InterruptedSpell = oldSpell;
                        }

                        Caster.Spell = this;

                        // Sphere51a: For players, defer visual effects until AFTER target selection
                        if (Caster.Player)
                        {
                            // Only reveal action immediately
                            if (!isWand && RevealOnCast)
                            {
                                Caster.RevealingAction();
                            }

                            // NOTE: Do NOT clear hands here - that happens in CheckSequenceAndStartTimer() after target selection
                            // NOTE: Do NOT clear weapon ability here - that happens in CheckSequenceAndStartTimer() after target selection

                            // Target cursor appears immediately - NO power words or animation yet
                            OnBeginCast();
                            OnCast();
                            return true;
                        }

                        // Original ModernUO flow for NPCs - visual effects happen immediately
                        if (!isWand && RevealOnCast)
                        {
                            Caster.RevealingAction();
                        }

                        SayMantra();

                        var castDelay = GetCastDelay();

                        if (ShowHandMovement && (Caster.Body.IsHuman || Caster.Player && Caster.Body.IsMonster))
                        {
                            var count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);

                            if (count != 0)
                            {
                                _animTimer = new AnimTimer(this, count);
                                _animTimer.Start();
                            }

                            if (Info.LeftHandEffect > 0)
                            {
                                Caster.FixedParticles(0, 10, 5, Info.LeftHandEffect, EffectLayer.LeftHand);
                            }

                            if (Info.RightHandEffect > 0)
                            {
                                Caster.FixedParticles(0, 10, 5, Info.RightHandEffect, EffectLayer.RightHand);
                            }
                        }

                        if (ClearHandsOnCast)
                        {
                            Caster.ClearHands();
                        }

                        if (Core.ML)
                        {
                            WeaponAbility.ClearCurrentAbility(Caster);
                        }

                        Caster.Delta(MobileDelta.Flags); // Start paralyze

                        // Original ModernUO: Timer-based OnCast for NPCs (unchanged)
                        _castTimer = new CastTimer(this, castDelay);
                        // m_CastTimer.Start();

                        OnBeginCast();

                        if (castDelay > TimeSpan.Zero)
                        {
                            _castTimer.Start();
                        }
                        else
                        {
                            _castTimer.Tick();
                        }

                        return true;
                    }
                }
                else if (Caster.NetState?.IsKRClient != true && Caster.NetState?.Version >= ClientVersion.Version70654)
                {
                    // Insufficient mana. You must have at least ~1_MANA_REQUIREMENT~ Mana to use this spell.
                    Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625, requiredMana.ToString());
                }
                else
                {
                    Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana
                }
            }
            } // End of spell validation conditional block

            return false;
        }

        public abstract void OnCast();

        public virtual void OnBeginCast()
        {
        }

        public virtual void GetCastSkills(out double min, out double max)
        {
            min = max = 0; // Intended but not required for overriding.
        }

        public virtual bool CheckFizzle()
        {
            if (Scroll is BaseWand)
            {
                return true;
            }

            GetCastSkills(out var minSkill, out var maxSkill);

            if (DamageSkill != CastSkill)
            {
                Caster.CheckSkill(DamageSkill, 0.0, Caster.Skills[DamageSkill].Cap);
            }

            return Caster.CheckSkill(CastSkill, minSkill, maxSkill);
        }

        public abstract int GetMana();

        public virtual int ScaleMana(int mana)
        {
            var scalar = 1.0;

            if (!MindRotSpell.GetMindRotScalar(Caster, ref scalar))
            {
                scalar = 1.0;
            }

            // Lower Mana Cost = 40%
            var lmc = AosAttributes.GetValue(Caster, AosAttribute.LowerManaCost);
            if (lmc > 40)
            {
                lmc = 40;
            }

            scalar -= (double)lmc / 100;

            mana = (int)(mana * scalar);

            // Sphere51a: Scroll mana reduction (43% cheaper) - PLAYERS ONLY
            if (Caster.Player && Scroll is SpellScroll)
            {
                mana = (int)(mana / 1.755); // 43% reduction (divide by 1.755)
            }

            return mana;
        }

        public virtual TimeSpan GetDisturbRecovery()
        {
            if (Core.AOS)
            {
                return TimeSpan.Zero;
            }

            var delay = Math.Max(
                1.0 - Math.Sqrt((Core.TickCount - StartCastTime) / 1000.0 / GetCastDelay().TotalSeconds),
                0.2
            );

            return TimeSpan.FromSeconds(delay);
        }

        public virtual TimeSpan GetCastRecovery()
        {
            if (!Core.AOS)
            {
                return NextSpellDelay;
            }

            var fcr = AosAttributes.GetValue(Caster, AosAttribute.CastRecovery) -
                      ThunderstormSpell.GetCastRecoveryMalus(Caster);

            var fcrDelay = -(CastRecoveryFastScalar * fcr);

            var delay = CastRecoveryBase + fcrDelay;

            if (delay < CastRecoveryMinimum)
            {
                delay = CastRecoveryMinimum;
            }

            return TimeSpan.FromSeconds((double)delay / CastRecoveryPerSecond);
        }

        public virtual TimeSpan GetCastDelay()
        {
            if (Scroll is BaseWand)
            {
                return Core.ML ? CastDelayBase : TimeSpan.Zero; // TODO: Should FC apply to wands?
            }

            // Sphere51a: Remove FC entirely for players
            if (Caster.Player)
            {
                return CastDelayBase; // No FC modification for players
            }

            // Original ModernUO FC logic for NPCs (unchanged)
            // Faster casting cap of 2 (if not using the protection spell)
            // Faster casting cap of 0 (if using the protection spell)
            // Paladin spells are subject to a faster casting cap of 4
            // Paladins with magery of 70.0 or above are subject to a faster casting cap of 2
            var fcMax = 4;

            if (CastSkill is SkillName.Magery or SkillName.Necromancy ||
                CastSkill == SkillName.Chivalry && Caster.Skills.Magery.Value >= 70.0)
            {
                fcMax = 2;
            }

            var fc = Math.Min(AosAttributes.GetValue(Caster, AosAttribute.CastSpeed), fcMax);

            if (ProtectionSpell.Registry.ContainsKey(Caster))
            {
                fc -= 2;
            }

            if (EssenceOfWindSpell.IsDebuffed(Caster))
            {
                fc -= EssenceOfWindSpell.GetFCMalus(Caster);
            }

            if (Core.SA)
            {
                // At some point OSI added 0.25s to every spell. This makes the minimum 0.5s
                // Note: This is done after multiplying for summon creature & blade spirits.
                fc--;
            }

            var fcDelay = TimeSpan.FromSeconds(-(CastDelayFastScalar * fc * CastDelaySecondsPerTick));

            return Utility.Max(CastDelayBase + fcDelay, CastDelayMinimum);
        }

        public virtual int ComputeKarmaAward() => 0;

        // Sphere51a: Player-only spell validation and delay timer start
        // Called after target selection - consumes mana upfront, then starts delay timer
        public virtual bool CheckSequenceAndStartTimer()
        {
            // Sphere51a: Check if there's a previous spell that needs to be interrupted
            // This happens when player casts a new spell while another is in progress
            // The old spell gets interrupted HERE (when new spell commits resources)
            if (Caster.Player && InterruptedSpell != null && InterruptedSpell.IsCasting)
            {
                InterruptedSpell.Disturb(DisturbType.NewCast);

                // CRITICAL: Disturb() sets Caster.Spell = null, so restore it to this spell
                Caster.Spell = this;

                InterruptedSpell = null; // Clear reference after interruption
            }

            var mana = ScaleMana(GetMana());

            // Early failure checks - no mana consumption
            if (Caster.Deleted || !Caster.Alive || Caster.Spell != this)
            {
                DoFizzle();
                return false;
            }

            // Sphere51a: NOW show power words and casting animation (after target selected)
            SayMantra();

            // Sphere51a: Disarm weapons when casting spell STARTS (players only)
            if (Caster.Player)
            {
                var oneHanded = Caster.FindItemOnLayer(Layer.OneHanded);
                var twoHanded = Caster.FindItemOnLayer(Layer.TwoHanded);

                if (oneHanded != null && Caster.Backpack != null)
                {
                    Caster.Backpack.DropItem(oneHanded);
                }

                if (twoHanded != null && Caster.Backpack != null)
                {
                    Caster.Backpack.DropItem(twoHanded);
                }

                if (oneHanded != null || twoHanded != null)
                {
                    Caster.SendLocalizedMessage(1062001); // You put your weapon away
                }

                // Freeze weapon swing timer during casting
                Caster.NextCombatTime = long.MaxValue;
            }

            var castDelay = GetCastDelay();

            if (ShowHandMovement && (Caster.Body.IsHuman || Caster.Player && Caster.Body.IsMonster))
            {
                var count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);

                if (count != 0)
                {
                    _animTimer = new AnimTimer(this, count);
                    _animTimer.Start();
                }

                if (Info.LeftHandEffect > 0)
                {
                    Caster.FixedParticles(0, 10, 5, Info.LeftHandEffect, EffectLayer.LeftHand);
                }

                if (Info.RightHandEffect > 0)
                {
                    Caster.FixedParticles(0, 10, 5, Info.RightHandEffect, EffectLayer.RightHand);
                }
            }

            Caster.Delta(MobileDelta.Flags); // Update paralyze flag

            // Scroll validation - no mana consumption
            if (Scroll != null && Scroll is not Runebook &&
                (Scroll.Amount <= 0 || Scroll.Deleted || Scroll.RootParent != Caster || Scroll is BaseWand baseWand &&
                    (baseWand.Charges <= 0 || baseWand.Parent != Caster)))
            {
                DoFizzle();
                return false;
            }

            // Reagent consumption - ALWAYS first
            if (!ConsumeReagents())
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.
                return false;
            }

            // Mana validation
            if (Caster.Mana < mana)
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana for this spell.
                return false;
            }

            // Frozen/Paralyzed check
            if (Core.AOS && (Caster.Frozen || Caster.Paralyzed))
            {
                Caster.SendLocalizedMessage(502646); // You cannot cast a spell while frozen.
                DoFizzle();
                return false;
            }

            // Peaced check
            if (Caster is PlayerMobile mobile && mobile.PeacedUntil > Core.Now)
            {
                mobile.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
                DoFizzle();
                return false;
            }

            // === Sphere51a: Store mana cost, but DON'T consume yet ===
            // Mana will be consumed AFTER the fizzle check in CastDelayTimer.OnTick()
            ManaConsumed = mana;

            // Start cast delay timer
            var delay = GetCastDelay();
            _castDelayTimer = new CastDelayTimer(this, delay);
            _castDelayTimer.Start();

            return true; // Validation passed, timer started, mana consumed
        }

        public virtual bool CheckSequence()
        {
            var mana = ScaleMana(GetMana());

            if (Caster.Deleted || !Caster.Alive || Caster.Spell != this || State != SpellState.Sequencing)
            {
                DoFizzle();
            }
            else if (Scroll != null && Scroll is not Runebook &&
                     (Scroll.Amount <= 0 || Scroll.Deleted || Scroll.RootParent != Caster || Scroll is BaseWand baseWand &&
                         (baseWand.Charges <= 0 || baseWand.Parent != Caster)))
            {
                DoFizzle();
            }
            else if (!ConsumeReagents())
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.
            }
            else if (Caster.Mana < mana)
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana for this spell.
            }
            else if (Core.AOS && (Caster.Frozen || Caster.Paralyzed))
            {
                Caster.SendLocalizedMessage(502646); // You cannot cast a spell while frozen.
                DoFizzle();
            }
            else if (Caster is PlayerMobile mobile && mobile.PeacedUntil > Core.Now)
            {
                mobile.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
                DoFizzle();
            }
            else if (CheckFizzle())
            {
                // Spell succeeds - full mana deduction
                Caster.Mana -= mana;

                if (Scroll is SpellScroll)
                {
                    Scroll.Consume();
                }
                else if (Scroll is BaseWand wand)
                {
                    wand.ConsumeCharge(Caster);
                    Caster.RevealingAction();
                }

                if (Scroll is BaseWand)
                {
                    var m = Scroll.Movable;

                    Scroll.Movable = false;

                    if (ClearHandsOnCast)
                    {
                        Caster.ClearHands();
                    }

                    Scroll.Movable = m;
                }
                else if (ClearHandsOnCast)
                {
                    Caster.ClearHands();
                }

                var karma = ComputeKarmaAward();

                if (karma != 0)
                {
                    Titles.AwardKarma(Caster, karma, true);
                }

                if (TransformationSpellHelper.UnderTransformation(Caster, typeof(VampiricEmbraceSpell)))
                {
                    var garlic = false;

                    for (var i = 0; !garlic && i < Info.Reagents.Length; ++i)
                    {
                        garlic = Info.Reagents[i] == Reagent.Garlic;
                    }

                    if (garlic)
                    {
                        Caster.SendLocalizedMessage(1061651); // The garlic burns you!
                        AOS.Damage(Caster, Utility.RandomMinMax(17, 23), 100, 0, 0, 0, 0);
                    }
                }

                return true;
            }
            else
            {
                // Sphere51a: Spell fizzles - 50% mana consumed for players, 100% for NPCs
                var fizzleMana = Caster.Player ? (mana / 2) : mana;
                Caster.Mana -= fizzleMana;
                DoFizzle();
            }

            return false;
        }

        // Sphere51a: CheckBSequence with callback for delayed execution
        public bool CheckBSequence(Mobile target, Action onSuccess, bool allowDead = false)
        {
            if (!target.Alive && !allowDead)
            {
                Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            if ((Caster as PlayerMobile)?.Young == true && (target as PlayerMobile)?.Young == false)
            {
                Caster.SendLocalizedMessage(500278); // As a young player, you may not cast beneficial spells onto older players.
                return false;
            }

            if (!Caster.CanBeBeneficial(target, true, allowDead))
            {
                return false;
            }

            // Sphere51a: Player path uses delayed validation with timer
            if (Caster.Player)
            {
                // Store target and callback for delayed execution
                TargetMobile = target;
                SpellEffect = onSuccess;

                // Start validation and timer - mana consumed here
                if (CheckSequenceAndStartTimer())
                {
                    Caster.DoBeneficial(target);
                    // Return TRUE - but execution is deferred to timer
                    return true;
                }
                return false;
            }

            // NPC path: Original immediate execution
            if (CheckSequence())
            {
                Caster.DoBeneficial(target);
                onSuccess?.Invoke(); // Execute immediately for NPCs
                return true;
            }

            return false;
        }

        // Original CheckBSequence (for backward compatibility with non-callback spells)
        public bool CheckBSequence(Mobile target, bool allowDead = false)
        {
            if (!target.Alive && !allowDead)
            {
                Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            if ((Caster as PlayerMobile)?.Young == true && (target as PlayerMobile)?.Young == false)
            {
                Caster.SendLocalizedMessage(500278); // As a young player, you may not cast beneficial spells onto older players.
                return false;
            }

            if (!Caster.CanBeBeneficial(target, true, allowDead))
            {
                return false;
            }

            // Sphere51a: Player path uses delayed validation
            if (Caster.Player)
            {
                // For players: Use CheckSequenceAndStartTimer which doesn't require Sequencing state
                if (CheckSequenceAndStartTimer())
                {
                    Caster.DoBeneficial(target);
                    return true; // Timer started - spell effect happens after delay
                }
                return false;
            }

            // NPC path: Original immediate execution
            if (CheckSequence())
            {
                Caster.DoBeneficial(target);
                return true;
            }

            return false;
        }

        // Sphere51a: CheckHSequence with callback for delayed execution
        public bool CheckHSequence(Mobile target, Action onSuccess)
        {
            if (!target.Alive)
            {
                Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            if (!Caster.CanBeHarmful(target))
            {
                return false;
            }

            // Sphere51a: Player path uses delayed validation with timer
            if (Caster.Player)
            {
                // Store target and callback for delayed execution
                TargetMobile = target;
                SpellEffect = onSuccess;

                // Start validation and timer - mana consumed here
                if (CheckSequenceAndStartTimer())
                {
                    Caster.DoHarmful(target);
                    // Return TRUE - but execution is deferred to timer
                    return true;
                }
                return false;
            }

            // NPC path: Original immediate execution
            if (CheckSequence())
            {
                Caster.DoHarmful(target);
                onSuccess?.Invoke(); // Execute immediately for NPCs
                return true;
            }

            return false;
        }

        // Original CheckHSequence (for backward compatibility with non-callback spells)
        // WARNING: For players, this returns FALSE to prevent immediate execution!
        // The spell effect code after this call will NOT run for players.
        // Instead, it will execute after the delay timer via the stored callback.
        public bool CheckHSequence(Mobile target)
        {
            if (!target.Alive)
            {
                Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            if (!Caster.CanBeHarmful(target))
            {
                return false;
            }

            // Sphere51a: Player path - validate and start timer, but return FALSE
            // This prevents immediate spell execution for players
            if (Caster.Player)
            {
                // Store target for delayed execution
                TargetMobile = target;

                // Validate and start timer
                if (CheckSequenceAndStartTimer())
                {
                    Caster.DoHarmful(target);
                    // Return FALSE so spell code doesn't execute yet
                    // Spell will execute from callback stored by SpellHelper or other means
                    return false;
                }
                return false;
            }

            // NPC path: Original immediate execution
            if (CheckSequence())
            {
                Caster.DoHarmful(target);
                return true; // Execute spell code immediately for NPCs
            }

            return false;
        }

        private class DelayedDamageContextWrapper
        {
            private readonly Dictionary<Mobile, Timer> m_Contexts = new();

            public void Add(Mobile m, Timer t)
            {
                if (m_Contexts.Remove(m, out var oldTimer))
                {
                    oldTimer.Stop();
                }

                m_Contexts.Add(m, t);
            }

            public bool Contains(Mobile m) => m_Contexts.ContainsKey(m);

            public void Remove(Mobile m)
            {
                if (m_Contexts.Remove(m, out var t))
                {
                    t.Stop();
                }
            }
        }

        private class AnimTimer : Timer
        {
            private readonly Spell m_Spell;

            public AnimTimer(Spell spell, int count) : base(TimeSpan.Zero, AnimateDelay, count)
            {
                m_Spell = spell;
            }

            protected override void OnTick()
            {
                var caster = m_Spell.Caster;

                if (m_Spell.State != SpellState.Casting || caster.Spell != m_Spell)
                {
                    Stop();
                    return;
                }

                if (!caster.Mounted && m_Spell.Info.Action >= 0)
                {
                    if (caster.Body.IsHuman)
                    {
                        caster.Animate(m_Spell.Info.Action, 7, 1, true, false, 0);
                    }
                    else if (caster.Player && caster.Body.IsMonster)
                    {
                        caster.Animate(12, 7, 1, true, false, 0);
                    }
                }

                if (!Running)
                {
                    m_Spell._animTimer = null;
                }
            }
        }

        private class CastTimer : Timer
        {
            private readonly Spell m_Spell;

            public CastTimer(Spell spell, TimeSpan castDelay) : base(castDelay)
            {
                m_Spell = spell;
            }

            protected override void OnTick()
            {
                var caster = m_Spell?.Caster;

                if (caster == null)
                {
                    return;
                }

                if (m_Spell.State == SpellState.Casting && caster.Spell == m_Spell)
                {
                    m_Spell.State = SpellState.Sequencing;
                    m_Spell._castTimer = null;
                    caster.OnSpellCast(m_Spell);
                    caster.Region?.OnSpellCast(caster, m_Spell);
                    caster.NextSpellTime =
                        Core.TickCount + (int)m_Spell.GetCastRecovery().TotalMilliseconds; // Spell.NextSpellDelay;

                    caster.Delta(MobileDelta.Flags); // Update paralyze

                    var originalTarget = caster.Target;

                    m_Spell.OnCast();

                    if (caster.Player && caster.Target != originalTarget)
                    {
                        caster.Target?.BeginTimeout(caster, 30000); // 30 seconds
                    }

                    m_Spell._castTimer = null;
                }
            }

            public void Tick()
            {
                OnTick();
            }
        }

        // Sphere51a: CastDelayTimer for player spell casting (target-first flow)
        // Handles fizzle check AFTER the delay, with 50% mana refund on fizzle
        private class CastDelayTimer : Timer
        {
            private readonly Spell _spell;

            public CastDelayTimer(Spell spell, TimeSpan delay) : base(delay)
            {
                _spell = spell;
            }

            protected override void OnTick()
            {
                if (_spell == null || _spell.Caster == null)
                {
                    return;
                }

                // Check if spell was interrupted during delay
                // NOTE: We do NOT check Caster.Spell != _spell because that gets overwritten when player selects a new spell
                // The old spell should continue casting until completion, then be interrupted when new spell commits resources
                if (_spell.State != SpellState.Casting)
                {
                    _spell.FinishSequence();
                    return;
                }

                // Transition to sequencing
                _spell.State = SpellState.Sequencing;

                // Perform fizzle check NOW (after delay)
                if (_spell.CheckFizzle())
                {
                    // SUCCESS - consume 100% mana
                    _spell.Caster.Mana -= _spell.ManaConsumed;

                    // SUCCESS - consume scroll and execute
                    if (_spell.Scroll is SpellScroll)
                    {
                        _spell.Scroll.Consume();
                    }
                    else if (_spell.Scroll is BaseWand wand)
                    {
                        wand.ConsumeCharge(_spell.Caster);
                        _spell.Caster.RevealingAction();
                    }

                    if (_spell.Scroll is BaseWand)
                    {
                        var m = _spell.Scroll.Movable;
                        _spell.Scroll.Movable = false;

                        // Sphere51a: Skip ClearHands for players - already disarmed in CheckSequenceAndStartTimer
                        // This allows players to re-equip weapons during casting
                        if (_spell.ClearHandsOnCast && !_spell.Caster.Player)
                        {
                            _spell.Caster.ClearHands();
                        }

                        _spell.Scroll.Movable = m;
                    }
                    else if (_spell.ClearHandsOnCast && !_spell.Caster.Player)
                    {
                        // Sphere51a: Skip ClearHands for players - already disarmed in CheckSequenceAndStartTimer
                        // This allows players to re-equip weapons during casting
                        _spell.Caster.ClearHands();
                    }

                    // Award karma
                    var karma = _spell.ComputeKarmaAward();
                    if (karma != 0)
                    {
                        Titles.AwardKarma(_spell.Caster, karma, true);
                    }

                    // Garlic check for vampires
                    if (TransformationSpellHelper.UnderTransformation(_spell.Caster, typeof(VampiricEmbraceSpell)))
                    {
                        var garlic = false;
                        for (var i = 0; !garlic && i < _spell.Info.Reagents.Length; ++i)
                        {
                            garlic = _spell.Info.Reagents[i] == Reagent.Garlic;
                        }

                        if (garlic)
                        {
                            _spell.Caster.SendLocalizedMessage(1061651); // The garlic burns you!
                            AOS.Damage(_spell.Caster, Utility.RandomMinMax(17, 23), 100, 0, 0, 0, 0);
                        }
                    }

                    // Spell succeeded - execute stored spell effect
                    if (_spell.SpellEffect != null)
                    {
                        _spell.SpellEffect.Invoke();
                        _spell.SpellEffect = null;
                    }
                }
                else
                {
                    // FIZZLE - consume only 50% mana
                    var manaLost = _spell.ManaConsumed / 2;
                    _spell.Caster.Mana -= manaLost;
                    _spell.DoFizzle();
                }

                _spell.FinishSequence();
                _spell._castDelayTimer = null;
                _spell.TargetMobile = null;
            }
        }
    }
}
