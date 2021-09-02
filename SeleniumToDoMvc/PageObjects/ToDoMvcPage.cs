using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToDoMvc.PageObjects
{
    public class ToDoMvcPage
    {
        private readonly IWebDriver _driver;
        private IWebElement _doneTab => _driver.FindElement(By.CssSelector("[href='#/completed']"));
        private IWebElement _toDoTab => _driver.FindElement(By.CssSelector("[href='#/active']"));
        private IWebElement _allTab => _driver.FindElement(By.CssSelector("[href='#/']"));
        private IWebElement _newCardInput => _driver.FindElement(By.XPath("//input[@class='new-todo']"));
        private IWebElement _clearCompletedButton => _driver.FindElement(By.CssSelector(".clear-completed"));
        private IWebElement GetCard(string id) => _driver.FindElement(By.CssSelector($"[data-reactid*='{DATA_REACT_ID_PREFIX + id}']"));
        private IWebElement GetDeleteButton(IWebElement cardRoot) => cardRoot.FindElement(By.CssSelector(".destroy"));
        private const string DATA_REACT_ID_PREFIX = ".0.1.2.$";

        public ToDoMvcPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoTo()
        {
            _driver.Url = "https://todomvc.com/examples/typescript-react/#/";
        }

        public Card CreateCard(string task)
        {
            var cards = GetCardsCount();
            _newCardInput.SendKeys(task);
            _newCardInput.SendKeys(Keys.Enter);
            new DefaultWait<int>(cards)
                .Until(x => GetCardsCount() > cards);
            // not really needed currently, but nice to have, can alternatively wait for a loader to appear / disappear
            // or for an element to exist that has the text
            // but that has its problems as well
            return Cards.Last();
        }

        public void MarkCompleted(Card card)
        {
            // quick / initial way to achieve the goal, should be refactored into cleaner code asap, for example upon entering a new sprint
            GetCard(card.Id).FindElement(By.CssSelector(".toggle")).Click();
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100)); // another thing that could be done better via awaiting a loader etc, also, depending on the current page, the awaiting strategy would be different
        }

        public void DeleteCard(Card card) => GetDeleteButton(GetCard(card.Id)).Click();

        public IReadOnlyList<Card> Cards
        {
            get
            {
                return
                    _driver.FindElements(By.CssSelector(".todo-list li"))
                    .Select(x =>
                    new Card()
                    {
                        Id = x.GetAttribute("data-reactid").Replace(DATA_REACT_ID_PREFIX, string.Empty),
                        IsCompleted = x.GetAttribute("class").Contains("completed", StringComparison.OrdinalIgnoreCase),
                        Text = x.Text
                    })
                    .ToList()
                    .AsReadOnly();
            }
        }

        public int GetCardsCount()
        {
            return Cards.Count();
        }

        public void ClearAllCompleted()
        {
            _clearCompletedButton.Click();
            new DefaultWait<int>(0)
                .Until(x => GetCardsCount() == x);
        }

        public ToDoMvcPage SwitchAllCards()
        {
            _allTab.Click();
            return this;
        }

        public DoneCardsPage SwitchToDone()
        {
            _doneTab.Click();
            return new DoneCardsPage(_driver);
        }

        public ToDoCardsPage SwitchToToDo()
        {
            _toDoTab.Click();
            return new ToDoCardsPage(_driver);
        }
    }
}
