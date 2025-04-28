using Redemption.BaseExtension;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardAnglonSkeletons : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[3] { NPCType<SkeletonWarden_SS>(), NPCType<SkeletonFlagbearer_SS>(), NPCType<SkeletonNoble_SS>() };
            SoulCost = 12;
            SpiritHealth = new int[3] { 120, 92, 144 };
            SpiritDefense = new int[3] { 11, 9, 15 };
            SpiritDamage = new int[3] { 18, 10, 28 };
            SpawnDustSize = 1.5f;
            Item.knockBack = 9;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 2, 66, 0);
            Item.Redemption().TechnicallyAxe = true;
        }
        public override void SpawnSpirits(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 1);
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 2);
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[1]);
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[2]);
            }
        }
    }
}