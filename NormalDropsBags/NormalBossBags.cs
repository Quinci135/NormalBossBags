using System;
using Terraria;
using TerrariaApi.Server;

namespace NormalBossBags
{
    [ApiVersion(2, 1)]
    public class NormalBossBags : TerrariaPlugin
    {

        public override string Author => "Quinci";

        public override string Description => "Makes normal mode bosses drop treasure bags.";

        public override string Name => "NormalBossBags";

        public override Version Version => new Version(1, 0, 0, 0);

        public NormalBossBags(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnDropLoot);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKill);
                ServerApi.Hooks.NpcLootDrop.Deregister(this, OnDropLoot);
            }
            base.Dispose(disposing);
        }

        private void OnNpcKill(NpcKilledEventArgs eventArgs)
        {
            if ((eventArgs.npc.boss || eventArgs.npc.netID == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.npc.netID != Terraria.ID.NPCID.CultistBoss)
            {
                switch (eventArgs.npc.netID)
                {
                    case Terraria.ID.NPCID.HallowBoss: //eol 636
                        if (eventArgs.npc.AI_120_HallowBoss_IsGenuinelyEnraged())
                        {
                            Terraria.Item.NewItem((int)eventArgs.npc.position.X, (int)eventArgs.npc.position.Y, (int)eventArgs.npc.Size.X, (int)eventArgs.npc.Size.Y, Terraria.ID.ItemID.EmpressBlade, 1);//5005 TerraPrisma
                        }
                        eventArgs.npc.DropItemInstanced(eventArgs.npc.position, eventArgs.npc.Size, Terraria.ID.ItemID.FairyQueenBossBag); //4782 eol boss bag
                        return;
                    case Terraria.ID.NPCID.QueenSlimeBoss: //queen slime 657
                        eventArgs.npc.DropItemInstanced(eventArgs.npc.position, eventArgs.npc.Size, Terraria.ID.ItemID.QueenSlimeBossBag); //4957 queen slime boss bag
                        return;
                    default:
                        eventArgs.npc.DropBossBags();
                        return;
                }
                
            }
        }
        private void OnDropLoot(NpcLootDropEventArgs eventArgs)
        {
            if ((Terraria.Main.npc[eventArgs.NpcArrayIndex].boss || eventArgs.NpcId == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.NpcId != Terraria.ID.NPCID.CultistBoss)
            {
                eventArgs.Handled = true;
            }
        }
    }
}
