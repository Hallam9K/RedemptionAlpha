using rail;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals.Players;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Usable;
using Redemption.Projectiles.Misc;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class ElementalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public sbyte[] OverrideElement = new sbyte[16];
        public float[] OverrideMultiplier = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public float[] elementDmg = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public bool uncappedBossMultiplier;

        private float bloodLossFill;
        private float bloodLossExtra;
        private float bloodLossCD;
        private float bloodLossResistance = 1;
        private bool bloodLossed;

        public override void SetDefaults(Terraria.NPC npc)
        {
            SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
        }
        public override void ResetEffects(Terraria.NPC npc)
        {
            bloodLossed = false;
            bloodLossCD--;
            if (bloodLossCD <= 0)
                bloodLossExtra--;

            if (bloodLossFill > 0 && bloodLossExtra <= 0)
                bloodLossFill *= 0.98f;
        }
        private bool BloodLossEffect(Terraria.NPC target, int maxHealth, int damage, bool crit = false)
        {
            if (bloodLossed || target.dontTakeDamage || target.immortal || target.buffImmune[BuffID.BloodButcherer] || NPCLists.Inorganic.Contains(target.type))
                return false;

            bloodLossExtra = Math.Max(bloodLossExtra, 0);

            float bloodResist = target.GetGlobalNPC<ElementalNPC>().elementDmg[ElementID.Blood];

            damage /= 10;
            if (crit)
                damage *= 2;

            if (bloodLossCD > 0)
                bloodLossExtra += damage * 1.5f;

            bloodLossCD = 90;

            bloodLossFill += ((damage) + bloodLossExtra) * bloodResist;

            if (bloodLossFill >= maxHealth * bloodLossResistance / bloodResist)
            {
                RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Blood);

                bloodLossFill = 0;
                bloodLossExtra = 0;
                bloodLossResistance += .5f;
                bloodLossed = true;

                AdvancedPopupRequest bloodText = new();
                bloodText.Text = Language.GetTextValue("Mods.Redemption.StatusMessage.Other.BloodLoss");
                bloodText.DurationInFrames = 60;
                bloodText.Color = Color.Red;
                bloodText.Velocity = new Vector2(0, -8);
                PopupText.NewText(bloodText, target.Top);

                SoundEngine.PlaySound(SoundID.NPCDeath21, target.position);
                SoundEngine.PlaySound(SoundID.NPCDeath22, target.position);
                for (int i = 0; i < 70; i++)
                {
                    int d = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 3));
                    Main.dust[d].velocity.X *= Main.rand.Next(4, 9);
                }
                return true;
            }
            return false;
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, Terraria.NPC.HitInfo hit)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                #region Elemental Attributes
                if (target.life > 0 && npc.HasElement(ElementID.Blood) && BloodLossEffect(target, target.lifeMax, hit.Damage, hit.Crit))
                {
                    int damage = target.boss ? (int)(target.lifeMax * 0.05f) : (int)(target.lifeMax * 0.15f);
                    damage += (int)(100 * target.GetGlobalNPC<ElementalNPC>().elementDmg[ElementID.Blood]);
                    BaseAI.DamageNPC(target, damage, 0, null, false);
                }

                if (NPCLists.Infected.Contains(target.type))
                {
                    if (Main.rand.NextBool(4) && target.life < target.lifeMax && npc.HasElement(ElementID.Ice))
                        target.AddBuff(BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(target.type))
                {
                    if (Main.rand.NextBool(8) && target.life < target.lifeMax && target.knockBackResist > 0 && !target.RedemptionNPCBuff().iceFrozen && npc.HasElement(ElementID.Ice))
                    {
                        SoundEngine.PlaySound(SoundID.Item30);
                        target.AddBuff(BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(target.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(target.type) || NPCLists.Cold.Contains(target.type) || NPCLists.IsSlime.Contains(target.type))
                {
                    if (Main.rand.NextBool(4) && npc.HasElement(ElementID.Fire))
                        target.AddBuff(BuffID.OnFire, 180);
                }
                if ((target.wet && !target.lavaWet) || target.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && npc.HasElement(ElementID.Thunder))
                        target.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!target.noTileCollide && target.collideY && target.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && npc.HasElement(ElementID.Earth))
                        target.AddBuff(BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(target.type))
                {
                    if (Main.rand.NextBool(4) && npc.HasElement(ElementID.Water))
                        target.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                }
                if (NPCLists.Demon.Contains(target.type) && Main.rand.NextBool(6) && npc.HasElement(ElementID.Holy))
                    target.AddBuff(BuffType<HolyFireDebuff>(), 120);

                if (npc.Redemption().spiritSummon)
                {
                    int player = (int)npc.ai[3];
                    if (player > -1)
                    {
                        if (npc.HasElement(ElementID.Shadow))
                        {
                            int c = Main.player[player].HasBuff<EvilJellyBuff>() ? 3 : 6;
                            if (Main.rand.NextBool(c) && target.life <= 0 && target.lifeMax > 5)
                                Item.NewItem(target.GetSource_Loot(), target.getRect(), ItemType<ShadowFuel>(), noGrabDelay: true);
                        }
                        if (npc.HasElement(ElementID.Nature) && target.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(target))
                        {
                            int c = 6;
                            if (Main.player[player].RedemptionPlayerBuff().shellNecklace)
                                c = (int)(c * 0.75f);
                            if (Main.player[player].RedemptionPlayerBuff().forestCore)
                                c = (int)(c * 0.75f);
                            if (Main.rand.NextBool(c) && target.CanBeChasedBy())
                                Item.NewItem(target.GetSource_Loot(), target.getRect(), ItemType<NaturePickup>(), noGrabDelay: true);
                        }
                        if (npc.HasElement(ElementID.Celestial))
                        {
                            int type = 0;
                            if (Main.player[player].setSolar)
                                type = 1;
                            int c = 6;
                            if (Main.player[player].GetModPlayer<WaterfowlEgg_Player>().equipped)
                                c = 12;
                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(c) && target.CanBeChasedBy())
                                Projectile.NewProjectile(target.GetSource_OnHurt(Main.player[player]), target.Center + RedeHelper.Spread(400), Vector2.Zero, ProjectileType<CelestialStar>(), 0, 0, Main.player[player].whoAmI, target.whoAmI, type);
                        }
                    }
                }
                #endregion
            }
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigServer.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                #region Elemental Attributes
                if (npc.life > 0 && item.HasElement(ElementID.Blood) && BloodLossEffect(npc, npc.lifeMax, hit.Damage, hit.Crit))
                {
                    int damage = npc.boss ? (int)(npc.lifeMax * 0.05f) : (int)(npc.lifeMax * 0.15f);
                    damage += (int)(100 * npc.GetGlobalNPC<ElementalNPC>().elementDmg[ElementID.Blood]);
                    BaseAI.DamageNPC(npc, damage, 0, null, false);
                }

                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && item.HasElement(ElementID.Ice))
                    {
                        npc.AddBuff(BuffType<PureChillDebuff>(), 600);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Ice);
                    }
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && item.HasElement(ElementID.Ice))
                    {
                        SoundEngine.PlaySound(SoundID.Item30);
                        npc.AddBuff(BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Ice);
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && item.HasElement(ElementID.Fire))
                    {
                        npc.AddBuff(BuffID.OnFire, 180);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Fire);
                    }
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && item.HasElement(ElementID.Thunder))
                    {
                        npc.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Thunder);
                    }
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && item.HasElement(ElementID.Earth))
                    {
                        npc.AddBuff(BuffType<StunnedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Earth);
                    }
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && item.HasElement(ElementID.Water))
                    {
                        npc.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Water);
                    }
                }
                if (NPCLists.Demon.Contains(npc.type) && Main.rand.NextBool(6) && item.HasElement(ElementID.Holy))
                {
                    npc.AddBuff(BuffType<HolyFireDebuff>(), 120);
                    RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Holy);
                }
                if (item.HasElement(ElementID.Shadow))
                {
                    int c = player.HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemType<ShadowFuel>(), noGrabDelay: true);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Shadow);
                    }
                }
                if (item.HasElement(ElementID.Nature) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (player.RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (player.RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemType<NaturePickup>(), noGrabDelay: true);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Nature);
                    }
                }
                if (item.HasElement(ElementID.Celestial))
                {
                    int type = 0;
                    if (player.setSolar)
                        type = 1;
                    int c = 6;
                    if (player.GetModPlayer<WaterfowlEgg_Player>().equipped)
                        c = 12;
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_OnHurt(player), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ProjectileType<CelestialStar>(), 0, 0, player.whoAmI, npc.whoAmI, type);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Celestial);
                    }
                }
                #endregion
            }
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigServer.Instance.ElementDisable && !ProjectileLists.NoElement.Contains(projectile.type))
            {
                #region Elemental Attributes
                if (npc.life > 0 && projectile.HasElement(ElementID.Blood) && BloodLossEffect(npc, npc.lifeMax, hit.Damage, hit.Crit))
                {
                    int damage = npc.boss ? (int)(npc.lifeMax * 0.05f) : (int)(npc.lifeMax * 0.15f);
                    damage += (int)(100 * npc.GetGlobalNPC<ElementalNPC>().elementDmg[ElementID.Blood]);
                    BaseAI.DamageNPC(npc, damage, 0, null, false);
                }

                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && projectile.HasElement(ElementID.Ice))
                    {
                        npc.AddBuff(BuffType<PureChillDebuff>(), 600);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Ice);
                    }
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && projectile.HasElement(ElementID.Ice))
                    {
                        SoundEngine.PlaySound(SoundID.Item30);
                        npc.AddBuff(BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Ice);
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && projectile.HasElement(ElementID.Fire))
                    {
                        npc.AddBuff(BuffID.OnFire, 180);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Fire);
                    }
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && projectile.HasElement(ElementID.Thunder))
                    {
                        npc.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Thunder);
                    }
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && projectile.HasElement(ElementID.Earth))
                    {
                        npc.AddBuff(BuffType<StunnedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Earth);
                    }
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && projectile.HasElement(ElementID.Water))
                    {
                        npc.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Water);
                    }
                }
                if (NPCLists.Demon.Contains(npc.type) && Main.rand.NextBool(6) && projectile.HasElement(ElementID.Holy))
                {
                    npc.AddBuff(BuffType<HolyFireDebuff>(), 120);
                    RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Holy);
                }
                if (projectile.HasElement(ElementID.Shadow))
                {
                    int c = Main.player[projectile.owner].HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemType<ShadowFuel>(), noGrabDelay: true);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Shadow);
                    }
                }
                if (projectile.HasElement(ElementID.Nature) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemType<NaturePickup>(), noGrabDelay: true);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Nature);
                    }
                }
                if (projectile.HasElement(ElementID.Celestial))
                {
                    int type = 0;
                    if (Main.player[projectile.owner].setSolar)
                        type = 1;
                    int c = 6;
                    if (Main.player[projectile.owner].GetModPlayer<WaterfowlEgg_Player>().equipped)
                        c = 12;
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_OnHurt(Main.player[projectile.owner]), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ProjectileType<CelestialStar>(), 0, 0, projectile.owner, npc.whoAmI, type);
                        RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Celestial);
                    }
                }
                #endregion
            }
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                BuffPlayer modPlayer = player.RedemptionPlayerBuff();
                for (int i = 1; i <= 14; i++)
                {
                    if (item.HasElementItem(i))
                        modifiers.FinalDamage *= 1 + modPlayer.ElementalDamage[i];
                }

                #region Elemental Damage Bonus
                float multiplier = 1;
                ElementalEffects(npc, player, item, ref multiplier, ref modifiers);
                SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
                for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().elementDmg.Length; j++)
                {
                    if (npc.GetGlobalNPC<ElementalNPC>().elementDmg[j] is 1 || !item.HasElement(j))
                        continue;
                    multiplier *= npc.GetGlobalNPC<ElementalNPC>().elementDmg[j];
                }
                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;
                if (npc.boss && !uncappedBossMultiplier)
                    multiplier = MathHelper.Clamp(multiplier, .75f, 1.25f);

                if (multiplier >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;
                #endregion
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable && !ItemLists.NoElement.Contains(projectile.type))
            {
                BuffPlayer modPlayer = Main.player[projectile.owner].RedemptionPlayerBuff();
                for (int i = 1; i <= 14; i++)
                {
                    if (projectile.HasElement(i))
                        modifiers.FinalDamage *= 1 + modPlayer.ElementalDamage[i];
                }

                #region Elemental Damage Bonus
                float multiplier = 1;
                ElementalEffects(npc, projectile, ref multiplier, ref modifiers);
                SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
                for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().elementDmg.Length; j++)
                {
                    if (npc.GetGlobalNPC<ElementalNPC>().elementDmg[j] is 1 || !projectile.HasElement(j))
                        continue;
                    multiplier *= npc.GetGlobalNPC<ElementalNPC>().elementDmg[j];
                }
                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;
                if (npc.boss && !uncappedBossMultiplier)
                    multiplier = MathHelper.Clamp(multiplier, .75f, 1.25f);

                if (multiplier >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;
                #endregion
            }
        }
        public override void ModifyHitNPC(Terraria.NPC npc, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                #region Elemental Damage Bonus
                float multiplier = 1;
                ElementalEffects(target, npc, ref multiplier, ref modifiers);
                SetElementalMultipliers(target, ref target.GetGlobalNPC<ElementalNPC>().elementDmg);
                for (int j = 0; j < target.GetGlobalNPC<ElementalNPC>().elementDmg.Length; j++)
                {
                    if (target.GetGlobalNPC<ElementalNPC>().elementDmg[j] is 1 || !npc.HasElement(j))
                        continue;
                    multiplier *= target.GetGlobalNPC<ElementalNPC>().elementDmg[j];
                }
                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;
                if (target.boss && !uncappedBossMultiplier)
                    multiplier = MathHelper.Clamp(multiplier, .75f, 1.25f);

                if (multiplier >= 1.1f)
                    CombatText.NewText(target.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(target.getRect(), Color.IndianRed, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;
                #endregion
            }
        }
        public static void SetElementalMultipliers(Terraria.NPC npc, ref float[] multiplier)
        {
            for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier.Length; j++)
            {
                if (npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[j] == 1)
                    multiplier[j] = 1;
                else
                    multiplier[j] = npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[j];
            }
            if (NPCLists.Plantlike.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Wind] *= 1.25f;
                multiplier[ElementID.Nature] *= 0.75f;
                multiplier[ElementID.Poison] *= 0.5f;
            }
            if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.25f;
                multiplier[ElementID.Shadow] *= 0.8f;
            }
            if (NPCLists.Demon.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.3f;
                multiplier[ElementID.Celestial] *= 1.2f;
                multiplier[ElementID.Fire] *= 0.5f;
                multiplier[ElementID.Water] *= 1.15f;
                multiplier[ElementID.Ice] *= 1.15f;
            }
            if (NPCLists.Spirit.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.15f;
                multiplier[ElementID.Celestial] *= 1.15f;
                multiplier[ElementID.Arcane] *= 1.25f;
            }
            if (NPCLists.IsSlime.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Ice] *= 0.75f;
                multiplier[ElementID.Water] *= 0.5f;
            }
            if (NPCLists.Cold.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Ice] *= 0.75f;
                multiplier[ElementID.Thunder] *= 1.1f;
                multiplier[ElementID.Wind] *= 1.1f;
                multiplier[ElementID.Poison] *= 0.9f;
            }
            if (NPCLists.Hot.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 0.8f;
                multiplier[ElementID.Ice] *= 1.25f;
                multiplier[ElementID.Water] *= 1.1f;
                multiplier[ElementID.Wind] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.1f;
            }
            if (NPCLists.Wet.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 0.75f;
                multiplier[ElementID.Ice] *= 1.25f;
                multiplier[ElementID.Poison] *= 1.25f;
                multiplier[ElementID.Water] *= 0.75f;
            }
            if (NPCLists.Infected.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.15f;
                multiplier[ElementID.Ice] *= 0.7f;
                multiplier[ElementID.Blood] *= 1.25f;
                multiplier[ElementID.Poison] *= 0.25f;
            }
            if (NPCLists.Robotic.Contains(npc.type))
            {
                multiplier[ElementID.Blood] *= 0.75f;
                if (!npc.HasBuff<HydraAcidDebuff>())
                    multiplier[ElementID.Poison] *= 0.75f;
                multiplier[ElementID.Thunder] *= 1.1f;
                multiplier[ElementID.Water] *= 1.35f;
            }
            if (!NPCLists.Inorganic.Contains(npc.type) && !NPCLists.Spirit.Contains(npc.type))
            {
                multiplier[ElementID.Blood] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.05f;
            }
            if (NPCLists.Hallowed.Contains(npc.type))
            {
                multiplier[ElementID.Celestial] *= 0.9f;
                multiplier[ElementID.Holy] *= 0.5f;
                multiplier[ElementID.Shadow] *= 1.25f;
            }
            if (NPCLists.Dark.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.15f;
                multiplier[ElementID.Nature] *= 1.25f;
                multiplier[ElementID.Shadow] *= 0.75f;
            }
            if (NPCLists.Blood.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.1f;
                multiplier[ElementID.Ice] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.25f;
                multiplier[ElementID.Shadow] *= 0.9f;
                multiplier[ElementID.Blood] *= 0.75f;
            }
        }
        public static void ElementalEffects(Terraria.NPC npc, Terraria.Player player, Item item, ref float multiplier, ref Terraria.NPC.HitModifiers knockback)
        {
            if (item.HasElement(ElementID.Shadow) && NPCLists.Dark.Contains(npc.type) && player.RedemptionPlayerBuff().eldritchRoot)
                multiplier *= 1.33333f;

            if (item.HasElement(ElementID.Thunder) && ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)))
                multiplier *= 1.1f;
            if (item.HasElement(ElementID.Earth) && !npc.noTileCollide && npc.collideY)
                multiplier *= 1.1f;

            if (item.HasElement(ElementID.Poison) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
            {
                multiplier *= 1.1f;
                RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Poison);
            }
            if (item.HasElement(ElementID.Wind) && (npc.noGravity || !npc.collideY))
            {
                knockback.Knockback *= 1.25f;
                if (npc.knockBackResist > 0)
                    knockback.Knockback.Flat += 2;

                RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Wind);
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
        public static void ElementalEffects(Terraria.NPC npc, Projectile proj, ref float multiplier, ref Terraria.NPC.HitModifiers knockback)
        {
            if (proj.HasElement(ElementID.Shadow) && NPCLists.Dark.Contains(npc.type) && Main.player[proj.owner].RedemptionPlayerBuff().eldritchRoot)
                multiplier *= 1.33333f;

            if (proj.HasElement(ElementID.Thunder) && ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)))
                multiplier *= 1.1f;
            if (proj.HasElement(ElementID.Earth) && !npc.noTileCollide && npc.collideY)
                multiplier *= 1.1f;

            if (proj.HasElement(ElementID.Poison) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
            {
                multiplier *= 1.1f;
                RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Poison);
            }
            if (proj.HasElement(ElementID.Wind) && (npc.noGravity || !npc.collideY))
            {
                knockback.Knockback *= 1.25f;
                if (npc.knockBackResist > 0)
                    knockback.Knockback.Flat += 2;

                RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Wind);
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
        public static void ElementalEffects(Terraria.NPC npc, Terraria.NPC attacker, ref float multiplier, ref Terraria.NPC.HitModifiers knockback)
        {
            if (attacker.HasElement(ElementID.Thunder) && ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)))
                multiplier *= 1.1f;
            if (attacker.HasElement(ElementID.Earth) && !npc.noTileCollide && npc.collideY)
                multiplier *= 1.1f;

            if (attacker.HasElement(ElementID.Poison) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
            {
                multiplier *= 1.1f;
            }
            if (attacker.HasElement(ElementID.Wind) && (npc.noGravity || !npc.collideY))
            {
                knockback.Knockback *= 1.25f;
                if (npc.knockBackResist > 0)
                    knockback.Knockback.Flat += 2;
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
        public static void ElementalEffects(Terraria.Player player, Terraria.NPC attacker, ref float multiplier, ref Terraria.Player.HurtModifiers knockback)
        {
            if (attacker.HasElement(ElementID.Thunder) && ((player.wet && !player.lavaWet) || player.HasBuff(BuffID.Wet)))
                multiplier *= 1.1f;
            if (attacker.HasElement(ElementID.Earth) && player.velocity.Y == 0)
                multiplier *= 1.1f;

            if (attacker.HasElement(ElementID.Poison) && (player.poisoned || player.venom || player.RedemptionPlayerBuff().dirtyWound))
                multiplier *= 1.1f;
            if (attacker.HasElement(ElementID.Wind) && player.velocity.Y != 0)
            {
                if (!player.noKnockback)
                    knockback.Knockback *= 1.25f;
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
        public static void ElementalEffects(Terraria.Player player, Projectile projectile, ref float multiplier, ref Terraria.Player.HurtModifiers knockback)
        {
            if (projectile.HasElement(ElementID.Thunder) && ((player.wet && !player.lavaWet) || player.HasBuff(BuffID.Wet)))
                multiplier *= 1.1f;
            if (projectile.HasElement(ElementID.Earth) && player.velocity.Y == 0)
                multiplier *= 1.1f;

            if (projectile.HasElement(ElementID.Poison) && (player.poisoned || player.venom || player.RedemptionPlayerBuff().dirtyWound))
                multiplier *= 1.1f;
            if (projectile.HasElement(ElementID.Wind) && player.velocity.Y != 0)
            {
                if (!player.noKnockback)
                    knockback.Knockback *= 1.25f;
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
    }
}