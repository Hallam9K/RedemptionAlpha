using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Projectiles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.Audio;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class SkullDiggerFlail : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ArcaneS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger's Skull Digger");
            /* Tooltip.SetDefault("Spinning the weapon around you will conjure " + ElementID.ArcaneS + " mirages\n" +
                "'Yes, he did name his weapon after himself...'"); */

            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.scale = 1f;
            Item.noUseGraphic = true;
            Item.width = 36;
            Item.height = 34;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SkullDiggerFlail_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<SkullDigger_FlailBlade_ProjF>());
            Item.shootSpeed = 32f;
        }
    }

    public class SkullDiggerFlail_Proj : Flail
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_FlailBlade";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger's Skull Digger");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        public override void SetStats(ref int throwTime, ref float throwSpeed, ref float recoverDistance, ref float recoverDistance2, ref int attackCooldown)
        {
            throwTime = 13;
            throwSpeed = 18f;
            recoverDistance = 22f;
            recoverDistance2 = 48f;
            attackCooldown = 15;
        }

        private int timer;
        private int soundTimer;
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = (player.Center - Projectile.Center).ToRotation() + (float)Math.PI / 2;
            if (Projectile.ai[0] == 0 && soundTimer++ % 30 == 0 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChainSwing with { Volume = .5f }, Projectile.position);

            if (Projectile.ai[0] != 0 && Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(CustomSounds.ChainSwing, Projectile.position);
                Projectile.localAI[0] = 1;
            }

            if (Projectile.ai[0] == 0)
            {
                if (++timer % 30 == 0 && Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<SkullDigger_FlailBlade_ProjF>(), Projectile.damage, 0, Projectile.owner, Projectile.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Chain").Value;
            Vector2 HeadPos = player.MountedCenter;
            Rectangle sourceRectangle = new(0, 0, chainTexture.Width, chainTexture.Height);
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            float num1 = chainTexture.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            var effects = player.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Main.EntitySpriteDraw(chainTexture, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), Projectile.GetAlpha(lightColor), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
