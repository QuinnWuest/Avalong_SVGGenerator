using System.Data;

class Program
{
    static int Main(string[] args)
    {
        Avalong.CreateLargeImage();

        Console.WriteLine("Done.");
        return 0;
    }
}

public enum EnumAR
{
    Merlin,
    Percival,
    Tristan,
    Isolde,
    Resistance,
    Blesser,
    Cupid,
    TeamPlanner,
    Desperado,
    Dragonmancer,
    OracleBomb,
    Assassin,
    Morgana,
    Witch,
    Mordred,
    MordredA,
    MordredB,
    Oberon,
    Spy,
}

public enum MissionStatus
{
    None,
    Success,
    Fail
}

public class AvalongRole
{
    public EnumAR Role;
    public string RoleName;
    public bool IsSpy;
    public bool IsVisibleToMerlin;
    public bool IsPercivalCandidate;

    public AvalongRole(EnumAR role, string roleName, bool isSpy, bool isVisibleToMerlin = true, bool isPercivalCandidate = false)
    {
        Role = role;
        RoleName = roleName;
        IsSpy = isSpy;
        IsVisibleToMerlin = isVisibleToMerlin;
        IsPercivalCandidate = isPercivalCandidate;
    }
}

public class PlayerInfo
{
    public string PlayerName;
    public AvalongRole Info;

    public PlayerInfo(string name, AvalongRole info)
    {
        PlayerName = name;
        Info = info;
    }
}

static class Avalong
{
    const int width = 1920;
    const int height = 700;
    const int imgSize = 160;

    public static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    public static (int x, int y)[] GeneratePlayerLocations(int numPlayers)
    {
        const int rx = 800;
        const int ry = 225;
        const int cx = 960;
        const int cy = 350;
        const int yOffset = 30;

        var pos = new List<(int x, int y)>();

        var n = (numPlayers % 4 == 1 ? numPlayers : numPlayers + 1) >> 1;
        for (var i = 0; i <= n; i++)
        {
            var mx = 2 * rx * i / n - rx;
            pos.Add((cx + mx, (int)(cy - Math.Sqrt(-mx * mx + rx * rx) * ry / rx) - yOffset));
        }

        for (var i = n + 1; i < numPlayers; i++)
        {
            var mx = rx - 2 * rx * (i - n) / ((numPlayers % 4 == 1 ? numPlayers + 1 : numPlayers) >> 1);
            pos.Add((cx + mx, (int)(cy + Math.Sqrt(-mx * mx + rx * rx) * ry / rx) - yOffset));
        }

        var h = (numPlayers % 4 == 2 ? numPlayers + 2 : numPlayers % 4 == 1 ? numPlayers : numPlayers + 1) >> 2;
        return pos.Skip(h).Concat(pos.Take(h)).ToArray();
    }

    public static void CreateLargeImage()
    {
        int seed = 0;
        var random = new Random(seed);
        // START EDITING BLOCK

        var dict = new Dictionary<EnumAR, AvalongRole>()
        {
            [EnumAR.Merlin] = new AvalongRole(EnumAR.Merlin, "Merlin", false, isPercivalCandidate: true),
            [EnumAR.Percival] = new AvalongRole(EnumAR.Percival, "Percival", false),
            [EnumAR.Tristan] = new AvalongRole(EnumAR.Tristan, "Tristan", false),
            [EnumAR.Isolde] = new AvalongRole(EnumAR.Isolde, "Isolde", false),
            [EnumAR.Resistance] = new AvalongRole(EnumAR.Resistance, "Resistance", false),
            [EnumAR.Oberon] = new AvalongRole(EnumAR.Oberon, "Oberon", true),
            [EnumAR.Assassin] = new AvalongRole(EnumAR.Assassin, "Assassin", true),
            [EnumAR.Morgana] = new AvalongRole(EnumAR.Morgana, "Morgana", true, isPercivalCandidate: true),
            [EnumAR.Witch] = new AvalongRole(EnumAR.Witch, "Witch", true),
            [EnumAR.Mordred] = new AvalongRole(EnumAR.Mordred, "Mordred", true, isVisibleToMerlin: false)
        };

        //All the players with their respective role. Order here is irrelevant, but is done alphabetically for easier editing.
        PlayerInfo pingu = new("pingu", dict[EnumAR.Resistance]);
        PlayerInfo rose = new("Rose", dict[EnumAR.Resistance]);
        PlayerInfo oolong = new("oolong", dict[EnumAR.Resistance]);
        PlayerInfo wang = new("wanglebangle", dict[EnumAR.Resistance]);
        PlayerInfo side = new("sidewalkill", dict[EnumAR.Resistance]);
        PlayerInfo z123 = new("z123", dict[EnumAR.Resistance]);
        PlayerInfo bum = new("bumfuzzle28", dict[EnumAR.Resistance]);
        PlayerInfo milli = new("milli", dict[EnumAR.Resistance]);
        PlayerInfo gus = new("Gus", dict[EnumAR.Resistance]);
        PlayerInfo murph = new("Murph", dict[EnumAR.Resistance]);
        PlayerInfo jer = new("Jer", dict[EnumAR.Resistance]);
        PlayerInfo obj = new("Object", dict[EnumAR.Resistance]);
        PlayerInfo rc = new("RadiantCowbells", dict[EnumAR.Resistance]);
        
        var playersInOrder = new PlayerInfo[] { pingu, rose, oolong, wang, side, z123, bum, milli, gus, murph, jer, obj, rc };

        // Mission status information
        MissionStatus[] missionInfo = new MissionStatus[7] {
            MissionStatus.None,
            MissionStatus.None,
            MissionStatus.None,
            MissionStatus.None,
            MissionStatus.None,
            MissionStatus.None,
            MissionStatus.None,
        };
        // Mission size information
        string[] sizes = new string[] { "4", "5", "6", "7*", "6", "7*", "7" };

        bool showAvatars = true;

        // Players on mission information
        bool missionSelected = false; // True = Display crown + shields. Set to "false" for initial role giveout.
        PlayerInfo[] leaders = new PlayerInfo[] {  };
        PlayerInfo[] onMission = new PlayerInfo[] {  };

        // Player to be banished. They appear slightly transparent with "BANISHED" over their avatar.
        PlayerInfo banishedPlayer = null;

        // Player to be self-bombed (Oracle bomb). They appear slightly transparent with an explosion over their avatar.
        PlayerInfo bombedPlayer = null;
        PlayerInfo bombRessedPlayer = null;

        // Hammer holder. This player has the hammer icon below their name.
        string hammerHolder = null;
        
        // Powers.
        // Top array = the names of the powers (refer to "powers" folder for image names).
        // Bottom array = the names of the players holding each power.
        var powers = new string[] { "ref", "lady" };
        var powerHolders = new PlayerInfo[] { pingu, z123 };

        // Game ending information. Display all roles + assassination choice.
        bool gameEnd = true;
        PlayerInfo[] assassinationChoice = new PlayerInfo[] { z123, pingu };

        // END EDITING BLOCK

        var positions = GeneratePlayerLocations(playersInOrder.Length);
        string mainSvg = "";
        mainSvg += $@"<rect
                    x=""0""
                    y=""0""
                    width=""{width}""
                    height=""{height}""
                    fill=""#404040""
                />";
        var playerSvgs = new string[playersInOrder.Length];
        for (int i = 0; i < playerSvgs.Length; i++)
        {
            playerSvgs[i] += $@"<rect
                    x=""0""
                    y=""0""
                    width=""{width}""
                    height=""{height}""
                    fill=""#404040""
                />";
        }

        for (int i = 0; i < missionInfo.Length; i++)
        {
            var color = missionInfo[i] == MissionStatus.None ? "rgb(150,150,150)" : missionInfo[i] == MissionStatus.Success ? "rgb(0,68,204)" : "rgb(189,54,47)";
            var str = $@"<g>
                <rect
                    width=""85""
                    height=""60""
                    x=""{620 + (i * 100)}""
                    y=""330""
                    rx=""10""
                    style=""fill:{color};""
                />
                <text
                    x=""{655 + (i * 100)}""
                    y=""370""
                    font-family=""Arial""
                    fill=""white""
                    font-size=""32px""
                >{sizes[i]}</text>
                </g>";
            mainSvg += str;
            for (int p = 0; p < playerSvgs.Length; p++)
                playerSvgs[p] += str;
        }

        int[] leaderIxs = Enumerable.Range(0, leaders.Length).Select(i => Array.IndexOf(playersInOrder, leaders[i])).Where(i => i != -1).ToArray();
        int[] onMissionIx = Enumerable.Range(0, onMission.Length).Select(i => Array.IndexOf(playersInOrder, onMission[i])).Where(i => i != -1).ToArray();
        var playerAlliances = Enumerable.Range(0, playersInOrder.Length).Select(i => !gameEnd ? "res" : playersInOrder[i].Info.IsSpy ? "spy" : "res").ToArray();
        int banishedIx = Array.IndexOf(playersInOrder, banishedPlayer);
        int bombedIx = Array.IndexOf(playersInOrder, bombedPlayer);
        int bombRessedIx = Array.IndexOf(playersInOrder, bombRessedPlayer);
        int[] assassinationChoiceIxs = Enumerable.Range(0, assassinationChoice.Length).Select(i => Array.IndexOf(playersInOrder, assassinationChoice[i])).Where(i => i != -1).ToArray();
        
        for (int i = 0; i < playersInOrder.Length; i++)
        {
            var x = positions[i].x - imgSize / 2;
            var y = positions[i].y - imgSize / 2;
            var halfOpacs = new[] { banishedIx, bombedIx };
            var opacity = halfOpacs.Contains(i) ? 0.5 : 1;
            string avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\0default-{playerAlliances[i]}.png"));
            if (showAvatars)
            {
                try
                {
                    avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\{playersInOrder[i].PlayerName}-{playerAlliances[i]}.png"));
                }
                catch (FileNotFoundException)
                {
                    avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\0default-{playerAlliances[i]}.png"));
                }
            }
            if (bombRessedIx != -1 && i == bombRessedIx)
            {
                mainSvg += $@"<g transform=""translate({positions[bombRessedIx].x - 120}, {positions[bombRessedIx].y - 120})"">
                <image
                    width=""{imgSize * 1.5}""
                    height=""{imgSize * 1.5}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\bombressed.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
            }
            var strA = $@"<g transform=""translate({x}, {y})"">
                <image
                    width=""{imgSize}""
                    height=""{imgSize}""
                    xlink:href=""data:image/png;base64,{avatarStr}""
                    x=""0""
                    y=""0""
                    opacity=""{opacity}""
                    />
                </g>";
            var strB = $@"<g transform=""translate({x}, {y})"">
                <text
                    x=""{imgSize / 2}""
                    y=""180""
                    dominant-baseline=""middle""
                    text-anchor=""middle""
                    font-family=""Arial""
                    fill=""white""
                    font-size=""26px""
                    font-weight=""bold""
                    >{playersInOrder[i].PlayerName}</text>
                </g>";
            mainSvg += strA;
            mainSvg += strB;
        }
        if (banishedIx != -1)
        {
            mainSvg += $@"<g transform=""translate({positions[banishedIx].x - 80}, {positions[banishedIx].y - 80})"">
                <image
                    width=""{imgSize}""
                    height=""{imgSize}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\banished.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
        }
        if (bombedIx != -1)
        {
            mainSvg += $@"<g transform=""translate({positions[bombedIx].x - 80}, {positions[bombedIx].y - 80})"">
                <image
                    width=""{imgSize}""
                    height=""{imgSize}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\bombed.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
        }
        

        // Assasasination and roles
        if (gameEnd)
        {
            for (int i = 0; i < playersInOrder.Length; i++)
            {
                var x = positions[i].x - imgSize / 2;
                var y = positions[i].y - imgSize / 2;
                mainSvg += $@"<g transform=""translate({x}, {y})"">
                    <text
                        x=""{imgSize / 2}""
                        y=""210""
                        dominant-baseline=""middle""
                        text-anchor=""middle""
                        font-family=""Arial""
                        fill=""white""
                        font-size=""16px""
                        font-weight=""bold""
                        >{playersInOrder[i].Info.RoleName}</text>
                    </g>";
            }
            if (!assassinationChoiceIxs.Contains(-1))
                for (int i = 0; i < assassinationChoice.Length; i++)
                {
                    mainSvg += $@"<g transform=""translate({positions[assassinationChoiceIxs[i]].x - 80}, {positions[assassinationChoiceIxs[i]].y - 95})"">
                        <image
                            width=""{imgSize}""
                            height=""{imgSize}""
                            xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\sword.png"))}""
                            x=""0""
                            y=""0""
                            />
                        </g>";
                }
        }

        for (int svgIx = 0; svgIx < playerSvgs.Length; svgIx++)
        {
            var isVisibleSpy = Enumerable.Range(0, playersInOrder.Length).Select(i => "res").ToArray();
            var spyTeamA = new[] { EnumAR.Assassin, EnumAR.Morgana, EnumAR.Witch, EnumAR.Mordred, EnumAR.Spy };
            var spyTeamB = new[] { EnumAR.MordredA, EnumAR.MordredB};
            var spyTeamC = new[] { EnumAR.Oberon };
            var roleStrings = new string[playersInOrder.Length];
            roleStrings[svgIx] = playersInOrder[svgIx].Info.RoleName;
            Console.WriteLine(roleStrings[svgIx]);
            if (playersInOrder[svgIx].Info.Role == EnumAR.Merlin)
            {
                isVisibleSpy = Enumerable.Range(0, playersInOrder.Length).Select(x => playersInOrder[x].Info.IsSpy && playersInOrder[x].Info.IsVisibleToMerlin ? "spy" : "res").ToArray();
            }
            else if (playersInOrder[svgIx].Info.IsSpy && spyTeamA.Contains(playersInOrder[svgIx].Info.Role))
            {
                isVisibleSpy = Enumerable.Range(0, playersInOrder.Length).Select(x => spyTeamA.Contains(playersInOrder[x].Info.Role) ? "spy" : "res").ToArray();
            }
            else if (playersInOrder[svgIx].Info.IsSpy && spyTeamB.Contains(playersInOrder[svgIx].Info.Role))
            {
                isVisibleSpy = Enumerable.Range(0, playersInOrder.Length).Select(x => spyTeamB.Contains(playersInOrder[x].Info.Role) ? "spy" : "res").ToArray();
            }
            else if (playersInOrder[svgIx].Info.IsSpy && spyTeamC.Contains(playersInOrder[svgIx].Info.Role))
            {
                isVisibleSpy = Enumerable.Range(0, playersInOrder.Length).Select(x => spyTeamC.Contains(playersInOrder[x].Info.Role) ? "spy" : "res").ToArray();
            }
            else if (playersInOrder[svgIx].Info.Role == EnumAR.Percival)
            {
                var pMerlins = Enumerable.Range(0, playersInOrder.Length).Where(p => playersInOrder[p].Info.IsPercivalCandidate).ToArray();
                for (int i = 0; i < pMerlins.Length; i++)
                    roleStrings[pMerlins[i]] = "Merlin?";
            }
            else if (playersInOrder[svgIx].Info.Role == EnumAR.Isolde || playersInOrder[svgIx].Info.Role == EnumAR.Tristan)
            {
                var isoldeIx = Enumerable.Range(0, playersInOrder.Length).Where(p => playersInOrder[p].Info.Role == EnumAR.Isolde).First();
                var tristanIx = Enumerable.Range(0, playersInOrder.Length).Where(p => playersInOrder[p].Info.Role == EnumAR.Tristan).First();
                roleStrings[isoldeIx] = "Isolde";
                roleStrings[tristanIx] = "Tristan";
            }

            int x;
            int y;
            for (int playerIx = 0; playerIx < playersInOrder.Length; playerIx++)
            {
                x = positions[playerIx].x - imgSize / 2;
                y = positions[playerIx].y - imgSize / 2;
                var alliance = isVisibleSpy[playerIx];
                string avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\0default-{alliance}.png"));
                if (showAvatars)
                {
                    try
                    {
                        avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\{playersInOrder[playerIx].PlayerName}-{alliance}.png"));
                    }
                    catch (FileNotFoundException)
                    {
                        avatarStr = Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\avatars\0default-{alliance}.png"));
                    }
                }

                playerSvgs[svgIx] += $@"<g transform=""translate({x}, {y})"">
                    <image
                        width=""{imgSize}""
                        height=""{imgSize}""
                        style=""fill-opacity:0.5""
                        xlink:href=""data:image/png;base64,{avatarStr}""
                        data-player=""{playersInOrder[playerIx].PlayerName}""
                        data-role=""{isVisibleSpy[playerIx]}""
                        x=""0""
                        y=""0""
                        />
                    </g>";
                playerSvgs[svgIx] += $@"<g transform=""translate({x}, {y})"">
                    <text
                        x=""{imgSize / 2}""
                        y=""180""
                        dominant-baseline=""middle""
                        text-anchor=""middle""
                        font-family=""Arial""
                        fill=""white""
                        font-size=""26px""
                        font-weight=""bold""
                        >{playersInOrder[playerIx].PlayerName}</text>
                    <text
                        x=""{imgSize / 2}""
                        y=""210""
                        dominant-baseline=""middle""
                        text-anchor=""middle""
                        font-family=""Arial""
                        fill=""white""
                        font-size=""20px""
                        font-weight=""bold""
                        >{roleStrings[playerIx]}</text>
                    </g>";
            }
        }
        if (missionSelected)
        {
            for (int i = 0; i < leaderIxs.Length; i++)
            {
                mainSvg += $@" <g transform=""translate({(positions[leaderIxs[i]].x - (imgSize / 2) + 80)}, {(positions[leaderIxs[i]].y - (imgSize / 2)) + 100})"">
                <image
                    width=""{imgSize * 0.5f}""
                    height=""{imgSize * 0.5f}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\mission\crown.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
            }
            
            for (int i = 0; i < onMissionIx.Length; i++)
            {
                mainSvg += $@"<g transform=""translate({(positions[onMissionIx[i]].x - (imgSize / 2) - 15)}, {(positions[onMissionIx[i]].y - (imgSize / 2)) + 105})"">
                    <image
                        width=""{imgSize * 0.4f}""
                        height=""{imgSize * 0.4f}""
                        xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\mission\shield.png"))}""
                        x=""0""
                        y=""0""
                        />
                    </g>";
            }
        }

        var hammerHolderPos = Array.IndexOf(playersInOrder, hammerHolder);
        if (!gameEnd && hammerHolderPos != -1)
        {
            mainSvg += $@"<g transform=""translate({(positions[hammerHolderPos].x - 22.5)}, {(positions[hammerHolderPos].y + 110)})"">
                <image
                    width=""{imgSize * 0.25f}""
                    height=""{imgSize * 0.25f}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\hammer.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
        }

        var powerHolderPos = Enumerable.Range(0, powers.Length).Select(i => Array.IndexOf(playersInOrder, powerHolders[i])).ToArray();
        for (int i = 0; i < powers.Length; i++)
        {
            mainSvg += $@"<g transform=""translate({(positions[powerHolderPos[i]].x - 80)}, {positions[powerHolderPos[i]].y - 75})"">
                <image
                    width=""{imgSize * 0.3f}""
                    height=""{imgSize * 0.3f}""
                    xlink:href=""data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes($@"..\..\..\..\powers\{powers[i]}.png"))}""
                    x=""0""
                    y=""0""
                    />
                </g>";
        }
        File.WriteAllText(@"..\..\..\..\image.svg", @$"<svg
            xmlns=""http://www.w3.org/2000/svg""
            xmlns:xlink=""http://www.w3.org/1999/xlink""
            viewBox=""0 0 {width} {height}"">
            {mainSvg}</svg>
        ");

        for (int i = 0; i < playersInOrder.Length; i++)
        {
            File.WriteAllText(@$"..\..\..\..\playerSvgs\{playersInOrder[i].PlayerName}.svg", @$"<svg
            xmlns=""http://www.w3.org/2000/svg""
            xmlns:xlink=""http://www.w3.org/1999/xlink""
            viewBox=""0 0 {width} {height}"">
            {playerSvgs[i]}</svg>
        ");
        }
    }

    public static class ListShuffler
    {
        private static Random rng = new Random(); // Initialize a single Random instance

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--; // Decrement n to represent the current upper bound of the unshuffled portion
                int k = rng.Next(n + 1); // Generate a random index k between 0 and n (inclusive)
                T value = list[k]; // Store the value at the random index
                list[k] = list[n]; // Move the value from the current upper bound to the random index
                list[n] = value; // Move the stored value to the current upper bound
            }
        }
    }
}