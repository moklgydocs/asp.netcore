using DDD.ECommerce.Domain.Catalog.Events;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 产品聚合根
    /// 表示电子商务系统中的商品
    /// </summary>
    public class Product : AggregateRoot<Guid>
    {
        // 值对象：产品名称
        public ProductName Name { get; private set; }
        
        // 值对象：产品描述
        public ProductDescription Description { get; private set; }
        
        // 值对象：价格
        public Money Price { get; private set; }
        
        // 库存数量
        public int StockQuantity { get; private set; }
        
        // 产品类别
        public ProductCategory Category { get; private set; }
        
        // 是否在售
        public bool IsActive { get; private set; }
        
        // 产品图片集合
        private readonly List<ProductImage> _images = new List<ProductImage>();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        // 必要的无参构造函数，供ORM使用
        protected Product() { }

        /// <summary>
        /// 创建新产品
        /// </summary>
        public Product(Guid id, ProductName name, ProductDescription description, 
            Money price, int stockQuantity, ProductCategory category)
        {
            // 参数验证
            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity must be non-negative.", nameof(stockQuantity));

            // 设置属性
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            Category = category;
            IsActive = true;
            
            // 添加领域事件
            AddDomainEvent(new ProductCreatedEvent(id, name.Value, price.Amount, price.Currency));
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        public void UpdateDetails(ProductName name, ProductDescription description, ProductCategory category)
        {
            Name = name;
            Description = description;
            Category = category;
            
            AddDomainEvent(new ProductUpdatedEvent(Id, name.Value));
        }

        /// <summary>
        /// 更新产品价格
        /// </summary>
        public void UpdatePrice(Money newPrice)
        {
            var oldPrice = Price;
            Price = newPrice;
            
            AddDomainEvent(new ProductPriceChangedEvent(
                Id, 
                Name.Value,
                oldPrice.Amount, 
                newPrice.Amount, 
                newPrice.Currency));
        }

        /// <summary>
        /// 调整库存数量
        /// </summary>
        public void AdjustStock(int quantity)
        {
            if (StockQuantity + quantity < 0)
                throw new InvalidOperationException("Cannot reduce stock below zero.");

            StockQuantity += quantity;
            
            // 如果库存归零，产品自动下架
            if (StockQuantity == 0)
                IsActive = false;
                
            AddDomainEvent(new ProductStockChangedEvent(Id, Name.Value, StockQuantity, quantity));
        }

        /// <summary>
        /// 添加产品图片
        /// </summary>
        public void AddImage(ProductImage image)
        {
            _images.Add(image);
        }

        /// <summary>
        /// 删除产品图片
        /// </summary>
        public void RemoveImage(Guid imageId)
        {
            _images.RemoveAll(i => i.Id == imageId);
        }

        /// <summary>
        /// 激活产品(上架)
        /// </summary>
        public void Activate()
        {
            if (StockQuantity <= 0)
                throw new InvalidOperationException("Cannot activate product with zero stock.");
                
            IsActive = true;
        }

        /// <summary>
        /// 停用产品(下架)
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}