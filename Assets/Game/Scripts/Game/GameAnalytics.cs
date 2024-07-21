using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace FD.Game
{
    public class GameAnalytics : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                GiveConsent(); //Get user consent according to various legislations
            }
            catch (ConsentCheckException e)
            {
                Debug.Log(e.ToString());
            }
        }

        private static void GiveConsent()
        {
            // Call if consent has been given by the user
            AnalyticsService.Instance.StartDataCollection();
        }
    }
}
