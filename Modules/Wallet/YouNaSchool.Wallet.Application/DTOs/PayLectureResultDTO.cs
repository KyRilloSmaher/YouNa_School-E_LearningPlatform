using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.DTOs
{
    public class PayLectureResultDTO
    {
        public Guid WalletId { get; set; }
        public Guid LectureId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

    }
}
