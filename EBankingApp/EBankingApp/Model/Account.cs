using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBankingApp.Model
{
    public class Account
    {
        
        public int Id { get; set; }
        public decimal Balance { get; set; }

        public Status Status { get; set; }
        public string Number { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }

        public override string ToString()
        {
            return $"Ид: {this.Id} Статус:{this.Status} Стање: {this.Balance} Број рачуна: {this.Number} " +
                $"Власник: {this.UserId} Валута: {this.CurrencyId}";
        }
    }


}
