using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab
{
    [AutoloadBossHead]
    public class Stage3Scientist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 7;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 28;
            NPC.damage = 95;
            NPC.defense = 25;
            NPC.lifeMax = 22000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.aiStyle = -1;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<NoHeadHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
        }
        public override void AI()
        {
            NPC.spriteDirection = 1;
            switch (NPC.ai[0])
            {
                default:
                    if (NPC.ai[1]++ == 0)
                    {
                        if (!Main.dedServ)
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Stage3Scientist.Name"), 60, 90, 0.8f, 0, Color.Green, Language.GetTextValue("Mods.Redemption.TitleCard.Stage3Scientist.Modifier"));
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);
                    }
                    if (NPC.ai[1] == 30)
                    {
                        NPC.velocity.X = 6;
                        NPC.velocity.Y = -5;
                    }
                    if (NPC.ai[1] >= 30)
                        NPC.velocity *= .95f;
                    else
                        NPC.velocity.Y = -2;

                    if (NPC.ai[1] >= 180)
                        NPC.ai[0] = 1;
                    break;
                case 1:
                    if (NPC.ai[1]++ >= 30)
                    {
                        NPC.noTileCollide = false;
                        NPC.velocity.Y += .2f;
                        NPC.velocity.X += .05f;
                    }

                    NPC.rotation += NPC.velocity.Y / 90;

                    if (!NPC.noTileCollide && BaseAI.HitTileOnSide(NPC, 3, false))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Shatter, NPC.position);
                        NPC.StrikeInstantKill();
                    }
                    break;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.SpreadUp(20), ModContent.Find<ModGore>("Redemption/PZGoreShard1").Type, 1);
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.SpreadUp(20), ModContent.Find<ModGore>("Redemption/PZGoreShard2").Type, 1);
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.SpreadUp(20), ModContent.Find<ModGore>("Redemption/PZGoreShard3").Type, 1);
                    }
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 6 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.5f);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
    }
}