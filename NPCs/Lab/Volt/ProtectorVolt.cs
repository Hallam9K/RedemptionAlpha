using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Redemption.Projectiles.Hostile;
using Redemption.Items.Usable;
using Terraria.DataStructures;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Lab.Volt
{
    [AutoloadBossHead]
    public class ProtectorVolt : ModNPC
    {
        private readonly float[] oldrot = new float[3];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 70;
            NPC.friendly = false;
            NPC.damage = 120;
            NPC.defense = 90;
            NPC.lifeMax = 160000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LabBiome>().Type };
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("")
            });
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[3])
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel4>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVolt, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (!player.active || player.dead)
                return;
        }
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (NPC.velocity.Length() == 0)
            {
                if (NPC.frameCounter++ >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
                NPC.frame.Y = 4 * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GunTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Gun").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                spriteBatch.Draw(texture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, RedeColor.VlitchGlowColour * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Vector2 gunCenter = new(NPC.Center.X, NPC.Center.Y + 6);
            int height = GunTex.Height / 4;
            spriteBatch.Draw(GunTex, gunCenter - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, height)), drawColor, NPC.rotation, new Vector2(GunTex.Width / 2f, height / 2f), NPC.scale, effects, 0f);
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
    }
}