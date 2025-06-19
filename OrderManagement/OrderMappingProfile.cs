using DDD.OrderManagement.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using AutoMapper;
using OrderManagement.Domain.Entities;

namespace DDD.OrderManagement
{
    /// <summary>
    /// 订单映射配置
    /// </summary>
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Order -> OrderDto
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplayName, opt => opt.MapFrom(src => GetStatusDisplayName(src.Status)))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.FormattedTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.ToString()))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // OrderItem -> OrderItemDto
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.Value))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
                .ForMember(dest => dest.FormattedUnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToString()))
                .ForMember(dest => dest.FormattedTotalPrice, opt => opt.MapFrom(src => src.TotalPrice.ToString()));

            // Address -> AddressDto
            CreateMap<Address, AddressDto>();

            // Order -> OrderSummaryDto
            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => GenerateOrderNumber(src.Id.Value)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplayName, opt => opt.MapFrom(src => GetStatusDisplayName(src.Status)))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.FormattedTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.ToString()))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count))
                .ForMember(dest => dest.FormattedOrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd HH:mm")))
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.ShippingAddress.RecipientName))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress.FullAddress));
        }

        private static string GetStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Draft => "草稿",
                OrderStatus.Confirmed => "已确认",
                OrderStatus.Paid => "已支付",
                OrderStatus.Shipped => "已发货",
                OrderStatus.Completed => "已完成",
                OrderStatus.Cancelled => "已取消",
                _ => status.ToString()
            };
        }

        private static string GenerateOrderNumber(Guid orderId)
        {
            // 生成友好的订单编号，例如：ORD20240619001
            var dateStr = DateTime.Now.ToString("yyyyMMdd");
            var shortId = orderId.ToString("N")[..6].ToUpper();
            return $"ORD{dateStr}{shortId}";
        }
    }
}
