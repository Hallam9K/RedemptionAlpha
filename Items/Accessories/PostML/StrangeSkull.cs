using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class StrangeSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mysterious Skull");
            // Tooltip.SetDefault("Summons a certain spooky skeleton");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HamSandwich>();
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<TiedPet>(), ModContent.BuffType<TiedPetBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 5);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}