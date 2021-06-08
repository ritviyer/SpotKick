using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPGController : MonoBehaviour
{
	#region DEFAULT_UNITY_CALLBACKS
	void Awake()
	{
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;

		// Activate the Google Play Games platform
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();
		LogInAuto();
		AddScoreToLeaderBorad();
	}
	#endregion

	#region BUTTON_CALLBACKS

	void LogInAuto()
	{
		PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
		{
			switch (result)
			{
				case SignInStatus.Success:
					Debug.Log("Login Sucess");
					break;
				default:
					Debug.Log("Login failed");
					break;
			}
		});
	}

	/// <summary>
	/// Shows All Available Leaderborad
	/// </summary>
	public void ShowLeaderBoard()
	{
		if (Social.localUser.authenticated)
		{
			AddScoreToLeaderBorad();
			Social.ShowLeaderboardUI(); // Show all leaderboard
			///((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_high_score); // Show current (Active) leaderboard
		}
		else
		{
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					AddScoreToLeaderBorad();
					Social.ShowLeaderboardUI();
				}
			});
		}
	}

	/// <summary>
	/// Adds Score To leader board
	/// </summary>
	public void AddScoreToLeaderBorad(long score = -1)
	{
		if (score == -1)
			score = (long)PlayerPrefs.GetInt("level", 1);
		if (Social.localUser.authenticated)
		{
			long totalStars = GetTotalStars();
            Social.ReportScore(score, GPGSIds.leaderboard_highest_level, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Update Score Success");
                }
                else
                {
                    Debug.Log("Update Score Fail");
                }
            });
			Social.ReportScore(totalStars, GPGSIds.leaderboard_total_stars, (bool success) =>
			{
				if (success)
				{
					Debug.Log("Update Score Success");
				}
				else
				{
					Debug.Log("Update Score Fail");
				}
			});
		}
	}
	long GetTotalStars()
    {
		long totalStars = 0;

		for (int i = 1; i <= PlayerPrefs.GetInt("level", 1); i++)
		{
			if(PlayerPrefs.HasKey("level" + i.ToString() + "stars"))
				totalStars+= PlayerPrefs.GetInt("level" + i.ToString() + "stars", 0);
		}

		return totalStars;
    }
	/// <summary>
	/// On Logout of your Google+ Account
	/// </summary>
	public void OnLogOut()
	{
		((PlayGamesPlatform)Social.Active).SignOut();
	}
	#endregion
}
