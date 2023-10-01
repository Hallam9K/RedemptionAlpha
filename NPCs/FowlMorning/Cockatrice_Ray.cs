using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Redemption.Dusts;

namespace Redemption.NPCs.FowlMorning
{
    public class Cockatrice_Ray : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Petrifying Gaze");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 700;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            return target.direction != npc.spriteDirection;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.HasBuff(BuffID.Stoned))
                target.AddBuff(BuffID.Stoned, Main.rand.Next(60, 121));
        }
        public override void AI()
        {
            Vector2 v = Projectile.position;
            int dust = Dust.NewDust(v - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.2f, 0.25f));
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;
            Color dustColor = new(255, 255, 255) { A = 0 };
            Main.dust[dust].color = dustColor;
        }
        public override void OnKill(int timeLeft)
        {
            RedeDraw.SpawnRing(Projectile.Center, Color.White);
        }
    }
}
