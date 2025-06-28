using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Redemption.Items.Placeable.Furniture.Shade;
using Redemption.Items.Quest;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.Projectiles.Ranged;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Furniture.Misc;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Walls;
using Redemption.WorldGeneration;
using Redemption.WorldGeneration.Misc;
using Redemption.WorldGeneration.Soulless;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using static Redemption.Globals.RedeNet;

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

        public bool elementStatsUISeen;

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
        public int fallSpeedIncrease;

        public int poopenBeamTimer = 0;

        public override void ResetEffects()
        {
            hitTarget = -1;
            hitTarget2 = -1;
            if (contactImmune)
                contactImmuneTrue = true;
            else
                contactImmuneTrue = false;
            meleeHitbox = Rectangle.Empty;
            slayerCursor = false;
            contactImmune = false;
            parried = false;
            onHit = false;

            if (!Player.sitting.isSitting)
            {
                poopenBeamTimer = 0;
            }
        }
        public override void UpdateEquips()
        {
            Player.maxFallSpeed += fallSpeedIncrease;
            fallSpeedIncrease = 0;
        }
        public override void Initialize()
        {
            foundHall = false;
            foundLab = false;
            omegaGiftGiven = false;
            medKit = false;
            galaxyHeart = false;
            elementStatsUISeen = false;
            heartStyle = 0;
        }
        public override void UpdateDead()
        {
            Player.fullRotation = 0f;
            slayerStarRating = 0;
            parryStance = false;
            parried = false;
        }

        readonly int[] bannedTeleportWalls = new int[]
        {
            WallType<BlackHardenedSludgeWallTile>(),
            WallType<DangerTapeWallTile>(),
            WallType<HardenedSludgeWallTile>(),
            WallType<JunkMetalWall>(),
            WallType<LabPlatingWallTileUnsafe>(),
            WallType<MossyLabPlatingWallTile>(),
            WallType<MossyLabWallTile>(),
            WallType<SlayerShipPanelWallTile>(),
            WallType<VentWallTile>(),
        };
        public override bool CanBeTeleportedTo(Vector2 teleportPosition, string context)
        {
            Point16 point = teleportPosition.ToTileCoordinates16();
            Tile tile = Framing.GetTileSafely(point.X, point.Y);

            if (context == "CheckForGoodTeleportationSpot" && bannedTeleportWalls.Contains(tile.WallType))
                return false;

            return base.CanBeTeleportedTo(teleportPosition, context);
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
            if (Player.RedemptionPlayerBuff().hardlightBonus == 3 && proj.DamageType == DamageClass.Melee && proj.type != ProjectileType<MiniSpaceship_Laser>())
            {
                hitTarget = target.whoAmI;
                hitTarget2 = target.whoAmI;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (Player.HasItem(ItemType<Taikasauva>()) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NoitaDeath);
            if (BasePlayer.HasArmorSet(Mod, Player, "Springlock", true) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.AftonScream);
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

            if (RedeConfigServer.Instance.FunniAllWasteland || RedeConfigServer.Instance.FunniJanitor || RedeConfigServer.Instance.FunniSpiders || RedeConfigServer.Instance.FunniWasteland)
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Caution"), Colors.RarityOrange);

            localNukeTimer = RedeWorld.nukeTimer;
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
                        if (!tile.HasTile || tile.TileType != TileType<KariBedTile>())
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
                        if (!tile.HasTile || tile.TileType != TileType<GathuramPortalTile>())
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
                        if (!tile.HasTile || tile.TileType != TileType<AncientAltarTile>())
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
                        if (!tile.HasTile || tile.TileType != TileType<AnglonPortalTile>())
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
                        if (!tile.HasTile || tile.TileType != TileType<SlayerChairTile>())
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
                Asset<Texture2D> emptyTex = Request<Texture2D>("Redemption/Empty");
                Asset<Texture2D> cursor0 = Request<Texture2D>("Terraria/Images/UI/Cursor_0");
                Asset<Texture2D> cursor1 = Request<Texture2D>("Terraria/Images/UI/Cursor_1");
                Asset<Texture2D> cursor11 = Request<Texture2D>("Terraria/Images/UI/Cursor_11");
                Asset<Texture2D> cursor12 = Request<Texture2D>("Terraria/Images/UI/Cursor_12");
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
            if (Player.InModBiome<LabBiome>() || SubworldSystem.AnyActive<Redemption>())
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

            UpdateFilterEffects();
            UpdateNukeCountdown();
        }

        private void UpdateFilterEffects()
        {
            if (Main.dedServ)
                return;

            if (Player.InModBiome<SoullessBiome>())
            {
                if (!Main.dedServ)
                {
                    if (!Filters.Scene["MoR:Shake"].IsActive())
                    {
                        Filters.Scene.Activate("MoR:Shake", Player.position, Array.Empty<object>());
                    }
                    Filters.Scene["MoR:Shake"].GetShader()
                        .UseIntensity(0.3f);
                }
            }

            int PalebatImpID = Terraria.NPC.FindFirstNPC(NPCType<PalebatImp>());
            if (PalebatImpID >= 0)
            {
                if ((Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer > 0)
                {
                    if (!Filters.Scene["MoR:Shake"].IsActive())
                    {
                        Filters.Scene.Activate("MoR:Shake", Main.LocalPlayer.position);
                    }
                    Filters.Scene["MoR:Shake"].GetShader().UseIntensity((Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer);
                }
                else
                {
                    if (Filters.Scene["MoR:Shake"].IsActive())
                        Filters.Scene.Deactivate("MoR:Shake");
                }
            }

            if (Player.GetModPlayer<BuffPlayer>().island)
            {
                if (!Filters.Scene["MoR:Shake"].IsActive())
                    Filters.Scene.Activate("MoR:Shake", Main.LocalPlayer.position);

                Filters.Scene["MoR:Shake"].GetShader().UseIntensity(0.5f);
            }
        }

        private int localNukeTimer = 1800;
        private void UpdateNukeCountdown()
        {
            if (Main.dedServ)
                return;

            if (!RedeWorld.nukeCountdownActive)
            {
                localNukeTimer = RedeWorld.nukeTimer;
            }
            else
            {
                int nukeTimerShown = localNukeTimer / 60;
                if (localNukeTimer % 60 == 0 && localNukeTimer > 0)
                {
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(nukeTimerShown.ToString(), 40, 8, 1, null, ((30f - nukeTimerShown) / 30f) * 2, Color.Red, Color.Black);
                }
                localNukeTimer--;
                if (localNukeTimer <= 0)
                {
                    MoonlordDeathDrama.RequestLight(1f, RedeWorld.nukeGroundZero);

                    if (Vector2.Distance(Player.Center, RedeWorld.nukeGroundZero) < 287 * 16)
                        MoonlordDeathDrama.RequestLight(1f, Player.Center);
                    else if (Vector2.Distance(Player.Center, RedeWorld.nukeGroundZero) < 287 * 2 * 16)
                        MoonlordDeathDrama.RequestLight(0.5f, Player.Center);
                    else
                        MoonlordDeathDrama.RequestLight(0.35f, Player.Center);
                }

                if (localNukeTimer <= -60)
                {
                    SoundEngine.PlaySound(CustomSounds.NukeExplosion, RedeWorld.nukeGroundZero);
                }
            }
        }


        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;
            if (!inWater)
                return;

            if (attempt.crate && !attempt.veryrare && !attempt.legendary && attempt.rare && Main.rand.NextBool())
            {
                if (Player.InModBiome<LabBiome>() && Terraria.NPC.downedMechBoss1 && Terraria.NPC.downedMechBoss2 && Terraria.NPC.downedMechBoss3)
                {
                    if (Terraria.NPC.downedMoonlord)
                        itemDrop = ItemType<LabCrate2>();
                    else
                        itemDrop = ItemType<LabCrate>();
                    return;
                }
            }
            if (Player.InModBiome<WastelandPurityBiome>())
            {
                int blinky = ItemType<Blinky>();
                if (attempt.questFish == blinky && attempt.uncommon)
                {
                    itemDrop = blinky;
                    return;
                }
                if (attempt.crate && !attempt.veryrare && !attempt.legendary && attempt.rare)
                {
                    itemDrop = ItemType<PetrifiedCrate>();
                    return;
                }
                else
                {
                    WeightedRandom<int> choice = new(Main.rand);
                    if (attempt.common)
                    {
                        choice.Add(ItemID.TinCan, 1);
                        choice.Add(ItemID.FishingSeaweed, 1);
                        choice.Add(ItemID.OldShoe, 1);
                        choice.Add(ItemID.Bone, .5);
                        choice.Add(ItemType<BloatedTrout>(), 2);
                    }
                    else if (attempt.uncommon)
                        choice.Add(ItemType<ToxicGlooper>(), 1);
                    else if (attempt.rare)
                        choice.Add(ItemType<ScrapMetal>(), 1);
                    else if (attempt.veryrare)
                    {
                        choice.Add(ItemID.AdhesiveBandage, 1);
                        choice.Add(ItemType<GasMask>(), 1);
                    }
                    else if (attempt.legendary)
                        choice.Add(ItemID.FartinaJar, 1);
                    itemDrop = choice;
                    return;
                }
            }
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (!mediumCoreDeath && (Player.name.Contains("Liz") || Player.name.Contains("Lizzy") || Player.name.Contains("Elizabeth")))
            {
                return new[] {
                    new Item(ItemType<LizzyCookie>())
                };
            }
            if (!mediumCoreDeath && (Player.name == "Uncon" || Player.name == "Dahlia"))
            {
                return new[] {
                    new Item(ItemType<UnconHead>()),
                    new Item(ItemType<UnconBody>()),
                    new Item(ItemType<UnconLegs>()),
                    new Item(ItemType<UnconPatreon_CapeAcc>()),
                    new Item(ItemType<UnconPetItem>())
                };
            }
            return base.AddStartingItems(mediumCoreDeath);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            RedePlayer redePlayer = targetCopy as RedePlayer;

            redePlayer.yesChoice = yesChoice;
            redePlayer.noChoice = noChoice;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            RedePlayer redePlayer = clientPlayer as RedePlayer;

            if (redePlayer.yesChoice != yesChoice || redePlayer.noChoice != noChoice)
                SyncPlayer(-1, -1, false);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)ModMessageType.SyncRedePlayer);
            packet.Write((byte)Player.whoAmI);
            BitsByte bb = new(yesChoice, noChoice);
            packet.Write(bb);
            packet.Send(toWho, fromWho);
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int sender)
        {
            int player = reader.ReadByte();
            RedePlayer redePlayer = Main.player[player].GetModPlayer<RedePlayer>();
            BitsByte bb = reader.ReadByte();
            bb.Retrieve(ref redePlayer.yesChoice, ref redePlayer.noChoice);

            if (Main.netMode == NetmodeID.Server)
                redePlayer.SyncPlayer(-1, sender, false);
        }

        public override void SaveData(TagCompound tag)
        {
            var saveS = new List<string>();
            if (foundHall) saveS.Add("foundHall");
            if (foundLab) saveS.Add("foundLab");
            if (omegaGiftGiven) saveS.Add("omegaGiftGiven");
            if (medKit) saveS.Add("medKit");
            if (galaxyHeart) saveS.Add("galaxyHeart");
            if (elementStatsUISeen) saveS.Add("elementStatsUISeen");

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
            elementStatsUISeen = saveS.Contains("elementStatsUISeen");

            heartStyle = tag.GetInt("heartStyle");
        }
    }
}
