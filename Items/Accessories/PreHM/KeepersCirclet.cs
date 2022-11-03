using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Face)]
    public class KeepersCirclet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keeper's Circlet");
            Tooltip.SetDefault("Humanoid skeletons become friendly");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().skeletonFriendly = true;
        }
    }
}
