using AutoMapper;
using Microsoft.Extensions.Logging;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.Interfaces;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class ProductServices : Services<Product>, IProductServices
    {
        readonly IMapper _mapper;
        public ProductServices(IUnitOfWork unitOfWork, IRepo<Product> repo, ILogger<ProductServices> logger, IMapper mapper) : base(unitOfWork, repo, logger)
        {
            _mapper = mapper;
        }
    }
}
