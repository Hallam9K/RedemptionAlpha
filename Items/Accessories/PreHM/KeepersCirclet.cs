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
            // DisplayName.SetDefault("Keeper's Circlet");
            // Tooltip.SetDefault("Humanoid skeletons become friendly");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.hasVanityEffects = true;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().skeletonFriendly = true;
        }
    }
}
