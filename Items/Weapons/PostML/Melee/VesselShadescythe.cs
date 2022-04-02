using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent.Creative;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Projectiles.Melee;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class VesselShadescythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Inflicts soulless" +
                "\nMelee swings deal double damage" +
                "\nRight-clicking is a normal swing"); // TODO: Make similar to Blind Justice
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 520;
            Item.DamageType = DamageClass.Melee;
            Item.width = 84;
            Item.height = 80;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 55, 0, 0);
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VesselScythe_Proj>();
            Item.shootSpeed = 10;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanShoot(Player player)
        {
            if (player.altFunctionUse == 2)
                return false;
            return true;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
            if (target.life <= 0 && modPlayer.shadowBinder && modPlayer.shadowBinderCharge < 100)
                modPlayer.shadowBinderCharge += 1;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            damage *= 2;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            string text;
            if (modPlayer.shadowBinder)
                text = "Enemies slain by melee swings grant you an additional Shadowbound Soul";
            else
                text = "Has a special ability if Sielukaivo Shadowbinder is equipped";
            TooltipLine line = new(Mod, "text", text) { OverrideColor = Color.DarkGray };
            tooltips.Insert(tooltipLocation, line);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VesselFragment>(), 34)
                .AddIngredient(ModContent.ItemType<Shadesoul>(), 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}