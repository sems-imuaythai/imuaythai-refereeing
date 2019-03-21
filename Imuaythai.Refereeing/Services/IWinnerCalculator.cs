using System.Collections.Generic;
using Imuaythai.Refereeing.Models;

namespace Imuaythai.Refereeing.Services
{
    public interface IWinnerCalculator
    {
        Fighter GetWinner(List<RoundPoints> fightPoints);
    }

    public class WinnerCalculator : IWinnerCalculator
    {

        public Fighter GetWinner(List<RoundPoints> fightPoints) {
            return null;
        }

    }

}