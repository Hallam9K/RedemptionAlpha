using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class CorruptedDoubleRifle : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ThunderS);
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Converts normal bullets into high velocity bullets\n" +
                "(3[i:" + ModContent.ItemType<EnergyPack>() + "]) Every 3rd shot fires a small laser beam if an Energy Pack is in your inventory, dealing " + ElementID.ThunderS + " damage\n" +
                "33% chance not to consume ammo"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 34;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
            Item.ExtraItemShoot(ModContent.ProjectileType<CorruptedDoubleRifle_Beam>());
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= .33f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ProjectileID.BulletHighVelocity;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DoubleRifle>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 4)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 3)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        private int shotCount;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (shotCount++ >= 2 && player.GetModPlayer<EnergyPlayer>().statEnergy >= 3)
            {
                player.GetModPlayer<EnergyPlayer>().statEnergy -= 3;
                SoundEngine.PlaySound(CustomSounds.PlasmaShot, player.position);
                Projectile.NewProjectile(source, position + RedeHelper.PolarVector(2, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), velocity / 60, ModContent.ProjectileType<CorruptedDoubleRifle_Beam>(), damage, knockback, player.whoAmI);
                shotCount = 0;
            }
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(8, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(2, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}
