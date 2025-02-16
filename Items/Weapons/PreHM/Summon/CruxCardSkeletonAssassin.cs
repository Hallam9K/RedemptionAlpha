using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardSkeletonAssassin : BaseCruxCard
    {
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[1] { NPCType<SkeletonAssassin_SS>() };
            SoulCost = 10;
            SpiritHealth = new int[1] { 116 };
            SpiritDefense = new int[1] { 8 };
            SpiritDamage = new int[1] { 23 };
            SpawnDustSize = 1.5f;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 33, 0);
        }
        public override void SpawnSpirits(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
            }
        }
    }
}