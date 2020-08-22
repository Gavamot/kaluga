using System;

namespace App.KeywordsSearchService
{
    public readonly struct Keyword
    {
        public Keyword(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Parameter keyword is null or empty");
            if (value.Length >= 100)
                throw new ArgumentException("Parameter keyword must has length less 100");
            this.value = value;
        }

        public readonly string value;
    }
}