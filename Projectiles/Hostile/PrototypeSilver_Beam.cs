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
            Projectile.extraUpdates = 70;
            Projectile.timeLeft = 1400;
            timeLeftMax = Projectile.timeLeft;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        private int timeLeftMax;
        public override bool? CanHitNPC(NPC target)
        {
            if (target.Redemption().spiritSummon)
                return null;
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return target == host.Redemption().attacker ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= NPCHelper.HostileProjDamageMultiplier();
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
            if (Projectile.localAI[0]++ > 0f && Projectile.localAI[0] % 2 == 0)
            {
                Vector2 v = Projectile.position;
                Color bright = Color.Multiply(new(255, 255, 255, 0), 1);
                Color mid = Color.Multiply(new(161, 255, 253, 0), 1);
                Color dark = Color.Multiply(new(40, 186, 242, 0), 1);

                Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1);
                Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1f);
                RedeParticleManager.CreateQuadParticle2(v, Vector2.Zero, new Vector2(.3f), emberColor, glowColor, 6);
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
