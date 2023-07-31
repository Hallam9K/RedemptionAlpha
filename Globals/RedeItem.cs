using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Usable;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using System.Linq;
using Redemption.NPCs.Critters;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using ReLogic.Content;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Biomes;
using Redemption.NPCs.Friendly;
using Redemption.Tiles.Furniture.Misc;
using Redemption.WorldGeneration.Misc;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria.Localization;
using SubworldLibrary;

namespace Redemption.Globals
{
    public class RedeItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }
        public bool TechnicallyHammer;
        public bool TechnicallyAxe;

        public override void ModifyShootStats(Item item, Terraria.Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.RedemptionPlayerBuff().bowString && item.useAmmo == AmmoID.Arrow)
                velocity *= 1.2f;
        }
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            if ((item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane) && player.RedemptionPlayerBuff().heartInsignia)
                player.AddBuff(ModContent.BuffType<HeartInsigniaBuff>(), 180);

            return true;
        }
        public override void MeleeEffects(Item item, Terraria.Player player, Rectangle hitbox)
        {
            player.Redemption().meleeHitbox = hitbox;
            if (player.RedemptionPlayerBuff().eldritchRoot && item.HasElementItem(ElementID.Nature))
                item.GetGlobalItem<ElementalItem>().OverrideElement[ElementID.Shadow] = 1;

            if (item.DamageType == DamageClass.Melee && player.HasBuff<ExplosiveFlaskBuff>())
            {
                if (Main.rand.NextBool(3))
                    Dust.NewDust(item.position, item.width, item.height, DustID.Smoke);
                if (Main.rand.NextBool(10))
                    Dust.NewDust(item.position, item.width, item.height, DustID.InfernoFork);
                item.GetGlobalItem<ElementalItem>().OverrideElement[ElementID.Explosive] = 1;
            }
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            /*if (item.type == ItemID.JungleFishingCrate)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BuddingBoline>(), 6));
            if (item.type == ItemID.JungleFishingCrateHard)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BuddingBoline>(), 12));*/
            if (item.type == ItemID.GolemBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<GolemStaff>(), 7));
        }

        #region Vanilla Set Bonuses
        public static readonly string copperSet = "MoR:CopperSet";
        public static readonly string tinSet = "MoR:TinSet";
        public static readonly string cactusSet = "MoR:CactusSet";
        public static readonly string ironSet = "MoR:IronSet";
        public static readonly string leadSet = "MoR:LeadSet";
        public static readonly string silverSet = "MoR:SilverSet";
        public static readonly string tungstenSet = "MoR:TungstenSet";
        public static readonly string goldSet = "MoR:GoldSet";
        public static readonly string platinumSet = "MoR:PlatinumSet";
        public static readonly string fossilSet = "MoR:FossilSet";
        public static readonly string jungleSet = "MoR:JungleSet";
        public static readonly string shadowSet = "MoR:ShadowSet";
        public static readonly string crimsonSet = "MoR:CrimsonSet";
        public static readonly string moltenSet = "MoR:MoltenSet";
        public static readonly string cobaltSet = "MoR:CobaltSet";
        public static readonly string palladiumSet = "MoR:PalladiumSet";
        public static readonly string mythrilSet = "MoR:MythrilSet";
        public static readonly string orichalcumSet = "MoR:OrichalcumSet";
        public static readonly string adamantiteSet = "MoR:AdamantiteSet";
        public static readonly string titaniumSet = "MoR:TitaniumSet";
        public static readonly string frostSet = "MoR:FrostSet";
        public static readonly string forbiddenSet = "MoR:ForbiddenSet";
        public static readonly string hallowedSet = "MoR:HallowedSet";
        public static readonly string turtleSet = "MoR:TurtleSet";
        public static readonly string beetleSet = "MoR:BeetleSet";
        public static readonly string spectreSet = "MoR:SpectreSet";
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.CopperHelmet && body.type == ItemID.CopperChainmail && legs.type == ItemID.CopperGreaves)
                return copperSet;
            if (head.type == ItemID.TinHelmet && body.type == ItemID.TinChainmail && legs.type == ItemID.TinGreaves)
                return tinSet;
            if (head.type == ItemID.CactusHelmet && body.type == ItemID.CactusBreastplate && legs.type == ItemID.CactusLeggings)
                return cactusSet;
            if ((head.type == ItemID.IronHelmet || head.type == ItemID.AncientIronHelmet) && body.type == ItemID.IronChainmail && legs.type == ItemID.IronGreaves)
                return ironSet;
            if (head.type == ItemID.LeadHelmet && body.type == ItemID.LeadChainmail && legs.type == ItemID.LeadGreaves)
                return leadSet;
            if (head.type == ItemID.SilverHelmet && body.type == ItemID.SilverChainmail && legs.type == ItemID.SilverGreaves)
                return silverSet;
            if (head.type == ItemID.TungstenHelmet && body.type == ItemID.TungstenChainmail && legs.type == ItemID.TungstenGreaves)
                return tungstenSet;
            if ((head.type == ItemID.GoldHelmet || head.type == ItemID.AncientGoldHelmet) && body.type == ItemID.GoldChainmail && legs.type == ItemID.GoldGreaves)
                return goldSet;
            if (head.type == ItemID.PlatinumHelmet && body.type == ItemID.PlatinumChainmail && legs.type == ItemID.PlatinumGreaves)
                return platinumSet;
            if (head.type == ItemID.FossilHelm && body.type == ItemID.FossilShirt && legs.type == ItemID.FossilPants)
                return fossilSet;
            if (head.type == ItemID.JungleHat && body.type == ItemID.JungleShirt && legs.type == ItemID.JunglePants)
                return jungleSet;
            if ((head.type == ItemID.ShadowHelmet && body.type == ItemID.ShadowScalemail && legs.type == ItemID.ShadowGreaves) &&
                (head.type == ItemID.AncientShadowHelmet && body.type == ItemID.AncientShadowScalemail && legs.type == ItemID.AncientShadowGreaves))
                return shadowSet;
            if (head.type == ItemID.CrimsonHelmet && body.type == ItemID.CrimsonScalemail && legs.type == ItemID.CrimsonGreaves)
                return crimsonSet;
            if (head.type == ItemID.MoltenHelmet && body.type == ItemID.MoltenBreastplate && legs.type == ItemID.MoltenGreaves)
                return moltenSet;
            if ((head.type == ItemID.CobaltHelmet || head.type == ItemID.CobaltMask || head.type == ItemID.CobaltHat) && body.type == ItemID.CobaltBreastplate && legs.type == ItemID.CobaltLeggings)
                return cobaltSet;
            if ((head.type == ItemID.PalladiumHelmet || head.type == ItemID.PalladiumHeadgear || head.type == ItemID.PalladiumMask) && body.type == ItemID.PalladiumBreastplate && legs.type == ItemID.PalladiumLeggings)
                return palladiumSet;
            if ((head.type == ItemID.MythrilHelmet || head.type == ItemID.MythrilHat || head.type == ItemID.MythrilHood) && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
                return mythrilSet;
            if ((head.type == ItemID.OrichalcumHelmet || head.type == ItemID.OrichalcumHeadgear || head.type == ItemID.OrichalcumMask) && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
                return orichalcumSet;
            if ((head.type == ItemID.AdamantiteHelmet || head.type == ItemID.AdamantiteHeadgear || head.type == ItemID.AdamantiteMask) && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
                return adamantiteSet;
            if ((head.type == ItemID.TitaniumHelmet || head.type == ItemID.TitaniumHeadgear || head.type == ItemID.TitaniumMask) && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings)
                return titaniumSet;
            if (head.type == ItemID.FrostHelmet && body.type == ItemID.FrostBreastplate && legs.type == ItemID.FrostLeggings)
                return frostSet;
            if (head.type == ItemID.AncientBattleArmorHat && body.type == ItemID.AncientBattleArmorShirt && legs.type == ItemID.AncientBattleArmorPants)
                return forbiddenSet;
            if (((head.type == ItemID.HallowedHeadgear || head.type == ItemID.HallowedHelmet || head.type == ItemID.HallowedHood || head.type == ItemID.HallowedMask) && body.type == ItemID.HallowedPlateMail && legs.type == ItemID.HallowedGreaves) || ((head.type == ItemID.AncientHallowedHeadgear || head.type == ItemID.AncientHallowedHelmet || head.type == ItemID.AncientHallowedHood || head.type == ItemID.AncientHallowedMask) && body.type == ItemID.AncientHallowedPlateMail && legs.type == ItemID.AncientHallowedGreaves))
                return hallowedSet;
            if (head.type == ItemID.TurtleHelmet && body.type == ItemID.TurtleScaleMail && legs.type == ItemID.TurtleLeggings)
                return turtleSet;
            if (head.type == ItemID.BeetleHelmet && body.type == ItemID.BeetleScaleMail && legs.type == ItemID.BeetleLeggings)
                return beetleSet;
            if ((head.type == ItemID.SpectreHood || head.type == ItemID.SpectreMask) && body.type == ItemID.SpectreRobe && legs.type == ItemID.SpectrePants)
                return spectreSet;

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Terraria.Player player, string set)
        {
            if (set == copperSet || set == tinSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.ThunderS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalResistance[ElementID.Thunder] += 0.2f;
            }
            if (set == silverSet || set == tungstenSet || set == titaniumSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.ThunderS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Thunder] += 0.2f;
            }
            if (set == cactusSet || set == jungleSet || set == orichalcumSet || set == beetleSet || set == turtleSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.NatureS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalResistance[ElementID.Nature] += 0.2f;
            }
            if (set == goldSet || set == mythrilSet || set == spectreSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.ArcaneS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalResistance[ElementID.Arcane] += 0.2f;
            }
            if (set == platinumSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.ArcaneS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Arcane] += 0.2f;
            }
            if (set == fossilSet || set == adamantiteSet || set == forbiddenSet || set == turtleSet || set == beetleSet || set == ironSet || set == leadSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.EarthS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Earth] += 0.2f;
            }
            if (set == shadowSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.ShadowS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Shadow] += 0.2f;
            }
            if (set == crimsonSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.BloodS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Blood] += 0.2f;
            }
            if (set == moltenSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.FireS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Fire] += 0.2f;
            }
            if (set == cobaltSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.WaterS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Water] += 0.2f;
            }
            if (set == palladiumSet || set == hallowedSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.HolyS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Holy] += 0.2f;
            }
            if (set == frostSet)
            {
                player.setBonus += Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.20Increased") + ElementID.IceS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance");
                player.RedemptionPlayerBuff().ElementalDamage[ElementID.Ice] += 0.2f;
            }
        }
        #endregion

        public override void PostUpdate(Item item)
        {
            if ((item.type is ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane) && Main.LocalPlayer.RedemptionPlayerBuff().heartInsignia)
            {
                if (!Main.rand.NextBool(6))
                    return;

                int sparkle = Dust.NewDust(new Vector2(item.position.X, item.position.Y), item.width, item.height,
                    DustID.ShadowbeamStaff, Scale: 2);
                Main.dust[sparkle].velocity.X = 0;
                Main.dust[sparkle].velocity.Y = -2;
                Main.dust[sparkle].noGravity = true;
            }
            if (item.type is ItemID.GoldCrown or ItemID.PlatinumCrown)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Terraria.NPC chicken = Main.npc[i];
                    if (!chicken.active || chicken.type != ModContent.NPCType<Chicken>())
                        continue;

                    if (chicken.frame.Y != 488 && chicken.frame.Y != 532)
                        continue;

                    if (!item.Hitbox.Intersects(chicken.Hitbox))
                        continue;

                    SoundEngine.PlaySound(SoundID.Item68, item.position);
                    SoundEngine.PlaySound(CustomSounds.Choir with { Pitch = 0.1f }, item.position);
                    RedeDraw.SpawnExplosion(item.Center, Color.White, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3", AssetRequestMode.ImmediateLoad).Value);
                    chicken.active = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Item.NewItem(item.GetSource_Loot(), item.getRect(), ModContent.ItemType<CrownOfTheKing>(), item.stack);
                        item.active = false;
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                    }
                }
            }
        }

        readonly int[] bannedArenaItems = new int[]
        {
            ItemID.IceRod,
            ItemID.PortalGun,
            ItemID.MagicMirror,
            ItemID.IceMirror,
            ItemID.CellPhone,
            ItemID.StaticHook,
            ItemID.AntiGravityHook,
            ItemID.Sandgun,
            ItemID.ActuationRod,
            ItemID.GravitationPotion
        };
        public override bool? UseItem(Item item, Terraria.Player player)
        {
            if (item.type is ItemID.RodOfHarmony && RedeHelper.BossActive(true))
            {
                if (player.HasBuff(BuffID.ChaosState))
                {
                    player.statLife -= player.statLifeMax2 / 6;
                    if (player.statLife <= 0)
                        player.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("DeathText.Teleport_1", player.name)), 1, 1);
                }
                player.AddBuff(BuffID.ChaosState, 360);
            }
            return base.UseItem(item, player);
        }
        public override bool CanUseItem(Item item, Terraria.Player player)
        {
            if (ArenaWorld.arenaActive && bannedArenaItems.Any(x => x == item.type))
                return false;
            if (player.InModBiome<LabBiome>() && !RedeBossDowned.downedPZ && (item.type is ItemID.RodofDiscord or ItemID.RodOfHarmony))
                return false;

            #region C
            Point coop = player.Center.ToTileCoordinates();
            if (item.type is ItemID.TeleportationPotion && player.RedemptionPlayerBuff().ChickenForm && Framing.GetTileSafely(coop.X, coop.Y).TileType == ModContent.TileType<ChickenCoopTile>())
            {
                if (!SubworldSystem.AnyActive<Redemption>())
                {
                    SubworldSystem.Enter<CSub>();
                    return false;
                }
            }
            #endregion
            return base.CanUseItem(item, player);
        }
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>() && !Terraria.NPC.AnyNPCs(ModContent.NPCType<Chalice_Intro>()))
            {
                RedeWorld.alignmentGiven = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);

                RedeHelper.SpawnNPC(item.GetSource_FromAI(), (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<Chalice_Intro>());
                item.TurnToAir();
            }
        }
        public static bool ChaliceInterest(int type)
        {
            if (ItemLists.AlignmentInterest.Contains(type))
            {
                if (type == ModContent.ItemType<WeddingRing>() && (!RedeBossDowned.downedKeeper || RedeBossDowned.skullDiggerSaved))
                    return false;
                if (type == ModContent.ItemType<SorrowfulEssence>() && RedeBossDowned.downedSkullDigger)
                    return false;
                if (type == ModContent.ItemType<AbandonedTeddy>() && RedeBossDowned.keeperSaved)
                    return false;
                if (type == ModContent.ItemType<CyberTech>() && RedeBossDowned.downedSlayer)
                    return false;
                if (type == ModContent.ItemType<SlayerShipEngine>() && RedeWorld.slayerRep >= 4)
                    return false;
                if (type == ModContent.ItemType<AnglonicMysticBlossom>() && (RedeWorld.alignment <= 0 || RedeQuest.forestNymphVar >= 2))
                    return false;
                if (type == ModContent.ItemType<KingsOakStaff>() && (RedeWorld.alignment <= 0 || RedeQuest.forestNymphVar > 0))
                    return false;
                if (type == ModContent.ItemType<NebSummon>() && RedeBossDowned.downedNebuleus && RedeBossDowned.nebDeath < 7)
                    return false;
                return true;
            }
            return false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type is ItemID.RodOfHarmony)
            {
                TooltipLine tooltip1Line = new(Mod, "Tooltip1", Language.GetTextValue("Mods.Redemption.GenericTooltips.RodOfHarmonyLine"));
                int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
                if (tooltipLocation != -1)
                    tooltips.Insert(tooltipLocation + 1, tooltip1Line);
            }
            if (ChaliceInterest(item.type) && RedeWorld.alignmentGiven)
            {
                TooltipLine chaliceLine = new(Mod, "ChaliceLine", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ChaliceLine")) { OverrideColor = new Color(203, 189, 99) };
                tooltips.Add(chaliceLine);
            }
            TooltipLine axeLine = new(Mod, "AxeBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.AxeBonus")) { OverrideColor = Colors.RarityOrange };
            if ((item.CountsAsClass(DamageClass.Melee) && item.damage > 0 && item.useStyle == ItemUseStyleID.Swing && !item.noUseGraphic))
            {
                if (item.axe > 0)
                    tooltips.Add(axeLine);

                else if (!ItemLists.BluntSwing.Contains(item.type) && item.hammer == 0 && item.pick == 0)
                {
                    TooltipLine slashLine = new(Mod, "SlashBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(slashLine);
                }
            }
            if (TechnicallyAxe)
                tooltips.Add(axeLine);

            if (item.hammer > 0 || item.type == ItemID.PaladinsHammer || TechnicallyHammer)
            {
                TooltipLine hammerLine = new(Mod, "HammerBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.HammerBonus")) { OverrideColor = Colors.RarityOrange };
                tooltips.Add(hammerLine);
            }
            if (!RedeConfigClient.Instance.ElementDisable && item.HasElementItem(ElementID.Explosive))
            {
                TooltipLine explodeLine = new(Mod, "ExplodeBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ExplodeBonus")) { OverrideColor = Colors.RarityOrange };
                tooltips.Add(explodeLine);
            }

            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type) && !ProjectileLists.NoElement.Contains(item.shoot))
            {
                if (item.HasElementItem(ElementID.Arcane))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ArcaneBonus")) { OverrideColor = Color.LightBlue };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Blood))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.BloodBonus")) { OverrideColor = Color.IndianRed };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Celestial))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.CelestialBonus")) { OverrideColor = Color.Pink };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Earth))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.EarthBonus")) { OverrideColor = Color.SandyBrown };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Fire))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.FireBonus")) { OverrideColor = Color.Orange };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Holy))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.HolyBonus")) { OverrideColor = Color.LightGoldenrodYellow };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Ice))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.IceBonus")) { OverrideColor = Color.LightCyan };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Nature))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.NatureBonus")) { OverrideColor = Color.LawnGreen };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Poison))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.PoisonBonus")) { OverrideColor = Color.MediumPurple };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Psychic))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.PsychicBonus")) { OverrideColor = Color.LightPink };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Shadow))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ShadowBonus")) { OverrideColor = Color.MediumSlateBlue };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Thunder))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ThunderBonus")) { OverrideColor = Color.LightYellow };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Water))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.WaterBonus")) { OverrideColor = Color.SkyBlue };
                    tooltips.Add(line);
                }
                if (item.HasElementItem(ElementID.Wind))
                {
                    TooltipLine line = new(Mod, "Element", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.WindBonus")) { OverrideColor = Color.LightGray };
                    tooltips.Add(line);
                }
            }
            if (item.rare == ModContent.RarityType<DonatorRarity>())
            {
                TooltipLine donatorLine = new(Mod, "DonatorLine", "-Donator Item-") { OverrideColor = Color.SpringGreen };
                tooltips.Add(donatorLine);
            }
        }
    }
}
