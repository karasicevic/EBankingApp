using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBankingApp.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfRealisation { get; set; }

        public override string ToString()
        {
            return $"{this.DateOfRealisation} Amount: {this.Amount} From: {this.FromAccountId} To: {this.ToAccountId}";
        }
    }
}
