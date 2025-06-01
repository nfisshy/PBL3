using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PBL3.Enums;
using System.Data;

namespace PBL3.Entity
{
    public class ReturnExchange
    {
        private int returnExchangeId;
        private int productId;
        private int orderId;

        private string reason;
        private byte[]? image;
        private DateTime requestDate;
        private DateTime responseDate;
        private int quantity; // số lượng sản phẩm yêu cầu đổi trả
        private ExchangeStatus status; // true: đã xử lý, false: chưa xử lý
        [Key]
        public int ReturnExchangeId
        {
            get { return returnExchangeId; }
            set { returnExchangeId = value; }
        }
        [ForeignKey("Product")]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        [ForeignKey("Order")]
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }
        public byte[]? Image
        {
            get { return image; }
            set { image = value; }
        }
        public DateTime RequestDate
        {
            get { return requestDate; }
            set { requestDate = value; }
        }
        public DateTime ResponseDate
        {
            get { return responseDate; }
            set { responseDate = value; }
        }
        public ExchangeStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}