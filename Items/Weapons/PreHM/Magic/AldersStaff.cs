using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Bosses.Thorn;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class AldersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CursedGrassBlade>();
            Item.staff[Item.type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.damage = 23;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 3;
            Item.width = 46;
            Item.height = 44;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.UseSound = SoundID.Item42;
            Item.value = Item.buyPrice(0, 0, 44, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArcaneBolt>();
            Item.shootSpeed = 0f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<AldersStaff_Proj>();
        }
    }
    public class AldersStaff_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/AldersStaff";
        public override void SetDefaults()
        {
            Projectile.width = 46;
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
        private bool swap;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            vector = new Vector2(6 * player.direction, -20 + Projectile.ai[0]);

            Vector2 playerCenter = player.MountedCenter;
            Projectile.Center = playerCenter + vector;

            Projectile.spriteDirection = player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (playerCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            int dustIndex = Dust.NewDust(new Vector2(player.position.X, player.Bottom.Y - 2), player.width, 2, DustID.MagicMirror);
            Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
            Main.dust[dustIndex].velocity.X = 0;
            Main.dust[dustIndex].noGravity = true;

            Projectile.rotation = ((float)Math.Sin(Projectile.localAI[0]++ / 20) / 6) + (player.direction == -1 ? .5f : -.5f);

            float shootingSpeed = 1 / player.GetAttackSpeed(DamageClass.Magic);

            if (Projectile.ai[1]++ > 0 && Main.myPlayer == Projectile.owner)
            {
                int useTime = (int)(player.inventory[player.selectedItem].useTime * shootingSpeed);
                if (!player.channel && Projectile.ai[1] >= useTime * 2)
                    Projectile.Kill();
                else
                {
                    if (Projectile.ai[1] % (useTime / 4) == 0 && Projectile.ai[1] <= useTime)
                    {
                        if (BasePlayer.ReduceMana(player, player.inventory[player.selectedItem].mana))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Saint9 with { Volume = .2f, Pitch = -.1f }, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + RedeHelper.OffsetWithRotation(Projectile, 17, -14), RedeHelper.PolarVector(Main.rand.Next(6, 11), -MathHelper.PiOver2 + Main.rand.NextFloat(-0.3f, .3f)), ModContent.ProjectileType<ArcaneBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    if (Projectile.ai[1] >= useTime * 2)
                        Projectile.ai[1] = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Vector2 origin = new(texture.Width() / 2f - (7 * player.direction), texture.Height() / 2f + 8);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}