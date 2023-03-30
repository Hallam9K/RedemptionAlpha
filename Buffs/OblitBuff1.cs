using Redemption.Items.Accessories.PostML;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class OblitBuff1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliteration Motivation");
            // Description.SetDefault("Increased damage and defense, reduced dash cooldown, less life regen");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 15)
                player.lifeRegen = 15;

            player.GetModPlayer<ObliterationDashPlayer>().MotivationStack = 1;
            player.GetDamage<GenericDamageClass>() += 0.03f;
            player.statDefense += 2;
            if (player.HasBuff(ModContent.BuffType<OblitBuff2>()) || player.HasBuff(ModContent.BuffType<OblitBuff3>()) || player.HasBuff(ModContent.BuffType<OblitBuff4>()) || player.HasBuff(ModContent.BuffType<OblitBuff5>()))
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class OblitBuff2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliteration Motivation");
            // Description.SetDefault("Increased damage and defense, reduced dash cooldown, less life regen");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 10)
                player.lifeRegen = 10;

            player.GetModPlayer<ObliterationDashPlayer>().MotivationStack = 2;
            player.GetDamage<GenericDamageClass>() += 0.06f;
            player.statDefense += 4;
            if (player.HasBuff(ModContent.BuffType<OblitBuff3>()) || player.HasBuff(ModContent.BuffType<OblitBuff4>()) || player.HasBuff(ModContent.BuffType<OblitBuff5>()))
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class OblitBuff3 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliteration Motivation");
            // Description.SetDefault("Increased damage and defense, reduced dash cooldown, less life regen");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 5)
                player.lifeRegen = 5;

            player.GetModPlayer<ObliterationDashPlayer>().MotivationStack = 3;
            player.GetDamage<GenericDamageClass>() += 0.09f;
            player.statDefense += 6;
            if (player.HasBuff(ModContent.BuffType<OblitBuff4>()) || player.HasBuff(ModContent.BuffType<OblitBuff5>()))
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class OblitBuff4 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliteration Motivation");
            // Description.SetDefault("Increased damage and defense, reduced dash cooldown, less life regen");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 2)
                player.lifeRegen = 2;

            player.GetModPlayer<ObliterationDashPlayer>().MotivationStack = 4;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.statDefense += 8;
            if (player.HasBuff(ModContent.BuffType<OblitBuff5>()))
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class OblitBuff5 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliteration Motivation");
            // Description.SetDefault("Increased damage and defense, reduced dash cooldown, less life regen");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.GetModPlayer<ObliterationDashPlayer>().MotivationStack = 5;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.statDefense += 10;
        }
    }
}