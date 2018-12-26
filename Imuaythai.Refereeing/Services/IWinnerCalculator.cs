using System.Collections.Generic;
using Imuaythai.Refereeing.Models;

namespace Imuaythai.Refereeing.Services
{
    public interface IWinnerCalculator
    {
        Fighter GetWinner(List<RoundPoints> fightPoints);
    }
}