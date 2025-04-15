using System;
using System.Collections.Generic;
using Godot;

namespace Quiz
{

    public class QuizData{

        private DataChoice choice;

        private List<Question> _questions = new()
        {
                new(
                    "Як створити клас для зберігання даних про гравця у C#?",
                    new(){
                        {-1, "Використовувати тільки змінні в класах"},
                        {2, "Використовувати структури даних як List або Dictionary"},
                        {3, "Використовувати клас з властивостями та методами"},
                        {0, "Використовувати масиви"}
                    }
                ),
                new(
                    "В якому випадку правильно використовувати GetTree().ChangeSceneToFile() в Godot?",
                    new(){
                        {-2, "Для ініціалізації нового скрипта"},
                        {1, "Щоб отримати ноду на сцені за шляхом"},
                        {3, "Для зміни поточної сцени"},
                        {-1, "Для змінення шляху до сцени"}
                    }
                ),
                new(
                    "Який клас в C# використовується для збереження пар \"ключ-значення\"?",
                    new(){
                        {2, "List<T>"},
                        {-1, "Stack"},
                        {3, "Dictionary<KeyT, ValT>"},
                        {-2, "Enum"}
                    }
                ),
                new(
                    "Що робить команда GetNode<Button>(\"button\") в Godot?",
                    new(){
                        {1, "Ініціалізує кнопку на сцені"},
                        {-1, "Завантажує нову сцену"},
                        {3, "Одержує ноду, яка є кнопкою на сцені"},
                        {0, "Встановлює текст на кнопці"}
                    }
                ),
                new(
                    "Як у C# оголосити список, який містить різні типи даних (наприклад, рядки та числа)?",
                    new(){
                        {2, "Використовувати List<Object> або ArrayList"},
                        {-1, "Використовувати тільки типи string або int"},
                        {3, "Використовувати Dictionary для різних типів"},
                        {0, "Використовувати масиви з фіксованим типом"}
                    }
                )
            };

        private string _rules = 
        "[b][center]\"Хто хоче стати міліонером\"[/center][/b]" +
        "\n" +
        "\n Тематика: Кодинг" +
        "\n Робив: Лебедюк Дмитро з групи АІ-225" +
        "\n" +
        "\n[b]      Покращена версія \"Хто хоче стати міліонером\" має такі правила: [/b]" +
        "\n" +
        "\n     - Відповідь на питання обов’язково нарахує бали; " +
        "\n     - Питання мають окрему вагу по балам, тобто немає конкретно однієї правильної відповіді;" +
        "\n     - Існують звичні функції, типу \"Допомога друга\", чи \"50/50\";" +
        "\n     - Питання мають і від’ємну вагу, що може знизити ваш виграш, та навіть створити борг.";

        public QuizData(DataChoice choice){
            this.choice = choice;
        }

        public List<Question> Questions{
            get => (choice == DataChoice.quizData) ? _questions : null;
            set => _questions = (choice == DataChoice.quizData) ? value : _questions;
        }

        public string Rules{
            get => (choice == DataChoice.rules) ? _rules : null;
        }
    }

    public class Question
    {
        public string Quest;
        public Dictionary<int, string> Answers;

        public Question(string quest, Dictionary<int, string> answers){
            Quest = quest;
            Answers = answers;
        }
    }

    public enum DataChoice
    {
        rules,
        quizData
    }
}