using FluentAssertions;
using FluentAssertions.Execution;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using SeleniumToDoMvc.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SeleniumToDoMvc
{
    public class ToDoMvcTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly ToDoMvcPage _page;
        private const string STORAGE_KEY = "react-todos";

        public ToDoMvcTests()
        {
            _driver = new ChromeDriver();
            _page = new ToDoMvcPage(_driver);
            _page.GoTo();
        }

        [Fact]
        public void Should_have_no_cards_on_initial_visit()
        {
            _page.GetCardsCount().Should().Be(0);
        }

        [Fact]
        public void Should_add_created_card()
        {
            var task = DateTime.Now.Ticks.ToString();
            var newCard = _page.CreateCard(task);
            _page.GetCardsCount().Should().Be(1);
            newCard.Text.Should().Be(task);
        }

        [Fact]
        public void Should_display_completed_cards()
        {
            var task = DateTime.Now.Ticks.ToString();
            var card = _page.CreateCard(task);
            _page.MarkCompleted(card);
            var completedPage = _page.SwitchToDone();
            using var scope = new AssertionScope();
            completedPage.GetCardsCount().Should().Be(1);
            completedPage.Cards.Last().Id.Should().Be(card.Id);
            completedPage.Cards.Last().Text.Should().Be(task);
        }

        // since the app has no db, you need to improvise
        // instead of having a saved user with data / injecting one into the db
        // browser storage is used to simulate that
        [Fact]
        public void Should_persist_created_cards()
        {
            var storage = ((RemoteWebDriver)_driver).WebStorage.LocalStorage;
            storage.SetItem(STORAGE_KEY, System.IO.File.ReadAllText("activeCard.json"));
            _driver.Navigate().Refresh();
            _page.GetCardsCount().Should().Be(1);
        }

        [Fact]
        public void Should_move_cards_to_completed()
        {
            _page.
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
