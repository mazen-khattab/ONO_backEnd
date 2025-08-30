using MailKit.Net.Smtp;
using MimeKit;
using ONO.Application.DTOs.OrderDto;
using ONO.Application.Interfaces;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using ONO.Application.DTOs.UserDTOs;
using ONO.Core.Enums;
using ONO.Application.DTOs.UsersCartDTOs;
using ONO.Application.DTOs.ProductsDTOs;
using AutoMapper;

namespace ONO.Application.Services
{
    public class OrderServices : Services<Order>, IOrderServices
    {
        readonly IServices<User> _userServices;
        readonly IServices<OrderDetails> _orderDetails;
        readonly IServices<Product> _productServices;
        readonly IServices<UsersCart> _UsersCartervice;
        readonly IServices<InventoryTransaction> _inventory;
        readonly IMapper _mapper;

        public OrderServices(IUnitOfWork unitOfWork, IRepo<Order> repo, IServices<User> user, IServices<OrderDetails> orderDetails, IServices<Product> productServices, IServices<UsersCart> UsersCart, IServices<InventoryTransaction> inventory, IMapper mapper) : base(unitOfWork, repo)
        {
            _userServices = user;
            _orderDetails = orderDetails;
            _productServices = productServices;
            _UsersCartervice = UsersCart;
            _inventory = inventory;
            _mapper = mapper;
        }

        public async Task<ResponseInfo> CompleteOrder(CheckoutOrderInfoDto orderInfo, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _userServices.GetAsync(u => u.Id == userId, includes: u => u.Addresses);

                if (user is null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new()
                    {
                        IsSuccess = false,
                        Message = "no user with this id!"
                    };
                }

                int orderId = await AddToOrder("Delivered", orderInfo.TotalPrice, orderInfo.Address, user.Id);

                await AddToOrderDetails(orderId, orderInfo.CartItems);
                await UpdateProductAmounts(orderInfo.CartItems);
                await DeleteUsersCart(userId);
                await AddToInventoryTransaction(inventoryEnum.Sale, userId, orderId, orderInfo.CartItems);

                await SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var pdf = CreateOrderPdf(orderInfo);
                await SendOrderPdf(orderInfo, pdf);

                return new()
                {
                    IsSuccess = true,
                    Message = "Order Completed Successful"
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task SendOrderPdf(CheckoutOrderInfoDto orderInfo, byte[] pdfBytes)
        {
            const string Email = "mazenkhtab123@gmail.com";
            const string Password = "zjcj qoqb aeup eswf";

            MimeMessage email = new();
            email.From.Add(new MailboxAddress("ONO Store Now Order", Email));
            email.To.Add(MailboxAddress.Parse("mazenkhtab11@gmail.com"));

            email.Subject = $"New order from {orderInfo.FirstName} {orderInfo.LastName}";

            var builder = new BodyBuilder
            {
                HtmlBody = "<p>Attached is the customer order in PDF format.</p>"
            };

            builder.Attachments.Add("OrderDetails.pdf", pdfBytes, ContentType.Parse("application/pdf"));
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(Email, Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        private byte[] CreateOrderPdf(CheckoutOrderInfoDto orderInfo)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.Background(Colors.White);

                    page.Header().Row(row =>
                    {
                        var image = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", "Logo.jpg");
                        row.ConstantItem(70).Image(image);

                        row.RelativeItem().AlignCenter().Text("ONO Store - Order Summary")
                            .FontColor(Colors.Red.Medium)
                            .FontSize(22)
                            .Bold();
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().PaddingTop(10).Text("User Info:")
                             .FontSize(16)
                             .Bold()
                             .FontColor(Colors.Red.Medium);
                        col.Item().Text($"Name: {orderInfo.FirstName} {orderInfo.LastName}").FontSize(14);
                        col.Item().Text($"Email: {orderInfo.Email}").FontSize(14);
                        col.Item().Text($"Phone: {orderInfo.Phone}").FontSize(14);

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        col.Item().PaddingTop(10).Text("Address:")
                             .FontSize(16)
                             .Bold()
                             .FontColor(Colors.Red.Medium);
                        col.Item().Text($"Governorate: {orderInfo.Address.Governorate}").FontSize(14);
                        col.Item().Text($"City: {orderInfo.Address.City}").FontSize(14);
                        col.Item().Text($"Full Address: {orderInfo.Address.FullAddress}").FontSize(14);
                        col.Item().Text($"Notes: {orderInfo.Notes}").FontSize(14);

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        col.Item().PaddingTop(10).Text("Order Items:")
                            .FontSize(16)
                            .Bold()
                            .FontColor(Colors.Red.Medium);

                        foreach (var item in orderInfo.CartItems)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem(5).Text(item.Name).FontSize(14);
                                row.ConstantItem(60).AlignRight().Text($"x {item.ProductAmount}").FontSize(14);
                                row.ConstantItem(80).AlignRight().Text($"{item.price} = ").FontSize(14);
                                row.ConstantItem(80).AlignRight().Text($"{item.price * item.ProductAmount}").FontSize(14);
                            });
                        }

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        col.Item().AlignRight().Text($"Total Price: {orderInfo.TotalPrice}");
                    });

                    page.Footer().AlignCenter()
                        .Text("Thank you for shopping at ONO!")
                        .FontSize(10)
                        .FontColor(Colors.Red.Medium);
                });
            });

            return pdf.GeneratePdf();
        }

        private async Task<int> AddToOrder(string status, decimal totalPrice, UserAddressDto addressInfo, int userId)
        {
            Order order = new()
            {
                Status = status,
                TotalPrice = totalPrice,
                UserId = userId,
                Governorate = addressInfo.Governorate,
                FullAddress = addressInfo.FullAddress,
                City = addressInfo.City
            };

            await AddAsync(order);
            await SaveChangesAsync();

            return order.Id;
        }

        private async Task AddToOrderDetails(int orderId, List<UsersCartDTO> cartItems)
        {
            foreach (var item in cartItems)
            {
                OrderDetails orderDetails = new()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    UnitPrice = item.price,
                    SubPrice = item.price * item.ProductAmount,
                    Quantity = item.ProductAmount
                };

                await _orderDetails.AddAsync(orderDetails);
            }
        }

        private async Task UpdateProductAmounts(List<UsersCartDTO> cartItems)
        {
            foreach (var item in cartItems)
            {
                var product = await _productServices.GetAsync(p => p.Id == item.ProductId);

                if (product is null) { return; }

                product.Reserved -= item.ProductAmount;
                product.StockUnit -= item.ProductAmount;

                await _productServices.UpdateAsync(product);
            }
        }

        private async Task DeleteUsersCart(int userId)
        {
            var userProduct = (await _UsersCartervice.GetAllAsync(up => up.UserId == userId)).Item1;

            if (!userProduct.Any()) { return; }

            foreach (var product in userProduct)
            {
                await _UsersCartervice.DeleteAsync(product);
            }
        }

        private async Task AddToInventoryTransaction(inventoryEnum transactionType, int userId, int orderId, List<UsersCartDTO> cartItem)
        {
            foreach (var item in cartItem)
            {
                InventoryTransaction transaction = new()
                {
                    TransactionType = transactionType.ToString(),
                    Quantity = item.ProductAmount,
                    UserId = userId,
                    ProductId = item.ProductId,
                    OrderId = orderId
                };

                await _inventory.AddAsync(transaction);
            }
        }

        public async Task<ICollection<OrderHistoryDto>> OrdersHistory(int userId)
        {
            var orders = await _unitOfWork.Orders.GetOrderHistory(userId);

            var result = orders.Select(order => new OrderHistoryDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                Products = _mapper.Map<ICollection<OrderHistoryItemsDto>>(order.OrderDetails)
            }).ToList();

            return result;
        }
    }
}
