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
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Furniture.Shade;
using Redemption.Items.Weapons.PostML.Melee;
using Terraria.Utilities;
using SubworldLibrary;
using Redemption.WorldGeneration.Soulless;
using Terraria.Audio;
using ReLogic.Utilities;
using Terraria.ID;
using Redemption.Walls;
using Redemption.Base;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Microsoft.Xna.Framework.Graphics;
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
        public int heartStyle;
        public bool stalkerSilence;
        public float musicVolume;
        public int slayerStarRating;
        public int SpaceBreathTimer = 0;
        public bool contactImmune;
        public bool contactImmuneTrue;
        public bool slayerCursor;
        public Rectangle meleeHitbox;
        public int crystalGlaiveLevel;
        public int crystalGlaiveShotCount;
        public bool parryStance;
        public bool parried;

        public override void ResetEffects()
        {
            hitTarget = -1;
            hitTarget2 = -1;
            stalkerSilence = false;
            contactImmune = false;
            if (contactImmune)
                contactImmuneTrue = true;
            else
                contactImmuneTrue = false;
            meleeHitbox = Rectangle.Empty;
            slayerCursor = false;
            contactImmune = false;
            parried = false;
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
        public override void OnEnterWorld(Terraria.Player player)
        {
            Main.NewText("===IMPORTANT===\n" +
                "You are using the Spoiler branch, which as you can guess, contains spoilers we don't yet want to reveal to public.\n" +
                "Keep your findings in this branch to yourself please.\n" +
                "===============", 244, 71, 255);
        }

        public string GetSpaceDeathQuote()
        {
            string[] SpaceDeathQuotes = new string[]
            {
                " was eaten by space",
                " burned while freezing",
                " forgot they needed to breathe",
                " was trapped in orbit",
                " took the wrong step for mankind",
                " wanted to be a satellite",
                " boldly went where no man has gone before",
                " thought the sky was the limit",
                " needed some space"
            };
            int randomQuote = Main.rand.Next(SpaceDeathQuotes.Length);

            return Player.name + SpaceDeathQuotes[randomQuote];
        }
        public override void PreUpdate()
        {
            if (Player.position.Y >= 16210 && Player.InModBiome<SpaceBiome>() && Player.whoAmI == Main.myPlayer)
            {
                int damage = 8;
                Player.AddBuff(BuffID.Obstructed, 3);
                Player.statLife -= damage;
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Player.whoAmI, -damage, 0f, 0f, 0, 0, 0);
                if (Player.statLife <= 0)
                    Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " fell into deep space..."), 10, 0, false);
            }

            if (Player.InModBiome<SpaceBiome>() && Player.whoAmI == Main.myPlayer)
            {
                Point point = Player.Center.ToTileCoordinates();
                ushort wallType = Framing.GetTileSafely(point.X, point.Y).WallType;
                if (wallType != ModContent.WallType<SlayerShipPanelWallTile>() && wallType != WallID.MartianConduit && wallType != WallID.Glass)
                {
                    Player.gravity /= 20;
                    Player.breath -= 3;
                    int airCD = 1;
                    if (Player.accDivingHelm || BasePlayer.HasAccessory(Player, ModContent.ItemType<GasMask>(), true, true) || Player.RedemptionPlayerBuff().hazmatSuit || Player.RedemptionPlayerBuff().HEVSuit)
                        airCD = 2;
                    if (++SpaceBreathTimer % airCD == 0)
                    {
                        SpaceBreathTimer = 0;
                        Player.breath--;
                    }
                    if (Player.breath < -5)
                    {
                        Player.lifeRegen = 0;
                        Player.lifeRegenTime = 0;
                        Player.breath = -5;
                        Player.statLife -= 2;
                        NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Player.whoAmI, -2, 0f, 0f, 0, 0, 0);
                        if (Player.statLife <= 0)
                        {
                            Player.statLife = 0;
                            PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(GetSpaceDeathQuote());
                            Player.KillMe(damageSource, 10.0, 0);
                        }
                    }
                }
                Player.accDepthMeter = 0;
            }
        }
        public override void OnRespawn(Terraria.Player player)
        {
            if (Player.InModBiome<SpaceBiome>())
                SubworldSystem.Exit();
        }

        public static readonly SoundStyle SoullessLoopSound = new("Redemption/Sounds/Custom/SoullessAmbient");
        private SlotId soullessLoopSoundSlot;
        private float soullessEffectIntensity;
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            float soullessEffectPitch = 0f;
            if (SubworldSystem.IsActive<SoullessSub>() && Player.InModBiome<SoullessBiome>())
            {
                if (stalkerSilence)
                {
                    soullessEffectIntensity = MathHelper.Min(0.05f, Main.ambientVolume);
                    soullessEffectPitch = -0.5f;
                }
                else
                {
                    soullessEffectIntensity = Main.ambientVolume;
                }
            }
            else
                soullessEffectIntensity = 0;
            CustomSounds.UpdateLoopingSound(ref soullessLoopSoundSlot, SoullessLoopSound, soullessEffectIntensity, soullessEffectPitch);
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
            if (Player.InModBiome<SoullessBiome>())
            {
                if (Main.rand.Next(100) < (10 + (Player.cratePotion ? 10 : 0)))
                    itemDrop = ModContent.ItemType<ShadestoneCrate>();
                else
                {
                    WeightedRandom<int> choice = new(Main.rand);
                    choice.Add(ModContent.ItemType<AbyssBloskus>(), 1);
                    choice.Add(ModContent.ItemType<SlumberEel>(), 1);
                    choice.Add(ModContent.ItemType<ChakrogAngler>(), 3);
                    choice.Add(ModContent.ItemType<AbyssStinger>(), 3);
                    choice.Add(ModContent.ItemType<DarkStar>(), 1);
                    choice.Add(ModContent.ItemType<LurkingKetred>(), 10);
                    choice.Add(ModContent.ItemType<ShadeFish>(), 9);
                    itemDrop = choice;
                }
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
