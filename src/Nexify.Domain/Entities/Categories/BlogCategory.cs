﻿using Nexify.Domain.Entities.Posts;
using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Categories
{
    public class BlogCategory
    {
        [Key]
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public IList<Post> Posts { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}