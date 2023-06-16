using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Tools.PostML
{
    public class NanoPickaxe : ModItem
	{
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Can mine Black Hardened Sludge");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Melee;
			Item.width = 52;
			Item.height = 58;
			Item.useTime = 4;
			Item.useAnimation = 11;
			Item.pick = 300;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(2, 0, 0, 0);
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item15;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.tileBoost += 3;
			if (!Main.dedServ)
				Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
		}
	}
}
