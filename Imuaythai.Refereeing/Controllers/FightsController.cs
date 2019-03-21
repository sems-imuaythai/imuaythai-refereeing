using System.Collections.Generic;
using Imuaythai.Refereeing.Models;
using Imuaythai.Refereeing.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Imuaythai.Refereeing.Controllers
{
    [Route("api/[controller]")]
    public class FightsController : Controller 
    {
        private readonly IFightStorage _fightStorage;

        public FightsController(IFightStorage fightStorage)
        {
            _fightStorage = fightStorage;
        }

        [HttpPost]
        public ActionResult AddFights([FromBody] IEnumerable<Fight> fights)
        {
            foreach(var fight in fights)
            {
                _fightStorage.SaveAsync(fight);
            }

            return Ok();
        }

        [HttpGet]
        public string[] Index()
        {
            return new string[]
            {
                "Hello",
                "World"
            };
        }

        [HttpGet]
        [Route("fights_list")]
        public async Task<IEnumerable<PlainFight>> FightsList()
        {
            var list =  await _fightStorage.GetAllAsync('A',0);
            return list;
        }

        [HttpGet]
        [Route("mock")]
        public Fight AddMockFights()
        {
            Random rand = new Random();
            Fight fight = new Fight
            {
                Id = rand.Next(1000000, 9999999),
                BlueFighter = new Fighter() {
                    Id = Guid.NewGuid(),
                    FirstName = "Rocky",
                    Surname = "Balbao"
                },
                RedFighter = new Fighter
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Rocky",
                    Surname = "Balbao"
                },
                MainJudge = new Judge {
                    Id = Guid.NewGuid(),
                    FirstName = "MainJudge",
                    Surname = "nazwisko"
                },
                Referee = new Judge
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Referee",
                    Surname = "nazwisko"
                },
                TimeKeeper = new Judge
                {
                    Id = Guid.NewGuid(),
                    FirstName = "TimeKeeper",
                    Surname = "nazwisko"
                },
                ContestId = 0,
                Winner = null,
                StartAt = new DateTime(),
                Ring = 'A',
                KnockOut = null,
                CategoryName = "Kategoria 1",
                WeightAge = "weight age",
                RoundsConfiguration = new RoundsConfiguration() { 
                    Name = "rounds",
                    Duration = 180,
                    RoundsCount = 3,
                    BreakDuration = 15
                },
                Points = null,
                Judges = new List<Judge>(),
                CurrentRound = 1,
                State = FightState.NotStarted
            };

            fight.Judges.Add(new Judge() {
                Id = Guid.NewGuid(),
                FirstName = "Judge1",
                Surname = "nazwisko"
            });

            fight.Judges.Add(new Judge(){
                Id = Guid.NewGuid(),
                FirstName = "Judge2",
                Surname = "nazwisko"
            });

            fight.Judges.Add(new Judge(){
                Id = Guid.NewGuid(),
                FirstName = "Judge3",
                Surname = "nazwisko"
            });

            fight.Judges.Add(new Judge(){
                Id = Guid.NewGuid(),
                FirstName = "Judge4",
                Surname = "nazwisko"
            });

            fight.Judges.Add(new Judge(){
                Id = Guid.NewGuid(),
                FirstName = "Judge5",
                Surname = "nazwisko"
            });

            _fightStorage.SaveAsync(fight);

            return fight;
        }

    }
}
