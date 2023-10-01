using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.ADD;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    public class RockPileSummon : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rock Pile");
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 54;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 2);
            Main.dust[dust].noGravity = true;
            Projectile.velocity.Y += 0.4f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity.Y = -7;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC host = Main.npc[(int)Projectile.ai[0]];
                int type = ModContent.NPCType<EaglecrestRockPile>();
                if (host.type == ModContent.NPCType<EaglecrestGolem2>())
                    type = ModContent.NPCType<EaglecrestRockPile2>();

                int index = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.position.Y, type);

                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
            }
        }
    }
}