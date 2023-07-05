using Redemption.Biomes;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using ReLogic.Content;
using Redemption.WorldGeneration;
using SubworldLibrary;
using Redemption.WorldGeneration.Misc;
using Redemption.Items.Weapons.HM.Magic;
using Terraria.Audio;
using Terraria.Localization;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Furniture.Misc;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Globals.Player
{
    public class RedePlayer : ModPlayer
    {
        public int spiritLevel = 0;
        public int maxSpiritLevel = 3;
        public bool foundHall;
        public bool foundLab;
        public bool omegaGiftGiven;
        public int hitTarget = -1;
        public int hitTarget2 = -1;
        public bool medKit;
        public bool galaxyHeart;
        public int heartStyle;
        public bool stalkerSilence;
        public float musicVolume;
        public int slayerStarRating;
        public bool contactImmune;
        public bool contactImmuneTrue;
        public bool slayerCursor;
        public Rectangle meleeHitbox;
        public int crystalGlaiveLevel;
        public int crystalGlaiveShotCount;
        public bool parryStance;
        public bool parried;
        public bool yesChoice;
        public bool noChoice;
        public bool onHit;

        public override void ResetEffects()
        {
            if (contactImmune)
                contactImmuneTrue = true;
            else
                contactImmuneTrue = false;
            meleeHitbox = Rectangle.Empty;
            slayerCursor = false;
            contactImmune = false;
            parried = false;
            onHit = false;
        }
        public override void Initialize()
        {
            foundHall = false;
            foundLab = false;
            omegaGiftGiven = false;
            medKit = false;
            galaxyHeart = false;
        }
        public override void UpdateDead()
        {
            Player.fullRotation = 0f;
            slayerStarRating = 0;
            parryStance = false;
            parried = false;
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if ((damageSource.SourceNPCIndex >= 0 || (damageSource.SourceProjectileLocalIndex >= 0 && Main.projectile[damageSource.SourceProjectileLocalIndex].Redemption().TechnicallyMelee)) && contactImmuneTrue)
                return true;
            if (((damageSource.SourceNPCIndex >= 0 && Main.npc[damageSource.SourceNPCIndex].velocity.Length() > Player.velocity.Length() / 2) ||
                (damageSource.SourceProjectileLocalIndex >= 0 && Main.projectile[damageSource.SourceProjectileLocalIndex].Redemption().TechnicallyMelee)) && parryStance)
            {
                parried = true;
                Player.immune = true;
                Player.immuneTime = (int)MathHelper.Max(Player.immuneTime, 4);
                return true;
            }
            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
        public override void OnHurt(Terraria.Player.HurtInfo info)
        {
            onHit = true;
        }
        public override void OnHitNPCWithItem(Item item, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (Player.RedemptionPlayerBuff().hardlightBonus == 3 && item.DamageType == DamageClass.Melee)
            {
                hitTarget = target.whoAmI;
                hitTarget2 = target.whoAmI;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (Player.RedemptionPlayerBuff().hardlightBonus == 3 && proj.DamageType == DamageClass.Melee && proj.type != ModContent.ProjectileType<MiniSpaceship_Laser>())
            {
                hitTarget = target.whoAmI;
                hitTarget2 = target.whoAmI;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (Player.HasItem(ModContent.ItemType<Taikasauva>()))
                SoundEngine.PlaySound(CustomSounds.NoitaDeath);
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
        public override void OnEnterWorld()
        {
            if (SubworldSystem.Current != null)
                return;
            if (RedeGen.GoldenGatewayVector.X == -1 || RedeGen.BastionVector.X == -1)
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Warning1"), Colors.RarityRed);
            if (!LabSearch() || !AnglonPortalSearch() || !GathPortalSearch() || !HallOfHeroesSearch() || !ShipSearch())
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Warning2"), Colors.RarityRed);

            if (RedeConfigClient.Instance.FunniAllWasteland || RedeConfigClient.Instance.FunniJanitor || RedeConfigClient.Instance.FunniSpiders || RedeConfigClient.Instance.FunniWasteland)
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Caution"), Colors.RarityOrange);
        }
        #region Structure Search
        public static bool LabSearch()
        {
            if (RedeGen.LabVector.X == -1)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    for (int y = 20; y < Main.maxTilesY - 20; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<KariBedTile>())
                            continue;
                        RedeGen.LabVector = new Vector2(x - 142, y - 209);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool GathPortalSearch()
        {
            if (RedeGen.gathicPortalVector.X == -1)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    for (int y = 20; y < Main.maxTilesY - 20; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<GathuramPortalTile>())
                            continue;
                        RedeGen.gathicPortalVector = new Vector2(x - 45, y - 11);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool HallOfHeroesSearch()
        {
            if (RedeGen.HallOfHeroesVector.X == -1)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    for (int y = 20; y < Main.maxTilesY - 20; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<AncientAltarTile>())
                            continue;
                        RedeGen.HallOfHeroesVector = new Vector2(x - 39, y - 25);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool AnglonPortalSearch()
        {
            if (RedeGen.newbCaveVector.X == -1)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    for (int y = 20; y < Main.maxTilesY - 20; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<AnglonPortalTile>())
                            continue;
                        RedeGen.newbCaveVector = new Vector2(x - 29, y);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ShipSearch()
        {
            if (RedeGen.slayerShipVector.X == -1)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    for (int y = 20; y < Main.maxTilesY - 20; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<SlayerChairTile>())
                            continue;
                        RedeGen.slayerShipVector = new Vector2(x - 90, y - 23);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        #endregion
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = (medKit ? 50 : 0) + (galaxyHeart ? 50 : 0);
            // Alternatively:  health = StatModifier.Default with { Base = exampleLifeFruits * ExampleLifeFruit.LifePerFruit };
            mana = StatModifier.Default;
        }
        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> emptyTex = ModContent.Request<Texture2D>("Redemption/Empty");
                Asset<Texture2D> cursor0 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_0");
                Asset<Texture2D> cursor1 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_1");
                Asset<Texture2D> cursor11 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_11");
                Asset<Texture2D> cursor12 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_12");
                if (slayerCursor)
                {
                    TextureAssets.Cursors[0] = emptyTex;
                    TextureAssets.Cursors[1] = emptyTex;
                    TextureAssets.Cursors[11] = emptyTex;
                    TextureAssets.Cursors[12] = emptyTex;
                }
                else
                {
                    TextureAssets.Cursors[0] = cursor0;
                    TextureAssets.Cursors[1] = cursor1;
                    TextureAssets.Cursors[11] = cursor11;
                    TextureAssets.Cursors[12] = cursor12;
                }
            }
            if (Player.InModBiome<LabBiome>())
            {
                Player.buffImmune[BuffID.Shimmer] = true;
            }
            if (SubworldSystem.IsActive<CSub>())
            {
                Player.noBuilding = true;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.RedemptionScreen().lockScreen = true;
                Player.RedemptionScreen().ScreenFocusPosition = new Vector2(100, 99) * 16;
                Player.RedemptionScreen().interpolantTimer = 100;
            }
        }
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Main.rand.Next(100) < (10 + (Player.cratePotion ? 10 : 0)))
            {
                if (Player.InModBiome<LabBiome>() && Terraria.NPC.downedMechBoss1 && Terraria.NPC.downedMechBoss2 && Terraria.NPC.downedMechBoss3)
                {
                    if (Terraria.NPC.downedMoonlord)
                        itemDrop = ModContent.ItemType<LabCrate2>();
                    else
                        itemDrop = ModContent.ItemType<LabCrate>();
                }
                if (Player.InModBiome<WastelandPurityBiome>())
                    itemDrop = ModContent.ItemType<PetrifiedCrate>();
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (!mediumCoreDeath && (Player.name.Contains("Liz") || Player.name.Contains("Lizzy") || Player.name.Contains("Elizabeth")))
            {
                return new[] {
                    new Item(ModContent.ItemType<LizzyCookie>())
                };
            }
            if (!mediumCoreDeath && (Player.name == "Uncon" || Player.name == "Dahlia"))
            {
                return new[] {
                    new Item(ModContent.ItemType<UnconHead>()),
                    new Item(ModContent.ItemType<UnconBody>()),
                    new Item(ModContent.ItemType<UnconLegs>()),
                    new Item(ModContent.ItemType<UnconPatreon_CapeAcc>()),
                    new Item(ModContent.ItemType<UnconPetItem>())
                };
            }
            return base.AddStartingItems(mediumCoreDeath);
        }

        public override void SaveData(TagCompound tag)
        {
            var saveS = new List<string>();
            if (foundHall) saveS.Add("foundHall");
            if (foundLab) saveS.Add("foundLab");
            if (omegaGiftGiven) saveS.Add("omegaGiftGiven");
            if (medKit) saveS.Add("medKit");
            if (galaxyHeart) saveS.Add("galaxyHeart");

            tag["saveS"] = saveS;
            tag["heartStyle"] = heartStyle;
        }

        public override void LoadData(TagCompound tag)
        {
            var saveS = tag.GetList<string>("saveS");
            foundHall = saveS.Contains("foundHall");
            foundLab = saveS.Contains("foundLab");
            omegaGiftGiven = saveS.Contains("omegaGiftGiven");
            medKit = saveS.Contains("medKit");
            galaxyHeart = saveS.Contains("galaxyHeart");

            heartStyle = tag.GetInt("heartStyle");
        }
    }
}
