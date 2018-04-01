using System;
using System.Collections.Generic;

namespace FranklinMutualAPI.Models
{
    public partial class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Publishdate { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }
}
