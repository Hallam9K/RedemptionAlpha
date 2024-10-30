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
            SyncRedePlayer
        }
    }
}
