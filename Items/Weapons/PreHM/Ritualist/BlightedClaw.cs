using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BlightedClaw_Slash : ModProjectile
    {
        public float directionLock;
        public Player Player => Main.player[Projectile.owner];
        public override string Texture => "Redemption/NPCs/Bosses/Thorn/Thorn_ClawSlash";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjNature[Type] = true;
            ElementID.ProjBlood[Type] = true;
        }
        public override void SetDefaults()
        {
            //Projectile.DamageType = GetInstance<RitualistDaggerClass>();
            //Projectile.Redemption().RitDagger = true;
            Projectile.Redemption().TechnicallyMelee = true;

            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.scale = 0.75f;

            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            directionLock = Player.direction;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.Pi;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
            if (Projectile.frame < 2)
            {
                Projectile.velocity *= 1.2f;
                Projectile.scale *= 1.1f;
            }
            else if (Projectile.frame > 2)
            {
                Projectile.velocity *= .85f;
                Projectile.scale *= .9f;
                Projectile.alpha += 10;
            }
            float curvature = 0.025f * Projectile.frame;
            Projectile.velocity = Projectile.velocity.RotatedBy(curvature * Projectile.ai[0] * directionLock * -1);
        }
        public override bool CanHitPlayer(Player target) => Projectile.frame < 3;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = rect.Size() / 2;

            bool dance = Projectile.ai[1] != 0;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k];
                Color color = Projectile.GetAlpha(Color.LightCyan with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                float scale = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture, drawPos + Projectile.Size / 2f - Main.screenPosition, new Rectangle?(rect), color, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale * scale, 0, 0f);

                if (dance)
                    color = Projectile.GetAlpha(Color.Purple with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture, drawPos + Projectile.Size / 2f - Main.screenPosition, new Rectangle?(rect), color, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale * scale, 0, 0f);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Slash2 with { Volume = .5f }, Projectile.position);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Vector2 velocity = Projectile.velocity.SafeNormalize(default) * 1;
            for (int i = 0; i < 4; i++)
            {
                RedeParticleManager.CreateSpeedParticle(Main.rand.NextVector2FromRectangle(target.Hitbox), velocity * 15, 1f, Color.DarkRed, 0.91f, 21, 16);
            }
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(target.Hitbox), DustID.Blood, velocity * 4, 0, Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
    }
}