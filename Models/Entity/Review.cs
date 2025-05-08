using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class Review
    {
        private int reviewId;
        private int productId;
        private int buyerId;
        private int rating;
        private string? comment;
        private DateTime dateReview;
        private bool isActive;

        [Key]
        public int ReviewId
        {
            get { return reviewId; }
            set { reviewId = value; }
        }
        [ForeignKey("Product")]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        [ForeignKey("Buyer")]
        public int BuyerId
        {
            get { return buyerId; }
            set { buyerId = value; }
        }
        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        public string? Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public DateTime DateReview
        {
            get { return dateReview; }
            set { dateReview = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public Product Product { get; set; }
        public Buyer Buyer { get; set; }
    }
}