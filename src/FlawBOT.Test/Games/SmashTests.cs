using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    internal class SmashTests
    {
        [Test]
        [Ignore("Character data currently unavailable")]
        public void Bayonetta()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Bayonetta").Result);
        }

        [Test]
        public void Bowser()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Bowser").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void BowserJr()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Bowser Jr.").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void CaptainFalcon()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Captain Falcon").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Charizard()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Charizard").Result);
        }

        [Test]
        public void Chrom()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Chrom").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Cloud()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Cloud").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Corrin()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Corrin").Result);
        }

        [Test]
        public void Daisy()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Daisy").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DarkPit()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Dark Pit").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DarkSamus()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Dark Samus").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DiddyKong()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Diddy Kong").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DonkeyKong()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Donkey Kong").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DrMario()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Dr.Mario").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void DuckHunt()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Duck Hunt").Result);
        }

        [Test]
        public void Falco()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Falco").Result);
        }

        [Test]
        public void Fox()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Fox").Result);
        }

        [Test]
        public void Ganondorf()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ganondorf").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Greninja()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Greninja").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void IceClimbers()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ice Climbers").Result);
        }

        [Test]
        public void Ike()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ike").Result);
        }

        [Test]
        public void Incineroar()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Incineroar").Result);
        }

        [Test]
        public void Inkling()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Inkling").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Isabelle()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Isabelle").Result);
        }

        [Test]
        public void Ivysaur()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ivysaur").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Jigglypuff()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Jigglypuff").Result);
        }

        [Test]
        public void Joker()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Joker").Result);
        }

        [Test]
        public void Ken()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ken").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void KingDedede()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("King Dedede").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void KingKRool()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("King K.Rool").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Kirby()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Kirby").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Link()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Link").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void LittleMac()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Little Mac").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Lucario()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Lucario").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Lucas()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Lucas").Result);
        }

        [Test]
        public void Lucina()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Lucina").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Luigi()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Luigi").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Mario()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mario").Result);
        }

        [Test]
        public void Marth()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Marth").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MegaMan()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mega Man").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MetaKnight()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Meta Knight").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Mewtwo()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mewtwo").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MiiBrawler()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mii Brawler").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MiiSwordfighter()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mii Swordfighter").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MiiGunner()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mii Gunner").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void MrGameWatch()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Mr.Game & Watch").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Ness()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ness").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Olimar()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Olimar").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void PacMan()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Pac-Man").Result);
        }

        [Test]
        public void Palutena()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Palutena").Result);
        }

        [Test]
        public void Peach()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Peach").Result);
        }

        [Test]
        public void Pichu()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Pichu").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Pikachu()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Pikachu").Result);
        }

        [Test]
        public void PiranhaPlant()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Piranha Plant").Result);
        }

        [Test]
        public void Pit()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Pit").Result);
        }

        [Test]
        public void Richter()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Richter").Result);
        }

        [Test]
        public void Ridley()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ridley").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void ROB()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("R.O.B.").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Robin()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Robin").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void RosalinaLuma()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Rosalina & Luma").Result);
        }

        [Test]
        public void Roy()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Roy").Result);
        }

        [Test]
        public void Ryu()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Ryu").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Samus()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Samus").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Sheik()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Sheik").Result);
        }

        [Test]
        public void Shulk()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Shulk").Result);
        }

        [Test]
        public void Simon()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Simon").Result);
        }

        [Test]
        public void Snake()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Snake").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Sonic()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Sonic").Result);
        }

        [Test]
        public void Squirtle()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Squirtle").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void ToonLink()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Toon Link").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void Villager()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Villager").Result);
        }

        [Test]
        public void Wario()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Wario").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void WiiFitTrainer()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Wii Fit Trainer").Result);
        }

        [Test]
        public void Wolf()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Wolf").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void YoungLink()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Young Link").Result);
        }

        [Test]
        public void Zelda()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Zelda").Result);
        }

        [Test]
        [Ignore("Character data currently unavailable")]
        public void ZeroSuitSamus()
        {
            Assert.IsNotNull(SmashService.GetSmashCharacterAsync("Zero Suit Samus").Result);
        }
    }
}