using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Weapons.HM.Ammo;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class CorruptedDAN : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Corrupted D.A.N");
            Tooltip.SetDefault("Fires two blasts of rockets per use\n" +
                "Continuing to hold left-click will spin the weapon while firing, creating a spiral of homing rockets\n" +
                "\n(15[i:" + ModContent.ItemType<EnergyPack>() + "]) Continuing to hold left-click while aiming downwards will charge a red beam that'll cause eruptions on impact\n" +
                "66% chance to not consume ammo, 90% chance during the homing rocket spiral");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 104;
            Item.height = 40;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CustomSounds.ShotgunBlast1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DAN_Rocket>();
            Item.shootSpeed = 10;
            Item.useAmmo = AmmoID.Rocket;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<CorruptedDAN_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DAN>())
                .AddIngredient(ModContent.ItemType<OmegaBattery>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 8)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
