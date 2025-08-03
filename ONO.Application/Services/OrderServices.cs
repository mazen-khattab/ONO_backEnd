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

namespace ONO.Application.Services
{
    public class OrderServices : Services<Order>, IOrderServices
    {
        IServices<User> _userServices;
        IServices<OrderDetails> _orderDetails;
        IServices<Product> _productServices;
        IServices<UserProducts> _userProductService;
        IServices<InventoryTransaction> _inventory;
        public OrderServices(IUnitOfWork unitOfWork, IRepo<Order> repo, IServices<User> user, IServices<OrderDetails> orderDetails, IServices<Product> productServices, IServices<UserProducts> userProducts, IServices<InventoryTransaction> inventory) : base(unitOfWork, repo)
        {
            _userServices = user;
            _orderDetails = orderDetails;
            _productServices = productServices;
            _userProductService = userProducts;
            _inventory = inventory;
        }

        public async Task<ResponseInfo> CompleteOrder(OrderInfoDto orderInfo, int userId)
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

                int orderId = await AddToOrder("Complete", orderInfo.TotalPrice, orderInfo.Address, user.Id);

                foreach (var item in orderInfo.CartItems)
                {
                    await AddToOrderDetails(orderId, item.ProductId, item.price, item.ProductAmount);
                    await UpdateProductAmounts(item.ProductId, item.ProductAmount);
                    await AddToInventoryTransaction(inventoryEnum.Sale, item.ProductAmount, userId, item.ProductId, orderId);
                }

                await DeleteUserProducts(userId);

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

        private async Task SendOrderPdf(OrderInfoDto orderInfo, byte[] pdfBytes)
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

        private byte[] CreateOrderPdf(OrderInfoDto orderInfo)
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

        private async Task AddToOrderDetails(int orderId, int productId, decimal price, int quantity)
        {
            OrderDetails orderDetails = new()
            {
                OrderId = orderId,
                ProductId = productId,
                Price = price,
                Quantity = quantity
            };

            await _orderDetails.AddAsync(orderDetails);
        }

        private async Task UpdateProductAmounts(int productId, int amount)
        {
            var product = await _productServices.GetAsync(p => p.Id == productId);

            if (product is null) { return; }

            product.Reserved -= amount;
            product.StockUnit -= amount;

            await _productServices.UpdateAsync(product);
        }

        private async Task DeleteUserProducts(int userId)
        {
            var userProduct = (await _userProductService.GetAllAsync(up => up.UserId == userId)).Item1;

            if (!userProduct.Any()) { return; }

            foreach (var product in userProduct)
            {
                product.IsCompleted = true;
            }
        }

        private async Task AddToInventoryTransaction(inventoryEnum transactionType, int quantity, int userId, int productId, int orderId)
        {
            InventoryTransaction transaction = new()
            {
                TransactionType = transactionType.ToString(),
                Quantity = quantity,
                UserId = userId,
                ProductId = productId,
                OrderId = orderId
            };

            await _inventory.AddAsync(transaction);
        }
    }
}
