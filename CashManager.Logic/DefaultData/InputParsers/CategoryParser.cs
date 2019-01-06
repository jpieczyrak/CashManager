using System;
using System.Collections.Generic;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Logic.DefaultData.InputParsers
{
    public class CategoryParser
    {
        public Category[] Parse(string input)
        {
            var output = new List<Category>();
            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var map = new Dictionary<int, Category>();

            foreach (string line in lines)
            {
                int depth = SpacesCount(line);
                string name = line.TrimStart(' ', '.');
                var category = new Category(name.GenerateGuid()) { Name = name };

                if (depth == 0) map = new Dictionary<int, Category>();
                if (depth != 0)
                {
                    var parent = map[depth - 1];
                    category.Parent = parent;
                }

                map[depth] = category;
                output.Add(category);
            }

            return output.ToArray();
        }

        private int SpacesCount(string input) => input.Length - input.TrimStart(' ', '.').Length;
    }
}