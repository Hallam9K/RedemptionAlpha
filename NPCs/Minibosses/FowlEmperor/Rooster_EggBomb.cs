using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class Rooster_EggBomb : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ranged/EggBomb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Egg Bomb");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.Center.Y < player.Center.Y)
                fallThrough = true;
            else
                fallThrough = false;
            return true;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;

            if (Projectile.timeLeft < 260)
                Projectile.tileCollide = true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft = 2;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            SoundEngine.PlaySound(SoundID.NPCDeath11 with { Volume = .5f }, Projectile.position);
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MothronEgg, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f, Scale: 2);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].velocity *= 3;
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].velocity *= 6;
                Main.dust[dust].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int g = 0; g < 3; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
            int radius = Main.expertMode ? 60 : 50;
            RedeHelper.PlayerRadiusDamage(radius, Projectile, NPCHelper.HostileProjDamageInc(Projectile.damage), Projectile.knockBack);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 6);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}