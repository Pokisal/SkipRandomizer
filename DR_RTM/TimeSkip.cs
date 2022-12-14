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

        public static List<string> TimeskipOrder = new(1) { "A" };

        public static List<string> TunnelsMissions = new() { "Tunnels", "Tank", "Brock" };

        public static List<string> PsychoSkips = new() { "Kent 3", "Convicts", "sean", "Snipers", "Adam", "Jo", "Paul", "Cletus", "Cliff" };

        public static List<uint> CutscenesToSkipOn = new() { };

        public static long TicksElapsed;

        public static long BeforeMovementLockedTicks;

        public static long MovementLockedTicks;

        private static IntPtr gameTimePtr;

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

        public static string SelectedCategory;

        private const int gameTimeOffset = 408;

        private static uint MovementLock1;

        private static uint MovementLock2;

        public static uint cutsceneID;

        public static uint LastCutscene;

        public static uint LastCutsceneSkip;

        public static uint Teleport;

        public static uint cutsceneOnLoad;

        public static byte cGametask;

        private static byte SpawnBosses;

        private static byte jessieSpeaking;

        private static byte CarlitoHideoutTrigger;

        private static int WatchCaseDisplay;

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

        private static uint PPRewards;

        private static byte caseMenuState;

        private static byte saveMenuState;

        private static byte Bombs;

        private static dynamic old = new ExpandoObject();

        public static bool carlitoHideoutFlag = false;

        public static bool startCutscene = false;

        public static bool includeOvertime = false;

        public static bool RandomizerStarted = false;

        public static int Seed;

        public static int currentSkip = 0;

        private static uint FrankX;

        private static uint FrankY;

        private static uint FrankZ;

        private static uint BossHealth;

        private static uint MainKillCount;

        private static uint SubKillCount = 999999;

        public static bool OnlyTriggerOnce = false;

        public static bool OnlyTriggerOnce2 = false;

        public static bool CurrentSkipOnce = false;

        public static bool FactsTriggered = false;

        public static void Init()
        {
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
        }
        public static void Shuffle<T>(List<T> list, int seed)
        {
            var rng = new Random(Seed);
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void Randomize()
        {
            Shuffle(TimeskipOrder, Seed);
        }
        public static void CreateList()
        {
            if (SelectedCategory == "Timeskip")
            {
                TimeskipOrder.Clear();
                CutscenesToSkipOn.Clear();
                TimeskipOrder.Add("Backup For Brad");
                TimeskipOrder.Add("An Odd Old Man");
                TimeskipOrder.Add("A Temporary Agreement");
                TimeskipOrder.Add("Rescue The Professor");
                TimeskipOrder.Add("Medicine Run");
                TimeskipOrder.Add("Girl Hunting");
                TimeskipOrder.Add("A Promise To Isabella");
                TimeskipOrder.Add("Transporting Isabella");
                TimeskipOrder.Add("Bomb Collector");
                TimeskipOrder.Add("Jamming Device");
                TimeskipOrder.Add("Hideout");
                TimeskipOrder.Add("Jessie's Discovery");
                TimeskipOrder.Add("The Butcher");
                TimeskipOrder.Add("Memories");
                CutscenesToSkipOn.Add(8);
                CutscenesToSkipOn.Add(10);
                CutscenesToSkipOn.Add(12);
                CutscenesToSkipOn.Add(13);
                CutscenesToSkipOn.Add(17);
                CutscenesToSkipOn.Add(24);
                CutscenesToSkipOn.Add(27);
                CutscenesToSkipOn.Add(31);
                CutscenesToSkipOn.Add(33);
                CutscenesToSkipOn.Add(39);
                CutscenesToSkipOn.Add(41);
                CutscenesToSkipOn.Add(42);
                CutscenesToSkipOn.Add(43);
                CutscenesToSkipOn.Add(45);
                CutscenesToSkipOn.Add(47);
                if (includeOvertime == true)
                {
                    TimeskipOrder.Add("Supplies");
                    TimeskipOrder.Add("Queens");
                    TimeskipOrder.Add("Tunnels");
                    TimeskipOrder.Add("Tank");
                    TimeskipOrder.Add("Brock");
                    CutscenesToSkipOn.Add(135);
                    CutscenesToSkipOn.Add(132);
                    CutscenesToSkipOn.Add(126);
                    CutscenesToSkipOn.Add(136);
                    CutscenesToSkipOn.Add(144);
                }
            }
            else if (SelectedCategory == "Psychoskip")
            {
                TimeskipOrder.Clear();
                CutscenesToSkipOn.Clear();
                TimeskipOrder.Add("Convicts");
                TimeskipOrder.Add("Cliff");
                TimeskipOrder.Add("Cletus");
                TimeskipOrder.Add("Adam");
                TimeskipOrder.Add("Jo");
                TimeskipOrder.Add("Snipers");
                TimeskipOrder.Add("Sean");
                TimeskipOrder.Add("Paul");
                TimeskipOrder.Add("Kent 3");
                CutscenesToSkipOn.Add(70);
                CutscenesToSkipOn.Add(71);
                CutscenesToSkipOn.Add(72);
                CutscenesToSkipOn.Add(73);
                CutscenesToSkipOn.Add(74);
                CutscenesToSkipOn.Add(75);
                CutscenesToSkipOn.Add(76);
                CutscenesToSkipOn.Add(77);
            }
            else if (SelectedCategory == "All Bosses")
            {
                TimeskipOrder.Clear();
                CutscenesToSkipOn.Clear();
                TimeskipOrder.Add("Backup For Brad");
                TimeskipOrder.Add("An Odd Old Man");
                TimeskipOrder.Add("A Temporary Agreement");
                TimeskipOrder.Add("Rescue The Professor");
                TimeskipOrder.Add("Medicine Run");
                TimeskipOrder.Add("Girl Hunting");
                TimeskipOrder.Add("A Promise To Isabella");
                TimeskipOrder.Add("Transporting Isabella");
                TimeskipOrder.Add("Bomb Collector");
                TimeskipOrder.Add("Jamming Device");
                TimeskipOrder.Add("Hideout");
                TimeskipOrder.Add("Jessie's Discovery");
                TimeskipOrder.Add("The Butcher");
                TimeskipOrder.Add("Memories");
                TimeskipOrder.Add("Convicts");
                TimeskipOrder.Add("Cliff");
                TimeskipOrder.Add("Cletus");
                TimeskipOrder.Add("Adam");
                TimeskipOrder.Add("Jo");
                TimeskipOrder.Add("Snipers");
                TimeskipOrder.Add("Sean");
                TimeskipOrder.Add("Paul");
                TimeskipOrder.Add("Kent 3");
                TimeskipOrder.Add("Supplies");
                TimeskipOrder.Add("Queens");
                TimeskipOrder.Add("Tunnels");
                TimeskipOrder.Add("Tank");
                TimeskipOrder.Add("Brock");
                CutscenesToSkipOn.Add(8);
                CutscenesToSkipOn.Add(10);
                CutscenesToSkipOn.Add(12);
                CutscenesToSkipOn.Add(13);
                CutscenesToSkipOn.Add(17);
                CutscenesToSkipOn.Add(24);
                CutscenesToSkipOn.Add(27);
                CutscenesToSkipOn.Add(31);
                CutscenesToSkipOn.Add(33);
                CutscenesToSkipOn.Add(39);
                CutscenesToSkipOn.Add(41);
                CutscenesToSkipOn.Add(42);
                CutscenesToSkipOn.Add(43);
                CutscenesToSkipOn.Add(45);
                CutscenesToSkipOn.Add(47);
                CutscenesToSkipOn.Add(70);
                CutscenesToSkipOn.Add(71);
                CutscenesToSkipOn.Add(72);
                CutscenesToSkipOn.Add(73);
                CutscenesToSkipOn.Add(74);
                CutscenesToSkipOn.Add(75);
                CutscenesToSkipOn.Add(76);
                CutscenesToSkipOn.Add(77);
                CutscenesToSkipOn.Add(135);
                CutscenesToSkipOn.Add(132);
                CutscenesToSkipOn.Add(126);
                CutscenesToSkipOn.Add(136);
                CutscenesToSkipOn.Add(144);
            }
            if (Form1.spawnEnemies == true)
            {
                CutscenesToSkipOn.Remove(8);
                CutscenesToSkipOn.Add(50);
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
            CarlitoHideoutTrigger = gameMemory.ReadByte(IntPtr.Add(SpawnBossesPtr, 134788));
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
            FrankX = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 200), 64));
            FrankY = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 200), 68));
            FrankZ = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 200), 72));
            BossHealth = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 30352928, 280), 4844));
            MainKillCount = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26582688), 944));
            MovementLock1 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26496472), 132024));
            MovementLock2 = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26496472), 132028));
            loadingRoomId = gameMemory.ReadInt(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 72));
            jessieSpeaking = gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26496472, 134592), 794));
            Bombs = gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26496472, 134592), 33933));
            inCutsceneOrLoad = (gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 112)) & 1) == 1;
            inCutsceneOrLoad = (gameMemory.ReadByte(IntPtr.Add(gameMemory.Pointer("DeadRising.exe", 26500976), 112)) & 1) == 1;
            form.TimeDisplayUpdate(StringTime(gameTime));
            if (SelectedCategory == "Timeskip" && Form1.TimeRandomized != DateTime.MinValue)
            {
                if (campaignProgress == 400 && inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10260000u);
                }
                if (campaignProgress == 400 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameTime + 18000 + 1);
                }
                if (campaignProgress == 402 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameTime + 18000 + 1);
                }
                if (campaignProgress == 404 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 408)) + 1);
                }
                if (campaignProgress == 402 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10358001u);
                }
                if (campaignProgress == 404 && old.inCutsceneOrLoad && !inCutsceneOrLoad && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10368001u);
                }
                if (campaignProgress == 406 || campaignProgress == 410 || campaignProgress == 415 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 408)) + 1);
                }
                if (old.campaignProgress != 410 && campaignProgress == 410 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11448000u);
                }
                if (old.campaignProgress != 415 && campaignProgress == 415 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11448000u);
                }
                if (campaignProgress == 420 && loadingRoomId == 288 && old.inCutsceneOrLoad && !inCutsceneOrLoad && TimeskipOrder.ElementAt(currentSkip) == " " && includeOvertime == false)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 69);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11664501u);
                }
                /// General stuff
                if (cutsceneID == 8 && cGametask == 7)
                {
                    RandomizerStarted = true;
                    CurrentSkipOnce = true;
                }
                if (cGametask == 3)
                {
                    startCutscene = false;
                    OnlyTriggerOnce = false;
                    OnlyTriggerOnce2 = false;
                    CurrentSkipOnce = false;
                }
                if (campaignProgress == 270 && cutsceneID == 30 && cGametask == 7)
                {
                    SubKillCount = MainKillCount;
                }
                if (MovementLock1 == 0 && MovementLock2 == 0)
                {
                    TicksElapsed = 0;
                    BeforeMovementLockedTicks = DateTime.Now.Ticks;
                }
                if (MovementLock1 == 1024 && MovementLock2 == 1024)
                {
                    MovementLockedTicks = DateTime.Now.Ticks;
                    TicksElapsed = MovementLockedTicks - BeforeMovementLockedTicks;
                }
                if (currentSkip == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                }
                if (currentSkip != 0)
                {
                    if (cutsceneID == 10 && TimeskipOrder.ElementAt(currentSkip - 1) != "Backup for Brad" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 17 && TimeskipOrder.ElementAt(currentSkip - 1) != "Rescue The Professor" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 27 && TimeskipOrder.ElementAt(currentSkip - 1) != "Girl Hunting" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 45 && TimeskipOrder.ElementAt(currentSkip - 1) != "The Butcher" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                }
                if (LastCutscene == 10 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 20000);
                }
                if (LastCutscene == 17 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 30000);
                }
                if (LastCutscene == 27 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 30000);
                }
                if (LastCutscene == 45 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 50000);
                }
                /// General Anti-Softlock Code
                if (caseMenuState == 19 && saveMenuState == 0)
                {
                    gameMemory.WriteByte(IntPtr.Add(saveMenuStatePtr, 9956), 1);
                }
                if (campaignProgress > 354 && campaignProgress < 500 && loadingRoomId != 1025)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 8);
                }
                if (campaignProgress < 354)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 0);
                }
                if (campaignProgress > 499)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 24);
                }
                if (cutsceneID == 42)
                {
                    carlitoHideoutFlag = true;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 135 && cGametask == 7 && carlitoHideoutFlag == true && OnlyTriggerOnce2 == false)
                {
                    OnlyTriggerOnce2 = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (cutsceneID == 13 && startCutscene == true)
                {
                    startCutscene = false;
                }
                if (cutsceneID == 48 && startCutscene == true)
                {
                    startCutscene = false;
                }
                if (cutsceneID == 37 && cGametask == 7)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 47 && cGametask == 7)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 50)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (gameTime > 10367999 && TimeskipOrder.ElementAt(currentSkip) != " " && !TunnelsMissions.Contains(TimeskipOrder.ElementAt(currentSkip)))
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 17 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Medicine Run")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 25 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Girl Hunting")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 37 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Bomb Collector")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 41 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Hideout")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 43 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "The Butcher")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 9 && TimeskipOrder.ElementAt(currentSkip) != "An Odd Old Man" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 44 && TimeskipOrder.ElementAt(currentSkip) != "The Butcher" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 26 && TimeskipOrder.ElementAt(currentSkip) != "Girl Hunting" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 21 && TimeskipOrder.ElementAt(currentSkip) != "Medicine Run" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 22);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 9 && TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && BossHealth == 1 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 16 && TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && TicksElapsed > 3000000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 21 && TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && TicksElapsed > 25000000 && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 22);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 26 && TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && TicksElapsed > 5600000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 44 && TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && TicksElapsed > 8000000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 144 && MovementLock1 == 1024 && MovementLock2 == 1024 && cGametask == 3 && TimeskipOrder.ElementAt(currentSkip) != " ")
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && cGametask == 3 && loadingRoomId == 2560 && cutsceneID != 9 && cutsceneID != 10)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 9);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && cGametask == 3 && loadingRoomId == 256 && cutsceneID != 16 && cutsceneID != 17 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 16);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && cGametask == 3 && loadingRoomId == 1537 && cutsceneID != 44 && cutsceneID != 45)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 44);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && campaignProgress == 270 && cGametask == 3 && MainKillCount > SubKillCount)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 215 && jessieSpeaking == 168 && OnlyTriggerOnce == false && cGametask == 3 && loadingRoomId == 288)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 24);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 110 && loadingRoomId == 256 && FrankX > 1177777193 && FrankY < 1133903905 && FrankZ < 1182494445 && FrankZ > 1179252602 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 130 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 13);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 250 && loadingRoomId == 1024 && FrankX == 3308748027 && FrankZ == 3329894401 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 30);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 250 && loadingRoomId == 1024 && FrankX == 3309274885 && FrankZ == 3329894401 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 30);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 320 && Bombs == 5 && loadingRoomId == 1536 && FrankX > 3327101379 && FrankZ > 3328263339 && FrankZ < 3329448231 && cGametask == 3 && OnlyTriggerOnce == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 340 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3 && startCutscene == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 355 && loadingRoomId == 1025 && FrankX > 1155545513 && FrankX < 1159950332 && cGametask == 3 && OnlyTriggerOnce == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 360 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 390 && loadingRoomId == 1025 && FrankX > 1155545513 && FrankX < 1159950332 && cGametask == 3 && FactsTriggered == false)
                {
                    FactsTriggered = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 46);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 47 && cGametask == 7 && startCutscene == false)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                /// Write Campaign Progress according to what randomized option you're on to prevent certain bosses from spawning and other strangeness
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && cGametask == 3 && cutsceneID != 9 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 80);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 110);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && cGametask == 3 && cutsceneID != 13 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 130);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && cGametask == 3 && cutsceneID != 16 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 160);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && cGametask == 3 && campaignProgress != 190 && campaignProgress != 210 && campaignProgress != 215 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 180);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && cGametask == 3 && cutsceneID != 26 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 230);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && cGametask == 3 && cutsceneID != 30 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 250);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 280);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 320);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 340);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && cGametask == 3 && cutsceneID != 80 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 350);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 360);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && cGametask == 3 && cutsceneID != 44 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 370);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && cGametask == 3 && RandomizerStarted == true && cutsceneID != 46)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 390);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 135 && RandomizerStarted == true || TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 42 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 500);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 680);
                }
                /// Write Watch Cases according to what current randomized option is
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039873);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039873);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039875);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039876);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039876);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039878);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039880);
                }
                /// Spawn Cases
                if (cGametask == 7 && CurrentSkipOnce == false && CutscenesToSkipOn.Contains(cutsceneID) && cutsceneID != 8)
                {
                    LastCutscene = cutsceneID;
                    CurrentSkipOnce = true;
                    currentSkip = currentSkip + 1;
                }
                if (cutsceneID == 8 && Form1.spawnEnemies == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 50);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 8)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 8);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 8;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 10)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 10;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 12)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 12;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 15)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 15);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 15;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 17)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 17;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 25)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 25);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 25;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 27)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 27;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 31)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 31;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 37)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 37);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 37;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 39)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 39;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 41)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 41;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 42)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 41;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 43)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 43;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 45)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 45;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 135)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 135);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 135;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 132)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 132);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 132;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Tunnels" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 126)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 126);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 126;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Tank" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 136)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 136);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 136;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Brock" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 144)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 144;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == " " && includeOvertime == false && cGametask == 7)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 52);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == " " && includeOvertime == true && cGametask == 7)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 134);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
            }
            else if (SelectedCategory == "Psychoskip" && Form1.TimeRandomized != DateTime.MinValue)
            {
                /// General stuff
                if (cutsceneID == 8 && cGametask == 7)
                {
                    RandomizerStarted = true;
                    if (Form1.spawnEnemies == true)
                    {
                        gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 50);
                        gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    }
                }
                if (gameTime > 4535999)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 4212000u);
                }
                if (cGametask == 3)
                {
                    startCutscene = false;
                }
                if (RandomizerStarted == true)
                {
                    /// Goes to the next boss in the list
                    if (TimeskipOrder.ElementAt(currentSkip) == "Convicts" && Convicts1 == 0 && Convicts2 == 0 && Convicts3 == 0 && cGametask == 3 && loadingRoomId == 1792)
                    {
                        currentSkip = currentSkip + 1;
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && Snipers1 == 0 && Snipers2 == 0 && Snipers3 == 0 && cGametask == 3 && loadingRoomId == 256)
                    {
                        currentSkip = currentSkip + 1;
                    }
                    if (cGametask == 7 && startCutscene == false && CutscenesToSkipOn.Contains(cutsceneID))
                    {
                        startCutscene = true;
                        currentSkip = currentSkip + 1;
                    }
                    /// Writes to memory what's required to make that boss spawn or play the ending cutscene
                    if (TimeskipOrder.ElementAt(currentSkip) == " ")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134789), 1);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Kent 3")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 32);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Cliff")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 64);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId != 1024 && loadingRoomId != 1281)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 208);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Adam")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 17);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Jo")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 210);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Paul")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 214);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Convicts")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134844), 132);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Cletus")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 36);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && loadingRoomId != 1024)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 77);
                    }
                    /// Write to memory to make bosses that shouldn't be active not be active
                    if (TimeskipOrder.ElementAt(currentSkip) != "Cletus" || TimeskipOrder.ElementAt(currentSkip) != "Snipers")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 32);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && loadingRoomId == 1281)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 32);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) != "Adam" || TimeskipOrder.ElementAt(currentSkip) != "Jo" || TimeskipOrder.ElementAt(currentSkip) != "Paul")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 16);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) != "Kent 3" || TimeskipOrder.ElementAt(currentSkip) != "Sean" || TimeskipOrder.ElementAt(currentSkip) != "Cliff")
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 0);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId == 1024 || TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId == 1281)
                    {
                        gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 0);
                    }
                    /// Writes to the watch the current boss you're on
                    if (TimeskipOrder.ElementAt(currentSkip) == "Kent 3" && RandomizerStarted == true && cutsceneID != 113)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777740);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Cliff" && RandomizerStarted == true && cutsceneID != 57)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777741);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && RandomizerStarted == true && cutsceneID != 58)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777742);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Adam" && RandomizerStarted == true && cutsceneID != 59)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777743);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Jo" && RandomizerStarted == true && cutsceneID != 60)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777744);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Paul" && RandomizerStarted == true && cutsceneID != 61)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777745);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && RandomizerStarted == true)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777751);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Cletus" && RandomizerStarted == true)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777759);
                    }
                    if (TimeskipOrder.ElementAt(currentSkip) == "Convicts" && RandomizerStarted == true)
                    {
                        gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777983);
                    }
                }
            }
            else if (SelectedCategory == "All Bosses")
            {
                /// Timeskip Code
                if (campaignProgress == 400 && inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10260000u);
                }
                if (campaignProgress == 400 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameTime + 18000 + 1);
                }
                if (campaignProgress == 402 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameTime + 18000 + 1);
                }
                if (campaignProgress == 404 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 408)) + 1);
                }
                if (campaignProgress == 402 && old.inCutsceneOrLoad && !inCutsceneOrLoad)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10358001u);
                }
                if (campaignProgress == 404 && old.inCutsceneOrLoad && !inCutsceneOrLoad && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 10368001u);
                }
                if (campaignProgress == 406 || campaignProgress == 410 || campaignProgress == 415 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 408)) + 1);
                }
                if (old.campaignProgress != 410 && campaignProgress == 410 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11448000u);
                }
                if (old.campaignProgress != 415 && campaignProgress == 415 && TimeskipOrder.Count == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11448000u);
                }
                if (campaignProgress == 420 && loadingRoomId == 288 && old.inCutsceneOrLoad && !inCutsceneOrLoad && TimeskipOrder.ElementAt(currentSkip) == " " && includeOvertime == false)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 69);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 11664501u);
                }
                /// General Code
                if (cutsceneID == 8)
                {
                    RandomizerStarted = true;
                    CurrentSkipOnce = true;
                }
                if (cGametask == 3)
                {
                    startCutscene = false;
                    OnlyTriggerOnce = false;
                    OnlyTriggerOnce2 = false;
                    CurrentSkipOnce = false;
                }
                if (campaignProgress == 270 && cutsceneID == 30 && cGametask == 7)
                {
                    SubKillCount = MainKillCount;
                }
                if (MovementLock1 == 0 && MovementLock2 == 0)
                {
                    TicksElapsed = 0;
                    BeforeMovementLockedTicks = DateTime.Now.Ticks;
                }
                if (MovementLock1 == 1024 && MovementLock2 == 1024)
                {
                    MovementLockedTicks = DateTime.Now.Ticks;
                    TicksElapsed = MovementLockedTicks - BeforeMovementLockedTicks;
                }
                if (currentSkip == 0)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                }
                if (currentSkip != 0)
                {
                    if (cutsceneID == 10 && TimeskipOrder.ElementAt(currentSkip - 1) != "Backup for Brad" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 17 && TimeskipOrder.ElementAt(currentSkip - 1) != "Rescue The Professor" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 27 && TimeskipOrder.ElementAt(currentSkip - 1) != "Girl Hunting" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                    if (cutsceneID == 45 && TimeskipOrder.ElementAt(currentSkip - 1) != "The Butcher" && OnlyTriggerOnce == false && cGametask == 7)
                    {
                        OnlyTriggerOnce = true;
                        gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 0);
                    }
                }
                if (LastCutscene == 10 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 20000);
                }
                if (LastCutscene == 17 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 30000);
                }
                if (LastCutscene == 27 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 30000);
                }
                if (LastCutscene == 45 && cGametask == 7 && startCutscene == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(PPRewardsPtr, 33628), 50000);
                }
                /// Softlock Prevention
                if (caseMenuState == 19 && saveMenuState == 0)
                {
                    gameMemory.WriteByte(IntPtr.Add(saveMenuStatePtr, 9956), 1);
                }
                if (LastCutscene == 126 || LastCutscene > 135)
                {
                    if (!TunnelsMissions.Contains(TimeskipOrder.ElementAt(currentSkip)) && cGametask == 3)
                    {
                        if (loadingRoomId == 2816 || loadingRoomId == 2817 || loadingRoomId == 2818 || loadingRoomId == 2819)
                        {
                            gameMemory.WriteUInt(IntPtr.Add(TeleportPtr, 440), 512);
                        }
                    }
                }
                if (gameTime > 10367999)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 42)
                {
                    carlitoHideoutFlag = true;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 135 && cGametask == 7 && carlitoHideoutFlag == true && OnlyTriggerOnce2 == false)
                {
                    OnlyTriggerOnce2 = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (cutsceneID == 42 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Jessie's Discovery" && cutsceneID != 84 && cutsceneID != 87)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), LastCutsceneSkip);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (campaignProgress > 354 && campaignProgress < 500 && loadingRoomId != 1025)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 8);
                }
                if (campaignProgress < 354)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 0);
                }
                if (campaignProgress > 499)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134788), 24);
                }
                if (cutsceneID == 13 && startCutscene == true)
                {
                    startCutscene = false;
                }
                if (cutsceneID == 48 && startCutscene == true)
                {
                    startCutscene = false;
                }
                if (cutsceneID == 37 && cGametask == 7)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 47 && cGametask == 7)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 50)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (gameTime > 10367999 && TimeskipOrder.ElementAt(currentSkip) != " " && !TunnelsMissions.Contains(TimeskipOrder.ElementAt(currentSkip)))
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                if (cutsceneID == 17 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Medicine Run")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 25 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Girl Hunting")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 37 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Bomb Collector")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 41 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "Hideout")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 43 && startCutscene == true && TimeskipOrder.ElementAt(currentSkip) != "The Butcher")
                {
                    startCutscene = false;
                }
                if (cutsceneID == 9 && TimeskipOrder.ElementAt(currentSkip) != "An Odd Old Man" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 44 && TimeskipOrder.ElementAt(currentSkip) != "The Butcher" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 26 && TimeskipOrder.ElementAt(currentSkip) != "Girl Hunting" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 21 && TimeskipOrder.ElementAt(currentSkip) != "Medicine Run" && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 22);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 9 && TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && BossHealth == 1 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 16 && TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && TicksElapsed > 3000000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 21 && TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && TicksElapsed > 25000000 && cGametask == 3)
                {
                    startCutscene = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 22);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 26 && TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && TicksElapsed > 5600000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 44 && TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && TicksElapsed > 8000000 && cGametask == 3)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 144 && MovementLock1 == 1024 && MovementLock2 == 1024 && cGametask == 3 && TimeskipOrder.ElementAt(currentSkip) != " ")
                {
                    if (!PsychoSkips.Contains(TimeskipOrder.ElementAt(currentSkip + 1)))
                    {
                        gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                        gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                        gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                    }
                    else if (PsychoSkips.Contains(TimeskipOrder.ElementAt(currentSkip + 1)) && OnlyTriggerOnce == false)
                    {
                        OnlyTriggerOnce = true;
                        currentSkip = currentSkip + 1;
                        gameMemory.WriteUInt(IntPtr.Add(TeleportPtr, 440), 512);
                    }
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && cGametask == 3 && loadingRoomId == 2560 && cutsceneID != 9 && cutsceneID != 10)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 9);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && cGametask == 3 && loadingRoomId == 256 && cutsceneID != 16 && cutsceneID != 17 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 16);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && cGametask == 3 && loadingRoomId == 1537 && cutsceneID != 44 && cutsceneID != 45)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 44);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && campaignProgress == 270 && cGametask == 3 && MainKillCount > SubKillCount)
                {
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 215 && jessieSpeaking == 168 && OnlyTriggerOnce == false && cGametask == 3 && loadingRoomId == 288)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 24);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 110 && loadingRoomId == 256 && FrankX > 1177777193 && FrankY < 1133903905 && FrankZ < 1182494445 && FrankZ > 1179252602 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 130 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 13);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 250 && loadingRoomId == 1024 && FrankX == 3308748027 && FrankZ == 3329894401 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 30);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 250 && loadingRoomId == 1024 && FrankX == 3309274885 && FrankZ == 3329894401 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 30);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 320 && Bombs == 5 && loadingRoomId == 1536 && FrankX > 3327101379 && FrankZ > 3328263339 && FrankZ < 3329448231 && cGametask == 3 && OnlyTriggerOnce == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 340 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3 && startCutscene == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 355 && loadingRoomId == 1025 && FrankX > 1155545513 && FrankX < 1159950332 && cGametask == 3 && OnlyTriggerOnce == false)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 360 && loadingRoomId == 288 && FrankX == 1181614080 && FrankZ == 1185910293 && OnlyTriggerOnce == false && cGametask == 3)
                {
                    OnlyTriggerOnce = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (campaignProgress == 390 && loadingRoomId == 1025 && FrankX > 1155545513 && FrankX < 1159950332 && cGametask == 3 && FactsTriggered == false)
                {
                    FactsTriggered = true;
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneIDPtr, 33544), 46);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                if (cutsceneID == 47 && cGametask == 7 && startCutscene == false)
                {
                    gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 408), 3888000u);
                }
                /// Force Cases to spawn
                if (cGametask == 7 && CurrentSkipOnce == false && CutscenesToSkipOn.Contains(cutsceneID) && cutsceneID != 8)
                {
                    LastCutscene = cutsceneID;
                    CurrentSkipOnce = true;
                    currentSkip = currentSkip + 1;
                }
                if (cutsceneID == 8 && Form1.spawnEnemies == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 50);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Convicts" && Convicts1 == 0 && Convicts2 == 0 && Convicts3 == 0 && cGametask == 3 && loadingRoomId == 1792)
                {
                    currentSkip = currentSkip + 1;
                    if (!PsychoSkips.Contains(TimeskipOrder.ElementAt(currentSkip)))
                    {
                        if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 8);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 8;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 10;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 12;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 15);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 15;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 17;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 25);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 25;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 27;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 31;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 37);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 37;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 39;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 41;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 42;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 43;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 45;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 135);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 135;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 132);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 132;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Tunnels" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 126);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 126;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Tank" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 136);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 136;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Brock" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 144;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == " ")
                        {
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 134);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                        }
                        currentSkip = currentSkip - 1;
                    }
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && Snipers1 == 0 && Snipers2 == 0 && Snipers3 == 0 && cGametask == 3 && loadingRoomId == 256)
                {
                    currentSkip = currentSkip + 1;
                    if (!PsychoSkips.Contains(TimeskipOrder.ElementAt(currentSkip)))
                    {
                        if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 8);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 8;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 10;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 12;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 15);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 15;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 17;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 25);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 25;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 27;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 31;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 37);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 37;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 39;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 41;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 42;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 43;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 45;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 135);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 135;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 132);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 132;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Tunnels" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 126);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 126;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Tank" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 136);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 136;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == "Brock" && startCutscene == false)
                        {
                            startCutscene = true;
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                            LastCutsceneSkip = 144;
                        }
                        if (TimeskipOrder.ElementAt(currentSkip) == " ")
                        {
                            gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 134);
                            gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                            gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                        }
                        currentSkip = currentSkip - 1;
                    }
                }
                if (cutsceneID == 8 && Form1.spawnEnemies == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 50);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 8)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 8);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 8;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 10)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 10);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 10;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 12)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 12);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 12;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 15)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 15);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 15;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 17)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 17);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 17;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 25)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 25);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 25;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 27)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 27);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 27;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 31)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 31);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 31;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 37)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 37);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 37;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 39)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 39);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 39;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 41)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 41);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 41;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 42)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 42);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 42;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 43)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 43);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 43;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 45)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 45);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 45;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 135)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 135);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 135;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 132)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 132);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 132;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Tunnels" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 126)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 126);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 126;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Tank" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 136)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 136);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 136;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Brock" && CutscenesToSkipOn.Contains(cutsceneID) && startCutscene == false && cGametask == 7 && cutsceneID != 144)
                {
                    startCutscene = true;
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 144);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    LastCutsceneSkip = 144;
                }
                if (TimeskipOrder.ElementAt(currentSkip) == " " && cGametask == 3 && cutsceneID != 144)
                {
                    gameMemory.WriteInt(IntPtr.Add(cutsceneIDPtr, 33544), 134);
                    gameMemory.WriteUInt(IntPtr.Add(cutsceneOnLoadPtr, 33552), 0);
                    gameMemory.WriteByte(IntPtr.Add(cGametaskPtr, 56), 4);
                }
                /// Force Bosses to spawn
                if (TimeskipOrder.ElementAt(currentSkip) == "Kent 3" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 32);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Cliff" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 64);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId != 1281 && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 208);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Adam" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 17);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jo" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 210);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Paul" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 214);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Convicts" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134844), 132);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Cletus" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 36);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && RandomizerStarted == true)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 77);
                }
                /// Write to memory to make bosses that shouldn't be active not be active
                if (TimeskipOrder.ElementAt(currentSkip) != "Cletus" && TimeskipOrder.ElementAt(currentSkip) != "Snipers")
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 32);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && loadingRoomId == 1281)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134845), 32);
                }
                if (TimeskipOrder.ElementAt(currentSkip) != "Adam" && TimeskipOrder.ElementAt(currentSkip) != "Jo" && TimeskipOrder.ElementAt(currentSkip) != "Paul")
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134842), 16);
                }
                if (TimeskipOrder.ElementAt(currentSkip) != "Kent 3" && TimeskipOrder.ElementAt(currentSkip) != "Sean" && TimeskipOrder.ElementAt(currentSkip) != "Cliff")
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId == 1024 || TimeskipOrder.ElementAt(currentSkip) == "Sean" && loadingRoomId == 1281)
                {
                    gameMemory.WriteByte(IntPtr.Add(SpawnBossesPtr, 134841), 0);
                }
                /// Writes campaign progress to make sure certain bosses aren't killed early and so the game doesn't overwrite it itself and softlock the player
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && cGametask == 3 && cutsceneID != 9 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 80);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 110);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && cGametask == 3 && cutsceneID != 13 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 130);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && cGametask == 3 && cutsceneID != 16 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 160);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && cGametask == 3 && campaignProgress != 190 && campaignProgress != 210 && campaignProgress != 215 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 180);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && cGametask == 3 && cutsceneID != 26 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 230);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && cGametask == 3 && cutsceneID != 30 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 250);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 280);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 320);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 340);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && cGametask == 3 && cutsceneID != 80 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 350);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && cGametask == 3 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 360);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && cGametask == 3 && cutsceneID != 44 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 370);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && cGametask == 3 && RandomizerStarted == true && cutsceneID != 46)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 390);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 135 && RandomizerStarted == true || TimeskipOrder.ElementAt(currentSkip) == "Supplies" && cutsceneID == 42 && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 500);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Queens" && RandomizerStarted == true)
                {
                    gameMemory.WriteUInt(IntPtr.Add(campaignProgressPtr, 336), 680);
                }
                /// Writes to the watch the current case the player is on
                if (TimeskipOrder.ElementAt(currentSkip) == "Backup For Brad" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "An Odd Old Man" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Temporary Agreement" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039872);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Rescue The Professor" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039873);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Medicine Run" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039873);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Girl Hunting" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039875);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "A Promise To Isabella" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039876);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Transporting Isabella" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039876);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Bomb Collector" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039878);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jamming Device" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Hideout" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jessie's Discovery" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "The Butcher" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039879);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Memories" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8024), 17039880);
                }
                /// Writes to the watch the current boss the player is on
                if (!PsychoSkips.Contains(TimeskipOrder.ElementAt(currentSkip)))
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 0);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Kent 3" && cutsceneID != 113 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777740);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Cliff" && cutsceneID != 57 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777741);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Sean" && cutsceneID != 58 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777742);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Adam" && cutsceneID != 59 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777743);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Jo" && cutsceneID != 60 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777744);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Paul" && cutsceneID != 61 && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777745);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Snipers" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777751);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Cletus" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777759);
                }
                if (TimeskipOrder.ElementAt(currentSkip) == "Convicts" && RandomizerStarted == true)
                {
                    gameMemory.WriteInt(IntPtr.Add(WatchCaseDisplayPtr, 8031), 16777983);
                }
            }
        }
    }
}