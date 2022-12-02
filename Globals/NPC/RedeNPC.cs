using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Wasteland;
using Redemption.Tiles.Tiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Donator.Megaswave;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.NPCs.Bosses.ADD;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Armor.Single;
using Redemption.Buffs;
using Redemption.Items.Armor.Vanity.Dev;
using Redemption.Projectiles.Misc;
using Redemption.Items.Weapons.PreHM.Summon;
using System;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.Items.Donator.Lordfunnyman;

namespace Redemption.Globals.NPC
{
    public class RedeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool decapitated;
        public bool invisible;
        public float elementDmg = 1;
        public bool fallDownPlatform;
        public Entity attacker = Main.LocalPlayer;
        public Terraria.NPC npcTarget;

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.SkeletonMerchant)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CalciteWand>());
            if (type == NPCID.Dryad)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DruidHat>());
            if (type == NPCID.Cyborg)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GlobalDischarge>());
            if (type == NPCID.Clothier)
            {
                if (RedeBossDowned.downedThorn)
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ThornPlush>());
                if (RedeBossDowned.downedEaglecrestGolem)
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EaglecrestGolemPlush>());
            }
            if (type == NPCID.Wizard)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<NoidanSauva>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Pommisauva>());
            }
            if (type == NPCID.Princess)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HamPatPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TiedBoiPainting>());
                if (Main.expertMode && Terraria.NPC.boughtCat)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HallamHoodie>());
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HallamLeggings>());
                }
            }
        }
        public override void ResetEffects(Terraria.NPC npc)
        {
            invisible = false;
        }

        public override bool CanHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            if (target.RedemptionPlayerBuff().skeletonFriendly)
            {
                if (NPCLists.SkeletonHumanoid.Contains(npc.type))
                    return false;
            }
            return true;
        }

        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                #region Elemental Attributes
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type) || ItemLists.Wind.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Nature.Contains(item.type))
                        elementDmg *= 0.75f;

                    if (ItemLists.Poison.Contains(item.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (ItemLists.Holy.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Shadow.Contains(item.type))
                        elementDmg *= 0.8f;
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (ItemLists.Holy.Contains(item.type) || ItemLists.Celestial.Contains(item.type))
                        elementDmg *= 1.3f;

                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 0.5f;

                    if (ItemLists.Water.Contains(item.type) || ItemLists.Ice.Contains(item.type))
                        elementDmg *= 1.15f;
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (ItemLists.Holy.Contains(item.type) || ItemLists.Celestial.Contains(item.type) || ItemLists.Arcane.Contains(item.type))
                        elementDmg *= 1.15f;
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Ice.Contains(item.type))
                        elementDmg *= 0.75f;

                    if (ItemLists.Water.Contains(item.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Ice.Contains(item.type))
                        elementDmg *= 0.75f;

                    if (ItemLists.Thunder.Contains(item.type) || ItemLists.Wind.Contains(item.type))
                        elementDmg *= 1.1f;

                    if (ItemLists.Poison.Contains(item.type))
                        elementDmg *= 0.9f;
                }
                if (NPCLists.Hot.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 0.5f;

                    if (ItemLists.Ice.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Water.Contains(item.type) || ItemLists.Wind.Contains(item.type) || ItemLists.Poison.Contains(item.type))
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Wet.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 0.75f;

                    if (ItemLists.Ice.Contains(item.type) || ItemLists.Poison.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Water.Contains(item.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (ItemLists.Fire.Contains(item.type))
                        elementDmg *= 1.15f;

                    if (ItemLists.Ice.Contains(item.type))
                        elementDmg *= 0.7f;

                    if (ItemLists.Blood.Contains(item.type))
                        elementDmg *= 1.25f;

                    if (ItemLists.Poison.Contains(item.type))
                        elementDmg *= 0.25f;
                }
                if (((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)) && ItemLists.Thunder.Contains(item.type))
                    elementDmg *= 1.1f;
                if (!npc.noTileCollide && npc.collideY && ItemLists.Earth.Contains(item.type))
                    elementDmg *= 1.1f;
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (ItemLists.Blood.Contains(item.type) || ItemLists.Poison.Contains(item.type))
                        elementDmg *= 0.5f;

                    if (ItemLists.Thunder.Contains(item.type))
                        elementDmg *= 1.1f;

                    if (ItemLists.Water.Contains(item.type))
                        elementDmg *= 1.3f;
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (ItemLists.Blood.Contains(item.type))
                        elementDmg *= 1.1f;

                    if (ItemLists.Poison.Contains(item.type))
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Hallowed.Contains(npc.type))
                {
                    if (ItemLists.Celestial.Contains(item.type))
                        elementDmg *= 0.9f;

                    if (ItemLists.Holy.Contains(item.type))
                        elementDmg *= 0.5f;

                    if (ItemLists.Shadow.Contains(item.type))
                        elementDmg *= 1.25f;
                }
                if (ItemLists.Poison.Contains(item.type) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                    elementDmg *= 1.15f;
                if (ItemLists.Wind.Contains(item.type) && (npc.noGravity || !npc.collideY))
                    knockback = (int)((knockback * 1.1f) + 2);

                elementDmg = (int)Math.Round(elementDmg * 100);
                elementDmg /= 100;
                if (elementDmg >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, "+" + elementDmg, true, true);
                else if (elementDmg <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, "-" + elementDmg, true, true);

                damage = (int)(damage * elementDmg);
                elementDmg = 1;
                #endregion
            }

            // Decapitation
            if (npc.life < npc.lifeMax && item.CountsAsClass(DamageClass.Melee) && item.pick == 0 && item.hammer == 0 && !item.noUseGraphic && item.damage > 0 && item.useStyle == ItemUseStyleID.Swing && NPCLists.SkeletonHumanoid.Contains(npc.type))
            {
                if (Main.rand.NextBool(200) && !ItemLists.BluntSwing.Contains(item.type))
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
                else if (Main.rand.NextBool(80) && (item.axe > 0 || item.Redemption().TechnicallyAxe) && item.type != ModContent.ItemType<BeardedHatchet>())
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ProjectileLists.NoElement.Contains(projectile.type))
            {
                #region Elemental Attributes
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type) || ProjectileLists.Wind.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Nature.Contains(projectile.type))
                        elementDmg *= 0.75f;

                    if (ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (ProjectileLists.Holy.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Shadow.Contains(projectile.type))
                        elementDmg *= 0.8f;
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (ProjectileLists.Holy.Contains(projectile.type) || ProjectileLists.Celestial.Contains(projectile.type))
                        elementDmg *= 1.3f;

                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 0.75f;

                    if (ProjectileLists.Water.Contains(projectile.type) || ProjectileLists.Ice.Contains(projectile.type))
                        elementDmg *= 1.15f;
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (ProjectileLists.Holy.Contains(projectile.type) || ProjectileLists.Celestial.Contains(projectile.type) || ProjectileLists.Arcane.Contains(projectile.type))
                        elementDmg *= 1.15f;
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Ice.Contains(projectile.type))
                        elementDmg *= 0.75f;

                    if (ProjectileLists.Water.Contains(projectile.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Ice.Contains(projectile.type))
                        elementDmg *= 0.5f;

                    if (ProjectileLists.Thunder.Contains(projectile.type) || ProjectileLists.Wind.Contains(projectile.type))
                        elementDmg *= 1.1f;

                    if (ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 0.9f;
                }
                if (NPCLists.Hot.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 0.5f;

                    if (ProjectileLists.Ice.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Water.Contains(projectile.type) || ProjectileLists.Wind.Contains(projectile.type) || ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Wet.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 0.75f;

                    if (ProjectileLists.Ice.Contains(projectile.type) || ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Water.Contains(projectile.type))
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (ProjectileLists.Fire.Contains(projectile.type))
                        elementDmg *= 1.15f;

                    if (ProjectileLists.Ice.Contains(projectile.type))
                        elementDmg *= 0.7f;

                    if (ProjectileLists.Blood.Contains(projectile.type))
                        elementDmg *= 1.25f;

                    if (ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 0.25f;
                }
                if (((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)) && ProjectileLists.Thunder.Contains(projectile.type))
                    elementDmg *= 1.1f;
                if (!npc.noTileCollide && npc.collideY && ProjectileLists.Earth.Contains(projectile.type))
                    elementDmg *= 1.1f;
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (ProjectileLists.Blood.Contains(projectile.type) || ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 0.5f;

                    if (ProjectileLists.Thunder.Contains(projectile.type))
                        elementDmg *= 1.1f;

                    if (ProjectileLists.Water.Contains(projectile.type))
                        elementDmg *= 1.25f;
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (ProjectileLists.Blood.Contains(projectile.type))
                        elementDmg *= 1.1f;

                    if (ProjectileLists.Poison.Contains(projectile.type))
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Hallowed.Contains(npc.type))
                {
                    if (ProjectileLists.Celestial.Contains(projectile.type))
                        elementDmg *= 0.9f;

                    if (ProjectileLists.Holy.Contains(projectile.type))
                        elementDmg *= 0.5f;

                    if (ProjectileLists.Shadow.Contains(projectile.type))
                        elementDmg *= 1.25f;
                }
                if (ProjectileLists.Poison.Contains(projectile.type) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                    elementDmg *= 1.15f;
                if (ProjectileLists.Wind.Contains(projectile.type) && (npc.noGravity || !npc.collideY))
                    knockback = (int)((knockback * 1.1f) + 2);

                elementDmg = (int)Math.Round(elementDmg * 100);
                elementDmg /= 100;
                if (elementDmg >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, "+" + elementDmg, true, true);
                else if (elementDmg <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, "-" + elementDmg, true, true);

                damage = (int)(damage * elementDmg);
                elementDmg = 1;
                #endregion
            }
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            target.Redemption().attacker = npc;
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, int damage, float knockback, bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && ItemLists.Ice.Contains(item.type))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && ItemLists.Ice.Contains(item.type))
                    {
                        SoundEngine.PlaySound(SoundID.Item30, npc.position);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ItemLists.Fire.Contains(item.type))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && ItemLists.Thunder.Contains(item.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && ItemLists.Earth.Contains(item.type))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ItemLists.Water.Contains(item.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (ItemLists.Shadow.Contains(item.type))
                {
                    int c = player.HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                if (ItemLists.Nature.Contains(item.type) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (player.RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (player.RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<NaturePickup>(), noGrabDelay: true);
                }
                if (ItemLists.Celestial.Contains(item.type))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(4) && npc.CanBeChasedBy())
                        Projectile.NewProjectile(npc.GetSource_OnHurt(player), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ModContent.ProjectileType<CelestialStar>(), 0, 0, player.whoAmI, npc.whoAmI);
                }
                #endregion
            }

            attacker = player;
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ProjectileLists.NoElement.Contains(projectile.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && ProjectileLists.Ice.Contains(projectile.type))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && ProjectileLists.Ice.Contains(projectile.type))
                    {
                        SoundEngine.PlaySound(SoundID.Item30, npc.position);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ProjectileLists.Fire.Contains(projectile.type))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && ProjectileLists.Thunder.Contains(projectile.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && ProjectileLists.Earth.Contains(projectile.type))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ProjectileLists.Water.Contains(projectile.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (ProjectileLists.Shadow.Contains(projectile.type))
                {
                    int c = Main.player[projectile.owner].HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                if (ProjectileLists.Nature.Contains(projectile.type) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<NaturePickup>(), noGrabDelay: true);
                }
                if (ProjectileLists.Celestial.Contains(projectile.type))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(4) && npc.CanBeChasedBy())
                        Projectile.NewProjectile(npc.GetSource_OnHurt(Main.player[projectile.owner]), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ModContent.ProjectileType<CelestialStar>(), 0, 0, projectile.owner, npc.whoAmI);
                }
                #endregion
            }

            if (RedeProjectile.projOwners.TryGetValue(projectile.whoAmI, out (Entity entity, IEntitySource source) value))
            {
                bool g = false;
                if (value.entity is Terraria.NPC valueNPC && valueNPC.whoAmI == npc.whoAmI)
                    g = true;

                if (!g)
                    attacker = value.entity;
            }
        }
        public override void OnKill(Terraria.NPC npc)
        {
            if (NPCID.Sets.Skeletons[npc.type] && Main.rand.NextBool(3) && !npc.SpawnedFromStatue)
                RedeHelper.SpawnNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
        public override void ModifyNPCLoot(Terraria.NPC npc, NPCLoot npcLoot)
        {
            if (NPCLists.SkeletonHumanoid.Contains(npc.type))
            {
                DecapitationCondition decapitationDropCondition = new();
                IItemDropRule conditionalRule = new LeadingConditionRule(decapitationDropCondition);
                int itemType = ItemID.Skull;
                if (npc.type == ModContent.NPCType<CorpseWalkerPriest>())
                    itemType = ModContent.ItemType<CorpseWalkerSkullVanity>();
                else if (npc.type == ModContent.NPCType<EpidotrianSkeleton>() || npc.type == ModContent.NPCType<SkeletonAssassin>() ||
                    npc.type == ModContent.NPCType<SkeletonDuelist>() || npc.type == ModContent.NPCType<SkeletonFlagbearer>() ||
                    npc.type == ModContent.NPCType<SkeletonNoble>() || npc.type == ModContent.NPCType<SkeletonWanderer>() ||
                    npc.type == ModContent.NPCType<SkeletonWarden>())
                    itemType = ModContent.ItemType<EpidotrianSkull>();

                IItemDropRule rule = ItemDropRule.Common(itemType);
                conditionalRule.OnSuccess(rule);
                npcLoot.Add(conditionalRule);
            }
            if (npc.type is NPCID.BoneSerpentHead)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmolderedScale>(), 20));
            if (npc.type is NPCID.Ghost or NPCID.Wraith)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<Soulshake>(), 150));
            if (npc.type is NPCID.AngryBones or NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.CursedSkull or NPCID.DarkCaster)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<Incisor>(), 100));
            if (npc.type is NPCID.Demon or NPCID.VoodooDemon or NPCID.FireImp)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<ForgottenSword>(), 100));
            if (npc.type is NPCID.GraniteFlyer or NPCID.GraniteGolem)
            {
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<GaucheStaff>(), 30));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LegoBrick>(), 200));
            }
            if (npc.type is NPCID.Dandelion)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<GiantDandelion>(), 10));
            if (npc.type is NPCID.MoonLordCore)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Keycard>()));
            if (npc.type is NPCID.Golem)
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GolemStaff>(), 7));
            if (npc.type is NPCID.IceGolem or NPCID.RockGolem)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LegoBrick>(), 50));
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new YoyosTidalWake(), ModContent.ItemType<TidalWake>(), 200));
        }
        public override void EditSpawnRate(Terraria.Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (RedeWorld.blobbleSwarm)
            {
                spawnRate = 10;
                maxSpawns = 20;
            }
            if (RedeWorld.SkeletonInvasion)
            {
                spawnRate = 19;
                maxSpawns = 12;
            }
            if (player.InModBiome<LabBiome>())
            {
                spawnRate = 20;
                maxSpawns = 12;
            }
            if (player.InModBiome<WastelandPurityBiome>())
            {
                spawnRate = 30;
                maxSpawns = 10;
            }
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (RedeWorld.blobbleSwarm)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Blobble>(), 10);
            }

            if (RedeWorld.SkeletonInvasion && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<RaveyardSkeletonSpawner>(), 2);
                pool.Add(ModContent.NPCType<EpidotrianSkeleton>(), 5);
                pool.Add(ModContent.NPCType<CavernSkeletonSpawner>(), 5);
                pool.Add(ModContent.NPCType<SurfaceSkeletonSpawner>(), 2);
                pool.Add(ModContent.NPCType<CorpseWalkerPriest>(), 0.5f);
                pool.Add(ModContent.NPCType<JollyMadman>(), 0.02f);
            }
            if (spawnInfo.Player.InModBiome<LabBiome>())
            {
                if (!RedeWorld.labSafe)
                {
                    pool.Clear();
                    pool.Add(ModContent.NPCType<LabSentryDrone>(), 10);
                }
                else
                {
                    int[] LabTileArray = { ModContent.TileType<LabPlatingTileUnsafe>(), ModContent.TileType<OvergrownLabPlatingTile>(), ModContent.TileType<DangerTapeTile>(), ModContent.TileType<HardenedSludgeTile>(), ModContent.TileType<BlackHardenedSludgeTile>() };
                    bool tileCheck = LabTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                    pool.Clear();
                    pool.Add(ModContent.NPCType<BlisteredScientist>(), tileCheck ? 1 : 0);
                    pool.Add(ModContent.NPCType<OozingScientist>(), tileCheck ? 0.7f : 0);
                    pool.Add(ModContent.NPCType<BloatedScientist>(), tileCheck ? 0.2f : 0);
                    if (spawnInfo.Water)
                        pool.Add(ModContent.NPCType<BlisteredFish>(), 0.4f);
                }
            }
            if (spawnInfo.Player.InModBiome<WastelandPurityBiome>() && !spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                int[] GrassTileArray = { ModContent.TileType<IrradiatedCorruptGrassTile>(), ModContent.TileType<IrradiatedCrimsonGrassTile>(), ModContent.TileType<IrradiatedGrassTile>() };
                bool tileCheck = GrassTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                pool.Clear();
                pool.Add(NPCID.ToxicSludge, !Terraria.NPC.downedMechBossAny ? 0.8f : 0.3f);
                pool.Add(NPCID.GreenJellyfish, spawnInfo.Water && !Terraria.NPC.downedMechBossAny ? 0.6f : 0);
                pool.Add(ModContent.NPCType<BloatedGoldfish>(), spawnInfo.Water ? 2f : 0);
                pool.Add(ModContent.NPCType<RadioactiveJelly>(), spawnInfo.Water && Terraria.NPC.downedMechBossAny ? 1f : 0);
                pool.Add(ModContent.NPCType<HazmatZombie>(), 1f);
                pool.Add(ModContent.NPCType<BobTheBlob>(), 0.05f);
                pool.Add(ModContent.NPCType<RadioactiveSlime>(), 0.9f);
                pool.Add(ModContent.NPCType<NuclearSlime>(), 0.3f);
                pool.Add(ModContent.NPCType<HazmatBunny>(), Main.dayTime ? 0.1f : 0);
                pool.Add(ModContent.NPCType<SickenedBunny>(), Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<SickenedDemonEye>(), !Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<NuclearShadow>(), 0.2f);
                pool.Add(ModContent.NPCType<MutatedLivingBloom>(), tileCheck ? (Main.raining ? 0.4f : 0.2f) : 0f);
                if (spawnInfo.Player.InModBiome<WastelandSnowBiome>())
                {
                    pool.Add(ModContent.NPCType<SneezyFlinx>(), 0.8f);
                    pool.Add(ModContent.NPCType<SicklyWolf>(), 0.7f);
                    pool.Add(ModContent.NPCType<SicklyPenguin>(), 0.6f);
                }
                if (spawnInfo.Player.InModBiome<WastelandDesertBiome>())
                {
                    pool.Add(ModContent.NPCType<BloatedGhoul>(), 1f);
                }
            }
            if (spawnInfo.Player.InModBiome<BlazingBastionBiome>())
            {
                pool.Clear();
                pool.Add(NPCID.Demon, 1f);
                pool.Add(NPCID.FireImp, 0.3f);
                pool.Add(NPCID.VoodooDemon, 0.3f);
                if (Terraria.NPC.downedMechBossAny)
                    pool.Add(NPCID.RedDevil, 0.2f);
                pool.Add(NPCID.HellButterfly, 0.1f);
                pool.Add(NPCID.Lavafly, 0.1f);
                pool.Add(NPCID.MagmaSnail, 0.1f);
                if (Terraria.NPC.downedPlantBoss)
                {
                    pool.Add(NPCID.HellArmoredBones, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesMace, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesSpikeShield, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesSword, 0.2f);
                }
            }
            if (spawnInfo.Player.RedemptionScreen().cutscene)
                pool.Clear();
        }
    }
}