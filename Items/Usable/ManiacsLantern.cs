using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ManiacsLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Maniac's Lantern");
            Tooltip.SetDefault("When held, creates an invisible force that repels soulless enemies away from you");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ManiacsLantern_Proj>()] == 0)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Item(Item), new Vector2(player.Center.X + (40 * player.direction), player.Center.Y - 40), Vector2.Zero, ModContent.ProjectileType<ManiacsLantern_Proj>(), 0, 0, player.whoAmI);
            }
        }
    }
}
