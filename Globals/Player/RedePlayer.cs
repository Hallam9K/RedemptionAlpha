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
        public bool stalkerSilence;
        public float musicVolume;
        public int slayerStarRating;
        public bool contactImmune;
        public bool slayerCursor;
        public Rectangle meleeHitbox;

        public override void ResetEffects()
        {
            hitTarget = -1;
            hitTarget2 = -1;
            contactImmune = false;
            meleeHitbox = Rectangle.Empty;
            slayerCursor = false;
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
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (damageSource.SourceNPCIndex >= 0 && contactImmune)
                return false;
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

        public override void PostUpdateMiscEffects()
        {
            Player.statLifeMax2 += (medKit ? 50 : 0) + (galaxyHeart ? 50 : 0);

            if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> emptyTex = ModContent.Request<Texture2D>("Redemption/Empty");
                Asset<Texture2D> cursor0 = ModContent.Request<Texture2D>("Redemption/Textures/Cursor_0");
                Asset<Texture2D> cursor1 = ModContent.Request<Texture2D>("Redemption/Textures/Cursor_1");
                Asset<Texture2D> cursor11 = ModContent.Request<Texture2D>("Redemption/Textures/Cursor_11");
                Asset<Texture2D> cursor12 = ModContent.Request<Texture2D>("Redemption/Textures/Cursor_12");
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
        }

        public override void LoadData(TagCompound tag)
        {
            var saveS = tag.GetList<string>("saveS");
            foundHall = saveS.Contains("foundHall");
            foundLab = saveS.Contains("foundLab");
            omegaGiftGiven = saveS.Contains("omegaGiftGiven");
            medKit = saveS.Contains("medKit");
            galaxyHeart = saveS.Contains("galaxyHeart");
        }
    }
}