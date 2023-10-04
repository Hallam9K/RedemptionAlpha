using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.UI.ChatUI;
using Terraria.Localization;
using System;

namespace Redemption.NPCs.Bosses.Erhan
{
    [AutoloadBossHead]
    public class ErhanSpirit : Erhan
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/Erhan";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Erhan's Spirit");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCHoly[Type] = true;
            ElementID.NPCArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GoldFlame, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 4);
            }
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedErhan, -1);
        }
        private bool Funny;
        public override bool PreAI()
        {
            switch (AIState)
            {
                case ActionState.Begin:
                    switch (TimerRand)
                    {
                        case 1:
                            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                                NPC.TargetClosest();

                            Player player = Main.player[NPC.target];

                            if (NPC.DespawnHandler(1))
                                return false;

                            if (AIState is not ActionState.Fallen && AIState is not ActionState.Death && AIState is not ActionState.Bible)
                            {
                                NPC.LookAtEntity(player);
                                NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;
                            }
                            if (RedeBossDowned.erhanDeath < 4)
                            {
                                if (AITimer++ == 0 && Main.rand.NextBool(10))
                                {
                                    Funny = true;
                                    NPC.netUpdate = true;
                                }

                                if (Funny)
                                {
                                    if (AITimer == 1 && !Main.dedServ)
                                    {
                                        Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.ErhanS.Funny"), Color.LightGoldenrodYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, null, Bubble, null, modifier);
                                        ChatUI.Visible = true;
                                        ChatUI.Add(d1);
                                    }
                                    if (AITimer >= 218)
                                    {
                                        if (!Main.dedServ)
                                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossErhan");

                                        if (RedeBossDowned.erhanDeath < 4 && Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            RedeBossDowned.erhanDeath = 4;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.WorldData);
                                        }

                                        TimerRand = 0;
                                        AITimer = 0;
                                        NPC.dontTakeDamage = false;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                    }
                                }
                                else
                                {
                                    if (AITimer == 1 && !Main.dedServ)
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.ErhanS.1"), Color.LightGoldenrodYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.ErhanS.2"), Color.LightGoldenrodYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                                        chain.OnEndTrigger += Chain_OnEndTrigger;
                                        ChatUI.Visible = true;
                                        ChatUI.Add(chain);
                                    }
                                    if (AITimer >= 1000)
                                    {
                                        if (!Main.dedServ)
                                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossErhan");

                                        if (RedeBossDowned.erhanDeath < 4 && Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            RedeBossDowned.erhanDeath = 4;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.WorldData);
                                        }

                                        TimerRand = 0;
                                        AITimer = 0;
                                        NPC.dontTakeDamage = false;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                    }
                                }
                            }
                            else
                            {
                                if (AITimer++ == 0 && !Main.dedServ)
                                {
                                    Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.ErhanS.3"), Color.LightGoldenrodYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, null, Bubble, null, modifier);
                                    ChatUI.Visible = true;
                                    ChatUI.Add(d1);
                                }
                                if (AITimer >= 150)
                                {
                                    if (!Main.dedServ)
                                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossErhan");

                                    TimerRand = 0;
                                    AITimer = 0;
                                    NPC.dontTakeDamage = false;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                            }
                            return false;
                    }
                    break;
            }
            return true;
        }
        public override bool CheckDead() => true;
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 1000;
        }
        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(1, 1, 0.1f, 0) * NPC.Opacity;
        }
    }
}