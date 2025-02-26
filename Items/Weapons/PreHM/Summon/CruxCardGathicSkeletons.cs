using Redemption.BaseExtension;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardGathicSkeletons : BaseCruxCard
    {
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[2] { NPCType<SkeletonWanderer_SS>(), NPCType<SkeletonDuelist_SS>() };
            SoulCost = 10;
            //MoveCost = 2;
            SpiritHealth = new int[2] { 116, 124 };
            SpiritDefense = new int[2] { 8, 8 };
            SpiritDamage = new int[2] { 20, 22 };
            SpawnDustSize = 1.5f;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.Redemption().CanSwordClash = true;
        }
        public override void SpawnSpirits(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 1);
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 1);
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[1]);
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[1]);
            }
        }
    }
}