using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ChallengeProgress
{
    private static readonly int[] BaseTargets = { 30, 15, 500 };
    private static readonly int[] TargetSteps = { 20, 10, 250 };
    private static readonly int[] TargetCaps = { 250, 100, 6000 };
    private static readonly int[] BaseRewards = { 300, 400, 600 };
    private static readonly int[] RewardSteps = { 100, 100, 150 };
    private static readonly string[] Keys = { "Challenge_Jumps", "Challenge_Stars", "Challenge_Height" };
    public const int Count = 3;
    private static bool Valid(int index) => index >= 0 && index < Count;
    private static string StageKey(int index) => "Challenge_Stage_" + index;

    public static int Stage(int index)
    {
        if (!Valid(index)) return 0;
        string key = StageKey(index);
        if (!PlayerPrefs.HasKey(key))
        {
            // Previously collected rewards advance once without awarding again.
            bool legacyClaimed = PlayerPrefs.GetInt("Challenge_Claimed_" + index, 0) == 1;
            PlayerPrefs.SetInt(key, legacyClaimed ? 1 : 0);
            if (legacyClaimed) { PlayerPrefs.SetInt(Keys[index], 0); PlayerPrefs.Save(); }
        }
        return Mathf.Clamp(PlayerPrefs.GetInt(key, 0), 0, 1000000);
    }
    public static int Target(int index) => Valid(index) ? Mathf.Min(BaseTargets[index] + TargetSteps[index] * Mathf.Min(Stage(index), 100), TargetCaps[index]) : 0;
    public static int Reward(int index) => Valid(index) ? BaseRewards[index] + RewardSteps[index] * Mathf.Min(Stage(index), 12) : 0;
    public static int Progress(int index)
    {
        if (!Valid(index)) return 0;
        int target = Target(index); // Migrate legacy saves before reading progress.
        return Mathf.Clamp(PlayerPrefs.GetInt(Keys[index], 0), 0, target);
    }
    public static bool Ready(int index) => Valid(index) && Progress(index) >= Target(index);
    public static void Jump() => Add(0);
    public static void Star() => Add(1);
    private static void Add(int index)
    {
        int progress = Progress(index);
        if (progress < Target(index)) PlayerPrefs.SetInt(Keys[index], progress + 1);
    }
    public static void Height(int score)
    {
        if (score > Progress(2)) PlayerPrefs.SetInt(Keys[2], Mathf.Clamp(score, 0, Target(2)));
    }
    public static bool Claim(int index)
    {
        if (!Ready(index)) return false;
        int reward = Reward(index);
        PlayerPrefs.SetInt(StageKey(index), Mathf.Min(Stage(index) + 1, 1000000));
        PlayerPrefs.SetInt(Keys[index], 0);
        PlayerPrefs.SetInt("ToplamCuzdanPuani", PlayerPrefs.GetInt("ToplamCuzdanPuani", 0) + reward);
        PlayerPrefs.Save();
        return true;
    }
}

public class ChallengesController : MonoBehaviour
{
    public GameObject panel;
    public MenuManager menu;
    public TextMeshProUGUI[] progressLabels;
    public Image[] progressFills;
    public Button[] claimButtons;
    public TextMeshProUGUI[] claimLabels;
    public TextMeshProUGUI balance;
    private TextMeshProUGUI[] descriptionLabels;
    private TextMeshProUGUI[] rewardLabels;
    private TextMeshProUGUI[] titleLabels;
    private static readonly string[] titles = { "BOUNCE BEGINNER", "STAR COLLECTOR", "FIRST PEAK" };

    private void CacheLabels()
    {
        if (descriptionLabels != null && descriptionLabels.Length == ChallengeProgress.Count && rewardLabels != null && rewardLabels.Length == ChallengeProgress.Count && titleLabels != null && titleLabels.Length == ChallengeProgress.Count) return;
        descriptionLabels = new TextMeshProUGUI[ChallengeProgress.Count];
        rewardLabels = new TextMeshProUGUI[ChallengeProgress.Count];
        titleLabels = new TextMeshProUGUI[ChallengeProgress.Count];
        for (int i = 0; i < ChallengeProgress.Count; i++)
        {
            var card = panel.transform.Find("Challenge" + i);
            if (!card) continue;
            descriptionLabels[i] = card.Find("Description")?.GetComponent<TextMeshProUGUI>();
            rewardLabels[i] = card.Find("Reward")?.GetComponent<TextMeshProUGUI>();
            titleLabels[i] = card.Find("Title")?.GetComponent<TextMeshProUGUI>();
        }
    }
    public void Open()
    {
        menu.AnaMenuPaneli.SetActive(false);
        menu.marketPaneli.SetActive(false);
        menu.ayarlarPaneli.SetActive(false);
        if (menu.liderTablosuPaneli) menu.liderTablosuPaneli.SetActive(false);
        panel.SetActive(true);
        menu.NavGorunumunuGuncelle(2);
        Refresh();
        if (SesYoneticisi.instance) SesYoneticisi.instance.ButonCal();
    }
    public void Claim(int index)
    {
        if (ChallengeProgress.Claim(index) && SesYoneticisi.instance) SesYoneticisi.instance.ButonCal();
        Refresh();
    }
    public void Refresh()
    {
        CacheLabels();
        if (balance) balance.text = PlayerPrefs.GetInt("ToplamCuzdanPuani", 0).ToString("N0");
        for (int i = 0; i < ChallengeProgress.Count; i++)
        {
            int current = ChallengeProgress.Progress(i);
            int target = ChallengeProgress.Target(i);
            int level = ChallengeProgress.Stage(i) + 1;
            if (progressLabels[i]) progressLabels[i].text = current + " / " + target;
            if (progressFills[i]) progressFills[i].fillAmount = (float)current / target;
            if (claimButtons[i]) claimButtons[i].interactable = current >= target;
            if (claimLabels[i]) claimLabels[i].text = current >= target ? "CLAIM" : "IN PROGRESS";
            if (titleLabels[i]) titleLabels[i].text = titles[i] + "  " + level;
            if (rewardLabels[i]) rewardLabels[i].text = "+" + ChallengeProgress.Reward(i) + " COINS";
            if (descriptionLabels[i]) descriptionLabels[i].text = i == 0 ? "Jump " + target + " times" : i == 1 ? "Collect " + target + " stars" : "Reach " + target + " height points";
        }
    }
}
