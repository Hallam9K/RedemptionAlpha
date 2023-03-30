using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class GrandLarvaFall : ModProjectile
    {
        public override string Texture => "Redemption/Items/Critters/GrandLarvaBait";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grand Larva");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 20 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            int index = NPC.NewNPC(Projectile.GetSource_FromThis(), (int) Projectile.Center.X, (int) Projectile.Center.Y,
                ModContent.NPCType<GrandLarva>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}