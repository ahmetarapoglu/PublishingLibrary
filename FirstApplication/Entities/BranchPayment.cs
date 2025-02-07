﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BookShop.Services;

namespace BookShop.Entities
{
    public class BranchPayment : BaseEntity
    {
        public int PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }


        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
    public class BranchPaymentConfig : IEntityTypeConfiguration<BranchPayment>
    {
        public void Configure(EntityTypeBuilder<BranchPayment> builder)
        {

            //------------------------//
            // One To Many RelationShip :
            //------------------------//

            // Branch - BranchPayment
             builder.HasOne(s => s.Branch)
                    .WithMany(g => g.BranchPayments)
                    .HasForeignKey(s => s.BranchId);
        }
    }
}
