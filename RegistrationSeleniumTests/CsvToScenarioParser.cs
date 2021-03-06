﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RegistrationSeleniumTests
{
    internal class CsvToScenarioParser
    {
        private Scenario _scenario;

        public Scenario Parse(string fileName)
        {
            _scenario = new Scenario();
            var csv = System.IO.File.ReadAllLines(fileName);
            foreach (var line in csv)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var actionParameters = line.Split(',').Select(p => p?.Trim()).ToList();
                var actionType = (ActionType)Enum.Parse(typeof(ActionType), actionParameters[0], true);
                _scenario.Actions.Add(ParseActionType(actionType, actionParameters));
            }

            return _scenario;
        }

        private Action ParseActionType(ActionType actionType, IReadOnlyList<string> actionParameters)
        {
            var action = new Action
            {
                ActionType = actionType,
                Title = actionParameters[1] ?? string.Empty,
                Disabled = false,
                PreDelay = 0,
                PostDelay = 300
            };

            switch (actionType)
            {
                case ActionType.None:
                case ActionType.WaitForKey:
                case ActionType.Stop:
                case ActionType.SwitchToDefault:
                    break;

                case ActionType.Click:
                case ActionType.ClickJs:
                case ActionType.MoveTo:
                case ActionType.SwitchToFrame:
                    action.ByType = ByType.XPath;
                    action.ByValue = actionParameters[2];
                    break;

                case ActionType.ClickAt:
                    action.ByType = ByType.XPath;
                    action.ByValue = actionParameters[2];
                    action.Value = actionParameters[3];
                    break;

                case ActionType.SetValue:
                case ActionType.SelectByIndex:
                case ActionType.SelectByText:
                case ActionType.SelectByValue:
                    action.ByType = ByType.XPath;
                    action.ByValue = actionParameters[2];
                    action.Value = actionParameters[3];
                    break;

                case ActionType.NavigateToUrl:
                    action.Value = actionParameters[2];
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }

            return action;
        }
    }
}
