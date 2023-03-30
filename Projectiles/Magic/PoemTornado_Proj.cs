using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Redemption.Globals;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Magic
{
    public class PoemTornado_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TornadoTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tornado");
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 4;
            Projectile.scale = 2;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        private int Frame1;
        private int Frame2 = 1;
        private int Frame3 = 2;
        private int Frame4 = 3;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.frameCounter++ % 4 == 0)
            {
                if (++Frame1 > 5)
                    Frame1 = 0;
                if (++Frame2 > 5)
                    Frame2 = 0;
                if (++Frame3 > 5)
                    Frame3 = 0;
                if (++Frame4 > 5)
                    Frame4 = 0;
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 90, 255);
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel)
                    Projectile.alpha -= 3;
                else
                    Projectile.alpha += 6;

                Projectile.Move(Main.MouseWorld, 24, 40);
                if (Projectile.localAI[0]++ % 180 == 0)
                    SoundEngine.PlaySound(CustomSounds.WindLong, Projectile.position);
                if (Projectile.localAI[0] >= 20 && Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Rectangle left = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            Rectangle right = new((int)Projectile.position.X + (Projectile.width / 2), (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.knockBackResist <= 0 || npc.boss)
                    continue;

                float dist = npc.Distance(Projectile.Center);
                if (left.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X += 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
                if (right.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X -= 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new((int)Projectile.position.X + 60, (int)Projectile.position.Y, 88, Projectile.height);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.alpha <= 200 ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int width = texture.Width / 5;
            int x = width * Frame1;
            int x2 = width * Frame2;
            int x3 = width * Frame3;
            int x4 = width * Frame3;
            Rectangle rect = new(x, 0, width, texture.Height);
            Rectangle rect2 = new(x2, 0, width, texture.Height);
            Rectangle rect3 = new(x3, 0, width, texture.Height);
            Rectangle rect4 = new(x4, 0, width, texture.Height);
            Vector2 drawOrigin = new(width / 2, texture.Height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect2), lightColor * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect3), lightColor * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect4), lightColor * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);

            return false;
        }
    }
}