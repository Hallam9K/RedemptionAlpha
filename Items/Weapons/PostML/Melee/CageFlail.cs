using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles;
using Redemption.Projectiles.Misc;
using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class CageFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cage Crusher");
            Tooltip.SetDefault("Hitting an enemy once per use will cause echos to appear and fight for you" +
                "\nThe cage deals increased damage to enemies with less knockback resistance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 158;
            Item.width = 46;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 14, 0, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CageFlail_Ball>();
            Item.shootSpeed = 32f;
            Item.UseSound = CustomSounds.ChainSwing;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = false;
            Item.channel = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Damage")
                {
                    string[] strings = line.Text.Split(' ');
                    int dmg = int.Parse(strings[0]);
                    dmg *= 2;
                    line.Text = dmg + "";
                    for (int i = 1; i < strings.Length; i++)
                    {
                        line.Text += " " + strings[i];
                    }
                }
            }
        }
    }
    public class CageFlail_Ball : Flail
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cage Crusher");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void SetStats(ref int throwTime, ref float throwSpeed, ref float recoverDistance, ref float recoverDistance2, ref int attackCooldown)
        {
            throwTime = 18;
            throwSpeed = 18f;
            recoverDistance = 32f;
            recoverDistance2 = 36f;
            attackCooldown = 15;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] != 0 && Projectile.localAI[0] == 1)
            {
                for (int i = 0; i < Main.rand.Next(6, 14); i++)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(3, 14), Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<Echo_Friendly>(), Projectile.damage, 0, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Zombie81, Projectile.position);
                Projectile.localAI[0] = 2;
            }
        }
        private int soundTimer;
        public override void PostAI()
        {
            if (Projectile.ai[0] == 0 && soundTimer++ % 30 == 0 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChainSwing with { Volume = .5f }, Projectile.position);

            if (Projectile.ai[0] != 0 && Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(CustomSounds.ChainSwing, Projectile.position);
                Projectile.localAI[0] = 1;
            }
            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) + 1.57f;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= (int)(target.knockBackResist + 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PostML/Melee/CageFlail_Chain").Value;
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
