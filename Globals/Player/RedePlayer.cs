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
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if ((damageSource.SourceNPCIndex >= 0 || (damageSource.SourceProjectileIndex >= 0 && Main.projectile[damageSource.SourceProjectileIndex].Redemption().TechnicallyMelee)) && contactImmuneTrue)
                return false;
            if (((damageSource.SourceNPCIndex >= 0 && Main.npc[damageSource.SourceNPCIndex].velocity.Length() > Player.velocity.Length() / 2) ||
                (damageSource.SourceProjectileIndex >= 0 && Main.projectile[damageSource.SourceProjectileIndex].Redemption().TechnicallyMelee)) && parryStance)
            {
                parried = true;
                Player.immune = true;
                Player.immuneTime = (int)MathHelper.Max(Player.immuneTime, 4);
                return false;
            }
            onHit = true;
            return true;
        }
        public override void OnHitNPC(Item item, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (Player.RedemptionPlayerBuff().hardlightBonus == 3 && item.DamageType == DamageClass.Melee)
            {
                hitTarget = target.whoAmI;
                hitTarget2 = target.whoAmI;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (Player.RedemptionPlayerBuff().hardlightBonus == 3 && proj.DamageType == DamageClass.Melee && proj.type != ModContent.ProjectileType<MiniSpaceship_Laser>())
            {
                hitTarget = target.whoAmI;
                hitTarget2 = target.whoAmI;
            }
        }
        public override void OnEnterWorld(Terraria.Player player)
        {
            if (SubworldSystem.Current != null)
                return;
            if (RedeGen.GoldenGatewayPoint.X == 0 || RedeGen.BastionPoint.X == 0 || RedeGen.gathicPortalPoint.X == 0 || RedeGen.HallOfHeroesPoint.X == 0 || RedeGen.slayerShipPoint.X == 0)
                Main.NewText("WARNING: Unable to locate a certain structure, new world is recommended!", Colors.RarityRed);
            if (RedeGen.LabPoint.X == 0 || RedeGen.newbCavePoint.X == 0)
                Main.NewText("WARNING: Unable to locate important structure, new world is required!", Colors.RarityRed);

            if (RedeConfigClient.Instance.FunniAllWasteland || RedeConfigClient.Instance.FunniJanitor || RedeConfigClient.Instance.FunniSpiders || RedeConfigClient.Instance.FunniWasteland)
                Main.NewText("CAUTION: You have a Funni config enabled that affects world gen. If you created a world just now, check which one you have enabled.", Colors.RarityOrange);
        }
        public override void PostUpdateMiscEffects()
        {
            Player.statLifeMax2 +=
                (medKit ? 50 : 0) +
                (galaxyHeart ? 50 : 0);

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
                Asset<Texture2D> heartMed = ModContent.Request<Texture2D>("Redemption/Textures/HeartMed");
                Asset<Texture2D> heartGalaxy = ModContent.Request<Texture2D>("Redemption/Textures/HeartGal");
                Asset<Texture2D> heartOriginal = ModContent.Request<Texture2D>("Redemption/Textures/Heart2");
                int totalHealthBoost =
                    (medKit ? 1 : 0) +
                    (galaxyHeart ? 1 : 0);
                if (totalHealthBoost == 2)
                {
                    TextureAssets.Heart2 = heartGalaxy;
                }
                else if (totalHealthBoost == 1)
                {
                    TextureAssets.Heart2 = heartMed;
                }
                else
                {
                    TextureAssets.Heart2 = heartOriginal;
                }
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