using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class Earthbind : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Holding left-click will channel a bind at cursor point, stunning any enemies caught in it\n" +
                "The bind is less effective on targets with high knockback resistance"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Mistfall>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 20;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.channel = true;
            Item.value = Item.sellPrice(0, 0, 72, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<Earthbind_Proj>();
            Item.UseSound = SoundID.Item69;
        }
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
    }
}