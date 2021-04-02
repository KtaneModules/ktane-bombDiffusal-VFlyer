using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using rnd = UnityEngine.Random;

public class bombDiffusalScript : MonoBehaviour
{
    public KMBombInfo bomb;
    public KMAudio Audio;
    public KMBombModule modSelf;
    public KMSelectable selectableSelf;

    public TextMesh licenseNoText;
    public RenderPlate[] plates;

    public GameObject[] menus;

    public GameObject manifest;
    public TextMesh manifestText;
    public GameObject postStamp;
    public GameObject stamp;

    public Material[] usa1Stamp;
    public Material[] usa2Stamp;
    public Material[] americaStamp;
    public Material[] eurasiaStamp;
    public Material[] africaStamp;
    public Material[] spaceStamp;

    public Material[] usa1;
    public Material[] usa2;
    public Material[] america;
    public Material[] eurasia;
    public Material[] africa;
    public Material[] space;

    public KMSelectable destinationButton;
    public KMSelectable componentButton;
    public KMSelectable goButton;
    public TextMesh destinationScn;
    public TextMesh specsScn;

    public KMSelectable nextSector;
    public KMSelectable prevSector;
    public KMSelectable nextArea;
    public KMSelectable prevArea;
    public KMSelectable destinationBack;
    public TextMesh sectorScn;
    public TextMesh areaScn;
    public Renderer flagScn;

    public KMSelectable addBattery;
    public KMSelectable subBattery;
    public KMSelectable addIndicator;
    public KMSelectable subIndicator;
    public KMSelectable nextPort;
    public KMSelectable prevPort;
    public KMSelectable addManual;
    public KMSelectable subManual;
    public KMSelectable componentBack;
    public TextMesh batteryScn;
    public TextMesh indicatorsScn;
    public TextMesh portsScn;
    public TextMesh manualScn;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;

    int port1, port2;
    string[] ports;
    string licenseNo;
    List<int> deliveryNo;
    int goalDestination;
    int goalBatteries;
    int goalIndicators;
    int goalPortIdx;
    int goalManuals;

    int selectedDestination = -1;
    int selectedBatteries = -1;
    int selectedIndicators = -1;
    int selectedPortIdx = -1;
    int selectedManuals = -1;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        destinationButton.OnInteract += delegate () { OpenDestinationMenu(); return false; };
        componentButton.OnInteract += delegate () { OpenComponentMenu(); return false; };
        destinationBack.OnInteract += delegate () { OpenMainMenu(); return false; };
        componentBack.OnInteract += delegate () { OpenMainMenu(); return false; };
        nextSector.OnInteract += delegate () { ChangeSector(1); return false; };
        prevSector.OnInteract += delegate () { ChangeSector(-1); return false; };
        nextArea.OnInteract += delegate () { ChangeArea(1); return false; };
        prevArea.OnInteract += delegate () { ChangeArea(-1); return false; };
        addBattery.OnInteract += delegate () { ChangeBatteries(1); return false; };
        subBattery.OnInteract += delegate () { ChangeBatteries(-1); return false; };
        addIndicator.OnInteract += delegate () { ChangeIndicators(1); return false; };
        subIndicator.OnInteract += delegate () { ChangeIndicators(-1); return false; };
        nextPort.OnInteract += delegate () { ChangePorts(1); return false; };
        prevPort.OnInteract += delegate () { ChangePorts(-1); return false; };
        addManual.OnInteract += delegate () { ChangeManuals(1); return false; };
        subManual.OnInteract += delegate () { ChangeManuals(-1); return false; };
        goButton.OnInteract += delegate () { CheckSolution(); return false; };
    }

    void UpdateSelectables(int idxNum)
    {
        switch (idxNum)
        {
            case 0:
                selectableSelf.Children = new KMSelectable[] { destinationButton, null, componentButton, null, goButton, null, };
                selectableSelf.ChildRowLength = 2;
                break;
            case 1:
                selectableSelf.Children = new KMSelectable[] { prevSector, nextSector, prevArea, nextArea, destinationBack, null };
                selectableSelf.ChildRowLength = 2;
                break;
            case 2:
                selectableSelf.Children = new KMSelectable[] { subBattery, addBattery, subIndicator, addIndicator, prevPort, nextPort, subManual, addManual, componentBack, null };
                selectableSelf.ChildRowLength = 2;
                break;
            default:
                selectableSelf.Children = new KMSelectable[0];
                selectableSelf.ChildRowLength = 1;
                break;
        }
        selectableSelf.UpdateChildren();
        //selectableSelf.UpdateChildren(selectableSelf.Children.FirstOrDefault());
    }

    void UpdateDestinationMenu() // Update the Destination Screen on the module
    {
        sectorScn.text = GetSectorName(selectedDestination / 100);
        areaScn.text = GetAreaName(selectedDestination);
        flagScn.material = GetFlag(selectedDestination);
    }
    void UpdateComponentMenu() // Update the Component Screen on the module
    {
        batteryScn.text = string.Format("Batteries: {0}", selectedBatteries);
        indicatorsScn.text = string.Format("Indicators: {0}", selectedIndicators);
        portsScn.text = ports[selectedPortIdx] == "PS2" ? "PS/2" : ports[selectedPortIdx];
        manualScn.text = string.Format("Manuals: {0}", selectedManuals);
    }
    void UpdateMainMenu()
    {
        destinationScn.text = string.Format("Sector: {0}\nArea: {1}", GetSectorName(selectedDestination / 100), GetAreaName(selectedDestination));
        specsScn.text = string.Format("\nComponents: {0}/{1}/{2}\nPort: {3}",
            selectedBatteries != -1 ? selectedBatteries.ToString() : "?",
            selectedIndicators != -1 ? selectedIndicators.ToString() : "?",
            selectedManuals != -1 ? selectedManuals.ToString() : "?",
            selectedPortIdx != -1 ? ports[selectedPortIdx] : "???");
    }

    void OpenDestinationMenu()
    {
        Audio.PlaySoundAtTransform("button", transform);

        if (selectedDestination == -1)
            selectedDestination = 100;

        UpdateDestinationMenu();

        menus[0].SetActive(false);
        menus[1].SetActive(true);
        UpdateSelectables(1);
    }

    void OpenComponentMenu()
    {
        Audio.PlaySoundAtTransform("button", transform);

        if (selectedBatteries == -1)
        {
            selectedBatteries = 0;
            selectedIndicators = 0;
            selectedPortIdx = 0;
            selectedManuals = 0;
        }

        UpdateComponentMenu();

        menus[0].SetActive(false);
        menus[2].SetActive(true);
        UpdateSelectables(2);
    }

    void OpenMainMenu()
    {
        Audio.PlaySoundAtTransform("button", transform);

        UpdateMainMenu();

        menus[1].SetActive(false);
        menus[2].SetActive(false);
        menus[0].SetActive(true);
        UpdateSelectables(0);
    }

    void ChangeSector(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedDestination += i * 100;

        if (selectedDestination >= 700)
            selectedDestination -= 600;
        if (selectedDestination < 100)
            selectedDestination += 600;

        UpdateDestinationMenu();
    }

    void ChangeArea(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedDestination += i;

        if (selectedDestination % 10 == 5)
            selectedDestination += 5;
        if (selectedDestination % 10 == 9)
            selectedDestination -= 5;
        if (selectedDestination % 100 / 10 == 5)
            selectedDestination = selectedDestination / 100 * 100;
        if (selectedDestination % 100 / 10 == 9)
            selectedDestination = (selectedDestination / 100) * 100 + 144;

        UpdateDestinationMenu();
    }

    void ChangeBatteries(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedBatteries += i;

        if (selectedBatteries <= -1)
            selectedBatteries = 0;
        if (selectedBatteries >= 10)
            selectedBatteries = 9;

        UpdateComponentMenu();
    }

    void ChangeIndicators(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedIndicators += i;

        if (selectedIndicators <= -1)
            selectedIndicators = 0;
        if (selectedIndicators >= 10)
            selectedIndicators = 9;

        UpdateComponentMenu();
    }

    void ChangePorts(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedPortIdx += i;

        if (selectedPortIdx <= -1)
            selectedPortIdx = ports.Length - 1;
        if (selectedPortIdx >= ports.Length)
            selectedPortIdx = 0;

        UpdateComponentMenu();
    }

    void ChangeManuals(int i)
    {
        Audio.PlaySoundAtTransform("button", transform);

        selectedManuals += i;

        if (selectedManuals == -1)
            selectedManuals = 0;
        if (selectedManuals == 10)
            selectedManuals = 9;

        UpdateComponentMenu();
    }

    void CheckSolution()
    {
        Audio.PlaySoundAtTransform("button", transform);

        var isAllFilledOut = true;
        var missingDebug = new List<string>();
        if (selectedDestination == -1)
        {
            isAllFilledOut = false;
            missingDebug.Add("Destination");
        }
        if (selectedManuals == -1 || selectedPortIdx == -1 || selectedIndicators == -1 || selectedBatteries == -1)
        {
            isAllFilledOut = false;
            missingDebug.Add("Components");
        }

        if (!isAllFilledOut)
        {
            modSelf.HandleStrike();
            Debug.LogFormat("[Bomb Diffusal #{0}] Strike! Infomation is missing from the following menus: [{1}]", moduleId, missingDebug.Join(", "));
            return;
        }

        bool isAllCorrect = true;

        if (goalDestination != selectedDestination)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Incorrectly selected destination was {1}. Expected {2}.", moduleId, GetAreaName(selectedDestination), GetAreaName(goalDestination));
            isAllCorrect = false;
        }
        if (goalBatteries != selectedBatteries)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Incorrectly selected {1} batter{3}. Expected {2}.", moduleId, selectedBatteries, goalBatteries, goalBatteries == 1 ? "y" : "ies");
            isAllCorrect = false;
        }
        if (goalIndicators != selectedIndicators)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Incorrectly selected {1} indicator(s). Expected {2}.", moduleId, selectedIndicators, goalIndicators);
            isAllCorrect = false;
        }
        if (goalPortIdx != selectedPortIdx)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Incorrectly selected {1} port. Expected {2} port.", moduleId, ports[selectedPortIdx], ports[goalPortIdx]);
            isAllCorrect = false;
        }
        if (goalManuals != selectedManuals)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Incorrectly selected {1} manual(s). Expected {2}.", moduleId, selectedManuals, goalManuals);
            isAllCorrect = false;
        }

        if (!isAllCorrect)
        {
            Debug.LogFormat("[Bomb Diffusal #{0}] Strike! Not all conditions are correctly selected!", moduleId);
            modSelf.HandleStrike();
            return;
        }

        Debug.LogFormat("[Bomb Diffusal #{0}] Input is correct. Module solved.", moduleId);

        menus[0].SetActive(false);
        menus[3].SetActive(true);

        StartCoroutine(PrintManifest());
    }

    void Start()
    {
        SetUpPorts();
        GenerateNumbers();
        CalcDestination();
        CalcComponents();

        menus[1].SetActive(false);
        menus[2].SetActive(false);
        UpdateSelectables(0);
    }

    void SetUpPorts()
    {
        port1 = rnd.Range(0, 13);
        port2 = rnd.Range(0, 13);

        Debug.LogFormat("[Bomb Diffusal #{0}] Port 1: {1}. Port 2: {2}.", moduleId, GetPortName(port1), GetPortName(port2));

        plates[0].RenderPort(port1);
        plates[1].RenderPort(port2);
    }

    void GenerateNumbers()
    {
        do
        {
            GenerateLicenseNo();
            GenerateDeliveryNo();
        } while (CheckValidDeliveryNo());

        licenseNoText.text = "Software License No.:\n" + licenseNo;

        Debug.LogFormat("[Bomb Diffusal #{0}] Software license number: {1}.", moduleId, licenseNo);
        Debug.LogFormat("[Bomb Diffusal #{0}] Delivery number: {1}.", moduleId, deliveryNo.Join(""));
    }

    void CalcDestination()
    {
        for (int i = 9; i >= 0; i--)
        {
            if (deliveryNo.FindAll(x => x == i).Count() == 1)
            {
                goalDestination = (deliveryNo.IndexOf(i) + 1) * 100;
                Debug.LogFormat("[Bomb Diffusal #{0}] Delivery sector: {1}.", moduleId, GetSectorName(deliveryNo.IndexOf(i) + 1));
                break;
            }
        }

        if (goalDestination == 600)
        {
            goalDestination += deliveryNo.ElementAt(5) % 5 * 10
                + deliveryNo.ElementAt(4) % 5;
            Debug.LogFormat("[Bomb Diffusal #{0}] (Delivery is in space. Reversing number for next step.)", moduleId);
        }
        else
        {
            goalDestination += deliveryNo.ElementAt(0) % 5 * 10
                + deliveryNo.ElementAt(1) % 5;
        }

        Debug.LogFormat("[Bomb Diffusal #{0}] Delivery area: {1}.", moduleId, GetAreaName(goalDestination));
    }

    void CalcComponents()
    {
        CalcBatteries();
        CalcIndicators();
        CalcPorts();
        CalcManuals();
    }

    void CalcBatteries()
    {
        goalBatteries = (bomb.GetBatteryCount() + bomb.GetBatteryHolderCount() + 1) % 10;
        Debug.LogFormat("[Bomb Diffusal #{0}] Required number of batteries: {1}.", moduleId, goalBatteries);
    }

    void CalcIndicators()
    {
        if (bomb.GetSerialNumberNumbers().Count() == 4)
        {
            goalIndicators = bomb.GetIndicators().Count() % 10;
        }
        else if (bomb.GetSerialNumberNumbers().Count() == 3)
        {
            goalIndicators = (bomb.GetOnIndicators().Count() * 2) % 10;
        }
        else
        {
            goalIndicators = (bomb.GetOffIndicators().Count() * 2) % 10;
        }
        Debug.LogFormat("[Bomb Diffusal #{0}] Required number of indicators: {1}.", moduleId, goalIndicators);
    }

    void CalcPorts()
    {
        ports = new string[] { "PS2", "Serial", "Parallel", "AC Power", "HDMI", "VGA", "USB", "RJ-45", "DVI-D", "Stereo RCA", "Component Video", "Composite Video", "PCMCIA" }.Shuffle();

        goalPortIdx = ports.ToList().IndexOf(GetPortName(port1)) - port2;
        while (goalPortIdx < 0) goalPortIdx += ports.Count();

        Debug.LogFormat("[Bomb Diffusal #{0}] Port menu order: [ {1} ].", moduleId, ports.Join(", "));
        Debug.LogFormat("[Bomb Diffusal #{0}] Required port type: {1}.", moduleId, ports[goalPortIdx]);
    }

    void CalcManuals()
    {
        goalManuals = 1;

        if ((goalDestination / 100) <= 2 || (goalDestination / 100) == 4)
            goalManuals += 2 * bomb.GetPortPlateCount();
        else if ((goalDestination / 100) == 3 || (goalDestination / 100) == 5)
            goalManuals += bomb.GetBatteryHolderCount();
        else if (goalDestination == 600 || goalDestination == 601 || goalDestination == 633)
            goalManuals = 9;

        if (bomb.GetSerialNumberNumbers().ElementAt(bomb.GetSerialNumberNumbers().Count() - 1) % 2 == 0)
            goalManuals += 1;

        goalManuals = Math.Min(9, goalManuals);

        Debug.LogFormat("[Bomb Diffusal #{0}] Required number of manuals: {1}.", moduleId, goalManuals);
    }

    void GenerateLicenseNo()
    {
        licenseNo = "";

        int nLetters = rnd.Range(0, 3) + 2;
        int nNumbers = 6 - nLetters;

        for (int i = 0; i < nLetters; i++)
            licenseNo += (char)(rnd.Range(0, 26) + 'A');
        for (int i = 0; i < nNumbers; i++)
            licenseNo += (char)(rnd.Range(0, 10) + '0');

        licenseNo = licenseNo.ToCharArray().Shuffle().Join("");
    }

    void GenerateDeliveryNo()
    {
        deliveryNo = new List<int>();
        string sn = bomb.GetSerialNumber();

        string[] rangesTables = { "ABCDE", "FGHIJ", "KLMNO", "PQRST", "UVWXY", "Z01234", "56789" };

        int[,] digitTable = {
            { 3, 9, 6, 3, 3, 1, 4 },
            { 5, 6, 8, 5, 9, 5, 0 },
            { 6, 2, 7, 3, 8, 6, 7 },
            { 1, 4, 6, 9, 8, 9, 1 },
            { 2, 3, 9, 7, 2, 7, 2 },
            { 5, 1, 8, 2, 8, 4, 4 },
            { 0, 1, 0, 7, 5, 4, 0 },
        };


        for (int i = 0; i < sn.Length; i++)
        {
            int idxLis = -1;
            for (int x = 0; x < rangesTables.Length; x++)
            {
                if (rangesTables[x].Contains(licenseNo[i]))
                {
                    idxLis = x;
                    break;
                }
            }
            int idxSn = -1;
            for (int x = 0; x < rangesTables.Length; x++)
            {
                if (rangesTables[x].Contains(sn[i]))
                {
                    idxSn = x;
                    break;
                }
            }
            //Debug.LogFormat("{0},{1}", idxLis, idxSn);
            if (idxSn >= 0 && idxLis >= 0)
                deliveryNo.Add(digitTable[idxSn, idxLis]);

        }
    }

    bool CheckValidDeliveryNo()
    {
        foreach (int n in deliveryNo)
            if (deliveryNo.FindAll(x => x == n).Count() == 1) return false;

        return true;
    }

    string GetPortName(int port)
    {
        switch (port)
        {
            case 0: return "PS2";
            case 1: return "Serial";
            case 2: return "Parallel";
            case 3: return "AC Power";
            case 4: return "HDMI";
            case 5: return "VGA";
            case 6: return "USB";
            case 7: return "RJ-45";
            case 8: return "DVI-D";
            case 9: return "Stereo RCA";
            case 10: return "Component Video";
            case 11: return "Composite Video";
            case 12: return "PCMCIA";
        }

        return "???";
    }

    Dictionary<int, string> sectorNames = new Dictionary<int, string>() {
        { 1, "USA #1" },
        { 2, "USA #2" },
        { 3, "Rest of America" },
        { 4, "Eurasia" },
        { 5, "Africa" },
        { 6, "Space" },
    };
    Dictionary<int, string> areaNames = new Dictionary<int, string>() {
        // USA #1
        { 100, "Alabama" },
        { 101, "Alaska" },
        { 102, "Arizona" },
        { 103, "Arkansas" },
        { 104, "California" },
        { 110, "Colorado" },
        { 111, "Connecticut" },
        { 112, "Delaware" },
        { 113, "Florida" },
        { 114, "Georgia" },
        { 120, "Hawaii" },
        { 121, "Idaho" },
        { 122, "Illinois" },
        { 123, "Indiana" },
        { 124, "Iowa" },
        { 130, "Kansas" },
        { 131, "Kentucky" },
        { 132, "Louisiana" },
        { 133, "Maine" },
        { 134, "Maryland" },
        { 140, "Massachusetts" },
        { 141, "Michigan" },
        { 142, "Minnesota" },
        { 143, "Mississippi" },
        { 144, "Missouri" },

        { 200, "Montana" },
        { 201, "Nebraska" },
        { 202, "Nevada" },
        { 203, "New Hampshire" },
        { 204, "New Jersey" },
        { 210, "New Mexico" },
        { 211, "New York" },
        { 212, "North Carolina" },
        { 213, "North Dakota" },
        { 214, "Ohio" },
        { 220, "Oklahoma" },
        { 221, "Oregon" },
        { 222, "Pennsylvania" },
        { 223, "Rhode Island" },
        { 224, "South Carolina" },
        { 230, "South Dakota" },
        { 231, "Tennessee" },
        { 232, "Texas" },
        { 233, "Utah" },
        { 234, "Vermont" },
        { 240, "Virginia" },
        { 241, "Washington" },
        { 242, "West Virginia" },
        { 243, "Wisconsin" },
        { 244, "Wyoming" },

        { 300, "Canada" },
        { 301, "Mexico" },
        { 302, "Argentina" },
        { 303, "Brazil" },
        { 304, "Bolivia" },
        { 310, "Chile" },
        { 311, "Colombia" },
        { 312, "Ecuador" },
        { 313, "Guyana" },
        { 314, "Paraguay" },
        { 320, "Peru" },
        { 321, "Suriname" },
        { 322, "Uruguay" },
        { 323, "Venezuela" },
        { 324, "Belize" },
        { 330, "Costa Rica" },
        { 331, "El Salvador" },
        { 332, "Guatemala" },
        { 333, "Honduras" },
        { 334, "Nicaragua" },
        { 340, "Panama" },
        { 341, "Dominican Republic" },
        { 342, "Bahamas" },
        { 343, "Barbados" },
        { 344, "Haiti" },

        { 400, "China" },
        { 401, "Germany" },
        { 402, "India" },
        { 403, "France" },
        { 404, "Indonesia" },
        { 410, "Croatia" },
        { 411, "Pakistan" },
        { 412, "Spain" },
        { 413, "Bangladesh" },
        { 414, "Italy" },
        { 420, "Japan" },
        { 421, "United Kingdom" },
        { 422, "Philippines" },
        { 423, "Switzerland" },
        { 424, "Vietnam" },
        { 430, "Belgium" },
        { 431, "Iran" },
        { 432, "Greece" },
        { 433, "South Korea" },
        { 434, "Netherlands" },
        { 440, "Laos" },
        { 441, "Poland" },
        { 442, "Thailand" },
        { 443, "Sweden" },
        { 444, "Russia" },

        { 500, "South Africa" },
        { 501, "Nigeria" },
        { 502, "Morocco" },
        { 503, "Kenya" },
        { 504, "Senegal" },
        { 510, "Ghana" },
        { 511, "DRC" },
        { 512, "Ethiopia" },
        { 513, "Algeria" },
        { 514, "Tanzania" },
        { 520, "Tunisia" },
        { 521, "Cameroon" },
        { 522, "Uganda" },
        { 523, "Mali" },
        { 524, "Zimbabwe" },
        { 530, "Madagascar" },
        { 531, "Angola" },
        { 532, "Sudan" },
        { 533, "Namibia" },
        { 534, "Zambia" },
        { 540, "Somalia" },
        { 541, "Libya" },
        { 542, "Niger" },
        { 543, "Swaziland" },
        { 544, "Egypt" },

        { 600, "Mercury" },
        { 601, "Venus" },
        { 602, "Mars" },
        { 603, "Jupiter" },
        { 604, "Saturn" },
        { 610, "Uranus" },
        { 611, "Neptune" },
        { 612, "The Moon" },
        { 613, "Titan" },
        { 614, "Io" },
        { 620, "Europa" },
        { 621, "Triton" },
        { 622, "Callisto" },
        { 623, "Ganymede" },
        { 624, "Rhea" },
        { 630, "Umbriel" },
        { 631, "Oberon" },
        { 632, "Phoebe" },
        { 633, "The Sun" },
        { 634, "Asteroid Belt" },
        { 640, "Pluto" },
        { 641, "Kepler-1638b" },
        { 642, "The ISS" },
        { 643, "Kepler-1229b" },
        { 644, "Kepler-452b" },
    };


    string GetSectorName(int sector)
    {
        return sectorNames.ContainsKey(sector) ? sectorNames[sector] : "???";
    }
   
    string GetAreaName(int area)
    {
        return areaNames.ContainsKey(area) ? areaNames[area] : "???";
    }

    Material GetFlag(int dest)
    {
        switch (dest / 100)
        {
            case 1: return usa1[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 2: return usa2[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 3: return america[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 4: return eurasia[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 5: return africa[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 6: return space[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
        }

        return null;
    }

    Material GetStamp(int dest)
    {
        switch (dest / 100)
        {
            case 1: return usa1Stamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 2: return usa2Stamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 3: return americaStamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 4: return eurasiaStamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 5: return africaStamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
            case 6: return spaceStamp[(selectedDestination % 100 / 10 * 5) + selectedDestination % 10];
        }

        return null;
    }

    IEnumerator PrintManifest()
    {
        int sectorVal = goalDestination / 100;

        string[] message = new string[] { "--==Shipping Manifest==--\n",
                                          "\n",
                                          string.Format("Delivery Nº: {0}\n", deliveryNo.Join("")),
                                          "\n",
                                          "From: Steel Crate Games,\n",
                                          "Ottawa, Ontario, Canada\n",
                                          "\n",
                                          string.Format("To: {0},\n", GetAreaName(goalDestination)),
                                          string.Format("{0}\n", GetSectorName(sectorVal)),
                                          string.Format("Area Code: {0}\n",goalDestination),
                                          "\n",
                                          "Content Details: \n",
                                          "   - Bomb;\n",
                                          "\n",
                                          "\n",
                                          "\n",
                                          "\n",
                                          ""
                                        };

        Audio.PlaySoundAtTransform("print", transform);

        for (int i = 0; i < 20; i++)
        {
            manifestText.gameObject.transform.localPosition += new Vector3(0, 0, 0.01f);
            manifest.gameObject.transform.localPosition += new Vector3(0, 0, 0.005f);
            manifest.gameObject.transform.localScale += new Vector3(0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < message.Length; i++)
        {
            manifestText.text = message[message.Length - 1 - i] + manifestText.text;
            for (int j = 0; j < 22; j++)
            {
                manifestText.gameObject.transform.localPosition += new Vector3(0, 0, 0.01f);
                manifest.gameObject.transform.localPosition += new Vector3(0, 0, 0.005f);
                manifest.gameObject.transform.localScale += new Vector3(0, 0, 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.3f);

        int[] vertical = new int[] { 110, 122, 124, 134, 141, 142, 144,
            203, 223, 224, 230, 231, 233, 240, 241,
            311, 312, 313, 321, 330, 332, 333, 342,
            400, 432, 434, 441, 444,
            511, 520, 523, 540, 541 };

        Audio.PlaySoundAtTransform("paper", transform);
        postStamp.GetComponentInChildren<Renderer>().material = GetStamp(goalDestination);
        if (vertical.ToList().Contains(goalDestination)) postStamp.transform.Rotate(0, 90f, 0);
        postStamp.SetActive(true);
        selectableSelf.AddInteractionPunch(2f);

        yield return new WaitForSeconds(0.3f);

        stamp.SetActive(true);
        Audio.PlaySoundAtTransform("stamp", transform);
        modSelf.HandlePass();
    }

    //twitch plays handling, originally by eXish. 
    /**private bool inputIsValid(string cmd)
    {
        if (cmd.EqualsIgnoreCase("Main") || cmd.EqualsIgnoreCase("Destination") || cmd.EqualsIgnoreCase("Components"))
        {
            return true;
        }
        return false;
    }
    private bool isTypeValid(string cmd)
    {
        if (cmd.EqualsIgnoreCase("batteries") || cmd.EqualsIgnoreCase("port") || cmd.EqualsIgnoreCase("indicators") || cmd.EqualsIgnoreCase("manuals"))
        {
            return true;
        }
        return false;
    }
    private bool isNumValid(string cmd)
    {
        string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        if (nums.Contains(cmd))
        {
            return true;
        }
        return false;
    }*/
    private int getShortestPath(int start, int end, int min, int max)
    {
        int ct1 = 0, ct2 = 0;
        int temp = start;
        while (start != end)
        {
            start++;
            ct2++;
            if (min == -1 && max == -1)
            {
                if (start % 10 == 5)
                    start += 5;
                if (start % 10 == 9)
                    start -= 5;
                if (start % 100 / 10 == 5)
                    start = start / 100 * 100;
                if (start % 100 / 10 == 9)
                    start = (start / 100) * 100 + 144;
            }
            else if (start > max)
                start = min;
        }
        start = temp;
        while (start != end)
        {
            start--;
            ct1++;
            if (min == -1 && max == -1)
            {
                if (start % 10 == 5)
                    start += 5;
                if (start % 10 == 9)
                    start -= 5;
                if (start % 100 / 10 == 5)
                    start = start / 100 * 100;
                if (start % 100 / 10 == 9)
                    start = (start / 100) * 100 + 144;
            }
            else if (start < min)
                start = max;
        }
        if (ct1 < ct2)
            return 0;
        else if (ct1 > ct2)
            return 1;
        else
            return rnd.Range(0, 2);
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        while (goalDestination != selectedDestination || goalBatteries != selectedBatteries || goalIndicators != selectedIndicators || goalManuals != selectedManuals || selectedPortIdx != goalPortIdx)
        {
            if (goalDestination != selectedDestination)
            {
                if (!menus[1].activeSelf)
                {
                    if (menus[2].activeSelf)
                    {
                        componentBack.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    destinationButton.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                KMSelectable[] sectorBtns = { prevSector, nextSector };
                KMSelectable[] areaBtns = { prevArea, nextArea };
                KMSelectable btn = sectorBtns[getShortestPath(selectedDestination / 100, goalDestination / 100, 1, 6)];
                while (goalDestination / 100 != selectedDestination / 100)
                {
                    btn.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                btn = areaBtns[getShortestPath(selectedDestination, goalDestination, -1, -1)];
                while (goalDestination % 100 != selectedDestination % 100)
                {
                    btn.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                destinationBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }

            if (goalBatteries != selectedBatteries || goalIndicators != selectedIndicators || goalManuals != selectedManuals || selectedPortIdx != goalPortIdx)
            {
                if (!menus[2].activeSelf)
                {
                    if (menus[1].activeSelf)
                    {
                        destinationBack.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    componentButton.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                while (goalBatteries != selectedBatteries)
                {
                    if (goalBatteries > selectedBatteries)
                        addBattery.OnInteract();
                    else
                        subBattery.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                while (goalIndicators != selectedIndicators)
                {
                    if (goalIndicators > selectedIndicators)
                        addIndicator.OnInteract();
                    else
                        subIndicator.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                while (goalManuals != selectedManuals)
                {
                    if (goalManuals > selectedManuals)
                        addManual.OnInteract();
                    else
                        subManual.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                KMSelectable[] portBtns = { prevPort, nextPort };
                KMSelectable btn = portBtns[getShortestPath(selectedPortIdx, goalPortIdx, 0, ports.Length - 1)];
                while (selectedPortIdx != goalPortIdx)
                {
                    btn.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                componentBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        goButton.OnInteract();
        while (!stamp.activeSelf) yield return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = "Open the specified menu with \"!{0} open Main/Destination/Components\" Go back to the main menu with \"!{0} back\"\n" +
        "Set both the destination's sector and area with \"!{0} set destination <Sector Name>;<Area Name>\" (',' can be used instead of ';'.)"
        + "Set just the destination's sector or area with \"!{0} set sector/area <Sector Name>/<Area Name>\""
        + "Set the specified components with \"!{0} set component batteries/manuals/indicators #\""
        + "Cycle the options on the ports left or right with \"!{0} component cycleports left/right\" \"set\" is optional. To check on the ports, you may use \"!{0} tilt r\" to check. (This is a general TP command.) Submit the configurations with \"!{0} go/submit/send\"";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Application.isEditor)
        {
            command = command.Trim();
        }
        if (menus[3].activeSelf)
        {
            yield return "sendtochaterror {0}, Bomb Diffusal (#{1}) is printing out the receipt. No other commands can be accepted.";
            yield break;
        }
        string intereptedCommand = command.Trim().ToLower();
        if (Regex.IsMatch(command, @"^(go|submit|send)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (menus[1].activeSelf)
            {
                destinationBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else if (menus[2].activeSelf)
            {
                componentBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            goButton.OnInteract();
            if (menus[3].activeSelf)
            {
                yield return "solve";
            }
            yield break;
        }
        else if (Regex.IsMatch(command, @"^back\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (menus[1].activeSelf)
            {
                destinationBack.OnInteract();
            }
            else if (menus[2].activeSelf)
            {
                componentBack.OnInteract();
            }
            else if (menus[0].activeSelf == true)
            {
                yield return "sendtochat Has {0} gone back enough on Bomb Diffusal (#{1})?";
            }
            yield break;
        }
        else if (Regex.IsMatch(command, @"^open\s", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            intereptedCommand = intereptedCommand.Substring(5);
            Dictionary<int, string[]> menuIdIntereperations = new Dictionary<int, string[]>() {
                { 0, new string[] { "Main" } },
                { 1, new string[] { "Destination", "Dest" } },
                { 2, new string[] { "Components", "Component", "Comp" } },
            };// 0 being the main menu, 1 being the destination, 2 being the component, 3 being the receipt menu
            int destinationID = -1;

            foreach (KeyValuePair<int, string[]> curIntereptation in menuIdIntereperations)
            {
                if (curIntereptation.Value.Any(a => a.EqualsIgnoreCase(intereptedCommand)))
                {
                    destinationID = curIntereptation.Key;
                    break;
                }
            }
            if (destinationID < 0 || destinationID > 2)
            {
                yield return string.Format("sendtochaterror I cannot find the menu \"{0}\" in the directory.", intereptedCommand);
                yield break;
            }
            if (menus[destinationID].activeSelf)
            {
                yield return string.Format("sendtochaterror I am already in the menu \"{0}\" in the directory.", intereptedCommand);
                yield break;
            }
            KMSelectable[] respectiveDestinationButtons = { null, destinationButton, componentButton };
            if (menus[1].activeSelf)
            {
                yield return null;
                destinationBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else if (menus[2].activeSelf)
            {
                yield return null;
                componentBack.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            if (respectiveDestinationButtons[destinationID] != null)
            {
                yield return null;
                respectiveDestinationButtons[destinationID].OnInteract();
            }
            yield break;
        }
        else
        {
            if (Regex.IsMatch(command, @"^set\s", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                command = command.Substring(3).Trim();
                intereptedCommand = intereptedCommand.ToLowerInvariant().Replace("set", "").Trim();
            }
            if (Regex.IsMatch(command, @"^destination\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                Dictionary<string, string[]> alternativeSectorNames = new Dictionary<string, string[]>() {
                                {"USA #1",  new string[] { "USA 1", "USA#1", "USA1" } },
                                {"USA #2",  new string[] { "USA 2", "USA#2", "USA2" } },
                                {"Rest of America",  new string[] { "RestOfAmerica", "RestOf America", "Rest OfAmerica" } },
                            };
                string[] intereptedParts = intereptedCommand.ToLowerInvariant().Replace("destination","").Trim().Split(new char[] { ',', ';' });
                if (intereptedParts.Length != 2)
                {
                    yield return string.Format("sendtochaterror You provided {0} than 2 pieces of infomation for setting the destination. I expected exactly 2, the sector name and the area name.", intereptedParts.Length > 2 ? "more" : "less");
                    yield break;
                }
                string intereptedSectorName = intereptedParts[0].Trim();
                string intereptedAreaName = intereptedParts[1].Trim();
                foreach (KeyValuePair<string, string[]> altSectorNames in alternativeSectorNames)
                {
                    if (altSectorNames.Value.Any(a => a.EqualsIgnoreCase(intereptedSectorName)))
                    {
                        intereptedSectorName = altSectorNames.Key;
                        break;
                    }
                }
                bool directionR = rnd.value < 0.5f, successful = false;
                if (!menus[1].activeSelf)
                {
                    /*
                    if (menus[2].activeSelf)
                    {
                        yield return null;
                        componentBack.OnInteract();
                    }
                    yield return null;
                    destinationButton.OnInteract();*/
                    yield return string.Format("sendtochaterror I am not at the menu for setting the destination. Open that up first before using this.");
                    yield break;
                }
                for (int x = 0; x < 6 && !successful; x++)
                {
                    yield return null;
                    if (directionR)
                        nextSector.OnInteract();
                    else
                        prevSector.OnInteract();
                    if (sectorNames[selectedDestination / 100].EqualsIgnoreCase(intereptedSectorName))
                    {
                        successful = true;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                if (!successful)
                {
                    yield return string.Format("sendtochaterror I cannot find the given sector \"{0}\" in the directory.", intereptedSectorName);
                    yield break;
                }
                directionR = rnd.value < 0.5f;
                successful = false;
                for (int x = 0; x < 25 && !successful; x++)
                {
                    yield return null;
                    if (directionR)
                        nextArea.OnInteract();
                    else
                        prevArea.OnInteract();
                    if (areaNames[selectedDestination].EqualsIgnoreCase(intereptedAreaName))
                    {
                        successful = true;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                if (!successful)
                {
                    yield return string.Format("sendtochaterror I cannot find the given area \"{0}\" in the directory.", intereptedAreaName);
                    yield break;
                }

            }
            else if (Regex.IsMatch(command, @"^sector\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                Dictionary<string, string[]> alternativeSectorNames = new Dictionary<string, string[]>() {
                                {"USA #1",  new string[] { "USA 1", "USA#1", "USA1" } },
                                {"USA #2",  new string[] { "USA 2", "USA#2", "USA2" } },
                                {"Rest of America",  new string[] { "RestOfAmerica", "RestOf America", "Rest OfAmerica" } },
                            };
                string intereptedSectorName = intereptedCommand.Substring(7).Trim();

                foreach (KeyValuePair<string, string[]> altSectorNames in alternativeSectorNames)
                {
                    if (altSectorNames.Value.Any(a => a.EqualsIgnoreCase(intereptedSectorName)))
                    {
                        intereptedSectorName = altSectorNames.Key;
                        break;
                    }
                }
                bool directionR = rnd.value < 0.5f, successful = false;
                if (!menus[1].activeSelf)
                {
                    /*
                    if (menus[2].activeSelf)
                    {
                        yield return null;
                        componentBack.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return null;
                    destinationButton.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                    */
                    yield return string.Format("sendtochaterror I am not at the menu for setting the sector. Open destination up first before using this.");
                    yield break;
                }
                for (int x = 0; x < 6 && !successful; x++)
                {
                    yield return null;
                    if (directionR)
                        nextSector.OnInteract();
                    else
                        prevSector.OnInteract();
                    if (sectorNames[selectedDestination / 100].EqualsIgnoreCase(intereptedSectorName))
                    {
                        successful = true;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                if (!successful)
                {
                    yield return string.Format("sendtochaterror I cannot find the given sector \"{0}\" in the directory.", intereptedSectorName);
                    yield break;
                }

            }
            else if (Regex.IsMatch(command, @"^area\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                string intereptedAreaName = intereptedCommand.Substring(5).Trim();
                bool directionR = rnd.value < 0.5f, successful = false;
                if (!menus[1].activeSelf)
                {
                    /*
                    if (menus[2].activeSelf)
                    {
                        yield return null;
                        componentBack.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return null;
                    destinationButton.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                    */
                    yield return string.Format("sendtochaterror I am not at the menu for setting the area. Open destination up first before using this.");
                    yield break;
                }
                for (int x = 0; x < 25 && !successful; x++)
                {
                    yield return null;
                    if (directionR)
                        nextArea.OnInteract();
                    else
                        prevArea.OnInteract();
                    if (areaNames[selectedDestination].EqualsIgnoreCase(intereptedAreaName))
                    {
                        successful = true;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                if (!successful)
                {
                    yield return string.Format("sendtochaterror I cannot find the given area \"{0}\" in the directory.", intereptedAreaName);
                    yield break;
                }

            }
            else if (Regex.IsMatch(command, @"^comp(onents?)?\s*", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                intereptedCommand = intereptedCommand.Substring(intereptedCommand.StartsWith("components") ? 11 : intereptedCommand.StartsWith("component") ? 10 : 5);
                if (Regex.IsMatch(intereptedCommand, @"^(batter(y|ies)|indicators?|manuals?)\s\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    string[] parameters = intereptedCommand.Split();

                    int value;
                    if (!int.TryParse(parameters[1], out value))
                    {
                        yield return string.Format("sendtochaterror I do not know what number \"{0}\" is.", parameters[2]);
                        yield break;
                    }
                    if (value < 0 || value > 9)
                    {
                        yield return string.Format("sendtochaterror I am not setting \"{1}\" to {0}.", value, parameters[0]);
                        yield break;
                    }
                    if (!menus[2].activeSelf)
                    {
                        /*
                        if (menus[1].activeSelf)
                        {
                            yield return null;
                            destinationBack.OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        yield return null;
                        componentButton.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                        */
                        yield return string.Format("sendtochaterror I am not at the menu for setting the specified component. Open Components up first before using this.");
                        yield break;
                    }
                    switch (parameters[0])
                    {
                        case "battery":
                        case "batteries":
                            {
                                if (selectedBatteries == value)
                                {
                                    yield return string.Format("sendtochaterror The number of batteries is already set to {0}.", selectedBatteries);
                                    yield break;
                                }
                                while (selectedBatteries != value)
                                {
                                    yield return null;
                                    if (selectedBatteries < value)
                                        addBattery.OnInteract();
                                    else
                                        subBattery.OnInteract();
                                    yield return new WaitForSeconds(0.1f);
                                }
                                break;
                            }
                        case "indicator":
                        case "indicators":
                            {
                                if (selectedIndicators == value)
                                {
                                    yield return string.Format("sendtochaterror The number of indicators is already set to {0}.", selectedIndicators);
                                    yield break;
                                }
                                while (selectedIndicators != value)
                                {
                                    yield return null;
                                    if (selectedIndicators < value)
                                        addIndicator.OnInteract();
                                    else
                                        subIndicator.OnInteract();
                                    yield return new WaitForSeconds(0.1f);
                                }
                                break;
                            }
                        case "manuals":
                        case "manual":
                            {
                                if (selectedManuals == value)
                                {
                                    yield return string.Format("sendtochaterror The number of manuals is already set to {0}.", selectedManuals);
                                    yield break;
                                }
                                while (selectedManuals != value)
                                {
                                    yield return null;
                                    if (selectedManuals < value)
                                        addManual.OnInteract();
                                    else
                                        subManual.OnInteract();
                                    yield return new WaitForSeconds(0.1f);
                                }
                                break;
                            }
                        default:
                            {
                                yield return string.Format("sendtochaterror That should not have happened.");
                                yield break;
                            }
                    }
                }
                else if (Regex.IsMatch(intereptedCommand, @"^cycleports?\s", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    string intereptedDirection = intereptedCommand.Substring(intereptedCommand.StartsWith("cycleports") ? 11 : 10);
                    Dictionary<string, string[]> alternativeDirectionIntereptations = new Dictionary<string, string[]>() {
                        {"l",  new string[] { "left", } },
                        {"r",  new string[] { "right", } },
                        };
                    foreach (KeyValuePair<string, string[]> altDirName in alternativeDirectionIntereptations)
                    {
                        if (altDirName.Value.Select(a => a.ToUpper()).Contains(intereptedDirection.ToUpper()))
                        {
                            intereptedDirection = altDirName.Key;
                            break;
                        }
                    }
                    if (!alternativeDirectionIntereptations.ContainsKey(intereptedDirection))
                    {
                        yield return string.Format("sendtochaterror I can only go left or right for cycling ports in the directory. I cannot go \"{0}.\"", intereptedDirection);
                        yield break;
                    }
                    if (!menus[2].activeSelf)
                    {
                        /*
                        if (menus[1].activeSelf)
                        {
                            yield return null;
                            destinationBack.OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        yield return null;
                        componentButton.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                        */
                        yield return string.Format("sendtochaterror I am not at the menu for cycling port types. Open Components up first before using this.");
                        yield break;
                    }
                    bool directionR = intereptedDirection.EqualsIgnoreCase("r");
                    for (int x = 0; x < 13; x++)
                    {
                        yield return null;
                        if (directionR)
                            nextPort.OnInteract();
                        else
                            prevPort.OnInteract();
                        yield return "trywaitcancel 1.0 Cycling ports has been canceled viva request.";
                    }
                }
                else if (Regex.IsMatch(intereptedCommand, @"^port\s", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    string intereptedPortName = intereptedCommand.Substring(5);
                    Dictionary<string, string[]> alternativePortIntereptations = new Dictionary<string, string[]>() {
                        {"PS/2",  new string[] { "PS2" } },
                        {"Stereo RCA",  new string[] { "StereoRCA", "RCA" } },
                        {"Component Video",  new string[] { "ComponentVideo" } },
                        {"AC Power",  new string[] { "AC", "ACPower" } },
                        {"Composite Video",  new string[] { "CompositeVideo" } },
                        };
                    foreach (KeyValuePair<string, string[]> altPortName in alternativePortIntereptations)
                    {
                        if (altPortName.Value.Select(a => a.ToUpper()).Contains(intereptedPortName))
                        {
                            intereptedPortName = altPortName.Key;
                            break;
                        }
                    }
                    if (!menus[2].activeSelf)
                    {
                        /*
                        if (menus[1].activeSelf)
                        {
                            yield return null;
                            destinationBack.OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        yield return null;
                        componentButton.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                        */
                        yield return string.Format("sendtochaterror I am not at the menu for setting port types. Open Components up first before using this.");
                        yield break;
                    }
                    bool directionR = rnd.value < 0.5f, successful = false;
                    for (int x = 0; x < 14 && !successful; x++)
                    {
                        yield return null;
                        if (directionR)
                            nextPort.OnInteract();
                        else
                            prevPort.OnInteract();
                        if (ports[selectedPortIdx].EqualsIgnoreCase(intereptedPortName))
                        {
                            successful = true;
                            break;
                        }
                    }
                    if (!successful)
                    {
                        yield return string.Format("sendtochaterror I cannot seem to find the port \"{0}\" in the directory.", intereptedPortName);
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}