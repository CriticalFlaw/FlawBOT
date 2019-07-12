﻿using FlawBOT.Framework.Services;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace GamesModule
{
    internal class TeamFortressTests
    {
        [Test]
        public void LoadItemSchema()
        {
            Assert.IsTrue(TeamFortressService.UpdateTF2SchemaAsync().Result);
        }

        [Test]
        public void GetConnectLink()
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match("192.168.22.11");
            Assert.IsTrue(regex.Success);
        }

        [Test]
        public void GetTeamworkMap()
        {
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("pl_upward").Result.MapName);
        }

        [Test]
        public void GetTeamworkNews()
        {
            Assert.IsNotNull(TeamFortressService.GetNewsOverviewAsync().Result);
        }

        [Test]
        public void GetTeamworkServer()
        {
            Assert.IsNotNull(TeamFortressService.GetServersAsync("payload").Result);
        }
    }
}