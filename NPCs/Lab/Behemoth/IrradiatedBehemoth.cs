using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Buffs.Debuffs;
using Terraria.DataStructures;
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
using Redemption.Buffs.NPCBuffs;
using Redemption.Base;

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
            DisplayName.SetDefault("Irradiated Behemoth");
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>(),
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 412;
            NPC.height = 56;
            NPC.friendly = false;
            NPC.damage = 500;
            NPC.defense = 0;
            NPC.lifeMax = 26000;
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
                new FlavorTextBestiaryInfoElement("An unfortunate scientist, disfigured and mutilated beyond recognition by the Xenomite infection. This specimen is entering the final stage of the infection, and has had its body transform into a sludgy slurry on the ceiling... God, that must be agonizing.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
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
                    Gore.NewGore(new Vector2(NPC.position.X + (i * (NPC.width / 3)), NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreFlesh").Type);
                for (int i = 0; i < 10; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreGoo").Type);

                Gore.NewGore(new Vector2(NPC.Center.X - 60, NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHand").Type);
                Gore.NewGore(new Vector2(NPC.Center.X + 60, NPC.Center.Y), NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHand1").Type);
                Gore.NewGore(NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Redemption/IBGoreHead").Type);
            }
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[1])
                Item.NewItem(NPC.GetItemSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel2>());

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
            DespawnHandler();

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Irradiated Behemoth", 60, 90, 0.8f, 0, Color.Green, "An Unfortunate Scientist");
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/SpookyNoise"), NPC.position);
                        }
                    }
                    if (AITimer < 180 && NPC.DistanceSQ(Main.LocalPlayer.Center) < 1300 * 1300)
                    {
                        NPC.velocity.Y = 0.1f;
                        player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                        player.RedemptionScreen().lockScreen = true;
                    }
                    else
                    {
                        AITimer = 0;
                        NPC.velocity.Y = 0;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.Crawl;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Gas:
                    if (AITimer == 20 || AITimer == 100)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int rot = 25 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Tele>(), 0, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 25)), false, SoundID.Item1.WithVolume(0));
                        }
                    }
                    if (AITimer == 60)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int rot = 20 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Tele>(), 0, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 40)), false, SoundID.Item1.WithVolume(0));
                        }
                    }
                    if (AITimer == 60 || AITimer == 140)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int rot = 25 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Proj>(), 75, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 25)), false, SoundID.NPCDeath13);
                        }
                    }
                    if (AITimer == 100)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int rot = 20 * i;
                            NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGas_Proj>(), 75, RedeHelper.PolarVector(5, MathHelper.PiOver2 + MathHelper.ToRadians(rot - 40)), false, SoundID.NPCDeath13);
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
                        NPC.Shoot(NPC.Center + new Vector2(0, 20), ModContent.ProjectileType<GreenGloop_Tele>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));

                    if (AITimer == 60)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/VomitAttack").WithPitchVariance(0.1f), NPC.position);
                        TimerRand = Main.rand.NextBool() ? 0 : MathHelper.Pi;
                        if (TimerRand == 0)
                            TimerRand2 = 1;
                    }

                    if (AITimer >= 60 && AITimer <= 100 && NPC.frameCounter % 3 == 0)
                    {
                        NPC.Shoot(NPC.Center + new Vector2(0, 40), ModContent.ProjectileType<GreenGloop_Proj>(), 90, RedeHelper.PolarVector(12, TimerRand), false, SoundID.Item1.WithVolume(0), "", NPC.whoAmI);
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
                NPC.Shoot(new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.Center.Y), ModContent.ProjectileType<GreenGas_Proj>(), 200, new Vector2(0, Main.rand.Next(-20, -10)), false, SoundID.Item1.WithVolume(0));

            if (NPC.Center.Y > (RedeGen.LabVector.Y + 119) * 16)
            {
                NPC.alpha += 2;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
        }
        private int AniFrameY;
        private Vector2 HandVector;
        public override void FindFrame(int frameHeight)
        {
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
                    }
                }
            }
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
            Texture2D HeadAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head").Value;
            Texture2D HandAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Hand").Value;
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
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(1000, 3200));
        }
    }
}