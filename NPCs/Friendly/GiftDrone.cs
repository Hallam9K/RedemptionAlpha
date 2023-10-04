using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.Items.Usable;
using Terraria.ModLoader.Utilities;
using Terraria.Localization;
using Redemption.BaseExtension;
using Redemption.Items.Usable.Summons;
using Redemption.NPCs.HM;
using Redemption.UI;
using SubworldLibrary;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Friendly
{
    public class GiftDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gift Drone");
            Main.npcFrameCount[NPC.type] = 3;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 16;
            NPC.friendly = true;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }

            switch (NPC.ai[0])
            {
                case 0: // Fly Down
                    NPC.ai[2]++;
                    if (NPC.DistanceSQ(player.Center) < 200 * 200 || NPC.ai[2] > 220)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[2] = 0;
                    }
                    else
                        NPC.Move(player.Center + new Vector2(100 * player.direction, -100), 7, 15);
                    break;
                case 1: // Stop
                    Point tile = NPC.Center.ToTileCoordinates();
                    if (Framing.GetTileSafely(tile.X, tile.Y).HasTile)
                    {
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y -= 0.1f;
                        NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -2f, 8);
                    }
                    else
                    {
                        NPC.velocity *= 0.96f;
                        if (NPC.ai[2]++ > 120 && NPC.DistanceSQ(player.Center) < 200 * 200)
                        {
                            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.Center.X, (int)NPC.Center.Y + 26, 1, 1, ModContent.ItemType<OmegaGift>(), 1, false, 0, true, false);
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 2;
                        }
                    }
                    break;
                case 2: // Yeet out
                    if (NPC.ai[2]++ > 10)
                    {
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
                            NPC.active = false;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight)
                    NPC.frame.Y = 0;
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.Redemption().foundLab || spawnInfo.Player.Redemption().omegaGiftGiven || NPC.AnyNPCs(Type) || RedeHelper.BossActive())
                return 0;

            return SpawnCondition.OverworldDay.Chance * 0.2f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D gift = ModContent.Request<Texture2D>(Texture + "_Gift").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.ai[0] < 2)
                spriteBatch.Draw(gift, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation * 2, NPC.frame.Size() / 2 + new Vector2(-6, -10), NPC.scale, effects, 0);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
    public class GiftDrone2 : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/GiftDrone";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gift Drone");
            Main.npcFrameCount[NPC.type] = 3;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 16;
            NPC.friendly = true;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
        }
        public override bool NeedSaving() => true;
        public override bool CheckActive() => false;
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[1]];
            if (!player.active)
                NPC.active = false;

            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }

            switch (NPC.ai[0])
            {
                case 0: // Fly Down
                    if (NPC.Sight(player, 200, false, true, true))
                    {
                        NPC.ai[0] = 1;
                    }
                    else
                        NPC.Move(player.Center + new Vector2(100 * player.direction, -100), NPC.DistanceSQ(player.Center) > 1800 * 1800 ? 50 : 7, 15);
                    break;
                case 1: // Stop
                    NPC.velocity *= 0.96f;
                    if (NPC.ai[2]++ > 120 && NPC.DistanceSQ(player.Center) < 400 * 400)
                    {
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.Center.X, (int)NPC.Center.Y + 26, 1, 1, ModContent.ItemType<OmegaTransmitter>(), 1, false, 0, true, false);
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 2;
                    }
                    break;
                case 2: // Yeet out
                    if (NPC.ai[2]++ > 10)
                    {
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
                            NPC.active = false;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight)
                    NPC.frame.Y = 0;
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D gift = ModContent.Request<Texture2D>(Texture + "2_Gift").Value;
            Texture2D giftGlow = ModContent.Request<Texture2D>(Texture + "2_Gift_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.ai[0] < 2)
            {
                spriteBatch.Draw(gift, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation * 2, NPC.frame.Size() / 2 + new Vector2(-6, -10), NPC.scale, effects, 0);
                spriteBatch.Draw(giftGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation * 2, NPC.frame.Size() / 2 + new Vector2(-6, -10), NPC.scale, effects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
    public class GiftDrone3 : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/GiftDrone";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gift Drone");
            Main.npcFrameCount[NPC.type] = 3;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 16;
            NPC.friendly = true;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
            NPC.ShowNameOnHover = false;
        }
        public override bool NeedSaving() => true;
        public override bool CheckActive() => false;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            NPC.LookAtEntity(player);

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }

            switch (NPC.ai[0])
            {
                case 0:
                    bool enemyNear = false;
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                            continue;

                        if (player.DistanceSQ(npc.Center) > 800 * 800)
                            continue;

                        enemyNear = true;
                    }
                    if (!RedeHelper.BossActive() && !enemyNear)
                        NPC.ai[0] = 1;
                    break;
                case 1:
                    GirusDialogue();
                    break;
                case 2: // Fly Down
                    GirusDialogue();
                    if (NPC.alpha > 0)
                        NPC.alpha -= 10;
                    NPC.ShowNameOnHover = true;
                    if (NPC.Sight(player, 200, false, true, true))
                    {
                        NPC.ai[0] = 3;
                    }
                    else
                        NPC.Move(player.Center + new Vector2(100 * player.direction, -100), NPC.DistanceSQ(player.Center) > 1800 * 1800 ? 50 : 7, 15);
                    break;
                case 3: // Stop
                    GirusDialogue();
                    if (NPC.alpha > 0)
                        NPC.alpha -= 10;
                    NPC.velocity *= 0.96f;
                    if (NPC.ai[2]++ > 120 && NPC.DistanceSQ(player.Center) < 400 * 400)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeWorld.keycardGiven = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.Center.X, (int)NPC.Center.Y + 26, 1, 1, ModContent.ItemType<Keycard>(), 1, false, 0, true, false);
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 4;
                    }
                    break;
                case 4: // Yeet out
                    GirusDialogue();
                    if (NPC.ai[2]++ > 10 && NPC.ai[3] > endTime)
                    {
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
                            NPC.active = false;
                    }
                    break;
            }
        }
        private int endTime;
        private void GirusDialogue()
        {
            if (NPC.ai[3]++ >= 540)
                RedeSystem.Silence = true;

            if (RedeBossDowned.downedBehemoth && (!RedeBossDowned.downedOmega1 || !RedeBossDowned.downedOmega2))
            {
                endTime = 2180;
                if (NPC.ai[3] == 1780)
                    NPC.ai[0] = 2;
                if (!Main.dedServ)
                {
                    if (NPC.ai[3] == 540)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 180, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 720)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v1.1"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 980)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v1.2"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 1380)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v1.3"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 1780)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v1.4"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == endTime)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v1.5"), 460, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                }
            }
            else if (!RedeBossDowned.downedBehemoth)
            {
                endTime = 1780;
                if (NPC.ai[3] == endTime)
                    NPC.ai[0] = 2;
                if (!Main.dedServ)
                {
                    if (NPC.ai[3] == 540)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 180, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 720)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v2.1"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 980)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v2.2"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 1380)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v2.3"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == endTime)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v2.4"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                }
            }
            else
            {
                endTime = 3760;
                if (NPC.ai[3] == 2100)
                    NPC.ai[0] = 2;
                if (!Main.dedServ)
                {
                    if (NPC.ai[3] == 540)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 180, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 720)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.1"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 980)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.2"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 1380)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.3"), 600, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 1980)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.4"), 180, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 2160)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.5"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 2560)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.6"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 2960)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.7"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == 3360)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.8"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                    if (NPC.ai[3] == endTime)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter4.v3.9"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                }
            }
            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight)
                    NPC.frame.Y = 0;
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D gift = ModContent.Request<Texture2D>(Texture + "3_Gift").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.ai[0] < 4)
            {
                spriteBatch.Draw(gift, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation * 2, NPC.frame.Size() / 2 + new Vector2(-6, -10), NPC.scale, effects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!NPC.downedMoonlord || RedeWorld.keycardGiven || SubworldSystem.Current != null)
                return 0;

            float m = NPC.AnyNPCs(Type) || NPC.AnyNPCs(ModContent.NPCType<Android>()) || NPC.AnyNPCs(ModContent.NPCType<SlayerSpawner>()) ? 0 : 100;
            return m;
        }
    }
}
