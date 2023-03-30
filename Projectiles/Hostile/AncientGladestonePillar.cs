using Redemption.BaseExtension;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class AncientGladestonePillar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.velocity.Length() != 0 && target.type != ModContent.NPCType<AncientGladestoneGolem>() ? null : false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return Projectile.velocity.Length() != 0;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2) && Projectile.localAI[0] < 30)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SlateDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 2);
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 30)
                Projectile.alpha -= 10;
            else if (Projectile.localAI[0] == 30)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = .5f }, Projectile.position);
                Projectile.velocity.Y -= 10;
            }
            else if (Projectile.localAI[0] == 40)
                Projectile.velocity.Y = 0;
            else if (Projectile.localAI[0] > 45)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (target.noKnockback || Projectile.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.velocity.Y = Projectile.velocity.Y * 1.5f;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.knockBackResist <= 0 || Projectile.velocity.Length() == 0 ||
                    !Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.velocity.Y = Projectile.velocity.Y * 1.5f;
            }
        }
    }
}