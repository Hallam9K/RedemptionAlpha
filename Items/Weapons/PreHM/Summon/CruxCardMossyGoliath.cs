using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardMossyGoliath : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemEarth[Type] = true;
            ElementID.ItemPoison[Type] = true;
            ElementID.ItemWind[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            BossCard = true;
            SpiritTypes = new int[1] { NPCType<MossyGoliath_SS>() };
            SoulCost = 30;
            //MoveCost = 5;
            SpiritHealth = new int[1] { 2000 };
            SpiritDefense = new int[1] { 14 };
            SpiritDamage = new int[1] { 40 };
            SpawnDustSize = 4;
            Item.knockBack = 8;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 9, 33, 0);
        }
        public override void SpawnSpirits(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 20, 0);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
        }
    }
}