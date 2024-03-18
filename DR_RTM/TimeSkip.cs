using System;
using System.Diagnostics;
using System.Dynamic;
using System.Timers;
using ReadWriteMemory;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace DR_RTM
{
    public static class TimeSkip
    {
        public static double TimerInterval = 16.666666666666668;

        public static System.Timers.Timer UpdateTimer = new System.Timers.Timer(TimerInterval);

        public static Process GameProcess;

        public static Form1 form;

        private static ReadWriteMemory.ProcessMemory gameMemory;

        public static Dictionary<string, int[]> TimeskipOrder = new Dictionary<string, int[]>() { };

        public static List<string> TunnelsMissions = new() { "Tunnels", "Tank", "Brock" };

        public static List<string> OvertimeMissions = new() { "Supplies", "Queens", "Tunnels", "Tank", "Brock" };

        public static List<string> PsychoSkips = new() { "Kent 3", "Convicts", "Sean", "Snipers", "Adam", "Jo", "Paul", "Cletus", "Cliff" };

        public static long TicksElapsed;

        public static long BeforeMovementLockedTicks;

        public static long MovementLockedTicks;

        public static byte MissionByte1;
        public static byte MissionByte2;
        public static byte MissionByte3;
        public static byte MissionByte4;
        public static byte MissionByte5;
        public static byte MissionByte6;
        public static byte MissionByte7;
        public static byte MissionByte8;
        public static byte MissionByte9;

        public static byte Carlito2Death;

        public static byte CultistsByte;


        private static IntPtr gameTimePtr;

        private static IntPtr DeadRisingPtr;

        private static IntPtr cutsceneIDPtr;

        private static IntPtr cutsceneOnLoadPtr;

        private static IntPtr cGametaskPtr;

        private static IntPtr caseMenuStatePtr;

        private static IntPtr saveMenuStatePtr;

        private static IntPtr WatchCaseDisplayPtr;

        private static IntPtr PPRewardsPtr;

        private static IntPtr campaignProgressPtr;

        private static IntPtr SpawnBossesPtr;

        private static IntPtr TeleportPtr;

        public static string SelectedCategory = "Timeskip";

        private const int gameTimeOffset = 408;

        public static uint cutsceneID;

        public static uint LastCutscene;

        public static uint Teleport;

        public static uint cutsceneOnLoad;

        public static byte cGametask;

        private static byte SpawnBosses;

        private static byte WatchCaseDisplay;

        private static uint gameTime;

        private static uint campaignProgress;

        private static uint Convicts1;
        private static uint Convicts2;
        private static uint Convicts3;

        private static uint Snipers1;
        private static uint Snipers2;
        private static uint Snipers3;

        private static bool inCutsceneOrLoad;

        private static int loadingRoomId;

        private static byte CletusActive;

        private static uint PPRewards;

        private static byte caseMenuState;

        private static byte saveMenuState;

        private static byte Bombs;

        private static byte JeepActivated;

        private static dynamic old = new ExpandoObject();

        public static bool startCutscene = false;

        public static bool includeOvertime = false;

        public static bool RandomizerStarted = false;

        public static int Seed;

        public static int currentSkip = 0;

        private static uint BossHealth;

        public static bool OnlyTriggerOnce = false;

        public static void Init()
        {
        }

        /// BytetoBit usage:
        /// Insert byte you want to check a bit's value
        /// Insert int of the bit's position
        /// 0 = 1's place 1 = 2's place 2 = 4's place etc.

        static bool BytetoBit(byte a, int b)
        {
            byte[] bytearray = { a };
            var bitarray = new BitArray(bytearray);
            return bitarray.Get(b);
        }

        static bool LowerProgress(int a)
        {
            if (a < TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(4))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string StringTime(uint time)
        {
            uint num = time / 108000u % 24u;
            uint num2 = time / 1800u % 60u;
            uint num3 = time / 30u % 60u;
            string text = "AM";
            if (num >= 12)
            {
                text = "PM";
                num %= 12u;
            }
            if (num == 0)
            {
                num = 12u;
            }
            return string.Format("{0}:{1}:{2} {3}", num.ToString("D2"), num2.ToString("D2"), num3.ToString("D2"), text);
        }
        public static void SeedRandomizer()
        {
            Random rand = new Random();
            int randomseed = rand.Next(0, 999999999);
            Seed = randomseed;
            Form1.localseed = Seed.ToString();
        }
        public static void Randomize()
        {
            Random rand = new Random(Seed);
            TimeskipOrder = TimeskipOrder.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);
            TimeskipOrder.Add("Ending", new int[] {52, 999, 0, 0 });
            TimeskipOrder.Add("Buffer", new int[] { });
        }

        public static void RestoreCode()
        {
            gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3171914), 26774409);
            gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3173063), 26774409);
            gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3173444), 26774409);
            gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3187063), 26774409);
        }
        public static void CreateList()
        {
            if (SelectedCategory == "Timeskip")
            {
                TimeskipOrder.Clear();
                /// Format is: Cutscene ID to be written to start mission, Cutscene ID when mission is finished, 
                /// an integer to be added to another integer to tell the game which mission is being written to the watch, any potential bytes to spawn bosses/survivors,
                /// and finally a byte to specify which pointer the boss/survivor integer needs to be written to.
                TimeskipOrder.Add("Backup For Brad", new int[] { 8, 10, 0, 0, 1});
                TimeskipOrder.Add("An Odd Old Man", new int[] { 10, 12, 0, 0, 2});
                TimeskipOrder.Add("A Temporary Agreement", new int[] { 12, 13, 0, 3});
                TimeskipOrder.Add("Rescue The Professor", new int[] { 15, 17, 1, 4});
                TimeskipOrder.Add("Medicine Run", new int[] { 17, 24, 1, 5});
                TimeskipOrder.Add("Girl Hunting", new int[] { 25, 27, 3, 6});
                TimeskipOrder.Add("A Promise To Isabella", new int[] { 27, 31, 4, 7});
                TimeskipOrder.Add("Transporting Isabella", new int[] { 31, 33, 4, 8});
                TimeskipOrder.Add("Bomb Collector", new int[] { 37, 39, 6, 9});
                TimeskipOrder.Add("Jamming Device", new int[] { 39, 41, 7, 10});
                TimeskipOrder.Add("Hideout", new int[] { 41, 42, 7, 11});
                TimeskipOrder.Add("Jessie's Discovery", new int[] { 42, 43, 7, 12});
                TimeskipOrder.Add("The Butcher", new int[] { 43, 45, 7, 13});
                TimeskipOrder.Add("Memories", new int[] { 45, 46, 8, 14});
                if (includeOvertime == true)
                {
                    TimeskipOrder.Add("Supplies", new int[] { 135, 132 });
                    TimeskipOrder.Add("Queens", new int[] { 132, 126 });
                    TimeskipOrder.Add("Tunnels", new int[] { 126, 136 });
                    TimeskipOrder.Add("Tank", new int[] { 136, 144 });
                    TimeskipOrder.Add("Brock", new int[] { 144, 144 });
                }
            }
            else if (SelectedCategory == "Psychoskip")
            {
                TimeskipOrder.Clear();
                TimeskipOrder.Add("Convicts", new int[] { 999, 999, 243, 132, 3 });
                TimeskipOrder.Add("Cliff", new int[] { 999, 71, 64, 1, 64, 0 });
                TimeskipOrder.Add("Cletus", new int[] { 999, 72, 19, 36, 4 });
                TimeskipOrder.Add("Adam", new int[] { 999, 74, 3, 17, 1 });
                TimeskipOrder.Add("Jo", new int[] { 999, 75, 4, 210, 1 });
                TimeskipOrder.Add("Snipers", new int[] { 999, 999, 11, 77, 4 });
                TimeskipOrder.Add("Sean", new int[] { 999, 73, 2, 208, 0 });
                TimeskipOrder.Add("Paul", new int[] { 999, 76, 5, 214, 1 });
                TimeskipOrder.Add("Kent 3", new int[] { 999, 70, 0, 32, 0 });
            }
        }

        public static void UpdateEvent(object source, ElapsedEventArgs e)
        {
            if (gameMemory != null && !gameMemory.CheckProcess())
            {
                gameMemory = null;
                UpdateTimer.Enabled = false;
                return;
            }
            if (gameMemory == null)
            {
                gameMemory = new ReadWriteMemory.ProcessMemory(GameProcess);
            }
            if (!gameMemory.IsProcessStarted())
            {
                gameMemory.StartProcess();
            }
            gameTimePtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            if (gameTimePtr == IntPtr.Zero)
            {
                if (!form.IsDisposed)
                {
                    form.TimeDisplayUpdate("<missing>");
                }
                return;
            }
            old.gameTime = gameTime;
            old.campaignProgress = campaignProgress;
            old.inCutsceneOrLoad = inCutsceneOrLoad;
            old.loadingRoomId = loadingRoomId;
            old.caseMenuState = caseMenuState;
            old.cutsceneID = cutsceneID;
            old.cGametask = cGametask;
            DeadRisingPtr = gameMemory.Pointer("DeadRising.exe");
            cutsceneIDPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            cutsceneOnLoadPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            PPRewardsPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            cGametaskPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            campaignProgressPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            TeleportPtr = gameMemory.Pointer("DeadRising.exe", 26496472, 134592);
            caseMenuStatePtr = gameMemory.Pointer("DeadRising.exe", 26505152, 192600);
            saveMenuStatePtr = gameMemory.Pointer("DeadRising.exe", 30352928, 1160);
            WatchCaseDisplayPtr = gameMemory.Pointer("DeadRising.exe", 30510208);
            SpawnBossesPtr = gameMemory.Pointer("DeadRising.exe", 26496472);
            SpawnBosses = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134841));
            JeepActivated = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134826));
            caseMenuState = gameMemory.ReadByte(IntPtr.Add(caseMenuStatePtr, 386));
            saveMenuState = gameMemory.ReadByte(IntPtr.Add(saveMenuStatePtr, 9956));
            cGametask = gameMemory.ReadByte(IntPtr.Add(cGametaskPtr, 56));
            WatchCaseDisplay = gameMemory.ReadByte(IntPtr.Add(WatchCaseDisplayPtr, 8024));
            Teleport = gameMemory.ReadUInt(IntPtr.Add(TeleportPtr, 440));
            PPRewards = gameMemory.ReadUInt(IntPtr.Add(PPRewardsPtr, 33628));
            cutsceneID = gameMemory.ReadUInt(IntPtr.Add(cutsceneIDPtr, 33544));
            campaignProgress = gameMemory.ReadUInt(IntPtr.Add(campaignProgressPtr, 336));
            cutsceneOnLoad = gameMemory.ReadUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552));
            gameTime = gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 408));
            Snipers1 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 280), 4844));
            Snipers2 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 280, 16), 4844));
            Snipers3 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 280, 16, 16), 4844));
            Convicts1 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 160, 4640, 448), 4844));
            Convicts2 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 160, 4640, 416), 4844));
            Convicts3 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 160, 4640, 384), 4844));
            BossHealth = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 280), 4844));
            loadingRoomId = gameMemory.ReadInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 72));
            Bombs = gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26496472, 134592), 33933));
            CletusActive = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134845));
            inCutsceneOrLoad = (gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 112)) & 1) == 1;
            inCutsceneOrLoad = (gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 112)) & 1) == 1;
            MissionByte1 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134777));
            MissionByte2 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134778));
            MissionByte3 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134779));
            MissionByte4 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134780));
            MissionByte5 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134781));
            MissionByte6 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134782));
            MissionByte7 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134788));
            MissionByte8 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134840));
            MissionByte9 = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134841));
            Carlito2Death = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134761));
            CultistsByte = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134784));
            form.TimeDisplayUpdate(StringTime(gameTime));
            if (TimeskipOrder != null)
            {
                gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3171914), 26774272);
                gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3173063), 26774272);
                gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3173444), 26774272);
                gameMemory.WriteUInt(IntPtr.Add(DeadRisingPtr, 3187063), 26774272);
            }
            if (campaignProgress == 10)
            {
                RandomizerStarted = false;
            }
            if (RandomizerStarted == true && cGametask == 7)
            {
                if (!BytetoBit(MissionByte6, 5) && Form1.spawnEnemies == 1 || !BytetoBit(MissionByte6, 5) && Form1.spawnEnemies == 3)
                {
                    gameMemory.WriteInt(IntPtr.Add(SpawnBossesPtr, 134782), (byte)(MissionByte6 + 32));
                }
                if (!BytetoBit(CultistsByte, 3) && Form1.spawnEnemies == 2 || !BytetoBit(CultistsByte, 3) && Form1.spawnEnemies == 3)
                {
                    gameMemory.WriteInt(IntPtr.Add(SpawnBossesPtr, 134784), (byte)(CultistsByte + 8));
                }
            }
            if (SelectedCategory == "Timeskip" && Form1.TimeRandomized != DateTime.MinValue)
            {
                if (cGametask == 3)
                {
                    OnlyTriggerOnce = false;
                }
                if (cutsceneID == 8 && RandomizerStarted == false)
                {
                    RandomizerStarted = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(0));
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (RandomizerStarted == true)
                {
                    if (cutsceneID == TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(1) && cGametask == 7)
                    {
                        currentSkip++;
                        if (cutsceneID != TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(0))
                        {
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(0));
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                        }
                    }
                    if (TimeskipOrder.ElementAt(currentSkip).Key == "Ending" && loadingRoomId == 288 && gameTime < 11664000)
                    {
                        gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11664000u);
                    }
                    if (!OvertimeMissions.Contains(TimeskipOrder.ElementAt(currentSkip).Key))
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872 + TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(2));
                    }
                    /// Ones with cGametask and cutsceneID force the boss music to play
                    if (!BytetoBit(MissionByte1, 4) && TimeskipOrder.ElementAt(currentSkip).Key == "Backup For Brad" || !BytetoBit(MissionByte1, 4) && loadingRoomId == 534)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 + 16));
                    }
                    else if (BytetoBit(MissionByte1, 4) && TimeskipOrder.ElementAt(currentSkip).Key != "Backup For Brad" && loadingRoomId != 534)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 - 16));
                    }
                    if (!BytetoBit(MissionByte2, 0) && TimeskipOrder.ElementAt(currentSkip).Key == "A Temporary Agreement")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 + 1));
                    }
                    if (BytetoBit(MissionByte2, 0) && TimeskipOrder.ElementAt(currentSkip).Key != "A Temporary Agreement")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 - 1));
                    }
                    if (!BytetoBit(MissionByte3, 0) && TimeskipOrder.ElementAt(currentSkip).Key != "Medicine Run" && loadingRoomId == 1280 && cGametask == 3)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 + 1));
                    }
                    if (cGametask == 7 && OnlyTriggerOnce == true)
                    {
                        if (!BytetoBit(MissionByte1, 5) && TimeskipOrder.ElementAt(currentSkip).Key == "Backup For Brad" && cutsceneID == 9 && cGametask == 7)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 + 32));
                        }
                        if (BytetoBit(MissionByte1, 5) && TimeskipOrder.ElementAt(currentSkip).Key != "Backup For Brad")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 - 32));
                        }
                        if (!BytetoBit(MissionByte1, 6) && TimeskipOrder.ElementAt(currentSkip).Key == "An Odd Old Man")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 + 64));
                        }
                        if (BytetoBit(MissionByte1, 6) && TimeskipOrder.ElementAt(currentSkip).Key != "An Odd Old Man")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134777), (byte)(MissionByte1 - 64));
                        }
                        if (!BytetoBit(MissionByte2, 3) && TimeskipOrder.ElementAt(currentSkip).Key == "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 + 8));
                        }
                        if (BytetoBit(MissionByte2, 3) && TimeskipOrder.ElementAt(currentSkip).Key != "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 - 8));
                        }
                        if (!BytetoBit(MissionByte2, 4) && TimeskipOrder.ElementAt(currentSkip).Key == "Rescue The Professor" && cutsceneID == 16 && cGametask == 7)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 + 16));
                        }
                        if (BytetoBit(MissionByte2, 4) && TimeskipOrder.ElementAt(currentSkip).Key == "Rescue The Professor" && cutsceneID != 16)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 - 16));
                        }
                        if (!BytetoBit(MissionByte2, 4) && TimeskipOrder.ElementAt(currentSkip).Key != "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 + 16));
                        }
                        if (!BytetoBit(MissionByte2, 5) && TimeskipOrder.ElementAt(currentSkip).Key != "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 + 32));
                        }
                        if (BytetoBit(MissionByte2, 5) && TimeskipOrder.ElementAt(currentSkip).Key == "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134778), (byte)(MissionByte2 - 32));
                        }
                        if (!BytetoBit(MissionByte3, 4) && TimeskipOrder.ElementAt(currentSkip).Key == "Girl Hunting")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 + 16));
                        }
                        if (BytetoBit(MissionByte3, 4) && TimeskipOrder.ElementAt(currentSkip).Key != "Girl Hunting")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 - 16));
                        }
                        if (!BytetoBit(MissionByte3, 5) && TimeskipOrder.ElementAt(currentSkip).Key == "Girl Hunting" && cutsceneID == 26 && cGametask == 7)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 + 32));
                        }
                        if (BytetoBit(MissionByte3, 5) && TimeskipOrder.ElementAt(currentSkip).Key != "Girl Hunting")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 - 32));
                        }
                        if (!BytetoBit(MissionByte3, 6) && TimeskipOrder.ElementAt(currentSkip).Key == "A Promise To Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 + 64));
                        }
                        if (BytetoBit(MissionByte3, 6) && TimeskipOrder.ElementAt(currentSkip).Key == "Girl Hunting")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134779), (byte)(MissionByte3 - 64));
                        }
                        if (!BytetoBit(MissionByte4, 2) && TimeskipOrder.ElementAt(currentSkip).Key == "A Promise To Isabella" && cutsceneID == 30 && cGametask == 7)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134780), (byte)(MissionByte4 + 4));
                        }
                        if (BytetoBit(MissionByte4, 2) && TimeskipOrder.ElementAt(currentSkip).Key != "A Promise To Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134780), (byte)(MissionByte4 - 4));
                        }
                        if (!BytetoBit(MissionByte4, 3) && TimeskipOrder.ElementAt(currentSkip).Key == "Transporting Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134780), (byte)(MissionByte4 + 8));
                        }
                        if (BytetoBit(MissionByte4, 3) && TimeskipOrder.ElementAt(currentSkip).Key != "Transporting Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134780), (byte)(MissionByte4 - 8));
                        }
                        if (!BytetoBit(MissionByte5, 0) && TimeskipOrder.ElementAt(currentSkip).Key == "Bomb Collector")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 1));
                        }
                        if (BytetoBit(MissionByte5, 0) && TimeskipOrder.ElementAt(currentSkip).Key != "Bomb Collector")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 - 1));
                        }
                        if (!BytetoBit(MissionByte5, 2) && TimeskipOrder.ElementAt(currentSkip).Key == "Jamming Device")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 4));
                        }
                        if (BytetoBit(MissionByte5, 2) && TimeskipOrder.ElementAt(currentSkip).Key != "Jamming Device")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 - 4));
                        }
                        if (!BytetoBit(MissionByte5, 4) && TimeskipOrder.ElementAt(currentSkip).Key == "Hideout")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 16));
                        }
                        if (BytetoBit(MissionByte5, 4) && TimeskipOrder.ElementAt(currentSkip).Key != "Hideout")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 - 16));
                        }
                        if (!BytetoBit(MissionByte5, 5) && TimeskipOrder.ElementAt(currentSkip).Key == "Jessie's Discovery")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 32));
                        }
                        if (BytetoBit(MissionByte5, 5) && TimeskipOrder.ElementAt(currentSkip).Key != "Jessie's Discovery")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 - 32));
                        }
                        if (!BytetoBit(MissionByte5, 6) && TimeskipOrder.ElementAt(currentSkip).Key == "The Butcher")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 64));
                        }
                        if (BytetoBit(MissionByte5, 6) && TimeskipOrder.ElementAt(currentSkip).Key != "The Butcher")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 - 64));
                        }
                        if (!BytetoBit(MissionByte5, 7) && TimeskipOrder.ElementAt(currentSkip).Key == "The Butcher" && cutsceneID == 46 && cGametask == 7)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134781), (byte)(MissionByte5 + 128));
                        }
                        if (BytetoBit(MissionByte5, 7) && TimeskipOrder.ElementAt(currentSkip).Key != "The Butcher")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134780), (byte)(MissionByte5 - 128));
                        }
                        if (cutsceneID == 46 && BytetoBit(MissionByte6, 1) || TimeskipOrder.ElementAt(currentSkip).Key == "Ending" && BytetoBit(MissionByte6, 1))
                        {
                            gameMemory.WriteInt(IntPtr.Add(SpawnBossesPtr, 134782), MissionByte6 - 2);
                        }
                        if (!BytetoBit(MissionByte7, 3) && TimeskipOrder.ElementAt(currentSkip).Key == "Hideout" && loadingRoomId == 1025)
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), (byte)(MissionByte7 + 8));
                        }
                        if (BytetoBit(MissionByte7, 3) && TimeskipOrder.ElementAt(currentSkip).Key != "Hideout")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), (byte)(MissionByte7 - 8));
                        }
                        if (!BytetoBit(MissionByte8, 3) && TimeskipOrder.ElementAt(currentSkip).Key == "Medicine Run")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134840), (byte)(MissionByte8 + 8));
                        }
                        if (BytetoBit(MissionByte8, 3) && TimeskipOrder.ElementAt(currentSkip).Key != "Medicine Run")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134840), (byte)(MissionByte8 - 8));
                        }
                        if (!BytetoBit(MissionByte8, 5) && TimeskipOrder.ElementAt(currentSkip).Key == "A Promise To Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134840), (byte)(MissionByte8 + 32));
                        }
                        if (BytetoBit(MissionByte8, 5) && TimeskipOrder.ElementAt(currentSkip).Key != "A Promise To Isabella")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134840), (byte)(MissionByte8 - 32));
                        }
                        if (!BytetoBit(MissionByte9, 0) && TimeskipOrder.ElementAt(currentSkip).Key == "Jamming Device")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), (byte)(MissionByte9 + 1));
                        }
                        if (BytetoBit(MissionByte9, 0) && TimeskipOrder.ElementAt(currentSkip).Key != "Jamming Device")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), (byte)(MissionByte9 - 1));
                        }
                        if (!BytetoBit(MissionByte9, 1) && TimeskipOrder.ElementAt(currentSkip).Key == "Memories")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), (byte)(MissionByte9 + 2));
                        }
                        if (BytetoBit(MissionByte9, 1) && TimeskipOrder.ElementAt(currentSkip).Key != "Memories")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), (byte)(MissionByte9 - 2));
                        }
                        if (BytetoBit(Carlito2Death, 1) && TimeskipOrder.ElementAt(currentSkip).Key == "Rescue The Professor")
                        {
                            gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134761), (byte)(Carlito2Death - 2));
                        }
                    }
                    if (cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                    }
                    /// General Anti-Softlock Code
                    if (caseMenuState == 19 && saveMenuState == 0)
                    {
                        gameMemory.WriteByte(IntPtr.Add(saveMenuStatePtr, 9956), 1);
                    }
                }
            }
            else if (SelectedCategory == "Psychoskip" && Form1.TimeRandomized != DateTime.MinValue)
            {
                /// General stuff
                if (cutsceneID == 8 && cGametask == 7 && RandomizerStarted == false)
                {
                    RandomizerStarted = true;
                }
                if (RandomizerStarted == true)
                {
                    byte value = (byte)TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(3);
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841 + TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(4)), value);
                    if (cGametask == 7 || TimeskipOrder.ElementAt(currentSkip).Key == "Mark of the Sniper" || TimeskipOrder.ElementAt(currentSkip).Key == "Convicts")
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777740 + TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(2));
                    }
                    if (gameTime > 4535900)
                    {
                        gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 4212000u);
                    }
                    /// Goes to the next boss in the list
                    if (TimeskipOrder.ElementAt(currentSkip).Key == "Convicts" && Convicts1 == 0 && Convicts2 == 0 && Convicts3 == 0 && cGametask == 3 && loadingRoomId == 1792)
                    {
                        currentSkip++;
                    }
                    if (TimeskipOrder.ElementAt(currentSkip).Key == "Snipers" && Snipers1 == 0 && Snipers2 == 0 && Snipers3 == 0 && cGametask == 3 && loadingRoomId == 256)
                    {
                        currentSkip++;
                    }
                    if (cGametask == 7 && cutsceneID == TimeskipOrder.ElementAt(currentSkip).Value.ElementAt(1))
                    {
                        currentSkip++;
                    }
                    if (TimeskipOrder.ElementAt(currentSkip).Key == "Ending")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134789), 1);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip).Key == "Adam")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 135082), 128);
                    }
                }
            }
        }
    }
}