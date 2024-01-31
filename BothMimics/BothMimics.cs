using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;


namespace BothMimics
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "Mariothedog, updated by Kuz_";

        public override string Description => "Lets you summon both crimosn and corruption mimics with commands";

        public override string Name => "Both Evil Biome Mimics";

        public override Version Version => new(1, 1, 0, 0);

        public Plugin(Main game) : base(game) {}


        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.mimic.corrupt", TradeForCorruptMimic, "corruptmimic"));
            Commands.ChatCommands.Add(new Command("tshock.mimic.crimson", TradeForCrimsonMimic, "crimsonmimic"));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool TryTradeForEvilMimic(TSPlayer tSPlayer, int npcID)
        {
            int keyOfNightIndex = -1;
            Item keyOfNightItem = null;
            for (int i = 0; i < NetItem.InventorySlots; i++)
            {
                Item item = tSPlayer.TPlayer.inventory[i];
                if (item.type == ItemID.NightKey)
                {
                    keyOfNightIndex = i;
                    keyOfNightItem = item;
                    break;
                }
            }

            if (keyOfNightItem == null)
            {
                tSPlayer.SendErrorMessage("You do not have a key of night in your inventory!");
                return false;
            }

            keyOfNightItem.stack -= 1;
            if (keyOfNightItem.stack == 0)
            {
                keyOfNightItem.netDefaults(0);
            }
            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, tSPlayer.Index, keyOfNightIndex);

            NPC npc = TShock.Utils.GetNPCById(npcID);
            bool foundValidTile = false;
            var validTileX = 0;
            var validTileY = 0;
            while (!foundValidTile)
            {
                TShock.Utils.GetRandomClearTileWithInRange(tSPlayer.TileX, tSPlayer.TileY, 50, 30, out var tileX, out var tileY);
                if (Math.Abs(tSPlayer.TileX - tileX) > 5)
                {
                    foundValidTile = true;
                    validTileX = tileX;
                    validTileY = tileY;
                }
            }
            
            NPC.NewNPC(new Terraria.DataStructures.EntitySource_DebugCommand(), validTileX * 16, validTileY * 16, npcID);
            return true;
        }
        
        private void TradeForCorruptMimic(CommandArgs args)
        {
            TSPlayer tSPlayer = args.Player;
            if (tSPlayer == null) return;

            if (!TryTradeForEvilMimic(tSPlayer, NPCID.BigMimicCorruption)) return;

            tSPlayer.SendSuccessMessage("A corrupt mimic has been spawned!");
        }

        private void TradeForCrimsonMimic(CommandArgs args)
        {
            TSPlayer tSPlayer = args.Player;
            if (tSPlayer == null) return;

            if (!TryTradeForEvilMimic(tSPlayer, NPCID.BigMimicCrimson)) return;

            tSPlayer.SendSuccessMessage("A crimson mimic has been spawned!");
        }
    }
}
