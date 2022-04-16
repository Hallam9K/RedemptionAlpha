using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Items.Accessories.PreHM;
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

namespace Redemption.Globals.NPC
{
    public class RedeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool decapitated;
        public bool invisible;
        public Entity attacker = Main.LocalPlayer;
        public Terraria.NPC npcTarget;

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.SkeletonMerchant)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CalciteWand>());
            if (type == NPCID.Cyborg)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GlobalDischarge>());
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
            if (!RedeConfigClient.Instance.ElementDisable && !ItemTags.NoElement.Has(item.type))
            {
                #region Elemental Attributes
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (ItemTags.Fire.Has(item.type))
                        damage = (int)(damage * 1.15f);

                    if (ItemTags.Nature.Has(item.type))
                        damage = (int)(damage * 0.75f);

                    if (ItemTags.Poison.Has(item.type))
                        damage = (int)(damage * 0.5f);
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (ItemTags.Holy.Has(item.type))
                        damage = (int)(damage * 1.15f);

                    if (ItemTags.Shadow.Has(item.type))
                        damage = (int)(damage * 0.8f);
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (ItemTags.Holy.Has(item.type) || ItemTags.Celestial.Has(item.type))
                        damage = (int)(damage * 1.25f);

                    if (ItemTags.Fire.Has(item.type))
                        damage = (int)(damage * 0.5f);

                    if (ItemTags.Water.Has(item.type) || ItemTags.Ice.Has(item.type))
                        damage = (int)(damage * 1.1f);
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (ItemTags.Holy.Has(item.type) || ItemTags.Celestial.Has(item.type) || ItemTags.Arcane.Has(item.type))
                        damage = (int)(damage * 1.15f);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (ItemTags.Fire.Has(item.type))
                        damage = (int)(damage * 1.25f);

                    if (ItemTags.Ice.Has(item.type))
                        damage = (int)(damage * 0.75f);

                    if (ItemTags.Water.Has(item.type))
                        damage = (int)(damage * 0.5f);
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (ItemTags.Fire.Has(item.type))
                        damage = (int)(damage * 1.25f);

                    if (ItemTags.Ice.Has(item.type))
                        damage = (int)(damage * 0.75f);

                    if (ItemTags.Thunder.Has(item.type))
                        damage = (int)(damage * 1.1f);
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (ItemTags.Fire.Has(item.type))
                        damage = (int)(damage * 1.15f);

                    if (ItemTags.Ice.Has(item.type))
                        damage = (int)(damage * 0.7f);

                    if (ItemTags.Blood.Has(item.type))
                        damage = (int)(damage * 1.25f);

                    if (ItemTags.Poison.Has(item.type))
                        damage = (int)(damage * 0.1f);
                }
                if (((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet)) && ItemTags.Thunder.Has(item.type))
                    damage = (int)(damage * 1.1f);
                if (!npc.noTileCollide && npc.collideY && ItemTags.Earth.Has(item.type))
                    damage = (int)(damage * 1.1f);
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (ItemTags.Blood.Has(item.type) || ItemTags.Poison.Has(item.type))
                        damage = (int)(damage * 0.5f);

                    if (ItemTags.Thunder.Has(item.type))
                        damage = (int)(damage * 1.1f);

                    if (ItemTags.Water.Has(item.type))
                        damage = (int)(damage * 1.25f);
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (ItemTags.Blood.Has(item.type))
                        damage = (int)(damage * 1.1f);

                    if (ItemTags.Poison.Has(item.type))
                        damage = (int)(damage * 1.1f);
                }
                if (ItemTags.Poison.Has(item.type) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                    damage = (int)(damage * 1.15f);
                if (ItemTags.Wind.Has(item.type) && (npc.noGravity || !npc.collideY))
                    knockback = (int)((knockback * 1.1f) + 2);
                #endregion
            }

            // Decapitation
            if (npc.life < npc.lifeMax && item.CountsAsClass(DamageClass.Melee) && item.pick == 0 && item.hammer == 0 && !item.noUseGraphic && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && NPCLists.SkeletonHumanoid.Contains(npc.type))
            {
                if (Main.rand.NextBool(200) && !ItemTags.BluntSwing.Has(item.type))
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
            if (!RedeConfigClient.Instance.ElementDisable && !ProjectileTags.NoElement.Has(projectile.type))
            {
                #region Elemental Attributes
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (ProjectileTags.Fire.Has(projectile.type))
                        damage = (int)(damage * 1.15f);

                    if (ProjectileTags.Nature.Has(projectile.type))
                        damage = (int)(damage * 0.75f);

                    if (ProjectileTags.Poison.Has(projectile.type))
                        damage = (int)(damage * 0.5f);
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (ProjectileTags.Holy.Has(projectile.type))
                        damage = (int)(damage * 1.15f);

                    if (ProjectileTags.Shadow.Has(projectile.type))
                        damage = (int)(damage * 0.8f);
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (ProjectileTags.Holy.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type))
                        damage = (int)(damage * 1.25f);

                    if (ProjectileTags.Fire.Has(projectile.type))
                        damage = (int)(damage * 0.5f);

                    if (ProjectileTags.Water.Has(projectile.type) || ProjectileTags.Ice.Has(projectile.type))
                        damage = (int)(damage * 1.1f);
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (ProjectileTags.Holy.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Arcane.Has(projectile.type))
                        damage = (int)(damage * 1.15f);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (ProjectileTags.Fire.Has(projectile.type))
                        damage = (int)(damage * 1.25f);

                    if (ProjectileTags.Ice.Has(projectile.type))
                        damage = (int)(damage * 0.75f);

                    if (ProjectileTags.Water.Has(projectile.type))
                        damage = (int)(damage * 0.5f);
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (ProjectileTags.Fire.Has(projectile.type))
                        damage = (int)(damage * 1.25f);

                    if (ProjectileTags.Ice.Has(projectile.type))
                        damage = (int)(damage * 0.5f);

                    if (ProjectileTags.Thunder.Has(projectile.type))
                        damage = (int)(damage * 1.1f);
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (ProjectileTags.Fire.Has(projectile.type))
                        damage = (int)(damage * 1.15f);

                    if (ProjectileTags.Ice.Has(projectile.type))
                        damage = (int)(damage * 0.7f);

                    if (ProjectileTags.Blood.Has(projectile.type))
                        damage = (int)(damage * 1.25f);

                    if (ProjectileTags.Poison.Has(projectile.type))
                        damage = (int)(damage * 0.1f);
                }
                if (((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet)) && ProjectileTags.Thunder.Has(projectile.type))
                    damage = (int)(damage * 1.1f);
                if (!npc.noTileCollide && npc.collideY && ProjectileTags.Earth.Has(projectile.type))
                    damage = (int)(damage * 1.1f);
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (ProjectileTags.Blood.Has(projectile.type) || ProjectileTags.Poison.Has(projectile.type))
                        damage = (int)(damage * 0.5f);

                    if (ProjectileTags.Thunder.Has(projectile.type))
                        damage = (int)(damage * 1.1f);

                    if (ProjectileTags.Water.Has(projectile.type))
                        damage = (int)(damage * 1.25f);
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (ProjectileTags.Blood.Has(projectile.type))
                        damage = (int)(damage * 1.1f);

                    if (ProjectileTags.Poison.Has(projectile.type))
                        damage = (int)(damage * 1.1f);
                }
                if (ProjectileTags.Poison.Has(projectile.type) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                    damage = (int)(damage * 1.15f);
                if (ProjectileTags.Wind.Has(projectile.type) && (npc.noGravity || !npc.collideY))
                    knockback = (int)((knockback * 1.1f) + 2);
                #endregion
            }
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            target.Redemption().attacker = npc;
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, int damage, float knockback, bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemTags.NoElement.Has(item.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && ItemTags.Ice.Has(item.type))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && ItemTags.Ice.Has(item.type))
                    {
                        SoundEngine.PlaySound(SoundID.Item30, npc.position);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ItemTags.Fire.Has(item.type))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && ItemTags.Thunder.Has(item.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && ItemTags.Earth.Has(item.type))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ItemTags.Water.Has(item.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (ItemTags.Shadow.Has(item.type))
                {
                    if (Main.rand.NextBool(6) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetItemSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                #endregion
            }

            attacker = player;
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ProjectileTags.NoElement.Has(projectile.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && ProjectileTags.Ice.Has(projectile.type))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && ProjectileTags.Ice.Has(projectile.type))
                    {
                        SoundEngine.PlaySound(SoundID.Item30, npc.position);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ProjectileTags.Fire.Has(projectile.type))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && ProjectileTags.Thunder.Has(projectile.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && ProjectileTags.Earth.Has(projectile.type))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && ProjectileTags.Water.Has(projectile.type))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (ProjectileTags.Shadow.Has(projectile.type))
                {
                    if (Main.rand.NextBool(6) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetItemSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                #endregion
            }

            if (RedeDetours.projOwners.TryGetValue(projectile.whoAmI, out (Entity entity, IEntitySource source) value))
                attacker = value.entity;
            else if (npc.ClosestNPCToNPC(ref npc, 1000, npc.Center))
                attacker = npc;
        }
        public override void OnKill(Terraria.NPC npc)
        {
            if (NPCID.Sets.Skeletons[npc.type] && Main.rand.NextBool(3) && !npc.SpawnedFromStatue)
                RedeHelper.SpawnNPC(npc.GetSpawnSourceForNPCFromNPCAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
        public override void ModifyNPCLoot(Terraria.NPC npc, NPCLoot npcLoot)
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

            if (npc.type == NPCID.EaterofSouls || npc.type == NPCID.LittleEater || npc.type == NPCID.BigEater || npc.type == NPCID.CorruptGoldfish || npc.type == NPCID.DevourerHead || npc.type == NPCID.Corruptor || npc.type == NPCID.CorruptSlime || npc.type == NPCID.Slimeling || npc.type == NPCID.Slimer2 || npc.type == NPCID.BloodCrawler || npc.type == NPCID.CrimsonGoldfish || npc.type == NPCID.FaceMonster || npc.type == NPCID.Crimera || npc.type == NPCID.BigCrimera || npc.type == NPCID.LittleCrimera || npc.type == NPCID.Herpling || npc.type == NPCID.Crimslime || npc.type == NPCID.BigCrimslime || npc.type == NPCID.LittleCrimslime || npc.type == NPCID.BloodFeeder || npc.type == NPCID.BloodJelly)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EldritchRoot>(), 500));
            if (npc.type == NPCID.BoneSerpentHead)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmolderedScale>(), 20));
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
                spawnRate = 18;
                maxSpawns = 12;
            }
            if (player.InModBiome(ModContent.GetInstance<LabBiome>()))
            {
                spawnRate = 20;
                maxSpawns = 12;
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (RedeWorld.blobbleSwarm)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Blobble>(), 10);
            }
            if (RedeWorld.SkeletonInvasion && spawnInfo.Player.ZoneOverworldHeight)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<RaveyardSkeletonSpawner>(), 3);
                pool.Add(ModContent.NPCType<EpidotrianSkeleton>(), 5);
                pool.Add(ModContent.NPCType<CavernSkeletonSpawner>(), 5);
                pool.Add(ModContent.NPCType<SurfaceSkeletonSpawner>(), 2);
                pool.Add(ModContent.NPCType<CorpseWalkerPriest>(), 0.5f);
                pool.Add(ModContent.NPCType<JollyMadman>(), 0.02f);
            }
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<LabBiome>()))
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
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<WastelandPurityBiome>()))
            {
                int[] GrassTileArray = { ModContent.TileType<IrradiatedCorruptGrassTile>(), ModContent.TileType<IrradiatedCrimsonGrassTile>(), ModContent.TileType<IrradiatedGrassTile>() };
                bool tileCheck = GrassTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                pool.Clear();
                pool.Add(ModContent.NPCType<HazmatZombie>(), 1f);
                pool.Add(ModContent.NPCType<BobTheBlob>(), 0.05f);
                pool.Add(ModContent.NPCType<RadioactiveSlime>(), 0.9f);
                pool.Add(ModContent.NPCType<NuclearSlime>(), 0.3f);
                pool.Add(ModContent.NPCType<HazmatBunny>(), Main.dayTime ? 0.1f : 0);
                pool.Add(ModContent.NPCType<SickenedBunny>(), Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<SickenedDemonEye>(), !Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<NuclearShadow>(), 0.2f);
                pool.Add(ModContent.NPCType<MutatedLivingBloom>(), tileCheck ? (Main.raining ? 0.4f : 0.2f) : 0f);
                if (spawnInfo.Player.InModBiome(ModContent.GetInstance<WastelandSnowBiome>()))
                {
                    pool.Add(ModContent.NPCType<SneezyFlinx>(), 0.8f);
                    pool.Add(ModContent.NPCType<SicklyWolf>(), 0.7f);
                    pool.Add(ModContent.NPCType<SicklyPenguin>(), 0.6f);
                }
            }
            if (spawnInfo.Player.RedemptionScreen().cutscene)
                pool.Clear();
        }
    }
}
