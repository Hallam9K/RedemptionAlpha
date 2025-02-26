using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.Textures;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class GiantStar_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Giant Star");
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 280;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != NPCType<Nebuleus>() && npc.type != NPCType<Nebuleus_Clone>() && npc.type != NPCType<Nebuleus2>() && npc.type != NPCType<Nebuleus2_Clone>()))
                Projectile.Kill();
            Projectile.Center = npc.Center;

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] == 10 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NebSound2, Projectile.position);

            Projectile.rotation += 0.1f;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.scale = 0.01f;
                    Projectile.localAI[0] = 1;
                    break;
                case 1:
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 20;

                    Projectile.scale += 0.06f;
                    Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.01f, 1.5f);
                    if (Projectile.scale >= 1.5f)
                        Projectile.localAI[0] = 2;
                    break;
                case 2:
                    if (Projectile.alpha < 255)
                        Projectile.alpha += 20;

                    Projectile.scale -= 0.06f;
                    Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.01f, 1.5f);
                    if (Projectile.scale <= 0.01f)
                        Projectile.Kill();
                    break;
            }
            if (Projectile.scale > 0.8f)
                Projectile.hostile = true;
            else
                Projectile.hostile = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
            Asset<Texture2D> whiteGlow = CommonTextures.WhiteGlow;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width(), texture.Height());
            Vector2 origin = new(texture.Width() / 2f, texture.Height() / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(whiteGlow.Value, position, null, Main.DiscoColor * 0.8f * Projectile.Opacity, 0, whiteGlow.Size() / 2, Projectile.scale * 1.8f, 0, 0);
            Main.EntitySpriteDraw(whiteGlow.Value, position, null, Color.White * .5f * Projectile.Opacity, 0, whiteGlow.Size() / 2, Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(whiteGlow.Value, position, null, Color.White * .5f * Projectile.Opacity, 0, whiteGlow.Size() / 2, Projectile.scale * .8f, 0, 0);

            Main.EntitySpriteDraw(texture.Value, position, new Rectangle?(rect), Main.DiscoColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, position, new Rectangle?(rect), Main.DiscoColor * 0.7f * Projectile.Opacity, -Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}