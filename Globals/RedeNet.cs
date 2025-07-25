using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeNet
    {
        public enum ModMessageType : byte
        {
            BossSpawnFromClient,
            NPCSpawnFromClient,
            SpawnNPCFromClient,
            SpawnTrail,
            StartFowlMorning,
            FowlMorningData,
            SyncRedeQuestFromClient,
            SyncRedeWorldFromClient,
            SyncAlignment,
            SyncChaliceDialogue,
            TitleCardFromServer,
            SyncRedePlayer,
            RequestArena,
            SyncArena,
            RequestSyncArena,
        }
        public static void SpawnNPCFromClient(int npcType, Vector2 pos, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.SpawnNPCFromClient, 24);
            packet.Write(npcType);
            packet.Write((int)pos.X);
            packet.Write((int)pos.Y);
            packet.Write(ai0);
            packet.Write(ai1);
            packet.Write(ai2);
            packet.Send();
        }
        public static void BossSpawnFromClient(int npcType, Vector2 pos)
        {
            ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.BossSpawnFromClient, 12);
            packet.Write(npcType);
            packet.Write((int)pos.X);
            packet.Write((int)pos.Y);
            packet.Send();
        }
    }
}
