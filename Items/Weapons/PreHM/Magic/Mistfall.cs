using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class Mistfall : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mistfall");
            // Tooltip.SetDefault("Lowers the air temperature at cursor point, slowing enemies caught in the mist");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Earthbind>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 8;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 0, 78, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<Icefall_Mist>();
            if (Main.netMode != NetmodeID.Server)
                Item.UseSound = CustomSounds.IceMist;
        }
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            velocity = new Vector2(Main.rand.NextFloat(-2, 2), 0);
        }
    }
}