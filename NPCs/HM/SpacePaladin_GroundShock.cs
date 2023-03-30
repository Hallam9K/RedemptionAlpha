using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.HM
{
    public class SpacePaladin_GroundShock : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Lab/MACE/MACE_GroundShock";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 102;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.noKnockback)
                target.velocity.Y -= 16;
            target.AddBuff(BuffID.Electrified, 60);
            target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(Projectile.Bottom, Projectile.width, 2, DustID.Frost, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity.Y = -Main.rand.Next(8, 17);
            }
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
            Projectile.hostile = false;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
                Projectile.hostile = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2 + 40);

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 30) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(new Color(255, 255, 255, 0)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
