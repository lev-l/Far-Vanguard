using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NameCreator
{
    private static Dictionary<string, string[]> _parsedNames = GetParsedNames();

    private static Dictionary<string, string[]> GetParsedNames()
    {
        Dictionary<string, string[]> parsedNames = new Dictionary<string, string[]>();

        parsedNames.Add("pre", new string[]
            {
            "Ам",
            "Ат",
            "Ак",
            "Ан",
            "Ас",
            "Бон",
            "Бер",
            "Вос",
            "Вик",
            "Ник",
            "Ток",
            "Нер"
            });
        parsedNames.Add("center", new string[]
            {
            "сте",
            "кора",
            "кои",
            "лина",
            "лу",
            "ника",
            "ниже",
            "горо",
            "анда",
            "оке",
            "инси",
            "сини"
            });
        parsedNames.Add("after", new string[]
            {
            "нас",
            "кил",
            "кар",
            "ни",
            "ку",
            "ну",
            "ки",
            "рад",
            "сен",
            "кито"
            });

        return parsedNames;
    }

    public static string CreateName()
    {
        string name = String.Empty;


        Random random = new Random();

        int nameLength = random.Next(2, 4);
        int previous = random.Next(_parsedNames["pre"].Length);
        int center = random.Next(_parsedNames["center"].Length);
        if (nameLength == 3)
        {
            int after = random.Next(_parsedNames["after"].Length);
            name = _parsedNames["pre"][previous]
                + _parsedNames["center"][center]
                + _parsedNames["after"][after];
        }
        else
        {
            name = _parsedNames["pre"][previous]
                + _parsedNames["center"][center];
        }

        return name;
    }
}