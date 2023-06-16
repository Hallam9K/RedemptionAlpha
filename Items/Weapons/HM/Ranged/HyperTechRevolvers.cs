using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class HyperTechRevolvers : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hyper-Tech Revolvers");
            /* Tooltip.SetDefault("Right-click to toss one in the air, catching it gives a stackable fire rate boost\n" +
                "Missing the catch will cause you to only shoot one gun for 5 seconds\n" +
                "Replaces normal bullets with nano bullets"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 50;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && (player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj2>()] > 0 || player.HasBuff<RevolverTossDebuff>()))
                return false;
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<HyperTechRevolvers_Proj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj2>()] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item7, player.Center);
                Projectile.NewProjectile(source, position, new Vector2(Main.rand.NextFloat(-3, 3), -10), ModContent.ProjectileType<HyperTechRevolvers_Proj2>(), 0, 0, player.whoAmI, -player.direction);
                return false;
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj2>()] == 0)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
            return true;
        }
    }
}
