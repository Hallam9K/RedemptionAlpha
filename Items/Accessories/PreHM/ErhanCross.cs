using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Globals;
using Redemption.Globals.Players;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class ErhanCross : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.HolyS, ElementID.ShadowS);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.ElementalResistance[ElementID.Holy] += 0.1f;
            modPlayer.ElementalResistance[ElementID.Shadow] -= 0.1f;

            modPlayer.erhanCross = true;
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && player.ownedProjectileCounts[ProjectileType<ErhanCross_Shield>()] < 1 &&
                !player.HasBuff<ErhanCrossCooldown>())
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ErhanCross_Shield>(), 0, 0, player.whoAmI);
        }
    }
    public class ErhanCross_Shield : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/Erhan_HolyShield";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Shield");
        }
        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.HasBuff<ErhanCrossCooldown>() || !player.RedemptionPlayerBuff().erhanCross)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.timeLeft = 10;
            Projectile.rotation = (playerCenter - Projectile.Center).ToRotation();

            Projectile.localAI[0] += 0.04f;
            Projectile.Center = playerCenter + Vector2.One.RotatedBy(Projectile.localAI[0]) * 80;

            Projectile.alpha--;
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 80, 255);
            Projectile.ai[1]--;

            if (Projectile.alpha < 100 && Projectile.ai[1] <= 0)
            {
                foreach (Projectile target in Main.ActiveProjectiles)
                {
                    if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile)
                        continue;

                    if (target.velocity.Length() == 0 || target.ProjBlockBlacklist() || !Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    Projectile.ai[1] = 10;
                    SoundEngine.PlaySound(SoundID.Item29, Projectile.position);
                    RedeDraw.SpawnExplosion(target.Center, new Color(246, 255, 148), shakeAmount: 0, scale: .5f, noDust: true, rot: RedeHelper.RandomRotation(), tex: "Redemption/Textures/SwordClash");

                    Projectile.localAI[1] += target.damage;
                    CombatText.NewText(Projectile.getRect(), Color.Orange, target.damage, true, true);

                    if (Projectile.localAI[1] >= 100)
                    {
                        player.AddBuff(BuffType<ErhanCrossCooldown>(), 1800);
                        DustHelper.DrawCircle(target.Center, DustID.GoldFlame, 2, 4, 4, 1, 3, nogravity: true);
                        Projectile.Kill();
                    }

                    if (target.velocity.Length() <= 1 || ProjectileID.Sets.CultistIsResistantTo[target.type])
                    {
                        target.Kill();
                        continue;
                    }
                    if (target.hostile)
                    {
                        target.friendly = true;
                        target.hostile = false;
                    }

                    target.Redemption().ReflectDamageIncrease = 4;
                    target.velocity = -target.velocity;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 0.15f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
