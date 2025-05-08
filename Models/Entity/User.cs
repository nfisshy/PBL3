using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PBL3.Enums;



namespace PBL3.Entity
{
    public class User
    {
        protected int id;
        protected string username;
        protected string password;
        protected string name;
        protected Gender sex;
        protected string phoneNumber;
        protected DateTime date;
        protected Roles roleName;


        [Key]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Gender Sex
        {
            get { return sex; }
            set { sex = value; }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public Roles RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }

        public PlatformWallet Wallet { get; set; }
    }
}