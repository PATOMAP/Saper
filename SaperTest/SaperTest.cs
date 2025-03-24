using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Saper;
using Saper.MVVM.Model.Board;

namespace SaperTest
{
    [TestClass]
    public class SaperTest
    {
        private PoleSaper _poleSaper;
        [DataTestMethod] 
        [DataRow("admin", "password123", true)]

        public void CheckLogin(string username, string password, bool expectedResult)
        {

            bool odp = DatabaseSaper.CheckName(username, password, false);
            Assert.IsFalse(odp);

        }
        [DataTestMethod]
        [DataRow("pawel", "1234", true)]

        public void CheckTrueLogin(string username, string password, bool expectedResult)
        {

            bool odp = DatabaseSaper.CheckName(username, password, false);
             odp.Should().BeTrue();

        }
        //check Pole
        [DataTestMethod]
        [DataRow(1, 10, 10)]
        [DataRow(2, 40, 40)]
        [DataRow(3, 99, 99)]
        public void CheckInitialize(int choiceLevel,int countFalg,int countBomb)//when we initialize board
        {
            //Given 
            string flagCount, bombCount;
            _poleSaper = new PoleSaper(choiceLevel);
            //When
             bool odp =_poleSaper.WaitForClick(out flagCount,out bombCount);
            //Then
            flagCount.Should().Be( $":{countFalg}");
            bombCount.Should().Be($":{countBomb}");
            Assert.IsFalse(odp);
        }


        [DataTestMethod]
        [DataRow(1, 10, 10)]
        [DataRow(2, 40, 40)]
        [DataRow(3, 99, 99)]
        public void CheckFirstClick(int choiceLevel, int countFalg, int countBomb)//when we click first time
        {
            //Given 
            string flagCount, bombCount;
            _poleSaper = new PoleSaper(choiceLevel);
            ButtonSaper buttonSaper = _poleSaper.Buttons.FirstOrDefault();
            buttonSaper.DisplayNew = true;
            //When

            bool odp = _poleSaper.WaitForClick(out flagCount, out bombCount);
            //Then
            flagCount.Should().Be($":{countFalg}");
            bombCount.Should().Be($":{countBomb}");
            buttonSaper.DisplayNew.Should().Be(false);
            Assert.IsTrue(odp);
        }

        [DataTestMethod]
        [DataRow(1, 10)]
        [DataRow(2, 40)]
        [DataRow(3, 99)]
        public void CheckLoseGame(int choiceLevel, int countFalg)//chcek lose situation
        {
            //Given 
            string flagCount, bombCount;
            _poleSaper = new PoleSaper(choiceLevel);
            ButtonSaper buttonSaper = _poleSaper.Buttons.FirstOrDefault();
            buttonSaper.DisplayNew = true;
            bool odp = _poleSaper.WaitForClick(out flagCount, out bombCount);
            ButtonSaper buttonSaperBomb = _poleSaper.Buttons.FirstOrDefault(a=>a.InfBomb==true);

            buttonSaperBomb.DisplayNew = true;
            int odpBomb=0;
            //When

                odpBomb = _poleSaper.EventInBoard(out flagCount);


            //Then
            buttonSaper.DisplayNew.Should().Be(false);
            Assert.AreNotEqual(null, buttonSaper);
            odpBomb.Should().Be(2);
        }
        [DataTestMethod]
        [DataRow(1, 80)]
        [DataRow(2, 252)]
        [DataRow(3, 480)]
        public void CheckWinGame(int choiceLevel, int countFalg)//chcek win situation
        {
            //Given 
            string flagCount, bombCount;
            _poleSaper = new PoleSaper(choiceLevel);
            ButtonSaper buttonSaper = _poleSaper.Buttons.FirstOrDefault();
            buttonSaper.DisplayNew = true;
            bool odp = _poleSaper.WaitForClick(out flagCount, out bombCount);
            ButtonSaper buttonSaperClean=null; 
            int odpBomb = 0;
            //When
            for (int i = 0; i < countFalg; i++)
            {
                buttonSaperClean = _poleSaper.Buttons.FirstOrDefault(a => a.InfBomb == false && a.Display==false);
                buttonSaperClean.DisplayNew = true;
                odpBomb = _poleSaper.EventInBoard(out flagCount);
                if (odpBomb == 1 || buttonSaperClean==null)
                {
                    break;
                }
            }



            //Then
            buttonSaperClean.Should().NotBe(null);
            odpBomb.Should().Be(1);
        }

        [DataTestMethod]
        [DataRow(1,10, 1)]
        [DataRow(2,40, 2)]
        [DataRow(3,99, 3)]
        public void CheckFlaga(int choiceLevel,int countBomb,int flag)//check count flag
        {
            //Given 
            string flagCount, bombCount;
            _poleSaper = new PoleSaper(choiceLevel);
            ButtonSaper buttonSaper = _poleSaper.Buttons.FirstOrDefault();
            buttonSaper.DisplayNew = true;
            bool odp = _poleSaper.WaitForClick(out flagCount, out bombCount);
            ButtonSaper buttonSaperFlag = null;
            int odpBomb = 0;
            //When
            for (int i = 0; i <flag ; i++)
            {
                buttonSaperFlag = _poleSaper.Buttons.FirstOrDefault(a =>a.Display == false && a.RightClick==false);
                buttonSaperFlag.RightClick = true;
                odpBomb = _poleSaper.EventInBoard(out flagCount);

            }



            //Then
            flagCount.Should().Be($":{countBomb-flag}");
            odpBomb.Should().Be(0);
        }
    }
}
