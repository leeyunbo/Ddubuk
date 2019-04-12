using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utiliy class to manage a list of symbol strings.
/// </summary>
public class DefineSymbols {
    private List<string> symbols;

    public DefineSymbols(string symbols)
    {
        Set(symbols);
    }

    public void Set(string symbols)
    {
        this.symbols = new List<string>(symbols.Split(new string[] { ";" }, System.StringSplitOptions.None));
    }

    public bool Has(string symbol)
    {
        return (symbols.FindIndex((string obj) => obj == symbol) >= 0);
    }

    public void Add(string symbol)
    {
        if (!Has(symbol))
        {
            symbols.Add(symbol);
        }
    }

    public void Remove(string symbol)
    {
        symbols.Remove(symbol);
    }

    public string Get()
    {
        return string.Join(";", symbols.ToArray());
    }
}
