using MediatR;
using Microsoft.AspNetCore.Mvc; 
using DDD.OrderManagement.Dtos;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Controllers
{
    /// <summary>
    /// 订单管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="request">创建订单请求</param>
        /// <returns>创建的订单信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var command = new CreateOrderCommand(request.CustomerId, request.ShippingAddress);
                var result = await _mediator.Send(command);

                var response = ApiResponse<OrderDto>.Ok(result, "订单创建成功");
                return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建订单失败: {Message}", ex.Message);
                return BadRequest(ApiResponse<OrderDto>.Error("创建订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>订单详情</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(ApiResponse<OrderDto>.Error("订单不存在", "ORDER_NOT_FOUND"));

                return Ok(ApiResponse<OrderDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单失败: {OrderId}", id);
                return BadRequest(ApiResponse<OrderDto>.Error("获取订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 分页查询订单
        /// </summary>
        /// <param name="parameters">查询参数</param>
        /// <returns>订单列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderSummaryDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderSummaryDto>>>> GetOrders([FromQuery] OrderQueryParameters parameters)
        {
            try
            {
                parameters.Validate();
                var query = new GetOrdersQuery(parameters);
                var result = await _mediator.Send(query);

                return Ok(ApiResponse<PagedResponse<OrderSummaryDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询订单失败: {Message}", ex.Message);
                return BadRequest(ApiResponse<PagedResponse<OrderSummaryDto>>.Error("查询订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 获取客户的所有订单
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>客户订单列表</returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderSummaryDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderSummaryDto>>>> GetOrdersByCustomer(
            Guid customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var parameters = new OrderQueryParameters
                {
                    CustomerId = customerId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                parameters.Validate();

                var query = new GetOrdersQuery(parameters);
                var result = await _mediator.Send(query);

                return Ok(ApiResponse<PagedResponse<OrderSummaryDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取客户订单失败: {CustomerId}", customerId);
                return BadRequest(ApiResponse<PagedResponse<OrderSummaryDto>>.Error("获取客户订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 添加订单项
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="request">添加订单项请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/items")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> AddOrderItem(Guid id, [FromBody] AddOrderItemRequest request)
        {
            try
            {
                var command = new AddOrderItemCommand(id, request.ProductId, request.Quantity);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("商品添加成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加订单项失败: {OrderId}, {ProductId}", id, request.ProductId);
                return BadRequest(ApiResponse.Error("添加商品失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 移除订单项
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="productId">产品ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}/items/{productId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> RemoveOrderItem(Guid id, Guid productId)
        {
            try
            {
                var command = new RemoveOrderItemCommand(id, productId);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("商品移除成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除订单项失败: {OrderId}, {ProductId}", id, productId);
                return BadRequest(ApiResponse.Error("移除商品失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/confirm")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> ConfirmOrder(Guid id)
        {
            try
            {
                var command = new ConfirmOrderCommand(id);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("订单确认成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "确认订单失败: {OrderId}", id);
                return BadRequest(ApiResponse.Error("确认订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/pay")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> PayOrder(Guid id)
        {
            try
            {
                var command = new PayOrderCommand(id);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("订单支付成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付订单失败: {OrderId}", id);
                return BadRequest(ApiResponse.Error("支付订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="request">取消请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var command = new CancelOrderCommand(id, request.Reason);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("订单取消成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消订单失败: {OrderId}", id);
                return BadRequest(ApiResponse.Error("取消订单失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/ship")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> ShipOrder(Guid id)
        {
            try
            {
                var command = new ShipOrderCommand(id);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("订单发货成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订单发货失败: {OrderId}", id);
                return BadRequest(ApiResponse.Error("订单发货失败: " + ex.Message));
            }
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse>> CompleteOrder(Guid id)
        {
            try
            {
                var command = new CompleteOrderCommand(id);
                await _mediator.Send(command);

                return Ok(ApiResponse.Ok("订单完成"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "完成订单失败: {OrderId}", id);
                return BadRequest(ApiResponse.Error("完成订单失败: " + ex.Message));
            }
        }
    }

    /// <summary>
    /// 取消订单请求
    /// </summary>
    public class CancelOrderRequest
    {
        /// <summary>
        /// 取消原因
        /// </summary>
        [Required(ErrorMessage = "取消原因不能为空")]
        [StringLength(500, ErrorMessage = "取消原因不能超过500个字符")]
        public string Reason { get; set; }
    }
}