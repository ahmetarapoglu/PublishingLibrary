﻿namespace BookShop.Entities
{
    public class BookVersion
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int BookCount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int LibraryRatio { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public List<Order> Orders { get; set; }

    }
}
