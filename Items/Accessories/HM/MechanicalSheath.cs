using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class MechanicalSheath : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 52;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().mechSheath = true;
            player.RedemptionPlayerBuff().TrueMeleeDamage += 0.25f;
        }
    }
    public class PhantomCleaver_F2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/PhantomCleaver";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.rotation = MathHelper.Pi;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.alpha -= 5;
                    if (!npc.active)
                        Projectile.velocity *= 0;
                    else
                    {
                        float y = MathHelper.Lerp(npc.position.Y - 300, npc.position.Y - 200, EaseFunction.EaseCubicIn.Ease(Projectile.alpha / 255f));
                        Projectile.Center = new Vector2(npc.Center.X, y);
                    }

                    if (Projectile.alpha <= 80)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                        Projectile.friendly = true;
                        Projectile.localAI[1] = 1;
                    }
                    break;
                case 1:
                    Projectile.friendly = true;
                    Projectile.velocity.X = 0;
                    Projectile.velocity.Y += 10;
                    Projectile.alpha += 10;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(30, 0);
                Color color = Projectile.GetAlpha(RedeColor.RedPulse) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
