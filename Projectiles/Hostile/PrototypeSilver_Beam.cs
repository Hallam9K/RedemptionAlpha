using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class PrototypeSilver_Beam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Beam");
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 1400;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return target == host.Redemption().attacker ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 60);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, 60);
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ > 0f)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 v = Projectile.position;
                    v -= Projectile.velocity * (i * 0.25f);
                    ParticleManager.NewParticle(v, Vector2.Zero, new LightningParticle(), Color.White, Main.rand.NextFloat(0.6f, 0.8f));
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.LightBlue, glowScale: 3);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast, Projectile.position);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 2;
        }
    }
}
