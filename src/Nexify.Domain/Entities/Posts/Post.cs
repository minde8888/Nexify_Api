﻿using Nexify.Domain.Entities.Categories;

namespace Nexify.Domain.Entities.Posts
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> ImageNames { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public IList<BlogCategory> Categories{ get; set; }
    }
}
