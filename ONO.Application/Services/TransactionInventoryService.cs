using ONO.Application.DTOs.InventoryTransactionDTOs;
using ONO.Application.Interfaces;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class TransactionInventoryService : Services<InventoryTransaction>, ITransactionInventoryServices
    {
        public TransactionInventoryService(IUnitOfWork unitOfWork, IRepo<InventoryTransaction> repo) : base(unitOfWork, repo) { }
    }
}
