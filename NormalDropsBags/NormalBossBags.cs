using System;
using Terraria;
using TerrariaApi.Server;

namespace NormalBossBags
{
    [ApiVersion(2, 1)]
    public class NormalBossBags : TerrariaPlugin
    {
        // Also see tshock's skeleton plugin https://tshock.readme.io/docs/hello-world

        public override string Author => "Quinci";

        public override string Description => "Makes normal mode bosses drop treasure bags.";

        public override string Name => "NormalBossBags";

        public override Version Version => new Version(1, 0, 0, 0);

        public NormalBossBags(Main game) : base(game) 
        {

        }

        public override void Initialize()
        {
            // Register hooks from terraria server api (tshock's patched version of the server and open terraria api/otapi)
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnDropLoot);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove hooks from their respective hook collections for garbage collection (IDisposable). Currently not very useful as this is used when the server is shut down
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKill);
                ServerApi.Hooks.NpcLootDrop.Deregister(this, OnDropLoot);
            }
            base.Dispose(disposing);
        }

        private void OnNpcKill(NpcKilledEventArgs eventArgs)
        {
                //Check if it's a boss or betsy, if its normal mode, and if it is not the lunatic cultist (it is not supposed to drop a treasure bag in any circumstances, unobtainable)
            if ((eventArgs.npc.boss || eventArgs.npc.netID == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.npc.netID != Terraria.ID.NPCID.CultistBoss)
            {
                switch (eventArgs.npc.netID) //Check for the npc type. Eol & queen slime have weird drop mechanics so I'm replacing it
                {
                    case Terraria.ID.NPCID.HallowBoss: //eol 636
                        if (eventArgs.npc.AI_120_HallowBoss_IsGenuinelyEnraged()) // Checks if it's the daytime eol and drop terraprisma if so
                        {
                            Terraria.Item.NewItem((int)eventArgs.npc.position.X, (int)eventArgs.npc.position.Y, (int)eventArgs.npc.Size.X, (int)eventArgs.npc.Size.Y, Terraria.ID.ItemID.EmpressBlade, 1);//5005 TerraPrisma
                        } //DropItemInstanced() will tell each client there's an item but will not be an active item slot on the server so it will not be overwritten thus each client can collect the item
                        eventArgs.npc.DropItemInstanced(eventArgs.npc.position, eventArgs.npc.Size, Terraria.ID.ItemID.FairyQueenBossBag); //4782 eol boss bag
                        return;
                    case Terraria.ID.NPCID.QueenSlimeBoss: //queen slime 657
                        eventArgs.npc.DropItemInstanced(eventArgs.npc.position, eventArgs.npc.Size, Terraria.ID.ItemID.QueenSlimeBossBag); //4957 queen slime boss bag
                        return;
                    default:
                        eventArgs.npc.DropBossBags(); // Drops a treasure bag depending on npc type (most bosses)
                        return;
                }
                
            }
        }
        private void OnDropLoot(NpcLootDropEventArgs eventArgs)
        {
            // Same conditions as above
            if ((Terraria.Main.npc[eventArgs.NpcArrayIndex].boss || eventArgs.NpcId == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.NpcId != Terraria.ID.NPCID.CultistBoss)
            {
                eventArgs.Handled = true; // //Prevents the game from processing this event. This stops npcs from dropping loot with the NPCLootOld() method. Coins, hearts, mana stars, and health potions use a differnet method so they retain normal behavior. Loot is cancelled as we are replacing it with treasure bags.
            }
        }
    }
}
