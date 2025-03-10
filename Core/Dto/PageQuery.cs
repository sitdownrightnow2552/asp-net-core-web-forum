﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.Dto
{
    public class PageQuery
    {
        private int _page = 1;
        private int _size = 15;
        private List<string> _sorts = new();

        public int Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }

        public int Size
        {
            get => _size;
            set => _size = value > 0 ? value : 15;
        }

        public string Sort
        {
            get => IsSorted ? _sorts[0] : "";
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Sorts = new List<string>();
                    return;
                }

                Sorts = new List<string> { value };
            }
        }

        public List<string> Sorts
        {
            get => _sorts;
            set
            {
                if (value == null || value.Count == 0)
                {
                    _sorts = new List<string>();
                    return;
                }

                _sorts = value
                    .Select(s =>
                    {
                        var direction = "asc";
                        if (s.StartsWith("-"))
                        {
                            s = s[1..];
                            direction = "desc";
                        }

                        s = char.ToUpper(s[0]) + s[1..];
                        return $"{s} {direction}";
                    })
                    .ToList();
            }
        }

        [JsonIgnore]
        public int Skip => _size * (_page - 1);

        [JsonIgnore]
        public int Take => _size;


        [JsonIgnore]
        public bool IsSorted => _sorts.Count > 0;
    }
}