using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles;
using Redemption.Textures;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class ThornSeed : ModProjectile
    {
        private bool spawnDust;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 900;
            Projectile.extraUpdates = 2;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() / 50 * Projectile.spriteDirection;
            if (Projectile.velocity.Y < 4)
                Projectile.velocity.Y += 0.02f;
            Projectile.velocity.X *= 0.996f;
            if (!spawnDust)
            {
                magicOpacity = 1;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                {
                    PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                });
                spawnDust = true;
            }
            if (magicOpacity > 0)
                magicOpacity -= .0025f;
        }
        float magicOpacity;
        float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(CommonTextures.WhiteGlow.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Brown with { A = 0 }) * (-magicOpacity + 1), Projectile.rotation, CommonTextures.WhiteGlow.Size() / 2, Projectile.scale * .1f, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);

            if (magicOpacity > 0)
            {
                Color color = Color.SkyBlue;
                RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, magicOpacity);

                Main.EntitySpriteDraw(CommonTextures.WhiteFlare.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color with { A = 0 }) * magicOpacity, Projectile.rotation, CommonTextures.WhiteFlare.Size() / 2, Projectile.scale * magicOpacity, 0, 0);
                Main.EntitySpriteDraw(CommonTextures.WhiteFlare.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }) * magicOpacity, -Projectile.rotation, CommonTextures.WhiteFlare.Size() / 2, Projectile.scale * .6f * magicOpacity, 0, 0);
            }
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Grass, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 4; i++)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, 8), new Vector2(Main.rand.Next(-4, 5), -Main.rand.Next(2, 9)), ModContent.ProjectileType<CursedThornVile>(), Projectile.damage, 3, Projectile.owner, ai2: Projectile.ai[2]);
            }
            return true;
        }
    }
}