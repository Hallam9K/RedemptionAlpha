using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Usable;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using System.Linq;
using Redemption.NPCs.Critters;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using ReLogic.Content;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals.Player;

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
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage,
            ref float knockBack, ref bool crit)
        {
            if ((item.axe > 0 || TechnicallyAxe) && crit)
                damage += damage / 2;
        }
        public override void ModifyWeaponCrit(Item item, Terraria.Player player, ref float crit)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (modPlayer.powerCell)
                {
                    if (ItemLists.Fire.Contains(item.type) || ProjectileLists.Fire.Contains(item.shoot))
                        crit += 6;
                    if (ItemLists.Holy.Contains(item.type) || ProjectileLists.Holy.Contains(item.shoot))
                        crit += 6;
                }
                if (modPlayer.sacredCross && (ItemLists.Holy.Contains(item.type) || ProjectileLists.Holy.Contains(item.shoot)))
                    crit += 6;
            }
        }
        public override void ModifyWeaponDamage(Item item, Terraria.Player player, ref StatModifier damage)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (ItemLists.Arcane.Contains(item.type) || ProjectileLists.Arcane.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[0];
                if (ItemLists.Fire.Contains(item.type) || ProjectileLists.Fire.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[1];
                if (ItemLists.Water.Contains(item.type) || ProjectileLists.Water.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[2];
                if (ItemLists.Ice.Contains(item.type) || ProjectileLists.Ice.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[3];
                if (ItemLists.Earth.Contains(item.type) || ProjectileLists.Earth.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[4];
                if (ItemLists.Wind.Contains(item.type) || ProjectileLists.Wind.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[5];
                if (ItemLists.Thunder.Contains(item.type) || ProjectileLists.Thunder.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[6];
                if (ItemLists.Holy.Contains(item.type) || ProjectileLists.Holy.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[7];
                if (ItemLists.Shadow.Contains(item.type) || ProjectileLists.Shadow.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[8];
                if (ItemLists.Nature.Contains(item.type) || ProjectileLists.Nature.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[9];
                if (ItemLists.Poison.Contains(item.type) || ProjectileLists.Poison.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[10];
                if (ItemLists.Blood.Contains(item.type) || ProjectileLists.Blood.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[11];
                if (ItemLists.Psychic.Contains(item.type) || ProjectileLists.Psychic.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[12];
                if (ItemLists.Celestial.Contains(item.type) || ProjectileLists.Celestial.Contains(item.shoot))
                    damage += modPlayer.ElementalDamage[13];
                #endregion
            }
        }
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            if (item.type == ItemID.Heart && player.RedemptionPlayerBuff().heartInsignia)
                player.AddBuff(ModContent.BuffType<HeartInsigniaBuff>(), 180);

            return true;
        }
        public override void MeleeEffects(Item item, Terraria.Player player, Rectangle hitbox)
        {
            player.Redemption().meleeHitbox = hitbox;
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.JungleFishingCrate)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BuddingBoline>(), 6));
            if (item.type == ItemID.JungleFishingCrateHard)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BuddingBoline>(), 12));
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
                player.setBonus += "\n20% increased Thunder elemental resistance";
                player.RedemptionPlayerBuff().ElementalResistance[6] += 0.2f;
            }
            if (set == silverSet || set == tungstenSet || set == titaniumSet)
            {
                player.setBonus += "\n20% increased Thunder elemental damage";
                player.RedemptionPlayerBuff().ElementalDamage[6] += 0.2f;
            }
            if (set == cactusSet || set == jungleSet || set == orichalcumSet || set == beetleSet || set == turtleSet)
            {
                player.setBonus += "\n20% increased Nature elemental resistance";
                player.RedemptionPlayerBuff().ElementalResistance[9] += 0.2f;
            }
            if (set == goldSet || set == mythrilSet || set == spectreSet)
            {
                player.setBonus += "\n20% increased Arcane elemental resistance";
                player.RedemptionPlayerBuff().ElementalResistance[0] += 0.2f;
            }
            if (set == platinumSet)
            {
                player.setBonus += "\n20% increased Arcane elemental damage";
                player.RedemptionPlayerBuff().ElementalDamage[0] += 0.2f;
            }
            if (set == fossilSet || set == adamantiteSet || set == forbiddenSet || set == turtleSet || set == beetleSet || set == ironSet || set == leadSet)
            {
                player.setBonus += "\n20% increased Earth elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[4] += 0.2f;
            }
            if (set == shadowSet)
            {
                player.setBonus += "\n20% increased Shadow elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[8] += 0.2f;
            }
            if (set == crimsonSet)
            {
                player.setBonus += "\n20% increased Blood elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[11] += 0.2f;
            }
            if (set == moltenSet)
            {
                player.setBonus += "\n20% increased Fire elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[1] += 0.2f;
            }
            if (set == cobaltSet)
            {
                player.setBonus += "\n20% increased Water elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[2] += 0.2f;
            }
            if (set == palladiumSet || set == hallowedSet)
            {
                player.setBonus += "\n20% increased Holy elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[7] += 0.2f;
            }
            if (set == frostSet)
            {
                player.setBonus += "\n20% increased Ice elemental resistance";
                player.RedemptionPlayerBuff().ElementalDamage[3] += 0.2f;
            }
        }
        #endregion

        public override void PostUpdate(Item item)
        {
            if (item.type == ItemID.Heart && Main.LocalPlayer.RedemptionPlayerBuff().heartInsignia)
            {
                if (!Main.rand.NextBool(6))
                    return;

                int sparkle = Dust.NewDust(new Vector2(item.position.X, item.position.Y), item.width, item.height,
                    DustID.ShadowbeamStaff, Scale: 2);
                Main.dust[sparkle].velocity.X = 0;
                Main.dust[sparkle].velocity.Y = -2;
                Main.dust[sparkle].noGravity = true;
            }
            if (item.type == ItemID.GoldCrown || item.type == ItemID.PlatinumCrown)
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
            ItemID.RodofDiscord,
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
        public override bool CanUseItem(Item item, Terraria.Player player)
        {
            if (ArenaWorld.arenaActive && bannedArenaItems.Any(x => x == item.type))
                return false;

            return base.CanUseItem(item, player);
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>())
                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Greetings, I am the Chalice of Alignment, and I believe any action can be redeemed.", 260, 30, 0, Color.DarkGoldenrod);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine axeLine = new(Mod, "AxeBonus", "Axe Bonus: 3x critical strike damage, increased chance to decapitate skeletons") { OverrideColor = Colors.RarityOrange };
            if ((item.CountsAsClass(DamageClass.Melee) && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && !item.noUseGraphic))
            {
                if (item.axe > 0)
                    tooltips.Add(axeLine);

                else if (!ItemLists.BluntSwing.Contains(item.type) && item.hammer == 0 && item.pick == 0)
                {
                    TooltipLine slashLine = new(Mod, "SlashBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(slashLine);
                }
            }
            if (TechnicallyAxe)
                tooltips.Add(axeLine);

            if (item.hammer > 0 || item.type == ItemID.PaladinsHammer || TechnicallyHammer)
            {
                TooltipLine hammerLine = new(Mod, "HammerBonus", "Hammer Bonus: Deals quadruple damage to Guard Points") { OverrideColor = Colors.RarityOrange };
                tooltips.Add(hammerLine);
            }
            if (!RedeConfigClient.Instance.ElementDisable && ProjectileLists.Explosive.Contains(item.shoot))
            {
                TooltipLine explodeLine = new(Mod, "ExplodeBonus", "Explosive Bonus: Deals quadruple damage to Guard Points") { OverrideColor = Colors.RarityOrange };
                tooltips.Add(explodeLine);
            }

            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type) && !ProjectileLists.NoElement.Contains(item.shoot))
            {
                if (ItemLists.Arcane.Contains(item.type) || ProjectileLists.Arcane.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Arcane Bonus: Can hit enemies from the spirit realm") { OverrideColor = Color.LightBlue };
                    tooltips.Add(line);
                }
                if (ItemLists.Blood.Contains(item.type) || ProjectileLists.Blood.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Blood Bonus: Increased damage to organic enemies, but decreased to robotic") { OverrideColor = Color.IndianRed };
                    tooltips.Add(line);
                }
                if (ItemLists.Celestial.Contains(item.type) || ProjectileLists.Celestial.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Celestial Bonus: Hitting foes can create stars around them, restoring life and mana once the foe is slain") { OverrideColor = Color.Pink };
                    tooltips.Add(line);
                }
                if (ItemLists.Earth.Contains(item.type) || ProjectileLists.Earth.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Earth Bonus: Deals extra damage and has a chance to stun grounded enemies") { OverrideColor = Color.SandyBrown };
                    tooltips.Add(line);
                }
                if (ItemLists.Fire.Contains(item.type) || ProjectileLists.Fire.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Fire Bonus: Chance to inflict a strong 'On Fire!' debuff on flammable enemies") { OverrideColor = Color.Orange };
                    tooltips.Add(line);
                }
                if (ItemLists.Holy.Contains(item.type) || ProjectileLists.Holy.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Holy Bonus: Increased damage to undead and demons") { OverrideColor = Color.LightGoldenrodYellow };
                    tooltips.Add(line);
                }
                if (ItemLists.Ice.Contains(item.type) || ProjectileLists.Ice.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Ice Bonus: Chance to freeze slimes and slow down infected enemies") { OverrideColor = Color.LightCyan };
                    tooltips.Add(line);
                }
                if (ItemLists.Nature.Contains(item.type) || ProjectileLists.Nature.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Nature Bonus: Has a chance to drop a defence pickup upon hitting enemies inflicted with a non-fire debuff") { OverrideColor = Color.LawnGreen };
                    tooltips.Add(line);
                }
                if (ItemLists.Poison.Contains(item.type) || ProjectileLists.Poison.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Poison Bonus: Increased damage to poisoned enemies") { OverrideColor = Color.MediumPurple };
                    tooltips.Add(line);
                }
                if (ItemLists.Psychic.Contains(item.type) || ProjectileLists.Psychic.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Psychic Bonus: Ignores enemy Guard Points") { OverrideColor = Color.LightPink };
                    tooltips.Add(line);
                }
                if (ItemLists.Shadow.Contains(item.type) || ProjectileLists.Shadow.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Shadow Bonus: Slain enemies have a chance to drop a pickup which increases Shadow damage") { OverrideColor = Color.MediumSlateBlue };
                    tooltips.Add(line);
                }
                if (ItemLists.Thunder.Contains(item.type) || ProjectileLists.Thunder.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Thunder Bonus: Electrifies and deals extra damage if the target is in water") { OverrideColor = Color.LightYellow };
                    tooltips.Add(line);
                }
                if (ItemLists.Water.Contains(item.type) || ProjectileLists.Water.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Water Bonus: Increased damage to demons and can electrify robotic targets") { OverrideColor = Color.SkyBlue };
                    tooltips.Add(line);
                }
                if (ItemLists.Wind.Contains(item.type) || ProjectileLists.Wind.Contains(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Wind Bonus: Deals extra knockback to airborne targets") { OverrideColor = Color.LightGray };
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