using Microsoft.AspNetCore.Mvc;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Interfaces
{
    public interface IProductServices : IServices<Product>
    {
    }
}
