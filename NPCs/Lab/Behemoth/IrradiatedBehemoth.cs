using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Buffs.Debuffs;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using Redemption.WorldGeneration;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Lore;
using Terraria.Localization;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Lab.Behemoth
{
    [AutoloadBossHead]
    public class IrradiatedBehemoth : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Crawl,
            Gas,
            Sludge
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Behemoth");
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 412;
            NPC.height = 56;
            NPC.friendly = false;
            NPC.damage = 500;
            NPC.defense = 0;
            NPC.lifeMax = 26600;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.behindTiles = true;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusicIB");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Behemoth"))
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, default, 4f);
                }
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.position.X + (i * (NPC.width / 3)), NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreFlesh").Type);
                for (int i = 0; i < 10; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreGoo").Type);

                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 60, NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHand").Type);
                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X + 60, NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHand1").Type);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHead").Type);
            }
        }
        public override bool PreKill()
        {
            if (!RedeBossDowned.downedBehemoth)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IB_GirusTalk>(), 0, 0, Main.myPlayer);
            return true;
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[1])
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel2>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedBehemoth, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk2>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk2_1>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.DespawnHandler(1, 5))
                return;

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Behemoth.Name"), 60, 90, 0.8f, 0, Color.Green, Language.GetTextValue("Mods.Redemption.TitleCard.Behemoth.Modifier"));
                            SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);
                        }
                    }
                    if (AITimer < 180 && NPC.DistanceSQ(Main.LocalPlayer.Center) < 1800 * 1800)
                    {
                        NPC.velocity.Y = 0.1f;
                        ScreenPlayer.CutsceneLock(player, NPC.Center, ScreenPlayer.CutscenePriority.High, 0, 0, 0, false, true);
                    }
                    else
                    {
                        AITimer = 0;
                        NPC.velocity.Y = 0;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.Crawl;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.Gas:
                    if (AITimer == 20 || AITimer == 100)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int rot = 25 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Tele>(), 0, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 25)));
                        }
                    }
                    if (AITimer == 60)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int rot = 20 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Tele>(), 0, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 40)));
                        }
                    }
                    if (AITimer == 60 || AITimer == 140)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int rot = 25 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Proj>(), 75, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 25)), SoundID.NPCDeath13);
                        }
                    }
                    if (AITimer == 100)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int rot = 20 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Proj>(), 75, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 40)), SoundID.NPCDeath13);
                        }
                    }
                    if (AITimer++ >= 180)
                    {
                        AITimer = 0;
                        AIState = ActionState.Crawl;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Sludge:
                    if (AITimer++ == 0 || AITimer == 8 || AITimer == 16)
                        NPC.Shoot(NPC.Center + new Vector2(0, 20), ModContent.ProjectileType<GreenGloop_Tele>(), 0, Vector2.Zero);

                    if (AITimer == 60)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.VomitAttack with { Pitch = -.4f }, NPC.position);
                        TimerRand = Main.rand.NextBool() ? 0 : MathHelper.Pi;
                        if (TimerRand == 0)
                            TimerRand2 = 1;
                    }

                    if (AITimer >= 60 && AITimer <= 100 && NPC.frameCounter % 3 == 0)
                    {
                        NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGloop_Proj>(), 90, RedeHelper.PolarVector(12, TimerRand), NPC.whoAmI);
                        TimerRand += TimerRand2 == 1 ? 0.2f : -0.2f;
                    }
                    if (AITimer >= 180)
                    {
                        TimerRand = 0;
                        TimerRand2 = 0;
                        AITimer = 0;
                        AIState = ActionState.Crawl;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (Main.LocalPlayer.Center.Y < NPC.Center.Y && Main.rand.NextBool(5))
                NPC.Shoot(new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.Center.Y), ModContent.ProjectileType<GreenGas_Proj>(), 200, new Vector2(0, Main.rand.Next(-20, -10)));

            if (NPC.Center.Y > (RedeGen.LabVector.Y + 119) * 16)
            {
                NPC.alpha += 2;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
            if (AIState is ActionState.Crawl)
            {
                if (AITimer++ < 50)
                    HandVector.Y += 1;
                else
                {
                    HandVector.Y -= 1;
                    NPC.velocity.Y = 1;
                    if (HandVector.Y <= 0)
                    {
                        AITimer = 0;
                        HandVector.Y = 0;
                        NPC.velocity.Y = 0;
                        AIState = (ActionState)Main.rand.Next(2, 4);
                        NPC.netUpdate = true;
                    }
                }
            }
        }
        private int AniFrameY;
        private Vector2 HandVector;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                AniFrameY++;
                if (AniFrameY > 5)
                    AniFrameY = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D HeadAni = ModContent.Request<Texture2D>(Texture + "_Head").Value;
            Texture2D HandAni = ModContent.Request<Texture2D>(Texture + "_Hand").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(new Color(100, 100, 100, 255)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            int Height = HeadAni.Height / 6;
            int y = Height * AniFrameY;
            Rectangle rect = new(0, y, HeadAni.Width, Height);
            Vector2 origin = new(HeadAni.Width / 2f, Height / 2f);
            spriteBatch.Draw(HeadAni, NPC.Center - screenPos + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(100, 100, 100, 255)), NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(HandAni, NPC.Center - screenPos + HandVector + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(100, 100, 100, 255)), NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(1000, 3200));
        }
    }
}
