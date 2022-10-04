using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Terraria;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;

namespace Redemption.Items.Tools.PreHM
{
    public class PureIronPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure-Iron Pickaxe");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 14;
            Item.useAnimation = 22;
            Item.pick = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = 1200;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}