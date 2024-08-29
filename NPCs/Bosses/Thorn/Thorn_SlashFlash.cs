using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Projectiles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_SlashFlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flash");
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 46;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.hide = true;
            Projectile.scale = 2;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public bool Dance;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Dance);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Dance = reader.ReadBoolean();
        }

        Vector2 originPos;
        public override void AI()
        {
            if (originPos == Vector2.Zero)
                originPos = Projectile.Center;
            Projectile.localAI[0] += .007f;
            Projectile.rotation += Projectile.localAI[0];

            if (Dance)
            {
                Projectile.scale -= .03f;
                Projectile.velocity *= .9f;
            }
            else
            {
                Projectile.scale -= .06f;
                Projectile.velocity *= .9f;
            }
            if (Projectile.scale <= .01f)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), -Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Slice5, Projectile.position);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, originPos.DirectionFrom(Projectile.Center) * (7 + Projectile.ai[0]), ModContent.ProjectileType<Thorn_ClawSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[1], 0, Projectile.ai[2]);
            }
        }
    }
}