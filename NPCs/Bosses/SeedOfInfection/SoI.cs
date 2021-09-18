using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Items.Usable;
using System.IO;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    [AutoloadBossHead]
    public class SoI : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seed of Infection");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 76;
            NPC.height = 76;
            NPC.damage = 36;
            NPC.defense = 10;
            NPC.lifeMax = 6500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.alpha = 0;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossXeno1");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool? CanHitNPC(NPC target) => false;


        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("A blistering pocket of the Xenomite infection.")
            });
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedSeed)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You've awoken the infection now. But don't worry, I'm sure we can handle it!", 60, 90, 0, Color.DarkGoldenrod);
                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSeed, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
            }
        }

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        public List<int> CopyList = null;
        public List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6 };
        private float move;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            DespawnHandler();

            if (AIState != ActionState.Death && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Begin:
                    break;
                case ActionState.Idle:
                    NPC.rotation += 0.09f;
                    AITimer++;
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Shard Shot
                        case 1:
                            NPC.rotation += 0.09f;
                            NPC.Move(new Vector2(move - 240, player.Center.Y - 240), 5f, 20, false);
                            break;
                            #endregion

                            #region Irradiated Rain
                            #endregion

                            #region Toxic Sludge
                            #endregion

                            #region Xenomite Shot
                            #endregion

                            #region Hive Growths
                            #endregion

                            #region Scatter Splatter
                            #endregion

                            #region Xenomite Beam
                            #endregion

                    }
                    break;
                case ActionState.Death:
                    break;
            }
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - 240)
                {
                    move = player.Center.X - 240;
                }
                else if (move > player.Center.X - 120)
                {
                    move = player.Center.X - 120;
                }
            }
            else
            {
                if (move > player.Center.X + 240)
                {
                    move = player.Center.X + 240;
                }
                else if (move < player.Center.X + 120)
                {
                    move = player.Center.X + 120;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 2;
            NPC.frame.X = 0;

            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {
                NPC.velocity *= 0;
                NPC.alpha = 0;
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;
                return false;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return null;
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
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}