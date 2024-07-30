using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.HM
{
    public class OmegaPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.damage = 46;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 56;
            Item.useTime = 5;
            Item.useAnimation = 16;
            Item.pick = 210;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.tileBoost += 1;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}