using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Terraria;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class PureIronPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure-Iron Pickaxe");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 18;
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
            if (player.GetModPlayer<BuffPlayer>().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronBar>(), 14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}