using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using Redemption.Projectiles.Magic;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class ElderWoodStaff : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS, ElementID.PoisonS);
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            ElementID.ItemNature[Type] = true;
            ElementID.ItemPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.UseSound = SoundID.DD2_DarkMageCastHeal;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
            Item.shoot = ProjectileType<AncientPixie_Proj>();
            Item.shootSpeed = 1f;

            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ProjectileType<ElderWoodStaff_Proj>();
        }
        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                bool isShot = false;
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI || proj.type != ProjectileType<AncientPixie_Proj>())
                        continue;

                    if (proj.ai[2] > 0)
                    {
                        isShot = true;
                        break;
                    }
                }
                return player.altFunctionUse == 2 && !isShot;
            }
            return player.altFunctionUse != 2;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0, 1);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ElderWood>(15)
            .AddIngredient(ItemID.Emerald)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
    public class ElderWoodStaff_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/ElderWoodStaff";
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        private Vector2 vector;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            vector = new Vector2(-2 * player.direction, 2) + RedeHelper.OffsetWithRotation(Projectile.rotation, 16 * player.direction, -10);

            Vector2 playerCenter = player.MountedCenter;
            Projectile.Center = playerCenter + vector;

            Projectile.spriteDirection = player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - (1.2f * player.direction)));

            if (Projectile.localAI[0] == 1)
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - (1.2f * player.direction) + ((Projectile.localAI[1] / 14) * player.direction));

                return;
            }
            float shootingSpeed = 1 / player.GetAttackSpeed(DamageClass.Magic);
            int useTime = (int)(player.inventory[player.selectedItem].useTime * shootingSpeed);

            Projectile.ai[1]++;
            if (Projectile.ai[2] == 1)
            {
                if (Main.MouseWorld.X < player.Center.X)
                    player.direction = -1;
                else
                    player.direction = 1;

                Projectile.rotation = MathHelper.Lerp(0 * player.direction, MathHelper.PiOver4 * player.direction, EaseFunction.EaseCubicOut.Ease(Projectile.ai[1] / useTime));

                if (Projectile.ai[1] >= useTime - 2)
                {
                    Projectile.localAI[2] -= 2;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - (1.2f * player.direction) + (Projectile.ai[0] / 14));
                }
                else
                {
                    Projectile.localAI[2] = -8;
                }
                if (Projectile.ai[1] >= useTime)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item101, Projectile.position);

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(player.position + new Vector2(-50 + (20 * player.direction), -50 + 40), player.width + 100, player.height + 100, DustType<DustSpark2>(), newColor: Color.LightGreen, Scale: 2f);
                        Main.dust[dust].velocity = RedeHelper.PolarVector(4, (Main.MouseWorld - player.Center).ToRotation());
                        Main.dust[dust].noGravity = true;
                    }

                    foreach (Projectile proj in Main.ActiveProjectiles)
                    {
                        if (proj.owner != player.whoAmI || proj.type != ProjectileType<AncientPixie_Proj>())
                            continue;

                        proj.ai[2] = 1;
                        proj.netUpdate = true;
                    }

                    Projectile.localAI[0] = 1;
                    Projectile.timeLeft = 30;
                }
            }
            else
            {
                Projectile.rotation = MathHelper.Lerp(MathHelper.PiOver4 * player.direction, -MathHelper.PiOver4 * player.direction, EaseFunction.EaseCubicOut.Ease(Projectile.ai[1] / useTime));
                if (Projectile.ai[1] >= useTime - 10)
                {
                    Projectile.localAI[1] += 2;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - (1.2f * player.direction) + ((Projectile.localAI[1] / 14) * player.direction));
                }
                if (Projectile.ai[1] >= useTime)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item101, Projectile.position);

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(player.position + new Vector2(-50, -50 + 40), player.width + 100, player.height + 100, DustType<DustSpark2>(), newColor: Color.LightGreen, Scale: 2f);
                        Main.dust[dust].velocity.Y = -2;
                        Main.dust[dust].velocity.X = 0;
                        Main.dust[dust].noGravity = true;
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 spawnPos = playerCenter + RedeHelper.PolarVector(60, MathHelper.ToRadians(45 * i));
                        int pixieType = Main.rand.Next(2);

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, Vector2.Zero, ProjectileType<AncientPixie_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i, pixieType);
                    }

                    Projectile.localAI[0] = 1;
                    Projectile.timeLeft = 30;
                }
            }
        }

        Asset<Texture2D> glowTex;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            glowTex = Request<Texture2D>(Texture + "_Glow");
            Vector2 origin = new(texture.Width() / 2f - (10 * player.direction), texture.Height() / 2f + 10);
            Vector2 position = Projectile.Center + new Vector2(Projectile.localAI[2] * player.direction, Projectile.localAI[1]) - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;

            Main.EntitySpriteDraw(texture.Value, position, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTex.Value, position, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}