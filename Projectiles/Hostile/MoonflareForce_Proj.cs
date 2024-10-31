using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Textures;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class MoonflareForce_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/BubbleShield2";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjNature[Type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 192;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Redemption().friendlyHostile = true;
            Projectile.hide = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.Redemption().spiritSummon)
                return null;
            NPC host = Main.npc[(int)Projectile.ai[2]];
            return target == host.Redemption().attacker ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback.Flat += 5;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage >= 10 && !target.noKnockback)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone >= 10 && target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
        }
        public override void AI()
        {
            if (Projectile.ai[0]++ <= 10)
                flareSize = MathHelper.Lerp(0, 2, EaseFunction.EaseCubicIn.Ease(Projectile.ai[0] / 10f));
            else
                flareSize = MathHelper.Lerp(2, 0, EaseFunction.EaseCubicOut.Ease((Projectile.ai[0] - 10) / 74f));

            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frameCounter = 0;
                bubbleFrameX++;
                if (bubbleFrameX > 4)
                {
                    bubbleFrameX = 0;
                    if (bubbleFrameY >= 4)
                        Projectile.Kill();
                    else
                        bubbleFrameY++;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] >= 8 && bubbleFrameY < 1 && Helper.CheckCircularCollision(Projectile.Center, 74, targetHitbox))
                return true;
            return false;
        }
        int bubbleFrameX;
        int bubbleFrameY;
        float flareSize;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> radial = TextureAssets.Projectile[Type];
            Asset<Texture2D> flare = CommonTextures.BigFlare;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();
            Rectangle radialRect = radial.Frame(5, 5, bubbleFrameX, bubbleFrameY);
            Main.EntitySpriteDraw(radial.Value, Projectile.Center - Main.screenPosition, radialRect, new Color(250, 205, 160, 200), Projectile.rotation, new Vector2(radialRect.Width, radialRect.Height) / 2, Projectile.scale, 0, 0);

            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(250, 205, 160) * flareSize, Projectile.rotation, flare.Size() / 2, (Projectile.scale * .5f) + flareSize, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 218) * flareSize, Projectile.rotation, flare.Size() / 2, (Projectile.scale * .25f) + flareSize, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}