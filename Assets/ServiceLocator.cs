using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;
    public float xBounds = 8f;
    public float yBounds = 6f;
    public float InfectionRadius = 0.3f;
    public float BaseInfectionChance = 0.02f;
    public float InfectionChanceReductionPercent = 0.3f;
    public float PersonSpeed = 0.1f;

    public bool HomeOffice = false; //
    public bool CloseSchools = false; //
    public bool SelfQuarantaine = false; //
    public bool CloseRestaurants = false; //
    public bool WashYourHands = false;
    public bool MoreHospitalCapacity = false; //
    public bool CoronaTests = false; //
    public bool SocialDistancing = false; // Haben wir grade keine story zu.
    public bool StayAtHome = false; //
    public bool DebugVis = false;

    public static bool oldValues = false;
    public static bool oldHomeOffice = false; //
    public static bool oldCloseSchools = false; //
    public static bool oldSelfQuarantaine = false; //
    public static bool oldCloseRestaurants = false; //
    public static bool oldWashYourHands = false;
    public static bool oldMoreHospitalCapacity = false; //
    public static bool oldCoronaTests = false; //
    public static bool oldSocialDistancing = false; // Haben wir grade keine story zu.
    public static bool oldStayAtHome = false; //
    public static bool oldDebugVis = false;

    public Spawner Spawner;
    public PersonBuilder PersonBuilder;
    public SimulationMaster SimMaster;
    public InfectionGraph InfectionGraph;
    public WebBridge WebBridge;
    public Graveyard Graveyard;

    public float InfectionChance
    {
        get
        {
            return WashYourHands ? BaseInfectionChance * InfectionChanceReductionPercent : BaseInfectionChance;
        }
    }

    void Start()
    {
        if(oldValues)
        {
            HomeOffice = oldHomeOffice; //
            CloseSchools = oldCloseSchools; //
            SelfQuarantaine = oldSelfQuarantaine; //
            CloseRestaurants = oldCloseRestaurants; //
            WashYourHands = oldWashYourHands;
            MoreHospitalCapacity = oldMoreHospitalCapacity; //
            CoronaTests = oldCoronaTests; //
            SocialDistancing = oldSocialDistancing; // Haben wir grade keine story zu.
            StayAtHome = oldStayAtHome; //
        }
    }

    void OnDestroy()
    {
        oldValues = true;
        oldHomeOffice = HomeOffice; //
        oldCloseSchools = CloseSchools; //
        oldSelfQuarantaine = SelfQuarantaine; //
        oldCloseRestaurants = CloseRestaurants; //
        oldWashYourHands = WashYourHands;
        oldMoreHospitalCapacity = MoreHospitalCapacity; //
        oldCoronaTests = CoronaTests; //
        oldSocialDistancing = SocialDistancing; // Haben wir grade keine story zu.
        oldStayAtHome = StayAtHome; //
    }

    #region WebFunctions
    public void TOGGLE_doubleKh() { MoreHospitalCapacity = !MoreHospitalCapacity; }
    public void TOGGLE_verhaltensregeln() { WashYourHands = !WashYourHands; }
    public void TOGGLE_stayHome() { StayAtHome = !StayAtHome; }
    public void TOGGLE_closeSchools() { CloseSchools = !CloseSchools; }
    public void TOGGLE_homeoffice() { HomeOffice = !HomeOffice; }
    public void TOGGLE_coronaTests() { CoronaTests = !CoronaTests; }
    public void TOGGLE_closePublic() { CloseRestaurants = !CloseRestaurants; }
    public void TOGGLE_selfQuarantine() { SelfQuarantaine = !SelfQuarantaine; }
    public void TOGGLE_debugVis() { DebugVis = !DebugVis; }
    #endregion


    private void Awake()
    {
        Instance = this;
    }
}
