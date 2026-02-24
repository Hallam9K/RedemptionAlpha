using Microsoft.Build.Execution;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class CommonGuardFlag_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 96;
            Projectile.tileCollide = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.alpha -= 10;
            if (Projectile.velocity.Y != 0)
                Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 100);
            else
                Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 0);

            Projectile.velocity.Y += 1;
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y, 15);

            if (planted && Projectile.ai[0]++ % 60 == 0)
            {
                RedeDraw.SpawnCirclePulse(Projectile.Center, Color.DarkRed, 1f, Projectile);
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.dead || Projectile.DistanceSQ(player.Center) > 340 * 340)
                        continue;

                    player.AddBuff(BuffType<FlagbearerBuff>(), 80);
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.friendly || Projectile.DistanceSQ(npc.Center) > 340 * 340)
                        continue;

                    npc.AddBuff(BuffType<FlagbearerBuff>(), 80);
                }
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        bool planted;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!planted)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact.WithPitchOffset(0.5f).WithVolumeScale(0.5f), Projectile.position);
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.Bottom - new Vector2(5, 4), 10, 4, DustID.WoodFurniture, 0, -Projectile.oldVelocity.Y / 4);
                }
                planted = true;
            }

            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active || !owner.RedemptionPlayerBuff().commonGuardBonus)
            {
                Projectile.Kill();
                return false;
            }
            return true;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Rectangle rect = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 drawOrigin = rect.Size() / 2;

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture.Value, ref drawTimer, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
}