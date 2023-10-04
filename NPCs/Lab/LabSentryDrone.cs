using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Walls;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.Biomes;

namespace Redemption.NPCs.Lab
{
    public class LabSentryDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Drone");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 20;
            NPC.friendly = false;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.immortal = true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric);
        }

        private bool customGunRot;
        private float gunRot;
        private float movX;
        private float movY;

        public override void AI()
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 30;
                if (NPC.frame.Y > 90)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
            Player player = Main.player[NPC.target];
            if (DespawnHandler())
                return;

            NPC.Move(new Vector2(movX, movY), 15, 30, true);
            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.ai[3]++;
            if (NPC.ai[3] % 60 == 0)
            {
                movX = Main.rand.Next(-100, 100) * 2;
                movY = Main.rand.Next(-80, 80) * 2;
            }

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            Point point = player.position.ToTileCoordinates();
            if (NPC.DistanceSQ(player.Center) < 650 * 650 && NPC.DistanceSQ(player.Center) >= 450 * 450)
            {
                customGunRot = false;
                if (NPC.ai[1] == 0)
                {
                    CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "INTRUDER DETECTED...", true, false);
                    NPC.ai[1] = 1;
                }
            }
            else if (NPC.DistanceSQ(player.Center) < 450 * 450)
            {
                customGunRot = false;
                if (Main.tile[point.X, point.Y].WallType == ModContent.WallType<BlackHardenedSludgeWallTile>() || Main.tile[point.X, point.Y].WallType == ModContent.WallType<HardenedSludgeWallTile>() || Main.tile[point.X, point.Y].WallType == ModContent.WallType<LabPlatingWallTileUnsafe>() || Main.tile[point.X, point.Y].WallType == ModContent.WallType<VentWallTile>())
                {
                    if (!player.dead && player.active)
                    {
                        NPC.ai[2]++;
                        if (NPC.ai[2] % 30 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(20, (player.Center - NPC.Center).ToRotation()), ProjectileID.MartianTurretBolt, 100, 0, Main.myPlayer);
                            SoundEngine.PlaySound(SoundID.Item91, NPC.position);
                            Main.projectile[proj].tileCollide = false;
                            Main.projectile[proj].timeLeft = 200;
                            Main.projectile[proj].netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                NPC.ai[2] = 0;
                customGunRot = true;
                gunRot = 1.5708f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D gunAni = ModContent.Request<Texture2D>(Texture + "_Head").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(gunAni, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, gunAni.Width, gunAni.Height)), drawColor, customGunRot ? gunRot : NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), new Vector2(gunAni.Width / 2f, gunAni.Height / 2f), NPC.scale, effects, 0);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
        private bool DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || !player.InModBiome<LabBiome>() || RedeWorld.labSafe)
            {
                NPC.TargetClosest(false);
                NPC.velocity.X *= .96f;
                NPC.velocity.Y -= 3;
                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;
                return true;
            }
            return false;
        }
    }
}