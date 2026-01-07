using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    public class DuelPitSession
    {
        private readonly Mobile _challenger;
        private readonly Mobile _challenged;
        private readonly DuelPitRules _rules;
        private readonly DuelPitController _controller;
        private readonly Point3D _originalChallengerLocation;
        private readonly Point3D _originalChallengedLocation;
        private readonly Map _originalMap;

        // Equipment managers for saving/restoring player gear
        private readonly DuelPitEquipmentManager _challengerEquipment;
        private readonly DuelPitEquipmentManager _challengedEquipment;

        private bool _accepted;
        private bool _started;
        private bool _countdownActive;
        private int _countdown;

        public Mobile Challenger => _challenger;
        public Mobile Challenged => _challenged;
        public DuelPitRules Rules => _rules;

        public DuelPitSession(Mobile challenger, Mobile challenged, DuelPitRules rules, DuelPitController controller)
        {
            _challenger = challenger ?? throw new ArgumentNullException(nameof(challenger));
            _challenged = challenged ?? throw new ArgumentNullException(nameof(challenged));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            // Store original locations
            _originalChallengerLocation = challenger.Location;
            _originalChallengedLocation = challenged.Location;
            _originalMap = challenger.Map;

            // Create equipment managers (challenger gets red surcoat, challenged gets blue)
            _challengerEquipment = new DuelPitEquipmentManager(challenger, true);
            _challengedEquipment = new DuelPitEquipmentManager(challenged, false);

            // Register this session for spell restriction tracking
            DuelPitSpellRestriction.RegisterSession(this);

            // Send initial messages
            challenger.SendMessage($"Duel challenge sent to {challenged.Name}. Waiting for response...");
            challenged.SendMessage($"{challenger.Name} has challenged you to a duel!");

            // Start challenge timeout (30 seconds)
            Timer.StartTimer(TimeSpan.FromSeconds(30.0), () =>
            {
                if (!_accepted)
                {
                    challenger.SendMessage($"{challenged.Name} did not respond to your duel challenge.");
                    challenged.SendMessage("Duel challenge timed out.");
                    Cancel();
                }
            });
        }

        public void AcceptChallenge(Mobile from)
        {
            if (from != _challenged)
                return;

            _accepted = true;

            // Notify both players
            _challenger.SendMessage($"{_challenged.Name} has accepted your duel challenge!");
            _challenged.SendMessage("You have accepted the duel challenge!");

            // Save and clear equipment, then equip arena gear
            PreparePlayersForArena();

            // Create recovery markers for server crash protection
            CreateRecoveryMarkers();

            // Teleport both players to the duel pit
            TeleportToDuelPit();

            // Start countdown
            StartCountdown();
        }

        public void DeclineChallenge(Mobile from)
        {
            if (from != _challenged)
                return;

            _challenger.SendMessage($"{_challenged.Name} has declined your duel challenge.");
            _challenged.SendMessage("You have declined the duel challenge.");
            Cancel();
        }

        private void PreparePlayersForArena()
        {
            // Save current equipment and clear it
            _challengerEquipment.SaveAndClearEquipment();
            _challengedEquipment.SaveAndClearEquipment();

            // Equip arena gear
            _challengerEquipment.EquipArenaGear();
            _challengedEquipment.EquipArenaGear();
        }

        private void TeleportToDuelPit()
        {
            // Get the addon's base location (arena center)
            Point3D arenaCenter;

            if (_controller.Addon != null)
            {
                arenaCenter = _controller.Addon.GetWorldLocation();
            }
            else
            {
                arenaCenter = _controller.GetWorldLocation();
            }

            // Place players in opposite corners of the 9x9 interior floor
            Point3D challengerPos = new Point3D(arenaCenter.X - 4, arenaCenter.Y - 4, arenaCenter.Z);
            Point3D challengedPos = new Point3D(arenaCenter.X + 4, arenaCenter.Y + 4, arenaCenter.Z);

            // Teleport players
            _challenger.MoveToWorld(challengerPos, _controller.Map);
            _challenged.MoveToWorld(challengedPos, _controller.Map);

            // Face each other
            _challenger.Direction = _challenger.GetDirectionTo(_challenged.Location);
            _challenged.Direction = _challenged.GetDirectionTo(_challenger.Location);

            // Send messages
            _challenger.SendMessage("You have been teleported to the duel pit!");
            _challenged.SendMessage("You have been teleported to the duel pit!");
        }

        private void StartCountdown()
        {
            _countdownActive = true;
            _countdown = 10;

            // Clear all buffs/debuffs before countdown
            ClearAllBuffs(_challenger);
            ClearAllBuffs(_challenged);

            // Restore both players to full stats before countdown
            RestoreFullStats(_challenger);
            RestoreFullStats(_challenged);

            // DO NOT freeze players - freezing blocks spell casting
            // Movement is blocked via CanMove() override instead
            // FreezePlayer(_challenger, true);
            // FreezePlayer(_challenged, true);

            // Send initial countdown message
            _challenger.SendMessage("Duel starts in 10 seconds! Prepare yourself...");
            _challenged.SendMessage("Duel starts in 10 seconds! Prepare yourself...");

            // Start countdown timer
            Timer.StartTimer(TimeSpan.FromSeconds(1.0), 10, () => CountdownTick());
        }

        private void RestoreFullStats(Mobile mobile)
        {
            if (mobile != null && !mobile.Deleted)
            {
                mobile.Hits = mobile.HitsMax;
                mobile.Stam = mobile.StamMax;
                mobile.Mana = mobile.ManaMax;
            }
        }

        private void ClearAllBuffs(Mobile mobile)
        {
            if (mobile == null || mobile.Deleted)
                return;

            // Remove all stat modifications (buffs/debuffs)
            mobile.RemoveStatMod("[Magic] Str Offset");
            mobile.RemoveStatMod("[Magic] Dex Offset");
            mobile.RemoveStatMod("[Magic] Int Offset");

            // Remove spell-specific stat mods
            mobile.RemoveStatMod("Strength");
            mobile.RemoveStatMod("Agility");
            mobile.RemoveStatMod("Cunning");
            mobile.RemoveStatMod("Bless");
            mobile.RemoveStatMod("Curse");
            mobile.RemoveStatMod("Weaken");
            mobile.RemoveStatMod("Clumsy");
            mobile.RemoveStatMod("Feeblemind");

            // Clear poison
            if (mobile.Poisoned)
                mobile.CurePoison(mobile);

            // Clear paralyze
            mobile.Paralyzed = false;
        }

        private void CountdownTick()
        {
            _countdown--;

            if (_countdown > 0)
            {
                // Send countdown messages
                string message = $"Duel starts in {_countdown} second{(_countdown == 1 ? "" : "s")}...";
                _challenger.SendMessage(message);
                _challenged.SendMessage(message);

                // Play sound effect
                _challenger.PlaySound(0x1E1);
                _challenged.PlaySound(0x1E1);
            }
            else
            {
                // Countdown finished - start the duel!
                StartDuel();
            }
        }

        private void StartDuel()
        {
            // Safety checks
            if (_challenger == null || _challenger.Deleted || _challenged == null || _challenged.Deleted)
            {
                Cancel();
                return;
            }

            _countdownActive = false;
            _started = true;

            // Unfreeze players
            FreezePlayer(_challenger, false);
            FreezePlayer(_challenged, false);

            // Clear any aggression flags
            RemoveAggressions();

            // Display "Fight!" overhead on both players
            _challenger.PublicOverheadMessage(MessageType.Regular, 0x35, false, "Fight!");
            _challenged.PublicOverheadMessage(MessageType.Regular, 0x35, false, "Fight!");

            // Send start messages
            _challenger.SendMessage("DUEL BEGINS! Fight!");
            _challenged.SendMessage("DUEL BEGINS! Fight!");

            // Play start sound
            _challenger.PlaySound(0x151);
            _challenged.PlaySound(0x151);

            // Start duel timer
            if (_rules != null)
            {
                Timer.StartTimer(TimeSpan.FromMinutes(_rules.TimeLimitMinutes), () => DuelTimeout());
            }
        }

        private void FreezePlayer(Mobile mobile, bool freeze)
        {
            if (mobile != null && !mobile.Deleted)
            {
                mobile.Frozen = freeze;
            }
        }

        public bool CanMove(Mobile mobile)
        {
            // Players cannot move during countdown
            if (_countdownActive && (mobile == _challenger || mobile == _challenged))
            {
                return false;
            }

            return true;
        }

        private void RemoveAggressions()
        {
            // Safety checks
            if (_challenger == null || _challenger.Deleted || _challenged == null || _challenged.Deleted)
                return;

            // Clear combatants to stop auto-attack
            if (_challenger.Combatant == _challenged)
                _challenger.Combatant = null;
            if (_challenged.Combatant == _challenger)
                _challenged.Combatant = null;

            // Clear all aggressor/aggressed entries between the two duelists
            _challenger.RemoveAggressed(_challenged);
            _challenger.RemoveAggressor(_challenged);
            _challenged.RemoveAggressed(_challenger);
            _challenged.RemoveAggressor(_challenger);

            // Force notoriety recalculation on clients
            _challenger.Delta(MobileDelta.Noto);
            _challenged.Delta(MobileDelta.Noto);
        }

        private void DuelTimeout()
        {
            if (!_started) return;

            // Clear aggression flags before ending
            RemoveAggressions();

            // Send timeout messages
            _challenger.SendMessage("Duel ended in a draw due to time limit!");
            _challenged.SendMessage("Duel ended in a draw due to time limit!");

            // Restore and return
            RestoreAndReturnPlayers();

            // Clean up
            Cancel();
        }

        public void OnPlayerDeath(Mobile victim, Mobile killer)
        {
            if (!_started) return;

            // Determine winner and loser
            Mobile winner = killer;
            Mobile loser = victim;

            // Handle case where killer is not one of the duelists
            if (winner != _challenger && winner != _challenged)
            {
                winner = (victim == _challenger) ? _challenged : _challenger;
            }

            // Delete corpse (arena items are blessed so they won't be in the corpse)
            if (loser.Corpse != null && !loser.Corpse.Deleted)
            {
                Console.WriteLine($"[DuelPit] Deleting corpse for {loser.Name}");
                loser.Corpse.Delete();
            }

            // Resurrect the loser immediately
            if (!loser.Alive)
            {
                loser.Resurrect();
            }

            // Remove death robe if it exists
            loser.FindItemOnLayer<DeathRobe>(Layer.OuterTorso)?.Delete();

            // Send victory/defeat messages
            winner.SendMessage("VICTORY! You have won the duel!");
            loser.SendMessage("DEFEAT! You have lost the duel!");

            // Play victory sound
            winner.PlaySound(0x5F1);
            loser.PlaySound(0x5F2);

            // Heal both players to full health
            winner.Hits = winner.HitsMax;
            winner.Stam = winner.StamMax;
            winner.Mana = winner.ManaMax;

            loser.Hits = loser.HitsMax;
            loser.Stam = loser.StamMax;
            loser.Mana = loser.ManaMax;

            // Add 2 second delay before teleporting out to prevent guard whack
            // This gives time for combat state to fully clear
            Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
            {
                // CRITICAL: Clear buffs/debuffs RIGHT BEFORE restoration to ensure STR is normal
                // This prevents PlateChest/Halberd from failing to equip due to low STR from Curse
                // Must happen HERE in the callback, not 2 seconds earlier
                ClearAllBuffs(winner);
                ClearAllBuffs(loser);

                // Cancel any active spells to prevent spells completing after restoration
                if (winner.Spell is Spell winnerSpell && winnerSpell.IsCasting)
                    winnerSpell.Disturb(DisturbType.Hurt, false, true);
                if (loser.Spell is Spell loserSpell && loserSpell.IsCasting)
                    loserSpell.Disturb(DisturbType.Hurt, false, true);

                // Clear aggression flags before ending
                RemoveAggressions();

                // Restore and return
                RestoreAndReturnPlayers();

                // Clean up session
                EndDuel();
            });
        }

        private void RestoreAndReturnPlayers()
        {
            // Remove recovery markers first (duel ending normally)
            RemoveRecoveryMarkers();

            // Remove arena gear
            _challengerEquipment.RemoveArenaGear();
            _challengedEquipment.RemoveArenaGear();

            // Restore original equipment
            _challengerEquipment.RestoreOriginalEquipment();
            _challengedEquipment.RestoreOriginalEquipment();

            // Clear combatants one final time before returning
            if (_challenger != null && !_challenger.Deleted && _challenger.Combatant == _challenged)
                _challenger.Combatant = null;
            if (_challenged != null && !_challenged.Deleted && _challenged.Combatant == _challenger)
                _challenged.Combatant = null;

            // Return players to their original locations
            _challenger.MoveToWorld(_originalChallengerLocation, _originalMap);
            _challenged.MoveToWorld(_originalChallengedLocation, _originalMap);

            // Reset directions
            _challenger.Direction = Direction.North;
            _challenged.Direction = Direction.North;

            // Ensure players are unfrozen
            FreezePlayer(_challenger, false);
            FreezePlayer(_challenged, false);
        }

        public void Cancel()
        {
            // Unregister from spell restriction system
            DuelPitSpellRestriction.UnregisterSession(this);

            // Clean up session
            _controller.CancelDuelSession(_challenger);
            _controller.CancelDuelSession(_challenged);
        }

        private void EndDuel()
        {
            // Mark duel as ended to prevent further processing
            _started = false;
            _countdownActive = false;

            // Unregister from spell restriction system
            DuelPitSpellRestriction.UnregisterSession(this);

            // Clean up session from controller
            _controller.EndDuelSession(_challenger);
            _controller.EndDuelSession(_challenged);
        }

        private void CreateRecoveryMarkers()
        {
            // Create marker for challenger
            if (_challenger?.Backpack != null)
            {
                var challengerMarker = new DuelPitRecoveryMarker(
                    _originalChallengerLocation,
                    _originalMap,
                    _challenged,
                    true,
                    _challengerEquipment.GetSavedEquipment(),
                    _challengerEquipment.GetSavedBackpackItems()
                );
                _challenger.Backpack.DropItem(challengerMarker);
            }

            // Create marker for challenged
            if (_challenged?.Backpack != null)
            {
                var challengedMarker = new DuelPitRecoveryMarker(
                    _originalChallengedLocation,
                    _originalMap,
                    _challenger,
                    false,
                    _challengedEquipment.GetSavedEquipment(),
                    _challengedEquipment.GetSavedBackpackItems()
                );
                _challenged.Backpack.DropItem(challengedMarker);
            }
        }

        private void RemoveRecoveryMarkers()
        {
            // Remove challenger marker
            _challenger?.Backpack?.FindItemByType<DuelPitRecoveryMarker>()?.Delete();

            // Remove challenged marker
            _challenged?.Backpack?.FindItemByType<DuelPitRecoveryMarker>()?.Delete();
        }

        public bool IsSpellAllowed(Mobile caster, Spell spell)
        {
            // Debug logging
            Console.WriteLine($"[DuelPit] IsSpellAllowed called - Caster: {caster.Name}, Spell: {spell.GetType().Name}, Countdown: {_countdownActive}, Started: {_started}");

            // Allow spells if duel not active or caster not a participant
            if (!_countdownActive && !_started) return true;
            if (caster != _challenger && caster != _challenged) return true;

            // RULE 1: Block polymorph and summon spells during ENTIRE DUEL (countdown + active)
            bool isPolymorphOrSummon = spell is Server.Spells.Seventh.PolymorphSpell ||
                                      spell is Server.Spells.Fifth.SummonCreatureSpell ||
                                      spell is Server.Spells.Fifth.BladeSpiritsSpell ||
                                      spell is Server.Spells.Eighth.EnergyVortexSpell ||
                                      spell is Server.Spells.Eighth.SummonDaemonSpell ||
                                      spell is Server.Spells.Eighth.AirElementalSpell ||
                                      spell is Server.Spells.Eighth.EarthElementalSpell ||
                                      spell is Server.Spells.Eighth.FireElementalSpell ||
                                      spell is Server.Spells.Eighth.WaterElementalSpell ||
                                      spell is Server.Spells.Necromancy.SummonFamiliarSpell ||
                                      spell is Server.Spells.Necromancy.AnimateDeadSpell ||
                                      spell is Server.Spells.Spellweaving.SummonFeySpell ||
                                      spell is Server.Spells.Spellweaving.SummonFiendSpell ||
                                      spell is Server.Spells.Spellweaving.NatureFurySpell;

            if (isPolymorphOrSummon)
            {
                caster.SendMessage("You cannot cast polymorph or summon spells during a duel!");
                return false;
            }

            // RULE 2: During countdown, only allow buff spells (Strength, Agility, Cunning, Bless, Protection, Magic Reflect)
            if (_countdownActive)
            {
                bool isAllowedBuff = spell is Server.Spells.Second.StrengthSpell ||
                                    spell is Server.Spells.Second.AgilitySpell ||
                                    spell is Server.Spells.Second.CunningSpell ||
                                    spell is Server.Spells.Third.BlessSpell ||
                                    spell is Server.Spells.Second.ProtectionSpell ||
                                    spell is Server.Spells.Fifth.MagicReflectSpell;

                Console.WriteLine($"[DuelPit] Countdown active - isAllowedBuff: {isAllowedBuff}");

                if (!isAllowedBuff)
                {
                    caster.SendMessage("You can only cast buff spells during the countdown!");
                    Console.WriteLine($"[DuelPit] Blocked spell during countdown: {spell.GetType().Name}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"[DuelPit] Allowed buff spell during countdown: {spell.GetType().Name}");
                }
            }

            // RULE 3: During active duel, check AllowSpellcasting rule
            if (_started && _rules != null && !_rules.AllowSpellcasting)
            {
                caster.SendMessage("Spellcasting is not allowed in this duel!");
                return false;
            }

            return true;
        }
    }
}
