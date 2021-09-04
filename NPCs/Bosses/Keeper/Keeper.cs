using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class Keeper : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death,
            Unveiled,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Keeper");
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.BossBestiaryPriority.Add(Type);

        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 3500;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.width = 106;
            NPC.height = 140;
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper");
            //BossBag = ModContent.ItemType<KeeperBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("A powerful fallen who had learnt forbidden necromancy, its prolonged usage having mutated her body.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            //int itemType = ModContent.ItemType<ExampleItem>();

            //notExpertRule.OnSuccess((itemType));

            //Finally add the leading rule
            //npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedKeeper)
            {                     
                RedeWorld.alignment++;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "+1", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("An undead... disgusting. Good thing you killed it.", 120, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedKeeper, -1);
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

        public List<int> AttackList = new() { 0, 1, 2, 3, 4 };
        public List<int> CopyList = null;

        private bool Unveiled;

        public ref float AITimer => ref NPC.ai[1];

        public int ID { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            Vector2 DefaultPos = new Vector2(player.Center.X > NPC.Center.X ? player.Center.X - 240 : player.Center.X + 240, player.Center.Y - 50);

            DespawnHandler();

            if (AIState != ActionState.Death)
                NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("The Keeper", 60, 90, 0.8f, 0, Color.Purple, "Octavia von Gailon");

                    AIState = ActionState.Idle;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    MoveToVector2(DefaultPos, 6f);
                    if (++AITimer > 60)
                    {
                        AITimer = 0;
                        if (!Unveiled && (Main.expertMode ? NPC.life < NPC.lifeMax / 2 : NPC.life < (int)(NPC.lifeMax * 0.35f)))
                        {
                            MoveToVector2(DefaultPos, 0f); //gotta do this to avoid her drifting off, there is most definitely a better solution but I am tired
                            AIState = ActionState.Unveiled;
                            break;
                        }                        
                        /*if (AITimer >= 200)
                        {
                            AttackChoice();
                            AITimer = 0;
                            AIState = ActionState.Attacks;
                            NPC.netUpdate = true;
                        }*/
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Reaper Slash
                        #endregion

                        #region Blood Wave
                        #endregion

                        #region Soul Charge
                        #endregion

                        #region Dread Coil
                        #endregion

                        #region Rupture
                        #endregion
                    }
                    break;
                case ActionState.Unveiled:
                    NPC.dontTakeDamage = true;
                    player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 2;

                    if (++AITimer == 60)
                    {
                        //veil comes off 

                    }
                    if (AITimer >= 220)
                    {
                        Unveiled = true;
                        NPC.dontTakeDamage = false;
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;

                    
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else;
            {

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;
                return false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 4 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public void MoveToVector2(Vector2 p, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = p - NPC.Center;
            float length = dist == Vector2.Zero ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }
            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }
            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }
            NPC.velocity = length == 0f ? Vector2.Zero : Vector2.Normalize(dist);
            NPC.velocity *= moveSpeed;
            NPC.velocity *= velMultiplier;
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
                    NPC.noTileCollide = true;
                    NPC.velocity = new Vector2(0f, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
        }
    }
}