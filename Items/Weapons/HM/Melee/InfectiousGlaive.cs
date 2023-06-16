using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using Redemption.Items.Materials.HM;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Weapons.PreHM.Melee;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class InfectiousGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Infectious Glaive");
            //Tooltip.SetDefault("Fires a spread of xenomite shards every two swings");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 76;
            Item.height = 82;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 4, 50);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 78;
            Item.crit = 16;
            Item.knockBack = 6f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<InfectiousGlaive_Proj>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        private bool side;
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (side)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Swing1, player.position);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0);
            }
            else
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Swoosh1, player.position);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
            }
            side = !side;
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenoXyston>())
                .AddIngredient(ModContent.ItemType<Xenomite>(), 8)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
