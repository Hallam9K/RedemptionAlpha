using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class TreeBugFall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tree Bug");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            int index = NPC.NewNPC(Projectile.GetSource_FromThis(), (int) Projectile.Center.X, (int) Projectile.Center.Y, ModContent.NPCType<TreeBug>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}