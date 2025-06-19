using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 产品图片实体
    /// </summary>
    public class ProductImage : Entity<Guid>
    {
        // 图片URL
        public string Url { get; private set; }
        
        // 图片类型
        public string ContentType { get; private set; }
        
        // 是否为主图片
        public bool IsPrimary { get; private set; }
        
        // 显示顺序
        public int DisplayOrder { get; private set; }
        
        // 图片描述
        public string Description { get; private set; }

        // 必要的无参构造函数，供ORM使用
        protected ProductImage() { }

        /// <summary>
        /// 创建产品图片
        /// </summary>
        public ProductImage(Guid id, string url, string contentType, bool isPrimary, int displayOrder, string description = null)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.", nameof(url));
                
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type cannot be empty.", nameof(contentType));
                
            // 设置属性
            Id = id;
            Url = url;
            ContentType = contentType;
            IsPrimary = isPrimary;
            DisplayOrder = displayOrder;
            Description = description;
        }

        /// <summary>
        /// 更新图片信息
        /// </summary>
        public void Update(string url, string contentType, bool isPrimary, int displayOrder, string description)
        {
            if (!string.IsNullOrWhiteSpace(url))
                Url = url;
                
            if (!string.IsNullOrWhiteSpace(contentType))
                ContentType = contentType;
                
            IsPrimary = isPrimary;
            DisplayOrder = displayOrder;
            Description = description;
        }

        /// <summary>
        /// 设置为主图片
        /// </summary>
        public void SetAsPrimary()
        {
            IsPrimary = true;
        }

        /// <summary>
        /// 取消设置为主图片
        /// </summary>
        public void UnsetAsPrimary()
        {
            IsPrimary = false;
        }
    }
}